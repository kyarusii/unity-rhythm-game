using System;

namespace UniRx.Operators
{
	internal class ThrottleObservable<T> : OperatorObservableBase<T>
	{
		private readonly TimeSpan dueTime;
		private readonly IScheduler scheduler;
		private readonly IObservable<T> source;

		public ThrottleObservable(IObservable<T> source, TimeSpan dueTime, IScheduler scheduler)
			: base(scheduler == Scheduler.CurrentThread || source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			this.dueTime = dueTime;
			this.scheduler = scheduler;
		}

		protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
		{
			return new Throttle(this, observer, cancel).Run();
		}

		private class Throttle : OperatorObserverBase<T, T>
		{
			private readonly object gate = new object();
			private readonly ThrottleObservable<T> parent;
			private SerialDisposable cancelable;
			private bool hasValue = false;
			private ulong id = 0;
			private T latestValue = default;

			public Throttle(ThrottleObservable<T> parent, IObserver<T> observer, IDisposable cancel) : base(observer,
				cancel)
			{
				this.parent = parent;
			}

			public IDisposable Run()
			{
				cancelable = new SerialDisposable();
				IDisposable subscription = parent.source.Subscribe(this);

				return StableCompositeDisposable.Create(cancelable, subscription);
			}

			private void OnNext(ulong currentid)
			{
				lock (gate)
				{
					if (hasValue && id == currentid)
					{
						observer.OnNext(latestValue);
					}

					hasValue = false;
				}
			}

			public override void OnNext(T value)
			{
				ulong currentid;
				lock (gate)
				{
					hasValue = true;
					latestValue = value;
					id = unchecked(id + 1);
					currentid = id;
				}

				SingleAssignmentDisposable d = new SingleAssignmentDisposable();
				cancelable.Disposable = d;
				d.Disposable = parent.scheduler.Schedule(parent.dueTime, () => OnNext(currentid));
			}

			public override void OnError(Exception error)
			{
				cancelable.Dispose();

				lock (gate)
				{
					hasValue = false;
					id = unchecked(id + 1);
					try
					{
						observer.OnError(error);
					}
					finally
					{
						Dispose();
					}
				}
			}

			public override void OnCompleted()
			{
				cancelable.Dispose();

				lock (gate)
				{
					if (hasValue)
					{
						observer.OnNext(latestValue);
					}

					hasValue = false;
					id = unchecked(id + 1);
					try
					{
						observer.OnCompleted();
					}
					finally
					{
						Dispose();
					}
				}
			}
		}
	}
}
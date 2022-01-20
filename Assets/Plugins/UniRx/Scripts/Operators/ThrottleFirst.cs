using System;

namespace UniRx.Operators
{
	internal class ThrottleFirstObservable<T> : OperatorObservableBase<T>
	{
		private readonly TimeSpan dueTime;
		private readonly IScheduler scheduler;
		private readonly IObservable<T> source;

		public ThrottleFirstObservable(IObservable<T> source, TimeSpan dueTime, IScheduler scheduler)
			: base(scheduler == Scheduler.CurrentThread || source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			this.dueTime = dueTime;
			this.scheduler = scheduler;
		}

		protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
		{
			return new ThrottleFirst(this, observer, cancel).Run();
		}

		private class ThrottleFirst : OperatorObserverBase<T, T>
		{
			private readonly object gate = new object();
			private readonly ThrottleFirstObservable<T> parent;
			private SerialDisposable cancelable;
			private bool open = true;

			public ThrottleFirst(ThrottleFirstObservable<T> parent, IObserver<T> observer, IDisposable cancel) : base(
				observer, cancel)
			{
				this.parent = parent;
			}

			public IDisposable Run()
			{
				cancelable = new SerialDisposable();
				IDisposable subscription = parent.source.Subscribe(this);

				return StableCompositeDisposable.Create(cancelable, subscription);
			}

			private void OnNext()
			{
				lock (gate)
				{
					open = true;
				}
			}

			public override void OnNext(T value)
			{
				lock (gate)
				{
					if (!open)
					{
						return;
					}

					observer.OnNext(value);
					open = false;
				}

				SingleAssignmentDisposable d = new SingleAssignmentDisposable();
				cancelable.Disposable = d;
				d.Disposable = parent.scheduler.Schedule(parent.dueTime, OnNext);
			}

			public override void OnError(Exception error)
			{
				cancelable.Dispose();

				lock (gate)
				{
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
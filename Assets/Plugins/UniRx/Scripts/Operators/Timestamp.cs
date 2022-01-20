using System;

namespace UniRx.Operators
{
	internal class TimestampObservable<T> : OperatorObservableBase<Timestamped<T>>
	{
		private readonly IScheduler scheduler;
		private readonly IObservable<T> source;

		public TimestampObservable(IObservable<T> source, IScheduler scheduler)
			: base(scheduler == Scheduler.CurrentThread || source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			this.scheduler = scheduler;
		}

		protected override IDisposable SubscribeCore(IObserver<Timestamped<T>> observer, IDisposable cancel)
		{
			return source.Subscribe(new Timestamp(this, observer, cancel));
		}

		private class Timestamp : OperatorObserverBase<T, Timestamped<T>>
		{
			private readonly TimestampObservable<T> parent;

			public Timestamp(TimestampObservable<T> parent, IObserver<Timestamped<T>> observer, IDisposable cancel)
				: base(observer, cancel)
			{
				this.parent = parent;
			}

			public override void OnNext(T value)
			{
				observer.OnNext(new Timestamped<T>(value, parent.scheduler.Now));
			}

			public override void OnError(Exception error)
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

			public override void OnCompleted()
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
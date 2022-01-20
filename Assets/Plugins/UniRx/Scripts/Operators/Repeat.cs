using System;

namespace UniRx.Operators
{
	internal class RepeatObservable<T> : OperatorObservableBase<T>
	{
		private readonly int? repeatCount;
		private readonly IScheduler scheduler;
		private readonly T value;

		public RepeatObservable(T value, int? repeatCount, IScheduler scheduler)
			: base(scheduler == Scheduler.CurrentThread)
		{
			this.value = value;
			this.repeatCount = repeatCount;
			this.scheduler = scheduler;
		}

		protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
		{
			observer = new Repeat(observer, cancel);

			if (repeatCount == null)
			{
				return scheduler.Schedule(self =>
				{
					observer.OnNext(value);
					self();
				});
			}

			if (scheduler == Scheduler.Immediate)
			{
				int count = repeatCount.Value;
				for (int i = 0; i < count; i++)
				{
					observer.OnNext(value);
				}

				observer.OnCompleted();
				return Disposable.Empty;
			}

			int currentCount = repeatCount.Value;
			return scheduler.Schedule(self =>
			{
				if (currentCount > 0)
				{
					observer.OnNext(value);
					currentCount--;
				}

				if (currentCount == 0)
				{
					observer.OnCompleted();
					return;
				}

				self();
			});
		}

		private class Repeat : OperatorObserverBase<T, T>
		{
			public Repeat(IObserver<T> observer, IDisposable cancel)
				: base(observer, cancel) { }

			public override void OnNext(T value)
			{
				try
				{
					observer.OnNext(value);
				}
				catch
				{
					Dispose();
					throw;
				}
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
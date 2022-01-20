using System;

namespace UniRx.Operators
{
	internal class TimerObservable : OperatorObservableBase<long>
	{
		private readonly DateTimeOffset? dueTimeA;
		private readonly TimeSpan? dueTimeB;
		private readonly TimeSpan? period;
		private readonly IScheduler scheduler;

		public TimerObservable(DateTimeOffset dueTime, TimeSpan? period, IScheduler scheduler)
			: base(scheduler == Scheduler.CurrentThread)
		{
			dueTimeA = dueTime;
			this.period = period;
			this.scheduler = scheduler;
		}

		public TimerObservable(TimeSpan dueTime, TimeSpan? period, IScheduler scheduler)
			: base(scheduler == Scheduler.CurrentThread)
		{
			dueTimeB = dueTime;
			this.period = period;
			this.scheduler = scheduler;
		}

		protected override IDisposable SubscribeCore(IObserver<long> observer, IDisposable cancel)
		{
			Timer timerObserver = new Timer(observer, cancel);

			TimeSpan dueTime = dueTimeA != null
				? dueTimeA.Value - scheduler.Now
				: dueTimeB.Value;

			// one-shot
			if (period == null)
			{
				return scheduler.Schedule(Scheduler.Normalize(dueTime), () =>
				{
					timerObserver.OnNext();
					timerObserver.OnCompleted();
				});
			}

			ISchedulerPeriodic periodicScheduler = scheduler as ISchedulerPeriodic;
			if (periodicScheduler != null)
			{
				if (dueTime == period.Value)
				{
					// same(Observable.Interval), run periodic
					return periodicScheduler.SchedulePeriodic(Scheduler.Normalize(dueTime), timerObserver.OnNext);
				}

				// Schedule Once + Scheudle Periodic
				SerialDisposable disposable = new SerialDisposable();

				disposable.Disposable = scheduler.Schedule(Scheduler.Normalize(dueTime), () =>
				{
					timerObserver.OnNext(); // run first

					TimeSpan timeP = Scheduler.Normalize(period.Value);
					disposable.Disposable =
						periodicScheduler.SchedulePeriodic(timeP, timerObserver.OnNext); // run periodic
				});

				return disposable;
			}

			{
				TimeSpan timeP = Scheduler.Normalize(period.Value);

				return scheduler.Schedule(Scheduler.Normalize(dueTime), self =>
				{
					timerObserver.OnNext();
					self(timeP);
				});
			}
		}

		private class Timer : OperatorObserverBase<long, long>
		{
			private long index = 0;

			public Timer(IObserver<long> observer, IDisposable cancel)
				: base(observer, cancel) { }

			public void OnNext()
			{
				try
				{
					observer.OnNext(index++);
				}
				catch
				{
					Dispose();
					throw;
				}
			}

			public override void OnNext(long value)
			{
				// no use.
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
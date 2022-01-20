using System;

namespace UniRx.Operators
{
	internal class SampleObservable<T> : OperatorObservableBase<T>
	{
		private readonly TimeSpan interval;
		private readonly IScheduler scheduler;
		private readonly IObservable<T> source;

		public SampleObservable(IObservable<T> source, TimeSpan interval, IScheduler scheduler)
			: base(source.IsRequiredSubscribeOnCurrentThread() || scheduler == Scheduler.CurrentThread)
		{
			this.source = source;
			this.interval = interval;
			this.scheduler = scheduler;
		}

		protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
		{
			return new Sample(this, observer, cancel).Run();
		}

		private class Sample : OperatorObserverBase<T, T>
		{
			private readonly object gate = new object();
			private readonly SampleObservable<T> parent;
			private bool isCompleted = false;
			private bool isUpdated = false;
			private T latestValue = default;
			private SingleAssignmentDisposable sourceSubscription;

			public Sample(SampleObservable<T> parent, IObserver<T> observer, IDisposable cancel) : base(observer,
				cancel)
			{
				this.parent = parent;
			}

			public IDisposable Run()
			{
				sourceSubscription = new SingleAssignmentDisposable();
				sourceSubscription.Disposable = parent.source.Subscribe(this);


				IDisposable scheduling;
				ISchedulerPeriodic periodicScheduler = parent.scheduler as ISchedulerPeriodic;
				if (periodicScheduler != null)
				{
					scheduling = periodicScheduler.SchedulePeriodic(parent.interval, OnNextTick);
				}
				else
				{
					scheduling = parent.scheduler.Schedule(parent.interval, OnNextRecursive);
				}

				return StableCompositeDisposable.Create(sourceSubscription, scheduling);
			}

			private void OnNextTick()
			{
				lock (gate)
				{
					if (isUpdated)
					{
						T value = latestValue;
						isUpdated = false;
						observer.OnNext(value);
					}

					if (isCompleted)
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

			private void OnNextRecursive(Action<TimeSpan> self)
			{
				lock (gate)
				{
					if (isUpdated)
					{
						T value = latestValue;
						isUpdated = false;
						observer.OnNext(value);
					}

					if (isCompleted)
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

				self(parent.interval);
			}

			public override void OnNext(T value)
			{
				lock (gate)
				{
					latestValue = value;
					isUpdated = true;
				}
			}

			public override void OnError(Exception error)
			{
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
				lock (gate)
				{
					isCompleted = true;
					sourceSubscription.Dispose();
				}
			}
		}
	}

	internal class SampleObservable<T, T2> : OperatorObservableBase<T>
	{
		private readonly IObservable<T2> intervalSource;
		private readonly IObservable<T> source;

		public SampleObservable(IObservable<T> source, IObservable<T2> intervalSource)
			: base(source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			this.intervalSource = intervalSource;
		}

		protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
		{
			return new Sample(this, observer, cancel).Run();
		}

		private class Sample : OperatorObserverBase<T, T>
		{
			private readonly object gate = new object();
			private readonly SampleObservable<T, T2> parent;
			private bool isCompleted = false;
			private bool isUpdated = false;
			private T latestValue = default;
			private SingleAssignmentDisposable sourceSubscription;

			public Sample(
				SampleObservable<T, T2> parent, IObserver<T> observer, IDisposable cancel)
				: base(observer, cancel)
			{
				this.parent = parent;
			}

			public IDisposable Run()
			{
				sourceSubscription = new SingleAssignmentDisposable();
				sourceSubscription.Disposable = parent.source.Subscribe(this);

				IDisposable scheduling = parent.intervalSource.Subscribe(new SampleTick(this));

				return StableCompositeDisposable.Create(sourceSubscription, scheduling);
			}

			public override void OnNext(T value)
			{
				lock (gate)
				{
					latestValue = value;
					isUpdated = true;
				}
			}

			public override void OnError(Exception error)
			{
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
				lock (gate)
				{
					isCompleted = true;
					sourceSubscription.Dispose();
				}
			}

			private class SampleTick : IObserver<T2>
			{
				private readonly Sample parent;

				public SampleTick(Sample parent)
				{
					this.parent = parent;
				}

				public void OnCompleted()
				{
					lock (parent.gate)
					{
						if (parent.isUpdated)
						{
							parent.isUpdated = false;
							parent.observer.OnNext(parent.latestValue);
						}

						if (parent.isCompleted)
						{
							try
							{
								parent.observer.OnCompleted();
							}
							finally
							{
								parent.Dispose();
							}
						}
					}
				}

				public void OnError(Exception error) { }

				public void OnNext(T2 _)
				{
					lock (parent.gate)
					{
						if (parent.isUpdated)
						{
							T value = parent.latestValue;
							parent.isUpdated = false;
							parent.observer.OnNext(value);
						}

						if (parent.isCompleted)
						{
							try
							{
								parent.observer.OnCompleted();
							}
							finally
							{
								parent.Dispose();
							}
						}
					}
				}
			}
		}
	}
}
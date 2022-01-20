using System;

namespace UniRx.Operators
{
	internal class TimeoutObservable<T> : OperatorObservableBase<T>
	{
		private readonly TimeSpan? dueTime;
		private readonly DateTimeOffset? dueTimeDT;
		private readonly IScheduler scheduler;
		private readonly IObservable<T> source;

		public TimeoutObservable(IObservable<T> source, TimeSpan dueTime, IScheduler scheduler)
			: base(scheduler == Scheduler.CurrentThread || source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			this.dueTime = dueTime;
			this.scheduler = scheduler;
		}

		public TimeoutObservable(IObservable<T> source, DateTimeOffset dueTime, IScheduler scheduler)
			: base(scheduler == Scheduler.CurrentThread || source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			dueTimeDT = dueTime;
			this.scheduler = scheduler;
		}

		protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
		{
			if (dueTime != null)
			{
				return new Timeout(this, observer, cancel).Run();
			}

			return new Timeout_(this, observer, cancel).Run();
		}

		private class Timeout : OperatorObserverBase<T, T>
		{
			private readonly object gate = new object();
			private readonly TimeoutObservable<T> parent;
			private bool isTimeout = false;
			private ulong objectId = 0ul;
			private SingleAssignmentDisposable sourceSubscription;
			private SerialDisposable timerSubscription;

			public Timeout(TimeoutObservable<T> parent, IObserver<T> observer, IDisposable cancel) : base(observer,
				cancel)
			{
				this.parent = parent;
			}

			public IDisposable Run()
			{
				sourceSubscription = new SingleAssignmentDisposable();
				timerSubscription = new SerialDisposable();
				timerSubscription.Disposable = RunTimer(objectId);
				sourceSubscription.Disposable = parent.source.Subscribe(this);

				return StableCompositeDisposable.Create(timerSubscription, sourceSubscription);
			}

			private IDisposable RunTimer(ulong timerId)
			{
				return parent.scheduler.Schedule(parent.dueTime.Value, () =>
				{
					lock (gate)
					{
						if (objectId == timerId)
						{
							isTimeout = true;
						}
					}

					if (isTimeout)
					{
						try
						{
							observer.OnError(new TimeoutException());
						}
						finally
						{
							Dispose();
						}
					}
				});
			}

			public override void OnNext(T value)
			{
				ulong useObjectId;
				bool timeout;
				lock (gate)
				{
					timeout = isTimeout;
					objectId++;
					useObjectId = objectId;
				}

				if (timeout)
				{
					return;
				}

				timerSubscription.Disposable = Disposable.Empty; // cancel old timer
				observer.OnNext(value);
				timerSubscription.Disposable = RunTimer(useObjectId);
			}

			public override void OnError(Exception error)
			{
				bool timeout;
				lock (gate)
				{
					timeout = isTimeout;
					objectId++;
				}

				if (timeout)
				{
					return;
				}

				timerSubscription.Dispose();
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
				bool timeout;
				lock (gate)
				{
					timeout = isTimeout;
					objectId++;
				}

				if (timeout)
				{
					return;
				}

				timerSubscription.Dispose();
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

		private class Timeout_ : OperatorObserverBase<T, T>
		{
			private readonly object gate = new object();
			private readonly TimeoutObservable<T> parent;
			private bool isFinished = false;
			private SingleAssignmentDisposable sourceSubscription;
			private IDisposable timerSubscription;

			public Timeout_(TimeoutObservable<T> parent, IObserver<T> observer, IDisposable cancel) : base(observer,
				cancel)
			{
				this.parent = parent;
			}

			public IDisposable Run()
			{
				sourceSubscription = new SingleAssignmentDisposable();

				timerSubscription = parent.scheduler.Schedule(parent.dueTimeDT.Value, OnNext);
				sourceSubscription.Disposable = parent.source.Subscribe(this);

				return StableCompositeDisposable.Create(timerSubscription, sourceSubscription);
			}

			// in timer
			private void OnNext()
			{
				lock (gate)
				{
					if (isFinished)
					{
						return;
					}

					isFinished = true;
				}

				sourceSubscription.Dispose();
				try
				{
					observer.OnError(new TimeoutException());
				}
				finally
				{
					Dispose();
				}
			}

			public override void OnNext(T value)
			{
				lock (gate)
				{
					if (!isFinished)
					{
						observer.OnNext(value);
					}
				}
			}

			public override void OnError(Exception error)
			{
				lock (gate)
				{
					if (isFinished)
					{
						return;
					}

					isFinished = true;
					timerSubscription.Dispose();
				}

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
				lock (gate)
				{
					if (!isFinished)
					{
						isFinished = true;
						timerSubscription.Dispose();
					}

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
#if UniRxLibrary
using UnityObservable = UniRx.ObservableUnity;
#else
using UnityObservable = UniRx.Observable;
#endif
using System;

namespace UniRx.Operators
{
	internal class TimeoutFrameObservable<T> : OperatorObservableBase<T>
	{
		private readonly int frameCount;
		private readonly FrameCountType frameCountType;
		private readonly IObservable<T> source;

		public TimeoutFrameObservable(IObservable<T> source, int frameCount, FrameCountType frameCountType) : base(
			source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			this.frameCount = frameCount;
			this.frameCountType = frameCountType;
		}

		protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
		{
			return new TimeoutFrame(this, observer, cancel).Run();
		}

		private class TimeoutFrame : OperatorObserverBase<T, T>
		{
			private readonly object gate = new object();
			private readonly TimeoutFrameObservable<T> parent;
			private bool isTimeout = false;
			private ulong objectId = 0ul;
			private SingleAssignmentDisposable sourceSubscription;
			private SerialDisposable timerSubscription;

			public TimeoutFrame(TimeoutFrameObservable<T> parent, IObserver<T> observer, IDisposable cancel) : base(
				observer, cancel)
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
				return UnityObservable.TimerFrame(parent.frameCount, parent.frameCountType)
					.Subscribe(new TimeoutFrameTick(this, timerId));
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

			private class TimeoutFrameTick : IObserver<long>
			{
				private readonly TimeoutFrame parent;
				private readonly ulong timerId;

				public TimeoutFrameTick(TimeoutFrame parent, ulong timerId)
				{
					this.parent = parent;
					this.timerId = timerId;
				}

				public void OnCompleted() { }

				public void OnError(Exception error) { }

				public void OnNext(long _)
				{
					lock (parent.gate)
					{
						if (parent.objectId == timerId)
						{
							parent.isTimeout = true;
						}
					}

					if (parent.isTimeout)
					{
						try
						{
							parent.observer.OnError(new TimeoutException());
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
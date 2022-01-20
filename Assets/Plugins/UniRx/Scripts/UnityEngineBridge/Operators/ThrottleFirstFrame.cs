#if UniRxLibrary
using UnityObservable = UniRx.ObservableUnity;
#else
using UnityObservable = UniRx.Observable;
#endif
using System;

namespace UniRx.Operators
{
	internal class ThrottleFirstFrameObservable<T> : OperatorObservableBase<T>
	{
		private readonly int frameCount;
		private readonly FrameCountType frameCountType;
		private readonly IObservable<T> source;

		public ThrottleFirstFrameObservable(IObservable<T> source, int frameCount, FrameCountType frameCountType) :
			base(source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			this.frameCount = frameCount;
			this.frameCountType = frameCountType;
		}

		protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
		{
			return new ThrottleFirstFrame(this, observer, cancel).Run();
		}

		private class ThrottleFirstFrame : OperatorObserverBase<T, T>
		{
			private readonly object gate = new object();
			private readonly ThrottleFirstFrameObservable<T> parent;
			private SerialDisposable cancelable;
			private bool open = true;

			private ThrottleFirstFrameTick tick;

			public ThrottleFirstFrame(ThrottleFirstFrameObservable<T> parent, IObserver<T> observer, IDisposable cancel)
				: base(observer, cancel)
			{
				this.parent = parent;
			}

			public IDisposable Run()
			{
				tick = new ThrottleFirstFrameTick(this);
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
				d.Disposable = UnityObservable.TimerFrame(parent.frameCount, parent.frameCountType)
					.Subscribe(tick);
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

			// immutable, can share.
			private class ThrottleFirstFrameTick : IObserver<long>
			{
				private readonly ThrottleFirstFrame parent;

				public ThrottleFirstFrameTick(ThrottleFirstFrame parent)
				{
					this.parent = parent;
				}

				public void OnCompleted() { }

				public void OnError(Exception error) { }

				public void OnNext(long _)
				{
					lock (parent.gate)
					{
						parent.open = true;
					}
				}
			}
		}
	}
}
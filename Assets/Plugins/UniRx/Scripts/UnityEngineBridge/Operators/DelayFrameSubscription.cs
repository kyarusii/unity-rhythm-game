#if UniRxLibrary
using UnityObservable = UniRx.ObservableUnity;
#else
using UnityObservable = UniRx.Observable;
#endif
using System;

namespace UniRx.Operators
{
	internal class DelayFrameSubscriptionObservable<T> : OperatorObservableBase<T>
	{
		private readonly int frameCount;
		private readonly FrameCountType frameCountType;
		private readonly IObservable<T> source;

		public DelayFrameSubscriptionObservable(IObservable<T> source, int frameCount, FrameCountType frameCountType)
			: base(source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			this.frameCount = frameCount;
			this.frameCountType = frameCountType;
		}

		protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
		{
			MultipleAssignmentDisposable d = new MultipleAssignmentDisposable();
			d.Disposable = UnityObservable.TimerFrame(frameCount, frameCountType)
				.SubscribeWithState3(observer, d, source, (_, o, disp, s) => { disp.Disposable = s.Subscribe(o); });

			return d;
		}
	}
}
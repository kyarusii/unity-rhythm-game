﻿using System;
using UnityEngine;

namespace UniRx.Operators
{
	internal class FrameTimeIntervalObservable<T> : OperatorObservableBase<TimeInterval<T>>
	{
		private readonly bool ignoreTimeScale;
		private readonly IObservable<T> source;

		public FrameTimeIntervalObservable(IObservable<T> source, bool ignoreTimeScale)
			: base(source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			this.ignoreTimeScale = ignoreTimeScale;
		}

		protected override IDisposable SubscribeCore(IObserver<TimeInterval<T>> observer, IDisposable cancel)
		{
			return source.Subscribe(new FrameTimeInterval(this, observer, cancel));
		}

		private class FrameTimeInterval : OperatorObserverBase<T, TimeInterval<T>>
		{
			private readonly FrameTimeIntervalObservable<T> parent;
			private float lastTime;

			public FrameTimeInterval(FrameTimeIntervalObservable<T> parent, IObserver<TimeInterval<T>> observer,
				IDisposable cancel)
				: base(observer, cancel)
			{
				this.parent = parent;
				lastTime = parent.ignoreTimeScale
					? Time.unscaledTime
					: Time.time;
			}

			public override void OnNext(T value)
			{
				float now = parent.ignoreTimeScale
					? Time.unscaledTime
					: Time.time;
				float span = now - lastTime;
				lastTime = now;

				observer.OnNext(new TimeInterval<T>(value, TimeSpan.FromSeconds(span)));
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
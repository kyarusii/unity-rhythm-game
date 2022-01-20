﻿using System;
using UnityEngine; // require keep for Windows Universal App

namespace UniRx.Triggers
{
	[DisallowMultipleComponent]
	public class ObservableVisibleTrigger : ObservableTriggerBase
	{
		private Subject<Unit> onBecameInvisible;

		private Subject<Unit> onBecameVisible;

		/// <summary>OnBecameInvisible is called when the renderer is no longer visible by any camera.</summary>
		private void OnBecameInvisible()
		{
			if (onBecameInvisible != null)
			{
				onBecameInvisible.OnNext(Unit.Default);
			}
		}

		/// <summary>OnBecameVisible is called when the renderer became visible by any camera.</summary>
		private void OnBecameVisible()
		{
			if (onBecameVisible != null)
			{
				onBecameVisible.OnNext(Unit.Default);
			}
		}

		/// <summary>OnBecameInvisible is called when the renderer is no longer visible by any camera.</summary>
		public IObservable<Unit> OnBecameInvisibleAsObservable()
		{
			return onBecameInvisible ?? (onBecameInvisible = new Subject<Unit>());
		}

		/// <summary>OnBecameVisible is called when the renderer became visible by any camera.</summary>
		public IObservable<Unit> OnBecameVisibleAsObservable()
		{
			return onBecameVisible ?? (onBecameVisible = new Subject<Unit>());
		}

		protected override void RaiseOnCompletedOnDestroy()
		{
			if (onBecameInvisible != null)
			{
				onBecameInvisible.OnCompleted();
			}

			if (onBecameVisible != null)
			{
				onBecameVisible.OnCompleted();
			}
		}
	}
}
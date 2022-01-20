﻿using System;
using UnityEngine; // require keep for Windows Universal App

namespace UniRx.Triggers
{
	[DisallowMultipleComponent]
	public class ObservableLateUpdateTrigger : ObservableTriggerBase
	{
		private Subject<Unit> lateUpdate;

		/// <summary>LateUpdate is called every frame, if the Behaviour is enabled.</summary>
		private void LateUpdate()
		{
			if (lateUpdate != null)
			{
				lateUpdate.OnNext(Unit.Default);
			}
		}

		/// <summary>LateUpdate is called every frame, if the Behaviour is enabled.</summary>
		public IObservable<Unit> LateUpdateAsObservable()
		{
			return lateUpdate ?? (lateUpdate = new Subject<Unit>());
		}

		protected override void RaiseOnCompletedOnDestroy()
		{
			if (lateUpdate != null)
			{
				lateUpdate.OnCompleted();
			}
		}
	}
}
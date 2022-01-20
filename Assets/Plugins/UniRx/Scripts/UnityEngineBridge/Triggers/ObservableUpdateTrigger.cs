﻿using System;
using UnityEngine; // require keep for Windows Universal App

namespace UniRx.Triggers
{
	[DisallowMultipleComponent]
	public class ObservableUpdateTrigger : ObservableTriggerBase
	{
		private Subject<Unit> update;

		/// <summary>Update is called every frame, if the MonoBehaviour is enabled.</summary>
		private void Update()
		{
			if (update != null)
			{
				update.OnNext(Unit.Default);
			}
		}

		/// <summary>Update is called every frame, if the MonoBehaviour is enabled.</summary>
		public IObservable<Unit> UpdateAsObservable()
		{
			return update ?? (update = new Subject<Unit>());
		}

		protected override void RaiseOnCompletedOnDestroy()
		{
			if (update != null)
			{
				update.OnCompleted();
			}
		}
	}
}
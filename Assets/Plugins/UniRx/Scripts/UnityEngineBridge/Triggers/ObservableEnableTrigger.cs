﻿using System;
using UnityEngine; // require keep for Windows Universal App

namespace UniRx.Triggers
{
	[DisallowMultipleComponent]
	public class ObservableEnableTrigger : ObservableTriggerBase
	{
		private Subject<Unit> onDisable;
		private Subject<Unit> onEnable;

		/// <summary>This function is called when the object becomes enabled and active.</summary>
		private void OnEnable()
		{
			if (onEnable != null)
			{
				onEnable.OnNext(Unit.Default);
			}
		}

		/// <summary>This function is called when the behaviour becomes disabled () or inactive.</summary>
		private void OnDisable()
		{
			if (onDisable != null)
			{
				onDisable.OnNext(Unit.Default);
			}
		}

		/// <summary>This function is called when the object becomes enabled and active.</summary>
		public IObservable<Unit> OnEnableAsObservable()
		{
			return onEnable ?? (onEnable = new Subject<Unit>());
		}

		/// <summary>This function is called when the behaviour becomes disabled () or inactive.</summary>
		public IObservable<Unit> OnDisableAsObservable()
		{
			return onDisable ?? (onDisable = new Subject<Unit>());
		}

		protected override void RaiseOnCompletedOnDestroy()
		{
			if (onEnable != null)
			{
				onEnable.OnCompleted();
			}

			if (onDisable != null)
			{
				onDisable.OnCompleted();
			}
		}
	}
}
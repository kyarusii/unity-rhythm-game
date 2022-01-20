﻿using System;
using UnityEngine; // require keep for Windows Universal App

namespace UniRx.Triggers
{
	public abstract class ObservableTriggerBase : MonoBehaviour
	{
		private Subject<Unit> awake;
		private bool calledAwake = false;


		private bool calledDestroy = false;

		private bool calledStart = false;
		private Subject<Unit> onDestroy;
		private Subject<Unit> start;

		/// <summary>Awake is called when the script instance is being loaded.</summary>
		private void Awake()
		{
			calledAwake = true;
			if (awake != null)
			{
				awake.OnNext(Unit.Default);
				awake.OnCompleted();
			}
		}

        /// <summary>
        ///     Start is called on the frame when a script is enabled just before any of the Update methods is called the
        ///     first time.
        /// </summary>
        private void Start()
		{
			calledStart = true;
			if (start != null)
			{
				start.OnNext(Unit.Default);
				start.OnCompleted();
			}
		}

		/// <summary>This function is called when the MonoBehaviour will be destroyed.</summary>
		private void OnDestroy()
		{
			calledDestroy = true;
			if (onDestroy != null)
			{
				onDestroy.OnNext(Unit.Default);
				onDestroy.OnCompleted();
			}

			RaiseOnCompletedOnDestroy();
		}

		/// <summary>Awake is called when the script instance is being loaded.</summary>
		public IObservable<Unit> AwakeAsObservable()
		{
			if (calledAwake)
			{
				return Observable.Return(Unit.Default);
			}

			return awake ?? (awake = new Subject<Unit>());
		}

        /// <summary>
        ///     Start is called on the frame when a script is enabled just before any of the Update methods is called the
        ///     first time.
        /// </summary>
        public IObservable<Unit> StartAsObservable()
		{
			if (calledStart)
			{
				return Observable.Return(Unit.Default);
			}

			return start ?? (start = new Subject<Unit>());
		}

		/// <summary>This function is called when the MonoBehaviour will be destroyed.</summary>
		public IObservable<Unit> OnDestroyAsObservable()
		{
			if (this == null)
			{
				return Observable.Return(Unit.Default);
			}

			if (calledDestroy)
			{
				return Observable.Return(Unit.Default);
			}

			return onDestroy ?? (onDestroy = new Subject<Unit>());
		}

		protected abstract void RaiseOnCompletedOnDestroy();
	}
}
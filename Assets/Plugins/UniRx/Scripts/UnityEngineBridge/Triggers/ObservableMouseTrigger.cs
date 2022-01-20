﻿#if !(UNITY_IPHONE || UNITY_ANDROID || UNITY_METRO)

using System;
using UnityEngine; // require keep for Windows Universal App

namespace UniRx.Triggers
{
	[DisallowMultipleComponent]
	public class ObservableMouseTrigger : ObservableTriggerBase
	{
		private Subject<Unit> onMouseDown;

		private Subject<Unit> onMouseDrag;

		private Subject<Unit> onMouseEnter;

		private Subject<Unit> onMouseExit;

		private Subject<Unit> onMouseOver;

		private Subject<Unit> onMouseUp;

		private Subject<Unit> onMouseUpAsButton;

		/// <summary>OnMouseDown is called when the user has pressed the mouse button while over the GUIElement or Collider.</summary>
		private void OnMouseDown()
		{
			if (onMouseDown != null)
			{
				onMouseDown.OnNext(Unit.Default);
			}
		}

        /// <summary>
        ///     OnMouseDrag is called when the user has clicked on a GUIElement or Collider and is still holding down the
        ///     mouse.
        /// </summary>
        private void OnMouseDrag()
		{
			if (onMouseDrag != null)
			{
				onMouseDrag.OnNext(Unit.Default);
			}
		}

		/// <summary>OnMouseEnter is called when the mouse entered the GUIElement or Collider.</summary>
		private void OnMouseEnter()
		{
			if (onMouseEnter != null)
			{
				onMouseEnter.OnNext(Unit.Default);
			}
		}

		/// <summary>OnMouseExit is called when the mouse is not any longer over the GUIElement or Collider.</summary>
		private void OnMouseExit()
		{
			if (onMouseExit != null)
			{
				onMouseExit.OnNext(Unit.Default);
			}
		}

		/// <summary>OnMouseOver is called every frame while the mouse is over the GUIElement or Collider.</summary>
		private void OnMouseOver()
		{
			if (onMouseOver != null)
			{
				onMouseOver.OnNext(Unit.Default);
			}
		}

		/// <summary>OnMouseUp is called when the user has released the mouse button.</summary>
		private void OnMouseUp()
		{
			if (onMouseUp != null)
			{
				onMouseUp.OnNext(Unit.Default);
			}
		}

        /// <summary>
        ///     OnMouseUpAsButton is only called when the mouse is released over the same GUIElement or Collider as it was
        ///     pressed.
        /// </summary>
        private void OnMouseUpAsButton()
		{
			if (onMouseUpAsButton != null)
			{
				onMouseUpAsButton.OnNext(Unit.Default);
			}
		}

		/// <summary>OnMouseDown is called when the user has pressed the mouse button while over the GUIElement or Collider.</summary>
		public IObservable<Unit> OnMouseDownAsObservable()
		{
			return onMouseDown ?? (onMouseDown = new Subject<Unit>());
		}

        /// <summary>
        ///     OnMouseDrag is called when the user has clicked on a GUIElement or Collider and is still holding down the
        ///     mouse.
        /// </summary>
        public IObservable<Unit> OnMouseDragAsObservable()
		{
			return onMouseDrag ?? (onMouseDrag = new Subject<Unit>());
		}

		/// <summary>OnMouseEnter is called when the mouse entered the GUIElement or Collider.</summary>
		public IObservable<Unit> OnMouseEnterAsObservable()
		{
			return onMouseEnter ?? (onMouseEnter = new Subject<Unit>());
		}

		/// <summary>OnMouseExit is called when the mouse is not any longer over the GUIElement or Collider.</summary>
		public IObservable<Unit> OnMouseExitAsObservable()
		{
			return onMouseExit ?? (onMouseExit = new Subject<Unit>());
		}

		/// <summary>OnMouseOver is called every frame while the mouse is over the GUIElement or Collider.</summary>
		public IObservable<Unit> OnMouseOverAsObservable()
		{
			return onMouseOver ?? (onMouseOver = new Subject<Unit>());
		}

		/// <summary>OnMouseUp is called when the user has released the mouse button.</summary>
		public IObservable<Unit> OnMouseUpAsObservable()
		{
			return onMouseUp ?? (onMouseUp = new Subject<Unit>());
		}

        /// <summary>
        ///     OnMouseUpAsButton is only called when the mouse is released over the same GUIElement or Collider as it was
        ///     pressed.
        /// </summary>
        public IObservable<Unit> OnMouseUpAsButtonAsObservable()
		{
			return onMouseUpAsButton ?? (onMouseUpAsButton = new Subject<Unit>());
		}

		protected override void RaiseOnCompletedOnDestroy()
		{
			if (onMouseDown != null)
			{
				onMouseDown.OnCompleted();
			}

			if (onMouseDrag != null)
			{
				onMouseDrag.OnCompleted();
			}

			if (onMouseEnter != null)
			{
				onMouseEnter.OnCompleted();
			}

			if (onMouseExit != null)
			{
				onMouseExit.OnCompleted();
			}

			if (onMouseOver != null)
			{
				onMouseOver.OnCompleted();
			}

			if (onMouseUp != null)
			{
				onMouseUp.OnCompleted();
			}

			if (onMouseUpAsButton != null)
			{
				onMouseUpAsButton.OnCompleted();
			}
		}
	}
}

#endif
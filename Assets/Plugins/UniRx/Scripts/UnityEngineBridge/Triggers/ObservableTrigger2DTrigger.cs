using System;
using UnityEngine; // require keep for Windows Universal App

namespace UniRx.Triggers
{
	[DisallowMultipleComponent]
	public class ObservableTrigger2DTrigger : ObservableTriggerBase
	{
		private Subject<Collider2D> onTriggerEnter2D;

		private Subject<Collider2D> onTriggerExit2D;

		private Subject<Collider2D> onTriggerStay2D;

		/// <summary>Sent when another object enters a trigger collider attached to this object (2D physics only).</summary>
		private void OnTriggerEnter2D(Collider2D other)
		{
			if (onTriggerEnter2D != null)
			{
				onTriggerEnter2D.OnNext(other);
			}
		}

		/// <summary>Sent when another object leaves a trigger collider attached to this object (2D physics only).</summary>
		private void OnTriggerExit2D(Collider2D other)
		{
			if (onTriggerExit2D != null)
			{
				onTriggerExit2D.OnNext(other);
			}
		}

		/// <summary>Sent each frame where another object is within a trigger collider attached to this object (2D physics only).</summary>
		private void OnTriggerStay2D(Collider2D other)
		{
			if (onTriggerStay2D != null)
			{
				onTriggerStay2D.OnNext(other);
			}
		}

		/// <summary>Sent when another object enters a trigger collider attached to this object (2D physics only).</summary>
		public IObservable<Collider2D> OnTriggerEnter2DAsObservable()
		{
			return onTriggerEnter2D ?? (onTriggerEnter2D = new Subject<Collider2D>());
		}

		/// <summary>Sent when another object leaves a trigger collider attached to this object (2D physics only).</summary>
		public IObservable<Collider2D> OnTriggerExit2DAsObservable()
		{
			return onTriggerExit2D ?? (onTriggerExit2D = new Subject<Collider2D>());
		}

		/// <summary>Sent each frame where another object is within a trigger collider attached to this object (2D physics only).</summary>
		public IObservable<Collider2D> OnTriggerStay2DAsObservable()
		{
			return onTriggerStay2D ?? (onTriggerStay2D = new Subject<Collider2D>());
		}

		protected override void RaiseOnCompletedOnDestroy()
		{
			if (onTriggerEnter2D != null)
			{
				onTriggerEnter2D.OnCompleted();
			}

			if (onTriggerExit2D != null)
			{
				onTriggerExit2D.OnCompleted();
			}

			if (onTriggerStay2D != null)
			{
				onTriggerStay2D.OnCompleted();
			}
		}
	}
}
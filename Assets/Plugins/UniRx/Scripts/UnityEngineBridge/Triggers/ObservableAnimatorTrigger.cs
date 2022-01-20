using System;
using UnityEngine; // require keep for Windows Universal App

namespace UniRx.Triggers
{
	[DisallowMultipleComponent]
	public class ObservableAnimatorTrigger : ObservableTriggerBase
	{
		private Subject<int> onAnimatorIK;

		private Subject<Unit> onAnimatorMove;

		/// <summary>Callback for setting up animation IK (inverse kinematics).</summary>
		private void OnAnimatorIK(int layerIndex)
		{
			if (onAnimatorIK != null)
			{
				onAnimatorIK.OnNext(layerIndex);
			}
		}

		/// <summary>Callback for processing animation movements for modifying root motion.</summary>
		private void OnAnimatorMove()
		{
			if (onAnimatorMove != null)
			{
				onAnimatorMove.OnNext(Unit.Default);
			}
		}

		/// <summary>Callback for setting up animation IK (inverse kinematics).</summary>
		public IObservable<int> OnAnimatorIKAsObservable()
		{
			return onAnimatorIK ?? (onAnimatorIK = new Subject<int>());
		}

		/// <summary>Callback for processing animation movements for modifying root motion.</summary>
		public IObservable<Unit> OnAnimatorMoveAsObservable()
		{
			return onAnimatorMove ?? (onAnimatorMove = new Subject<Unit>());
		}

		protected override void RaiseOnCompletedOnDestroy()
		{
			if (onAnimatorIK != null)
			{
				onAnimatorIK.OnCompleted();
			}

			if (onAnimatorMove != null)
			{
				onAnimatorMove.OnCompleted();
			}
		}
	}
}
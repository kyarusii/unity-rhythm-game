using System;
using UnityEngine; // require keep for Windows Universal App

namespace UniRx.Triggers
{
	[DisallowMultipleComponent]
	public class ObservableJointTrigger : ObservableTriggerBase
	{
		private Subject<float> onJointBreak;


		private Subject<Joint2D> onJointBreak2D;

		private void OnJointBreak(float breakForce)
		{
			if (onJointBreak != null)
			{
				onJointBreak.OnNext(breakForce);
			}
		}

		private void OnJointBreak2D(Joint2D brokenJoint)
		{
			if (onJointBreak2D != null)
			{
				onJointBreak2D.OnNext(brokenJoint);
			}
		}

		public IObservable<float> OnJointBreakAsObservable()
		{
			return onJointBreak ?? (onJointBreak = new Subject<float>());
		}

		public IObservable<Joint2D> OnJointBreak2DAsObservable()
		{
			return onJointBreak2D ?? (onJointBreak2D = new Subject<Joint2D>());
		}


		protected override void RaiseOnCompletedOnDestroy()
		{
			if (onJointBreak != null)
			{
				onJointBreak.OnCompleted();
			}

			if (onJointBreak2D != null)
			{
				onJointBreak2D.OnCompleted();
			}
		}
	}
}
﻿using System;
using UnityEngine; // require keep for Windows Universal App

namespace UniRx.Triggers
{
	[DisallowMultipleComponent]
	public class ObservableParticleTrigger : ObservableTriggerBase
	{
		private Subject<GameObject> onParticleCollision;
#if UNITY_5_4_OR_NEWER
		private Subject<Unit> onParticleTrigger;
#endif

		/// <summary>OnParticleCollision is called when a particle hits a collider.</summary>
		private void OnParticleCollision(GameObject other)
		{
			if (onParticleCollision != null)
			{
				onParticleCollision.OnNext(other);
			}
		}

		/// <summary>OnParticleCollision is called when a particle hits a collider.</summary>
		public IObservable<GameObject> OnParticleCollisionAsObservable()
		{
			return onParticleCollision ?? (onParticleCollision = new Subject<GameObject>());
		}

		protected override void RaiseOnCompletedOnDestroy()
		{
			if (onParticleCollision != null)
			{
				onParticleCollision.OnCompleted();
			}
#if UNITY_5_4_OR_NEWER
			if (onParticleTrigger != null)
			{
				onParticleTrigger.OnCompleted();
			}
#endif
		}

#if UNITY_5_4_OR_NEWER

		/// <summary>OnParticleTrigger is called when any particles in a particle system meet the conditions in the trigger module.</summary>
		private void OnParticleTrigger()
		{
			if (onParticleTrigger != null)
			{
				onParticleTrigger.OnNext(Unit.Default);
			}
		}

		/// <summary>OnParticleTrigger is called when any particles in a particle system meet the conditions in the trigger module.</summary>
		public IObservable<Unit> OnParticleTriggerAsObservable()
		{
			return onParticleTrigger ?? (onParticleTrigger = new Subject<Unit>());
		}

#endif
	}
}
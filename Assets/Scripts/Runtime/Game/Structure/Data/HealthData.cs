using System;
using RGF.Game.Common;
using UnityEngine;
using Enum = RGF.Game.Common.Enum;

namespace RGF.Game.Structure.Data
{
	[Serializable]
	public class HealthData : INoteProcessor
	{
		public float maxHealth;
		public float health;

		public void OnHandleNote(Enum.JudgeType judge)
		{
			switch (judge)
			{
				case Enum.JudgeType.PERFECT:
				{
					health += Constant.Health.PERFECT;
					break;
				}
				case Enum.JudgeType.GREAT:
				{
					health += Constant.Health.GREAT;
					break;
				}
				case Enum.JudgeType.GOOD:
				{
					health += Constant.Health.GOOD;
					break;
				}
				case Enum.JudgeType.BAD:
				{
					health += Constant.Health.BAD;
					break;
				}
				case Enum.JudgeType.POOR:
				{
					health -= Constant.Health.DAMAGE0;
					break;
				}
			}

			health = Mathf.Clamp(health, 0, maxHealth);
		}
	}
}
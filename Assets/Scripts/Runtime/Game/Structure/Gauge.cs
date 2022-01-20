using RGF.Game.Common;

namespace RGF.Game.Structure
{
	public sealed class Gauge
	{
		public readonly float BadDamage;
		public readonly float GoodHealAmount = 0;
		public readonly float GreatHealAmount;
		public readonly float PoorDamage;
		public readonly Enum.GaugeType Type;

		public Gauge(Enum.GaugeType type, float total, int noteCount)
		{
			Type = type;

			if (type == Enum.GaugeType.Groove)
			{
				Hp = 0.2f;
				GreatHealAmount = total / noteCount;
				GoodHealAmount = GreatHealAmount / 2;
				BadDamage = 0.04f;
				PoorDamage = 0.06f;
			}
			else if (type == Enum.GaugeType.Easy)
			{
				Hp = 0.2f;
				GreatHealAmount = total / noteCount * 1.2f;
				GoodHealAmount = GreatHealAmount / 2;
				BadDamage = 0.032f;
				PoorDamage = 0.048f;
			}
			else if (type == Enum.GaugeType.Survival)
			{
				Hp = 1.0f;
				GreatHealAmount = 0.1f;
				BadDamage = 0.06f;
				PoorDamage = 0.1f;
			}
			else if (type == Enum.GaugeType.EXSurvival)
			{
				Hp = 1.0f;
				GreatHealAmount = 0.1f;
				BadDamage = 0.1f;
				PoorDamage = 0.18f;
			}
			else if (type == Enum.GaugeType.MAXCombo)
			{
				Hp = 1.0f;
				GreatHealAmount = 0.0f;
				BadDamage = 1.0f;
				PoorDamage = 1.0f;
			}
			else if (type == Enum.GaugeType.Perfect)
			{
				Hp = 1.0f;
				GreatHealAmount = 0.0f;
				GoodHealAmount = -100.0f;
				BadDamage = 1.0f;
				PoorDamage = 1.0f;
			}

			GreatHealAmount /= 100;
			GoodHealAmount /= 100;
		}

		public float Hp { get; set; }
	}
}
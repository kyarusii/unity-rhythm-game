using System;

namespace RGF.Game.Common
{
	public class Enum
	{
		public enum Direction
		{
			Vertical,
			Horizontal
		}

		public enum GaugeType
		{
			Easy,
			Groove,
			Survival,
			EXSurvival,
			MAXCombo,
			Perfect
		}

		public enum JudgeType
		{
			IGNORE,
			POOR,
			BAD,
			GOOD,
			GREAT,
			PERFECT
		}

		[Flags]
		public enum Lntype
		{
			NONE = 0,
			LN1 = 1 << 1,
			LN2 = 1 << 2,
			LNOBJ = 1 << 3
		}

		[Serializable]
		public enum Position
		{
			LEFT,
			CENTER,
			RIGHT
		}

		public enum Rank
		{
			SSS,
			SS,
			S,
			AAA,
			AA,
			A,
			B,
			C,
			D,
			E,
			F
		}

		[Serializable]
		public enum Travel
		{
			Loading = 0,
			Select,
			Game,
			Result
		}
	}
}
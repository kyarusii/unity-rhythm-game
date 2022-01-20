using RGF.Game.BMS.Object;
using RGF.Game.Common;

namespace RGF.Game.Utility
{
	public static class Judge
	{
		public static Enum.JudgeType Evaluate(NoteObject note, double currentTime)
		{
			double diff = Math.Abs(note.timing - currentTime) * 1000;

			if (diff <= Constant.Delta.PERFECT)
			{
				return Enum.JudgeType.PERFECT;
			}

			if (diff <= Constant.Delta.GREAT)
			{
				return Enum.JudgeType.GREAT;
			}

			if (diff <= Constant.Delta.GOOD)
			{
				return Enum.JudgeType.GOOD;
			}

			if (diff <= Constant.Delta.BAD)
			{
				return Enum.JudgeType.BAD;
			}

			if (currentTime > note.timing)
			{
				return Enum.JudgeType.POOR;
			}

			return Enum.JudgeType.IGNORE;
		}

		public static Enum.Rank Rank(double ratio)
		{
			if (ratio >= 1)
			{
				return Enum.Rank.SSS;
			}

			if (ratio >= 0.99)
			{
				return Enum.Rank.SS;
			}

			if (ratio >= 0.97)
			{
				return Enum.Rank.S;
			}

			if (ratio >= 0.95)
			{
				return Enum.Rank.AAA;
			}

			if (ratio >= 0.93)
			{
				return Enum.Rank.AA;
			}

			if (ratio >= 0.90)
			{
				return Enum.Rank.A;
			}

			if (ratio >= 0.80)
			{
				return Enum.Rank.B;
			}

			if (ratio >= 0.75)
			{
				return Enum.Rank.C;
			}

			if (ratio >= 0.70)
			{
				return Enum.Rank.D;
			}

			if (ratio >= 0.60)
			{
				return Enum.Rank.E;
			}

			return Enum.Rank.F;
		}
	}
}
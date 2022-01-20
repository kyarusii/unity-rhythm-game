using System;
using RGF.Game.Common;
using Enum = RGF.Game.Common.Enum;

namespace RGF.Game.Structure.Data
{
	[Serializable]
	public class ScoreData : INoteProcessor
	{
		public long bestCombo;
		public long combo;

		public long score;
		public long pureScore;

		public long bestPureCombo;
		public long pureCombo;

		public double accuracyAverage;
		public double accuracySum;

		public double gapEarlyAverage;
		public double gapLateAverage;

		public long gapEarly;
		public long gapLate;

		public long earlyCount;
		public long lateCount;

		public int fever = 1;

		public void OnHandleNote(Enum.JudgeType judge)
		{
			switch (judge)
			{
				case Enum.JudgeType.PERFECT:
				{
					Process(Constant.Score.PERFECT, Constant.Rate.PERFECT);
					break;
				}
				case Enum.JudgeType.GREAT:
				{
					Process(Constant.Score.GREAT, Constant.Rate.GREAT);
					break;
				}
				case Enum.JudgeType.GOOD:
				{
					Process(Constant.Score.GOOD, Constant.Rate.GOOD);
					break;
				}
				case Enum.JudgeType.BAD:
				{
					Process(Constant.Score.BAD, Constant.Rate.BAD);
					break;
				}
				case Enum.JudgeType.POOR:
				{
					combo = 0;
					break;
				}
			}

			// 베스트 콤보보다 크면 저장
			if (bestCombo < combo)
			{
				bestCombo = combo;
			}

			// 베스트 콤보보다 크면 저장
			if (bestPureCombo < pureCombo)
			{
				bestPureCombo = pureCombo;
			}
		}

		private void Process(long scorePoint, float accuracy)
		{
			score += scorePoint * fever;
			pureScore += scorePoint;
			accuracySum += accuracy;
			combo += fever;
			pureCombo += 1;
		}
	}
}
using System;
using UnityEngine;

namespace RGF.Game.Common.Policy
{
	[CreateAssetMenu(menuName = "RGF/고정 점수 정책 (Score Policy)", order = 400)]
	public class FixedScorePolicy : ScorePolicy
	{
		public int fever2Add = 200;
		public int fever3Add = 300;
		public int fever4Add = 400;
		public int fever5Add = 500;


		public override float GetScore(Enum.JudgeType judge, long wholeNoteCount, int fever)
		{
			float add = fever switch
			{
				2 => fever2Add,
				3 => fever3Add,
				4 => fever4Add,
				5 => fever5Add,
				_ => 0
			};

			float value = 0f;
			switch (judge)
			{
				case Enum.JudgeType.IGNORE:
					break;
				case Enum.JudgeType.POOR:
					value = 0;
					break;
				case Enum.JudgeType.BAD:
				case Enum.JudgeType.GOOD:
				case Enum.JudgeType.GREAT:
				case Enum.JudgeType.PERFECT:
					value = maxScore / (float)wholeNoteCount;
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(judge), judge, null);
			}

			return value + add;
		}
	}
}
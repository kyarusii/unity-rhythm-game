using System;
using UnityEngine;

namespace RGF.Game.Common.Policy
{
	[CreateAssetMenu(menuName = "RGF/배율 점수 정책 (Score Policy)", order = 400)]
	public class RatioScorePolicy : ScorePolicy
	{
		public float fever1Multiply = 1.00f;
		public float fever2Multiply = 1.05f;
		public float fever3Multiply = 1.10f;
		public float fever4Multiply = 1.15f;
		public float fever5Multiply = 1.20f;

		public JudgeScore[] judgeScores;

		[Serializable]
		public struct JudgeScore
		{
			public Enum.JudgeType type;
			public long maxScore;
		}

		public override float GetScore(Enum.JudgeType judge, long wholeNoteCount, int fever)
		{
			float multiply = fever switch
			{
				2 => fever2Multiply,
				3 => fever3Multiply,
				4 => fever4Multiply,
				5 => fever5Multiply,
				_ => fever1Multiply
			};

			float value = maxScore / (float)wholeNoteCount * multiply;

			foreach (JudgeScore judgeScore in judgeScores)
			{
				if (judgeScore.type == judge)
				{
					value = judgeScore.maxScore / (float)wholeNoteCount * multiply;
					break;
				}
			}

			return value;
		}
	}
}
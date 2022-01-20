using UnityEngine;

namespace RGF.Game.Common.Policy
{
	/// <summary>
	/// 
	/// </summary>
	/// <see cref="https://namu.wiki/w/DJMAX%20RESPECT/%EA%B2%8C%EC%9E%84%20%EA%B4%80%EB%A0%A8%20%EC%A0%95%EB%B3%B4#s-4.5"/>
	public abstract class ScorePolicy : ScriptableObject
	{
		public long maxScore = 350000;

		public virtual float GetScore(Enum.JudgeType judge, long wholeNoteCount, int fever)
		{
			return maxScore / (float)wholeNoteCount;
		}
	}
}
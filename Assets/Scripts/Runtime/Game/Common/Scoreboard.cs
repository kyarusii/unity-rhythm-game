using RGF.Game.Common.Policy;

namespace RGF.Game.Common
{
	public sealed class Scoreboard
	{
		private ScorePolicy policy;
		private long noteCount;

		private long combo;
		private long bestCombo;

		public Scoreboard() { }

		public void Clean()
		{
			policy = default;
			noteCount = default;

			combo = default;
			bestCombo = default;
		}

		public void SetPolicy(ScorePolicy _policy)
		{
			policy = _policy;
		}

		public void SetNoteCount(long _noteCount)
		{
			noteCount = _noteCount;
		}

		public void AddNote(Enum.JudgeType judge, int fever)
		{
			float score = policy.GetScore(judge, noteCount, fever);
		}
	}
}
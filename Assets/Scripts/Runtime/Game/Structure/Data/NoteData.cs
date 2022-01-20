using System;
using RGF.Game.Common;
using Enum = RGF.Game.Common.Enum;

namespace RGF.Game.Structure.Data
{
	[Serializable]
	public class NoteData : INoteProcessor
	{
		public long totalNote;
		public long hitCount;

		public long perfect;
		public long great;
		public long good;
		public long bad;
		public long poor;

		public bool IsFailed => totalNote > hitCount;

		public void OnHandleNote(Enum.JudgeType judge)
		{
			switch (judge)
			{
				case Enum.JudgeType.POOR:
					poor++;
					break;
				case Enum.JudgeType.BAD:
					bad++;
					break;
				case Enum.JudgeType.GOOD:
					good++;
					break;
				case Enum.JudgeType.GREAT:
					great++;
					break;
				case Enum.JudgeType.PERFECT:
					perfect++;
					break;
				case Enum.JudgeType.IGNORE:
				default:
					throw new ArgumentOutOfRangeException(nameof(judge), judge, null);
			}

			hitCount++;
		}
	}
}
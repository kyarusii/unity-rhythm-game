using System;
using RGF.Game.Common;
using UnityEngine;
using Enum = RGF.Game.Common.Enum;

namespace RGF.Game.Structure.Data
{
	[Serializable]
	public class FeverData : INoteProcessor
	{
		public float gauge = 0f;

		public float chance = 50f;
		public int current = 1;
		public int max = 5;

		public bool inFever = false;

		public void OnHandleNote(Enum.JudgeType judgeType)
		{
			switch (judgeType)
			{
				case Enum.JudgeType.POOR:
				{
					if (inFever)
					{
						chance -= Constant.Chance.POOR;
					}

					gauge += Constant.Fever.POOR;
				}
					break;
				case Enum.JudgeType.BAD:
				{
					if (inFever)
					{
						chance -= Constant.Chance.BAD;
					}

					gauge += Constant.Fever.BAD;
				}
					break;
				case Enum.JudgeType.GOOD:
				{
					if (inFever)
					{
						chance -= Constant.Chance.GOOD;
					}

					gauge += Constant.Fever.GOOD;
				}
					break;
				case Enum.JudgeType.GREAT:
				{
					if (inFever)
					{
						chance -= Constant.Chance.GREAT;
					}

					gauge += Constant.Fever.GREAT;
				}
					break;
				case Enum.JudgeType.PERFECT:
				{
					if (inFever)
					{
						chance -= Constant.Chance.PERFECT;
					}

					gauge += Constant.Fever.PERFECT;
				}
					break;

				default:
					throw new ArgumentOutOfRangeException(nameof(judgeType), judgeType, null);
			}

			if (inFever)
			{
				if (chance <= 0.0f)
				{
					inFever = false;
					current = 1;

					Message.Execute(Event.OnFeverFinished);
				}

				chance = Mathf.Clamp(chance, 0, 50f);
			}

			// fever
			if (gauge >= 100f)
			{
				current += 1;
				current = Mathf.Clamp(current, 2, max);

				chance = 50f;
				gauge = 0f;

				inFever = true;

				Message.Execute<int>(Event.OnFeverIncrease, current);
			}
		}
	}
}
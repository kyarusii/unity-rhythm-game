using System;
using UnityEngine.Events;
using Enum = RGF.Game.Common.Enum;

namespace RGF.Game.Model.Event
{
	[Serializable]
	public class UnityJudgeIntEvent : UnityEvent<Enum.JudgeType, int> { }
}
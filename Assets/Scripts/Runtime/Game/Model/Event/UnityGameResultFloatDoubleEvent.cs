using System;
using RGF.Game.Structure.Data;
using UnityEngine.Events;

namespace RGF.Game.Model.Event
{
	[Serializable]
	public class UnityGameResultFloatDoubleEvent : UnityEvent<GameResult, float, double> { }
}
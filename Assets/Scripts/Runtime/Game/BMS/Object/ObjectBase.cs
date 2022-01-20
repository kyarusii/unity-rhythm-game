using System;
using Math = RGF.Game.Common.Math;

namespace RGF.Game.BMS.Object
{
	public abstract class ObjectBase : IComparable<ObjectBase>
	{
		protected ObjectBase(int bar, double beat, double beatLength)
		{
			this.bar = bar;
			this.beat = beat / beatLength * 4.0;
		}

		public int bar { get; protected set; }
		public double beat { get; protected set; }
		public double timing { get; set; }

		public int CompareTo(ObjectBase other)
		{
			if (beat < other.beat)
			{
				return 1;
			}

			if (Math.Abs(beat - other.beat) < Math.Tolerance)
			{
				return 0;
			}

			return -1;
		}

		public void CalculateBeat(double prevBeats, double beatC)
		{
			beat = beat * beatC + prevBeats;
		}
	}
}
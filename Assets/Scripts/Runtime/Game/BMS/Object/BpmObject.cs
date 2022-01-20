namespace RGF.Game.BMS.Object
{
	public class BpmObject : ObjectBase
	{
		public BpmObject(int bar, double bpm, double beat, double beatLength) : base(bar, beat, beatLength)
		{
			Bpm = bpm;
		}

		public double Bpm { get; }
	}
}
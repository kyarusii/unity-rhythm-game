namespace RGF.Game.BMS.Object
{
	public class StopObject : ObjectBase
	{
		public string Key;

		public StopObject(int bar, string key, double beat, double beatLength) : base(bar, beat, beatLength)
		{
			Key = key;
		}
	}
}
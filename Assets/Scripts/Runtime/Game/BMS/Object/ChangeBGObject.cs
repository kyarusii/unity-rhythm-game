namespace RGF.Game.BMS.Object
{
	public class ChangeBGObject : ObjectBase
	{
		public ChangeBGObject(int bar, string key, double beat, double beatLength, bool isPic) : base(bar, beat,
			beatLength)
		{
			Key = key;
			IsPic = isPic;
		}

		public string Key { get; }
		public bool IsPic { get; set; }
	}
}
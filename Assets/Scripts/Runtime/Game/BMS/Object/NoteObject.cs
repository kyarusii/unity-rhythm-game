using UnityEngine;

namespace RGF.Game.BMS.Object
{
	public class NoteObject : ObjectBase
	{
		public NoteObject(int bar, int keySound, double beat, double beatLength, int extra) : base(bar, beat,
			beatLength)
		{
			KeySound = keySound;
			Extra = extra;
		}

		public int KeySound { get; }
		public int Extra { get; set; }
		public GameObject Model { get; set; }
	}
}
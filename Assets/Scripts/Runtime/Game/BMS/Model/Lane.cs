using System.Collections.Generic;
using RGF.Game.BMS.Object;

namespace RGF.Game.BMS.Model
{
	public sealed class Lane
	{
		public readonly List<NoteObject> mineList = new List<NoteObject>(20);
		public readonly List<NoteObject> noteList = new List<NoteObject>(225);

		public Lane() { }

		public Lane Clone()
		{
			Lane lane = new Lane();

			lane.mineList.AddRange(mineList);
			lane.noteList.AddRange(noteList);

			return lane;
		}
	}
}
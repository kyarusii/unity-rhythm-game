using System.Collections.Generic;
using RGF.Game.BMS.Object;

namespace RGF.Game.BMS.Model
{
	public sealed class PlayableTrack
	{
		#region HEADER

		public string Title;
		public string SubTitle;
		public int Level;
		public double BeatsPerMinute;

		public Dictionary<int, string> WavTable = new Dictionary<int, string>();
		public Dictionary<string, double> BpmTable = new Dictionary<string, double>();
		public Dictionary<string, double> StopDurations = new Dictionary<string, double>();

		#endregion

		#region BODY

		public int NoteCount = 0;
		public int BarCount = 0;

		public List<ChangeBGObject> ChangeBgList = new List<ChangeBGObject>();
		public List<NoteObject> NoteList = new List<NoteObject>();
		public List<BpmObject> BpmList = new List<BpmObject>();
		public List<StopObject> StopList = new List<StopObject>();

		public Dictionary<int, double> BeatTable = new Dictionary<int, double>();

		#endregion

		public PlayableTrack() { }

		public void SetHeader(Track header)
		{
			Title = header.title;
			SubTitle = header.subtitle;
			Level = header.playerLevel;
			BeatsPerMinute = header.bpm;

			WavTable = header.wavNoteMap;
			BpmTable = header.bpms;
			StopDurations = header.stopDurations;
		}

		// public void SetBody(TrackBody body)
		// {
		// 	this.NoteCount = body.noteCount;
		// 	this.BarCount = body.barCount;
		//
		// 	this.ChangeBgList = body.changeBgObjs;
		// 	this.NoteList = body.noteObjs;
		// 	this.BpmList = body.bpmObjs;
		// 	this.StopList = body.stopObjs;
		//
		// 	this.BeatTable = body.beatCTable;
		// }
	}
}
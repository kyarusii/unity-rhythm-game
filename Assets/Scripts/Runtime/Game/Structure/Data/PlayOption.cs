using System;
using System.Collections.Generic;
using UnityEngine;

namespace RGF.Game.Structure.Data
{
	[Serializable]
	public class PlayOption
	{
		public float magnification = default;
		public GameMode gameMode = default;
		public FeverMode feverMode = default;

		public List<NoteKey> autoLane = default;
		// public bool autoPlay = default;

		public PlayOption()
		{
			autoLane = new List<NoteKey>();
		}

		public PlayOption Clone()
		{
			return new PlayOption
			{
				magnification = magnification,
				gameMode = gameMode,
				feverMode = feverMode,
				autoLane = autoLane
			};
		}

		public void Save()
		{
			string json = JsonUtility.ToJson(this);
			PlayerPrefs.SetString("Rhythm.Game.PlayOption", json);
		}

		public static PlayOption Load()
		{
			string key = "Rhythm.Game.PlayOption";
			if (PlayerPrefs.HasKey(key))
			{
				return JsonUtility.FromJson<PlayOption>(PlayerPrefs.GetString(key));
			}

			return new PlayOption();
		}
	}

	[Serializable]
	public enum GameMode
	{
		NONE,
		MIRROR,
		RANDOM
	}

	[Serializable]
	public enum FeverMode
	{
		MANUAL,
		AUTO
	}
}
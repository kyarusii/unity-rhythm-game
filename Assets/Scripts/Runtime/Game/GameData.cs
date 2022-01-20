using System;
using System.Reflection;
using UnityEngine;

namespace RGF.Game
{
	[Serializable]
	public sealed class GameData : Singleton<GameData>
	{
		public int lastPlayedSongIndex;
		public int lastPlayedPatternIndex;
		public string lastPlayedSong;
		public string lastPlayedPattern;

		public long currentCombo;

		public GameData() { }

		public void Load()
		{
			string json = PlayerPrefs.GetString("RGF.GameData", JsonUtility.ToJson(this));
			GameData loaded = JsonUtility.FromJson<GameData>(json);

			FieldInfo[] fields = typeof(GameData).GetFields();
			foreach (FieldInfo field in fields)
			{
				object value = field.GetValue(loaded);
				field.SetValue(this, value);
			}

			Debug.Log($"GameData Loaded ({lastPlayedSongIndex}, {lastPlayedPatternIndex}, {currentCombo})");
		}

		public void Save()
		{
			PlayerPrefs.SetString("RGF.GameData", JsonUtility.ToJson(this));

			Debug.Log($"GameData Saved ({lastPlayedSongIndex}, {lastPlayedPatternIndex}, {currentCombo})");
		}
	}
}
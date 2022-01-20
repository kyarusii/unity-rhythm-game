using System;
using UnityEngine;

namespace RGF.Game.Structure.Data
{
	[Serializable]
	public sealed class GameResult
	{
		public string songInfo;

		public HealthData healthData;
		public ScoreData scoreData;
		public NoteData noteData;
		public PlayOption playOption;

		public void Save()
		{
			string json = JsonUtility.ToJson(this);
			PlayerPrefs.SetString($"Rhythm.Game.Result.{songInfo}", json);
		}

		public static GameResult Load(string songInfo)
		{
			string key = $"Rhythm.Game.Result.{songInfo}";
			if (PlayerPrefs.HasKey(key))
			{
				return JsonUtility.FromJson<GameResult>(PlayerPrefs.GetString(key));
			}

			return null;
		}
	}
}
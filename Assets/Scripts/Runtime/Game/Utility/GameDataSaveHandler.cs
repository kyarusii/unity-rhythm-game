using System.IO;
using RGF.Game.Structure.Data;
using UnityEngine;

namespace RGF.Game.Utility
{
	public class GameDataSaveHandler
	{
		private static string resultPath =>
			$"{Application.dataPath}/{Path.GetFileName(GameService.Inst.track.title)}.Result.json";

		public static GameResult GetSavedResult()
		{
			if (!ExistSavedData())
			{
				SaveResult(new GameResult());
			}

			GameResult result = JsonUtility.FromJson<GameResult>(File.ReadAllText(resultPath));
			return result;
		}

		public static void SaveResult(GameResult result)
		{
			string json = JsonUtility.ToJson(result);
#if UNITY_EDITOR
			Debug.Log(json);
#else
			File.WriteAllText(resultPath, json.ToString());
#endif
		}

		public static bool ExistSavedData()
		{
			return File.Exists(resultPath);
		}
	}
}
using System;
using UnityEditor;
using UnityEngine;
using Enum = RGF.Game.Common.Enum;

namespace RGF.Game.Structure
{
	[Serializable]
	public class SettingObject
	{
		public Enum.Position position = default;
		public float speed = default;
		public float judgemenetSyncOffset = 0f;

		public SettingObject()
		{
			speed = 4f;
			position = Enum.Position.CENTER;
		}

#if UNITY_EDITOR
		[MenuItem("Tools/Setting Object/Override")]
		public static void Override()
		{
			new SettingObject().Save();
		}
#endif

		#region API

		private const string key = "Rhythm.Setting";
		private static SettingObject current = default;

		public void Save()
		{
			PlayerPrefs.SetString(key, JsonUtility.ToJson(this));
		}

		public static SettingObject Get()
		{
			if (current == null)
			{
				string data = PlayerPrefs.GetString(key, JsonUtility.ToJson(new SettingObject()));
				current = JsonUtility.FromJson<SettingObject>(data);
			}

			return current;
		}

		#endregion
	}
}
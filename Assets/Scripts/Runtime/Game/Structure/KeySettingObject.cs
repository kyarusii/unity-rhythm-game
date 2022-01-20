using System;
using UnityEditor;
using UnityEngine;

namespace RGF.Game.Structure
{
	[Serializable]
	public class KeySettingObject
	{
		private const string key = "Rhythm.Setting.KeyConfig";
		public KeyCode scratch1 = default;
		public KeyCode scratch2 = default;

		public KeyCode note1 = default;
		public KeyCode note2 = default;
		public KeyCode note3 = default;
		public KeyCode note4 = default;
		public KeyCode note5 = default;
		public KeyCode note6 = default;
		public KeyCode note7 = default;

		public KeySettingObject()
		{
			scratch1 = KeyCode.LeftShift;
			scratch2 = KeyCode.LeftControl;

			note1 = KeyCode.S;
			note2 = KeyCode.D;
			note3 = KeyCode.F;

			note4 = KeyCode.Space;

			note5 = KeyCode.J;
			note6 = KeyCode.K;
			note7 = KeyCode.L;
		}

		public KeyCode[] Keys {
			get
			{
				return new[]
				{
					note1,
					note2,
					note3,
					note4,
					note5,

					scratch1,
					KeyCode.None,

					note6,
					note7,
					scratch2
				};
			}
		}

		public void Save()
		{
			PlayerPrefs.SetString(key, JsonUtility.ToJson(this));
		}

		public static KeySettingObject Load()
		{
			string json = PlayerPrefs.GetString(key, JsonUtility.ToJson(new KeySettingObject()));
			return JsonUtility.FromJson<KeySettingObject>(json);
		}

#if UNITY_EDITOR
		[MenuItem("Debug/Key Config Override")]
		private static void Override()
		{
			new KeySettingObject().Save();
		}
#endif
	}

	public enum NoteKey
	{
		NOTE1 = 0,
		NOTE2 = 1,
		NOTE3 = 2,
		NOTE4 = 3,
		NOTE5 = 4,

		SCRATCH1 = 5,
		NONE = 6, // PEDAL

		NOTE6 = 7,
		NOTE7 = 8,
		SCRATCH2 = 9
	}

	public static class EnumExtension
	{
		public static bool IsScratch(this NoteKey noteKey)
		{
			return noteKey == NoteKey.SCRATCH1 || noteKey == NoteKey.SCRATCH2;
		}
	}
}
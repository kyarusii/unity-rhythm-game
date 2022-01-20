#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace RGF.Editor
{
	internal class StartFromFirstSceneShortcut
	{
		[InitializeOnLoadMethod]
		public static void Init()
		{
			EditorApplication.playModeStateChanged += LogPlayModeState;
		}

		private const string LastSceneKey = "LastActiveSceneIndex";

		private static void LogPlayModeState(PlayModeStateChange state)
		{
			if (state == PlayModeStateChange.EnteredEditMode && EditorPrefs.HasKey(LastSceneKey))
			{
				EditorSceneManager.OpenScene(
					SceneUtility.GetScenePathByBuildIndex(EditorPrefs.GetInt(LastSceneKey)));
				EditorPrefs.DeleteKey(LastSceneKey);
			}
		}

		[MenuItem("Tools/Play At First #&P")]
		private static void PlayAtFirstScene()
		{
			if (!EditorApplication.isPlaying)
			{
				EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
				EditorPrefs.SetInt(LastSceneKey, SceneManager.GetActiveScene().buildIndex);
				EditorSceneManager.OpenScene(SceneUtility.GetScenePathByBuildIndex(0));
			}

			EditorApplication.isPlaying = !EditorApplication.isPlaying;
		}
	}
}

#endif
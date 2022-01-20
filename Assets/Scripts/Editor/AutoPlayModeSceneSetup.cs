#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.SceneManagement;

namespace RGF.Editor
{
	[InitializeOnLoad]
	public class AutoPlayModeSceneSetup
	{
		static AutoPlayModeSceneSetup()
		{
			if (EditorBuildSettings.scenes.Length < 1)
			{
				return;
			}

			string path = EditorBuildSettings.scenes[0].path;
			EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
		}
	}
}

#endif
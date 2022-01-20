using RGF.UI.Component;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace RGF.Editor
{
	[CustomEditor(typeof(GroupedButton))]
	public class GroupedButtonEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			bool forceDirty = false;
			EditorGUI.BeginChangeCheck();

			EditorGUILayout.Space(10);
			GroupedButton button = (GroupedButton)target;

			EditorGUILayout.LabelField("Text");
			EditorGUI.indentLevel++;
			{
				string str = EditorGUILayout.TextArea(button.GetText());
				if (string.CompareOrdinal(str, button.GetText()) != 0)
				{
					button.SetText(str);
					forceDirty = true;
				}
			}
			EditorGUI.indentLevel--;

			if (EditorGUI.EndChangeCheck() || forceDirty)
			{
				EditorSceneManager.MarkSceneDirty(((GroupedButton)target).gameObject.scene);
			}

			serializedObject.ApplyModifiedProperties();
		}
	}
}
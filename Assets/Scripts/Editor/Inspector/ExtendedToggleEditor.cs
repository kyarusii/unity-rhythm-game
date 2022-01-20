using System;
using RGF.UI.Component;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.UI;

namespace RGF.Editor
{
	[CustomEditor(typeof(ExtendedToggle))]
	public class ExtendedToggleEditor : ToggleEditor
	{
		private SerializedProperty mainText;
		private SerializedProperty subText;
		private SerializedProperty mainImage;
		private SerializedProperty subImage;

		protected override void OnEnable()
		{
			base.OnEnable();

			mainText = serializedObject.FindProperty(nameof(mainText));
			subText = serializedObject.FindProperty(nameof(subText));
			mainImage = serializedObject.FindProperty(nameof(mainImage));
			subImage = serializedObject.FindProperty(nameof(subImage));
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			EditorGUI.BeginChangeCheck();

			EditorGUILayout.Space(10);
			EditorGUILayout.LabelField("Extended");
			EditorGUI.indentLevel++;
			{
				EditorGUILayout.PropertyField(mainText);
				EditorGUILayout.PropertyField(subText);
				EditorGUILayout.PropertyField(mainImage);
				EditorGUILayout.PropertyField(subImage);
			}
			EditorGUI.indentLevel--;

			if (EditorGUI.EndChangeCheck())
			{
				EditorSceneManager.MarkSceneDirty(((ExtendedToggle)target).gameObject.scene);
			}

			serializedObject.ApplyModifiedProperties();
		}
	}
}
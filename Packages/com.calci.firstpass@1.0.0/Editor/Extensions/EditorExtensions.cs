using UnityEditor;

public static class EditorExtensions
{
	public static void DrawScriptField(this SerializedObject serializedObject)
	{
		SerializedProperty iterator = serializedObject.GetIterator();
		for (bool enterChildren = true; iterator.NextVisible(enterChildren); enterChildren = false)
		{
			if ("m_Script" == iterator.propertyPath)
			{
				using (new EditorGUI.DisabledScope(true))
					EditorGUILayout.PropertyField(iterator, true);
			}
		}
	}
}
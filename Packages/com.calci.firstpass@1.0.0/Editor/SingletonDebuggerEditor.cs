using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
#if USE_CALCI_NAMESPACE
namespace Calci.Shared.Editor
{
#endif
	
[CustomEditor(typeof(SingletonDebugger))]
public class SingletonDebuggerEditor : UnityEditor.Editor
{
	private List<Type> types;
	private List<Assembly> assemblies;

	private void OnEnable()
	{
		assemblies = AppDomain
			.CurrentDomain
			.GetAssemblies().ToList();

		types = assemblies
			.SelectMany(e => e.GetTypes())
			.Where(e => e.IsSubclassOfRawGeneric(typeof(Singleton<>)))
			.Where(e => !e.IsAbstract)
			.ToList();
	}

	[ContextMenu("Debug")]
	private void Debug()
	{
		foreach (Type type in types)
		{
			var properties = type.GetProperties(BindingFlags.Static);
			foreach (PropertyInfo propertyInfo in properties)
			{
				UnityEngine.Debug.Log($"{type} :: {propertyInfo}");
			}
		}
	}

	public override void OnInspectorGUI()
	{
		serializedObject.UpdateIfRequiredOrScript();
		serializedObject.DrawScriptField();

		EditorGUI.BeginChangeCheck();

		if (!Application.isPlaying)
		{
			foreach (Type type in types)
			{
				EditorGUILayout.LabelField(type.Name, EditorStyles.boldLabel);
			}
		}
		else
		{
			foreach (Type type in types)
			{
				try
				{
					Assert.IsNotNull(type);
					EditorGUILayout.LabelField(type.Name, EditorStyles.boldLabel);

					Assert.IsNotNull(type.BaseType);
					PropertyInfo inst = type.BaseType.GetProperty("Inst");
					Assert.IsNotNull(inst);
					object instance = inst.GetValue(null, null);

					var fields = type.GetFields();

					EditorGUI.indentLevel++;
					foreach (FieldInfo field in fields)
					{
						EditorGUILayout.LabelField($"{field.Name}",
							$"{field.GetValue(instance)} ({field.FieldType})");
					}

					EditorGUI.indentLevel--;
				}
				catch
				{
					// ignore
					// UnityEngine.Debug.LogException(e);
				}
			}
		}

		if (EditorGUI.EndChangeCheck()) { }

		serializedObject.ApplyModifiedProperties();
	}
}


#if USE_CALCI_NAMESPACE
}
#endif
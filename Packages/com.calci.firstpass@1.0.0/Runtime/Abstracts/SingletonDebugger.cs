using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class SingletonDebugger : MonoSingleton<SingletonDebugger>
{
	[ContextMenu("Print")]
	private void Debug()
	{
		var assemblies = AppDomain
			.CurrentDomain
			.GetAssemblies().ToList();

		var types = assemblies
			.SelectMany(e => e.GetTypes())
			.Where(e => e.IsSubclassOfRawGeneric(typeof(Singleton<>)))
			.Where(e => !e.IsAbstract)
			.ToList();
		
		foreach (Type type in types)
		{
			var properties = type.BaseType.GetProperties();
			foreach (PropertyInfo propertyInfo in properties)
			{
				UnityEngine.Debug.Log($"{type} :: {propertyInfo}");
			}
		}
	}
}
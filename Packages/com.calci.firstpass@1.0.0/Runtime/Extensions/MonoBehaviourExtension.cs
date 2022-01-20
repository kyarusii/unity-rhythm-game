using UnityEngine;

public static class MonoBehaviourExtension
{
	public static T GetOrAddComponent<T>(this GameObject target)
		where T : Component
	{
		T component = target.GetComponent<T>();
		if (component == null)
		{
			component = target.AddComponent<T>();
		}

		return component;
	}
	
	public static T GetOrAddComponent<T>(this Component target)
		where T : Component
	{
		return target.gameObject.GetOrAddComponent<T>();
	}
}
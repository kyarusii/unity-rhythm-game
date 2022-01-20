using System.IO;
using System.Linq;
using UnityEngine;

/// <summary>
/// 프로젝트에 단 하나만 존재하는 ScriptableObject 제네릭 싱글톤 템플릿 클래스
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class ScriptableObjectSingleton<T> : ScriptableObject
	where T : ScriptableObjectSingleton<T>
{
	private const string PATH = "ScriptableObjects";
	protected static T _instance = default;

	public static T Inst {
		get
		{
			if (_instance == null)
			{
				_instance = Resources.FindObjectsOfTypeAll<T>().FirstOrDefault();

#if UNITY_EDITOR
				if (_instance == null)
				{
					_instance = CreateInstance<T>();
					_instance.OnCreate();
					
					UnityEditor.AssetDatabase.CreateAsset(_instance, GetFullPath());
					UnityEditor.AssetDatabase.SaveAssets();
				}
				else
				{
				}
#endif
			}

			_instance.OnAccessed();
			return _instance;
		}
	}

#if UNITY_EDITOR && UNITY_2019_3_OR_NEWER
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
	private static void ReloadDomain()
	{
		_instance = null;
	}
#endif

	protected virtual void OnCreate() { }

	protected virtual void OnAccessed() { }
	
	
	private static string GetFullPath()
	{
		string fullPath = "Assets/Resources/" + PATH + "/" + typeof(T).FullName + ".asset";
		DirectoryInfo directory = new FileInfo(fullPath).Directory;

		UnityEngine.Assertions.Assert.IsNotNull(directory);
		
		if (!directory.Exists)
		{
			directory.Create();
		}

		return fullPath;
	}
}
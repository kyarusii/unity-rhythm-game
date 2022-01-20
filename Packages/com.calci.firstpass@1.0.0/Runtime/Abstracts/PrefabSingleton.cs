using UnityEngine;

public abstract class PrefabSingleton<T> : MonoBehaviour where T : PrefabSingleton<T>
{
	private static T _instance = default;

	public static T Inst {
		get
		{
			if (_instance == null)
			{
				_instance = FindObjectOfType<T>();

				if (_instance == null)
				{
					T prefab = Resources.Load<T>($"Prefab/{typeof(T).Name}");
					
#if UNITY_EDITOR || DEVELOPMENT_BUILD
					UnityEngine.Assertions.Assert.IsNotNull(prefab);
#endif

					_instance = Instantiate(prefab);
					DontDestroyOnLoad(_instance.gameObject);
				}
			}

			return _instance;
		}
	}

	protected virtual string L(string msg)
	{
		return msg;
	}

#if UNITY_EDITOR && UNITY_2019_3_OR_NEWER
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
	private static void ReloadDomain()
	{
		_instance = null;
	}
#endif
}
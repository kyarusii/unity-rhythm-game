using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// MonoBehaviour 버전 다이나믹 제네릭 싱글톤 템플릿 클래스
/// 만약 인스턴스가 없으면, 동적으로 생성합니다.
/// </summary>
/// <typeparam name="T">Instance로 리턴받을 타입</typeparam>
[DisallowMultipleComponent]
public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
	private static T _instance = default;

	public static T Inst {
		get
		{
#if UNITY_EDITOR
			Assert.IsTrue(Application.isPlaying);
#endif

			if (_instance == null)
			{
				_instance = FindObjectOfType<T>();

				if (_instance == null)
				{
					_instance = new GameObject($"[{typeof(T).Name}]").AddComponent<T>();
					_instance.gameObject.AddComponent<ManagedDontDestroy>();
					_instance.OnCreateInternal();

					// DontDestroyOnLoad(_instance.gameObject);
				}
			}

			return _instance;
		}
	}

	private void OnCreateInternal()
	{
		OnCreate();
	}

	private void OnDestroy()
	{
		_instance = null;
		OnDelete();
	}

	public static bool HasInstance()
	{
		return _instance != null;
	}

	protected virtual void OnCreate() { }
	protected virtual void OnDelete() { }
	
	protected virtual void OnReloadDomain() { }


#if UNITY_EDITOR && UNITY_2019_3_OR_NEWER
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
	private static void ReloadDomain()
	{
		if (_instance != null)
		{
			_instance.OnReloadDomain();
			_instance = null;
		}
	}
#endif
}
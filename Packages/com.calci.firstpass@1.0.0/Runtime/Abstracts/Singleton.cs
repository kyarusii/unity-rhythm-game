using UnityEngine;

/// <summary>
/// 제네릭 싱글톤 템플릿 클래스
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class Singleton<T> where T : Singleton<T>, new()
{
	protected static T _instance = default;

	public static T Inst {
		get
		{
			if (_instance == null)
			{
				_instance = new T();
			}

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

	public virtual void Init() { }
}
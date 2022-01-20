using System;
using UnityEngine;

/// <summary>
/// MonoBehaviour 버전 제네릭 싱글톤 템플릿 클래스
/// 만약 등록된 인스턴스가 없으면, Exception을 발생시킵니다.
/// </summary>
/// <typeparam name="T"></typeparam>
[DisallowMultipleComponent]
public abstract class MonoInstance<T> : MonoBehaviour 
	where T : MonoInstance<T>
{
	private static T _instance;

	public static T Inst {
		get
		{
			if (_instance == null)
			{
				_instance = FindObjectOfType<T>();

				if (_instance == null)
				{
					throw new InstanceNotRegisteredException(typeof(T));
				}
			}

			return _instance;
		}
	}

	public static bool HasInstance()
	{
		return _instance != null && Application.isPlaying;
	}

#if UNITY_EDITOR && UNITY_2019_3_OR_NEWER
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
	private static void ReloadDomain()
	{
		_instance = null;
	}
#endif

	private void Awake()
	{
		_instance = GetComponent<T>();
		_OnAwake();
	}

	private void OnDestroy()
	{
		_instance = null;
		_OnDestroy();
	}

	protected virtual void _OnAwake() { }

	protected virtual void _OnDestroy() { }
}

internal sealed class InstanceNotRegisteredException : Exception
{
	public InstanceNotRegisteredException() : base()
	{
	}

	public InstanceNotRegisteredException(Type type) : base(type.FullName)
	{
	}
}
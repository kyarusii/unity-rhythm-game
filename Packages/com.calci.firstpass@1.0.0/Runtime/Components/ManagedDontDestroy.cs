using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class ManagedDontDestroy : MonoBehaviour
{
	[SerializeField] private bool enable = true;
	[SerializeField] private bool managed = true;

	private static readonly List<MonoBehaviour> managedList = new List<MonoBehaviour>();
	public static IReadOnlyList<MonoBehaviour> ManagedList => managedList;

	private void Awake()
	{
		if (enable)
		{
#if UNITY_EDITOR
			// 이 컴포넌트가 붙은 게임오브젝트는 필수적으로 씬 루트 오브젝트이어야 합니다.
			UnityEngine.Assertions.Assert.IsNull(transform.parent);
#endif
			DontDestroyOnLoad(gameObject);

			if (managed)
			{
				managedList.Add(this);
			}
		}
	}

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
	private static void OnDomainReset()
	{
		managedList?.Clear();
	}
}
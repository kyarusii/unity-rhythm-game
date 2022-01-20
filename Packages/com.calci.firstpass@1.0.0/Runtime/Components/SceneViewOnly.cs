using UnityEngine;

[DisallowMultipleComponent]
public sealed class SceneViewOnly : MonoBehaviour
{
#if UNITY_EDITOR
	[SerializeField] private bool enable = true;
	private void Awake()
	{
		if (enable)
		{
			gameObject.SetActive(false);
		}
	}
#endif
}
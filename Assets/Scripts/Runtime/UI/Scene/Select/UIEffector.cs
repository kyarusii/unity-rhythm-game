using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RGF.UI.Scene.Select
{
	public class UIEffector : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
	{
		private static Dictionary<string, List<UIEffector>> _map;
		[SerializeField] private Image m_graphic = default;

		public string key = default;

		private void Awake()
		{
			_map ??= new Dictionary<string, List<UIEffector>>();

			if (!string.IsNullOrEmpty(key))
			{
				if (!_map.TryGetValue(key, out List<UIEffector> list))
				{
					list = new List<UIEffector>();
				}

				list.Add(this);

				_map[key] = list;
			}

			Focus(false);
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			if (!string.IsNullOrEmpty(key))
			{
				foreach (UIEffector effector in _map[key])
				{
					if (ReferenceEquals(effector, this))
					{
						continue;
					}

					effector.Focus(false);
				}
			}

			Focus(true);
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			Focus(false);
		}

		public void Focus(bool focus)
		{
			if (m_graphic != null)
			{
				m_graphic.enabled = focus;
			}
		}

#if UNITY_EDITOR

		private void Reset()
		{
			m_graphic = GetComponentInChildren<Image>();
		}

#if UNITY_2019_3_OR_NEWER

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void ResetDomain()
		{
			_map = null;
		}
#endif

#endif
	}
}
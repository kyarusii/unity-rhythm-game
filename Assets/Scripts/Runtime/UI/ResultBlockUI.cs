using TMPro;
using UnityEngine;

namespace RGF.UI
{
	public sealed class ResultBlockUI : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI m_name = default;
		[SerializeField] private TextMeshProUGUI m_value = default;

		public void Initialize(string title, string value)
		{
			m_name.SetText(title);
			m_value.SetText(value);
		}
	}
}
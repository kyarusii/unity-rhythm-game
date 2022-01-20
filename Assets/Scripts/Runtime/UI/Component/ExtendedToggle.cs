using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RGF.UI.Component
{
	public class ExtendedToggle : Toggle
	{
		public TextMeshProUGUI mainText;
		public TextMeshProUGUI subText;

		public Image mainImage;
		public Image subImage;

		protected override void Awake()
		{
			base.Awake();

			if (Application.isPlaying)
			{
				onValueChanged.AddListener(OnValueChanged);
			}
		}

		private void OnValueChanged(bool active)
		{
			if (active)
			{
				mainImage.gameObject.SetActive(true);
				subImage.gameObject.SetActive(false);
				mainText.gameObject.SetActive(true);
				subText.gameObject.SetActive(false);

				targetGraphic = subImage;
			}
			else
			{
				mainImage.gameObject.SetActive(false);
				subImage.gameObject.SetActive(true);
				mainText.gameObject.SetActive(false);
				subText.gameObject.SetActive(true);

				targetGraphic = mainImage;
			}
		}

		public void SetText(string msg)
		{
			mainText.SetText(msg);
			subText.SetText(msg);
		}
	}
}
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace RGF.UI
{
	public class Fader : MonoBehaviour
	{
		private Image image = default;

		private void Awake()
		{
			image = GetComponent<Image>();
			image.DOFade(1.0f, 0.0f);
		}

		private void OnEnable()
		{
			Message.Register<float>(Event.OnFadeIn, OnFadeIn);
			Message.Register<float>(Event.OnFadeOut, OnFadeOut);
		}

		private void OnDisable()
		{
			Message.Unregister<float>(Event.OnFadeIn, OnFadeIn);
			Message.Unregister<float>(Event.OnFadeOut, OnFadeOut);
		}

		private void OnFadeIn(float duration)
		{
			image.DOFade(0.0f, duration);
		}

		private void OnFadeOut(float duration)
		{
			image.DOFade(1.0f, duration);
		}
	}
}
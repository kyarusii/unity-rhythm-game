using DG.Tweening;
using MEC;
using TMPro;
using UnityEngine;

namespace RGF.UI.Scene
{
	public class FeverEffector : MonoBehaviour
	{
		[SerializeField] private Animator m_animator = default;
		[SerializeField] private TextMeshPro m_magnificationText = default;
		[SerializeField] private TextMeshPro m_shadowText = default;

		[SerializeField] private SpriteRenderer[] m_auras = default;

		[SerializeField] private Color x2 = default;
		[SerializeField] private Color x3 = default;
		[SerializeField] private Color x4 = default;
		[SerializeField] private Color x5 = default;

		private Color[] colors;
		private Color transparent;

		private void Awake()
		{
			m_magnificationText.transform.localScale = Vector3.zero;
			colors = new[] { x2, x3, x4, x5 };
			transparent = new Color(0, 0, 0, 0);
		}

		private void OnEnable()
		{
			Message.Register<int>(Event.OnFeverIncrease, OnFeverIncrease);
			Message.Register(Event.OnFeverFinished, OnFeverFinished);
		}

		private void OnDisable()
		{
			Message.Unregister<int>(Event.OnFeverIncrease, OnFeverIncrease);
			Message.Unregister(Event.OnFeverFinished, OnFeverFinished);
		}

		private void OnFeverIncrease(int fever)
		{
			m_animator.Rebind();
			m_animator.Play("Energy Field 07");

			m_magnificationText.SetText($"X{fever}");
			m_shadowText.SetText($"X{fever}");

			m_magnificationText.transform.localScale = Vector3.zero;
			m_magnificationText.transform.DOScale(1.0f, 0.3f);

			m_magnificationText.DOFade(1.0f, 0.1f);
			m_shadowText.DOFade(1.0f, 0.1f);

			Timing.CallDelayed(0.8f, () =>
			{
				m_magnificationText.DOFade(0.0f, 0.1f);
				m_shadowText.DOFade(0.0f, 0.1f);
			});

			foreach (SpriteRenderer spriteRenderer in m_auras)
			{
				spriteRenderer.color = colors[fever - 2];
			}
		}

		private void OnFeverFinished()
		{
			foreach (SpriteRenderer spriteRenderer in m_auras)
			{
				spriteRenderer.color = transparent;
			}
		}
	}
}
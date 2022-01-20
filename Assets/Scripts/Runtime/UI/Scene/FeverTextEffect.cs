using DG.Tweening;
using RGF.Game;
using UnityEngine;

namespace RGF.UI.Scene
{
	public class FeverTextEffect : MonoBehaviour
	{
		[SerializeField] private float m_min = -2f;
		[SerializeField] private float m_max = 10f;

		[SerializeField] private float m_speed = 2f;

		private bool isDisplay = false;
		private SpriteRenderer m_renderer = default;

		private Sprite[] sprites = default;

		private void Awake()
		{
			m_renderer = GetComponent<SpriteRenderer>();
			m_renderer.sprite = null;

			sprites = new[]
			{
				Resource.Load<Sprite>("Sprites/Combo x2"),
				Resource.Load<Sprite>("Sprites/Combo x3"),
				Resource.Load<Sprite>("Sprites/Combo x4"),
				Resource.Load<Sprite>("Sprites/Combo x5")
			};
		}

		private void Update()
		{
			if (!isDisplay)
			{
				return;
			}

			Vector3 pos = transform.localPosition;
			pos.x += Time.deltaTime * m_speed;

			if (pos.x > m_max)
			{
				pos.x = m_min;
			}

			transform.localPosition = pos;
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
			isDisplay = true;
			m_renderer.sprite = sprites[fever - 2];
			m_renderer.DOFade(0.4f, 0.4f);
		}

		private void OnFeverFinished()
		{
			m_renderer.DOFade(0.0f, 0.4f)
				.OnComplete(() =>
				{
					m_renderer.sprite = null;
					isDisplay = false;
				});
		}
	}
}
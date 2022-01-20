using System.Collections;
using RGF.Game.Core.Select;
using UnityEngine;
using UnityEngine.UI;

namespace RGF.UI.Scene.Select
{
	public class OptionUI : MonoBehaviour
	{
		private static readonly int _blurSize = Shader.PropertyToID("_Size");
		[SerializeField] private RectTransform m_panel = default;
		[SerializeField] private CanvasGroup m_canvasGroup = default;
		[SerializeField] private Image m_blur = default;

		[SerializeField] private float m_slideSpeed = default;
		private Coroutine _current;

		private bool _isClosed;

		private void Awake()
		{
			_isClosed = true;

			OnCompleteCloseInternal();

			m_blur.material = Instantiate(m_blur.material);
		}

		private void OnEnable()
		{
			Message.Register(Event.OnToggleOption, ToggleInternal);
		}

		private void OnDisable()
		{
			Message.Unregister(Event.OnToggleOption, ToggleInternal);
		}

		private void ToggleInternal()
		{
			if (_isClosed)
			{
				TurnTable.canChangedSong = false;
				TurnTable.canChangedPattern = false;

				if (_current != null)
				{
					StopCoroutine(_current);
					OnCompleteOpenInternal();
				}

				_current = StartCoroutine(OpenCoroutine());
			}
			else
			{
				TurnTable.canChangedSong = true;
				TurnTable.canChangedPattern = true;

				if (_current != null)
				{
					StopCoroutine(_current);
					OnCompleteCloseInternal();
				}

				_current = StartCoroutine(CloseCoroutine());
			}

			_isClosed = !_isClosed;
		}

		private IEnumerator OpenCoroutine()
		{
			float value = 0f;
			while (value < 0.95f)
			{
				value += Time.deltaTime * m_slideSpeed;

				m_canvasGroup.alpha = value;

				float newX = Mathf.Lerp(-1000, 0, value);
				m_panel.anchoredPosition = new Vector2(newX, 0);
				m_blur.material.SetFloat(_blurSize, 5.0f * value);

				yield return null;
			}

			OnCompleteOpenInternal();
		}

		private void OnCompleteOpenInternal()
		{
			m_canvasGroup.alpha = 1;
			m_canvasGroup.interactable = true;
			m_canvasGroup.blocksRaycasts = true;

			m_panel.anchoredPosition = new Vector2(0, 0);
			m_blur.material.SetFloat(_blurSize, 5.0f);
		}

		private IEnumerator CloseCoroutine()
		{
			float value = 0f;
			while (value < 0.95f)
			{
				value += Time.deltaTime * m_slideSpeed;

				m_canvasGroup.alpha = 1 - value;

				float newX = Mathf.Lerp(-1000, 0, 1 - value);
				m_panel.anchoredPosition = new Vector2(newX, 0);
				m_blur.material.SetFloat(_blurSize, 5.0f * (1 - value));

				yield return null;
			}

			OnCompleteCloseInternal();
		}

		private void OnCompleteCloseInternal()
		{
			m_canvasGroup.alpha = 0;
			m_canvasGroup.interactable = false;
			m_canvasGroup.blocksRaycasts = false;

			m_panel.anchoredPosition = new Vector2(-1000, 0);
			m_blur.material.SetFloat(_blurSize, 0f);
		}
	}
}
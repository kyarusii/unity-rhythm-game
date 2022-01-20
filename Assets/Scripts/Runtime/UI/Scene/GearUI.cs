using DG.Tweening;
using RGF.Game;
using RGF.Game.Common;
using TMPro;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
#if UNITY_EDITOR
#endif

namespace RGF.UI.Scene
{
	public class GearUI : MonoBehaviour
	{
		[SerializeField] private Camera m_camera = default;
		[SerializeField] private CanvasScaler m_scaler = default;

		[SerializeField] private TextMeshProUGUI m_judgeText = default;
		[SerializeField] private TextMeshProUGUI m_comboText = default;
		[SerializeField] private TextMeshProUGUI m_comboTitleText = default;

		[SerializeField] private TextMeshPro m_gapText = default;
		[SerializeField] private TextMeshPro m_scoreText = default;

		[SerializeField] private Transform m_judgeWorldPos = default;
		[SerializeField] private Transform m_comboWorldPos = default;

		[SerializeField] private Transform m_fever = default;

		[SerializeField] private TextMeshProUGUI m_songText = default;
		[SerializeField] private TextMeshProUGUI m_artistText = default;

		[Header("Tweener")] [Range(0f, 1f)] [SerializeField]
		private float m_duration = 0.05f;

		[Range(0f, 1f)] [SerializeField] private float m_strength = 0.2f;

		private void Start()
		{
			OnComboUpdate(GameData.Inst.currentCombo);
			m_judgeText.gameObject.SetActive(false);

			UpdateGap(0);
			UpdateScore(0);

			m_songText.SetText($"{GameService.Inst.track.title}");
			m_artistText.SetText(GameService.Inst.track.artist);
		}

		private void Update()
		{
			float x = m_scaler.referenceResolution.x / UnityEngine.Screen.width;
			float y = m_scaler.referenceResolution.y / UnityEngine.Screen.height;

			Vector2 rate = new Vector2(x, y);

			Vector3 judgePos = m_camera.WorldToScreenPoint(m_judgeWorldPos.position);
			judgePos.z = 0f;

			m_judgeText.rectTransform.anchoredPosition = judgePos * rate;

			Vector3 comboPos = m_camera.WorldToScreenPoint(m_comboWorldPos.position);
			comboPos.z = 0f;

			m_comboText.rectTransform.anchoredPosition = comboPos * rate;
			m_comboTitleText.rectTransform.anchoredPosition = comboPos * rate;
		}

		private void OnEnable()
		{
			Message.Register<long>(Event.OnScoreUpdate, OnScoreUpdate);
			Message.Register<long>(Event.OnComboUpdate, OnComboUpdate);
			Message.Register<int>(Event.OnExistGap, OnExistGap);
			Message.Register<float>(Event.OnFeverUpdate, OnFeverUpdate);
			Message.Register<Enum.JudgeType>(Event.OnJudgeUpdate, OnJudgeUpdate);
		}

		private void OnDisable()
		{
			Message.Unregister<long>(Event.OnScoreUpdate, OnScoreUpdate);
			Message.Unregister<long>(Event.OnComboUpdate, OnComboUpdate);
			Message.Unregister<int>(Event.OnExistGap, OnExistGap);
			Message.Unregister<float>(Event.OnFeverUpdate, OnFeverUpdate);
			Message.Unregister<Enum.JudgeType>(Event.OnJudgeUpdate, OnJudgeUpdate);
		}

		[ContextMenu("Configure Positions")]
		private void ConfigurePositions()
		{
			Vector3 judgePos = m_camera.WorldToScreenPoint(m_judgeWorldPos.position);
			judgePos.z = 0f;

			m_judgeText.rectTransform.anchoredPosition = judgePos;

			Vector3 comboPos = m_camera.WorldToScreenPoint(m_comboWorldPos.position);
			comboPos.z = 0f;

			m_comboText.rectTransform.anchoredPosition = comboPos;
			m_comboTitleText.rectTransform.anchoredPosition = comboPos;

#if UNITY_EDITOR
			EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
#endif
		}

		private void UpdateScore(long score)
		{
			m_scoreText.SetText($"{score:000000000}");
		}

		private void UpdateGap(int gap)
		{
			if (gap > 0)
			{
				m_gapText.SetText($"{gap:N0}ms");
			}
			else
			{
				m_gapText.SetText("");
			}
		}

		#region Event

		private void OnComboUpdate(long newCombo)
		{
			if (newCombo == 0)
			{
				m_comboText.gameObject.SetActive(false);
				m_comboTitleText.gameObject.SetActive(false);
			}
			else
			{
				m_comboText.gameObject.SetActive(true);
				m_comboTitleText.gameObject.SetActive(true);
			}

			m_comboText.SetText(newCombo.ToString());
			Tweener tweener = m_comboText.transform.DOShakeScale(m_duration, m_strength);
			tweener.OnComplete(() => m_comboText.transform.localScale = Vector3.one);
		}

		private void OnJudgeUpdate(Enum.JudgeType judge)
		{
			if (!m_judgeText.gameObject.activeSelf)
			{
				m_judgeText.gameObject.SetActive(true);
			}

			m_judgeText.SetText(judge.ToString());
			m_judgeText.colorGradientPreset = Resource.Load<TMP_ColorGradient>($"TMP_Gradient/Judge/{judge}");

			Tweener tweener = m_judgeText.transform.DOShakeScale(m_duration, m_strength);
			tweener.OnComplete(() => m_judgeText.transform.localScale = Vector3.one);
		}

		private void OnExistGap(int gap)
		{
			UpdateGap(gap);
		}

		private void OnScoreUpdate(long score)
		{
			UpdateScore(score);
		}

		private void OnFeverUpdate(float fever)
		{
			float scaleX = Mathf.Lerp(0, 60f, fever);
			m_fever.DOScaleX(scaleX, 0.1f);
		}

		#endregion
	}
}
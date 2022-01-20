using RGF.Game.Core.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RGF.UI.Scene
{
	public class LoadingSceneUI : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI m_logger = default;
		[SerializeField] private Image m_progress = default;

		private float _progress = 0f;

		private void Start()
		{
			m_progress.fillAmount = 0f;

			Database.Inst.LogAction += PrintLog;
			Database.Inst.ProgressAction += PrintProgress;
		}

		private void Update()
		{
			m_progress.fillAmount = Mathf.Lerp(m_progress.fillAmount, _progress, 0.1f);
		}

		private void OnDisable()
		{
			Database.Inst.LogAction -= PrintLog;
			Database.Inst.ProgressAction -= PrintProgress;
		}

		private void PrintLog(string log)
		{
			m_logger.SetText(log);
		}

		private void PrintProgress(float value)
		{
			_progress = value;
		}
	}
}
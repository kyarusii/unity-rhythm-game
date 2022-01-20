using RGF.Game;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RGF.UI
{
	public class JudgeSensitivityUI : MonoBehaviour, IPointerClickHandler
	{
		private Text text;

		private void Start()
		{
			text = GetComponentInChildren<Text>();
			UpdateText();
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			if (eventData.button == PointerEventData.InputButton.Left)
			{
				if (GameService.Inst.judgementSensitivity > 29)
				{
					return;
				}

				GameService.Inst.judgementSensitivity += 1;
				UpdateText();
			}
			else if (eventData.button == PointerEventData.InputButton.Right)
			{
				if (GameService.Inst.judgementSensitivity < -29)
				{
					return;
				}

				GameService.Inst.judgementSensitivity -= 1;
				UpdateText();
			}
		}

		private void UpdateText()
		{
			text.text = GameService.Inst.judgementSensitivity + "ms";
		}
	}
}
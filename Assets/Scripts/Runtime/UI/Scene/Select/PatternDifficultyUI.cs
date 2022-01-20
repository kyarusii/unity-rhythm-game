using RGF.Game;
using RGF.Game.BMS.Model;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RGF.UI.Scene.Select
{
	public class PatternDifficultyUI : MonoBehaviour
	{
		[SerializeField] private Image m_image = default;
		[SerializeField] private string m_defaultSpritePath = default;
		[SerializeField] private string m_selectedSpritePath = default;
		[SerializeField] private TextMeshProUGUI m_patternName = default;
		[SerializeField] private Color m_defaultColor = default;
		[SerializeField] private Color m_textColor = default;

		public void Init(Track track)
		{
			if (track.subtitle == "BLACK ANOTHER")
			{
				m_patternName.SetText("BLACK");
			}
			else
			{
				m_patternName.SetText(track.subtitle);
			}

			m_image.color = m_defaultColor;
		}

		[ContextMenu("Select")]
		public void Select()
		{
			m_image.sprite = Resource.Load<Sprite>(m_selectedSpritePath);
			m_image.color = m_defaultColor;
			m_patternName.color = m_textColor;
			m_image.fillCenter = true;
		}

		[ContextMenu("Deselect")]
		public void Deselect()
		{
			m_image.sprite = Resource.Load<Sprite>(m_defaultSpritePath);
			m_image.color = m_defaultColor;
			m_patternName.color = m_defaultColor;
			m_image.fillCenter = false;
		}
	}
}
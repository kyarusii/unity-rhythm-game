using RGF.Game.Core.Game;
using UnityEngine;

namespace RGF.Game.Core.Rail
{
	public sealed class BarDrawer : MonoBehaviour
	{
		[SerializeField] private GameSession m_gameManager = default;
		// [SerializeField] private SheetBuilder m_parser = default;

		[SerializeField] private Transform m_bar = default;

		[SerializeField] private float m_barMin = -.25f;
		[SerializeField] private float m_barMax = 12f;

		private int _index = default;

		/// <summary>
		///     특정 마디마다 바를 그립니다.
		///     @TODO :: DrawNotes 메서드로 옮기기
		/// </summary>
		private void Update()
		{
			if (!GameService.Inst.isPlaying)
			{
				return;
			}

			if (GameService.Inst.isPaused)
			{
				return;
			}

			for (int i = _index; i < GameService.Inst.track.barCount; ++i)
			{
				float y = (float)(GameService.Inst.track.GetPreviousBarBeatSum(i) * GameService.Inst.Setting.speed
				                  - m_gameManager.scrollValue);
				if (y < m_barMin)
				{
					_index = i - 1;
					continue;
				}

				if (y > m_barMax)
				{
					break;
				}

				m_bar.localPosition = new Vector3(0, y, 0);
			}
		}
	}
}
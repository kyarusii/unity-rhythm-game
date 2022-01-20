using DG.Tweening;
using RGF.Game.BMS.Model;
using RGF.Game.Common;
using UnityEngine;
using UnityEngine.UI;

namespace RGF.UI.Screen
{
	public class MusicSelectScreen : MonoBehaviour
	{
		public RawImage background1;
		public RawImage background2;

		private const float duration = 0.5f;
		private Color color = new Color(0.4f, 0.4f, 0.4f, 1.0f);

		private bool first = true;

		private Song selectedSong;
		private Tweener tweener;

		public void Awake()
		{
			Message.Register<Song>(Event.OnSongChanged, OnSongChanged);
			Message.Register<int>(Event.OnPatternChanged, OnPatternSelectionChanged);

			background1.color = Color.black;
			background2.color = Color.black;
		}

		private void OnDestroy()
		{
			Message.Unregister<Song>(Event.OnSongChanged, OnSongChanged);
			Message.Unregister<int>(Event.OnPatternChanged, OnPatternSelectionChanged);

			if (tweener != null && tweener.IsPlaying())
			{
				tweener.Kill(false);
			}
		}

		private void OnSongChanged(Song song)
		{
			selectedSong = song;
		}

		private void OnPatternSelectionChanged(int patternIndex)
		{
			Texture2D tex = selectedSong.tracks[patternIndex].stageTexture;
			SetBackground(tex);
		}

		private void SetBackground(Texture2D texture)
		{
			if (tweener != null && tweener.IsPlaying())
			{
				tweener.Kill(false);
			}

			if (first)
			{
				background1.color = color;
				Extension.SetAlpha(background1, 0);
				background1.texture = texture;
				background1.transform.SetAsLastSibling();
				tweener = background1.DOFade(1.0f, duration)
					.OnComplete(() => tweener = null);
			}
			else
			{
				background2.color = color;
				Extension.SetAlpha(background2, 0);
				background2.texture = texture;
				background2.transform.SetAsLastSibling();
				tweener = background2.DOFade(1.0f, duration)
					.OnComplete(() => tweener = null);
			}

			first = !first;
		}
	}
}
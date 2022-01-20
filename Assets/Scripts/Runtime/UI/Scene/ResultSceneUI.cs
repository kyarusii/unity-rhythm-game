using System.Collections;
using RGF.Game;
using RGF.Game.BMS.Model;
using RGF.Game.Core.Data;
using RGF.Game.Structure.Data;
using RGF.Game.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Enum = RGF.Game.Common.Enum;

namespace RGF.UI.Scene
{
	public sealed class ResultSceneUI : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI m_rank = default;
		[SerializeField] private TextMeshProUGUI m_score = default;
		[SerializeField] private TextMeshProUGUI m_songTitle = default;
		[SerializeField] private TextMeshProUGUI m_artist = default;
		[SerializeField] private RawImage m_artwork = default;
		[SerializeField] private ResultBlockUI[] m_results = default;

		[SerializeField] private RectTransform m_starRoot = default;

		private void Awake() { }

		public void Start()
		{
			StartCoroutine(DisplayResult());
		}

		private IEnumerator DisplayResult()
		{
			Track track = GameService.Inst.track;
			string uri = $"file://{track.RootPath}/{track.stageFile}";

			if (!Texture2DLibrary.Inst.HasObject(uri))
			{
				yield return Texture2DLibrary.Inst.LoadTexture2D(uri);
			}

			m_artwork.texture = Texture2DLibrary.Inst.Get(uri);

			// bool complete = false;
			// Texture2D tex2D = null;
			// TextureDownloader.Instance.GetTexture2D(uri, texture =>
			// {
			// 	tex2D = texture;
			// 	complete = true;
			// });
			//
			// yield return new WaitUntil(() => complete);
			//
			// m_artwork.texture = tex2D;
			//
			// SafeEditorCall.CallEditorOnly(() =>
			// {
			// 	if (tex2D == null)
			// 	{
			// 		Debug.LogWarning("아트워크가 없습니다.");
			// 	}
			// });

			GameResult result = GameService.Inst.GetLastResult();

			// 노트 통계
			RefreshNote(result.noteData);
			RefreshScore(result.scoreData);
			RefreshStars(track);

			m_score.SetText($"{result.scoreData.score}");
			m_songTitle.SetText(track.title);
			m_artist.SetText(track.artist);
		}

		private void RefreshStars(Track track)
		{
			int childCount = m_starRoot.childCount;
			int level = track.playerLevel;

			if (level >= childCount)
			{
				Debug.LogWarning($"별 개수보다 난이도가 높음! {level}");
				level = childCount;
			}

			for (int i = level; i < childCount; i++)
			{
				m_starRoot.GetChild(i).gameObject.SetActive(false);
			}

			for (int i = 0; i < level; i++)
			{
				m_starRoot.GetChild(i).gameObject.SetActive(true);
			}
		}

		private void RefreshNote(NoteData noteData)
		{
			m_results[0].Initialize("PERFECT", noteData.perfect.ToString("D4"));
			m_results[1].Initialize("GREAT", noteData.great.ToString("D4"));
			m_results[2].Initialize("GOOD", noteData.good.ToString("D4"));
			m_results[3].Initialize("BAD", noteData.bad.ToString("D4"));
			m_results[4].Initialize("POOR", noteData.poor.ToString("D4"));
		}

		private void RefreshScore(ScoreData scoreData)
		{
			m_results[5].Initialize("RATE", scoreData.accuracyAverage.ToString("P"));
			m_results[6].Initialize("BEST COMBO", scoreData.bestCombo.ToString("D4"));

			// 랭크 계산
			Enum.Rank rate = Judge.Rank(scoreData.accuracyAverage);
			m_rank.SetText(rate.ToString());
		}
	}
}
using System.Collections;
using System.Linq;
using RGF.Firstpass;
using RGF.Game.BMS.Model;
using RGF.Game.Core.Data;
using RGF.UI.Component;
using RGF.UI.Scroller;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RGF.UI
{
	public sealed class SongUI : ScrollerItem, IPointerDownHandler
	{
		private const float multiplier = 3.5f;
		[SerializeField] private RawImage m_cover = default;
		[SerializeField] private TextMeshProUGUI m_bigTitle = default;
		[SerializeField] private TextMeshProUGUI m_title = default;
		[SerializeField] private TextMeshProUGUI m_author = default;

		[SerializeField] private Image m_label = default;
		[SerializeField] private UIGradient m_gradient = default;
		[SerializeField] private Image m_gradientMask = default;

		[SerializeField] private GradientAsset m_gradientAsset = default;

		public RectTransform rectTransform = default;

		private Song _info;

		private Coroutine coroutine = default;

		public void OnPointerDown(PointerEventData eventData)
		{
			Message.Execute<Song>(Event.OnSongPointerDown, _info);
		}

		public void Initialize(Song songInfo)
		{
			_info = songInfo;
			m_gradient.EffectGradient = m_gradientAsset.data;

			m_gradientMask.fillAmount = 0f;
			m_gradientMask.gameObject.SetActive(false);

			Track track = songInfo.tracks.FirstOrDefault();

			Assert.IsNotNull(track);

			m_title.SetText(songInfo.songName);
			m_bigTitle.SetText(songInfo.songName);

			m_cover.texture = track.stageTexture;

			m_author.SetText(track.artist);
			string path = "file://" + track.RootPath + "/" + track.stageFile;

			// StartCoroutine(LoadImage(path));

			m_bigTitle.gameObject.SetActive(false);
			m_title.gameObject.SetActive(true);
			m_author.gameObject.SetActive(true);

			Reference.Use(m_label);
		}

		public override void UpdateItem(int count) { }

		private IEnumerator LoadImage(string path)
		{
			// bool complete = false;
			// Texture2D tex2D = null;
			// TextureDownloader.Instance.GetTexture2D(path, texture =>
			// {
			// 	tex2D = texture;
			// 	complete = true;
			// });
			//
			// yield return new WaitUntil(() => complete);

			if (!Texture2DLibrary.Inst.HasObject(path))
			{
				yield return Texture2DLibrary.Inst.LoadTexture2D(path);
			}

			m_cover.texture = Texture2DLibrary.Inst.Get(path);
		}

		public void Select()
		{
			if (coroutine != null)
			{
				StopCoroutine(coroutine);
			}

			coroutine = StartCoroutine(SelectCoroutine());

			m_bigTitle.gameObject.SetActive(true);
			m_title.gameObject.SetActive(false);
			m_author.gameObject.SetActive(false);
		}

		public void Deselect()
		{
			if (coroutine != null)
			{
				StopCoroutine(coroutine);
			}

			coroutine = StartCoroutine(DeselectCoroutine());

			m_bigTitle.gameObject.SetActive(false);
			m_title.gameObject.SetActive(true);
			m_author.gameObject.SetActive(true);
		}


		private IEnumerator SelectCoroutine()
		{
			m_gradientMask.fillAmount = 0f;
			m_gradientMask.gameObject.SetActive(true);

			float value = 0.0f;
			while (value < 0.99f)
			{
				value += Time.deltaTime * multiplier;

				m_gradientMask.fillAmount = value;

				yield return null;
			}

			m_gradientMask.fillAmount = 1.0f;
		}

		private IEnumerator DeselectCoroutine()
		{
			m_gradientMask.fillAmount = 1f;

			float value = 0.0f;
			while (value < 0.99f)
			{
				value += Time.deltaTime * multiplier;

				m_gradientMask.fillAmount = 1 - value;

				yield return null;
			}

			m_gradientMask.fillAmount = 0.0f;
			m_gradientMask.gameObject.SetActive(true);

			m_bigTitle.gameObject.SetActive(false);
		}
	}
}
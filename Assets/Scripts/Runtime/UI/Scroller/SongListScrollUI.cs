using System;
using System.Collections.Generic;
using System.Linq;
using RGF.Game.BMS.Model;
using RGF.Game.Core.Data;
using UnityEngine;
using UnityEngine.EventSystems;
using Enum = RGF.Game.Common.Enum;

namespace RGF.UI.Scroller
{
	public class SongListScrollUI : UIBehaviour
	{
		protected const float m_padding = 10f;
		[SerializeField] protected SongUI itemPrototype = default;
		[SerializeField] protected int instantateItemCount = 9;
		[SerializeField] protected Enum.Direction direction = default;
		[SerializeField] protected RectTransform rectTransform = default;
		[SerializeField] protected float lerp = 0.1f;
		[SerializeField] private RectTransform target;
		protected int currentItemNo = 0;

		protected float diffPreFramePosition = 0;

		[NonSerialized] public LinkedList<SongUI> itemList = new LinkedList<SongUI>();

		private int selectedIndex = 0;
		private List<Song> songInfos;

		private List<SongUI> songs;

		protected float anchoredPosition => -rectTransform.anchoredPosition.y;
		private float height => itemPrototype.rectTransform.sizeDelta.y;


		protected override void Start()
		{
			itemPrototype.gameObject.SetActive(false);
			SetItems(Database.songInfoArray);
		}

		protected void Update()
		{
			if (itemList.First == null)
			{
				return;
			}

			while (anchoredPosition - diffPreFramePosition < -height * 2)
			{
				diffPreFramePosition -= height + m_padding;

				SongUI item = itemList.First.Value;
				itemList.RemoveFirst();
				itemList.AddLast(item);

				float pos = height * (instantateItemCount + 13) + height * currentItemNo
				                                                + m_padding * currentItemNo
					;

				item.rectTransform.anchoredPosition =
					direction == Enum.Direction.Vertical ? new Vector2(0, -pos) : new Vector2(pos, 0);

				currentItemNo++;
			}

			while (anchoredPosition - diffPreFramePosition > 0)
			{
				diffPreFramePosition += height + m_padding;

				SongUI item = itemList.Last.Value;
				itemList.RemoveLast();
				itemList.AddFirst(item);

				currentItemNo--;

				float pos = height * currentItemNo + m_padding * (currentItemNo - 1);

				item.rectTransform.anchoredPosition =
					direction == Enum.Direction.Vertical ? new Vector2(0, -pos) : new Vector2(pos, 0);
			}

			// 현재 위치
			float a = direction == Enum.Direction.Horizontal
				? rectTransform.anchoredPosition.x
				: rectTransform.anchoredPosition.y;

			// 예상 위치
			float b = (direction == Enum.Direction.Horizontal
				          ? target.anchoredPosition.x
				          : target.anchoredPosition.y) - 0.5f * height * (1 + selectedIndex * 2) -
			          m_padding * (selectedIndex - 1);

			float value = Mathf.Lerp(a, b, lerp);

			if (direction == Enum.Direction.Horizontal)
			{
				rectTransform.anchoredPosition = new Vector2(value, rectTransform.anchoredPosition.y);
			}
			else
			{
				rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, value);
			}
		}

		protected override void OnEnable()
		{
			base.OnEnable();

			Message.Register<int>(Event.OnChangeSong, OnChangeSong);
		}

		protected override void OnDisable()
		{
			base.OnDisable();

			Message.Unregister<int>(Event.OnChangeSong, OnChangeSong);
		}


		public void SetItems(Song[] infos)
		{
			songInfos = infos.ToList();

			List<IInfiniteScrollSetup> controllers =
				GetComponents<MonoBehaviour>()
					.OfType<IInfiniteScrollSetup>()
					.ToList();

			songs = new List<SongUI>();

			instantateItemCount = infos.Length;

			for (int i = 0; i < instantateItemCount; i++)
			{
				SongUI item = Instantiate(itemPrototype);
				item.rectTransform.SetParent(transform, false);
				item.name = i.ToString();
				item.rectTransform.anchoredPosition = direction == Enum.Direction.Vertical
					? new Vector2(0, -height * i + -m_padding * (i - 1))
					: new Vector2(height * i + m_padding * (i - 1), 0);
				itemList.AddLast(item);
				songs.Add(item);

				item.gameObject.SetActive(true);
				item.Initialize(infos[i]);


				foreach (IInfiniteScrollSetup controller in controllers)
				{
					controller.OnUpdateItem(i, item.gameObject);
				}
			}

			foreach (IInfiniteScrollSetup controller in controllers)
			{
				controller.OnPostSetupItems();
			}

			selectedIndex = 0;

			songs[selectedIndex].Select();
		}

		private void OnChangeSong(int offset)
		{
			int absIndex = songs.Count - selectedIndex;
			while (absIndex < 0)
			{
				absIndex += songs.Count;
			}

			absIndex %= songs.Count;
			songs[absIndex].Deselect();

			selectedIndex -= offset;

			absIndex = songs.Count - selectedIndex;
			while (absIndex < 0)
			{
				absIndex += songs.Count;
			}

			absIndex %= songs.Count;
			songs[absIndex].Select();
		}
	}
}
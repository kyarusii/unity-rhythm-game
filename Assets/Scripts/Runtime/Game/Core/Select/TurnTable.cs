using System;
using System.Collections;
using System.Collections.Generic;
using MEC;
using RGF.Game.BMS.Model;
using RGF.Game.Core.Data;
using RGF.Game.Utility;
using UnityEngine;
using Enum = RGF.Game.Common.Enum;

namespace RGF.Game.Core.Select
{
	public class TurnTable : MonoBehaviour
	{
		public static int selectedSongIndex = 0;
		public static int selectedPatternIndex = 0;

		public static bool canChangedSong;
		public static bool canChangedPattern;

		private bool isPreparing;

		private List<Song> songList = default;

		private void Start()
		{
			ImportSongs_Internal();

			selectedSongIndex = 0;
			selectedPatternIndex = 0;

			canChangedSong = true;
			canChangedPattern = true;

			// run at next frame
			Timing.CallDelayed(0.01f, () =>
			{
				ChangeSong(GameData.Inst.lastPlayedSongIndex);
				ChangePattern(GameData.Inst.lastPlayedPatternIndex);
			});
		}

		/// <summary>
		/// 빠른 곡 검색을 위한 키보드 단축키 초성 검색
		/// </summary>
		/// <param name="alphabet"></param>
		public void SearchSongStartWith(char alphabet)
		{
			string firstAlphabet = alphabet.ToString();

			for (int i = selectedSongIndex + 1; i < selectedSongIndex + songList.Count; i++)
			{
				int songIndex = i % songList.Count;
				Song song = songList[songIndex];

				// 곡 이름이 일치하는 알파벳으로 시작한다면
				if (song.songName.StartsWith(firstAlphabet, StringComparison.OrdinalIgnoreCase))
				{
					int offset = (songIndex - selectedSongIndex) % songList.Count;
					ChangeSong(offset);

					break;
				}
			}
		}

		public void OnNextSong()
		{
			if (canChangedSong)
			{
				ChangeSong(1);
			}
		}

		public void OnPrevSong()
		{
			if (canChangedSong)
			{
				ChangeSong(-1);
			}
		}

		private void ChangeSong(int offset)
		{
			selectedSongIndex += offset;

			selectedSongIndex += songList.Count;
			selectedSongIndex %= songList.Count;
			selectedPatternIndex = 0;

			Message.Execute<int>(Event.OnChangeSong, offset);
			Message.Execute<Song>(Event.OnSongChanged, songList[selectedSongIndex]);
			Message.Execute(Event.OnPatternChanged, selectedPatternIndex);
		}

		public void OnNextPattern()
		{
			if (canChangedPattern)
			{
				ChangePattern(1);
			}
		}

		public void OnPrevPattern()
		{
			if (canChangedPattern)
			{
				ChangePattern(-1);
			}
		}

		private void ChangePattern(int offset)
		{
			int headerCount = songList[selectedSongIndex].tracks.Count;

			selectedPatternIndex += offset;
			selectedPatternIndex += headerCount;
			selectedPatternIndex %= headerCount;

			Message.Execute(Event.OnPatternChanged, selectedPatternIndex);
		}

		public void OnOption()
		{
			Message.Execute(Event.OnToggleOption);
		}

		public void OnDecide()
		{
			GameStart();

			GameData.Inst.lastPlayedSongIndex = selectedSongIndex;
			GameData.Inst.lastPlayedPatternIndex = selectedPatternIndex;
			GameData.Inst.lastPlayedSong = songList[selectedSongIndex].songName;
			GameData.Inst.lastPlayedPattern = songList[selectedSongIndex].tracks[selectedPatternIndex].subtitle;
			GameData.Inst.Save();
		}

		private void OnEnable()
		{
			Message.Register<Song>(Event.OnSongPointerDown, OnSongPointerDown);
		}

#if UNITY_EDITOR && UNITY_2019_3_OR_NEWER
		/// <link>
		///     https://docs.unity3d.com/Manual/ScriptingRestrictions.html
		/// </link>
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
		private static void ReloadDomain()
		{
			selectedSongIndex = 0;
			selectedPatternIndex = 0;
		}
#endif

		private void ImportSongs_Internal()
		{
			songList = new List<Song>();
			foreach (Song songInfo in Database.songInfoArray)
			{
				songList.Add(songInfo);
			}
		}

		private void OnSongPointerDown(Song songInfo)
		{
			int index = songList.IndexOf(songInfo);
			int indexOffset = index - selectedSongIndex;
		}

		private void GameStart()
		{
			if (isPreparing)
			{
				return;
			}

			isPreparing = true;

			StartCoroutine(GameStartCoroutine());
		}

		private IEnumerator GameStartCoroutine()
		{
			GameService.Inst.song = songList[selectedSongIndex];
			GameService.Inst.track = songList[selectedSongIndex].tracks[selectedPatternIndex];

			string path = $"{GameService.Inst.track.RootPath}/{GameService.Inst.track.stageFile}";
			string uri = $"file://{path}";

			if (!Texture2DLibrary.Inst.HasObject(uri))
			{
				yield return Texture2DLibrary.Inst.LoadTexture2D(uri);
			}

			Texture2D tex = Texture2DLibrary.Inst.Find(uri);

			// bool complete = false;
			// Texture2D t = null;
			// TextureDownloader.Instance.GetTexture2D(path, texture =>
			// {
			// 	t = texture;
			// 	complete = true;
			// });
			//
			// yield return new WaitUntil(() => complete);

			SceneLoader.Change(Enum.Travel.Game);
		}

		public void OnBack() { }
	}
}
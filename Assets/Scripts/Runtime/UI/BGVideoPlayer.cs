using RGF.Game.BMS.Model;
using UnityEngine;
using UnityEngine.Video;

namespace RGF.UI
{
	public class BGVideoPlayer : MonoBehaviour
	{
		public VideoPlayer videoPlayer;

		private void Awake()
		{
			Message.Register<Song>(Event.OnSongChanged, OnSongSelectionChange);
		}

		private void OnDestroy()
		{
			Message.Unregister<Song>(Event.OnSongChanged, OnSongSelectionChange);
		}

		private void OnSongSelectionChange(Song song)
		{
			// if (videoPlayer.isPlaying)
			// {
			// 	videoPlayer.Stop();
			// }
			//
			// Assert.IsNotNull(song);
			// Assert.IsNotNull(song.Headers);
			//
			// var root = string.Empty;
			// var videoPath = string.Empty;
			// foreach (Header header in song.Headers)
			// {
			// 	Assert.IsNotNull(header);
			//
			// 	foreach (string key in header.bgVideoTable.Keys)
			// 	{
			// 		Debug.Log($"Key : {key}, Value : {header.bgVideoTable[key]}");
			// 		string value = header.bgVideoTable[key];
			// 		if (!string.IsNullOrWhiteSpace(value))
			// 		{
			// 			root = header.RootPath;
			// 			videoPath = value;
			// 			break;
			// 		}
			// 	}
			//
			// 	if (!string.IsNullOrEmpty(videoPath))
			// 	{
			// 		break;
			// 	}
			// }
			//
			// string uri = $"file://{root}\\{UnityWebRequest.EscapeURL(videoPath).Replace('+', ' ')}";
			// videoPlayer.url = $"file://{root}\\{videoPath}";
			// videoPlayer.Play();
		}
	}
}
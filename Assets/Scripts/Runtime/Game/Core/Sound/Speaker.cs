using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using RGF.Game.Core.Data;
using UnityEngine;
using UnityEngine.Networking;

namespace RGF.Game.Core.Sound
{
	public sealed class Speaker : MonoBehaviour
	{
		public GameObject KeySoundObject;
		public Audio audioComponent;

		[NonSerialized] public bool isPrepared = default;

		[NonSerialized] public Dictionary<int, string> wavNoteMap = default;
		[NonSerialized] public Dictionary<int, AudioClip> loadedAudioClipMap = default;

		private void Awake()
		{
			wavNoteMap = new Dictionary<int, string>();
			loadedAudioClipMap = new Dictionary<int, AudioClip>();
		}

		private void OnDestroy()
		{
			wavNoteMap.Clear();
			loadedAudioClipMap.Clear();
		}

		public void PrepareAudioClips()
		{
			StartCoroutine(PrepareAudioClipsCoroutine());
		}

		private IEnumerator PrepareAudioClipsCoroutine()
		{
			float beginTime = Time.realtimeSinceStartup;

			foreach (int key in wavNoteMap.Keys)
			{
				AudioType type;

				string path = wavNoteMap[key];
				if (path.Contains(".ogg"))
				{
					type = AudioType.OGGVORBIS;
					path = GetPath($"{path}.ogg");
				}
				else if (path.Contains(".mp3"))
				{
					type = AudioType.MPEG;
					path = GetPath($"{path}.mp3");
				}
				else if (path.Contains(".wav"))
				{
					type = AudioType.WAV;
					path = GetPath($"{path}.wav");
				}
				else
				{
					if (File.Exists($"{GameService.Inst.track.RootPath}\\{path}.ogg"))
					{
						type = AudioType.OGGVORBIS;
						path = GetPath($"{path}.ogg");
					}
					else if (File.Exists($"{GameService.Inst.track.RootPath}\\{path}.mp3"))
					{
						type = AudioType.MPEG;
						path = GetPath($"{path}.mp3");
					}
					else if (File.Exists($"{GameService.Inst.track.RootPath}\\{path}.wav"))
					{
						type = AudioType.WAV;
						path = GetPath($"{path}.wav");
					}
					else
					{
						Debug.LogError(path);
						throw new Exception("Not compatible audio type!");
					}
				}

				string GetPath(string originPath)
				{
					return
						$"{GameService.Inst.track.RootPath}\\{UnityWebRequest.EscapeURL(originPath).Replace('+', ' ')}";
				}

				string filePath = path;

				if (File.Exists(filePath))
				{
					if (!AudioLibrary.Inst.HasObject(filePath))
					{
						yield return AudioLibrary.Inst.LoadAudioClip(filePath, type);
					}

					loadedAudioClipMap.Add(key, AudioLibrary.Inst.Get(filePath));
				}
			}

			isPrepared = true;
			float estimated = Time.realtimeSinceStartup - beginTime;
			Debug.Log($"Audio Loading : {estimated:N2}s");
		}

		public void PlayKeySound(int key, float volume = 1.0f)
		{
			if (key == 0)
			{
				return;
			}

			if (loadedAudioClipMap.ContainsKey(key))
			{
				audioComponent.PlayOneShot(loadedAudioClipMap[key], volume);
			}
		}

		public void Pause()
		{
			audioComponent.Pause();
		}

		public void Resume()
		{
			audioComponent.Resume();
		}

		public IEnumerator WaitForAudioEnd()
		{
			while (true)
			{
				if (audioComponent.IsDone())
				{
					break;
				}

				yield return new WaitForSeconds(1.0f);
			}
		}
	}
}
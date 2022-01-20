using System.Collections.Generic;
using UnityEngine;

namespace RGF.Game.Core.Sound
{
	public class Audio : MonoBehaviour
	{
		[SerializeField] private int ChannelLength = default;

		private AudioSource[] _audioSources;
		private readonly List<AudioSource> paused = new List<AudioSource>();

		private void Awake()
		{
			_audioSources = new AudioSource[ChannelLength];

			for (int i = 0; i < ChannelLength; ++i)
			{
				_audioSources[i] = gameObject.AddComponent<AudioSource>();
				_audioSources[i].loop = false;
				_audioSources[i].playOnAwake = false;
			}
		}

		public void Play(AudioClip clip, float volume = 1.0f)
		{
			foreach (AudioSource a in _audioSources)
			{
				if (a.isPlaying)
				{
					continue;
				}

				a.clip = clip;
				a.volume = volume;
				a.Play();
				break;
			}
		}

		public void PlayOneShot(AudioClip clip, float volume = 1.0f)
		{
			foreach (AudioSource a in _audioSources)
			{
				if (a.isPlaying)
				{
					continue;
				}

				a.PlayOneShot(clip, volume);
				break;
			}
		}

		public bool IsDone()
		{
			foreach (AudioSource a in _audioSources)
			{
				if (a.isPlaying)
				{
					return false;
				}
			}

			return true;
		}

		public void Pause()
		{
			foreach (AudioSource a in _audioSources)
			{
				if (a.isPlaying)
				{
					a.Pause();
					paused.Add(a);
				}
			}
		}

		public void Resume()
		{
			foreach (AudioSource a in paused)
			{
				a.UnPause();
			}

			paused.Clear();
		}
	}
}
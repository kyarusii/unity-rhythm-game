using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace RGF.Game.Core.Data
{
	public class AudioLibrary : ObjectLibrary<AudioLibrary, AudioClip>
	{
		public IEnumerator LoadAudioClip(string path, AudioType type)
		{
			if (m_objectMap.ContainsKey(path))
			{
				yield break;
			}

			UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(path, type);
			yield return www.SendWebRequest();

			if (www.downloadHandler.data.Length > 0)
			{
				AudioClip clipContainer = DownloadHandlerAudioClip.GetContent(www);
				clipContainer.LoadAudioData();

				m_objectMap.Add(path, clipContainer);
			}
		}
	}
}
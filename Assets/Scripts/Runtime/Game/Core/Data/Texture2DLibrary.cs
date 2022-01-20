using System;
using System.Collections;
using System.IO;
using RGF.Game.BMP;
using UnityEngine;
using UnityEngine.Networking;

namespace RGF.Game.Core.Data
{
	public sealed class Texture2DLibrary : ObjectLibrary<Texture2DLibrary, Texture2D>
	{
		public IEnumerator LoadTexture2D(string uriPath)
		{
			string filePath;
			if (uriPath.StartsWith("file://"))
			{
				filePath = uriPath.Replace("file://", "");
			}
			else
			{
				filePath = uriPath;
				uriPath = $"file://{filePath}";
			}

			if (!File.Exists(filePath))
			{
				Debug.LogWarning($"텍스쳐 경로를 확인하세요 : {filePath}");
				yield break;
			}

			if (uriPath.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase))
			{
				yield return DownloadBMP(uriPath);
			}
			else if (uriPath.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
			{
				yield return DownloadPNG(uriPath);
			}
			else if (uriPath.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase))
			{
				yield return DownloadJPG(uriPath);
			}
			else
			{
				throw new Exception($"텍스처 포맷 불명 : {uriPath}");
			}
		}

		private IEnumerator DownloadBMP(string path)
		{
			UnityWebRequest www = UnityWebRequest.Get(path);

			yield return www.SendWebRequest();
			yield return new WaitUntil(() => www.isDone);

			BMPLoader loader = new BMPLoader();
			BMPImage img = loader.LoadBMP(www.downloadHandler.data, path);

			if (img != null)
			{
				Texture2D texture = img.ToTexture2D();
				m_objectMap.Add(path, texture);
			}
			else
			{
				Debug.LogError($"Load BMP Failed : {path}");
			}

			www?.Dispose();
		}

		private IEnumerator DownloadPNG(string path)
		{
			UnityWebRequest www = UnityWebRequestTexture.GetTexture(path);
			yield return www.SendWebRequest();

			Texture2D texture = (www.downloadHandler as DownloadHandlerTexture)?.texture;
			m_objectMap.Add(path, texture);

			www?.Dispose();
		}

		private IEnumerator DownloadJPG(string path)
		{
			UnityWebRequest www = UnityWebRequestTexture.GetTexture(path);
			yield return www.SendWebRequest();

			Texture2D texture = (www.downloadHandler as DownloadHandlerTexture)?.texture;
			m_objectMap.Add(path, texture);

			www?.Dispose();
		}
	}
}
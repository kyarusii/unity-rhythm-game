using System.Collections;
using Calci.CommandLine;
using RGF.Auth;
using RGF.Game.Common;
using RGF.Game.Core.Data;
using RGF.Game.Utility;
using UnityEngine;

namespace RGF.Game
{
	public sealed class Startup : MonoBehaviour
	{
		private void Awake()
		{
			int width = CommandLineParser.GetInt("Width", 1920);
			int height = CommandLineParser.GetInt("Height", 1080);
			bool isFullScreen = CommandLineParser.GetBool("FullScreen", false);

			Screen.SetResolution(width, height, isFullScreen);
		}

		private IEnumerator Start()
		{
			yield return new WaitForSeconds(.1f);

			Coroutine loadSongs = StartCoroutine(Database.Inst.Load());
			AuthPlatform auth = AuthUtil.Get();
			Coroutine authenticate = StartCoroutine(auth.Login());

			yield return loadSongs;
			yield return authenticate;

			auth.QueryStat();

			auth.IngestStat("TestStat", (int)10);

			SceneLoader.Change(Enum.Travel.Select);
		}
	}
}
using System;
using Calci.CommandLine;
using RGF.Game.BMS.Model;
using RGF.Game.Structure;
using RGF.Game.Structure.Data;
using UnityEngine;

namespace RGF.Game
{
	public sealed class GameService : MonoInstance<GameService>
	{
		[Header("Settings")] public int judgementSensitivity = default;

		public PlayOption option = default;

		[NonSerialized] public Track track = default;

		[NonSerialized] public bool isPaused;
		public bool isPlaying { get; set; }
		[NonSerialized] public bool isResuming;
		[NonSerialized] public float lastResumeTime;
		[NonSerialized] public int RestPauseCount = 3;

		[NonSerialized] private GameResult result = default;
		[NonSerialized] public Song song = default;

		public long StackedCombo { get; set; }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
		public bool autoPlay;
#endif
		public bool needAuth;

		public SettingObject Setting => SettingObject.Get();

		public void SetResult(GameResult gameResult)
		{
			result = gameResult;
			gameResult.Save();
		}

		public GameResult GetLastResult()
		{
			return result;
		}

		protected override void _OnAwake()
		{
			result = new GameResult();
			option = PlayOption.Load();
			GameData.Inst.Load();

#if UNITY_EDITOR || DEVELOPMENT_BUILD
			autoPlay = CommandLineParser.GetBool("AutoPlay", false);
#endif
		}

		public void GameStart() { }

		public void Exit()
		{
#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
#endif
			Application.Quit(0);
		}

		private void OnApplicationQuit()
		{
			GameData.Inst.Save();
		}
	}
}
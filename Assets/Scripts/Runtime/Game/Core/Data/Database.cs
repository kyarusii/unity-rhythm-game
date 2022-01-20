using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Calci.CommandLine;
using RGF.Game.BMS.Model;
using RGF.Game.Common;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
#if UNITY_EDITOR
#endif

namespace RGF.Game.Core.Data
{
	public class Database : MonoInstance<Database>
	{
		public static Song[] songInfoArray;

		public bool loadAsync;

		private string m_rootPath;
		private int patternCount = 0;

		private readonly CancellationTokenSource LoadCancelToken = new CancellationTokenSource();
		private Task LoadTask;

		public Action<string> LogAction { get; set; } = delegate(string s) { };
		public Action<float> ProgressAction { get; set; } = delegate(float f) { };

		// private bool authSuccess;

		protected override async void _OnAwake()
		{
// 			ParseArguments();
// 			Message.Register("AuthSuccess", OnAuth);
// 			
// 			if (loadAsync)
// 			{
// 				bool needLoad = true;
//
// #if UNITY_EDITOR
// 				if (EditorSettings.enterPlayModeOptionsEnabled)
// 				{
// 					if (songInfoArray != null && songInfoArray.Length > 0)
// 					{
// 						needLoad = false;
// 					}
// 				}
// #endif
// 				
// 				if (needLoad)
// 				{
// 					LogAction?.Invoke("로딩 시작");
// 					LoadTask = Task.Run(async () => await LoadAsync(), LoadCancelToken.Token);
//
// 					await Task.WhenAll(LoadTask);
// 				}
// 				
// 				LogAction?.Invoke("로딩 태스크 끝");
//
// 				Assert.IsNotNull(songInfoArray);
// 				StartCoroutine(LoadResources());
// 			}
// 			else
// 			{
// 				StartCoroutine(LoadCoroutine());
// 			}
//
// 			StartCoroutine(ChangeScene());
		}

		public IEnumerator Load()
		{
			ParseArguments();

			if (loadAsync)
			{
				bool needLoad = true;

#if UNITY_EDITOR
				if (EditorSettings.enterPlayModeOptionsEnabled)
				{
					if (songInfoArray != null && songInfoArray.Length > 0)
					{
						needLoad = false;
					}
				}
#endif

				if (needLoad)
				{
					LogAction?.Invoke("로딩 시작");
					LoadTask = Task.Run(async () => await LoadAsync(), LoadCancelToken.Token);

					while (!LoadTask.IsCompleted)
					{
						yield return new WaitForSeconds(0.1f);
					}
				}

				LogAction?.Invoke("로딩 태스크 끝");
				Assert.IsNotNull(songInfoArray);

				yield return LoadResources();
			}
			else
			{
				yield return LoadCoroutine();
			}

			yield return ChangeScene();
		}

		private void OnDestroy()
		{
			// Message.Unregister("AuthSuccess", OnAuth);

			if (loadAsync)
			{
				LoadCancelToken.Cancel();
			}
		}

		private void ParseArguments()
		{
			m_rootPath = CommandLineParser.GetString("ResourcePath");
		}

		private void OnAuth()
		{
			// authSuccess = true;
		}

		private IEnumerator ChangeScene()
		{
			if (GameService.Inst.needAuth)
			{
				// yield return new WaitUntil(() => authSuccess);
			}

			yield break;
		}

		private IEnumerator LoadResources()
		{
			foreach (Song song in songInfoArray)
			{
				foreach (Track header in song.tracks)
				{
					string stagePath = $"{header.RootPath}/{header.stageFile}";
					string fileLink = $"file://{stagePath}";

					if (File.Exists(stagePath))
					{
						yield return LoadImage(fileLink, header);
					}

					song.songName = header.SongName;
				}
			}

			ProgressAction?.Invoke(1.0f);
			LogAction?.Invoke("로딩 완료");
		}

		#region ASYNC

		private async Task LoadAsync()
		{
			string[] directories = Directory.GetDirectories(m_rootPath);
			int directoryCount = directories.Length;

			songInfoArray = new Song[directoryCount];
			for (int i = 0; i < directoryCount; ++i)
			{
				songInfoArray[i] = await Parse(directories[i]);
			}
		}

		private async Task<Song> Parse(string dir)
		{
			Song songinfo = new Song();

			string[] Files = Directory
				.EnumerateFiles(dir, "*.*", SearchOption.AllDirectories)
				.Where(Extension.IsBMSFile)
				.ToArray();

			if (Files.Length == 0)
			{
				return songinfo;
			}

			// 모든 파일에 대해 헤더 생성
			foreach (string filePath in Files)
			{
				Track currentTrack = new Track(filePath);

				bool result = await currentTrack.ReadAsync();
				patternCount++;

				if (result)
				{
					songinfo.tracks.Add(currentTrack);
				}
			}

			songinfo.tracks.Sort();
			return songinfo;
		}

		#endregion

		#region Coroutine

		private IEnumerator LoadCoroutine()
		{
			string[] directories = Directory.GetDirectories(m_rootPath);
			int directoryCount = directories.Length;

			float pointPerSong = 100f / directoryCount;

			songInfoArray = new Song[directoryCount];
			for (int i = 0; i < directoryCount; ++i)
			{
				Song songinfo = new Song();

				string[] Files = Directory
					.EnumerateFiles(directories[i], "*.*", SearchOption.AllDirectories)
					.Where(Extension.IsBMSFile)
					.ToArray();

				int fileCount = Files.Length;

				if (fileCount < 1)
				{
					yield break;
				}

				// 모든 파일에 대해 헤더 생성
				for (int j = 0; j < fileCount; j++)
				{
					string filePath = Files[j];
					Track currentTrack = new Track(filePath);

					bool result = currentTrack.ReadAll();
					patternCount++;

					if (result)
					{
						songinfo.tracks.Add(currentTrack);
					}

					ProgressAction(i * pointPerSong + pointPerSong * (j / (float)fileCount));
					LogAction(filePath);
				}

				songinfo.tracks.Sort();
				songInfoArray[i] = songinfo;
			}

			yield return LoadResources();
		}

		#endregion

		private IEnumerator LoadImage(string path, Track track)
		{
			if (!Texture2DLibrary.Inst.HasObject(path))
			{
				yield return Texture2DLibrary.Inst.LoadTexture2D(path);
			}

			track.stageTexture = Texture2DLibrary.Inst.Get(path);
		}

#if UNITY_EDITOR
		[MenuItem("Debug/Limit Song Count")]
		private static void LimitSongCount()
		{
			EditorPrefs.SetBool("Limit Song Count", true);
		}

		[MenuItem("Debug/Unlimit Song Count")]
		private static void UnlimitSongCount()
		{
			EditorPrefs.SetBool("Limit Song Count", false);
		}
#endif
	}
}
// This code is provided for demonstration purposes and is not intended to represent ideal practices.

using System;
using System.Collections;
using System.Collections.Generic;
using Epic.OnlineServices;
using Epic.OnlineServices.Auth;
using Epic.OnlineServices.Logging;
using Epic.OnlineServices.Platform;
using Epic.OnlineServices.Connect;
using Epic.OnlineServices.Stats;
using RGF;
using RGF.Auth;
using UnityEngine;
using Credentials = Epic.OnlineServices.Auth.Credentials;
using LoginCallbackInfo = Epic.OnlineServices.Auth.LoginCallbackInfo;
using LoginOptions = Epic.OnlineServices.Auth.LoginOptions;

public sealed class EOSAuthPlatform : AuthPlatform
{
	// Set these values as appropriate. For more information, see the Developer Portal documentation.
	public string m_ProductName = "MyUnityApplication";
	public string m_ProductVersion = "1.0";
	public string m_ProductId = "";
	public string m_SandboxId = "";
	public string m_DeploymentId = "";
	public string m_ClientId = "";
	public string m_ClientSecret = "";
	public LoginCredentialType m_LoginCredentialType = LoginCredentialType.AccountPortal;
	public string email;
	public string password;

	/// These fields correspond to <see cref="Epic.OnlineServices.Auth.Credentials.Id" /> and <see cref="Epic.OnlineServices.Auth.Credentials.Token" />,
	/// and their use differs based on the login type. For more information, see <see cref="Epic.OnlineServices.Auth.Credentials" />
	/// and the Auth Interface documentation.
	public string m_LoginCredentialId = null;

	public string m_LoginCredentialToken = null;
	public string m_EpicToken = null;

	private static PlatformInterface s_PlatformInterface;
	private const float c_PlatformTickInterval = 0.1f;
	private float m_PlatformTickTimer = 0f;

	public static PlatformInterface GetPlatformInterface()
	{
		return s_PlatformInterface;
	}

	// If we're in editor, we should dynamically load and unload the SDK between play sessions.
	// This allows us to initialize the SDK each time the game is run in editor.
#if UNITY_EDITOR
	private IntPtr m_LibraryPointer;
#endif

	public bool runInEditor;

	private EpicAccountId localUserId;
	private ProductUserId localProductUserId = null;

	protected override void Awake()
	{
		base.Awake();
		
#if UNITY_EDITOR
		if (!runInEditor) return;

		string libraryPath = "Assets/Plugins/EOS/" + Config.LibraryName;

		m_LibraryPointer = Native.Windows.Import.Kernel32.LoadLibrary(libraryPath);
		if (m_LibraryPointer == IntPtr.Zero)
		{
			throw new Exception("Failed to load library" + libraryPath);
		}

		Bindings.Hook(m_LibraryPointer, Native.Windows.Import.Kernel32.GetProcAddress);
#endif
	}

	private void OnApplicationQuit()
	{
		if (s_PlatformInterface != null)
		{
			s_PlatformInterface.Release();
			s_PlatformInterface = null;
			PlatformInterface.Shutdown();
		}

#if UNITY_EDITOR
		if (m_LibraryPointer != IntPtr.Zero)
		{
			Bindings.Unhook();

			// Free until the module ref count is 0
			while (Native.Windows.Import.Kernel32.FreeLibrary(m_LibraryPointer) != 0) { }

			m_LibraryPointer = IntPtr.Zero;
		}
#endif
	}

	public override IEnumerator Login()
	{
		bool finished = false;
		
		InitializeOptions initializeOptions = new InitializeOptions()
		{
			ProductName = m_ProductName,
			ProductVersion = m_ProductVersion
		};

		Result initializeResult = PlatformInterface.Initialize(initializeOptions);
		if (initializeResult != Result.Success)
		{
			throw new Exception("Failed to initialize platform: " + initializeResult);
		}

		// The SDK outputs lots of information that is useful for debugging.
		// Make sure to set up the logging interface as early as possible: after initializing.
		LoggingInterface.SetLogLevel(LogCategory.AllCategories, LogLevel.Error);
		LoggingInterface.SetCallback((LogMessage logMessage) => Debug.Log(logMessage.Message));
		
		// Ensure platform tick is called on an interval, or this will not callback.
		Options options = new Options()
		{
			ProductId = m_ProductId,
			SandboxId = m_SandboxId,
			DeploymentId = m_DeploymentId,
			ClientCredentials = new ClientCredentials()
			{
				ClientId = m_ClientId,
				ClientSecret = m_ClientSecret
			}
		};

		s_PlatformInterface = PlatformInterface.Create(options);
		if (s_PlatformInterface == null)
		{
			throw new Exception("Failed to create platform");
		}

		LoginOptions loginOptions = new LoginOptions()
		{
			Credentials = new Credentials()
			{
				Type = m_LoginCredentialType,
				Id = m_LoginCredentialId,
				Token = m_LoginCredentialToken,
			}
		};
		//
		// CreateDeviceIdOptions createDeviceIdOptions = new CreateDeviceIdOptions
		// {
		// };
		//
		// s_PlatformInterface.GetConnectInterface().CreateDeviceId(createDeviceIdOptions, null, data =>
		// {
		// 	if (data.ResultCode == Result.Success)
		// 	{
		// 		Debug.Log("CreateDeviceId Success");
		// 	}
		// });
		//
		// var connectLoginOption = new Epic.OnlineServices.Connect.LoginOptions()
		// {
		// 	Credentials = new Epic.OnlineServices.Connect.Credentials()
		// 	{
		// 		Type = ExternalCredentialType.DeviceidAccessToken,
		// 		Token = null
		// 	}, UserLoginInfo = new UserLoginInfo()
		// 	{
		// 		DisplayName = $"Guest-{SystemInfo.deviceUniqueIdentifier}".Substring(0, 32)
		// 	}
		// };
		//
		// Debug.Log(connectLoginOption.UserLoginInfo.DisplayName);
		//
		// s_PlatformInterface.GetConnectInterface().Login(connectLoginOption, null, data =>
		// {
		// 	if (data.ResultCode == Result.Success)
		// 	{
		// 		Debug.Log("Successs Device Id Login");
		// 		finished = true;
		//
		// 		localProductUserId = data.LocalUserId;
		// 		Debug.Log(data.LocalUserId);
		// 	}
		// 	else
		// 	{
		// 		Debug.LogError(data.ResultCode);
		// 	}
		// });
		//
		// yield return new WaitUntil(() => finished);
		// yield break;
		
		// Auth 로그인 - LocalUserId 가져오기
		s_PlatformInterface.GetAuthInterface().Login(loginOptions, null, (LoginCallbackInfo loginCallbackInfo) =>
		{
			if (loginCallbackInfo.ResultCode == Result.Success)
			{
				Debug.Log("Login succeeded");
			}
			else if (Common.IsOperationComplete(loginCallbackInfo.ResultCode))
			{
				Debug.Log("Login failed: " + loginCallbackInfo.ResultCode);
			}

			if (localUserId == null)
			{
				localUserId = (loginCallbackInfo.LocalUserId);
			}

			var copyUserTokenOptions = new Epic.OnlineServices.Auth.CopyUserAuthTokenOptions();
			s_PlatformInterface.GetAuthInterface()
				.CopyUserAuthToken(copyUserTokenOptions, localUserId, out var userToken);

			var account = new Epic.OnlineServices.Connect.LoginOptions()
			{
				Credentials = new Epic.OnlineServices.Connect.Credentials()
				{
					Token = userToken.AccessToken,
					Type = ExternalCredentialType.Epic
				}
			};

			// Connection 로그인 시도
			s_PlatformInterface.GetConnectInterface().Login(account, null,
				data =>
				{
					if (data.LocalUserId != null)
					{
						localProductUserId = data.LocalUserId;
					}

					// 성공
					if (data.ResultCode == Result.Success)
					{
						finished = true;
					}
					else if (data.ResultCode == Result.InvalidUser)
					{
						var createOptions = new CreateUserOptions();
						createOptions.ContinuanceToken = data.ContinuanceToken;

						s_PlatformInterface.GetConnectInterface().CreateUser(createOptions, null, info =>
						{
							if (info.ResultCode == Result.Success)
							{
								if (data.LocalUserId != null)
								{
									localProductUserId = data.LocalUserId;
								}
							}

							s_PlatformInterface.GetConnectInterface().Login(account, null, callbackInfo =>
							{
								if (data.ResultCode == Result.Success)
								{
									finished = true;
								}
							});
						});
					}
				});
		});

		yield return new WaitUntil(() => finished);
	}

	// Calling tick on a regular interval is required for callbacks to work.
	private void Update()
	{
		if (s_PlatformInterface != null)
		{
			m_PlatformTickTimer += Time.deltaTime;

			if (m_PlatformTickTimer >= c_PlatformTickInterval)
			{
				m_PlatformTickTimer = 0;
				s_PlatformInterface.Tick();
			}
		}
	}

	public override void IngestStat(string statName, int ingestAmount)
	{
		IngestData[] stats =
		{
			new IngestData()
			{
				StatName = statName,
				IngestAmount = ingestAmount
			}
		};

		IngestStatOptions options = new IngestStatOptions()
		{
			LocalUserId = localProductUserId,
			TargetUserId = localProductUserId,
			Stats = stats,
		};

		s_PlatformInterface.GetStatsInterface().IngestStat(options, null, (data =>
		{
			if (data == null)
			{
				Debug.LogError("Leaderboard (StatsIngestCallbackFn): data is null");
				return;
			}

			if (data.ResultCode != Result.Success)
			{
				Debug.LogErrorFormat("Leaderboard (StatsIngestCallbackFn): Ingest Stats error: {0}", data.ResultCode);
				return;
			}

			Debug.Log("Leaderboard (StatsIngestCallbackFn): Ingest Stats Complete");
		}));
	}

	public override void QueryStat()
	{
		QueryStatsOptions options = new QueryStatsOptions()
		{
			LocalUserId = localProductUserId,
			TargetUserId = localProductUserId,
		};

		s_PlatformInterface.GetStatsInterface().QueryStats(options, null, data =>
		{
			if (data == null)
			{
				Debug.LogError("QueryStats data is null");
				return;
			}

			if (data.ResultCode != Result.Success)
			{
				Debug.LogError($"QueryStats Not Success : {data.ResultCode}");
				return;
			}

			GetStatCountOptions getStatCountOptions = new GetStatCountOptions()
			{
				TargetUserId = localProductUserId,
			};

			uint statCount = s_PlatformInterface.GetStatsInterface().GetStatsCount(getStatCountOptions);
			
			Debug.Log($"Stat Count : {statCount}");

			var copyStatByIndexOption = new CopyStatByIndexOptions()
			{
				TargetUserId = localProductUserId,
				StatIndex = 0,
			};

			var collectedStats = new List<Stat>();
			
			for (uint i = 0; i < statCount; i++)
			{
				copyStatByIndexOption.StatIndex = i;

				var result = s_PlatformInterface.GetStatsInterface().CopyStatByIndex(copyStatByIndexOption, out Stat stat);
				if (result == Result.Success)
				{
					collectedStats.Add(stat);
				}
				else
				{
					Debug.Log($"GetStatByIndex Failed : {i}");
				}
			}

			foreach (Stat stat in collectedStats)
			{
				Debug.Log($"Loaded Stat : {stat.Name}. {stat.Value}");
			}
		});

	}
}
using System;
using System.Collections.Generic;
using Epic.OnlineServices.Auth;
using Epic.OnlineServices.Platform;
using UnityEngine;
using UnityEngine.UI;

public class EOSSDKComponent2 : MonoBehaviour
{
    public string m_productName = "MyCSharpApplication";
    public string m_productVersion = "1.0";
    public string m_clientId = "내 클라이언트 ID";
    public string m_clientSecret = "내 클라이언트 암호";
    public string m_productId = "내 제품 ID";
    public string m_sandboxId = "내 샌드박스 ID";
    public string m_deploymentId = "내 제품 ID";
    
    private PlatformInterface platformInterface;
    private Text text;
    
    // Start is called before the first frame update
    private void Start()
    {
        text = FindObjectOfType<Text>();
        text.text = string.Empty;
        
        Init();
        Login();
    }

    private void Init()
    {
        var initializeOptions = new Epic.OnlineServices.Platform.InitializeOptions();
        initializeOptions.ProductName = m_productName;
        initializeOptions.ProductVersion = m_productVersion;
        Epic.OnlineServices.Result result = Epic.OnlineServices.Platform.PlatformInterface.Initialize(initializeOptions);
        
        if (result != Epic.OnlineServices.Result.Success)
        {
            throw new Exception("플랫폼을 초기화하지 못함");
        }

        var clientCredentials = new Epic.OnlineServices.Platform.ClientCredentials();
        clientCredentials.ClientId = m_clientId;
        clientCredentials.ClientSecret = m_clientSecret;

        var options = new Epic.OnlineServices.Platform.Options();
        options.ClientCredentials = clientCredentials;
        options.ProductId = m_productId;
        options.SandboxId = m_sandboxId;
        options.DeploymentId = m_deploymentId;

        platformInterface = Epic.OnlineServices.Platform.PlatformInterface.Create(options);
        if (platformInterface == null)
        {
            throw new Exception("플랫폼을 생성하지 못함");
        }
        else
        {
            text.text += "플랫폼 인터페이스 생성\n";
        }
    }

    private void Login()
    {
        var authInterface = platformInterface.GetAuthInterface();
        if (authInterface == null)
        {
            throw new Exception("인증 인터페이스를 얻지 못함");
        }

        var credentials = new Epic.OnlineServices.Auth.Credentials();
        credentials.Type = LoginCredentialType.AccountPortal;

        var loginOptions = new Epic.OnlineServices.Auth.LoginOptions();
        loginOptions.Credentials = credentials;
        loginOptions.ScopeFlags = Epic.OnlineServices.Auth.AuthScopeFlags.BasicProfile
                                  // | Epic.OnlineServices.Auth.AuthScopeFlags.FriendsList | Epic.OnlineServices.Auth.AuthScopeFlags.Presence
                                  ;

        authInterface.Login(loginOptions, null, (Epic.OnlineServices.Auth.LoginCallbackInfo callbackInfo) =>
        {
            if (callbackInfo.ResultCode == Epic.OnlineServices.Result.Success)
            {
                // 로그인했습니다!
                actionQueue.Add(() =>
                {
                    text.text += "로그인 성공\n";
                });
            }
            else
            {
                actionQueue.Add(() =>
                {
                    text.text += $"{callbackInfo.ResultCode}\n";
                });
            }
        });
    }

    private List<Action> actionQueue = new List<Action>();

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            platformInterface?.Release();
            Epic.OnlineServices.Platform.PlatformInterface.Shutdown();
            
            Application.Quit(0);
        }
        
        platformInterface?.Tick();
        
        if (actionQueue.Count > 0)
        {
            foreach (Action action in actionQueue)
            {
                action();
            }
            
            actionQueue.Clear();
        }
    }
}
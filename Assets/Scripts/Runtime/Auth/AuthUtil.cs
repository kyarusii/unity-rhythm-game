using System;
using UnityEngine;

namespace RGF.Auth
{
	public static class AuthUtil
	{
		private static AuthPlatform authPlatform;

		public static void Init(AuthPlatform platform)
		{
			if (authPlatform != null)
			{
				throw new Exception("하나의 인증 플랫폼만 초기화되어야합니다.");
			}

			authPlatform = platform;
		}

		public static AuthPlatform Get()
		{
			return authPlatform;
		}

		public static void Release()
		{
			authPlatform = null;
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void ResetDomain()
		{
			Release();
		}
	}
}
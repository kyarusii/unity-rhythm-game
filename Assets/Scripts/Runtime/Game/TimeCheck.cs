using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace RGF.Game
{
	public sealed class TimeCheck
	{
		private static Stopwatch stopWatch;
		private static bool isStarted;

		public static void Start()
		{
			isStarted = true;
			stopWatch ??= new Stopwatch();

			stopWatch.Start();
		}

		public static void Pin(string message)
		{
			if (!isStarted)
			{
				Debug.LogError("Time checker was not started.");
				Start();
			}

			Debug.Log($"{stopWatch.ElapsedMilliseconds}ms::{message}");
			stopWatch.Restart();
		}

		public static void Log(string message)
		{
			Pin(message);
		}
	}
}
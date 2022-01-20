using System;
using System.Diagnostics;

namespace RGF.Game.Utility
{
	public class SafeEditorCall
	{
		[Conditional("UNITY_EDITOR")]
		public static void CallEditorOnly(Action action)
		{
			action.Invoke();
		}
	}
}
using System;
using System.Runtime.ExceptionServices;

namespace UniRx.InternalUtil
{
	internal static class ExceptionExtensions
	{
		public static void Throw(this Exception exception)
		{
#if (NET_4_6 || NET_STANDARD_2_0)
			ExceptionDispatchInfo.Capture(exception).Throw();
#endif
			throw exception;
		}
	}
}
using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UniRx.Diagnostics
{
	public struct LogEntry
	{
		// requires
		public string LoggerName { get; }
		public LogType LogType { get; }
		public string Message { get; }
		public DateTime Timestamp { get; }

		// options

		/// <summary>[Optional]</summary>
		public Object Context { get; }

		/// <summary>[Optional]</summary>
		public Exception Exception { get; }

		/// <summary>[Optional]</summary>
		public string StackTrace { get; }

		/// <summary>[Optional]</summary>
		public object State { get; }

		public LogEntry(string loggerName, LogType logType, DateTime timestamp, string message, Object context = null,
			Exception exception = null, string stackTrace = null, object state = null)
			: this()
		{
			LoggerName = loggerName;
			LogType = logType;
			Timestamp = timestamp;
			Message = message;
			Context = context;
			Exception = exception;
			StackTrace = stackTrace;
			State = state;
		}

		public override string ToString()
		{
			string plusEx = Exception != null ? Environment.NewLine + Exception : "";
			return "[" + Timestamp + "]"
			       + "[" + LoggerName + "]"
			       + "[" + LogType + "]"
			       + Message
			       + plusEx;
		}
	}
}
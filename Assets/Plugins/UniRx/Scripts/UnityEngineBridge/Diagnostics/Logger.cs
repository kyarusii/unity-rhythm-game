﻿using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UniRx.Diagnostics
{
	public class Logger
	{
		private static bool isInitialized = false;
		private static bool isDebugBuild = false;
		protected readonly Action<LogEntry> logPublisher;

		public Logger(string loggerName)
		{
			Name = loggerName;
			logPublisher = ObservableLogger.RegisterLogger(this);
		}

		public string Name { get; }

		/// <summary>Output LogType.Log but only enables isDebugBuild</summary>
		public virtual void Debug(object message, Object context = null)
		{
			if (!isInitialized)
			{
				isInitialized = true;
				isDebugBuild = UnityEngine.Debug.isDebugBuild;
			}

			if (isDebugBuild)
			{
				logPublisher(new LogEntry(
					message: message != null ? message.ToString() : "",
					logType: LogType.Log,
					timestamp: DateTime.Now,
					loggerName: Name,
					context: context));
			}
		}

		/// <summary>Output LogType.Log but only enables isDebugBuild</summary>
		public virtual void DebugFormat(string format, params object[] args)
		{
			if (!isInitialized)
			{
				isInitialized = true;
				isDebugBuild = UnityEngine.Debug.isDebugBuild;
			}

			if (isDebugBuild)
			{
				logPublisher(new LogEntry(
					message: format != null ? string.Format(format, args) : "",
					logType: LogType.Log,
					timestamp: DateTime.Now,
					loggerName: Name,
					context: null));
			}
		}

		public virtual void Log(object message, Object context = null)
		{
			logPublisher(new LogEntry(
				message: message != null ? message.ToString() : "",
				logType: LogType.Log,
				timestamp: DateTime.Now,
				loggerName: Name,
				context: context));
		}

		public virtual void LogFormat(string format, params object[] args)
		{
			logPublisher(new LogEntry(
				message: format != null ? string.Format(format, args) : "",
				logType: LogType.Log,
				timestamp: DateTime.Now,
				loggerName: Name,
				context: null));
		}

		public virtual void Warning(object message, Object context = null)
		{
			logPublisher(new LogEntry(
				message: message != null ? message.ToString() : "",
				logType: LogType.Warning,
				timestamp: DateTime.Now,
				loggerName: Name,
				context: context));
		}

		public virtual void WarningFormat(string format, params object[] args)
		{
			logPublisher(new LogEntry(
				message: format != null ? string.Format(format, args) : "",
				logType: LogType.Warning,
				timestamp: DateTime.Now,
				loggerName: Name,
				context: null));
		}

		public virtual void Error(object message, Object context = null)
		{
			logPublisher(new LogEntry(
				message: message != null ? message.ToString() : "",
				logType: LogType.Error,
				timestamp: DateTime.Now,
				loggerName: Name,
				context: context));
		}

		public virtual void ErrorFormat(string format, params object[] args)
		{
			logPublisher(new LogEntry(
				message: format != null ? string.Format(format, args) : "",
				logType: LogType.Error,
				timestamp: DateTime.Now,
				loggerName: Name,
				context: null));
		}

		public virtual void Exception(Exception exception, Object context = null)
		{
			logPublisher(new LogEntry(
				message: exception != null ? exception.ToString() : "",
				exception: exception,
				logType: LogType.Exception,
				timestamp: DateTime.Now,
				loggerName: Name,
				context: context));
		}

		/// <summary>Publish raw LogEntry.</summary>
		public virtual void Raw(LogEntry logEntry)
		{
			logPublisher(logEntry);
		}
	}
}
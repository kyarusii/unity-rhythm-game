using System;
using System.Threading;

namespace UniRx
{
	public static partial class Scheduler
	{
		public static readonly IScheduler Immediate = new ImmediateScheduler();

		private class ImmediateScheduler : IScheduler
		{
			public DateTimeOffset Now => Scheduler.Now;

			public IDisposable Schedule(Action action)
			{
				action();
				return Disposable.Empty;
			}

			public IDisposable Schedule(TimeSpan dueTime, Action action)
			{
				TimeSpan wait = Normalize(dueTime);
				if (wait.Ticks > 0)
				{
					Thread.Sleep(wait);
				}

				action();
				return Disposable.Empty;
			}
		}
	}
}
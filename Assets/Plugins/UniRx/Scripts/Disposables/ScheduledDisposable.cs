using System;
using System.Threading;

namespace UniRx
{
	public sealed class ScheduledDisposable : ICancelable
	{
		private volatile IDisposable disposable;
		private int isDisposed = 0;

		public ScheduledDisposable(IScheduler scheduler, IDisposable disposable)
		{
			this.Scheduler = scheduler;
			this.disposable = disposable;
		}

		public IScheduler Scheduler { get; }

		public IDisposable Disposable => disposable;

		public bool IsDisposed => isDisposed != 0;

		public void Dispose()
		{
			Scheduler.Schedule(DisposeInner);
		}

		private void DisposeInner()
		{
			if (Interlocked.Increment(ref isDisposed) == 1)
			{
				disposable.Dispose();
			}
		}
	}
}
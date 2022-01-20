﻿using System;

namespace UniRx
{
	// should be use Interlocked.CompareExchange for Threadsafe?
	// but CompareExchange cause ExecutionEngineException on iOS.
	// AOT...
	// use lock instead

	public sealed class SingleAssignmentDisposable : IDisposable, ICancelable
	{
		private readonly object gate = new object();
		private IDisposable current;
		private bool disposed;

		public IDisposable Disposable {
			get => current;
			set
			{
				IDisposable old = default(IDisposable);
				bool alreadyDisposed;
				lock (gate)
				{
					alreadyDisposed = disposed;
					old = current;
					if (!alreadyDisposed)
					{
						if (value == null)
						{
							return;
						}

						current = value;
					}
				}

				if (alreadyDisposed && value != null)
				{
					value.Dispose();
					return;
				}

				if (old != null)
				{
					throw new InvalidOperationException("Disposable is already set");
				}
			}
		}

		public bool IsDisposed {
			get
			{
				lock (gate)
				{
					return disposed;
				}
			}
		}


		public void Dispose()
		{
			IDisposable old = null;

			lock (gate)
			{
				if (!disposed)
				{
					disposed = true;
					old = current;
					current = null;
				}
			}

			if (old != null)
			{
				old.Dispose();
			}
		}
	}
}
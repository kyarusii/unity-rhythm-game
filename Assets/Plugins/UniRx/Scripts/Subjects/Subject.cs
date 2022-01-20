using System;
using UniRx.InternalUtil;

namespace UniRx
{
	public sealed class Subject<T> : ISubject<T>, IDisposable, IOptimizedObservable<T>
	{
		private bool isDisposed;

		private bool isStopped;
		private Exception lastError;
		private readonly object observerLock = new object();
		private IObserver<T> outObserver = EmptyObserver<T>.Instance;

		public bool HasObservers => !(outObserver is EmptyObserver<T>) && !isStopped && !isDisposed;

		public void Dispose()
		{
			lock (observerLock)
			{
				isDisposed = true;
				outObserver = DisposedObserver<T>.Instance;
			}
		}

		public bool IsRequiredSubscribeOnCurrentThread()
		{
			return false;
		}

		public void OnCompleted()
		{
			IObserver<T> old;
			lock (observerLock)
			{
				ThrowIfDisposed();
				if (isStopped)
				{
					return;
				}

				old = outObserver;
				outObserver = EmptyObserver<T>.Instance;
				isStopped = true;
			}

			old.OnCompleted();
		}

		public void OnError(Exception error)
		{
			if (error == null)
			{
				throw new ArgumentNullException("error");
			}

			IObserver<T> old;
			lock (observerLock)
			{
				ThrowIfDisposed();
				if (isStopped)
				{
					return;
				}

				old = outObserver;
				outObserver = EmptyObserver<T>.Instance;
				isStopped = true;
				lastError = error;
			}

			old.OnError(error);
		}

		public void OnNext(T value)
		{
			outObserver.OnNext(value);
		}

		public IDisposable Subscribe(IObserver<T> observer)
		{
			if (observer == null)
			{
				throw new ArgumentNullException("observer");
			}

			Exception ex = default(Exception);

			lock (observerLock)
			{
				ThrowIfDisposed();
				if (!isStopped)
				{
					ListObserver<T> listObserver = outObserver as ListObserver<T>;
					if (listObserver != null)
					{
						outObserver = listObserver.Add(observer);
					}
					else
					{
						IObserver<T> current = outObserver;
						if (current is EmptyObserver<T>)
						{
							outObserver = observer;
						}
						else
						{
							outObserver =
								new ListObserver<T>(new ImmutableList<IObserver<T>>(new[] {current, observer}));
						}
					}

					return new Subscription(this, observer);
				}

				ex = lastError;
			}

			if (ex != null)
			{
				observer.OnError(ex);
			}
			else
			{
				observer.OnCompleted();
			}

			return Disposable.Empty;
		}

		private void ThrowIfDisposed()
		{
			if (isDisposed)
			{
				throw new ObjectDisposedException("");
			}
		}

		private class Subscription : IDisposable
		{
			private readonly object gate = new object();
			private Subject<T> parent;
			private IObserver<T> unsubscribeTarget;

			public Subscription(Subject<T> parent, IObserver<T> unsubscribeTarget)
			{
				this.parent = parent;
				this.unsubscribeTarget = unsubscribeTarget;
			}

			public void Dispose()
			{
				lock (gate)
				{
					if (parent != null)
					{
						lock (parent.observerLock)
						{
							ListObserver<T> listObserver = parent.outObserver as ListObserver<T>;
							if (listObserver != null)
							{
								parent.outObserver = listObserver.Remove(unsubscribeTarget);
							}
							else
							{
								parent.outObserver = EmptyObserver<T>.Instance;
							}

							unsubscribeTarget = null;
							parent = null;
						}
					}
				}
			}
		}
	}
}
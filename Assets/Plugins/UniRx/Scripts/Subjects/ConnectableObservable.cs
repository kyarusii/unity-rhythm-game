using System;

namespace UniRx
{
	public interface IConnectableObservable<T> : IObservable<T>
	{
		IDisposable Connect();
	}

	public static partial class Observable
	{
		private class ConnectableObservable<T> : IConnectableObservable<T>
		{
			private readonly object gate = new object();
			private readonly IObservable<T> source;
			private readonly ISubject<T> subject;
			private Connection connection;

			public ConnectableObservable(IObservable<T> source, ISubject<T> subject)
			{
				this.source = source.AsObservable();
				this.subject = subject;
			}

			public IDisposable Connect()
			{
				lock (gate)
				{
					// don't subscribe twice
					if (connection == null)
					{
						IDisposable subscription = source.Subscribe(subject);
						connection = new Connection(this, subscription);
					}

					return connection;
				}
			}

			public IDisposable Subscribe(IObserver<T> observer)
			{
				return subject.Subscribe(observer);
			}

			private class Connection : IDisposable
			{
				private readonly ConnectableObservable<T> parent;
				private IDisposable subscription;

				public Connection(ConnectableObservable<T> parent, IDisposable subscription)
				{
					this.parent = parent;
					this.subscription = subscription;
				}

				public void Dispose()
				{
					lock (parent.gate)
					{
						if (subscription != null)
						{
							subscription.Dispose();
							subscription = null;
							parent.connection = null;
						}
					}
				}
			}
		}
	}
}
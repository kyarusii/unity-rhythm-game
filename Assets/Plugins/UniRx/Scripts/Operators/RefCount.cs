using System;

namespace UniRx.Operators
{
	internal class RefCountObservable<T> : OperatorObservableBase<T>
	{
		private readonly object gate = new object();
		private readonly IConnectableObservable<T> source;
		private IDisposable connection;
		private int refCount = 0;

		public RefCountObservable(IConnectableObservable<T> source)
			: base(source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
		}

		protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
		{
			return new RefCount(this, observer, cancel).Run();
		}

		private class RefCount : OperatorObserverBase<T, T>
		{
			private readonly RefCountObservable<T> parent;

			public RefCount(RefCountObservable<T> parent, IObserver<T> observer, IDisposable cancel) : base(observer,
				cancel)
			{
				this.parent = parent;
			}

			public IDisposable Run()
			{
				IDisposable subcription = parent.source.Subscribe(this);

				lock (parent.gate)
				{
					if (++parent.refCount == 1)
					{
						parent.connection = parent.source.Connect();
					}
				}

				return Disposable.Create(() =>
				{
					subcription.Dispose();

					lock (parent.gate)
					{
						if (--parent.refCount == 0)
						{
							parent.connection.Dispose();
						}
					}
				});
			}

			public override void OnNext(T value)
			{
				observer.OnNext(value);
			}

			public override void OnError(Exception error)
			{
				try
				{
					observer.OnError(error);
				}
				finally
				{
					Dispose();
				}
			}

			public override void OnCompleted()
			{
				try
				{
					observer.OnCompleted();
				}
				finally
				{
					Dispose();
				}
			}
		}
	}
}
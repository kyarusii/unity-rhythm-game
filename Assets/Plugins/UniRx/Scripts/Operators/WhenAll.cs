using System;
using System.Collections.Generic;

namespace UniRx.Operators
{
	internal class WhenAllObservable<T> : OperatorObservableBase<T[]>
	{
		private readonly IObservable<T>[] sources;
		private readonly IEnumerable<IObservable<T>> sourcesEnumerable;

		public WhenAllObservable(IObservable<T>[] sources)
			: base(false)
		{
			this.sources = sources;
		}

		public WhenAllObservable(IEnumerable<IObservable<T>> sources)
			: base(false)
		{
			sourcesEnumerable = sources;
		}

		protected override IDisposable SubscribeCore(IObserver<T[]> observer, IDisposable cancel)
		{
			if (sources != null)
			{
				return new WhenAll(sources, observer, cancel).Run();
			}

			IList<IObservable<T>> xs = sourcesEnumerable as IList<IObservable<T>>;
			if (xs == null)
			{
				xs = new List<IObservable<T>>(sourcesEnumerable); // materialize observables
			}

			return new WhenAll_(xs, observer, cancel).Run();
		}

		private class WhenAll : OperatorObserverBase<T[], T[]>
		{
			private readonly object gate = new object();
			private readonly IObservable<T>[] sources;
			private int completedCount;
			private int length;
			private T[] values;

			public WhenAll(IObservable<T>[] sources, IObserver<T[]> observer, IDisposable cancel)
				: base(observer, cancel)
			{
				this.sources = sources;
			}

			public IDisposable Run()
			{
				length = sources.Length;

				// fail safe...
				if (length == 0)
				{
					OnNext(new T[0]);
					try
					{
						observer.OnCompleted();
					}
					finally
					{
						Dispose();
					}

					return Disposable.Empty;
				}

				completedCount = 0;
				values = new T[length];

				IDisposable[] subscriptions = new IDisposable[length];
				for (int index = 0; index < length; index++)
				{
					IObservable<T> source = sources[index];
					WhenAllCollectionObserver observer = new WhenAllCollectionObserver(this, index);
					subscriptions[index] = source.Subscribe(observer);
				}

				return StableCompositeDisposable.CreateUnsafe(subscriptions);
			}

			public override void OnNext(T[] value)
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

			private class WhenAllCollectionObserver : IObserver<T>
			{
				private readonly int index;
				private readonly WhenAll parent;
				private bool isCompleted = false;

				public WhenAllCollectionObserver(WhenAll parent, int index)
				{
					this.parent = parent;
					this.index = index;
				}

				public void OnNext(T value)
				{
					lock (parent.gate)
					{
						if (!isCompleted)
						{
							parent.values[index] = value;
						}
					}
				}

				public void OnError(Exception error)
				{
					lock (parent.gate)
					{
						if (!isCompleted)
						{
							parent.OnError(error);
						}
					}
				}

				public void OnCompleted()
				{
					lock (parent.gate)
					{
						if (!isCompleted)
						{
							isCompleted = true;
							parent.completedCount++;
							if (parent.completedCount == parent.length)
							{
								parent.OnNext(parent.values);
								parent.OnCompleted();
							}
						}
					}
				}
			}
		}

		private class WhenAll_ : OperatorObserverBase<T[], T[]>
		{
			private readonly object gate = new object();
			private readonly IList<IObservable<T>> sources;
			private int completedCount;
			private int length;
			private T[] values;

			public WhenAll_(IList<IObservable<T>> sources, IObserver<T[]> observer, IDisposable cancel)
				: base(observer, cancel)
			{
				this.sources = sources;
			}

			public IDisposable Run()
			{
				length = sources.Count;

				// fail safe...
				if (length == 0)
				{
					OnNext(new T[0]);
					try
					{
						observer.OnCompleted();
					}
					finally
					{
						Dispose();
					}

					return Disposable.Empty;
				}

				completedCount = 0;
				values = new T[length];

				IDisposable[] subscriptions = new IDisposable[length];
				for (int index = 0; index < length; index++)
				{
					IObservable<T> source = sources[index];
					WhenAllCollectionObserver observer = new WhenAllCollectionObserver(this, index);
					subscriptions[index] = source.Subscribe(observer);
				}

				return StableCompositeDisposable.CreateUnsafe(subscriptions);
			}

			public override void OnNext(T[] value)
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

			private class WhenAllCollectionObserver : IObserver<T>
			{
				private readonly int index;
				private readonly WhenAll_ parent;
				private bool isCompleted = false;

				public WhenAllCollectionObserver(WhenAll_ parent, int index)
				{
					this.parent = parent;
					this.index = index;
				}

				public void OnNext(T value)
				{
					lock (parent.gate)
					{
						if (!isCompleted)
						{
							parent.values[index] = value;
						}
					}
				}

				public void OnError(Exception error)
				{
					lock (parent.gate)
					{
						if (!isCompleted)
						{
							parent.OnError(error);
						}
					}
				}

				public void OnCompleted()
				{
					lock (parent.gate)
					{
						if (!isCompleted)
						{
							isCompleted = true;
							parent.completedCount++;
							if (parent.completedCount == parent.length)
							{
								parent.OnNext(parent.values);
								parent.OnCompleted();
							}
						}
					}
				}
			}
		}
	}

	internal class WhenAllObservable : OperatorObservableBase<Unit>
	{
		private readonly IObservable<Unit>[] sources;
		private readonly IEnumerable<IObservable<Unit>> sourcesEnumerable;

		public WhenAllObservable(IObservable<Unit>[] sources)
			: base(false)
		{
			this.sources = sources;
		}

		public WhenAllObservable(IEnumerable<IObservable<Unit>> sources)
			: base(false)
		{
			sourcesEnumerable = sources;
		}

		protected override IDisposable SubscribeCore(IObserver<Unit> observer, IDisposable cancel)
		{
			if (sources != null)
			{
				return new WhenAll(sources, observer, cancel).Run();
			}

			IList<IObservable<Unit>> xs = sourcesEnumerable as IList<IObservable<Unit>>;
			if (xs == null)
			{
				xs = new List<IObservable<Unit>>(sourcesEnumerable); // materialize observables
			}

			return new WhenAll_(xs, observer, cancel).Run();
		}

		private class WhenAll : OperatorObserverBase<Unit, Unit>
		{
			private readonly object gate = new object();
			private readonly IObservable<Unit>[] sources;
			private int completedCount;
			private int length;

			public WhenAll(IObservable<Unit>[] sources, IObserver<Unit> observer, IDisposable cancel)
				: base(observer, cancel)
			{
				this.sources = sources;
			}

			public IDisposable Run()
			{
				length = sources.Length;

				// fail safe...
				if (length == 0)
				{
					OnNext(Unit.Default);
					try
					{
						observer.OnCompleted();
					}
					finally
					{
						Dispose();
					}

					return Disposable.Empty;
				}

				completedCount = 0;

				IDisposable[] subscriptions = new IDisposable[length];
				for (int index = 0; index < sources.Length; index++)
				{
					IObservable<Unit> source = sources[index];
					WhenAllCollectionObserver observer = new WhenAllCollectionObserver(this);
					subscriptions[index] = source.Subscribe(observer);
				}

				return StableCompositeDisposable.CreateUnsafe(subscriptions);
			}

			public override void OnNext(Unit value)
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

			private class WhenAllCollectionObserver : IObserver<Unit>
			{
				private readonly WhenAll parent;
				private bool isCompleted = false;

				public WhenAllCollectionObserver(WhenAll parent)
				{
					this.parent = parent;
				}

				public void OnNext(Unit value) { }

				public void OnError(Exception error)
				{
					lock (parent.gate)
					{
						if (!isCompleted)
						{
							parent.OnError(error);
						}
					}
				}

				public void OnCompleted()
				{
					lock (parent.gate)
					{
						if (!isCompleted)
						{
							isCompleted = true;
							parent.completedCount++;
							if (parent.completedCount == parent.length)
							{
								parent.OnNext(Unit.Default);
								parent.OnCompleted();
							}
						}
					}
				}
			}
		}

		private class WhenAll_ : OperatorObserverBase<Unit, Unit>
		{
			private readonly object gate = new object();
			private readonly IList<IObservable<Unit>> sources;
			private int completedCount;
			private int length;

			public WhenAll_(IList<IObservable<Unit>> sources, IObserver<Unit> observer, IDisposable cancel)
				: base(observer, cancel)
			{
				this.sources = sources;
			}

			public IDisposable Run()
			{
				length = sources.Count;

				// fail safe...
				if (length == 0)
				{
					OnNext(Unit.Default);
					try
					{
						observer.OnCompleted();
					}
					finally
					{
						Dispose();
					}

					return Disposable.Empty;
				}

				completedCount = 0;

				IDisposable[] subscriptions = new IDisposable[length];
				for (int index = 0; index < length; index++)
				{
					IObservable<Unit> source = sources[index];
					WhenAllCollectionObserver observer = new WhenAllCollectionObserver(this);
					subscriptions[index] = source.Subscribe(observer);
				}

				return StableCompositeDisposable.CreateUnsafe(subscriptions);
			}

			public override void OnNext(Unit value)
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

			private class WhenAllCollectionObserver : IObserver<Unit>
			{
				private readonly WhenAll_ parent;
				private bool isCompleted = false;

				public WhenAllCollectionObserver(WhenAll_ parent)
				{
					this.parent = parent;
				}

				public void OnNext(Unit value) { }

				public void OnError(Exception error)
				{
					lock (parent.gate)
					{
						if (!isCompleted)
						{
							parent.OnError(error);
						}
					}
				}

				public void OnCompleted()
				{
					lock (parent.gate)
					{
						if (!isCompleted)
						{
							isCompleted = true;
							parent.completedCount++;
							if (parent.completedCount == parent.length)
							{
								parent.OnNext(Unit.Default);
								parent.OnCompleted();
							}
						}
					}
				}
			}
		}
	}
}
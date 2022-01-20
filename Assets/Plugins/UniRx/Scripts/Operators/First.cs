using System;

namespace UniRx.Operators
{
	internal class FirstObservable<T> : OperatorObservableBase<T>
	{
		private readonly Func<T, bool> predicate;
		private readonly IObservable<T> source;
		private readonly bool useDefault;

		public FirstObservable(IObservable<T> source, bool useDefault)
			: base(source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			this.useDefault = useDefault;
		}

		public FirstObservable(IObservable<T> source, Func<T, bool> predicate, bool useDefault)
			: base(source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			this.predicate = predicate;
			this.useDefault = useDefault;
		}

		protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
		{
			if (predicate == null)
			{
				return source.Subscribe(new First(this, observer, cancel));
			}

			return source.Subscribe(new First_(this, observer, cancel));
		}

		private class First : OperatorObserverBase<T, T>
		{
			private readonly FirstObservable<T> parent;
			private bool notPublished;

			public First(FirstObservable<T> parent, IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
			{
				this.parent = parent;
				notPublished = true;
			}

			public override void OnNext(T value)
			{
				if (notPublished)
				{
					notPublished = false;
					observer.OnNext(value);
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
				if (parent.useDefault)
				{
					if (notPublished)
					{
						observer.OnNext(default);
					}

					try
					{
						observer.OnCompleted();
					}
					finally
					{
						Dispose();
					}
				}
				else
				{
					if (notPublished)
					{
						try
						{
							observer.OnError(new InvalidOperationException("sequence is empty"));
						}
						finally
						{
							Dispose();
						}
					}
					else
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

		// with predicate
		private class First_ : OperatorObserverBase<T, T>
		{
			private readonly FirstObservable<T> parent;
			private bool notPublished;

			public First_(FirstObservable<T> parent, IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
			{
				this.parent = parent;
				notPublished = true;
			}

			public override void OnNext(T value)
			{
				if (notPublished)
				{
					bool isPassed;
					try
					{
						isPassed = parent.predicate(value);
					}
					catch (Exception ex)
					{
						try
						{
							observer.OnError(ex);
						}
						finally
						{
							Dispose();
						}

						return;
					}

					if (isPassed)
					{
						notPublished = false;
						observer.OnNext(value);
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
				if (parent.useDefault)
				{
					if (notPublished)
					{
						observer.OnNext(default);
					}

					try
					{
						observer.OnCompleted();
					}
					finally
					{
						Dispose();
					}
				}
				else
				{
					if (notPublished)
					{
						try
						{
							observer.OnError(new InvalidOperationException("sequence is empty"));
						}
						finally
						{
							Dispose();
						}
					}
					else
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
	}
}
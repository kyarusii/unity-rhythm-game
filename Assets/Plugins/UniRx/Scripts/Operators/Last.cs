using System;

namespace UniRx.Operators
{
	internal class LastObservable<T> : OperatorObservableBase<T>
	{
		private readonly Func<T, bool> predicate;
		private readonly IObservable<T> source;
		private readonly bool useDefault;

		public LastObservable(IObservable<T> source, bool useDefault)
			: base(source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			this.useDefault = useDefault;
		}

		public LastObservable(IObservable<T> source, Func<T, bool> predicate, bool useDefault)
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
				return source.Subscribe(new Last(this, observer, cancel));
			}

			return source.Subscribe(new Last_(this, observer, cancel));
		}

		private class Last : OperatorObserverBase<T, T>
		{
			private readonly LastObservable<T> parent;
			private T lastValue;
			private bool notPublished;

			public Last(LastObservable<T> parent, IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
			{
				this.parent = parent;
				notPublished = true;
			}

			public override void OnNext(T value)
			{
				notPublished = false;
				lastValue = value;
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
					else
					{
						observer.OnNext(lastValue);
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
						observer.OnNext(lastValue);
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

		private class Last_ : OperatorObserverBase<T, T>
		{
			private readonly LastObservable<T> parent;
			private T lastValue;
			private bool notPublished;

			public Last_(LastObservable<T> parent, IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
			{
				this.parent = parent;
				notPublished = true;
			}

			public override void OnNext(T value)
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
					lastValue = value;
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
					else
					{
						observer.OnNext(lastValue);
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
						observer.OnNext(lastValue);
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
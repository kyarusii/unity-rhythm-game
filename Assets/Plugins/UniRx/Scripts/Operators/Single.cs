using System;

namespace UniRx.Operators
{
	internal class SingleObservable<T> : OperatorObservableBase<T>
	{
		private readonly Func<T, bool> predicate;
		private readonly IObservable<T> source;
		private readonly bool useDefault;

		public SingleObservable(IObservable<T> source, bool useDefault)
			: base(source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			this.useDefault = useDefault;
		}

		public SingleObservable(IObservable<T> source, Func<T, bool> predicate, bool useDefault)
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
				return source.Subscribe(new Single(this, observer, cancel));
			}

			return source.Subscribe(new Single_(this, observer, cancel));
		}

		private class Single : OperatorObserverBase<T, T>
		{
			private readonly SingleObservable<T> parent;
			private T lastValue;
			private bool seenValue;

			public Single(SingleObservable<T> parent, IObserver<T> observer, IDisposable cancel) : base(observer,
				cancel)
			{
				this.parent = parent;
				seenValue = false;
			}

			public override void OnNext(T value)
			{
				if (seenValue)
				{
					try
					{
						observer.OnError(new InvalidOperationException("sequence is not single"));
					}
					finally
					{
						Dispose();
					}
				}
				else
				{
					seenValue = true;
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
					if (!seenValue)
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
					if (!seenValue)
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

		private class Single_ : OperatorObserverBase<T, T>
		{
			private readonly SingleObservable<T> parent;
			private T lastValue;
			private bool seenValue;

			public Single_(SingleObservable<T> parent, IObserver<T> observer, IDisposable cancel) : base(observer,
				cancel)
			{
				this.parent = parent;
				seenValue = false;
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
					if (seenValue)
					{
						try
						{
							observer.OnError(new InvalidOperationException("sequence is not single"));
						}
						finally
						{
							Dispose();
						}
					}
					else
					{
						seenValue = true;
						lastValue = value;
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
					if (!seenValue)
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
					if (!seenValue)
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
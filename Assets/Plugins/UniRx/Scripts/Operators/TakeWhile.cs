using System;

namespace UniRx.Operators
{
	internal class TakeWhileObservable<T> : OperatorObservableBase<T>
	{
		private readonly Func<T, bool> predicate;
		private readonly Func<T, int, bool> predicateWithIndex;
		private readonly IObservable<T> source;

		public TakeWhileObservable(IObservable<T> source, Func<T, bool> predicate)
			: base(source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			this.predicate = predicate;
		}

		public TakeWhileObservable(IObservable<T> source, Func<T, int, bool> predicateWithIndex)
			: base(source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			this.predicateWithIndex = predicateWithIndex;
		}

		protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
		{
			if (predicate != null)
			{
				return new TakeWhile(this, observer, cancel).Run();
			}

			return new TakeWhile_(this, observer, cancel).Run();
		}

		private class TakeWhile : OperatorObserverBase<T, T>
		{
			private readonly TakeWhileObservable<T> parent;

			public TakeWhile(TakeWhileObservable<T> parent, IObserver<T> observer, IDisposable cancel) : base(observer,
				cancel)
			{
				this.parent = parent;
			}

			public IDisposable Run()
			{
				return parent.source.Subscribe(this);
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
					observer.OnNext(value);
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

		private class TakeWhile_ : OperatorObserverBase<T, T>
		{
			private readonly TakeWhileObservable<T> parent;
			private int index = 0;

			public TakeWhile_(TakeWhileObservable<T> parent, IObserver<T> observer, IDisposable cancel) : base(observer,
				cancel)
			{
				this.parent = parent;
			}

			public IDisposable Run()
			{
				return parent.source.Subscribe(this);
			}

			public override void OnNext(T value)
			{
				bool isPassed;
				try
				{
					isPassed = parent.predicateWithIndex(value, index++);
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
					observer.OnNext(value);
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
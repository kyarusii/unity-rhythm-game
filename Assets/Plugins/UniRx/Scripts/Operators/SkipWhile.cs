using System;

namespace UniRx.Operators
{
	internal class SkipWhileObservable<T> : OperatorObservableBase<T>
	{
		private readonly Func<T, bool> predicate;
		private readonly Func<T, int, bool> predicateWithIndex;
		private readonly IObservable<T> source;

		public SkipWhileObservable(IObservable<T> source, Func<T, bool> predicate)
			: base(source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			this.predicate = predicate;
		}

		public SkipWhileObservable(IObservable<T> source, Func<T, int, bool> predicateWithIndex)
			: base(source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			this.predicateWithIndex = predicateWithIndex;
		}

		protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
		{
			if (predicate != null)
			{
				return new SkipWhile(this, observer, cancel).Run();
			}

			return new SkipWhile_(this, observer, cancel).Run();
		}

		private class SkipWhile : OperatorObserverBase<T, T>
		{
			private readonly SkipWhileObservable<T> parent;
			private bool endSkip = false;

			public SkipWhile(SkipWhileObservable<T> parent, IObserver<T> observer, IDisposable cancel) : base(observer,
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
				if (!endSkip)
				{
					try
					{
						endSkip = !parent.predicate(value);
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

					if (!endSkip)
					{
						return;
					}
				}

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

		private class SkipWhile_ : OperatorObserverBase<T, T>
		{
			private readonly SkipWhileObservable<T> parent;
			private bool endSkip = false;
			private int index = 0;

			public SkipWhile_(SkipWhileObservable<T> parent, IObserver<T> observer, IDisposable cancel) : base(observer,
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
				if (!endSkip)
				{
					try
					{
						endSkip = !parent.predicateWithIndex(value, index++);
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

					if (!endSkip)
					{
						return;
					}
				}

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
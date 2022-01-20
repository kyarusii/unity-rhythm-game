using System;

namespace UniRx.Operators
{
	internal class WhereObservable<T> : OperatorObservableBase<T>
	{
		private readonly Func<T, bool> predicate;
		private readonly Func<T, int, bool> predicateWithIndex;
		private readonly IObservable<T> source;

		public WhereObservable(IObservable<T> source, Func<T, bool> predicate)
			: base(source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			this.predicate = predicate;
		}

		public WhereObservable(IObservable<T> source, Func<T, int, bool> predicateWithIndex)
			: base(source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			this.predicateWithIndex = predicateWithIndex;
		}

		// Optimize for .Where().Where()

		public IObservable<T> CombinePredicate(Func<T, bool> combinePredicate)
		{
			if (predicate != null)
			{
				return new WhereObservable<T>(source, x => predicate(x) && combinePredicate(x));
			}

			return new WhereObservable<T>(this, combinePredicate);
		}

		// Optimize for .Where().Select()

		public IObservable<TR> CombineSelector<TR>(Func<T, TR> selector)
		{
			if (predicate != null)
			{
				return new WhereSelectObservable<T, TR>(source, predicate, selector);
			}

			return new SelectObservable<T, TR>(this, selector); // can't combine
		}

		protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
		{
			if (predicate != null)
			{
				return source.Subscribe(new Where(this, observer, cancel));
			}

			return source.Subscribe(new Where_(this, observer, cancel));
		}

		private class Where : OperatorObserverBase<T, T>
		{
			private readonly WhereObservable<T> parent;

			public Where(WhereObservable<T> parent, IObserver<T> observer, IDisposable cancel)
				: base(observer, cancel)
			{
				this.parent = parent;
			}

			public override void OnNext(T value)
			{
				bool isPassed = false;
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

		private class Where_ : OperatorObserverBase<T, T>
		{
			private readonly WhereObservable<T> parent;
			private int index;

			public Where_(WhereObservable<T> parent, IObserver<T> observer, IDisposable cancel)
				: base(observer, cancel)
			{
				this.parent = parent;
				index = 0;
			}

			public override void OnNext(T value)
			{
				bool isPassed = false;
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
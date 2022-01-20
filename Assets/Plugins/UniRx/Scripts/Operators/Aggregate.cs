using System;

namespace UniRx.Operators
{
	internal class AggregateObservable<TSource> : OperatorObservableBase<TSource>
	{
		private readonly Func<TSource, TSource, TSource> accumulator;
		private readonly IObservable<TSource> source;

		public AggregateObservable(IObservable<TSource> source, Func<TSource, TSource, TSource> accumulator)
			: base(source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			this.accumulator = accumulator;
		}

		protected override IDisposable SubscribeCore(IObserver<TSource> observer, IDisposable cancel)
		{
			return source.Subscribe(new Aggregate(this, observer, cancel));
		}

		private class Aggregate : OperatorObserverBase<TSource, TSource>
		{
			private readonly AggregateObservable<TSource> parent;
			private TSource accumulation;
			private bool seenValue;

			public Aggregate(AggregateObservable<TSource> parent, IObserver<TSource> observer, IDisposable cancel) :
				base(observer, cancel)
			{
				this.parent = parent;
				seenValue = false;
			}

			public override void OnNext(TSource value)
			{
				if (!seenValue)
				{
					seenValue = true;
					accumulation = value;
				}
				else
				{
					try
					{
						accumulation = parent.accumulator(accumulation, value);
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
				if (!seenValue)
				{
					throw new InvalidOperationException("Sequence contains no elements.");
				}

				observer.OnNext(accumulation);
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

	internal class AggregateObservable<TSource, TAccumulate> : OperatorObservableBase<TAccumulate>
	{
		private readonly Func<TAccumulate, TSource, TAccumulate> accumulator;
		private readonly TAccumulate seed;
		private readonly IObservable<TSource> source;

		public AggregateObservable(IObservable<TSource> source, TAccumulate seed,
			Func<TAccumulate, TSource, TAccumulate> accumulator)
			: base(source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			this.seed = seed;
			this.accumulator = accumulator;
		}

		protected override IDisposable SubscribeCore(IObserver<TAccumulate> observer, IDisposable cancel)
		{
			return source.Subscribe(new Aggregate(this, observer, cancel));
		}

		private class Aggregate : OperatorObserverBase<TSource, TAccumulate>
		{
			private readonly AggregateObservable<TSource, TAccumulate> parent;
			private TAccumulate accumulation;

			public Aggregate(AggregateObservable<TSource, TAccumulate> parent, IObserver<TAccumulate> observer,
				IDisposable cancel) : base(observer, cancel)
			{
				this.parent = parent;
				accumulation = parent.seed;
			}

			public override void OnNext(TSource value)
			{
				try
				{
					accumulation = parent.accumulator(accumulation, value);
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
				observer.OnNext(accumulation);
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

	internal class AggregateObservable<TSource, TAccumulate, TResult> : OperatorObservableBase<TResult>
	{
		private readonly Func<TAccumulate, TSource, TAccumulate> accumulator;
		private readonly Func<TAccumulate, TResult> resultSelector;
		private readonly TAccumulate seed;
		private readonly IObservable<TSource> source;

		public AggregateObservable(IObservable<TSource> source, TAccumulate seed,
			Func<TAccumulate, TSource, TAccumulate> accumulator, Func<TAccumulate, TResult> resultSelector)
			: base(source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			this.seed = seed;
			this.accumulator = accumulator;
			this.resultSelector = resultSelector;
		}

		protected override IDisposable SubscribeCore(IObserver<TResult> observer, IDisposable cancel)
		{
			return source.Subscribe(new Aggregate(this, observer, cancel));
		}

		private class Aggregate : OperatorObserverBase<TSource, TResult>
		{
			private readonly AggregateObservable<TSource, TAccumulate, TResult> parent;
			private TAccumulate accumulation;

			public Aggregate(AggregateObservable<TSource, TAccumulate, TResult> parent, IObserver<TResult> observer,
				IDisposable cancel) : base(observer, cancel)
			{
				this.parent = parent;
				accumulation = parent.seed;
			}

			public override void OnNext(TSource value)
			{
				try
				{
					accumulation = parent.accumulator(accumulation, value);
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
				TResult result;
				try
				{
					result = parent.resultSelector(accumulation);
				}
				catch (Exception ex)
				{
					OnError(ex);
					return;
				}

				observer.OnNext(result);
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
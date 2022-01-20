using System;

namespace UniRx.Operators
{
	internal class ForEachAsyncObservable<T> : OperatorObservableBase<Unit>
	{
		private readonly Action<T> onNext;
		private readonly Action<T, int> onNextWithIndex;
		private readonly IObservable<T> source;

		public ForEachAsyncObservable(IObservable<T> source, Action<T> onNext)
			: base(source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			this.onNext = onNext;
		}

		public ForEachAsyncObservable(IObservable<T> source, Action<T, int> onNext)
			: base(source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			onNextWithIndex = onNext;
		}

		protected override IDisposable SubscribeCore(IObserver<Unit> observer, IDisposable cancel)
		{
			if (onNext != null)
			{
				return source.Subscribe(new ForEachAsync(this, observer, cancel));
			}

			return source.Subscribe(new ForEachAsync_(this, observer, cancel));
		}

		private class ForEachAsync : OperatorObserverBase<T, Unit>
		{
			private readonly ForEachAsyncObservable<T> parent;

			public ForEachAsync(ForEachAsyncObservable<T> parent, IObserver<Unit> observer, IDisposable cancel) : base(
				observer, cancel)
			{
				this.parent = parent;
			}

			public override void OnNext(T value)
			{
				try
				{
					parent.onNext(value);
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
				observer.OnNext(Unit.Default);

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

		// with index
		private class ForEachAsync_ : OperatorObserverBase<T, Unit>
		{
			private readonly ForEachAsyncObservable<T> parent;
			private int index = 0;

			public ForEachAsync_(ForEachAsyncObservable<T> parent, IObserver<Unit> observer, IDisposable cancel) : base(
				observer, cancel)
			{
				this.parent = parent;
			}

			public override void OnNext(T value)
			{
				try
				{
					parent.onNextWithIndex(value, index++);
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
				observer.OnNext(Unit.Default);

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
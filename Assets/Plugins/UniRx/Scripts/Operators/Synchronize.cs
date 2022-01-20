using System;

namespace UniRx.Operators
{
	internal class SynchronizeObservable<T> : OperatorObservableBase<T>
	{
		private readonly object gate;
		private readonly IObservable<T> source;

		public SynchronizeObservable(IObservable<T> source, object gate)
			: base(source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			this.gate = gate;
		}

		protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
		{
			return source.Subscribe(new Synchronize(this, observer, cancel));
		}

		private class Synchronize : OperatorObserverBase<T, T>
		{
			private readonly SynchronizeObservable<T> parent;

			public Synchronize(SynchronizeObservable<T> parent, IObserver<T> observer, IDisposable cancel) : base(
				observer, cancel)
			{
				this.parent = parent;
			}

			public override void OnNext(T value)
			{
				lock (parent.gate)
				{
					observer.OnNext(value);
				}
			}

			public override void OnError(Exception error)
			{
				lock (parent.gate)
				{
					try
					{
						observer.OnError(error);
					}
					finally
					{
						Dispose();
					}

					;
				}
			}

			public override void OnCompleted()
			{
				lock (parent.gate)
				{
					try
					{
						observer.OnCompleted();
					}
					finally
					{
						Dispose();
					}

					;
				}
			}
		}
	}
}
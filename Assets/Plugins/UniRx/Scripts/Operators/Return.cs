using System;

namespace UniRx.Operators
{
	internal class ReturnObservable<T> : OperatorObservableBase<T>
	{
		private readonly IScheduler scheduler;
		private readonly T value;

		public ReturnObservable(T value, IScheduler scheduler)
			: base(scheduler == Scheduler.CurrentThread)
		{
			this.value = value;
			this.scheduler = scheduler;
		}

		protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
		{
			observer = new Return(observer, cancel);

			if (scheduler == Scheduler.Immediate)
			{
				observer.OnNext(value);
				observer.OnCompleted();
				return Disposable.Empty;
			}

			return scheduler.Schedule(() =>
			{
				observer.OnNext(value);
				observer.OnCompleted();
			});
		}

		private class Return : OperatorObserverBase<T, T>
		{
			public Return(IObserver<T> observer, IDisposable cancel)
				: base(observer, cancel) { }

			public override void OnNext(T value)
			{
				try
				{
					observer.OnNext(value);
				}
				catch
				{
					Dispose();
					throw;
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

	internal class ImmediateReturnObservable<T> : IObservable<T>, IOptimizedObservable<T>
	{
		private readonly T value;

		public ImmediateReturnObservable(T value)
		{
			this.value = value;
		}

		public IDisposable Subscribe(IObserver<T> observer)
		{
			observer.OnNext(value);
			observer.OnCompleted();
			return Disposable.Empty;
		}

		public bool IsRequiredSubscribeOnCurrentThread()
		{
			return false;
		}
	}

	internal class ImmutableReturnUnitObservable : IObservable<Unit>, IOptimizedObservable<Unit>
	{
		internal static ImmutableReturnUnitObservable Instance = new ImmutableReturnUnitObservable();

		private ImmutableReturnUnitObservable() { }

		public IDisposable Subscribe(IObserver<Unit> observer)
		{
			observer.OnNext(Unit.Default);
			observer.OnCompleted();
			return Disposable.Empty;
		}

		public bool IsRequiredSubscribeOnCurrentThread()
		{
			return false;
		}
	}

	internal class ImmutableReturnTrueObservable : IObservable<bool>, IOptimizedObservable<bool>
	{
		internal static ImmutableReturnTrueObservable Instance = new ImmutableReturnTrueObservable();

		private ImmutableReturnTrueObservable() { }

		public IDisposable Subscribe(IObserver<bool> observer)
		{
			observer.OnNext(true);
			observer.OnCompleted();
			return Disposable.Empty;
		}

		public bool IsRequiredSubscribeOnCurrentThread()
		{
			return false;
		}
	}

	internal class ImmutableReturnFalseObservable : IObservable<bool>, IOptimizedObservable<bool>
	{
		internal static ImmutableReturnFalseObservable Instance = new ImmutableReturnFalseObservable();

		private ImmutableReturnFalseObservable() { }

		public IDisposable Subscribe(IObserver<bool> observer)
		{
			observer.OnNext(false);
			observer.OnCompleted();
			return Disposable.Empty;
		}

		public bool IsRequiredSubscribeOnCurrentThread()
		{
			return false;
		}
	}

	internal class ImmutableReturnInt32Observable : IObservable<int>, IOptimizedObservable<int>
	{
		private static readonly ImmutableReturnInt32Observable[] Caches =
		{
			new ImmutableReturnInt32Observable(-1),
			new ImmutableReturnInt32Observable(0),
			new ImmutableReturnInt32Observable(1),
			new ImmutableReturnInt32Observable(2),
			new ImmutableReturnInt32Observable(3),
			new ImmutableReturnInt32Observable(4),
			new ImmutableReturnInt32Observable(5),
			new ImmutableReturnInt32Observable(6),
			new ImmutableReturnInt32Observable(7),
			new ImmutableReturnInt32Observable(8),
			new ImmutableReturnInt32Observable(9)
		};

		private readonly int x;

		private ImmutableReturnInt32Observable(int x)
		{
			this.x = x;
		}

		public IDisposable Subscribe(IObserver<int> observer)
		{
			observer.OnNext(x);
			observer.OnCompleted();
			return Disposable.Empty;
		}

		public bool IsRequiredSubscribeOnCurrentThread()
		{
			return false;
		}

		public static IObservable<int> GetInt32Observable(int x)
		{
			if (-1 <= x && x <= 9)
			{
				return Caches[x + 1];
			}

			return new ImmediateReturnObservable<int>(x);
		}
	}
}
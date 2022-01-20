using System;
using System.Collections.Generic;

namespace UniRx.Operators
{
	internal class BufferObservable<T> : OperatorObservableBase<IList<T>>
	{
		private readonly int count;
		private readonly IScheduler scheduler;
		private readonly int skip;
		private readonly IObservable<T> source;
		private readonly TimeSpan timeShift;

		private readonly TimeSpan timeSpan;

		public BufferObservable(IObservable<T> source, int count, int skip)
			: base(source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			this.count = count;
			this.skip = skip;
		}

		public BufferObservable(IObservable<T> source, TimeSpan timeSpan, TimeSpan timeShift, IScheduler scheduler)
			: base(scheduler == Scheduler.CurrentThread || source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			this.timeSpan = timeSpan;
			this.timeShift = timeShift;
			this.scheduler = scheduler;
		}

		public BufferObservable(IObservable<T> source, TimeSpan timeSpan, int count, IScheduler scheduler)
			: base(scheduler == Scheduler.CurrentThread || source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			this.timeSpan = timeSpan;
			this.count = count;
			this.scheduler = scheduler;
		}

		protected override IDisposable SubscribeCore(IObserver<IList<T>> observer, IDisposable cancel)
		{
			// count,skip
			if (scheduler == null)
			{
				if (skip == 0)
				{
					return new Buffer(this, observer, cancel).Run();
				}

				return new Buffer_(this, observer, cancel).Run();
			}

			// time + count
			if (count > 0)
			{
				return new BufferTC(this, observer, cancel).Run();
			}

			if (timeSpan == timeShift)
			{
				return new BufferT(this, observer, cancel).Run();
			}

			return new BufferTS(this, observer, cancel).Run();
		}

		// count only
		private class Buffer : OperatorObserverBase<T, IList<T>>
		{
			private readonly BufferObservable<T> parent;
			private List<T> list;

			public Buffer(BufferObservable<T> parent, IObserver<IList<T>> observer, IDisposable cancel) : base(observer,
				cancel)
			{
				this.parent = parent;
			}

			public IDisposable Run()
			{
				list = new List<T>(parent.count);
				return parent.source.Subscribe(this);
			}

			public override void OnNext(T value)
			{
				list.Add(value);
				if (list.Count == parent.count)
				{
					observer.OnNext(list);
					list = new List<T>(parent.count);
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
				if (list.Count > 0)
				{
					observer.OnNext(list);
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
		}

		// count and skip
		private class Buffer_ : OperatorObserverBase<T, IList<T>>
		{
			private readonly BufferObservable<T> parent;
			private int index;
			private Queue<List<T>> q;

			public Buffer_(BufferObservable<T> parent, IObserver<IList<T>> observer, IDisposable cancel) : base(
				observer, cancel)
			{
				this.parent = parent;
			}

			public IDisposable Run()
			{
				q = new Queue<List<T>>();
				index = -1;
				return parent.source.Subscribe(this);
			}

			public override void OnNext(T value)
			{
				index++;

				if (index % parent.skip == 0)
				{
					q.Enqueue(new List<T>(parent.count));
				}

				int len = q.Count;
				for (int i = 0; i < len; i++)
				{
					List<T> list = q.Dequeue();
					list.Add(value);
					if (list.Count == parent.count)
					{
						observer.OnNext(list);
					}
					else
					{
						q.Enqueue(list);
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
				foreach (List<T> list in q)
				{
					observer.OnNext(list);
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
		}

		// timespan = timeshift
		private class BufferT : OperatorObserverBase<T, IList<T>>
		{
			private static readonly T[] EmptyArray = new T[0];
			private readonly object gate = new object();

			private readonly BufferObservable<T> parent;

			private List<T> list;

			public BufferT(BufferObservable<T> parent, IObserver<IList<T>> observer, IDisposable cancel) : base(
				observer, cancel)
			{
				this.parent = parent;
			}

			public IDisposable Run()
			{
				list = new List<T>();

				IDisposable timerSubscription = Observable.Interval(parent.timeSpan, parent.scheduler)
					.Subscribe(new Buffer(this));

				IDisposable sourceSubscription = parent.source.Subscribe(this);

				return StableCompositeDisposable.Create(timerSubscription, sourceSubscription);
			}

			public override void OnNext(T value)
			{
				lock (gate)
				{
					list.Add(value);
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
				List<T> currentList;
				lock (gate)
				{
					currentList = list;
				}

				observer.OnNext(currentList);
				try
				{
					observer.OnCompleted();
				}
				finally
				{
					Dispose();
				}
			}

			private class Buffer : IObserver<long>
			{
				private readonly BufferT parent;

				public Buffer(BufferT parent)
				{
					this.parent = parent;
				}

				public void OnNext(long value)
				{
					bool isZero = false;
					List<T> currentList;
					lock (parent.gate)
					{
						currentList = parent.list;
						if (currentList.Count != 0)
						{
							parent.list = new List<T>();
						}
						else
						{
							isZero = true;
						}
					}

					parent.observer.OnNext(isZero ? (IList<T>) EmptyArray : currentList);
				}

				public void OnError(Exception error) { }

				public void OnCompleted() { }
			}
		}

		// timespan + timeshift
		private class BufferTS : OperatorObserverBase<T, IList<T>>
		{
			private readonly object gate = new object();
			private readonly BufferObservable<T> parent;
			private TimeSpan nextShift;
			private TimeSpan nextSpan;

			private Queue<IList<T>> q;
			private SerialDisposable timerD;
			private TimeSpan totalTime;

			public BufferTS(BufferObservable<T> parent, IObserver<IList<T>> observer, IDisposable cancel) : base(
				observer, cancel)
			{
				this.parent = parent;
			}

			public IDisposable Run()
			{
				totalTime = TimeSpan.Zero;
				nextShift = parent.timeShift;
				nextSpan = parent.timeSpan;

				q = new Queue<IList<T>>();

				timerD = new SerialDisposable();
				q.Enqueue(new List<T>());
				CreateTimer();

				IDisposable subscription = parent.source.Subscribe(this);

				return StableCompositeDisposable.Create(subscription, timerD);
			}

			private void CreateTimer()
			{
				SingleAssignmentDisposable m = new SingleAssignmentDisposable();
				timerD.Disposable = m;

				bool isSpan = false;
				bool isShift = false;
				if (nextSpan == nextShift)
				{
					isSpan = true;
					isShift = true;
				}
				else if (nextSpan < nextShift)
				{
					isSpan = true;
				}
				else
				{
					isShift = true;
				}

				TimeSpan newTotalTime = isSpan ? nextSpan : nextShift;
				TimeSpan ts = newTotalTime - totalTime;
				totalTime = newTotalTime;

				if (isSpan)
				{
					nextSpan += parent.timeShift;
				}

				if (isShift)
				{
					nextShift += parent.timeShift;
				}

				m.Disposable = parent.scheduler.Schedule(ts, () =>
				{
					lock (gate)
					{
						if (isShift)
						{
							List<T> s = new List<T>();
							q.Enqueue(s);
						}

						if (isSpan)
						{
							IList<T> s = q.Dequeue();
							observer.OnNext(s);
						}
					}

					CreateTimer();
				});
			}

			public override void OnNext(T value)
			{
				lock (gate)
				{
					foreach (IList<T> s in q)
					{
						s.Add(value);
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
				lock (gate)
				{
					foreach (IList<T> list in q)
					{
						observer.OnNext(list);
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
			}
		}

		// timespan + count
		private class BufferTC : OperatorObserverBase<T, IList<T>>
		{
			private static readonly T[] EmptyArray = new T[0]; // cache
			private readonly object gate = new object();

			private readonly BufferObservable<T> parent;

			private List<T> list;
			private SerialDisposable timerD;
			private long timerId;

			public BufferTC(BufferObservable<T> parent, IObserver<IList<T>> observer, IDisposable cancel) : base(
				observer, cancel)
			{
				this.parent = parent;
			}

			public IDisposable Run()
			{
				list = new List<T>();
				timerId = 0L;
				timerD = new SerialDisposable();

				CreateTimer();
				IDisposable subscription = parent.source.Subscribe(this);

				return StableCompositeDisposable.Create(subscription, timerD);
			}

			private void CreateTimer()
			{
				long currentTimerId = timerId;
				SingleAssignmentDisposable timerS = new SingleAssignmentDisposable();
				timerD.Disposable = timerS; // restart timer(dispose before)


				ISchedulerPeriodic periodicScheduler = parent.scheduler as ISchedulerPeriodic;
				if (periodicScheduler != null)
				{
					timerS.Disposable =
						periodicScheduler.SchedulePeriodic(parent.timeSpan, () => OnNextTick(currentTimerId));
				}
				else
				{
					timerS.Disposable =
						parent.scheduler.Schedule(parent.timeSpan, self => OnNextRecursive(currentTimerId, self));
				}
			}

			private void OnNextTick(long currentTimerId)
			{
				bool isZero = false;
				List<T> currentList;
				lock (gate)
				{
					if (currentTimerId != timerId)
					{
						return;
					}

					currentList = list;
					if (currentList.Count != 0)
					{
						list = new List<T>();
					}
					else
					{
						isZero = true;
					}
				}

				observer.OnNext(isZero ? (IList<T>) EmptyArray : currentList);
			}

			private void OnNextRecursive(long currentTimerId, Action<TimeSpan> self)
			{
				bool isZero = false;
				List<T> currentList;
				lock (gate)
				{
					if (currentTimerId != timerId)
					{
						return;
					}

					currentList = list;
					if (currentList.Count != 0)
					{
						list = new List<T>();
					}
					else
					{
						isZero = true;
					}
				}

				observer.OnNext(isZero ? (IList<T>) EmptyArray : currentList);
				self(parent.timeSpan);
			}

			public override void OnNext(T value)
			{
				List<T> currentList = null;
				lock (gate)
				{
					list.Add(value);
					if (list.Count == parent.count)
					{
						currentList = list;
						list = new List<T>();
						timerId++;
						CreateTimer();
					}
				}

				if (currentList != null)
				{
					observer.OnNext(currentList);
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
				List<T> currentList;
				lock (gate)
				{
					timerId++;
					currentList = list;
				}

				observer.OnNext(currentList);
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

	internal class BufferObservable<TSource, TWindowBoundary> : OperatorObservableBase<IList<TSource>>
	{
		private readonly IObservable<TSource> source;
		private readonly IObservable<TWindowBoundary> windowBoundaries;

		public BufferObservable(IObservable<TSource> source, IObservable<TWindowBoundary> windowBoundaries)
			: base(source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			this.windowBoundaries = windowBoundaries;
		}

		protected override IDisposable SubscribeCore(IObserver<IList<TSource>> observer, IDisposable cancel)
		{
			return new Buffer(this, observer, cancel).Run();
		}

		private class Buffer : OperatorObserverBase<TSource, IList<TSource>>
		{
			private static readonly TSource[] EmptyArray = new TSource[0]; // cache

			private readonly BufferObservable<TSource, TWindowBoundary> parent;
			private readonly object gate = new object();
			private List<TSource> list;

			public Buffer(BufferObservable<TSource, TWindowBoundary> parent, IObserver<IList<TSource>> observer,
				IDisposable cancel) : base(observer, cancel)
			{
				this.parent = parent;
			}

			public IDisposable Run()
			{
				list = new List<TSource>();

				IDisposable sourceSubscription = parent.source.Subscribe(this);
				IDisposable windowSubscription = parent.windowBoundaries.Subscribe(new Buffer_(this));

				return StableCompositeDisposable.Create(sourceSubscription, windowSubscription);
			}

			public override void OnNext(TSource value)
			{
				lock (gate)
				{
					list.Add(value);
				}
			}

			public override void OnError(Exception error)
			{
				lock (gate)
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
			}

			public override void OnCompleted()
			{
				lock (gate)
				{
					List<TSource> currentList = list;
					list = new List<TSource>(); // safe
					observer.OnNext(currentList);
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

			private class Buffer_ : IObserver<TWindowBoundary>
			{
				private readonly Buffer parent;

				public Buffer_(Buffer parent)
				{
					this.parent = parent;
				}

				public void OnNext(TWindowBoundary value)
				{
					bool isZero = false;
					List<TSource> currentList;
					lock (parent.gate)
					{
						currentList = parent.list;
						if (currentList.Count != 0)
						{
							parent.list = new List<TSource>();
						}
						else
						{
							isZero = true;
						}
					}

					if (isZero)
					{
						parent.observer.OnNext(EmptyArray);
					}
					else
					{
						parent.observer.OnNext(currentList);
					}
				}

				public void OnError(Exception error)
				{
					parent.OnError(error);
				}

				public void OnCompleted()
				{
					parent.OnCompleted();
				}
			}
		}
	}
}
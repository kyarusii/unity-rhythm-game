using System;
using System.Collections.Generic;

namespace UniRx.Operators
{
	internal class TakeLastObservable<T> : OperatorObservableBase<T>
	{
		// count
		private readonly int count;

		// duration
		private readonly TimeSpan duration;
		private readonly IScheduler scheduler;
		private readonly IObservable<T> source;

		public TakeLastObservable(IObservable<T> source, int count)
			: base(source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			this.count = count;
		}

		public TakeLastObservable(IObservable<T> source, TimeSpan duration, IScheduler scheduler)
			: base(scheduler == Scheduler.CurrentThread || source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			this.duration = duration;
			this.scheduler = scheduler;
		}

		protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
		{
			if (scheduler == null)
			{
				return new TakeLast(this, observer, cancel).Run();
			}

			return new TakeLast_(this, observer, cancel).Run();
		}

		// count
		private class TakeLast : OperatorObserverBase<T, T>
		{
			private readonly TakeLastObservable<T> parent;
			private readonly Queue<T> q;

			public TakeLast(TakeLastObservable<T> parent, IObserver<T> observer, IDisposable cancel) : base(observer,
				cancel)
			{
				this.parent = parent;
				q = new Queue<T>();
			}

			public IDisposable Run()
			{
				return parent.source.Subscribe(this);
			}

			public override void OnNext(T value)
			{
				q.Enqueue(value);
				if (q.Count > parent.count)
				{
					q.Dequeue();
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
				foreach (T item in q)
				{
					observer.OnNext(item);
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

		// time
		private class TakeLast_ : OperatorObserverBase<T, T>
		{
			private readonly TakeLastObservable<T> parent;
			private readonly Queue<TimeInterval<T>> q;
			private DateTimeOffset startTime;

			public TakeLast_(TakeLastObservable<T> parent, IObserver<T> observer, IDisposable cancel) : base(observer,
				cancel)
			{
				this.parent = parent;
				q = new Queue<TimeInterval<T>>();
			}

			public IDisposable Run()
			{
				startTime = parent.scheduler.Now;
				return parent.source.Subscribe(this);
			}

			public override void OnNext(T value)
			{
				DateTimeOffset now = parent.scheduler.Now;
				TimeSpan elapsed = now - startTime;
				q.Enqueue(new TimeInterval<T>(value, elapsed));
				Trim(elapsed);
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

				;
			}

			public override void OnCompleted()
			{
				DateTimeOffset now = parent.scheduler.Now;
				TimeSpan elapsed = now - startTime;
				Trim(elapsed);

				foreach (TimeInterval<T> item in q)
				{
					observer.OnNext(item.Value);
				}

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

			private void Trim(TimeSpan now)
			{
				while (q.Count > 0 && now - q.Peek().Interval >= parent.duration)
				{
					q.Dequeue();
				}
			}
		}
	}
}
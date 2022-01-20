﻿using System;
using System.Collections.Generic;

namespace UniRx.Operators
{
	// needs to more improvement

	internal class ConcatObservable<T> : OperatorObservableBase<T>
	{
		private readonly IEnumerable<IObservable<T>> sources;

		public ConcatObservable(IEnumerable<IObservable<T>> sources)
			: base(true)
		{
			this.sources = sources;
		}

		public IObservable<T> Combine(IEnumerable<IObservable<T>> combineSources)
		{
			return new ConcatObservable<T>(CombineSources(sources, combineSources));
		}

		private static IEnumerable<IObservable<T>> CombineSources(IEnumerable<IObservable<T>> first,
			IEnumerable<IObservable<T>> second)
		{
			foreach (IObservable<T> item in first)
			{
				yield return item;
			}

			foreach (IObservable<T> item in second)
			{
				yield return item;
			}
		}

		protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
		{
			return new Concat(this, observer, cancel).Run();
		}

		private class Concat : OperatorObserverBase<T, T>
		{
			private readonly object gate = new object();
			private readonly ConcatObservable<T> parent;
			private IEnumerator<IObservable<T>> e;

			private bool isDisposed;
			private Action nextSelf;
			private SerialDisposable subscription;

			public Concat(ConcatObservable<T> parent, IObserver<T> observer, IDisposable cancel)
				: base(observer, cancel)
			{
				this.parent = parent;
			}

			public IDisposable Run()
			{
				isDisposed = false;
				e = parent.sources.GetEnumerator();
				subscription = new SerialDisposable();

				IDisposable schedule = Scheduler.DefaultSchedulers.TailRecursion.Schedule(RecursiveRun);

				return StableCompositeDisposable.Create(schedule, subscription, Disposable.Create(() =>
				{
					lock (gate)
					{
						isDisposed = true;
						e.Dispose();
					}
				}));
			}

			private void RecursiveRun(Action self)
			{
				lock (gate)
				{
					nextSelf = self;
					if (isDisposed)
					{
						return;
					}

					IObservable<T> current = default(IObservable<T>);
					bool hasNext = false;
					Exception ex = default(Exception);

					try
					{
						hasNext = e.MoveNext();
						if (hasNext)
						{
							current = e.Current;
							if (current == null)
							{
								throw new InvalidOperationException("sequence is null.");
							}
						}
						else
						{
							e.Dispose();
						}
					}
					catch (Exception exception)
					{
						ex = exception;
						e.Dispose();
					}

					if (ex != null)
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

					if (!hasNext)
					{
						try
						{
							observer.OnCompleted();
						}
						finally
						{
							Dispose();
						}

						return;
					}

					IObservable<T> source = current;
					SingleAssignmentDisposable d = new SingleAssignmentDisposable();
					subscription.Disposable = d;
					d.Disposable = source.Subscribe(this);
				}
			}

			public override void OnNext(T value)
			{
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
				nextSelf();
			}
		}
	}
}
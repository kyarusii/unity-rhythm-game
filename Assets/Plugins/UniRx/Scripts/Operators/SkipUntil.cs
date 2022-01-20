using System;
using UniRx.InternalUtil;

namespace UniRx.Operators
{
	internal class SkipUntilObservable<T, TOther> : OperatorObservableBase<T>
	{
		private readonly IObservable<TOther> other;
		private readonly IObservable<T> source;

		public SkipUntilObservable(IObservable<T> source, IObservable<TOther> other)
			: base(source.IsRequiredSubscribeOnCurrentThread() || other.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			this.other = other;
		}

		protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
		{
			return new SkipUntilOuterObserver(this, observer, cancel).Run();
		}

		private class SkipUntilOuterObserver : OperatorObserverBase<T, T>
		{
			private readonly SkipUntilObservable<T, TOther> parent;

			public SkipUntilOuterObserver(SkipUntilObservable<T, TOther> parent, IObserver<T> observer,
				IDisposable cancel) : base(observer, cancel)
			{
				this.parent = parent;
			}

			public IDisposable Run()
			{
				SingleAssignmentDisposable sourceSubscription = new SingleAssignmentDisposable();
				SkipUntil sourceObserver = new SkipUntil(this, sourceSubscription);

				SingleAssignmentDisposable otherSubscription = new SingleAssignmentDisposable();
				SkipUntilOther otherObserver = new SkipUntilOther(this, sourceObserver, otherSubscription);

				sourceSubscription.Disposable = parent.source.Subscribe(sourceObserver);
				otherSubscription.Disposable = parent.other.Subscribe(otherObserver);

				return StableCompositeDisposable.Create(otherSubscription, sourceSubscription);
			}

			public override void OnNext(T value) { }

			public override void OnError(Exception error) { }

			public override void OnCompleted() { }

			private class SkipUntil : IObserver<T>
			{
				private readonly SkipUntilOuterObserver parent;
				private readonly IDisposable subscription;
				public volatile IObserver<T> observer;

				public SkipUntil(SkipUntilOuterObserver parent, IDisposable subscription)
				{
					this.parent = parent;
					observer = EmptyObserver<T>.Instance;
					this.subscription = subscription;
				}

				public void OnNext(T value)
				{
					observer.OnNext(value);
				}

				public void OnError(Exception error)
				{
					try
					{
						observer.OnError(error);
					}
					finally
					{
						parent.Dispose();
					}
				}

				public void OnCompleted()
				{
					try
					{
						observer.OnCompleted();
					}
					finally
					{
						subscription.Dispose();
					}
				}
			}

			private class SkipUntilOther : IObserver<TOther>
			{
				private readonly SkipUntilOuterObserver parent;
				private readonly SkipUntil sourceObserver;
				private readonly IDisposable subscription;

				public SkipUntilOther(SkipUntilOuterObserver parent, SkipUntil sourceObserver, IDisposable subscription)
				{
					this.parent = parent;
					this.sourceObserver = sourceObserver;
					this.subscription = subscription;
				}

				public void OnNext(TOther value)
				{
					sourceObserver.observer = parent.observer;
					subscription.Dispose();
				}

				public void OnError(Exception error)
				{
					try
					{
						parent.observer.OnError(error);
					}
					finally
					{
						parent.Dispose();
					}
				}

				public void OnCompleted()
				{
					subscription.Dispose();
				}
			}
		}
	}
}
using System;
using UniRx.InternalUtil;

namespace UniRx.Operators
{
	internal class AmbObservable<T> : OperatorObservableBase<T>
	{
		private readonly IObservable<T> second;
		private readonly IObservable<T> source;

		public AmbObservable(IObservable<T> source, IObservable<T> second)
			: base(source.IsRequiredSubscribeOnCurrentThread() || second.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			this.second = second;
		}

		protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
		{
			return new AmbOuterObserver(this, observer, cancel).Run();
		}

		private class AmbOuterObserver : OperatorObserverBase<T, T>
		{
			private readonly object gate = new object();

			private readonly AmbObservable<T> parent;
			private AmbState choice = AmbState.Neither;
			private SingleAssignmentDisposable leftSubscription;
			private SingleAssignmentDisposable rightSubscription;

			public AmbOuterObserver(AmbObservable<T> parent, IObserver<T> observer, IDisposable cancel)
				: base(observer, cancel)
			{
				this.parent = parent;
			}

			public IDisposable Run()
			{
				leftSubscription = new SingleAssignmentDisposable();
				rightSubscription = new SingleAssignmentDisposable();
				ICancelable d = StableCompositeDisposable.Create(leftSubscription, rightSubscription);

				Amb left = new Amb();
				left.targetDisposable = d;
				left.targetObserver = new AmbDecisionObserver(this, AmbState.Left, rightSubscription, left);

				Amb right = new Amb();
				right.targetDisposable = d;
				right.targetObserver = new AmbDecisionObserver(this, AmbState.Right, leftSubscription, right);

				leftSubscription.Disposable = parent.source.Subscribe(left);
				rightSubscription.Disposable = parent.second.Subscribe(right);

				return d;
			}

			public override void OnNext(T value)
			{
				// no use
			}

			public override void OnError(Exception error)
			{
				// no use
			}

			public override void OnCompleted()
			{
				// no use
			}

			private enum AmbState
			{
				Left,
				Right,
				Neither
			}

			private class Amb : IObserver<T>
			{
				public IDisposable targetDisposable;
				public IObserver<T> targetObserver;

				public void OnNext(T value)
				{
					targetObserver.OnNext(value);
				}

				public void OnError(Exception error)
				{
					try
					{
						targetObserver.OnError(error);
					}
					finally
					{
						targetObserver = EmptyObserver<T>.Instance;
						targetDisposable.Dispose();
					}
				}

				public void OnCompleted()
				{
					try
					{
						targetObserver.OnCompleted();
					}
					finally
					{
						targetObserver = EmptyObserver<T>.Instance;
						targetDisposable.Dispose();
					}
				}
			}

			private class AmbDecisionObserver : IObserver<T>
			{
				private readonly AmbState me;
				private readonly IDisposable otherSubscription;
				private readonly AmbOuterObserver parent;
				private readonly Amb self;

				public AmbDecisionObserver(AmbOuterObserver parent, AmbState me, IDisposable otherSubscription,
					Amb self)
				{
					this.parent = parent;
					this.me = me;
					this.otherSubscription = otherSubscription;
					this.self = self;
				}

				public void OnNext(T value)
				{
					lock (parent.gate)
					{
						if (parent.choice == AmbState.Neither)
						{
							parent.choice = me;
							otherSubscription.Dispose();
							self.targetObserver = parent.observer;
						}

						if (parent.choice == me)
						{
							self.targetObserver.OnNext(value);
						}
					}
				}

				public void OnError(Exception error)
				{
					lock (parent.gate)
					{
						if (parent.choice == AmbState.Neither)
						{
							parent.choice = me;
							otherSubscription.Dispose();
							self.targetObserver = parent.observer;
						}

						if (parent.choice == me)
						{
							self.targetObserver.OnError(error);
						}
					}
				}

				public void OnCompleted()
				{
					lock (parent.gate)
					{
						if (parent.choice == AmbState.Neither)
						{
							parent.choice = me;
							otherSubscription.Dispose();
							self.targetObserver = parent.observer;
						}

						if (parent.choice == me)
						{
							self.targetObserver.OnCompleted();
						}
					}
				}
			}
		}
	}
}
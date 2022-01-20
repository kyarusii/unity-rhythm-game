using System;

namespace UniRx.Operators
{
	// Do, DoOnError, DoOnCompleted, DoOnTerminate, DoOnSubscribe, DoOnCancel

	internal class DoObservable<T> : OperatorObservableBase<T>
	{
		private readonly Action onCompleted;
		private readonly Action<Exception> onError;
		private readonly Action<T> onNext;
		private readonly IObservable<T> source;

		public DoObservable(IObservable<T> source, Action<T> onNext, Action<Exception> onError, Action onCompleted)
			: base(source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			this.onNext = onNext;
			this.onError = onError;
			this.onCompleted = onCompleted;
		}

		protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
		{
			return new Do(this, observer, cancel).Run();
		}

		private class Do : OperatorObserverBase<T, T>
		{
			private readonly DoObservable<T> parent;

			public Do(DoObservable<T> parent, IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
			{
				this.parent = parent;
			}

			public IDisposable Run()
			{
				return parent.source.Subscribe(this);
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

					;
					return;
				}

				observer.OnNext(value);
			}

			public override void OnError(Exception error)
			{
				try
				{
					parent.onError(error);
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

					;
					return;
				}

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
				try
				{
					parent.onCompleted();
				}
				catch (Exception ex)
				{
					observer.OnError(ex);
					Dispose();
					return;
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
		}
	}

	internal class DoObserverObservable<T> : OperatorObservableBase<T>
	{
		private readonly IObserver<T> observer;
		private readonly IObservable<T> source;

		public DoObserverObservable(IObservable<T> source, IObserver<T> observer)
			: base(source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			this.observer = observer;
		}

		protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
		{
			return new Do(this, observer, cancel).Run();
		}

		private class Do : OperatorObserverBase<T, T>
		{
			private readonly DoObserverObservable<T> parent;

			public Do(DoObserverObservable<T> parent, IObserver<T> observer, IDisposable cancel) : base(observer,
				cancel)
			{
				this.parent = parent;
			}

			public IDisposable Run()
			{
				return parent.source.Subscribe(this);
			}

			public override void OnNext(T value)
			{
				try
				{
					parent.observer.OnNext(value);
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

				observer.OnNext(value);
			}

			public override void OnError(Exception error)
			{
				try
				{
					parent.observer.OnError(error);
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
					parent.observer.OnCompleted();
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

	internal class DoOnErrorObservable<T> : OperatorObservableBase<T>
	{
		private readonly Action<Exception> onError;
		private readonly IObservable<T> source;

		public DoOnErrorObservable(IObservable<T> source, Action<Exception> onError)
			: base(source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			this.onError = onError;
		}

		protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
		{
			return new DoOnError(this, observer, cancel).Run();
		}

		private class DoOnError : OperatorObserverBase<T, T>
		{
			private readonly DoOnErrorObservable<T> parent;

			public DoOnError(DoOnErrorObservable<T> parent, IObserver<T> observer, IDisposable cancel) : base(observer,
				cancel)
			{
				this.parent = parent;
			}

			public IDisposable Run()
			{
				return parent.source.Subscribe(this);
			}

			public override void OnNext(T value)
			{
				observer.OnNext(value);
			}

			public override void OnError(Exception error)
			{
				try
				{
					parent.onError(error);
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

	internal class DoOnCompletedObservable<T> : OperatorObservableBase<T>
	{
		private readonly Action onCompleted;
		private readonly IObservable<T> source;

		public DoOnCompletedObservable(IObservable<T> source, Action onCompleted)
			: base(source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			this.onCompleted = onCompleted;
		}

		protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
		{
			return new DoOnCompleted(this, observer, cancel).Run();
		}

		private class DoOnCompleted : OperatorObserverBase<T, T>
		{
			private readonly DoOnCompletedObservable<T> parent;

			public DoOnCompleted(DoOnCompletedObservable<T> parent, IObserver<T> observer, IDisposable cancel) : base(
				observer, cancel)
			{
				this.parent = parent;
			}

			public IDisposable Run()
			{
				return parent.source.Subscribe(this);
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
				try
				{
					parent.onCompleted();
				}
				catch (Exception ex)
				{
					observer.OnError(ex);
					Dispose();
					return;
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
		}
	}

	internal class DoOnTerminateObservable<T> : OperatorObservableBase<T>
	{
		private readonly Action onTerminate;
		private readonly IObservable<T> source;

		public DoOnTerminateObservable(IObservable<T> source, Action onTerminate)
			: base(source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			this.onTerminate = onTerminate;
		}

		protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
		{
			return new DoOnTerminate(this, observer, cancel).Run();
		}

		private class DoOnTerminate : OperatorObserverBase<T, T>
		{
			private readonly DoOnTerminateObservable<T> parent;

			public DoOnTerminate(DoOnTerminateObservable<T> parent, IObserver<T> observer, IDisposable cancel) : base(
				observer, cancel)
			{
				this.parent = parent;
			}

			public IDisposable Run()
			{
				return parent.source.Subscribe(this);
			}

			public override void OnNext(T value)
			{
				observer.OnNext(value);
			}

			public override void OnError(Exception error)
			{
				try
				{
					parent.onTerminate();
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
				try
				{
					parent.onTerminate();
				}
				catch (Exception ex)
				{
					observer.OnError(ex);
					Dispose();
					return;
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
		}
	}

	internal class DoOnSubscribeObservable<T> : OperatorObservableBase<T>
	{
		private readonly Action onSubscribe;
		private readonly IObservable<T> source;

		public DoOnSubscribeObservable(IObservable<T> source, Action onSubscribe)
			: base(source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			this.onSubscribe = onSubscribe;
		}

		protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
		{
			return new DoOnSubscribe(this, observer, cancel).Run();
		}

		private class DoOnSubscribe : OperatorObserverBase<T, T>
		{
			private readonly DoOnSubscribeObservable<T> parent;

			public DoOnSubscribe(DoOnSubscribeObservable<T> parent, IObserver<T> observer, IDisposable cancel) : base(
				observer, cancel)
			{
				this.parent = parent;
			}

			public IDisposable Run()
			{
				try
				{
					parent.onSubscribe();
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

					return Disposable.Empty;
				}

				return parent.source.Subscribe(this);
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

	internal class DoOnCancelObservable<T> : OperatorObservableBase<T>
	{
		private readonly Action onCancel;
		private readonly IObservable<T> source;

		public DoOnCancelObservable(IObservable<T> source, Action onCancel)
			: base(source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			this.onCancel = onCancel;
		}

		protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
		{
			return new DoOnCancel(this, observer, cancel).Run();
		}

		private class DoOnCancel : OperatorObserverBase<T, T>
		{
			private readonly DoOnCancelObservable<T> parent;
			private bool isCompletedCall = false;

			public DoOnCancel(DoOnCancelObservable<T> parent, IObserver<T> observer, IDisposable cancel)
				: base(observer, cancel)
			{
				this.parent = parent;
			}

			public IDisposable Run()
			{
				return StableCompositeDisposable.Create(parent.source.Subscribe(this), Disposable.Create(() =>
				{
					if (!isCompletedCall)
					{
						parent.onCancel();
					}
				}));
			}

			public override void OnNext(T value)
			{
				observer.OnNext(value);
			}

			public override void OnError(Exception error)
			{
				isCompletedCall = true;
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
				isCompletedCall = true;
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
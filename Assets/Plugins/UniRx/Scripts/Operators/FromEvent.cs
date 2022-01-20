using System;

namespace UniRx.Operators
{
	// FromEvent, FromEventPattern

	internal class FromEventPatternObservable<TDelegate, TEventArgs> : OperatorObservableBase<EventPattern<TEventArgs>>
		where TEventArgs : EventArgs
	{
		private readonly Action<TDelegate> addHandler;
		private readonly Func<EventHandler<TEventArgs>, TDelegate> conversion;
		private readonly Action<TDelegate> removeHandler;

		public FromEventPatternObservable(Func<EventHandler<TEventArgs>, TDelegate> conversion,
			Action<TDelegate> addHandler, Action<TDelegate> removeHandler)
			: base(false)
		{
			this.conversion = conversion;
			this.addHandler = addHandler;
			this.removeHandler = removeHandler;
		}

		protected override IDisposable SubscribeCore(IObserver<EventPattern<TEventArgs>> observer, IDisposable cancel)
		{
			FromEventPattern fe = new FromEventPattern(this, observer);
			return fe.Register() ? fe : Disposable.Empty;
		}

		private class FromEventPattern : IDisposable
		{
			private readonly IObserver<EventPattern<TEventArgs>> observer;
			private readonly FromEventPatternObservable<TDelegate, TEventArgs> parent;
			private TDelegate handler;

			public FromEventPattern(FromEventPatternObservable<TDelegate, TEventArgs> parent,
				IObserver<EventPattern<TEventArgs>> observer)
			{
				this.parent = parent;
				this.observer = observer;
			}

			public void Dispose()
			{
				if (handler != null)
				{
					parent.removeHandler(handler);
					handler = default;
				}
			}

			public bool Register()
			{
				handler = parent.conversion(OnNext);
				try
				{
					parent.addHandler(handler);
				}
				catch (Exception ex)
				{
					observer.OnError(ex);
					return false;
				}

				return true;
			}

			private void OnNext(object sender, TEventArgs eventArgs)
			{
				observer.OnNext(new EventPattern<TEventArgs>(sender, eventArgs));
			}
		}
	}

	internal class FromEventObservable<TDelegate> : OperatorObservableBase<Unit>
	{
		private readonly Action<TDelegate> addHandler;
		private readonly Func<Action, TDelegate> conversion;
		private readonly Action<TDelegate> removeHandler;

		public FromEventObservable(Func<Action, TDelegate> conversion, Action<TDelegate> addHandler,
			Action<TDelegate> removeHandler)
			: base(false)
		{
			this.conversion = conversion;
			this.addHandler = addHandler;
			this.removeHandler = removeHandler;
		}

		protected override IDisposable SubscribeCore(IObserver<Unit> observer, IDisposable cancel)
		{
			FromEvent fe = new FromEvent(this, observer);
			return fe.Register() ? fe : Disposable.Empty;
		}

		private class FromEvent : IDisposable
		{
			private readonly IObserver<Unit> observer;
			private readonly FromEventObservable<TDelegate> parent;
			private TDelegate handler;

			public FromEvent(FromEventObservable<TDelegate> parent, IObserver<Unit> observer)
			{
				this.parent = parent;
				this.observer = observer;
			}

			public void Dispose()
			{
				if (handler != null)
				{
					parent.removeHandler(handler);
					handler = default;
				}
			}

			public bool Register()
			{
				handler = parent.conversion(OnNext);

				try
				{
					parent.addHandler(handler);
				}
				catch (Exception ex)
				{
					observer.OnError(ex);
					return false;
				}

				return true;
			}

			private void OnNext()
			{
				observer.OnNext(Unit.Default);
			}
		}
	}

	internal class FromEventObservable<TDelegate, TEventArgs> : OperatorObservableBase<TEventArgs>
	{
		private readonly Action<TDelegate> addHandler;
		private readonly Func<Action<TEventArgs>, TDelegate> conversion;
		private readonly Action<TDelegate> removeHandler;

		public FromEventObservable(Func<Action<TEventArgs>, TDelegate> conversion, Action<TDelegate> addHandler,
			Action<TDelegate> removeHandler)
			: base(false)
		{
			this.conversion = conversion;
			this.addHandler = addHandler;
			this.removeHandler = removeHandler;
		}

		protected override IDisposable SubscribeCore(IObserver<TEventArgs> observer, IDisposable cancel)
		{
			FromEvent fe = new FromEvent(this, observer);
			return fe.Register() ? fe : Disposable.Empty;
		}

		private class FromEvent : IDisposable
		{
			private readonly IObserver<TEventArgs> observer;
			private readonly FromEventObservable<TDelegate, TEventArgs> parent;
			private TDelegate handler;

			public FromEvent(FromEventObservable<TDelegate, TEventArgs> parent, IObserver<TEventArgs> observer)
			{
				this.parent = parent;
				this.observer = observer;
			}

			public void Dispose()
			{
				if (handler != null)
				{
					parent.removeHandler(handler);
					handler = default;
				}
			}

			public bool Register()
			{
				handler = parent.conversion(OnNext);

				try
				{
					parent.addHandler(handler);
				}
				catch (Exception ex)
				{
					observer.OnError(ex);
					return false;
				}

				return true;
			}

			private void OnNext(TEventArgs args)
			{
				observer.OnNext(args);
			}
		}
	}

	internal class FromEventObservable : OperatorObservableBase<Unit>
	{
		private readonly Action<Action> addHandler;
		private readonly Action<Action> removeHandler;

		public FromEventObservable(Action<Action> addHandler, Action<Action> removeHandler)
			: base(false)
		{
			this.addHandler = addHandler;
			this.removeHandler = removeHandler;
		}

		protected override IDisposable SubscribeCore(IObserver<Unit> observer, IDisposable cancel)
		{
			FromEvent fe = new FromEvent(this, observer);
			return fe.Register() ? fe : Disposable.Empty;
		}

		private class FromEvent : IDisposable
		{
			private readonly IObserver<Unit> observer;
			private readonly FromEventObservable parent;
			private Action handler;

			public FromEvent(FromEventObservable parent, IObserver<Unit> observer)
			{
				this.parent = parent;
				this.observer = observer;
				handler = OnNext;
			}

			public void Dispose()
			{
				if (handler != null)
				{
					parent.removeHandler(handler);
					handler = null;
				}
			}

			public bool Register()
			{
				try
				{
					parent.addHandler(handler);
				}
				catch (Exception ex)
				{
					observer.OnError(ex);
					return false;
				}

				return true;
			}

			private void OnNext()
			{
				observer.OnNext(Unit.Default);
			}
		}
	}

	internal class FromEventObservable_<T> : OperatorObservableBase<T>
	{
		private readonly Action<Action<T>> addHandler;
		private readonly Action<Action<T>> removeHandler;

		public FromEventObservable_(Action<Action<T>> addHandler, Action<Action<T>> removeHandler)
			: base(false)
		{
			this.addHandler = addHandler;
			this.removeHandler = removeHandler;
		}

		protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
		{
			FromEvent fe = new FromEvent(this, observer);
			return fe.Register() ? fe : Disposable.Empty;
		}

		private class FromEvent : IDisposable
		{
			private readonly IObserver<T> observer;
			private readonly FromEventObservable_<T> parent;
			private Action<T> handler;

			public FromEvent(FromEventObservable_<T> parent, IObserver<T> observer)
			{
				this.parent = parent;
				this.observer = observer;
				handler = OnNext;
			}

			public void Dispose()
			{
				if (handler != null)
				{
					parent.removeHandler(handler);
					handler = null;
				}
			}

			public bool Register()
			{
				try
				{
					parent.addHandler(handler);
				}
				catch (Exception ex)
				{
					observer.OnError(ex);
					return false;
				}

				return true;
			}

			private void OnNext(T value)
			{
				observer.OnNext(value);
			}
		}
	}
}
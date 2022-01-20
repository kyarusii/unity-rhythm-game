using System;

namespace UniRx
{
	public static class SubjectExtensions
	{
		public static ISubject<T> Synchronize<T>(this ISubject<T> subject)
		{
			return new AnonymousSubject<T>((subject as IObserver<T>).Synchronize(), subject);
		}

		public static ISubject<T> Synchronize<T>(this ISubject<T> subject, object gate)
		{
			return new AnonymousSubject<T>((subject as IObserver<T>).Synchronize(gate), subject);
		}

		private class AnonymousSubject<T, U> : ISubject<T, U>
		{
			private readonly IObservable<U> observable;
			private readonly IObserver<T> observer;

			public AnonymousSubject(IObserver<T> observer, IObservable<U> observable)
			{
				this.observer = observer;
				this.observable = observable;
			}

			public void OnCompleted()
			{
				observer.OnCompleted();
			}

			public void OnError(Exception error)
			{
				if (error == null)
				{
					throw new ArgumentNullException("error");
				}

				observer.OnError(error);
			}

			public void OnNext(T value)
			{
				observer.OnNext(value);
			}

			public IDisposable Subscribe(IObserver<U> observer)
			{
				if (observer == null)
				{
					throw new ArgumentNullException("observer");
				}

				return observable.Subscribe(observer);
			}
		}

		private class AnonymousSubject<T> : AnonymousSubject<T, T>, ISubject<T>
		{
			public AnonymousSubject(IObserver<T> observer, IObservable<T> observable)
				: base(observer, observable) { }
		}
	}
}
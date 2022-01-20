using System;

namespace UniRx
{
	public sealed class MultipleAssignmentDisposable : IDisposable, ICancelable
	{
		private static readonly BooleanDisposable True = new BooleanDisposable(true);
		private IDisposable current;

		private readonly object gate = new object();

		public IDisposable Disposable {
			get
			{
				lock (gate)
				{
					return current == True
						? UniRx.Disposable.Empty
						: current;
				}
			}
			set
			{
				bool shouldDispose = false;
				lock (gate)
				{
					shouldDispose = current == True;
					if (!shouldDispose)
					{
						current = value;
					}
				}

				if (shouldDispose && value != null)
				{
					value.Dispose();
				}
			}
		}

		public bool IsDisposed {
			get
			{
				lock (gate)
				{
					return current == True;
				}
			}
		}

		public void Dispose()
		{
			IDisposable old = null;

			lock (gate)
			{
				if (current != True)
				{
					old = current;
					current = True;
				}
			}

			if (old != null)
			{
				old.Dispose();
			}
		}
	}
}
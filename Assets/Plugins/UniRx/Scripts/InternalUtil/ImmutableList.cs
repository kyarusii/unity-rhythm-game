using System;

namespace UniRx.InternalUtil
{
	// ImmutableList is sometimes useful, use for public.
	public class ImmutableList<T>
	{
		public static readonly ImmutableList<T> Empty = new ImmutableList<T>();

		private ImmutableList()
		{
			Data = new T[0];
		}

		public ImmutableList(T[] data)
		{
			Data = data;
		}

		public T[] Data { get; }

		public ImmutableList<T> Add(T value)
		{
			T[] newData = new T[Data.Length + 1];
			Array.Copy(Data, newData, Data.Length);
			newData[Data.Length] = value;
			return new ImmutableList<T>(newData);
		}

		public ImmutableList<T> Remove(T value)
		{
			int i = IndexOf(value);
			if (i < 0)
			{
				return this;
			}

			int length = Data.Length;
			if (length == 1)
			{
				return Empty;
			}

			T[] newData = new T[length - 1];

			Array.Copy(Data, 0, newData, 0, i);
			Array.Copy(Data, i + 1, newData, i, length - i - 1);

			return new ImmutableList<T>(newData);
		}

		public int IndexOf(T value)
		{
			for (int i = 0; i < Data.Length; ++i)
			{
				// ImmutableList only use for IObserver(no worry for boxed)
				if (Equals(Data[i], value))
				{
					return i;
				}
			}

			return -1;
		}
	}
}
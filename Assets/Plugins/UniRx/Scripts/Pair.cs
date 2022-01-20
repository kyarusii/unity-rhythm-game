using System;
using System.Collections.Generic;

namespace UniRx
{
	// Pair is used for Observable.Pairwise
	[Serializable]
	public struct Pair<T> : IEquatable<Pair<T>>
	{
		private readonly T current;
		private readonly T previous;

		public Pair(T previous, T current)
		{
			this.previous = previous;
			this.current = current;
		}

		public T Previous => previous;

		public T Current => current;

		public bool Equals(Pair<T> other)
		{
			EqualityComparer<T> comparer = EqualityComparer<T>.Default;

			return comparer.Equals(previous, other.Previous) &&
			       comparer.Equals(current, other.Current);
		}

		public override int GetHashCode()
		{
			EqualityComparer<T> comparer = EqualityComparer<T>.Default;

			int h0;
			h0 = comparer.GetHashCode(previous);
			h0 = ((h0 << 5) + h0) ^ comparer.GetHashCode(current);
			return h0;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is Pair<T>))
			{
				return false;
			}

			return Equals((Pair<T>) obj);
		}

		public override string ToString()
		{
			return string.Format("({0}, {1})", previous, current);
		}
	}
}
using System;

namespace UniRx
{
	[Serializable]
	public struct Unit : IEquatable<Unit>
	{
		private static readonly Unit @default = new Unit();

		public static Unit Default => @default;

		public bool Equals(Unit other)
		{
			return true;
		}

		public static bool operator ==(Unit first, Unit second)
		{
			return true;
		}

		public static bool operator !=(Unit first, Unit second)
		{
			return false;
		}

		public override bool Equals(object obj)
		{
			return obj is Unit;
		}

		public override int GetHashCode()
		{
			return 0;
		}

		public override string ToString()
		{
			return "()";
		}
	}
}
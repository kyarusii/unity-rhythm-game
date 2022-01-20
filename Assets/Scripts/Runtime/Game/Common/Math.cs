namespace RGF.Game.Common
{
	public static class Math
	{
		public const double Tolerance = 0.0001d;

		public static double Abs(double value)
		{
			if (value > 0d)
			{
				return value;
			}

			return -value;
		}

		public static int CeilToInt(double value)
		{
			return (int)(value + 1);
		}
	}
}
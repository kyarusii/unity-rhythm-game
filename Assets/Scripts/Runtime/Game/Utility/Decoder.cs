namespace RGF.Game.Utility
{
	public class Decoder
	{
		public static int Decode36(string value)
		{
			if (value.Length != 2)
			{
				return -1;
			}

			int result = 0;
			if (value[1] >= 'A')
			{
				result += value[1] - 'A' + 10;
			}
			else
			{
				result += value[1] - '0';
			}

			if (value[0] >= 'A')
			{
				result += (value[0] - 'A' + 10) * 36;
			}
			else
			{
				result += (value[0] - '0') * 36;
			}

			return result;
		}
	}
}
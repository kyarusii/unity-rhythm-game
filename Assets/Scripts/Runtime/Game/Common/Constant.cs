namespace RGF.Game.Common
{
	public static class Constant
	{
		public const string APP_NAME = "Rhythm Game Template";

		public static readonly string[] SOUND_EXTENSIONS = { ".ogg", ".wav", "mp3" };
		public static readonly string[] BMS_EXTENSIONS = { ".bms", ".bme", "bml" };

		public static class Order
		{
			public const int GAME_MANAGER = 100;
			public const int GAME_UI = 99;
		}

		public static class Delta
		{
			public const double PERFECT = 21.0d;
			public const double GREAT = 60.0d;
			public const double GOOD = 150.0d;
			public const double BAD = 220.0d;
		}

		public static class Rate
		{
			public const float PERFECT = 1.0f;
			public const float GREAT = 0.8f;
			public const float GOOD = 0.6f;
			public const float BAD = 0.4f;
			public const float POOR = 0.0f;
		}

		public static class Score
		{
			public const long PERFECT = 190;
			public const long GREAT = 100;
			public const long GOOD = 50;
			public const long BAD = 30;
			public const long POOR = 0;
		}

		public static class Health
		{
			public const float PERFECT = 3.0f;
			public const float GREAT = 1.8f;
			public const float GOOD = 1.2f;
			public const float BAD = 1.0f;
			public const float POOR = 0.0f;

			public const float DAMAGE0 = 9f;
		}

		public static class Fever
		{
			public const float PERFECT = 3.1f;
			public const float GREAT = 1.8f;
			public const float GOOD = 1.0f;
			public const float BAD = 0.0f;
			public const float POOR = 0.0f;
		}

		public static class Chance
		{
			public const float PERFECT = 1.0f;
			public const float GREAT = 1.0f;
			public const float GOOD = 1.0f;
			public const float BAD = 4.0f;
			public const float POOR = 8.2f;
		}
	}
}
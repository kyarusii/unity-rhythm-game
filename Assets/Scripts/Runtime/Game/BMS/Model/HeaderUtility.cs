using System;

namespace RGF.Game.BMS.Model
{
	internal static class HeaderUtility
	{
		public static bool IsPlayerLevel(string s)
		{
			return s.Length > 10 &&
			       string.Compare(s.Substring(0, 10), "#PLAYLEVEL", StringComparison.OrdinalIgnoreCase) == 0;
		}

		public static bool IsStageFile(string s)
		{
			return s.Length > 11 &&
			       string.Compare(s.Substring(0, 10), "#STAGEFILE", StringComparison.OrdinalIgnoreCase) == 0;
		}

		public static bool IsSubTitle(string s)
		{
			return s.Length >= 9 &&
			       string.Compare(s.Substring(0, 9), "#SUBTITLE", StringComparison.OrdinalIgnoreCase) == 0;
		}

		public static bool IsPreview(string s)
		{
			return s.Length >= 8 &&
			       string.Compare(s.Substring(0, 8), "#PREVIEW", StringComparison.OrdinalIgnoreCase) == 0;
		}

		public static bool IsBackBmp(string s)
		{
			return s.Length >= 8 &&
			       string.Compare(s.Substring(0, 8), "#BACKBMP", StringComparison.OrdinalIgnoreCase) == 0;
		}

		public static bool IsPlayer(string s)
		{
			return s.Length >= 7 &&
			       string.Compare(s.Substring(0, 7), "#PLAYER", StringComparison.OrdinalIgnoreCase) == 0;
		}

		public static bool IsArtist(string s)
		{
			return s.Length >= 7 &&
			       string.Compare(s.Substring(0, 7), "#ARTIST", StringComparison.OrdinalIgnoreCase) == 0;
		}

		public static bool IsBanner(string s)
		{
			return s.Length >= 7 &&
			       string.Compare(s.Substring(0, 7), "#BANNER", StringComparison.OrdinalIgnoreCase) == 0;
		}

		public static bool IsLnType(string s)
		{
			return s.Length >= 7 &&
			       string.Compare(s.Substring(0, 7), "#LNTYPE", StringComparison.OrdinalIgnoreCase) == 0;
		}

		public static bool IsGenre(string s)
		{
			return s.Length >= 6 &&
			       string.Compare(s.Substring(0, 6), "#GENRE", StringComparison.OrdinalIgnoreCase) == 0;
		}

		public static bool IsTitle(string s)
		{
			return s.Length >= 6 &&
			       string.Compare(s.Substring(0, 6), "#TITLE", StringComparison.OrdinalIgnoreCase) == 0;
		}

		public static bool IsTotal(string s)
		{
			return s.Length >= 6 &&
			       string.Compare(s.Substring(0, 6), "#TOTAL", StringComparison.OrdinalIgnoreCase) == 0;
		}

		public static bool IsRank(string s)
		{
			return s.Length >= 5 && string.Compare(s.Substring(0, 5), "#RANK", StringComparison.OrdinalIgnoreCase) == 0;
		}

		public static bool IsBPM(string s)
		{
			return s.Length >= 6 &&
			       string.Compare(s.Substring(0, 4), "#BPM", StringComparison.OrdinalIgnoreCase) == 0 && s[4] == ' ';
		}

		public static bool IsHeaderEnd(string s)
		{
			return s.Length >= 30 && string.Compare(s, "*---------------------- MAIN DATA FIELD",
				StringComparison.OrdinalIgnoreCase) == 0;
		}

		public static bool IsWav(string s)
		{
			return s.StartsWith("#WAV", StringComparison.OrdinalIgnoreCase);
		}

		public static bool IsBMP(string s)
		{
			return s.StartsWith("#BMP", StringComparison.OrdinalIgnoreCase);
		}

		public static bool IsStop(string s)
		{
			return s.StartsWith("#STOP", StringComparison.OrdinalIgnoreCase);
		}

		public static bool IsLnObj(string s)
		{
			return s.StartsWith("#LNOBJ", StringComparison.OrdinalIgnoreCase);
		}
	}
}
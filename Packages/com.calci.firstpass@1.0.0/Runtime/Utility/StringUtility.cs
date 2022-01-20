﻿using System.Text;

public static class StringUtility
{
	public static string AddSpacesToSentence(string text)
	{
		if (string.IsNullOrWhiteSpace(text))
			return string.Empty;
			
		StringBuilder newText = new StringBuilder(text.Length * 2);
		newText.Append(text[0]);
		for (int i = 1; i < text.Length; i++)
		{
			if (char.IsUpper(text[i]) && text[i - 1] != ' ')
				newText.Append(' ');
			newText.Append(text[i]);
		}
			
		return newText.ToString();
	}
}
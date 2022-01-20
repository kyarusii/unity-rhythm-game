using UnityEngine;
using UnityEngine.Assertions;

public static class UGUIExtension
{
	public static void SetText(this UnityEngine.UI.Text text, string content)
	{
#if UNITY_EDITOR || DEVELOPMENT_BUILD
		Assert.IsNotNull(text);
#endif
		text.text = content;
	}

	public static void Clear(this UnityEngine.UI.Text text)
	{
#if UNITY_EDITOR || DEVELOPMENT_BUILD
		Assert.IsNotNull(text);
#endif
		text.text = string.Empty;
	}

	public static void SetAlpha(this UnityEngine.UI.Graphic graphic, float alpha)
	{
		Color color = graphic.color;
		color.a = alpha;

		graphic.color = color;
	}
}
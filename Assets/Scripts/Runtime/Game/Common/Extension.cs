using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RGF.Game.Common
{
	public static class Extension
	{
		public static void RemoveLast<T>(this List<T> target)
		{
			target.RemoveAt(target.Count - 1);
		}

		public static T Peek<T>(this List<T> target)
		{
			return target[target.Count - 1];
		}

		/// <summary>
		/// 지정된 확장자로 BMS 파일 포맷인지 확인합니다.
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static bool IsBMSFile(string path)
		{
			foreach (string extension in Constant.BMS_EXTENSIONS)
			{
				if (path.EndsWith(extension))
				{
					return true;
				}
			}

			return false;
		}

		public static void SetAlpha(this Graphic graphic, float alpha)
		{
			Color color = graphic.color;
			color.a = alpha;
			graphic.color = color;
		}
	}
}
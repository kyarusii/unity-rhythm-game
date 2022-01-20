using System;
using System.Collections.Generic;

public static class CollectionExtension
{
	public static void Shuffle<T>(this IList<T> list)
	{
		list.Shuffle(new Random());
	}
	
	public static void Shuffle<T>(this IList<T> list, System.Random random)
	{
		int i = list.Count;
		while (i > 1)
		{
			i--;
			int index = random.Next(i + 1);
			
#if UNITY_2021_2_OR_NEWER
			(list[index], list[i]) = (list[i], list[index]);
#else
			T value = list[index];
			list[index] = list[i];
			list[i] = value;
#endif
		}
	}
}
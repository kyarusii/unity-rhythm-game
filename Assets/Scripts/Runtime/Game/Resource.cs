using System.Collections.Generic;
using UnityEngine;

namespace RGF.Game
{
	public sealed class Resource : MonoSingleton<Resource>
	{
		private Dictionary<string, Object> _cached;

		protected override void OnCreate()
		{
			_cached ??= new Dictionary<string, Object>();
		}

		public static T Load<T>(string path) where T : Object
		{
			if (!Inst._cached.TryGetValue(path, out Object obj))
			{
				obj = Resources.Load<T>(path);
				Inst._cached[path] = obj;
			}

			return obj as T;
		}
	}
}
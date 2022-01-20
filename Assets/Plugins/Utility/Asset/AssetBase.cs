using UnityEngine;

namespace RGF.Firstpass
{
	public abstract class AssetBase<T> : ScriptableObject where T : class, new()
	{
		public T asset;

		public static implicit operator T(AssetBase<T> me)
		{
			return me.asset;
		}

		public static implicit operator AssetBase<T>(T t)
		{
			AssetBase<T> so = CreateInstance<AssetBase<T>>();
			so.asset = t;

			return so;
		}
	}
}
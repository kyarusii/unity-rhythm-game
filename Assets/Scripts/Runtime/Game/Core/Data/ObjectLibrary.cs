using System.Collections.Generic;

namespace RGF.Game.Core.Data
{
	public abstract class ObjectLibrary<T, ObjectType> : MonoInstance<T> where T : ObjectLibrary<T, ObjectType>
	{
		protected readonly Dictionary<string, ObjectType> m_objectMap = new Dictionary<string, ObjectType>();

		private void OnDestroy()
		{
			m_objectMap.Clear();
		}

		public ObjectType Find(string path)
		{
			if (!m_objectMap.ContainsKey(path))
			{
				return default;
			}

			return m_objectMap[path];
		}

		public ObjectType Get(string path)
		{
			return m_objectMap[path];
		}

		public bool HasObject(string path)
		{
			return m_objectMap.ContainsKey(path);
		}

		public bool Unload(string path)
		{
			if (!m_objectMap.ContainsKey(path))
			{
				return false;
			}

			m_objectMap.Remove(path);

			return true;
		}
	}
}
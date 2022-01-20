using System.Collections;
using UnityEngine;

namespace RGF.Auth
{
	public abstract class AuthPlatform : MonoBehaviour
	{
		protected virtual void Awake()
		{
			AuthUtil.Init(this);
		}

		public virtual void IngestStat(string statName, int ingestAmount) { }

		public virtual void QueryStat() { }

		public virtual IEnumerator Login()
		{
			yield break;
		}
	}
}
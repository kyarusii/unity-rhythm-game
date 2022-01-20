using UnityEngine;

namespace RGF.UI.Scroller
{
	public interface IInfiniteScrollSetup
	{
		void OnPostSetupItems();
		void OnUpdateItem(int itemCount, GameObject obj);
	}
}
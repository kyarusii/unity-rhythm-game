using UnityEngine;
using UnityEngine.EventSystems;

namespace RGF.UI.Scroller
{
	// [RequireComponent(typeof(InfiniteScroller))]
	public class ItemControllerLoop : UIBehaviour, IInfiniteScrollSetup
	{
		private readonly bool isSetuped = false;

		public void OnPostSetupItems()
		{
			// GetComponentInParent<ScrollRect>().movementType = ScrollRect.MovementType.Unrestricted;
			// isSetuped = true;
		}

		public void OnUpdateItem(int itemCount, GameObject obj)
		{
			if (isSetuped)
			{
				return;
			}

			ScrollerItem item = obj.GetComponentInChildren<ScrollerItem>();
			item.UpdateItem(itemCount);
		}
	}
}
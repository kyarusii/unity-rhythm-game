using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace RGF.UI.Component
{
	public class SelectionGroup : MonoBehaviour
	{
		private GroupedButton selected;
		private GroupedButton defaultOption;

		private List<GroupedButton> buttons = new List<GroupedButton>();

		public bool allowNonSelection;

		public void Add(GroupedButton groupedButton)
		{
			buttons.Add(groupedButton);

			if (groupedButton.isDefault)
			{
				Assert.IsNull(defaultOption);
				defaultOption = groupedButton;
			}
		}

		public void Remove(GroupedButton groupedButton)
		{
			buttons.Remove(groupedButton);
		}

		public void Select(GroupedButton groupedButton)
		{
			selected = groupedButton;

			foreach (GroupedButton button in buttons)
			{
				if (ReferenceEquals(button, selected))
				{
					continue;
				}

				button.DeselectWithoutCallback();
			}
		}

		public void Deselect(GroupedButton groupedButton)
		{
			if (allowNonSelection)
			{
				selected = null;
				return;
			}

			// foreach (GroupedButton button in buttons)
			// {
			// 	selected = button;
			// 	button.SelectWithoutCallback();
			// 	Debug.Log("Another Selected!", selected.gameObject);
			// 	break;
			// }

			if (defaultOption)
			{
				defaultOption.SelectWithoutCallback();
			}
			else
			{
				buttons.FirstOrDefault()?.SelectWithoutCallback();
			}
		}

		private void Start()
		{
			foreach (GroupedButton button in buttons)
			{
				button.DeselectWithoutCallback();
			}

			if (!allowNonSelection)
			{
				if (defaultOption)
				{
					defaultOption.SelectWithoutCallback();
				}
				else
				{
					buttons.FirstOrDefault()?.SelectWithoutCallback();
				}
			}
		}
	}
}
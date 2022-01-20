using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace RGF.UI.Component
{
	[ExecuteInEditMode]
	public class GroupedButton : MonoBehaviour,
		IPointerEnterHandler,
		IPointerExitHandler,
		IPointerClickHandler
	{
		[SerializeField] public bool isDefault;
		[SerializeField] private SelectionGroup group;
		[SerializeField] private MonoBehaviour[] selected;
		[SerializeField] private MonoBehaviour[] deselected;
		[SerializeField] private bool selectOnPointer = true;
		[SerializeField] private TextMeshProUGUI[] texts;

		[NonSerialized] public UnityEvent onClick = new UnityEvent();

		private bool isSelected;
		public bool IsSelected => isSelected;

		public SelectionGroup Group {
			get => @group;
			set
			{
				@group?.Remove(this);
				@group = value;
				@group.Add(this);
			}
		}

		private void Awake()
		{
			if (Application.isPlaying)
			{
				Group = @group;
			}
		}

		[ContextMenu("Update Text")]
		public void SetText(string message)
		{
			foreach (TextMeshProUGUI text in texts)
			{
				text.SetText(message);
			}
		}

		public string GetText()
		{
			if (texts == null)
			{
				return string.Empty;
			}

			foreach (TextMeshProUGUI text in texts)
			{
				return text.text;
			}

			return string.Empty;
		}

		public void Select()
		{
			SelectWithoutCallback();
			@group.Select(this);
		}

		public void DeselectWithoutCallback()
		{
			isSelected = false;

			foreach (MonoBehaviour o in selected)
			{
				o.enabled = false;
			}

			foreach (MonoBehaviour o in deselected)
			{
				o.enabled = true;
			}
		}

		public void Deselect()
		{
			DeselectWithoutCallback();

			@group.Deselect(this);
		}


		void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
		{
			if (selectOnPointer)
			{
				Select();
			}
		}

		void IPointerExitHandler.OnPointerExit(PointerEventData eventData) { }

		public void SelectWithoutCallback()
		{
			isSelected = true;

			foreach (MonoBehaviour o in selected)
			{
				o.enabled = true;
			}

			foreach (MonoBehaviour o in deselected)
			{
				o.enabled = false;
			}
		}

		void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
		{
			onClick.Invoke();
		}
	}
}
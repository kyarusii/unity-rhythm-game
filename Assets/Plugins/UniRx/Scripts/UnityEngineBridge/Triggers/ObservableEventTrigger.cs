// for uGUI(from 4.6)

#if !(UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5)

using System;
using UnityEngine;
using UnityEngine.EventSystems; // require keep for Windows Universal App

namespace UniRx.Triggers
{
	[DisallowMultipleComponent]
	public class ObservableEventTrigger : ObservableTriggerBase, IEventSystemHandler, IPointerEnterHandler,
		IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IBeginDragHandler,
		IInitializePotentialDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IScrollHandler,
		IUpdateSelectedHandler, ISelectHandler, IDeselectHandler, IMoveHandler, ISubmitHandler, ICancelHandler
	{
		protected override void RaiseOnCompletedOnDestroy()
		{
			if (onDeselect != null)
			{
				onDeselect.OnCompleted();
			}

			if (onMove != null)
			{
				onMove.OnCompleted();
			}

			if (onPointerDown != null)
			{
				onPointerDown.OnCompleted();
			}

			if (onPointerEnter != null)
			{
				onPointerEnter.OnCompleted();
			}

			if (onPointerExit != null)
			{
				onPointerExit.OnCompleted();
			}

			if (onPointerUp != null)
			{
				onPointerUp.OnCompleted();
			}

			if (onSelect != null)
			{
				onSelect.OnCompleted();
			}

			if (onPointerClick != null)
			{
				onPointerClick.OnCompleted();
			}

			if (onSubmit != null)
			{
				onSubmit.OnCompleted();
			}

			if (onDrag != null)
			{
				onDrag.OnCompleted();
			}

			if (onBeginDrag != null)
			{
				onBeginDrag.OnCompleted();
			}

			if (onEndDrag != null)
			{
				onEndDrag.OnCompleted();
			}

			if (onDrop != null)
			{
				onDrop.OnCompleted();
			}

			if (onUpdateSelected != null)
			{
				onUpdateSelected.OnCompleted();
			}

			if (onInitializePotentialDrag != null)
			{
				onInitializePotentialDrag.OnCompleted();
			}

			if (onCancel != null)
			{
				onCancel.OnCompleted();
			}

			if (onScroll != null)
			{
				onScroll.OnCompleted();
			}
		}

		#region IDeselectHandler

		private Subject<BaseEventData> onDeselect;

		void IDeselectHandler.OnDeselect(BaseEventData eventData)
		{
			if (onDeselect != null)
			{
				onDeselect.OnNext(eventData);
			}
		}

		public IObservable<BaseEventData> OnDeselectAsObservable()
		{
			return onDeselect ?? (onDeselect = new Subject<BaseEventData>());
		}

		#endregion

		#region IMoveHandler

		private Subject<AxisEventData> onMove;

		void IMoveHandler.OnMove(AxisEventData eventData)
		{
			if (onMove != null)
			{
				onMove.OnNext(eventData);
			}
		}

		public IObservable<AxisEventData> OnMoveAsObservable()
		{
			return onMove ?? (onMove = new Subject<AxisEventData>());
		}

		#endregion

		#region IPointerDownHandler

		private Subject<PointerEventData> onPointerDown;

		void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
		{
			if (onPointerDown != null)
			{
				onPointerDown.OnNext(eventData);
			}
		}

		public IObservable<PointerEventData> OnPointerDownAsObservable()
		{
			return onPointerDown ?? (onPointerDown = new Subject<PointerEventData>());
		}

		#endregion

		#region IPointerEnterHandler

		private Subject<PointerEventData> onPointerEnter;

		void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
		{
			if (onPointerEnter != null)
			{
				onPointerEnter.OnNext(eventData);
			}
		}

		public IObservable<PointerEventData> OnPointerEnterAsObservable()
		{
			return onPointerEnter ?? (onPointerEnter = new Subject<PointerEventData>());
		}

		#endregion

		#region IPointerExitHandler

		private Subject<PointerEventData> onPointerExit;

		void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
		{
			if (onPointerExit != null)
			{
				onPointerExit.OnNext(eventData);
			}
		}

		public IObservable<PointerEventData> OnPointerExitAsObservable()
		{
			return onPointerExit ?? (onPointerExit = new Subject<PointerEventData>());
		}

		#endregion

		#region IPointerUpHandler

		private Subject<PointerEventData> onPointerUp;

		void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
		{
			if (onPointerUp != null)
			{
				onPointerUp.OnNext(eventData);
			}
		}

		public IObservable<PointerEventData> OnPointerUpAsObservable()
		{
			return onPointerUp ?? (onPointerUp = new Subject<PointerEventData>());
		}

		#endregion

		#region ISelectHandler

		private Subject<BaseEventData> onSelect;

		void ISelectHandler.OnSelect(BaseEventData eventData)
		{
			if (onSelect != null)
			{
				onSelect.OnNext(eventData);
			}
		}

		public IObservable<BaseEventData> OnSelectAsObservable()
		{
			return onSelect ?? (onSelect = new Subject<BaseEventData>());
		}

		#endregion

		#region IPointerClickHandler

		private Subject<PointerEventData> onPointerClick;

		void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
		{
			if (onPointerClick != null)
			{
				onPointerClick.OnNext(eventData);
			}
		}

		public IObservable<PointerEventData> OnPointerClickAsObservable()
		{
			return onPointerClick ?? (onPointerClick = new Subject<PointerEventData>());
		}

		#endregion

		#region ISubmitHandler

		private Subject<BaseEventData> onSubmit;

		void ISubmitHandler.OnSubmit(BaseEventData eventData)
		{
			if (onSubmit != null)
			{
				onSubmit.OnNext(eventData);
			}
		}

		public IObservable<BaseEventData> OnSubmitAsObservable()
		{
			return onSubmit ?? (onSubmit = new Subject<BaseEventData>());
		}

		#endregion

		#region IDragHandler

		private Subject<PointerEventData> onDrag;

		void IDragHandler.OnDrag(PointerEventData eventData)
		{
			if (onDrag != null)
			{
				onDrag.OnNext(eventData);
			}
		}

		public IObservable<PointerEventData> OnDragAsObservable()
		{
			return onDrag ?? (onDrag = new Subject<PointerEventData>());
		}

		#endregion

		#region IBeginDragHandler

		private Subject<PointerEventData> onBeginDrag;

		void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
		{
			if (onBeginDrag != null)
			{
				onBeginDrag.OnNext(eventData);
			}
		}

		public IObservable<PointerEventData> OnBeginDragAsObservable()
		{
			return onBeginDrag ?? (onBeginDrag = new Subject<PointerEventData>());
		}

		#endregion

		#region IEndDragHandler

		private Subject<PointerEventData> onEndDrag;

		void IEndDragHandler.OnEndDrag(PointerEventData eventData)
		{
			if (onEndDrag != null)
			{
				onEndDrag.OnNext(eventData);
			}
		}

		public IObservable<PointerEventData> OnEndDragAsObservable()
		{
			return onEndDrag ?? (onEndDrag = new Subject<PointerEventData>());
		}

		#endregion

		#region IDropHandler

		private Subject<PointerEventData> onDrop;

		void IDropHandler.OnDrop(PointerEventData eventData)
		{
			if (onDrop != null)
			{
				onDrop.OnNext(eventData);
			}
		}

		public IObservable<PointerEventData> OnDropAsObservable()
		{
			return onDrop ?? (onDrop = new Subject<PointerEventData>());
		}

		#endregion

		#region IUpdateSelectedHandler

		private Subject<BaseEventData> onUpdateSelected;

		void IUpdateSelectedHandler.OnUpdateSelected(BaseEventData eventData)
		{
			if (onUpdateSelected != null)
			{
				onUpdateSelected.OnNext(eventData);
			}
		}

		public IObservable<BaseEventData> OnUpdateSelectedAsObservable()
		{
			return onUpdateSelected ?? (onUpdateSelected = new Subject<BaseEventData>());
		}

		#endregion

		#region IInitializePotentialDragHandler

		private Subject<PointerEventData> onInitializePotentialDrag;

		void IInitializePotentialDragHandler.OnInitializePotentialDrag(PointerEventData eventData)
		{
			if (onInitializePotentialDrag != null)
			{
				onInitializePotentialDrag.OnNext(eventData);
			}
		}

		public IObservable<PointerEventData> OnInitializePotentialDragAsObservable()
		{
			return onInitializePotentialDrag ?? (onInitializePotentialDrag = new Subject<PointerEventData>());
		}

		#endregion

		#region ICancelHandler

		private Subject<BaseEventData> onCancel;

		void ICancelHandler.OnCancel(BaseEventData eventData)
		{
			if (onCancel != null)
			{
				onCancel.OnNext(eventData);
			}
		}

		public IObservable<BaseEventData> OnCancelAsObservable()
		{
			return onCancel ?? (onCancel = new Subject<BaseEventData>());
		}

		#endregion

		#region IScrollHandler

		private Subject<PointerEventData> onScroll;

		void IScrollHandler.OnScroll(PointerEventData eventData)
		{
			if (onScroll != null)
			{
				onScroll.OnNext(eventData);
			}
		}

		public IObservable<PointerEventData> OnScrollAsObservable()
		{
			return onScroll ?? (onScroll = new Subject<PointerEventData>());
		}

		#endregion
	}
}

#endif
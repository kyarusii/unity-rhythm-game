using System.Collections.Generic;
using DG.Tweening;
using RGF.Game.Structure;
using UnityEngine;

namespace RGF.UI.Scene
{
	public class GearEffect : MonoBehaviour
	{
		[Header("Key Press Effect")] [SerializeField]
		private SpriteRenderer[] m_keyPressed = default;

		[SerializeField] private SpriteRenderer[] m_padPressed = default;

		[SerializeField] private float m_keyPressDuration = 0.1f;
		[SerializeField] private float m_keyPressAlpha = 0.25f;
		[SerializeField] private float m_padPressAlpha = 1.0f;
		[SerializeField] private float m_turnOffMultiply = 0.5f;

		[Header("Hit Animation")] [SerializeField]
		private Animator[] m_animators = default;

		[SerializeField] private string[] names =
		{
			"Energy Field 02",
			"Energy Field 17"
		};

		private Dictionary<NoteKey, Tweener> keyPressMap;
		private Dictionary<NoteKey, Tweener> padPressMap;

		private void Awake()
		{
			keyPressMap = new Dictionary<NoteKey, Tweener>();
			padPressMap = new Dictionary<NoteKey, Tweener>();

			foreach (SpriteRenderer keyEffect in m_keyPressed)
			{
				keyEffect.DOFade(0f, 0f);
			}

			foreach (SpriteRenderer keyEffect in m_padPressed)
			{
				keyEffect.DOFade(0f, 0f);
			}
		}

		private void OnEnable()
		{
			Message.Register<NoteKey>(Event.OnKeyDown, OnKeyDown);
			Message.Register<NoteKey>(Event.OnKeyDownAuto, OnKeyDownAuto);
			Message.Register<NoteKey>(Event.OnKeyUp, OnKeyUp);
			Message.Register<NoteKey>(Event.OnHandleNote, OnHandleNote);
		}

		private void OnDisable()
		{
			Message.Unregister<NoteKey>(Event.OnKeyDown, OnKeyDown);
			Message.Unregister<NoteKey>(Event.OnKeyDownAuto, OnKeyDownAuto);
			Message.Unregister<NoteKey>(Event.OnKeyUp, OnKeyUp);
			Message.Unregister<NoteKey>(Event.OnHandleNote, OnHandleNote);
		}

		private void OnKeyDown(NoteKey noteKey)
		{
			int index = ConvertIndex(noteKey);

			Tweener tweener = m_keyPressed[index].DOFade(m_keyPressAlpha, m_keyPressDuration);
			keyPressMap[noteKey] = tweener;

			Tweener tweener2 = m_padPressed[index].DOFade(m_padPressAlpha, m_keyPressDuration);
			padPressMap[noteKey] = tweener2;
		}

		private void OnKeyDownAuto(NoteKey noteKey)
		{
			int index = ConvertIndex(noteKey);

			Tweener tweener = m_keyPressed[index].DOFade(m_keyPressAlpha, m_keyPressDuration);
			tweener.OnComplete(() => { m_keyPressed[index].DOFade(0f, m_keyPressDuration * m_turnOffMultiply); });

			Tweener tweener2 = m_padPressed[index].DOFade(m_padPressAlpha, m_keyPressDuration);
			tweener2.OnComplete(() => { m_padPressed[index].DOFade(0f, m_keyPressDuration * m_turnOffMultiply); });
		}

		private void OnKeyUp(NoteKey noteKey)
		{
			// // @BUG:: Tweener가 Kill 된 상태인 경우
			// if (keyPressMap.TryGetValue(noteKey, out Tweener tweener))
			// {
			// 	try
			// 	{
			// 		if (tweener != null && !tweener.IsComplete())
			// 		{
			// 			tweener.Complete(false);
			// 		}	
			// 	}
			// 	catch (Exception e)
			// 	{
			// 		// ignore					
			// 	}
			// }
			//
			// if (padPressMap.TryGetValue(noteKey, out Tweener tweener2))
			// {
			// 	try
			// 	{
			// 		if (tweener2 != null && tweener2.IsPlaying())
			// 		{
			// 			tweener2.Complete(false);
			// 		}
			// 	}
			// 	catch (Exception e)
			// 	{
			// 		// ignore
			// 	}
			// }

			int index = ConvertIndex(noteKey);

			m_keyPressed[index].DOFade(0f, m_keyPressDuration * m_turnOffMultiply);
			m_padPressed[index].DOFade(0f, m_keyPressDuration * m_turnOffMultiply);
		}

		private int ConvertIndex(NoteKey noteKey)
		{
			return noteKey switch
			{
				NoteKey.SCRATCH1 => 0,
				NoteKey.NOTE1 => 1,
				NoteKey.NOTE2 => 2,
				NoteKey.NOTE3 => 3,
				NoteKey.NOTE4 => 4,
				NoteKey.NOTE5 => 5,
				NoteKey.NOTE6 => 6,
				NoteKey.NOTE7 => 7,
				_ => 0
			};
		}

		private void OnHandleNote(NoteKey noteKey)
		{
			int index = ConvertIndex(noteKey);
			int random = Random.Range(0, names.Length);

			m_animators[index].Rebind();
			m_animators[index].Play(names[random]);
		}
	}
}
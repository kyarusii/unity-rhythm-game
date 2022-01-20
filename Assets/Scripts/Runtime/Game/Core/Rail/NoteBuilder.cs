using System;
using RGF.Game.BMS.Object;
using UnityEngine;
using Enum = RGF.Game.Common.Enum;

namespace RGF.Game.Core.Rail
{
	public sealed class NoteBuilder : MonoBehaviour
	{
		[Header("Note Prefabs")] [SerializeField]
		private GameObject m_notePrefab = default;

		[SerializeField] private GameObject m_longNotePrefab = default;

		[Header("Note Transform")] [SerializeField]
		private Transform m_noteParent = default;

		[SerializeField] private Transform[] m_notePositions = default;

		[SerializeField] private float m_offset = 7f;
		private Sprite evenNote = default;

		private Sprite landMine = default;
		private Sprite longEvenNote = default;

		private Sprite longOddNote = default;

		private Sprite oddNote = default;
		private Sprite scratchLongNote = default;
		private Sprite scratchNote = default;

		private float[] xPoses = default;

		/// <summary>
		///     스프라이트를 로드합니다.
		///     @TODO :: 에셋 번들로 포팅
		/// </summary>
		private void LoadSprites()
		{
			oddNote = Resource.Load<Sprite>("Sprites/Note/White Note");
			evenNote = Resource.Load<Sprite>("Sprites/Note/Blue Note");
			scratchNote = Resource.Load<Sprite>("Sprites/Note/Green Note");
			longOddNote = Resource.Load<Sprite>("Sprites/Note/Basic Note");
			longEvenNote = Resource.Load<Sprite>("Sprites/Note/Blue Note");
			scratchLongNote = Resource.Load<Sprite>("Sprites/Note/Green Note");
			landMine = Resource.Load<Sprite>("Sprites/Note/Orange Note");
		}

		/// <summary>
		///     노트가 생성될 레퍼런스 위치를 가져옵니다.
		/// </summary>
		private void ConfigueNoteRefPos()
		{
			Vector3 pos = transform.position;

			pos.x = GameService.Inst.Setting.position switch
			{
				Enum.Position.LEFT => -m_offset,
				Enum.Position.CENTER => 0,
				Enum.Position.RIGHT => m_offset,

				_ => throw new ArgumentOutOfRangeException()
			};

			transform.position = pos;

			xPoses = new float[9];

			xPoses[0] = m_notePositions[1].transform.position.x;
			xPoses[1] = m_notePositions[2].transform.position.x;
			xPoses[2] = m_notePositions[3].transform.position.x;
			xPoses[3] = m_notePositions[4].transform.position.x;
			xPoses[4] = m_notePositions[5].transform.position.x;
			xPoses[7] = m_notePositions[6].transform.position.x;
			xPoses[8] = m_notePositions[7].transform.position.x;

			// Scratch
			xPoses[5] = m_notePositions[0].transform.position.x;

			// Pedal
			xPoses[6] = -10;
		}

		/// <summary>
		///     노트를 그립니다.
		/// </summary>
		public void Generate()
		{
			LoadSprites();
			ConfigueNoteRefPos();

			for (int i = 0; i < 9; ++i)
			{
				Vector3 prev = Vector2.zero;
				for (int j = GameService.Inst.track.lanes[i].noteList.Count - 1; j >= 0; --j)
				{
					NoteObject n = GameService.Inst.track.lanes[i].noteList[j];
					GameObject note = Instantiate(m_notePrefab, m_noteParent);

					if (i == 5)
					{
						note.GetComponent<SpriteRenderer>().sprite = scratchNote;
					}
					else if ((i & 1) == 0)
					{
						note.GetComponent<SpriteRenderer>().sprite = oddNote;
					}
					else
					{
						note.GetComponent<SpriteRenderer>().sprite = evenNote;
					}

					note.transform.position =
						new Vector3(
							xPoses[i],
							(float)(n.beat * GameService.Inst.Setting.speed),
							note.transform.position.z);

					if (n.Extra == 1)
					{
						GameObject longNote = Instantiate(m_longNotePrefab, m_noteParent);
						if (i == 5)
						{
							longNote.GetComponent<SpriteRenderer>().sprite = scratchLongNote;
						}
						else if ((i & 1) == 0)
						{
							longNote.GetComponent<SpriteRenderer>().sprite = longOddNote;
						}
						else
						{
							longNote.GetComponent<SpriteRenderer>().sprite = longEvenNote;
						}

						longNote.transform.position = (note.transform.position + prev) * 0.5f + Vector3.up * 0.1875f;
						longNote.transform.localScale =
							new Vector3(1.0f, (note.transform.position - prev).y * 2.666666f, 1.0f);
					}

					prev = note.transform.position;
					n.Model = note;
				}
			}

			for (int i = 0; i < 9; ++i)
			{
				for (int j = GameService.Inst.track.lanes[i].mineList.Count - 1; j >= 0; --j)
				{
					NoteObject n = GameService.Inst.track.lanes[i].mineList[j];
					GameObject note = Instantiate(m_notePrefab, m_noteParent);
					note.GetComponent<SpriteRenderer>().sprite = landMine;
					note.transform.localPosition =
						new Vector2(xPoses[i], (float)(n.beat * GameService.Inst.Setting.speed));
					n.Model = note;
				}
			}
		}
	}
}
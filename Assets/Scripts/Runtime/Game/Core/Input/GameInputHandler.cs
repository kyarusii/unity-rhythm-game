using Minis;
using RGF.Game.Core.Game;
using RGF.Game.Structure;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RGF.Game.Core.Input
{
	public class GameInputHandler : MonoBehaviour, NewInput.IGameplayActions
	{
		[SerializeField] private GameSession m_game = default;

#if UNITY_EDITOR
		private void Reset()
		{
			m_game = FindObjectOfType<GameSession>();
		}
#endif

		private void OnEnable()
		{
			Singleton.Inst.input.Gameplay.SetCallbacks(this);
			Singleton.Inst.input.Gameplay.Enable();

			InputSystem.onDeviceChange += MidiRegistryCallback;
		}

		private void OnDisable()
		{
			Singleton.Inst.input.Gameplay.Disable();

			InputSystem.onDeviceChange -= MidiRegistryCallback;
		}

		private void MidiRegistryCallback(InputDevice device, InputDeviceChange change)
		{
			if (change != InputDeviceChange.Added)
			{
				return;
			}

			MidiDevice midiDevice = device as Minis.MidiDevice;
			if (midiDevice == null)
			{
				return;
			}

			midiDevice.onWillNoteOn += (note, velocity) =>
			{
				// Note that you can't use note.velocity because the state
				// hasn't been updated yet (as this is "will" event). The note
				// object is only useful to specify the target note (note
				// number, channel number, device name, etc.) Use the velocity
				// argument as an input note velocity.
				// Debug.Log(string.Format(
				// 	"Note On #{0} ({1}) vel:{2:0.00} ch:{3} dev:'{4}'",
				// 	note.noteNumber,
				// 	note.shortDisplayName,
				// 	velocity,
				// 	(note.device as Minis.MidiDevice)?.channel,
				// 	note.device.description.product
				// ));

				if (!Application.isPlaying)
				{
					return;
				}

				switch (note.noteNumber)
				{
					case 44:
						m_game.OnKeyDown(NoteKey.NOTE1);
						break;
					case 45:
						m_game.OnKeyDown(NoteKey.NOTE2);
						break;
					case 46:
						m_game.OnKeyDown(NoteKey.NOTE5);
						break;
					case 47:
						m_game.OnKeyDown(NoteKey.NOTE6);
						break;

					case 48:
						m_game.OnKeyDown(NoteKey.SCRATCH1);
						break;
					case 49:
						m_game.OnKeyDown(NoteKey.NOTE3);
						break;
					case 50:
						m_game.OnKeyDown(NoteKey.NOTE4);
						break;
					case 51:
						m_game.OnKeyDown(NoteKey.NOTE7);
						break;
				}
			};

			midiDevice.onWillNoteOff += (note) =>
			{
				if (!Application.isPlaying)
				{
					return;
				}

				switch (note.noteNumber)
				{
					case 44:
						m_game.OnKeyUp(NoteKey.NOTE1);
						break;
					case 45:
						m_game.OnKeyUp(NoteKey.NOTE2);
						break;
					case 46:
						m_game.OnKeyUp(NoteKey.NOTE5);
						break;
					case 47:
						m_game.OnKeyUp(NoteKey.NOTE6);
						break;

					case 48:
						m_game.OnKeyUp(NoteKey.SCRATCH1);
						break;
					case 49:
						m_game.OnKeyUp(NoteKey.NOTE3);
						break;
					case 50:
						m_game.OnKeyUp(NoteKey.NOTE4);
						break;
					case 51:
						m_game.OnKeyUp(NoteKey.NOTE7);
						break;
				}
				//
				// Debug.Log(string.Format(
				// 	"Note Off #{0} ({1}) ch:{2} dev:'{3}'",
				// 	note.noteNumber,
				// 	note.shortDisplayName,
				// 	(note.device as Minis.MidiDevice)?.channel,
				// 	note.device.description.product
				// ));
			};
		}

		public void OnPause(InputAction.CallbackContext context)
		{
			if (context.started)
			{
				m_game.TryPause();
			}
		}

		public void OnScratch(InputAction.CallbackContext context)
		{
			if (context.started)
			{
				m_game.OnKeyDown(NoteKey.SCRATCH1);
			}
			else if (context.canceled)
			{
				m_game.OnKeyUp(NoteKey.SCRATCH1);
			}
		}

		public void OnNote1(InputAction.CallbackContext context)
		{
			if (context.started)
			{
				m_game.OnKeyDown(NoteKey.NOTE1);
			}
			else if (context.canceled)
			{
				m_game.OnKeyUp(NoteKey.NOTE1);
			}
		}

		public void OnNote2(InputAction.CallbackContext context)
		{
			if (context.started)
			{
				m_game.OnKeyDown(NoteKey.NOTE2);
			}
			else if (context.canceled)
			{
				m_game.OnKeyUp(NoteKey.NOTE2);
			}
		}

		public void OnNote3(InputAction.CallbackContext context)
		{
			if (context.started)
			{
				m_game.OnKeyDown(NoteKey.NOTE3);
			}
			else if (context.canceled)
			{
				m_game.OnKeyUp(NoteKey.NOTE3);
			}
		}

		public void OnNote4(InputAction.CallbackContext context)
		{
			if (context.started)
			{
				m_game.OnKeyDown(NoteKey.NOTE4);
			}
			else if (context.canceled)
			{
				m_game.OnKeyUp(NoteKey.NOTE4);
			}
		}

		public void OnNote5(InputAction.CallbackContext context)
		{
			if (context.started)
			{
				m_game.OnKeyDown(NoteKey.NOTE5);
			}
			else if (context.canceled)
			{
				m_game.OnKeyUp(NoteKey.NOTE5);
			}
		}

		public void OnNote6(InputAction.CallbackContext context)
		{
			if (context.started)
			{
				m_game.OnKeyDown(NoteKey.NOTE6);
			}
			else if (context.canceled)
			{
				m_game.OnKeyUp(NoteKey.NOTE6);
			}
		}

		public void OnNote7(InputAction.CallbackContext context)
		{
			if (context.started)
			{
				m_game.OnKeyDown(NoteKey.NOTE7);
			}
			else if (context.canceled)
			{
				m_game.OnKeyUp(NoteKey.NOTE7);
			}
		}

		public void OnSpeedUp(InputAction.CallbackContext context)
		{
			if (context.started)
			{
				Message.Execute(Event.OnSpeedUp);
			}
		}

		public void OnSpeedDown(InputAction.CallbackContext context)
		{
			if (context.started)
			{
				Message.Execute(Event.OnSpeedDown);
			}
		}
	}
}
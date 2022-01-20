using UnityEngine;
using UnityEngine.InputSystem;

namespace RGF.Game.Core.Select
{
	[RequireComponent(typeof(TurnTable))]
	public sealed class SelectorInputHandler : MonoBehaviour, NewInput.ISelectorActions
	{
		[SerializeField] private TurnTable turnTable = default;

		private const float tickInterval = 0.10f;
		private const float firstTickInterval = 0.28f;

		private float lastTickTime = default;

		private bool firstDelay;
		private bool isUpPushed;
		private bool isDownPushed;

#if UNITY_EDITOR
		private void Reset()
		{
			turnTable = GetComponent<TurnTable>();
		}
#endif

		private void OnEnable()
		{
			Singleton.Inst.input.Selector.SetCallbacks(this);
			Singleton.Inst.input.Selector.Enable();
		}

		private void OnDisable()
		{
			Singleton.Inst.input.Selector.Disable();
		}

		private void Update()
		{
			if (isDownPushed)
			{
				if (firstDelay)
				{
					if (Time.realtimeSinceStartup - lastTickTime > firstTickInterval)
					{
						turnTable.OnNextSong();
						lastTickTime = Time.realtimeSinceStartup;

						firstDelay = false;
					}
				}
				else if (Time.realtimeSinceStartup - lastTickTime > tickInterval)
				{
					turnTable.OnNextSong();
					lastTickTime = Time.realtimeSinceStartup;
				}
			}

			if (isUpPushed)
			{
				if (firstDelay)
				{
					if (Time.realtimeSinceStartup - lastTickTime > firstTickInterval)
					{
						turnTable.OnPrevSong();
						lastTickTime = Time.realtimeSinceStartup;

						firstDelay = false;
					}
				}
				else if (Time.realtimeSinceStartup - lastTickTime > tickInterval)
				{
					turnTable.OnPrevSong();
					lastTickTime = Time.realtimeSinceStartup;
				}
			}

			// process keyboard alphabets by ascii index
			for (int i = 65; i <= 90; i++)
			{
#if ENABLE_INPUT_SYSTEM
				Key key = (Key)(i - 50);
				if (Keyboard.current[key].wasPressedThisFrame)
#elif ENABLE_LEGACY_INPUT_MANAGER
				KeyCode keyCode = (KeyCode)(i + 32);
				if (Input.GetKeyDown(keyCode))
#endif
				{
					char alphabet = (char)i;
					turnTable.SearchSongStartWith(alphabet);
				}
			}

			// process keyboard number pad
			for (int i = 0; i <= 9; i++)
			{
#if ENABLE_INPUT_SYSTEM
				Key key = (Key)(i + 84);
				if (Keyboard.current[key].wasPressedThisFrame)
#elif ENABLE_LEGACY_INPUT_MANAGER
				KeyCode keyCode = (KeyCode)(i + 256);
				if (Input.GetKeyDown(keyCode))
#endif
				{
					// behaviour
				}
			}

			// process function keys
			for (int i = 1; i <= 12; i++)
			{
#if ENABLE_INPUT_SYSTEM
				Key key = (Key)(i + 93);
				if (Keyboard.current[key].wasPressedThisFrame)
#elif ENABLE_LEGACY_INPUT_MANAGER
				KeyCode keyCode = (KeyCode)(i + 281);
				if (Input.GetKeyDown(keyCode))
#endif
				{
					// behaviour
				}
			}

			// process digits
			for (int i = 1; i <= 10; i++)
			{
#if ENABLE_INPUT_SYSTEM
				Key key = (Key)(i + 40);
				int number = i % 10;
				if (Keyboard.current[key].wasPressedThisFrame)
#elif ENABLE_LEGACY_INPUT_MANAGER
				int number = i % 10;
				KeyCode keyCode = (KeyCode)(number + 48);
				if (Input.GetKeyDown(keyCode))
#endif
				{
					// behaviour
				}
			}
		}


		public void OnUp(InputAction.CallbackContext context)
		{
			if (context.started)
			{
				isUpPushed = true;
				firstDelay = true;

				turnTable.OnPrevSong();
				lastTickTime = Time.realtimeSinceStartup;
			}

			else if (context.canceled)
			{
				isUpPushed = false;
				firstDelay = false;
			}
		}

		public void OnDown(InputAction.CallbackContext context)
		{
			if (context.started)
			{
				isDownPushed = true;
				firstDelay = true;

				turnTable.OnNextSong();
				lastTickTime = Time.realtimeSinceStartup;
			}
			else if (context.canceled)
			{
				isDownPushed = false;
				firstDelay = false;
			}
		}

		public void OnRight(InputAction.CallbackContext context)
		{
			if (context.started)
			{
				turnTable.OnNextPattern();
			}
		}

		public void OnLeft(InputAction.CallbackContext context)
		{
			if (context.started)
			{
				turnTable.OnPrevPattern();
			}
		}

		public void OnOption(InputAction.CallbackContext context)
		{
			if (context.started)
			{
				turnTable.OnOption();
			}
		}

		public void OnDecide(InputAction.CallbackContext context)
		{
			if (context.started)
			{
				turnTable.OnDecide();
			}
		}

		public void OnBack(InputAction.CallbackContext context)
		{
			if (context.started)
			{
				turnTable.OnBack();
			}
		}
	}
}
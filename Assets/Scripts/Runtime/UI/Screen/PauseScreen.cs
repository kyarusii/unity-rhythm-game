using DG.Tweening;
using RGF.Game;
using RGF.Game.Common;
using RGF.Game.Core.Game;
using RGF.Game.Utility;
using RGF.UI.Component;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace RGF.UI.Screen
{
	public sealed class PauseScreen : MonoBehaviour, NewInput.IPauseActions
	{
		public CanvasGroup menuGroup;
		public TextMeshProUGUI countdown;
		public TextMeshProUGUI title;
		public Image background;

		[Header("Menu")] public SelectionGroup selectionGroup;
		public GroupedButton resume;
		public GroupedButton restart;
		public GroupedButton select;
		public GroupedButton exit;

		private GameSession session;

		private void Awake()
		{
			Message.Register(Event.OnPause, OnPause);
			Message.Register(Event.OnResumeBegin, OnResumeBegin);
			Message.Register(Event.OnResumeEnd, OnResumeEnd);
			Message.Register<int>(Event.OnCountdown, OnCountdown);

			session = FindObjectOfType<GameSession>();

			resume.onClick.AddListener(session.ResumeGame);
			@select.onClick.AddListener(MoveToMusicSelect);
			exit.onClick.AddListener(GameService.Inst.Exit);
		}

		private void OnDestroy()
		{
			Message.Unregister(Event.OnPause, OnPause);
			Message.Unregister(Event.OnResumeBegin, OnResumeBegin);
			Message.Unregister(Event.OnResumeEnd, OnResumeEnd);
			Message.Unregister<int>(Event.OnCountdown, OnCountdown);

			resume.onClick.RemoveAllListeners();
			restart.onClick.RemoveAllListeners();
			@select.onClick.RemoveAllListeners();
			exit.onClick.RemoveAllListeners();
		}

		private void MoveToMusicSelect()
		{
			SceneLoader.Change(Enum.Travel.Select);
		}

		private void Start()
		{
			title.gameObject.SetActive(false);
			background.gameObject.SetActive(false);
			menuGroup.gameObject.SetActive(false);
			countdown.gameObject.SetActive(false);
		}

		private void OnEnable()
		{
			Singleton.Inst.input.Pause.SetCallbacks(this);
			Singleton.Inst.input.Pause.Enable();
		}

		private void OnDisable()
		{
			Singleton.Inst.input.Pause.Disable();
		}

		private void OnPause()
		{
			menuGroup.gameObject.SetActive(true);
			title.gameObject.SetActive(true);

			background.gameObject.SetActive(true);
			background.DOFade(0.0f, 0.0f);
			background.DOFade(1.0f, 1.0f);
		}

		private void OnResumeBegin()
		{
			menuGroup.gameObject.SetActive(false);
			countdown.gameObject.SetActive(true);

			background.DOFade(0.3f, 1.5f);
		}

		private void OnResumeEnd()
		{
			title.gameObject.SetActive(false);
			countdown.gameObject.SetActive(false);

			background.gameObject.SetActive(false);
			background.DOFade(0.0f, 0.0f);
		}

		private void OnCountdown(int second)
		{
			countdown.SetText($"{second}");
		}

		public void OnMoveUp(InputAction.CallbackContext context)
		{
			if (context.started)
			{
				if (resume.IsSelected)
				{
					exit.Select();
				}
				else if (restart.IsSelected)
				{
					resume.Select();
				}
				else if (@select.IsSelected)
				{
					restart.Select();
				}
				else if (exit.IsSelected)
				{
					@select.Select();
				}
			}
		}

		public void OnMoveDown(InputAction.CallbackContext context)
		{
			if (context.started)
			{
				if (resume.IsSelected)
				{
					restart.Select();
				}
				else if (restart.IsSelected)
				{
					@select.Select();
				}
				else if (@select.IsSelected)
				{
					exit.Select();
				}
				else if (exit.IsSelected)
				{
					resume.Select();
				}
			}
		}

		public void OnSelect(InputAction.CallbackContext context)
		{
			if (context.started)
			{
				if (resume.IsSelected)
				{
					resume.onClick.Invoke();
				}
				else if (restart.IsSelected)
				{
					restart.onClick.Invoke();
				}
				else if (@select.IsSelected)
				{
					@select.onClick.Invoke();
				}
				else if (exit.IsSelected)
				{
					exit.onClick.Invoke();
				}
			}
		}
	}
}
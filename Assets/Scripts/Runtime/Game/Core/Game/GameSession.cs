using System;
using System.Collections;
using System.Collections.Generic;
using RGF.Auth;
using RGF.Game.BMS.Model;
using RGF.Game.BMS.Object;
using RGF.Game.Common;
using RGF.Game.Core.Rail;
using RGF.Game.Core.Sound;
using RGF.Game.Structure;
using RGF.Game.Structure.Data;
using RGF.Game.Utility;
using UnityEngine;
using UnityEngine.InputSystem;
using Enum = RGF.Game.Common.Enum;
#if UNITY_EDITOR
#endif

namespace RGF.Game.Core.Game
{
	[DefaultExecutionOrder(Constant.Order.GAME_MANAGER)]
	public class GameSession : MonoBehaviour
	{
		[SerializeField] private NoteBuilder m_drawer = default;
		[SerializeField] private Speaker m_sound = default;
		[SerializeField] private Transform m_noteParent = default;

		private double _currentBPM = default;

		private double _currentScrollTime = default;
		private double _currentTime = default;

		private KeyCode[] _keyBindings = default;
		private double _stopTime = default;

		private bool forceCompleteSong = false;

		[NonSerialized] public double scrollValue = default;

		private WaitForSeconds Wait2Sec = default;

		private Track Current { get; set; }
		private event Action<Enum.JudgeType> onHandleNote;

		public Lane[] lanes;
		private List<ChangeBGObject> changeBgObjs;
		private List<NoteObject> noteObjs;
		private List<BpmObject> bpms;
		private List<StopObject> stopObjs;
		private long stackedCombo = 0;

		// Use this for initialization
		private void Start()
		{
			Initialize();
		}

		private void Update()
		{
			QuickExit();
		}

		/// <summary>
		///     Call 160 times in a second
		/// </summary>
		private void FixedUpdate()
		{
			if (!GameService.Inst.isPlaying)
			{
				return;
			}

			if (GameService.Inst.isPaused)
			{
				return;
			}

			double prevStop = 0;
			double deltaTime = Time.fixedDeltaTime;

			PlayNotes();

			_currentTime += Time.fixedDeltaTime;
			if (_stopTime > 0.0)
			{
				if (_stopTime >= Time.fixedDeltaTime)
				{
					_stopTime -= Time.fixedDeltaTime;
					return;
				}

				deltaTime -= _stopTime;
				prevStop = _stopTime;
			}

			double average = _currentBPM * deltaTime;

			ObjectBase next = null;
			bool flag = false;

			if (stopObjs.Count > 0)
			{
				next = stopObjs.Peek();
				if (next.timing < _currentScrollTime + deltaTime)
				{
					flag = true;
					average = 0;
				}
			}

			if (bpms.Count > 0)
			{
				BpmObject bpm = bpms.Peek();
				if (next == null)
				{
					next = bpm;
				}
				else if (bpm.beat <= next.beat)
				{
					next = bpm;
				}

				if (next.timing < _currentScrollTime + deltaTime)
				{
					flag = true;
					average = 0;
				}
			}

			double sub = 0;
			double prevTime = _currentScrollTime;
			while (next != null && next.timing + _stopTime < _currentScrollTime + Time.fixedDeltaTime)
			{
				// bpm obj
				if (next is BpmObject b)
				{
					double diff = b.timing - prevTime;
					average += _currentBPM * diff;
					_currentBPM = b.Bpm;

					prevTime = b.timing;
					bpms.RemoveLast();
				}

				// stop obj
				if (next is StopObject stop)
				{
					double diff = stop.timing - prevTime;
					average += _currentBPM * diff;
					prevTime = stop.timing;

					double duration = Current.stopDurations[stop.Key] / _currentBPM * 240;
					_stopTime += duration;
					stopObjs.RemoveLast();

					if (prevTime + _stopTime >= _currentScrollTime + deltaTime)
					{
						double sdiff = _currentScrollTime + deltaTime - prevTime;
						sub += sdiff;
						_stopTime -= sdiff;
						break;
					}
				}

				next = null;

				if (stopObjs.Count > 0)
				{
					next = stopObjs.Peek();
				}

				if (bpms.Count > 0)
				{
					BpmObject bpm = bpms.Peek();
					if (next == null)
					{
						next = bpm;
					}
					else if (bpm.beat <= next.beat)
					{
						next = bpm;
					}
				}
			}

			deltaTime -= sub;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
			if (deltaTime < 0)
			{
				Debug.LogError($"deltaTime이 음수입니다. {deltaTime}");
			}
#endif

			if (flag && prevTime <= _currentScrollTime + deltaTime)
			{
				average += _currentBPM * (_currentScrollTime + deltaTime - prevTime);
			}


			_stopTime -= prevStop;
			average /= 60;
			_currentScrollTime += deltaTime;

			scrollValue += average * GameService.Inst.Setting.speed;

			// 스크롤링
			Vector3 scrollPos = m_noteParent.transform.position;
			scrollPos.y = (float)-scrollValue;
			m_noteParent.transform.position = scrollPos;
		}

		private void OnEnable()
		{
			result = new GameResult();

			noteData = new NoteData();
			scoreData = new ScoreData();
			healthData = new HealthData();
			feverData = new FeverData();

			onHandleNote += noteData.OnHandleNote;
			onHandleNote += feverData.OnHandleNote;
			onHandleNote += scoreData.OnHandleNote;
			onHandleNote += healthData.OnHandleNote;

			Message.Register<int>(Event.OnFeverIncrease, OnFeverIncrease);
			Message.Register(Event.OnFeverFinished, OnFeverFinished);

			Message.Register(Event.OnSpeedDown, OnSpeedDown);
			Message.Register(Event.OnSpeedUp, OnSpeedUp);
		}

		private void OnDisable()
		{
			onHandleNote -= noteData.OnHandleNote;
			onHandleNote -= scoreData.OnHandleNote;
			onHandleNote -= healthData.OnHandleNote;

			Message.Unregister<int>(Event.OnFeverIncrease, OnFeverIncrease);
			Message.Unregister(Event.OnFeverFinished, OnFeverFinished);

			Message.Unregister(Event.OnSpeedDown, OnSpeedDown);
			Message.Unregister(Event.OnSpeedUp, OnSpeedUp);
		}

		private void OnSpeedDown()
		{
			// Game.Instance.Setting.speed -= 0.5f;
		}

		private void OnSpeedUp()
		{
			// Game.Instance.Setting.speed += 0.5f;
		}

		private void Initialize()
		{
#if UNITY_EDITOR || DEVELOPMENT_BUILD
			// Game.Instance.option.autoPlay = EditorPrefs.GetBool("Rhythm.AutoPlay");
#else
			// Game.Instance.option.autoPlay = false;
#endif
			GameService.Inst.isPaused = true;

			KeySettingObject settingObject = KeySettingObject.Load();
			_keyBindings = settingObject.Keys;

			Wait2Sec = new WaitForSeconds(2.0f);

			StartCoroutine(PreLoad());
		}

		private void QuickExit()
		{
#if UNITY_EDITOR || DEVELOPMENT_BUILD

#if ENABLE_INPUT_SYSTEM
			if (Keyboard.current.f10Key.wasPressedThisFrame)
#else
			if (Input.GetKeyDown(KeyCode.F10))
#endif
			{
				forceCompleteSong = true;
				Debug.Log("Force Complete this song.");
			}
#endif
		}

		public void OnKeyDown(NoteKey key)
		{
			int laneIndex = (int)key;

			if (key.IsScratch())
			{
				laneIndex = 5;
			}

			bool hasNoteInLane = lanes[laneIndex].noteList.Count > 0;
			NoteObject targetNote = hasNoteInLane
				? lanes[laneIndex].noteList.Peek()
				: null;

			Message.Execute<NoteKey>(Event.OnKeyDown, key);

			// 노트가 존재하고 Extra가 1이 아닐 때
			if (targetNote != null && targetNote.Extra != 1)
			{
				// 소리 재생
				m_sound.PlayKeySound(targetNote.KeySound);

				// 판정
				if (Judge.Evaluate(targetNote, _currentTime) != Enum.JudgeType.IGNORE)
				{
					// 노트 프로세스
					HandleNote(lanes[laneIndex], laneIndex);
				}
			}
		}

		public void OnKeyUp(NoteKey key)
		{
			int laneIndex = (int)key;

			if (key.IsScratch())
			{
				laneIndex = 5;
			}

			// 현재 시간에 해당하는 노트 피킹 
			NoteObject n = lanes[laneIndex].noteList.Count > 0
				? lanes[laneIndex].noteList.Peek()
				: null;

			if (n != null && n.Extra == 1)
			{
				// 사운드 재생
				m_sound.PlayKeySound(n.KeySound);

				// 노트 프로세스
				HandleNote(lanes[laneIndex], laneIndex);
			}

			// m_keyPressed[lineIdx].DOFade(0f, m_duration * 0.4f);

			Message.Execute<NoteKey>(Event.OnKeyUp, key);
		}

		private IEnumerator PreLoad()
		{
			TimeCheck.Start();

			Current = GameService.Inst.track;
			m_sound.wavNoteMap = Current.wavNoteMap;

			lanes = new Lane[9];
			for (int i = 0; i < lanes.Length; i++)
			{
				lanes[i] = Current.lanes[i].Clone();
			}

			bpms = Current.GetBpms();
			stopObjs = Current.GetStops();
			noteObjs = Current.GetNotes();
			changeBgObjs = Current.GetChangeBGs();

			// 오디오 클립 준비
			m_sound.PrepareAudioClips();

			// 노트 생성
			m_drawer.Generate();

			noteData.totalNote = Current.noteCount;
			_currentBPM = bpms.Peek().Bpm;
			bpms.RemoveLast();

			yield return new WaitUntil(() => m_sound.isPrepared);

			// 준비 끝

			Message.Execute(Event.OnFadeIn, 1.0f);

			Debug.Log("2초 후 게임 시작");

			yield return Wait2Sec;

			_currentTime += GameService.Inst.Setting.judgemenetSyncOffset / 1000.0f;


			// 게임 시작
			GameService.Inst.isPaused = false;
			GameService.Inst.isPlaying = true;
			GameService.Inst.RestPauseCount = 3;

			stackedCombo = GameData.Inst.currentCombo;

			StartCoroutine(WaitForGameFinishCoroutine());
		}

		private void HandleNote(Lane l, int idx, float volume = 1.0f)
		{
			if (l.noteList.Count <= 0)
			{
				return;
			}

			NoteObject n = l.noteList.Peek();

			// 노트 끄기
			n.Model.SetActive(false);
			l.noteList.RemoveLast();

			// 노트 판정
			Enum.JudgeType judge = Judge.Evaluate(n, _currentTime);

			// POOR 인 경우
			if (l.noteList.Count > 0 && l.noteList.Peek().Extra == 1 && judge == Enum.JudgeType.POOR)
			{
				l.noteList.Peek().Model.SetActive(false);
				l.noteList.RemoveLast();
			}

			if (judge == Enum.JudgeType.POOR
			    || judge == Enum.JudgeType.IGNORE)
			{
				stackedCombo = 0;
			}

			// 무시하는 노트면
			if (n.Extra == 1 && judge == Enum.JudgeType.IGNORE)
			{
				// 강제로 POOR 입력
				judge = Enum.JudgeType.POOR;
				// 비주얼 끄기
				n.Model.SetActive(false);
				l.noteList.RemoveLast();
			}

			// BAD 판정 이상인 경우
			if (judge > Enum.JudgeType.BAD)
			{
				NoteKey note = (NoteKey)idx;
				Message.Execute<NoteKey>(Event.OnHandleNote, note);
			}

			onHandleNote?.Invoke(judge);

			int gap = (int)(n.timing - _currentTime) * 1000;
			if (gap > 0)
			{
				scoreData.gapLate += gap;
				scoreData.lateCount += 1;
			}
			else
			{
				scoreData.gapEarly += gap;
				scoreData.earlyCount += 1;
			}

			Message.Execute<long>(Event.OnScoreUpdate, scoreData.score);
			Message.Execute<long>(Event.OnComboUpdate, scoreData.combo + stackedCombo);
			Message.Execute<int>(Event.OnExistGap, gap);
			Message.Execute<Enum.JudgeType>(Event.OnJudgeUpdate, judge);

			Message.Execute<float>(Event.OnFeverUpdate, feverData.gauge / 100f);

			LifeCheck();
		}

		private void OnFeverIncrease(int fever)
		{
			scoreData.fever = fever;

			// Debug.Log($"Fever : {fever}");
		}

		private void OnFeverFinished()
		{
			scoreData.fever = 1;
			// Debug.Log("Fever End");
		}

		private void LifeCheck()
		{
			if (healthData.health <= 0f)
			{
				// Failed
			}
		}

		private void PlayNotes()
		{
			while (noteObjs.Count > 0 && noteObjs.Peek().timing <= _currentTime)
			{
				int keySound = noteObjs.Peek().KeySound;
				m_sound.PlayKeySound(keySound);
				noteObjs.RemoveLast();
			}

#if UNITY_EDITOR || DEVELOPMENT_BUILD
			if (GameService.Inst.autoPlay)
			{
				for (int i = 0; i < lanes.Length; ++i)
				{
					Lane l = lanes[i];
					while (l.noteList.Count > 0 && l.noteList.Peek().timing <= _currentTime)
					{
						m_sound.PlayKeySound(l.noteList.Peek().KeySound);
						HandleNote(l, i);

						NoteKey noteKey = (NoteKey)i;
						Message.Execute(Event.OnKeyDownAuto, noteKey);
					}

					while (l.mineList.Count > 0 &&
					       Judge.Evaluate(l.mineList.Peek(), _currentTime) == Enum.JudgeType.POOR)
					{
						NoteObject n = l.mineList.Peek();
						n.Model.SetActive(false);
						l.mineList.RemoveLast();
					}
				}
			}
			else
#endif
			{
				for (int i = 0; i < lanes.Length; ++i)
				{
					Lane l = lanes[i];
					while (l.noteList.Count > 0 &&
					       Judge.Evaluate(l.noteList.Peek(), _currentTime) == Enum.JudgeType.POOR)
					{
						NoteObject n = l.noteList.Peek();
						m_sound.PlayKeySound(n.KeySound, 0.3f);
						HandleNote(l, i, 0.3f);
					}

					while (l.mineList.Count > 0 &&
					       Judge.Evaluate(l.mineList.Peek(), _currentTime) == Enum.JudgeType.POOR)
					{
						NoteObject n = l.mineList.Peek();
						n.Model.SetActive(false);
						l.mineList.RemoveLast();
					}
				}
			}
		}

		public IEnumerator WaitForGameFinishCoroutine()
		{
			while (true)
			{
				bool allNoteProcessed = noteData.hitCount >= Current.noteCount;
				bool songDone = allNoteProcessed;

				if (songDone || forceCompleteSong)
				{
					Debug.Log($"All Note Processed : {Time.realtimeSinceStartup:0}s");

					if (!forceCompleteSong)
					{
						// @TODO :: MAX COMBO EFFECT
						Message.Execute("OnMaxCombo");

						yield return m_sound.WaitForAudioEnd();
						Debug.Log($"Audio Finished : {Time.realtimeSinceStartup:0}s");

						yield return new WaitForSeconds(3.0f);
					}

					// Game Finished

					// calculate
					scoreData.accuracyAverage = scoreData.accuracySum / noteData.hitCount;

					scoreData.gapEarlyAverage = scoreData.gapEarly / (double)scoreData.earlyCount;
					scoreData.gapLateAverage = scoreData.gapLate / (double)scoreData.lateCount;

					// Transfer Data
					result.healthData = healthData;
					result.noteData = noteData;
					result.scoreData = scoreData;
					result.playOption = GameService.Inst.option;

					result.songInfo = GameService.Inst.track.title;

					GameService.Inst.isPlaying = false;
					GameService.Inst.SetResult(result);
					GameData.Inst.currentCombo = stackedCombo + scoreData.combo;

					AuthUtil.Get().IngestStat("Combo", (int)GameData.Inst.currentCombo);

					yield return Wait2Sec;

					Message.Execute(Event.OnFadeOut, 1.0f);

					yield return new WaitForSeconds(0.35f);

					SceneLoader.Change(Enum.Travel.Result);

					yield break;
				}

				// every 2 second
				yield return Wait2Sec;
			}
		}

		public void TryPause()
		{
			if (!GameService.Inst.isPlaying)
			{
				PauseNotAllowed(EPauseFailureReason.NotInPlaying);
				return;
			}

			if (GameService.Inst.isPaused)
			{
				PauseNotAllowed(EPauseFailureReason.AlreadyPaused);
				return;
			}

			if (GameService.Inst.lastResumeTime + 10 > Time.realtimeSinceStartup)
			{
				PauseNotAllowed(EPauseFailureReason.MinimumIntervalNotAcquired);
				return;
			}

			if (GameService.Inst.RestPauseCount < 1)
			{
				PauseNotAllowed(EPauseFailureReason.TooManyPause);
				return;
			}

			PauseGame();
		}

		public void PauseGame()
		{
			GameService.Inst.RestPauseCount--;
			GameService.Inst.isPaused = true;

			m_sound.Pause();

			Message.Execute(Event.OnPause);
		}

		public void ResumeGame()
		{
			if (!GameService.Inst.isPlaying)
			{
				Debug.LogWarning("Game Is Not Playing");
				return;
			}

			if (!GameService.Inst.isPaused)
			{
				Debug.LogWarning("Game Is Not Paused");
				return;
			}

			if (GameService.Inst.isResuming)
			{
				Debug.LogWarning("Game Is Already Resuming");
				return;
			}

			GameService.Inst.isResuming = true;
			GameService.Inst.lastResumeTime = Time.realtimeSinceStartup;

			Message.Execute(Event.OnResumeBegin);
			StartCoroutine(Resume_Coroutine());
		}

		private void PauseNotAllowed(EPauseFailureReason reason)
		{
			Debug.LogError(reason);
		}

		private IEnumerator Resume_Coroutine()
		{
			WaitForSeconds waiter = new WaitForSeconds(1.0f);
			for (int i = 3; i > 0; i--)
			{
				Message.Execute<int>(Event.OnCountdown, i);
				yield return waiter;
			}

			m_sound.Resume();

			GameService.Inst.isPaused = false;
			GameService.Inst.isResuming = false;

			Message.Execute(Event.OnResumeEnd);
		}

		#region Result Object

		private GameResult result;
		private NoteData noteData;
		private ScoreData scoreData;
		private HealthData healthData;
		private FeverData feverData;

		#endregion
	}

	public enum EPauseFailureReason
	{
		Invalid,
		NotInPlaying,
		AlreadyPaused,
		TooManyPause,
		MinimumIntervalNotAcquired
	}
}
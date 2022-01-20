using System;
using System.Collections;
using UnityEngine;

namespace UniRx
{
#if UniRxLibrary
    public static partial class SchedulerUnity
    {
#else
	public static partial class Scheduler
	{
		public static void SetDefaultForUnity()
		{
			DefaultSchedulers.ConstantTimeOperations = Immediate;
			DefaultSchedulers.TailRecursion = Immediate;
			DefaultSchedulers.Iteration = CurrentThread;
			DefaultSchedulers.TimeBasedOperations = MainThread;
			DefaultSchedulers.AsyncConversions = ThreadPool;
		}
#endif
		private static IScheduler mainThread;

		private static IScheduler mainThreadIgnoreTimeScale;

		private static IScheduler mainThreadFixedUpdate;

		private static IScheduler mainThreadEndOfFrame;

        /// <summary>
        ///     Unity native MainThread Queue Scheduler. Run on mainthread and delayed on coroutine update loop, elapsed time is
        ///     calculated based on Time.time.
        /// </summary>
        public static IScheduler MainThread => mainThread ?? (mainThread = new MainThreadScheduler());

        /// <summary>
        ///     Another MainThread scheduler, delay elapsed time is calculated based on Time.unscaledDeltaTime.
        /// </summary>
        public static IScheduler MainThreadIgnoreTimeScale => mainThreadIgnoreTimeScale ??
                                                              (mainThreadIgnoreTimeScale =
                                                                  new IgnoreTimeScaleMainThreadScheduler());

        /// <summary>
        ///     Run on fixed update mainthread, delay elapsed time is calculated based on Time.fixedTime.
        /// </summary>
        public static IScheduler MainThreadFixedUpdate =>
			mainThreadFixedUpdate ?? (mainThreadFixedUpdate = new FixedUpdateMainThreadScheduler());

        /// <summary>
        ///     Run on end of frame mainthread, delay elapsed time is calculated based on Time.deltaTime.
        /// </summary>
        public static IScheduler MainThreadEndOfFrame =>
			mainThreadEndOfFrame ?? (mainThreadEndOfFrame = new EndOfFrameMainThreadScheduler());

		private class MainThreadScheduler : IScheduler, ISchedulerPeriodic, ISchedulerQueueing
		{
			private readonly Action<object> scheduleAction;

			public MainThreadScheduler()
			{
				MainThreadDispatcher.Initialize();
				scheduleAction = Schedule;
			}

			public DateTimeOffset Now => Scheduler.Now;

			public IDisposable Schedule(Action action)
			{
				BooleanDisposable d = new BooleanDisposable();
				MainThreadDispatcher.Post(scheduleAction, Tuple.Create(d, action));
				return d;
			}

			public IDisposable Schedule(TimeSpan dueTime, Action action)
			{
				BooleanDisposable d = new BooleanDisposable();
				TimeSpan time = Normalize(dueTime);

				MainThreadDispatcher.SendStartCoroutine(DelayAction(time, action, d));

				return d;
			}

			public IDisposable SchedulePeriodic(TimeSpan period, Action action)
			{
				BooleanDisposable d = new BooleanDisposable();
				TimeSpan time = Normalize(period);

				MainThreadDispatcher.SendStartCoroutine(PeriodicAction(time, action, d));

				return d;
			}

			public void ScheduleQueueing<T>(ICancelable cancel, T state, Action<T> action)
			{
				MainThreadDispatcher.Post(QueuedAction<T>.Instance, Tuple.Create(cancel, state, action));
			}

			// delay action is run in StartCoroutine
			// Okay to action run synchronous and guaranteed run on MainThread
			private IEnumerator DelayAction(TimeSpan dueTime, Action action, ICancelable cancellation)
			{
				// zero == every frame
				if (dueTime == TimeSpan.Zero)
				{
					yield return null; // not immediately, run next frame
				}
				else
				{
					yield return new WaitForSeconds((float) dueTime.TotalSeconds);
				}

				if (cancellation.IsDisposed)
				{
					yield break;
				}

				MainThreadDispatcher.UnsafeSend(action);
			}

			private IEnumerator PeriodicAction(TimeSpan period, Action action, ICancelable cancellation)
			{
				// zero == every frame
				if (period == TimeSpan.Zero)
				{
					while (true)
					{
						yield return null; // not immediately, run next frame
						if (cancellation.IsDisposed)
						{
							yield break;
						}

						MainThreadDispatcher.UnsafeSend(action);
					}
				}

				float seconds = (float) (period.TotalMilliseconds / 1000.0);
				WaitForSeconds yieldInstruction = new WaitForSeconds(seconds); // cache single instruction object

				while (true)
				{
					yield return yieldInstruction;
					if (cancellation.IsDisposed)
					{
						yield break;
					}

					MainThreadDispatcher.UnsafeSend(action);
				}
			}

			private void Schedule(object state)
			{
				Tuple<BooleanDisposable, Action> t = (Tuple<BooleanDisposable, Action>) state;
				if (!t.Item1.IsDisposed)
				{
					t.Item2();
				}
			}

			public IDisposable Schedule(DateTimeOffset dueTime, Action action)
			{
				return Schedule(dueTime - Now, action);
			}

			private void ScheduleQueueing<T>(object state)
			{
				Tuple<ICancelable, T, Action<T>> t = (Tuple<ICancelable, T, Action<T>>) state;
				if (!t.Item1.IsDisposed)
				{
					t.Item3(t.Item2);
				}
			}

			private static class QueuedAction<T>
			{
				public static readonly Action<object> Instance = Invoke;

				public static void Invoke(object state)
				{
					Tuple<ICancelable, T, Action<T>> t = (Tuple<ICancelable, T, Action<T>>) state;

					if (!t.Item1.IsDisposed)
					{
						t.Item3(t.Item2);
					}
				}
			}
		}

		private class IgnoreTimeScaleMainThreadScheduler : IScheduler, ISchedulerPeriodic, ISchedulerQueueing
		{
			private readonly Action<object> scheduleAction;

			public IgnoreTimeScaleMainThreadScheduler()
			{
				MainThreadDispatcher.Initialize();
				scheduleAction = Schedule;
			}

			public DateTimeOffset Now => Scheduler.Now;

			public IDisposable Schedule(Action action)
			{
				BooleanDisposable d = new BooleanDisposable();
				MainThreadDispatcher.Post(scheduleAction, Tuple.Create(d, action));
				return d;
			}

			public IDisposable Schedule(TimeSpan dueTime, Action action)
			{
				BooleanDisposable d = new BooleanDisposable();
				TimeSpan time = Normalize(dueTime);

				MainThreadDispatcher.SendStartCoroutine(DelayAction(time, action, d));

				return d;
			}

			public IDisposable SchedulePeriodic(TimeSpan period, Action action)
			{
				BooleanDisposable d = new BooleanDisposable();
				TimeSpan time = Normalize(period);

				MainThreadDispatcher.SendStartCoroutine(PeriodicAction(time, action, d));

				return d;
			}

			public void ScheduleQueueing<T>(ICancelable cancel, T state, Action<T> action)
			{
				MainThreadDispatcher.Post(QueuedAction<T>.Instance, Tuple.Create(cancel, state, action));
			}

			private IEnumerator DelayAction(TimeSpan dueTime, Action action, ICancelable cancellation)
			{
				if (dueTime == TimeSpan.Zero)
				{
					yield return null;
					if (cancellation.IsDisposed)
					{
						yield break;
					}

					MainThreadDispatcher.UnsafeSend(action);
				}
				else
				{
					float elapsed = 0f;
					float dt = (float) dueTime.TotalSeconds;
					while (true)
					{
						yield return null;
						if (cancellation.IsDisposed)
						{
							break;
						}

						elapsed += Time.unscaledDeltaTime;
						if (elapsed >= dt)
						{
							MainThreadDispatcher.UnsafeSend(action);
							break;
						}
					}
				}
			}

			private IEnumerator PeriodicAction(TimeSpan period, Action action, ICancelable cancellation)
			{
				// zero == every frame
				if (period == TimeSpan.Zero)
				{
					while (true)
					{
						yield return null; // not immediately, run next frame
						if (cancellation.IsDisposed)
						{
							yield break;
						}

						MainThreadDispatcher.UnsafeSend(action);
					}
				}

				float elapsed = 0f;
				float dt = (float) period.TotalSeconds;
				while (true)
				{
					yield return null;
					if (cancellation.IsDisposed)
					{
						break;
					}

					elapsed += Time.unscaledDeltaTime;
					if (elapsed >= dt)
					{
						MainThreadDispatcher.UnsafeSend(action);
						elapsed = 0;
					}
				}
			}

			private void Schedule(object state)
			{
				Tuple<BooleanDisposable, Action> t = (Tuple<BooleanDisposable, Action>) state;
				if (!t.Item1.IsDisposed)
				{
					t.Item2();
				}
			}

			public IDisposable Schedule(DateTimeOffset dueTime, Action action)
			{
				return Schedule(dueTime - Now, action);
			}

			private static class QueuedAction<T>
			{
				public static readonly Action<object> Instance = Invoke;

				public static void Invoke(object state)
				{
					Tuple<ICancelable, T, Action<T>> t = (Tuple<ICancelable, T, Action<T>>) state;

					if (!t.Item1.IsDisposed)
					{
						t.Item3(t.Item2);
					}
				}
			}
		}

		private class FixedUpdateMainThreadScheduler : IScheduler, ISchedulerPeriodic, ISchedulerQueueing
		{
			public FixedUpdateMainThreadScheduler()
			{
				MainThreadDispatcher.Initialize();
			}

			public DateTimeOffset Now => Scheduler.Now;

			public IDisposable Schedule(Action action)
			{
				return Schedule(TimeSpan.Zero, action);
			}

			public IDisposable Schedule(TimeSpan dueTime, Action action)
			{
				BooleanDisposable d = new BooleanDisposable();
				TimeSpan time = Normalize(dueTime);

				MainThreadDispatcher.StartFixedUpdateMicroCoroutine(DelayAction(time, action, d));

				return d;
			}

			public IDisposable SchedulePeriodic(TimeSpan period, Action action)
			{
				BooleanDisposable d = new BooleanDisposable();
				TimeSpan time = Normalize(period);

				MainThreadDispatcher.StartFixedUpdateMicroCoroutine(PeriodicAction(time, action, d));

				return d;
			}

			public void ScheduleQueueing<T>(ICancelable cancel, T state, Action<T> action)
			{
				MainThreadDispatcher.StartFixedUpdateMicroCoroutine(ImmediateAction(state, action, cancel));
			}

			private IEnumerator ImmediateAction<T>(T state, Action<T> action, ICancelable cancellation)
			{
				yield return null;
				if (cancellation.IsDisposed)
				{
					yield break;
				}

				MainThreadDispatcher.UnsafeSend(action, state);
			}

			private IEnumerator DelayAction(TimeSpan dueTime, Action action, ICancelable cancellation)
			{
				if (dueTime == TimeSpan.Zero)
				{
					yield return null;
					if (cancellation.IsDisposed)
					{
						yield break;
					}

					MainThreadDispatcher.UnsafeSend(action);
				}
				else
				{
					float startTime = Time.fixedTime;
					float dt = (float) dueTime.TotalSeconds;
					while (true)
					{
						yield return null;
						if (cancellation.IsDisposed)
						{
							break;
						}

						float elapsed = Time.fixedTime - startTime;
						if (elapsed >= dt)
						{
							MainThreadDispatcher.UnsafeSend(action);
							break;
						}
					}
				}
			}

			private IEnumerator PeriodicAction(TimeSpan period, Action action, ICancelable cancellation)
			{
				// zero == every frame
				if (period == TimeSpan.Zero)
				{
					while (true)
					{
						yield return null;
						if (cancellation.IsDisposed)
						{
							yield break;
						}

						MainThreadDispatcher.UnsafeSend(action);
					}
				}

				float startTime = Time.fixedTime;
				float dt = (float) period.TotalSeconds;
				while (true)
				{
					yield return null;
					if (cancellation.IsDisposed)
					{
						break;
					}

					float ft = Time.fixedTime;
					float elapsed = ft - startTime;
					if (elapsed >= dt)
					{
						MainThreadDispatcher.UnsafeSend(action);
						startTime = ft;
					}
				}
			}

			public IDisposable Schedule(DateTimeOffset dueTime, Action action)
			{
				return Schedule(dueTime - Now, action);
			}
		}

		private class EndOfFrameMainThreadScheduler : IScheduler, ISchedulerPeriodic, ISchedulerQueueing
		{
			public EndOfFrameMainThreadScheduler()
			{
				MainThreadDispatcher.Initialize();
			}

			public DateTimeOffset Now => Scheduler.Now;

			public IDisposable Schedule(Action action)
			{
				return Schedule(TimeSpan.Zero, action);
			}

			public IDisposable Schedule(TimeSpan dueTime, Action action)
			{
				BooleanDisposable d = new BooleanDisposable();
				TimeSpan time = Normalize(dueTime);

				MainThreadDispatcher.StartEndOfFrameMicroCoroutine(DelayAction(time, action, d));

				return d;
			}

			public IDisposable SchedulePeriodic(TimeSpan period, Action action)
			{
				BooleanDisposable d = new BooleanDisposable();
				TimeSpan time = Normalize(period);

				MainThreadDispatcher.StartEndOfFrameMicroCoroutine(PeriodicAction(time, action, d));

				return d;
			}

			public void ScheduleQueueing<T>(ICancelable cancel, T state, Action<T> action)
			{
				MainThreadDispatcher.StartEndOfFrameMicroCoroutine(ImmediateAction(state, action, cancel));
			}

			private IEnumerator ImmediateAction<T>(T state, Action<T> action, ICancelable cancellation)
			{
				yield return null;
				if (cancellation.IsDisposed)
				{
					yield break;
				}

				MainThreadDispatcher.UnsafeSend(action, state);
			}

			private IEnumerator DelayAction(TimeSpan dueTime, Action action, ICancelable cancellation)
			{
				if (dueTime == TimeSpan.Zero)
				{
					yield return null;
					if (cancellation.IsDisposed)
					{
						yield break;
					}

					MainThreadDispatcher.UnsafeSend(action);
				}
				else
				{
					float elapsed = 0f;
					float dt = (float) dueTime.TotalSeconds;
					while (true)
					{
						yield return null;
						if (cancellation.IsDisposed)
						{
							break;
						}

						elapsed += Time.deltaTime;
						if (elapsed >= dt)
						{
							MainThreadDispatcher.UnsafeSend(action);
							break;
						}
					}
				}
			}

			private IEnumerator PeriodicAction(TimeSpan period, Action action, ICancelable cancellation)
			{
				// zero == every frame
				if (period == TimeSpan.Zero)
				{
					while (true)
					{
						yield return null;
						if (cancellation.IsDisposed)
						{
							yield break;
						}

						MainThreadDispatcher.UnsafeSend(action);
					}
				}

				float elapsed = 0f;
				float dt = (float) period.TotalSeconds;
				while (true)
				{
					yield return null;
					if (cancellation.IsDisposed)
					{
						break;
					}

					elapsed += Time.deltaTime;
					if (elapsed >= dt)
					{
						MainThreadDispatcher.UnsafeSend(action);
						elapsed = 0;
					}
				}
			}

			public IDisposable Schedule(DateTimeOffset dueTime, Action action)
			{
				return Schedule(dueTime - Now, action);
			}
		}
	}
}
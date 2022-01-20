#if !UNITY_METRO

using System;
using System.Collections.Generic;
using System.Threading;
using UniRx.InternalUtil;

namespace UniRx
{
	public static partial class Scheduler
	{
		public static readonly IScheduler ThreadPool = new ThreadPoolScheduler();

		private class ThreadPoolScheduler : IScheduler, ISchedulerPeriodic
		{
			public DateTimeOffset Now => Scheduler.Now;

			public IDisposable Schedule(Action action)
			{
				BooleanDisposable d = new BooleanDisposable();

				System.Threading.ThreadPool.QueueUserWorkItem(_ =>
				{
					if (!d.IsDisposed)
					{
						action();
					}
				});

				return d;
			}

			public IDisposable Schedule(TimeSpan dueTime, Action action)
			{
				return new Timer(dueTime, action);
			}

			public IDisposable SchedulePeriodic(TimeSpan period, Action action)
			{
				return new PeriodicTimer(period, action);
			}

			public IDisposable Schedule(DateTimeOffset dueTime, Action action)
			{
				return Schedule(dueTime - Now, action);
			}

			public void ScheduleQueueing<T>(ICancelable cancel, T state, Action<T> action)
			{
				System.Threading.ThreadPool.QueueUserWorkItem(callBackState =>
				{
					if (!cancel.IsDisposed)
					{
						action((T) callBackState);
					}
				}, state);
			}

			// timer was borrwed from Rx Official

			private sealed class Timer : IDisposable
			{
				private static readonly HashSet<System.Threading.Timer>
					s_timers = new HashSet<System.Threading.Timer>();

				private readonly SingleAssignmentDisposable _disposable;

				private Action _action;

				private readonly bool _hasAdded;
				private bool _hasRemoved;
				private System.Threading.Timer _timer;

				public Timer(TimeSpan dueTime, Action action)
				{
					_disposable = new SingleAssignmentDisposable();
					_disposable.Disposable = Disposable.Create(Unroot);

					_action = action;
					_timer = new System.Threading.Timer(Tick, null, dueTime,
						TimeSpan.FromMilliseconds(Timeout.Infinite));

					lock (s_timers)
					{
						if (!_hasRemoved)
						{
							s_timers.Add(_timer);

							_hasAdded = true;
						}
					}
				}

				public void Dispose()
				{
					_disposable.Dispose();
				}

				private void Tick(object state)
				{
					try
					{
						if (!_disposable.IsDisposed)
						{
							_action();
						}
					}
					finally
					{
						Unroot();
					}
				}

				private void Unroot()
				{
					_action = Stubs.Nop;

					System.Threading.Timer timer = default(System.Threading.Timer);

					lock (s_timers)
					{
						if (!_hasRemoved)
						{
							timer = _timer;
							_timer = null;

							if (_hasAdded && timer != null)
							{
								s_timers.Remove(timer);
							}

							_hasRemoved = true;
						}
					}

					if (timer != null)
					{
						timer.Dispose();
					}
				}
			}

			private sealed class PeriodicTimer : IDisposable
			{
				private static readonly HashSet<System.Threading.Timer>
					s_timers = new HashSet<System.Threading.Timer>();

				private readonly AsyncLock _gate;

				private Action _action;
				private System.Threading.Timer _timer;

				public PeriodicTimer(TimeSpan period, Action action)
				{
					_action = action;
					_timer = new System.Threading.Timer(Tick, null, period, period);
					_gate = new AsyncLock();

					lock (s_timers)
					{
						s_timers.Add(_timer);
					}
				}

				public void Dispose()
				{
					System.Threading.Timer timer = default(System.Threading.Timer);

					lock (s_timers)
					{
						timer = _timer;
						_timer = null;

						if (timer != null)
						{
							s_timers.Remove(timer);
						}
					}

					if (timer != null)
					{
						timer.Dispose();
						_action = Stubs.Nop;
					}
				}

				private void Tick(object state)
				{
					_gate.Wait(() => { _action(); });
				}
			}
		}
	}
}

#endif
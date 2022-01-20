using System;

namespace UniRx.Operators
{
	internal class SubscribeOnObservable<T> : OperatorObservableBase<T>
	{
		private readonly IScheduler scheduler;
		private readonly IObservable<T> source;

		public SubscribeOnObservable(IObservable<T> source, IScheduler scheduler)
			: base(scheduler == Scheduler.CurrentThread || source.IsRequiredSubscribeOnCurrentThread())
		{
			this.source = source;
			this.scheduler = scheduler;
		}

		protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
		{
			SingleAssignmentDisposable m = new SingleAssignmentDisposable();
			SerialDisposable d = new SerialDisposable();
			d.Disposable = m;

			m.Disposable = scheduler.Schedule(() =>
			{
				d.Disposable = new ScheduledDisposable(scheduler, source.Subscribe(observer));
			});

			return d;
		}
	}
}
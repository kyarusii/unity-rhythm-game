using System;

namespace UniRx
{
    /// <summary>
    ///     Notify boolean flag.
    /// </summary>
    public class BooleanNotifier : IObservable<bool>
	{
		private readonly Subject<bool> boolTrigger = new Subject<bool>();

		private bool boolValue;

        /// <summary>
        ///     Setup initial flag.
        /// </summary>
        public BooleanNotifier(bool initialValue = false)
		{
			Value = initialValue;
		}

		/// <summary>Current flag value</summary>
		public bool Value {
			get => boolValue;
			set
			{
				boolValue = value;
				boolTrigger.OnNext(value);
			}
		}


        /// <summary>
        ///     Subscribe observer.
        /// </summary>
        public IDisposable Subscribe(IObserver<bool> observer)
		{
			return boolTrigger.Subscribe(observer);
		}

        /// <summary>
        ///     Set and raise true if current value isn't true.
        /// </summary>
        public void TurnOn()
		{
			if (Value != true)
			{
				Value = true;
			}
		}

        /// <summary>
        ///     Set and raise false if current value isn't false.
        /// </summary>
        public void TurnOff()
		{
			if (Value)
			{
				Value = false;
			}
		}

        /// <summary>
        ///     Set and raise reverse value.
        /// </summary>
        public void SwitchValue()
		{
			Value = !Value;
		}
	}
}
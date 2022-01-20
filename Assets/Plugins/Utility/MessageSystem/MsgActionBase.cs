using System;

namespace RGF
{
	public abstract class MsgActionBase
	{
		public abstract Delegate Get();
	}

	public class MsgAction : MsgActionBase
	{
		private event Action m_action;

		public void Init(Action action)
		{
			m_action = action;
		}

		public void Invoke()
		{
			m_action();
		}

		public bool Equals(Action action)
		{
			return m_action == action;
		}

		public override Delegate Get()
		{
			return m_action;
		}
	}

	public class MsgAction<T1> : MsgActionBase
	{
		private event Action<T1> m_action;

		public void Init(Action<T1> action)
		{
			m_action = action;
		}

		public void Invoke(T1 arg1)
		{
			m_action(arg1);
		}

		public bool Equals(Action<T1> action)
		{
			return m_action == action;
		}

		public override Delegate Get()
		{
			return m_action;
		}
	}
	
	public class MsgAction<T1, T2> : MsgActionBase
	{
		private event Action<T1, T2> m_action;

		public void Init(Action<T1, T2> action)
		{
			m_action = action;
		}

		public void Invoke(T1 arg1, T2 arg2)
		{
			m_action(arg1, arg2);
		}

		public bool Equals(Action<T1, T2> action)
		{
			return m_action == action;
		}

		public override Delegate Get()
		{
			return m_action;
		}
	}
}
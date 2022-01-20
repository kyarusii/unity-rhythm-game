using System;

public abstract class DisposableSingleton<T> : Singleton<T>, IDisposable
	where T : Singleton<T>, new()
{
	public void Dispose()
	{
		_instance = null;
	}
}
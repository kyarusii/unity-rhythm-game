namespace RGF.Game.Model.Exception
{
	public abstract class LoadingException : System.Exception
	{
		protected LoadingException(string msg) : base(msg) { }
	}

	public class TextureNotFoundException : LoadingException
	{
		public TextureNotFoundException(string msg) : base(msg) { }
	}

	public class AudioDownloadFailedException : LoadingException
	{
		public AudioDownloadFailedException(string filePath) : base(filePath) { }
	}
}
namespace RGF.Game
{
	public class Singleton : Singleton<Singleton>
	{
		public NewInput input = new NewInput();
	}
}

namespace Danmaku2Lib
{
	public interface IFrame<T> where T : class, IAssets
	{
		IFrame<T> GetNextFrame(IKeyboard keyboardInput, IMouse mouseInput, IKeyboard previousKeyboardInput, IMouse previousMouseInput, IDisplay<T> display);
		void ProcessMusic();
		void Render(IDisplay<T> display);
		void RenderMusic(IDisplay<T> display);
	}
}

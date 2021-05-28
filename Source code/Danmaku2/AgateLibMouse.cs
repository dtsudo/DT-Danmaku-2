
namespace Danmaku2
{
	using AgateLib.InputLib;
	using Danmaku2Lib;

	public class AgateLibMouse : IMouse
	{
		public AgateLibMouse()
		{
		}

		public int GetX()
		{
			return Mouse.X;
		}

		public int GetY()
		{
			return Mouse.Y;
		}

		public bool IsLeftMouseButtonPressed()
		{
			return Mouse.Buttons[Mouse.MouseButtons.Primary];
		}

		public bool IsRightMouseButtonPressed()
		{
			return Mouse.Buttons[Mouse.MouseButtons.Secondary];
		}
	}
}

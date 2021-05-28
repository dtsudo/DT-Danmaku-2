
namespace Danmaku2Lib
{
	public class CreditsProgrammingDisplay
	{
		public static void Render(
			int x,
			int y,
			bool isWebBrowserVersion,
			IDisplay<Danmaku2Assets> display)
		{
			if (!isWebBrowserVersion)
			{
				display.GetAssets().DrawImage(
					image: Danmaku2Image.CreditsProgrammingDesktopVersion,
					x: x + 5,
					y: y + 5);
			}
		}
	}
}

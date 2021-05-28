
namespace Danmaku2Lib
{
	public class NumLivesDisplay
	{
		public static void Render(IDisplay<Danmaku2Assets> display, int numLivesRemaining)
		{
			var assets = display.GetAssets();

			Danmaku2Image image = Danmaku2Image.PlayerLifeIcon;

			int width = assets.GetWidth(image);

			int x = GameLogic.GAME_WINDOW_WIDTH_IN_PIXELS - 1 - width;

			if (numLivesRemaining > 50)
				numLivesRemaining = 50;

			for (int i = 0; i < numLivesRemaining; i++)
			{
				assets.DrawImage(
					image: image,
					x: x,
					y: 2);

				x -= width;
			}
		}
	}
}

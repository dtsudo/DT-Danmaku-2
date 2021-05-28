
namespace Danmaku2Lib
{
	public class TranslatedDisplay : IDisplay<Danmaku2Assets>
	{
		private class TranslatedDanmaku2Assets : Danmaku2Assets
		{
			private Danmaku2Assets assets;
			private int xOffsetInPixels;
			private int yOffsetInPixels;

			public TranslatedDanmaku2Assets(Danmaku2Assets assets,
				int xOffsetInPixels,
				int yOffsetInPixels)
			{
				this.assets = assets;
				this.xOffsetInPixels = xOffsetInPixels;
				this.yOffsetInPixels = yOffsetInPixels;
			}

			public override void DrawInitialLoadingScreen()
			{
				this.assets.DrawInitialLoadingScreen();
			}

			public override bool LoadImages()
			{
				return this.assets.LoadImages();
			}

			public override void DisposeImages()
			{
				this.assets.DisposeImages();
			}

			public override void DrawImageRotatedCounterclockwise(Danmaku2Image image, int x, int y, int degreesScaled, int scalingFactorScaled)
			{
				this.assets.DrawImageRotatedCounterclockwise(
					image: image,
					x: x + this.xOffsetInPixels,
					y: y + this.yOffsetInPixels,
					degreesScaled: degreesScaled,
					scalingFactorScaled: scalingFactorScaled);
			}

			public override bool LoadSounds()
			{
				return this.assets.LoadSounds();
			}

			public override void PlaySound(Danmaku2Sound sound, int volume)
			{
				this.assets.PlaySound(sound: sound, volume: volume);
			}

			public override void DisposeSounds()
			{
				this.assets.DisposeSounds();
			}

			public override bool LoadMusic()
			{
				return this.assets.LoadMusic();
			}

			public override void PlayMusic(Danmaku2Music music, int volume)
			{
				this.assets.PlayMusic(music: music, volume: volume);
			}

			public override void StopMusic()
			{
				this.assets.StopMusic();
			}

			public override void DisposeMusic()
			{
				this.assets.DisposeMusic();
			}
		}

		private IDisplay<Danmaku2Assets> display;
		private Danmaku2Assets translatedAssets;
		private int xOffsetInPixels;
		private int yOffsetInPixels;

		public TranslatedDisplay(
			IDisplay<Danmaku2Assets> display,
			int xOffsetInPixels,
			int yOffsetInPixels)
		{
			this.display = display;
			this.translatedAssets = new TranslatedDanmaku2Assets(
				assets: display.GetAssets(),
				xOffsetInPixels: xOffsetInPixels,
				yOffsetInPixels: yOffsetInPixels);
			this.xOffsetInPixels = xOffsetInPixels;
			this.yOffsetInPixels = yOffsetInPixels;
		}

		public void DrawRectangle(int x, int y, int width, int height, DTColor color, bool fill)
		{
			this.display.DrawRectangle(
				x: x + this.xOffsetInPixels,
				y: y + this.yOffsetInPixels,
				width: width,
				height: height,
				color: color,
				fill: fill);
		}

		public void DebugPrint(int x, int y, string debugText)
		{
			this.display.DebugPrint(
				x: x + this.xOffsetInPixels,
				y: y + this.yOffsetInPixels,
				debugText: debugText);
		}

		public Danmaku2Assets GetAssets()
		{
			return this.translatedAssets;
		}
	}
}

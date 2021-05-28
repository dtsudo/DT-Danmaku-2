
namespace Danmaku2Lib
{
	using System;

	public class BackgroundRenderer
	{
		private int currentMillipixelOffset;
		private Danmaku2Image[][] tiles;

		private IDTRandom nonGameLogicRng;

		private int millipixelIncrementPerFrame;

		private DTColor darkenBackground;

		public BackgroundRenderer(IDTRandom nonGameLogicRng, int fps)
		{
			this.nonGameLogicRng = nonGameLogicRng;
			this.millipixelIncrementPerFrame = 37 * 1024 / fps;

			this.currentMillipixelOffset = 0;
			this.tiles = new Danmaku2Image[800 / 104 + 1 + 6][];
			for (int i = 0; i < this.tiles.Length; i++)
			{
				this.tiles[i] = new Danmaku2Image[800 / 120 + 1 + 4];
				for (int j = 0; j < this.tiles[i].Length; j++)
				{
					this.tiles[i][j] = this.GetRandomTile();
				}
			}

			this.darkenBackground = new DTColor(0, 0, 0, 25);
		}

		private Danmaku2Image GetRandomTile()
		{
			switch (this.nonGameLogicRng.NextInt(6))
			{
				case 0: return Danmaku2Image.TileGrass10;
				case 1: return Danmaku2Image.TileGrass11;
				case 2: return Danmaku2Image.TileGrass12;
				case 3: return Danmaku2Image.TileDirt06;
				case 4: return Danmaku2Image.TileDirt11;
				case 5: return Danmaku2Image.TileDirt15;
				default: throw new Exception();
			}
		}

		public void ProcessFrame()
		{
			this.currentMillipixelOffset = this.currentMillipixelOffset + this.millipixelIncrementPerFrame;
			while (this.currentMillipixelOffset >= 104 * 2 * 1024)
			{
				Danmaku2Image[] oldArray1 = this.tiles[this.tiles.Length - 1];
				Danmaku2Image[] oldArray2 = this.tiles[this.tiles.Length - 2];

				this.currentMillipixelOffset = this.currentMillipixelOffset - 104 * 2 * 1024;
				for (int i = this.tiles.Length - 1; i >= 2; i--)
				{
					this.tiles[i] = this.tiles[i - 2];
				}
				this.tiles[0] = oldArray1;
				this.tiles[1] = oldArray2;
				for (int j = 0; j < this.tiles[0].Length; j++)
				{
					this.tiles[0][j] = this.GetRandomTile();
					this.tiles[1][j] = this.GetRandomTile();
				}
			}
		}

		public void Render(IDisplay<Danmaku2Assets> display)
		{
			Danmaku2Assets assets = display.GetAssets();
			int yOffset = (this.currentMillipixelOffset >> 10) - 270;

			for (int i = 0; i < this.tiles.Length; i++)
			{
				int xOffset = -61;
				if (i % 2 == 0)
					xOffset += 60;
				for (int j = 0; j < this.tiles[i].Length; j++)
				{
					assets.DrawImage(this.tiles[i][j], xOffset, yOffset);

					xOffset += 120;
				}

				yOffset += 104;
			}

			display.DrawRectangle(0, 0, 800, 800, this.darkenBackground, true);
		}
	}
}


namespace Danmaku2Lib
{
	using System;
	using System.Collections.Generic;

	public class StandardExplosionEnemy : IEnemy
	{
		private const int RATE_OF_ANIMATION = 60;

		private int xMillis;
		private int yMillis;
		private int x;
		private int yInverted;
		private int elapsedMillis;

		private DTImmutableList<IEnemy> enemyList;
		private EnemyFrameResult enemyFrameResult;

		private int degreesScaled;

		private int scalingFactorScaled;

		public StandardExplosionEnemy(
			int xMillis,
			int yMillis,
			IDTDeterministicRandom rng,
			int scalingFactorScaled)
		{
			this.xMillis = xMillis;
			this.yMillis = yMillis;
			this.elapsedMillis = 0;
			this.x = xMillis >> 10;
			this.yInverted = GameLogic.GAME_WINDOW_HEIGHT_IN_PIXELS - (yMillis >> 10);

			this.enemyList = DTImmutableList<IEnemy>.AsImmutableList(new List<IEnemy>() { this });
			this.enemyFrameResult = new EnemyFrameResult(
				enemies: this.enemyList,
				sounds: null,
				bossHealthMeterNumber: null,
				bossHealthMeterMilliPercentage: null,
				shouldEndLevel: false);

			this.degreesScaled = rng.NextInt(360 * 128);

			this.scalingFactorScaled = scalingFactorScaled;
		}

		public List<ObjectBox> GetCollisionBoxes()
		{
			return null;
		}

		public List<ObjectBox> GetDamageBoxes()
		{
			return null;
		}

		public int GetXMillis()
		{
			return this.xMillis;
		}

		public int GetYMillis()
		{
			return this.yMillis;
		}

		public void HandleCollisionWithPlayer()
		{
		}

		public void HandleCollisionWithPlayerBullet(PlayerBulletStrength bulletStrength)
		{
		}

		public EnemyFrameResult ProcessFrame(
			int elapsedMillisPerFrame,
			int playerXMillis,
			int playerYMillis)
		{
			if (this.elapsedMillis == 0)
			{
				this.elapsedMillis += elapsedMillisPerFrame;
				return new EnemyFrameResult(
					enemies: this.enemyList,
					sounds: DTImmutableList<Danmaku2Sound>.AsImmutableList(
						new List<Danmaku2Sound>() { Danmaku2Sound.StandardDeath }),
					bossHealthMeterNumber: null,
					bossHealthMeterMilliPercentage: null,
					shouldEndLevel: false);
			}

			this.elapsedMillis += elapsedMillisPerFrame;

			if (this.elapsedMillis >= RATE_OF_ANIMATION * 9)
			{
				return null;
			}

			return this.enemyFrameResult;
		}

		public ZIndex GetZIndex()
		{
			return ZIndex.DeathAnimation;
		}

		public void Render(IDisplay<Danmaku2Assets> display)
		{
			var assets = display.GetAssets();

			Danmaku2Image image;

			if (this.elapsedMillis < RATE_OF_ANIMATION)
				image = Danmaku2Image.Explosion1;
			else if (this.elapsedMillis < RATE_OF_ANIMATION * 2)
				image = Danmaku2Image.Explosion2;
			else if (this.elapsedMillis < RATE_OF_ANIMATION * 3)
				image = Danmaku2Image.Explosion3;
			else if (this.elapsedMillis < RATE_OF_ANIMATION * 4)
				image = Danmaku2Image.Explosion4;
			else if (this.elapsedMillis < RATE_OF_ANIMATION * 5)
				image = Danmaku2Image.Explosion5;
			else if (this.elapsedMillis < RATE_OF_ANIMATION * 6)
				image = Danmaku2Image.Explosion6;
			else if (this.elapsedMillis < RATE_OF_ANIMATION * 7)
				image = Danmaku2Image.Explosion7;
			else if (this.elapsedMillis < RATE_OF_ANIMATION * 8)
				image = Danmaku2Image.Explosion8;
			else
				image = Danmaku2Image.Explosion9;

			if (this.scalingFactorScaled == 128)
				assets.DrawImageRotatedCounterclockwise(
					image: image,
					x: this.x - (assets.GetWidth(image) >> 1),
					y: this.yInverted - (assets.GetHeight(image) >> 1),
					degreesScaled: this.degreesScaled);
			else
				assets.DrawImageRotatedCounterclockwise(
					image: image,
					x: this.x - ((assets.GetWidth(image) * this.scalingFactorScaled) >> 8),
					y: this.yInverted - ((assets.GetHeight(image) * this.scalingFactorScaled) >> 8),
					degreesScaled: this.degreesScaled,
					scalingFactorScaled: this.scalingFactorScaled);

		}
	}
}

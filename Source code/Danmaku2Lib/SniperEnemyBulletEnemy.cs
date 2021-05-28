
namespace Danmaku2Lib
{
	using System;
	using System.Collections.Generic;

	public class SniperEnemyBulletEnemy : IEnemy
	{
		private int xMillis;
		private int yMillis;
		private int xDelta;
		private int yDelta;

		private List<ObjectBox> collisionBoxes;
		
		private EnemyFrameResult enemyFrameResult;

		private bool hasCollided;

		private int displayRotationDegreesScaled;
		private int rotationUpdatePerFrame;

		private Danmaku2Image image;

		public SniperEnemyBulletEnemy(
			int xMillis,
			int yMillis,
			int degreesScaled,
			int elapsedMillisPerFrame,
			Difficulty difficulty,
			Danmaku2Image image,
			List<ObjectBox> collisionBoxes,
			IDTDeterministicRandom rng)
		{
			this.xMillis = xMillis;
			this.yMillis = yMillis;

			int speedInMillipixelsPerMillisecond;
			switch (difficulty)
			{
				case Difficulty.Easy:
					speedInMillipixelsPerMillisecond = 100;
					break;
				case Difficulty.Normal:
					speedInMillipixelsPerMillisecond = 120;
					break;
				case Difficulty.Hard:
					speedInMillipixelsPerMillisecond = 200;
					break;
				default:
					throw new Exception();
			}

			var offset = DanmakuMath.GetOffset(
				speedInMillipixelsPerMillisecond: speedInMillipixelsPerMillisecond,
				movementDirectionInDegreesScaled: degreesScaled,
				elapsedMillisecondsPerIteration: elapsedMillisPerFrame);

			this.xDelta = offset.DeltaXInMillipixels;
			this.yDelta = offset.DeltaYInMillipixels;

			this.collisionBoxes = collisionBoxes;
			
			this.enemyFrameResult = new EnemyFrameResult(
				enemies: DTImmutableList<IEnemy>.AsImmutableList(
					new List<IEnemy>() { this }),
				sounds: null,
				bossHealthMeterNumber: null,
				bossHealthMeterMilliPercentage: null,
				shouldEndLevel: false);

			this.hasCollided = false;

			this.image = image;

			this.displayRotationDegreesScaled = rng.NextInt(360 * 128);
			this.rotationUpdatePerFrame = 10 * elapsedMillisPerFrame;
		}

		public List<ObjectBox> GetCollisionBoxes()
		{
			return this.collisionBoxes;
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

		public ZIndex GetZIndex()
		{
			return ZIndex.EnemyBullet;
		}

		public void HandleCollisionWithPlayer()
		{
			this.hasCollided = true;
		}

		public void HandleCollisionWithPlayerBullet(PlayerBulletStrength bulletStrength)
		{
		}

		public EnemyFrameResult ProcessFrame(int elapsedMillisPerFrame, int playerXMillis, int playerYMillis)
		{
			if (this.hasCollided)
				return null;

			this.xMillis = this.xMillis + this.xDelta;
			this.yMillis = this.yMillis + this.yDelta;

			if (this.xMillis < -30 * 1024)
				return null;
			if (this.xMillis > (GameLogic.GAME_WINDOW_WIDTH_IN_PIXELS + 30) * 1024)
				return null;
			if (this.yMillis < -30 * 1024)
				return null;
			if (this.yMillis > (GameLogic.GAME_WINDOW_HEIGHT_IN_PIXELS + 30) * 1024)
				return null;

			this.displayRotationDegreesScaled = this.displayRotationDegreesScaled + this.rotationUpdatePerFrame;
			while (this.displayRotationDegreesScaled > 360 * 128)
				this.displayRotationDegreesScaled = this.displayRotationDegreesScaled - 360 * 128;

			return this.enemyFrameResult;
		}

		public void Render(IDisplay<Danmaku2Assets> display)
		{
			var assets = display.GetAssets();

			assets.DrawImageRotatedCounterclockwise(
				image: this.image,
				x: (this.xMillis >> 10) - (assets.GetWidth(this.image) >> 1),
				y: GameLogic.GAME_WINDOW_HEIGHT_IN_PIXELS - (this.yMillis >> 10) - (assets.GetHeight(this.image) >> 1),
				degreesScaled: this.displayRotationDegreesScaled);
		}
	}
}

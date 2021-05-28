
namespace Danmaku2Lib
{
	using System;
	using System.Collections.Generic;

	public class BasicEnemyBulletEnemy : IEnemy
	{
		private int xMillis;
		private int yMillis;
		private int xDelta;
		private int yDelta;

		private List<ObjectBox> collisionBoxes;
		
		private EnemyFrameResult enemyFrameResult;

		private bool hasCollided;

		private int degreesScaled;

		public BasicEnemyBulletEnemy(
			int xMillis,
			int yMillis,
			int degreesScaled,
			int elapsedMillisPerFrame,
			Difficulty difficulty,
			List<ObjectBox> collisionBoxes)
		{
			this.xMillis = xMillis;
			this.yMillis = yMillis;

			int speedInMillipixelsPerMillisecond;
			switch (difficulty)
			{
				case Difficulty.Easy:
					speedInMillipixelsPerMillisecond = 200;
					break;
				case Difficulty.Normal:
					speedInMillipixelsPerMillisecond = 220;
					break;
				case Difficulty.Hard:
					speedInMillipixelsPerMillisecond = 250;
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

			this.degreesScaled = degreesScaled;
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
			
			return this.enemyFrameResult;
		}

		public void Render(IDisplay<Danmaku2Assets> display)
		{
			var assets = display.GetAssets();

			var image = Danmaku2Image.BasicEnemyBullet;

			assets.DrawImageRotatedCounterclockwise(
				image: image,
				x: (this.xMillis >> 10) - (assets.GetWidth(image) >> 1),
				y: GameLogic.GAME_WINDOW_HEIGHT_IN_PIXELS - (this.yMillis >> 10) - (assets.GetHeight(image) >> 1),
				degreesScaled: -this.degreesScaled);
		}
	}
}

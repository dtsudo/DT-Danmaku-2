
namespace Danmaku2Lib
{
	using System.Collections.Generic;

	public class SimpleEnemyBulletWithVelocityOffsetEnemy : IEnemy
	{
		private int xMillis;
		private int yMillis;
		private int xDelta;
		private int yDelta;

		private List<ObjectBox> collisionBoxes;
		
		private EnemyFrameResult enemyFrameResult;

		private bool hasCollided;

		private int degreesScaled;

		private Danmaku2Image danmaku2Image;

		private int numPixelsOffscreenBeforeDespawn;

		public SimpleEnemyBulletWithVelocityOffsetEnemy(
			int xMillis,
			int yMillis,
			int degreesScaled,
			int speedInMillipixelsPerMillisecond,
			int xVelocityOffsetPerFrameMillis,
			int yVelocityOffsetPerFrameMillis,
			int elapsedMillisPerFrame,
			List<ObjectBox> collisionBoxes,
			Danmaku2Image danmaku2Image,
			int numPixelsOffscreenBeforeDespawn)
		{
			this.xMillis = xMillis;
			this.yMillis = yMillis;
			
			var offset = DanmakuMath.GetOffset(
				speedInMillipixelsPerMillisecond: speedInMillipixelsPerMillisecond,
				movementDirectionInDegreesScaled: degreesScaled,
				elapsedMillisecondsPerIteration: elapsedMillisPerFrame);

			this.xDelta = offset.DeltaXInMillipixels + xVelocityOffsetPerFrameMillis;
			this.yDelta = offset.DeltaYInMillipixels + yVelocityOffsetPerFrameMillis;

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

			this.danmaku2Image = danmaku2Image;

			this.numPixelsOffscreenBeforeDespawn = numPixelsOffscreenBeforeDespawn;
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

			if (this.xMillis < -(this.numPixelsOffscreenBeforeDespawn << 10))
				return null;
			if (this.xMillis > (GameLogic.GAME_WINDOW_WIDTH_IN_PIXELS + this.numPixelsOffscreenBeforeDespawn) << 10)
				return null;
			if (this.yMillis < -(this.numPixelsOffscreenBeforeDespawn << 10))
				return null;
			if (this.yMillis > (GameLogic.GAME_WINDOW_HEIGHT_IN_PIXELS + this.numPixelsOffscreenBeforeDespawn) << 10)
				return null;
			
			return this.enemyFrameResult;
		}

		public void Render(IDisplay<Danmaku2Assets> display)
		{
			var assets = display.GetAssets();

			var image = this.danmaku2Image;

			assets.DrawImageRotatedCounterclockwise(
				image: image,
				x: (this.xMillis >> 10) - (assets.GetWidth(image) >> 1),
				y: GameLogic.GAME_WINDOW_HEIGHT_IN_PIXELS - (this.yMillis >> 10) - (assets.GetHeight(image) >> 1),
				degreesScaled: -this.degreesScaled);
		}
	}
}

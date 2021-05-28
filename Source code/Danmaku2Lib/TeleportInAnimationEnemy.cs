
namespace Danmaku2Lib
{
	using System.Collections.Generic;

	public class TeleportInAnimationEnemy : IEnemy
	{
		private EnemyFrameResult enemyFrameResult;

		private IEnemy enemy;

		private int radiusMillis;

		private int xMillis;
		private int yMillis;

		public TeleportInAnimationEnemy(IEnemy enemy)
		{
			this.enemy = enemy;
			this.enemyFrameResult = new EnemyFrameResult(
				enemies: DTImmutableList<IEnemy>.AsImmutableList(new List<IEnemy>() { this }),
				sounds: null,
				bossHealthMeterNumber: null,
				bossHealthMeterMilliPercentage: null,
				shouldEndLevel: false);
			this.radiusMillis = 80 * 1024;
			this.xMillis = enemy.GetXMillis();
			this.yMillis = enemy.GetYMillis();
		}

		public int GetXMillis()
		{
			return this.xMillis;
		}

		public int GetYMillis()
		{
			return this.yMillis;
		}

		public List<ObjectBox> GetCollisionBoxes()
		{
			return null;
		}

		public List<ObjectBox> GetDamageBoxes()
		{
			return null;
		}

		public EnemyFrameResult ProcessFrame(
			int elapsedMillisPerFrame,
			int playerXMillis,
			int playerYMillis)
		{
			this.radiusMillis -= elapsedMillisPerFrame * 400;

			if (this.radiusMillis < 0)
				return null;

			this.xMillis = this.enemy.GetXMillis();
			this.yMillis = this.enemy.GetYMillis();

			return this.enemyFrameResult;
		}

		public void HandleCollisionWithPlayer()
		{
		}

		public void HandleCollisionWithPlayerBullet(PlayerBulletStrength bulletStrength)
		{
		}

		public ZIndex GetZIndex()
		{
			return ZIndex.EnemyBullet;
		}

		public void Render(IDisplay<Danmaku2Assets> display)
		{
			var assets = display.GetAssets();

			Danmaku2Image image = Danmaku2Image.Line;

			int halfWidth = assets.GetWidth(image) >> 1;
			int halfHeight = assets.GetHeight(image) >> 1;

			for (int i = 0; i < 360 * 128; i += 10 * 128)
			{
				int xMillis = this.xMillis + ((this.radiusMillis * DTMath.CosineScaled(degreesScaled: i)) >> 10);
				int yMillis = this.yMillis + ((this.radiusMillis * DTMath.SineScaled(degreesScaled: i)) >> 10);

				assets.DrawImageRotatedCounterclockwise(
					image: image,
					x: (xMillis >> 10) - halfWidth,
					y: GameLogic.GAME_WINDOW_HEIGHT_IN_PIXELS - (yMillis >> 10) - halfHeight,
					degreesScaled: i);
			}
		}
	}
}

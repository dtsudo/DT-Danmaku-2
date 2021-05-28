
namespace Danmaku2Lib
{
	using System;
	using System.Collections.Generic;

	public class MediumDescendingEnemyCreator
	{
		private List<ObjectBox> collisionBoxes;
		private List<ObjectBox> damageBoxes;

		private int elapsedMillisPerFrame;

		private IDTDeterministicRandom rng;

		public MediumDescendingEnemyCreator(
			IDTDeterministicRandom rng,
			int elapsedMillisPerFrame)
		{
			this.collisionBoxes = MediumEnemyCreator.GetCollisionBoxes();

			this.damageBoxes = MediumEnemyCreator.GetDamageBoxes();

			this.elapsedMillisPerFrame = elapsedMillisPerFrame;
			this.rng = rng;
		}

		public MediumDescendingEnemy CreateMediumDescendingEnemy(
			int xMillis,
			bool bulletsRotateClockwise,
			int shootDirectionScaled1,
			int shootDirectionScaled2,
			Difficulty difficulty)
		{
			return new MediumDescendingEnemy(
				xMillis: xMillis,
				collisionBoxes: this.collisionBoxes,
				damageBoxes: this.damageBoxes,
				bulletsRotateClockwise: bulletsRotateClockwise,
				shootDirectionScaled1: shootDirectionScaled1,
				shootDirectionScaled2: shootDirectionScaled2,
				difficulty: difficulty,
				elapsedMillisPerFrame: this.elapsedMillisPerFrame,
				rng: this.rng);
		}

		public class MediumDescendingEnemy : IEnemy
		{
			private List<ObjectBox> collisionBoxes;
			private List<ObjectBox> damageBoxes;
			private Difficulty difficulty;
			
			private EnemyFrameResult enemyFrameResult;

			private int xMillis;
			private int yMillis;
			private int milliHp;

			private int deltaYPerFrame;

			private IDTDeterministicRandom rng;
			
			private MediumEnemyShootPattern mediumEnemyShootPattern;
			
			private bool isDestroyedOrDespawned;

			public MediumDescendingEnemy(
				int xMillis,
				List<ObjectBox> collisionBoxes,
				List<ObjectBox> damageBoxes,
				bool bulletsRotateClockwise,
				int shootDirectionScaled1,
				int shootDirectionScaled2,
				Difficulty difficulty,
				int elapsedMillisPerFrame,
				IDTDeterministicRandom rng)
			{
				this.collisionBoxes = collisionBoxes;
				this.damageBoxes = damageBoxes;
				this.difficulty = difficulty;
				this.enemyFrameResult = new EnemyFrameResult(
					enemies: DTImmutableList<IEnemy>.AsImmutableList(new List<IEnemy>() { this }),
					sounds: null,
					bossHealthMeterNumber: null,
					bossHealthMeterMilliPercentage: null,
					shouldEndLevel: false);

				this.mediumEnemyShootPattern = new MediumEnemyShootPattern(
					enemy: this,
					bulletsRotateClockwise: bulletsRotateClockwise,
					shootDirectionScaled1: shootDirectionScaled1,
					shootDirectionScaled2: shootDirectionScaled2,
					xVelocityOffsetPerFrameMillis: 0,
					yVelocityOffsetPerFrameMillis: -90 * elapsedMillisPerFrame,
					difficulty: difficulty,
					rng: rng);

				this.xMillis = xMillis;
				this.yMillis = (GameLogic.GAME_WINDOW_HEIGHT_IN_PIXELS + 100) * 1024;
				
				this.deltaYPerFrame = -90 * elapsedMillisPerFrame;

				this.rng = rng;
				
				switch (difficulty)
				{
					case Difficulty.Easy:
						this.milliHp = 4800;
						break;
					case Difficulty.Normal:
						this.milliHp = 4800;
						break;
					case Difficulty.Hard:
						this.milliHp = 4800;
						break;
					default:
						throw new Exception();
				}
				
				this.isDestroyedOrDespawned = false;
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
				return this.collisionBoxes;
			}

			public List<ObjectBox> GetDamageBoxes()
			{
				return this.damageBoxes;
			}

			public bool IsDestroyedOrDespawned()
			{
				return this.isDestroyedOrDespawned;
			}

			public EnemyFrameResult ProcessFrame(
				int elapsedMillisPerFrame,
				int playerXMillis,
				int playerYMillis)
			{
				if (this.milliHp <= 0)
				{
					this.isDestroyedOrDespawned = true;

					return new EnemyFrameResult(
						enemies: DTImmutableList<IEnemy>.AsImmutableList(
							new List<IEnemy>() { new StandardExplosionEnemy(
								xMillis: this.xMillis,
								yMillis: this.yMillis,
								rng: this.rng,
								scalingFactorScaled: 280) }),
						sounds: null,
						bossHealthMeterNumber: null,
						bossHealthMeterMilliPercentage: null,
						shouldEndLevel: false);
				}
				
				this.yMillis += this.deltaYPerFrame;

				if (this.yMillis < -100 * 1024)
				{
					this.isDestroyedOrDespawned = true;
					return null;
				}
				
				MediumEnemyShootPattern.Result shootPatternResult = this.mediumEnemyShootPattern.ProcessFrame(elapsedMillisPerFrame: elapsedMillisPerFrame);

				if (shootPatternResult != null)
				{
					List<IEnemy> enemies = new List<IEnemy>();
					foreach (IEnemy enemy in shootPatternResult.Enemies)
						enemies.Add(enemy);
					enemies.Add(this);

					return new EnemyFrameResult(
						DTImmutableList<IEnemy>.AsImmutableList(enemies),
						sounds: DTImmutableList<Danmaku2Sound>.AsImmutableList(shootPatternResult.Sounds), 
						bossHealthMeterNumber: null,
						bossHealthMeterMilliPercentage: null,
						shouldEndLevel: false);
				}
				
				return this.enemyFrameResult;
			}

			public void HandleCollisionWithPlayer()
			{
			}

			public void HandleCollisionWithPlayerBullet(PlayerBulletStrength bulletStrength)
			{
				this.milliHp -= PlayerBullet.GetBulletDamage(playerBulletStrength: bulletStrength);
				
				if (this.milliHp < 0)
					this.milliHp = 0;
			}

			public ZIndex GetZIndex()
			{
				return ZIndex.Enemy;
			}

			public void Render(IDisplay<Danmaku2Assets> display)
			{
				var assets = display.GetAssets();

				Danmaku2Image image = Danmaku2Image.MediumEnemyShip;

				var halfWidth = assets.GetWidth(image) >> 1;
				var halfHeight = assets.GetHeight(image) >> 1;

				assets.DrawImageRotatedCounterclockwise(
					image: image,
					x: (this.xMillis >> 10) - halfWidth,
					y: GameLogic.GAME_WINDOW_HEIGHT_IN_PIXELS - (this.yMillis >> 10) - halfHeight,
					degreesScaled: 180 * 128);
			}
		}
	}
}

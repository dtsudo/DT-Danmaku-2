
namespace Danmaku2Lib
{
	using System;
	using System.Collections.Generic;

	public class MediumEnemyCreator
	{
		private List<ObjectBox> collisionBoxes;
		private List<ObjectBox> damageBoxes;

		private IDTDeterministicRandom rng;

		public static List<ObjectBox> GetCollisionBoxes()
		{
			return new List<ObjectBox>()
			{
				new ObjectBox(-20 * 1024, 20 * 1024, -60 * 1024, 65 * 1024),
				new ObjectBox(-40 * 1024, 40 * 1024, -35 * 1024, 50 * 1024),
				new ObjectBox(-60 * 1024, 60 * 1024, -15 * 1024, 30 * 1024)
			};
		}

		public static List<ObjectBox> GetDamageBoxes()
		{
			return new List<ObjectBox>()
			{
				new ObjectBox(-20 * 1024, 20 * 1024, -70 * 1024, 75 * 1024),
				new ObjectBox(-40 * 1024, 40 * 1024, -50 * 1024, 65 * 1024),
				new ObjectBox(-71 * 1024, 71 * 1024, -35 * 1024, 50 * 1024)
			};
		}

		public MediumEnemyCreator(IDTDeterministicRandom rng)
		{
			this.collisionBoxes = GetCollisionBoxes();

			this.damageBoxes = GetDamageBoxes();

			this.rng = rng;
		}

		public MediumEnemy CreateMediumEnemy(
			int xMillis,
			bool bulletsRotateClockwise,
			int shootDirectionScaled1,
			int shootDirectionScaled2,
			Difficulty difficulty)
		{
			return new MediumEnemy(
				xMillis: xMillis,
				collisionBoxes: this.collisionBoxes,
				damageBoxes: this.damageBoxes,
				bulletsRotateClockwise: bulletsRotateClockwise,
				shootDirectionScaled1: shootDirectionScaled1,
				shootDirectionScaled2: shootDirectionScaled2,
				difficulty: difficulty,
				rng: this.rng);
		}

		public class MediumEnemy : IEnemy
		{
			private List<ObjectBox> collisionBoxes;
			private List<ObjectBox> damageBoxes;
			private Difficulty difficulty;
			
			private EnemyFrameResult enemyFrameResult;

			private int xMillis;
			private int yMillis;
			private int milliHp;
			
			private IDTDeterministicRandom rng;

			private StationaryEnemyMovement stationaryEnemyMovement;
			private MediumEnemyShootPattern mediumEnemyShootPattern;
			
			private bool isDestroyedOrDespawned;

			public MediumEnemy(
				int xMillis,
				List<ObjectBox> collisionBoxes,
				List<ObjectBox> damageBoxes,
				bool bulletsRotateClockwise,
				int shootDirectionScaled1,
				int shootDirectionScaled2,
				Difficulty difficulty,
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
					yVelocityOffsetPerFrameMillis: 0,
					difficulty: difficulty,
					rng: rng);

				this.xMillis = xMillis;
				this.yMillis = (GameLogic.GAME_WINDOW_HEIGHT_IN_PIXELS + 100) * 1024;
				
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

				this.stationaryEnemyMovement = new StationaryEnemyMovement(
					xMillis: this.xMillis,
					initialYMillis: this.yMillis,
					destinationYMillis: 400 * 1024,
					stoppedDurationInMillis: 10 * 1000);

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

				this.stationaryEnemyMovement.ProcessNextFrame(elapsedMillisPerFrame: elapsedMillisPerFrame);
				this.xMillis = this.stationaryEnemyMovement.GetXMillis();
				this.yMillis = this.stationaryEnemyMovement.GetYMillis();

				if (this.stationaryEnemyMovement.HasLeftScreen())
				{
					this.isDestroyedOrDespawned = true;
					return null;
				}
								
				if (this.stationaryEnemyMovement.HasStopped())
				{
					MediumEnemyShootPattern.Result shootPatternResult = this.mediumEnemyShootPattern.ProcessFrame(elapsedMillisPerFrame: elapsedMillisPerFrame);

					if (shootPatternResult != null)
					{
						List<IEnemy> enemies = new List<IEnemy>();

						if (shootPatternResult.Enemies != null)
						{
							foreach (IEnemy enemy in shootPatternResult.Enemies)
								enemies.Add(enemy);
						}

						enemies.Add(this);

						DTImmutableList<Danmaku2Sound> sounds;

						if (shootPatternResult.Sounds != null)
							sounds = DTImmutableList<Danmaku2Sound>.AsImmutableList(shootPatternResult.Sounds);
						else
							sounds = null;

						return new EnemyFrameResult(
							DTImmutableList<IEnemy>.AsImmutableList(enemies),
							sounds: sounds, 
							bossHealthMeterNumber: null,
							bossHealthMeterMilliPercentage: null,
							shouldEndLevel: false);
					}
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

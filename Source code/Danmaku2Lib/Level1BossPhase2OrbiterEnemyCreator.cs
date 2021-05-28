
namespace Danmaku2Lib
{
	using System;
	using System.Collections.Generic;

	public class Level1BossPhase2OrbiterEnemyCreator
	{
		private List<ObjectBox> collisionBoxes;
		private List<ObjectBox> damageBoxes;
		private List<ObjectBox> bulletCollisionBoxes;

		private int elapsedMillisPerFrame;
		private IDTDeterministicRandom rng;

		public Level1BossPhase2OrbiterEnemyCreator(
			IDTDeterministicRandom rng,
			int elapsedMillisPerFrame)
		{
			this.collisionBoxes = new List<ObjectBox>()
			{
				new ObjectBox(-8000, 8000, -12000, -4000),
				new ObjectBox(-16000, 16000, -4000, 12000)
			};

			this.damageBoxes = new List<ObjectBox>()
			{
				new ObjectBox(-26 * 1024, 26 * 1024, 10 * 1024, 21 * 1024),
				new ObjectBox(-17 * 1024, 17 * 1024, -5 * 1024, 21 * 1024),
				new ObjectBox(-13 * 1024, 13 * 1024, -21 * 1024, 21 * 1024)
			};

			this.bulletCollisionBoxes = new List<ObjectBox>()
			{
				new ObjectBox(-3 * 1024, 3 * 1024, -3 * 1024, 3 * 1024)
			};

			this.elapsedMillisPerFrame = elapsedMillisPerFrame;
			this.rng = rng;
		}

		public IEnemy CreateBossPhase2OrbiterEnemy(
			Level1BossPhase2Enemy bossEnemy,
			int initialDegreesScaled,
			bool shouldRotateClockwise,
			int orbiterEnemyNumber,
			Difficulty difficulty)
		{
			return new BossPhase2OrbiterEnemy(
				bossEnemy: bossEnemy,
				initialDegreesScaled: initialDegreesScaled,
				shouldRotateClockwise: shouldRotateClockwise,
				orbiterEnemyNumber: orbiterEnemyNumber,
				collisionBoxes: this.collisionBoxes,
				damageBoxes: this.damageBoxes,
				bulletCollisionBoxes: this.bulletCollisionBoxes,
				difficulty: difficulty,
				elapsedMillisPerFrame: this.elapsedMillisPerFrame,
				rng: this.rng);
		}

		private class BossPhase2OrbiterEnemy : IEnemy
		{
			private List<ObjectBox> collisionBoxes;
			private List<ObjectBox> damageBoxes;
			private List<ObjectBox> bulletCollisionBoxes;
			private Difficulty difficulty;

			private EnemyFrameResult enemyFrameResult;

			private int xMillis;
			private int yMillis;
			private int milliHp;

			private IDTDeterministicRandom rng;

			private Level1BossPhase2Enemy bossEnemy;

			private int degreesScaled;
			private bool shouldRotateClockwise;

			private int currentShootCooldownMillis;
			private int shootCooldownMillis;

			private int deltaDegreesScaled;

			private Danmaku2Image orbiterEnemyBulletImage;

			private int elapsedMillisPerFrame;

			private int deltaBulletDegreesScaled;

			private bool hasSpawnedTeleportInAnimation;

			private static int ComputeXMillis(IEnemy bossEnemy, int locationInDegreesScaled)
			{
				return bossEnemy.GetXMillis() + 140 * DTMath.SineScaled(locationInDegreesScaled);
			}

			private static int ComputeYMillis(IEnemy bossEnemy, int locationInDegreesScaled)
			{
				return bossEnemy.GetYMillis() + 140 * DTMath.CosineScaled(locationInDegreesScaled);
			}

			public BossPhase2OrbiterEnemy(
				Level1BossPhase2Enemy bossEnemy,
				int initialDegreesScaled,
				bool shouldRotateClockwise,
				int orbiterEnemyNumber,
				List<ObjectBox> collisionBoxes,
				List<ObjectBox> damageBoxes,
				List<ObjectBox> bulletCollisionBoxes,
				Difficulty difficulty,
				int elapsedMillisPerFrame,
				IDTDeterministicRandom rng)
			{
				initialDegreesScaled = DanmakuMath.NormalizeAngleInDegreesScaled(angleInDegreesScaled: initialDegreesScaled);

				this.collisionBoxes = collisionBoxes;
				this.damageBoxes = damageBoxes;
				this.bulletCollisionBoxes = bulletCollisionBoxes;
				this.difficulty = difficulty;
				this.enemyFrameResult = new EnemyFrameResult(
					enemies: DTImmutableList<IEnemy>.AsImmutableList(new List<IEnemy>() { this }),
					sounds: null,
					bossHealthMeterNumber: null,
					bossHealthMeterMilliPercentage: null,
					shouldEndLevel: false);

				this.xMillis = ComputeXMillis(bossEnemy: bossEnemy, locationInDegreesScaled: initialDegreesScaled);
				this.yMillis = ComputeYMillis(bossEnemy: bossEnemy, locationInDegreesScaled: initialDegreesScaled);

				this.bossEnemy = bossEnemy;
				this.degreesScaled = initialDegreesScaled;
				this.shouldRotateClockwise = shouldRotateClockwise;

				this.deltaDegreesScaled = 2 * elapsedMillisPerFrame;

				this.rng = rng;
				
				switch (difficulty)
				{
					case Difficulty.Easy:
						this.shootCooldownMillis = 12500;
						break;
					case Difficulty.Normal:
						this.shootCooldownMillis = 9500;
						break;
					case Difficulty.Hard:
						this.shootCooldownMillis = 1000;
						break;
					default:
						throw new Exception();
				}

				this.currentShootCooldownMillis = this.shootCooldownMillis * orbiterEnemyNumber / 5;

				this.milliHp = 5600;

				switch (orbiterEnemyNumber)
				{
					case 1:
						this.orbiterEnemyBulletImage = Danmaku2Image.Level1BossPhase2OrbiterEnemyBullet1;
						break;
					case 2:
						this.orbiterEnemyBulletImage = Danmaku2Image.Level1BossPhase2OrbiterEnemyBullet2;
						break;
					case 3:
						this.orbiterEnemyBulletImage = Danmaku2Image.Level1BossPhase2OrbiterEnemyBullet3;
						break;
					case 4:
						this.orbiterEnemyBulletImage = Danmaku2Image.Level1BossPhase2OrbiterEnemyBullet4;
						break;
					case 5:
						this.orbiterEnemyBulletImage = Danmaku2Image.Level1BossPhase2OrbiterEnemyBullet5;
						break;
					default:
						throw new Exception();
				}

				this.elapsedMillisPerFrame = elapsedMillisPerFrame;

				switch (difficulty)
				{
					case Difficulty.Easy:
						this.deltaBulletDegreesScaled = 360 * 128 / 6;
						break;
					case Difficulty.Normal:
						this.deltaBulletDegreesScaled = 360 * 128 / 9;
						break;
					case Difficulty.Hard:
						this.deltaBulletDegreesScaled = 360 * 128 / 12;
						break;
					default:
						throw new Exception();
				}

				this.hasSpawnedTeleportInAnimation = false;
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

			public EnemyFrameResult ProcessFrame(
				int elapsedMillisPerFrame,
				int playerXMillis,
				int playerYMillis)
			{
				if (this.bossEnemy.IsDead())
					this.milliHp = 0;

				if (this.bossEnemy.HasDespawned())
					return null;

				if (this.milliHp <= 0)
				{
					return new EnemyFrameResult(
						enemies: DTImmutableList<IEnemy>.AsImmutableList(
							new List<IEnemy>() { new StandardExplosionEnemy(
								xMillis: this.xMillis,
								yMillis: this.yMillis,
								rng: this.rng,
								scalingFactorScaled: 128) }),
						sounds: null,
						bossHealthMeterNumber: null,
						bossHealthMeterMilliPercentage: null,
						shouldEndLevel: false);
				}

				List<IEnemy> enemies = null;

				if (!this.hasSpawnedTeleportInAnimation)
				{
					this.hasSpawnedTeleportInAnimation = true;

					if (enemies == null)
						enemies = new List<IEnemy>();

					enemies.Add(new TeleportInAnimationEnemy(enemy: this));
				}

				if (this.shouldRotateClockwise)
				{
					this.degreesScaled += this.deltaDegreesScaled;
					while (this.degreesScaled >= 360 * 128)
						this.degreesScaled -= 360 * 128;
				}
				else
				{
					this.degreesScaled -= this.deltaDegreesScaled;
					while (this.degreesScaled < 0)
						this.degreesScaled += 360 * 128;
				}

				this.xMillis = ComputeXMillis(bossEnemy: this.bossEnemy, locationInDegreesScaled: this.degreesScaled);
				this.yMillis = ComputeYMillis(bossEnemy: this.bossEnemy, locationInDegreesScaled: this.degreesScaled);

				this.currentShootCooldownMillis = this.currentShootCooldownMillis - elapsedMillisPerFrame;
				if (this.currentShootCooldownMillis <= 0)
				{
					this.currentShootCooldownMillis += this.shootCooldownMillis;

					if (this.currentShootCooldownMillis <= 0)
						this.currentShootCooldownMillis = 0;

					if (this.bossEnemy.IsInCombat())
					{
						if (enemies == null)
							enemies = new List<IEnemy>();
						enemies.Add(this);

						int bulletDegreesScaled = this.rng.NextInt(this.deltaBulletDegreesScaled);

						int nextXMillis = ComputeXMillis(bossEnemy: this.bossEnemy, locationInDegreesScaled: shouldRotateClockwise ? (this.degreesScaled + 4 * 128) : (this.degreesScaled - 4 * 128));
						int nextYMillis = ComputeYMillis(bossEnemy: this.bossEnemy, locationInDegreesScaled: shouldRotateClockwise ? (this.degreesScaled + 4 * 128) : (this.degreesScaled - 4 * 128));

						int deltaBulletMovementDirection = DanmakuMath.GetMovementDirectionInDegreesScaled(currentX: this.xMillis, currentY: this.yMillis, desiredX: nextXMillis, desiredY: nextYMillis);

						var bulletOffset = DanmakuMath.GetOffset(
							speedInMillipixelsPerMillisecond: 10,
							movementDirectionInDegreesScaled: deltaBulletMovementDirection,
							elapsedMillisecondsPerIteration: 1000);

						int bulletXMillis = this.xMillis + bulletOffset.DeltaXInMillipixels;
						int bulletYMillis = this.yMillis + bulletOffset.DeltaYInMillipixels;

						while (true)
						{
							if (bulletDegreesScaled >= 360 * 128)
								break;

							int speedInMillipixelsPerMillisecond;
							switch (this.difficulty)
							{
								case Difficulty.Easy:
									speedInMillipixelsPerMillisecond = 50;
									break;
								case Difficulty.Normal:
									speedInMillipixelsPerMillisecond = 100;
									break;
								case Difficulty.Hard:
									speedInMillipixelsPerMillisecond = 125;
									break;
								default:
									throw new Exception();
							}

							enemies.Add(new SimpleEnemyBulletEnemy(
								xMillis: bulletXMillis,
								yMillis: bulletYMillis,
								degreesScaled: bulletDegreesScaled,
								speedInMillipixelsPerMillisecond: speedInMillipixelsPerMillisecond,
								elapsedMillisPerFrame: elapsedMillisPerFrame,
								collisionBoxes: this.bulletCollisionBoxes,
								danmaku2Image: this.orbiterEnemyBulletImage,
								numPixelsOffscreenBeforeDespawn: 50));

							bulletDegreesScaled += this.deltaBulletDegreesScaled;
						}

						return new EnemyFrameResult(
							DTImmutableList<IEnemy>.AsImmutableList(enemies),
							sounds: DTImmutableList<Danmaku2Sound>.AsImmutableList(
								new List<Danmaku2Sound> { Danmaku2Sound.EnemyShoot }),
							bossHealthMeterNumber: null,
							bossHealthMeterMilliPercentage: null,
							shouldEndLevel: false);
					}
				}

				if (enemies == null)
					return this.enemyFrameResult;

				enemies.Add(this);

				return new EnemyFrameResult(
					DTImmutableList<IEnemy>.AsImmutableList(enemies),
					sounds: null,
					bossHealthMeterNumber: null,
					bossHealthMeterMilliPercentage: null,
					shouldEndLevel: false);
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

				Danmaku2Image image = Danmaku2Image.Level1BossPhase2OrbiterEnemyShip;

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


namespace Danmaku2Lib
{
	using System;
	using System.Collections.Generic;

	public class Level1MidbossOrbiterEnemyCreator
	{
		private List<ObjectBox> collisionBoxes;
		private List<ObjectBox> damageBoxes;
		private List<ObjectBox> bulletCollisionBoxes;

		private int elapsedMillisPerFrame;
		private IDTDeterministicRandom rng;

		public Level1MidbossOrbiterEnemyCreator(
			IDTDeterministicRandom rng,
			int elapsedMillisPerFrame)
		{
			this.collisionBoxes = new List<ObjectBox>()
			{
				new ObjectBox(-8000, 8000, -12000, 20000),
				new ObjectBox(-16000, 16000, -4000, 12000)
			};

			this.damageBoxes = new List<ObjectBox>()
			{
				new ObjectBox(-26 * 1024, 26 * 1024, -8 * 1024, 21 * 1024),
				new ObjectBox(-17 * 1024, 17 * 1024, -21 * 1024, 21 * 1024)
			};

			this.bulletCollisionBoxes = new List<ObjectBox>()
			{
				new ObjectBox(-4 * 1024, 4 * 1024, -4 * 1024, 4 * 1024)
			};

			this.elapsedMillisPerFrame = elapsedMillisPerFrame;
			this.rng = rng;
		}

		public IEnemy CreateMidbossOrbiterEnemy(
			Level1MidbossEnemy level1MidbossEnemy,
			int initialDegreesScaled,
			bool shouldRotateClockwise,
			int orbiterEnemyNumber,
			Difficulty difficulty)
		{
			return new MidbossOrbiterEnemy(
				level1MidbossEnemy: level1MidbossEnemy,
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

		private class MidbossOrbiterEnemy : IEnemy
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

			private Level1MidbossEnemy midbossEnemy;

			private int degreesScaled;
			private bool shouldRotateClockwise;

			private int currentShootCooldownMillis;
			private int shootCooldownMillis;

			private int deltaDegreesScaled;

			private Danmaku2Image orbiterEnemyBulletImage;

			private static int ComputeXMillis(IEnemy midbossEnemy, int locationInDegreesScaled, Difficulty difficulty)
			{
				if (difficulty == Difficulty.Normal)
					return midbossEnemy.GetXMillis() + 170 * DTMath.SineScaled(locationInDegreesScaled);

				return midbossEnemy.GetXMillis() + 140 * DTMath.SineScaled(locationInDegreesScaled);
			}

			private static int ComputeYMillis(IEnemy midbossEnemy, int locationInDegreesScaled, Difficulty difficulty)
			{
				if (difficulty == Difficulty.Normal)
					return midbossEnemy.GetYMillis() + 170 * DTMath.CosineScaled(locationInDegreesScaled);

				return midbossEnemy.GetYMillis() + 140 * DTMath.CosineScaled(locationInDegreesScaled);
			}

			public MidbossOrbiterEnemy(
				Level1MidbossEnemy level1MidbossEnemy,
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

				this.xMillis = ComputeXMillis(midbossEnemy: level1MidbossEnemy, locationInDegreesScaled: initialDegreesScaled, difficulty: difficulty);
				this.yMillis = ComputeYMillis(midbossEnemy: level1MidbossEnemy, locationInDegreesScaled: initialDegreesScaled, difficulty: difficulty);

				this.midbossEnemy = level1MidbossEnemy;
				this.degreesScaled = initialDegreesScaled;
				this.shouldRotateClockwise = shouldRotateClockwise;

				this.deltaDegreesScaled = 10 * elapsedMillisPerFrame;

				this.rng = rng;
				
				switch (difficulty)
				{
					case Difficulty.Easy:
						this.shootCooldownMillis = 9000;
						break;
					case Difficulty.Normal:
						this.shootCooldownMillis = 7000;
						break;
					case Difficulty.Hard:
						this.shootCooldownMillis = 2 * 1024;
						break;
					default:
						throw new Exception();
				}

				this.currentShootCooldownMillis = this.shootCooldownMillis * orbiterEnemyNumber / 5;

				this.milliHp = 5600;

				switch (orbiterEnemyNumber)
				{
					case 1:
						this.orbiterEnemyBulletImage = Danmaku2Image.Level1MidbossOrbiterEnemyBullet1;
						break;
					case 2:
						this.orbiterEnemyBulletImage = Danmaku2Image.Level1MidbossOrbiterEnemyBullet2;
						break;
					case 3:
						this.orbiterEnemyBulletImage = Danmaku2Image.Level1MidbossOrbiterEnemyBullet3;
						break;
					case 4:
						this.orbiterEnemyBulletImage = Danmaku2Image.Level1MidbossOrbiterEnemyBullet4;
						break;
					case 5:
						this.orbiterEnemyBulletImage = Danmaku2Image.Level1MidbossOrbiterEnemyBullet5;
						break;
					default:
						throw new Exception();
				}
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

				if (this.midbossEnemy.HasDespawned())
					return null;

				if (shouldRotateClockwise)
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

				this.xMillis = ComputeXMillis(midbossEnemy: this.midbossEnemy, locationInDegreesScaled: this.degreesScaled, difficulty: this.difficulty);
				this.yMillis = ComputeYMillis(midbossEnemy: this.midbossEnemy, locationInDegreesScaled: this.degreesScaled, difficulty: this.difficulty);
				
				this.currentShootCooldownMillis = this.currentShootCooldownMillis - elapsedMillisPerFrame;
				if (this.currentShootCooldownMillis <= 0)
				{
					this.currentShootCooldownMillis += this.shootCooldownMillis;

					if (this.currentShootCooldownMillis <= 0)
						this.currentShootCooldownMillis = 0;

					List<IEnemy> enemies = new List<IEnemy>();
					enemies.Add(this);

					if (this.midbossEnemy.IsInCombat())
					{
						var bulletYMillis = this.yMillis - 25 * 1024;

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

						enemies.Add(new SimpleEnemyBulletEnemy(
							xMillis: this.xMillis,
							yMillis: bulletYMillis,
							degreesScaled: 180 * 128,
							speedInMillipixelsPerMillisecond: speedInMillipixelsPerMillisecond,
							elapsedMillisPerFrame: elapsedMillisPerFrame,
							collisionBoxes: this.bulletCollisionBoxes,
							danmaku2Image: this.orbiterEnemyBulletImage,
							numPixelsOffscreenBeforeDespawn: 20));

						if (this.difficulty == Difficulty.Normal || this.difficulty == Difficulty.Hard)
						{
							enemies.Add(new SimpleEnemyBulletEnemy(
								xMillis: this.xMillis,
								yMillis: bulletYMillis,
								degreesScaled: -170 * 128,
								speedInMillipixelsPerMillisecond: speedInMillipixelsPerMillisecond,
								elapsedMillisPerFrame: elapsedMillisPerFrame,
								collisionBoxes: this.bulletCollisionBoxes,
								danmaku2Image: this.orbiterEnemyBulletImage,
								numPixelsOffscreenBeforeDespawn: 20));
							enemies.Add(new SimpleEnemyBulletEnemy(
								xMillis: this.xMillis,
								yMillis: bulletYMillis,
								degreesScaled: -190 * 128,
								speedInMillipixelsPerMillisecond: speedInMillipixelsPerMillisecond,
								elapsedMillisPerFrame: elapsedMillisPerFrame,
								collisionBoxes: this.bulletCollisionBoxes,
								danmaku2Image: this.orbiterEnemyBulletImage,
								numPixelsOffscreenBeforeDespawn: 20));

							if (this.difficulty == Difficulty.Hard)
							{
								enemies.Add(new SimpleEnemyBulletEnemy(
									xMillis: this.xMillis,
									yMillis: bulletYMillis,
									degreesScaled: -160 * 128,
									speedInMillipixelsPerMillisecond: speedInMillipixelsPerMillisecond,
									elapsedMillisPerFrame: elapsedMillisPerFrame,
									collisionBoxes: this.bulletCollisionBoxes,
									danmaku2Image: this.orbiterEnemyBulletImage,
									numPixelsOffscreenBeforeDespawn: 20));
								enemies.Add(new SimpleEnemyBulletEnemy(
									xMillis: this.xMillis,
									yMillis: bulletYMillis,
									degreesScaled: -200 * 128,
									speedInMillipixelsPerMillisecond: speedInMillipixelsPerMillisecond,
									elapsedMillisPerFrame: elapsedMillisPerFrame,
									collisionBoxes: this.bulletCollisionBoxes,
									danmaku2Image: this.orbiterEnemyBulletImage,
									numPixelsOffscreenBeforeDespawn: 20));
							}
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

				Danmaku2Image image = Danmaku2Image.Level1MidbossOrbiterEnemyShip;

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


namespace Danmaku2Lib
{
	using System;
	using System.Collections.Generic;

	public class Level1MidbossEnemy : IEnemy
	{
		private enum Status
		{
			Approaching,
			InCombat,
			Leaving,
			Despawned
		}

		private int xMillis;
		private int yMillis;

		private List<ObjectBox> collisionBoxes;
		private List<ObjectBox> damageBoxes;
		private List<ObjectBox> bulletCollisionBoxes;
		private List<ObjectBox> anticampingBulletCollisionBoxes;

		private StationaryEnemyMovement stationaryEnemyMovement;

		private Status status;

		private const int INITIAL_MILLI_HP = 40000 / 25;
		private int milliHp;

		private int timerCountdownInMillis;

		private bool hasSpawnedOrbiters;

		private Difficulty difficulty;

		private Level1MidbossOrbiterEnemyCreator orbiterEnemyCreator;

		private IDTDeterministicRandom rng;
		
		private int currentShootCooldownMillis;
		private int shootCooldownMillis;

		private int currentAnticampingShootCooldownMillis;
		private int anticampingShootCooldownMillis;

		public Level1MidbossEnemy(
			IDTDeterministicRandom rng,
			Difficulty difficulty,
			int elapsedMillisPerFrame)
		{
			this.xMillis = 250 * 1024;
			this.yMillis = (GameLogic.GAME_WINDOW_HEIGHT_IN_PIXELS + 500) * 1024;

			this.collisionBoxes = new List<ObjectBox>()
			{
				new ObjectBox(-50 * 1024, 50 * 1024, -50 * 1024, 40 * 1024),
				new ObjectBox(-70 * 1024, 70 * 1024, -30 * 1024, 20 * 1024)
			};

			this.damageBoxes = new List<ObjectBox>()
			{
				new ObjectBox(-35 * 1024, 35 * 1024, 47 * 1024, 67 * 1024),
				new ObjectBox(-47 * 1024, 47 * 1024, -80 * 1024, 47 * 1024),
				new ObjectBox(-60 * 1024, 60 * 1024, -55 * 1024, 30 * 1024),
				new ObjectBox(-72 * 1024, 72 * 1024, -30 * 1024, 30 * 1024),
				new ObjectBox(-90 * 1024, 90 * 1024, -2 * 1024, 30 * 1024),
				new ObjectBox(-103 * 1024, 103 * 1024, 15 * 1024, 30 * 1024)
			};

			this.bulletCollisionBoxes = new List<ObjectBox>()
			{
				new ObjectBox(-4 * 1024, 4 * 1024, -4 * 1024, 4 * 1024)
			};

			this.anticampingBulletCollisionBoxes = new List<ObjectBox>()
			{
				new ObjectBox(-4 * 1024, 4 * 1024, -4 * 1024, 4 * 1024)
			};

			this.difficulty = difficulty;

			this.rng = rng;

			this.stationaryEnemyMovement = new StationaryEnemyMovement(
				xMillis: this.xMillis,
				initialYMillis: this.yMillis,
				destinationYMillis: 500 * 1024,
				stoppedDurationInMillis: null);

			this.status = Status.Approaching;

			this.milliHp = INITIAL_MILLI_HP;

			this.timerCountdownInMillis = 80 * 1000;

			this.hasSpawnedOrbiters = false;

			this.orbiterEnemyCreator = new Level1MidbossOrbiterEnemyCreator(rng: rng, elapsedMillisPerFrame: elapsedMillisPerFrame);

			switch (difficulty)
			{
				case Difficulty.Easy:
					this.shootCooldownMillis = 2000;
					this.anticampingShootCooldownMillis = 2000;
					break;
				case Difficulty.Normal:
					this.shootCooldownMillis = 2000;
					this.anticampingShootCooldownMillis = 2000;
					break;
				case Difficulty.Hard:
					this.shootCooldownMillis = 2000;
					this.anticampingShootCooldownMillis = 700;
					break;
				default:
					throw new Exception();
			}

			this.currentShootCooldownMillis = rng.NextInt(this.shootCooldownMillis);
			this.currentAnticampingShootCooldownMillis = rng.NextInt(this.anticampingShootCooldownMillis);
		}

		public List<ObjectBox> GetCollisionBoxes()
		{
			return this.collisionBoxes;
		}

		public List<ObjectBox> GetDamageBoxes()
		{
			return this.damageBoxes;
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
			return ZIndex.Enemy;
		}

		public void HandleCollisionWithPlayer()
		{
		}

		public void HandleCollisionWithPlayerBullet(PlayerBulletStrength bulletStrength)
		{
			if (this.status == Status.InCombat)
			{
				int bulletDamage = PlayerBullet.GetBulletDamage(playerBulletStrength: bulletStrength);
				this.milliHp = this.milliHp - (bulletDamage / 25);
				
				if (this.milliHp < 0)
					this.milliHp = 0;
			}
		}

		private static bool IsPlayerCamping(int playerXMillis, int playerYMillis, Difficulty difficulty)
		{
			switch (difficulty)
			{
				case Difficulty.Easy:
					return false;
				case Difficulty.Normal:
					if (playerYMillis > 500 * 1024)
						return true;

					// The line y = (35/9) * x + 100
					// starts at (0, 100) and hits (103, 500)
					if (playerYMillis >= 35 * playerXMillis / 9 + 100 * 1024)
						return true;

					// The line y = (-35/9) * x + 2045
					// hits (397, 500) and ends at (500, 100)
					if (playerYMillis >= -35 * playerXMillis / 9 + 2045 * 1024)
						return true;

					return false;
				case Difficulty.Hard:
					if (playerYMillis > 500 * 1024)
						return true;

					// The line y = (5/4) * x + 250
					// starts at (0, 250) and hits (200, 500)
					if (playerYMillis >= 5 * playerXMillis / 4 + 250 * 1024)
						return true;

					// The line y = (-5/4) * x + 875
					// hits (300, 500) and ends at (500, 250)
					if (playerYMillis >= -5 * playerXMillis / 4 + 875 * 1024)
						return true;

					return false;
				default:
					throw new Exception();
			}
		}

		public EnemyFrameResult ProcessFrame(int elapsedMillisPerFrame, int playerXMillis, int playerYMillis)
		{
			List<IEnemy> enemies = new List<IEnemy>();

			if (!this.hasSpawnedOrbiters)
			{
				this.hasSpawnedOrbiters = true;

				bool shouldRotateClockwise = this.rng.NextBool();

				int initialDegreesScaled = this.rng.NextInt(360 * 128);

				List<int> orbiterNum = new List<int>() { 1, 2, 3, 4, 5 };

				for (int i = 0; i < 5; i++)
				{
					int random = this.rng.NextInt(orbiterNum.Count);
					int orbiterEnemyNumber = orbiterNum[random];
					orbiterNum.RemoveAt(random);

					enemies.Add(this.orbiterEnemyCreator.CreateMidbossOrbiterEnemy(
						level1MidbossEnemy: this,
						initialDegreesScaled: initialDegreesScaled + (360 * 128 / 5) * i,
						shouldRotateClockwise: shouldRotateClockwise,
						orbiterEnemyNumber: orbiterEnemyNumber,
						difficulty: this.difficulty));
				}
			}

			this.stationaryEnemyMovement.ProcessNextFrame(elapsedMillisPerFrame: elapsedMillisPerFrame);

			this.xMillis = this.stationaryEnemyMovement.GetXMillis();
			this.yMillis = this.stationaryEnemyMovement.GetYMillis();

			if (this.status == Status.Approaching && this.stationaryEnemyMovement.HasStopped())
				this.status = Status.InCombat;

			int? bossHealthMeterNumber;
			int? bossHealthMeterMilliPercentage;

			if (this.status == Status.InCombat)
			{
				bossHealthMeterNumber = 1;
				bossHealthMeterMilliPercentage = (this.milliHp << 17) / INITIAL_MILLI_HP;
			}
			else
			{
				bossHealthMeterNumber = null;
				bossHealthMeterMilliPercentage = null;
			}

			if (this.status == Status.InCombat && this.milliHp <= 0)
			{
				this.stationaryEnemyMovement.ForceLeave();
				this.status = Status.Leaving;
			}

			if (this.status == Status.InCombat)
			{
				this.timerCountdownInMillis -= elapsedMillisPerFrame;

				if (this.timerCountdownInMillis <= 0)
				{
					this.timerCountdownInMillis = 0;
					this.stationaryEnemyMovement.ForceLeave();
					this.status = Status.Leaving;
				}
			}

			bool hasFiredBullet = false;
			bool hasFiredAnticampingBullet = false;

			if (this.status == Status.InCombat)
			{
				this.currentShootCooldownMillis = this.currentShootCooldownMillis - elapsedMillisPerFrame;
				this.currentAnticampingShootCooldownMillis = this.currentAnticampingShootCooldownMillis - elapsedMillisPerFrame;

				if (this.currentShootCooldownMillis <= 0)
				{
					this.currentShootCooldownMillis += this.shootCooldownMillis;
					if (this.currentShootCooldownMillis < 0)
						this.currentShootCooldownMillis = 0;

					hasFiredBullet = true;

					int degreesScaled = DanmakuMath.GetMovementDirectionInDegreesScaled(
						currentX: this.xMillis,
						currentY: this.yMillis,
						desiredX: playerXMillis,
						desiredY: playerYMillis);

					var offset1 = DanmakuMath.GetOffset(
						speedInMillipixelsPerMillisecond: 9,
						movementDirectionInDegreesScaled: degreesScaled + 90 * 128,
						elapsedMillisecondsPerIteration: 1000);

					var offset2 = DanmakuMath.GetOffset(
						speedInMillipixelsPerMillisecond: 18,
						movementDirectionInDegreesScaled: degreesScaled + 110 * 128,
						elapsedMillisecondsPerIteration: 1000);

					var offset3 = DanmakuMath.GetOffset(
						speedInMillipixelsPerMillisecond: 18,
						movementDirectionInDegreesScaled: degreesScaled - 110 * 128,
						elapsedMillisecondsPerIteration: 1000);

					enemies.Add(new SimpleEnemyBulletEnemy(
						xMillis: this.xMillis,
						yMillis: this.yMillis,
						degreesScaled: degreesScaled,
						speedInMillipixelsPerMillisecond: 200,
						elapsedMillisPerFrame: elapsedMillisPerFrame,
						collisionBoxes: this.bulletCollisionBoxes,
						danmaku2Image: Danmaku2Image.Level1MidbossEnemyBullet,
						numPixelsOffscreenBeforeDespawn: 40));
					enemies.Add(new SimpleEnemyBulletEnemy(
						xMillis: this.xMillis + offset1.DeltaXInMillipixels,
						yMillis: this.yMillis + offset1.DeltaYInMillipixels,
						degreesScaled: degreesScaled,
						speedInMillipixelsPerMillisecond: 200,
						elapsedMillisPerFrame: elapsedMillisPerFrame,
						collisionBoxes: this.bulletCollisionBoxes,
						danmaku2Image: Danmaku2Image.Level1MidbossEnemyBullet,
						numPixelsOffscreenBeforeDespawn: 40));
					enemies.Add(new SimpleEnemyBulletEnemy(
						xMillis: this.xMillis - offset1.DeltaXInMillipixels,
						yMillis: this.yMillis - offset1.DeltaYInMillipixels,
						degreesScaled: degreesScaled,
						speedInMillipixelsPerMillisecond: 200,
						elapsedMillisPerFrame: elapsedMillisPerFrame,
						collisionBoxes: this.bulletCollisionBoxes,
						danmaku2Image: Danmaku2Image.Level1MidbossEnemyBullet,
						numPixelsOffscreenBeforeDespawn: 40));
					enemies.Add(new SimpleEnemyBulletEnemy(
						xMillis: this.xMillis + offset2.DeltaXInMillipixels,
						yMillis: this.yMillis + offset2.DeltaYInMillipixels,
						degreesScaled: degreesScaled,
						speedInMillipixelsPerMillisecond: 200,
						elapsedMillisPerFrame: elapsedMillisPerFrame,
						collisionBoxes: this.bulletCollisionBoxes,
						danmaku2Image: Danmaku2Image.Level1MidbossEnemyBullet,
						numPixelsOffscreenBeforeDespawn: 40));
					enemies.Add(new SimpleEnemyBulletEnemy(
						xMillis: this.xMillis + offset3.DeltaXInMillipixels,
						yMillis: this.yMillis + offset3.DeltaYInMillipixels,
						degreesScaled: degreesScaled,
						speedInMillipixelsPerMillisecond: 200,
						elapsedMillisPerFrame: elapsedMillisPerFrame,
						collisionBoxes: this.bulletCollisionBoxes,
						danmaku2Image: Danmaku2Image.Level1MidbossEnemyBullet,
						numPixelsOffscreenBeforeDespawn: 40));
				}

				if (this.currentAnticampingShootCooldownMillis <= 0)
				{
					this.currentAnticampingShootCooldownMillis += this.anticampingShootCooldownMillis;
					if (this.currentAnticampingShootCooldownMillis < 0)
						this.currentAnticampingShootCooldownMillis = 0;

					if (IsPlayerCamping(playerXMillis: playerXMillis, playerYMillis: playerYMillis, difficulty: this.difficulty))
					{
						hasFiredAnticampingBullet = true;

						int maxDegreesScaled = this.difficulty == Difficulty.Normal
							? 150 * 128
							: 140 * 128;

						for (int degreesScaled = rng.NextInt(5 * 128); degreesScaled <= maxDegreesScaled; degreesScaled += 5 * 128)
						{
							enemies.Add(new SimpleEnemyBulletEnemy(
								xMillis: this.xMillis,
								yMillis: this.yMillis,
								degreesScaled: degreesScaled,
								speedInMillipixelsPerMillisecond: 40,
								elapsedMillisPerFrame: elapsedMillisPerFrame,
								collisionBoxes: this.anticampingBulletCollisionBoxes,
								danmaku2Image: Danmaku2Image.Level1MidbossEnemyAnticampingBullet,
								numPixelsOffscreenBeforeDespawn: 20));
							enemies.Add(new SimpleEnemyBulletEnemy(
								xMillis: this.xMillis,
								yMillis: this.yMillis,
								degreesScaled: -degreesScaled,
								speedInMillipixelsPerMillisecond: 40,
								elapsedMillisPerFrame: elapsedMillisPerFrame,
								collisionBoxes: this.anticampingBulletCollisionBoxes,
								danmaku2Image: Danmaku2Image.Level1MidbossEnemyAnticampingBullet,
								numPixelsOffscreenBeforeDespawn: 20));
						}
					}
				}
			}

			if (this.status == Status.Leaving)
			{
				if (this.stationaryEnemyMovement.HasLeftScreen())
					this.status = Status.Despawned;
			}

			if (this.status != Status.Despawned)
				enemies.Add(this);

			List<Danmaku2Sound> sounds = null;

			if (hasFiredBullet)
			{
				if (sounds == null)
					sounds = new List<Danmaku2Sound>();

				sounds.Add(Danmaku2Sound.LargeShoot);
			}

			if (hasFiredAnticampingBullet)
			{
				if (sounds == null)
					sounds = new List<Danmaku2Sound>();

				sounds.Add(Danmaku2Sound.ShootNoise);
			}

			return new EnemyFrameResult(
				enemies: DTImmutableList<IEnemy>.AsImmutableList(enemies),
				sounds: sounds == null ? null : DTImmutableList<Danmaku2Sound>.AsImmutableList(sounds),
				bossHealthMeterNumber: bossHealthMeterNumber,
				bossHealthMeterMilliPercentage: bossHealthMeterMilliPercentage,
				shouldEndLevel: false);
		}

		public bool IsInCombat()
		{
			return this.status == Status.InCombat;
		}

		public bool HasDespawned()
		{
			return this.status == Status.Despawned;
		}

		public void Render(IDisplay<Danmaku2Assets> display)
		{
			var assets = display.GetAssets();

			Danmaku2Image image = Danmaku2Image.Level1Midboss;

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

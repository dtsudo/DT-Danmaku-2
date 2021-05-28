
namespace Danmaku2Lib
{
	using System;
	using System.Collections.Generic;

	public class MediumEnemyShootPattern
	{
		private IEnemy enemy;

		private List<ObjectBox> bulletCollisionBoxes;
		
		private Difficulty difficulty;
		private IDTDeterministicRandom rng;

		private List<Danmaku2Sound> bulletShootSound;

		private int xVelocityOffsetPerFrameMillis;
		private int yVelocityOffsetPerFrameMillis;

		private int currentShootCooldownMillis1;
		private int currentShootDirectionScaled1;
		private int currentShootDirectionScaled2;
		private bool shouldDirection1RotateClockwise;
		private const int SHOOT_COOLDOWN_MILLIS_EASY = 575;
		private const int SHOOT_COOLDOWN_MILLIS_NORMAL = 250;
		private const int SHOOT_COOLDOWN_MILLIS_HARD = 150;

		public MediumEnemyShootPattern(
			IEnemy enemy,
			bool bulletsRotateClockwise,
			int shootDirectionScaled1,
			int shootDirectionScaled2,
			int xVelocityOffsetPerFrameMillis,
			int yVelocityOffsetPerFrameMillis,
			Difficulty difficulty,
			IDTDeterministicRandom rng)
		{
			this.enemy = enemy;
			
			this.bulletCollisionBoxes = new List<ObjectBox>()
			{
				new ObjectBox(-4 * 1024, 4 * 1024, -4 * 1024, 4 * 1024)
			};
			
			this.difficulty = difficulty;
			
			this.rng = rng;

			this.bulletShootSound = new List<Danmaku2Sound>() { Danmaku2Sound.Shoot23 };

			switch (difficulty)
			{
				case Difficulty.Easy:
					this.currentShootCooldownMillis1 = rng.NextInt(SHOOT_COOLDOWN_MILLIS_EASY);
					break;
				case Difficulty.Normal:
					this.currentShootCooldownMillis1 = rng.NextInt(SHOOT_COOLDOWN_MILLIS_NORMAL);
					break;
				case Difficulty.Hard:
					this.currentShootCooldownMillis1 = rng.NextInt(SHOOT_COOLDOWN_MILLIS_HARD);
					break;
				default:
					throw new Exception();
			}
			
			this.currentShootDirectionScaled1 = DanmakuMath.NormalizeAngleInDegreesScaled(angleInDegreesScaled: shootDirectionScaled1);
			this.currentShootDirectionScaled2 = DanmakuMath.NormalizeAngleInDegreesScaled(angleInDegreesScaled: shootDirectionScaled2);
			this.shouldDirection1RotateClockwise = bulletsRotateClockwise;

			this.xVelocityOffsetPerFrameMillis = xVelocityOffsetPerFrameMillis;
			this.yVelocityOffsetPerFrameMillis = yVelocityOffsetPerFrameMillis;
		}
		
		public class Result
		{
			public Result(
				List<IEnemy> enemies,
				List<Danmaku2Sound> sounds)
			{
				this.Enemies = enemies;
				this.Sounds = sounds;
			}

			/// <summary>
			/// Can be null or empty
			/// </summary>
			public List<IEnemy> Enemies { get; private set; }

			/// <summary>
			/// Can be null or empty
			/// </summary>
			public List<Danmaku2Sound> Sounds { get; private set; }
		}

		/// <summary>
		/// Can return a null value.
		/// </summary>
		public Result ProcessFrame(int elapsedMillisPerFrame)
		{
			this.currentShootCooldownMillis1 = this.currentShootCooldownMillis1 - elapsedMillisPerFrame;

			if (this.shouldDirection1RotateClockwise)
			{
				this.currentShootDirectionScaled1 = this.currentShootDirectionScaled1 + (elapsedMillisPerFrame << 3);
				this.currentShootDirectionScaled2 = this.currentShootDirectionScaled2 - (elapsedMillisPerFrame << 4);

				while (this.currentShootDirectionScaled1 >= 360 * 128)
				{
					this.currentShootDirectionScaled1 -= 360 * 128;
				}
				while (this.currentShootDirectionScaled2 < 0)
				{
					this.currentShootDirectionScaled2 += 360 * 128;
				}
			}
			else
			{
				this.currentShootDirectionScaled1 = this.currentShootDirectionScaled1 - (elapsedMillisPerFrame << 3);
				this.currentShootDirectionScaled2 = this.currentShootDirectionScaled2 + (elapsedMillisPerFrame << 4);

				while (this.currentShootDirectionScaled1 < 0)
				{
					this.currentShootDirectionScaled1 += 360 * 128;
				}
				while (this.currentShootDirectionScaled2 >= 360 * 128)
				{
					this.currentShootDirectionScaled2 -= 360 * 128;
				}
			}
				
			if (this.currentShootCooldownMillis1 <= 0)
			{
				switch (this.difficulty)
				{
					case Difficulty.Easy:
						this.currentShootCooldownMillis1 += SHOOT_COOLDOWN_MILLIS_EASY;
						break;
					case Difficulty.Normal:
						this.currentShootCooldownMillis1 += SHOOT_COOLDOWN_MILLIS_NORMAL;
						break;
					case Difficulty.Hard:
						this.currentShootCooldownMillis1 += SHOOT_COOLDOWN_MILLIS_HARD;
						break;
					default:
						throw new Exception();
				}

				if (this.currentShootCooldownMillis1 <= 0)
					this.currentShootCooldownMillis1 = 0;
				
				List<IEnemy> enemies = new List<IEnemy>();

				int xMillis = this.enemy.GetXMillis();
				int yMillis = this.enemy.GetYMillis() - 25 * 1024;

				switch (this.difficulty)
				{
					case Difficulty.Easy:
						var bullet1 = new SimpleEnemyBulletWithVelocityOffsetEnemy(
							xMillis: xMillis,
							yMillis: yMillis,
							degreesScaled: this.currentShootDirectionScaled1,
							speedInMillipixelsPerMillisecond: 70,
							elapsedMillisPerFrame: elapsedMillisPerFrame,
							collisionBoxes: this.bulletCollisionBoxes,
							danmaku2Image: Danmaku2Image.MediumEnemyBullet1,
							numPixelsOffscreenBeforeDespawn: 100,
							xVelocityOffsetPerFrameMillis: this.xVelocityOffsetPerFrameMillis,
							yVelocityOffsetPerFrameMillis: this.yVelocityOffsetPerFrameMillis);
						enemies.Add(bullet1);

						var bullet2 = new SimpleEnemyBulletWithVelocityOffsetEnemy(
							xMillis: xMillis,
							yMillis: yMillis,
							degreesScaled: this.currentShootDirectionScaled1 + 180 * 128,
							speedInMillipixelsPerMillisecond: 70,
							elapsedMillisPerFrame: elapsedMillisPerFrame,
							collisionBoxes: this.bulletCollisionBoxes,
							danmaku2Image: Danmaku2Image.MediumEnemyBullet1,
							numPixelsOffscreenBeforeDespawn: 100,
							xVelocityOffsetPerFrameMillis: this.xVelocityOffsetPerFrameMillis,
							yVelocityOffsetPerFrameMillis: this.yVelocityOffsetPerFrameMillis);
						enemies.Add(bullet2);

						break;
					case Difficulty.Normal:
					{
						int bulletDegrees = this.currentShootDirectionScaled1;
								
						for (int i = 0; i < 3; i++)
						{
							var bullet = new SimpleEnemyBulletWithVelocityOffsetEnemy(
								xMillis: xMillis,
								yMillis: yMillis,
								degreesScaled: bulletDegrees,
								speedInMillipixelsPerMillisecond: 110,
								elapsedMillisPerFrame: elapsedMillisPerFrame,
								collisionBoxes: this.bulletCollisionBoxes,
								danmaku2Image: Danmaku2Image.MediumEnemyBullet1,
								numPixelsOffscreenBeforeDespawn: 100,
								xVelocityOffsetPerFrameMillis: this.xVelocityOffsetPerFrameMillis,
								yVelocityOffsetPerFrameMillis: this.yVelocityOffsetPerFrameMillis);
								enemies.Add(bullet);
									
							bulletDegrees += 120 * 128;
						}

						break;
					}
					case Difficulty.Hard:
					{
						int bulletDegrees1 = this.currentShootDirectionScaled1;
								
						for (int i = 0; i < 3; i++)
						{
							var bullet = new SimpleEnemyBulletWithVelocityOffsetEnemy(
								xMillis: xMillis,
								yMillis: yMillis,
								degreesScaled: bulletDegrees1,
								speedInMillipixelsPerMillisecond: 230,
								elapsedMillisPerFrame: elapsedMillisPerFrame,
								collisionBoxes: this.bulletCollisionBoxes,
								danmaku2Image: Danmaku2Image.MediumEnemyBullet2,
								numPixelsOffscreenBeforeDespawn: 100,
								xVelocityOffsetPerFrameMillis: this.xVelocityOffsetPerFrameMillis,
								yVelocityOffsetPerFrameMillis: this.yVelocityOffsetPerFrameMillis);
								enemies.Add(bullet);
									
							bulletDegrees1 += 120 * 128;
						}

						int bulletDegrees2 = this.currentShootDirectionScaled2;
								
						for (int i = 0; i < 5; i++)
						{
							var bullet = new SimpleEnemyBulletWithVelocityOffsetEnemy(
								xMillis: xMillis,
								yMillis: yMillis,
								degreesScaled: bulletDegrees2,
								speedInMillipixelsPerMillisecond: 110,
								elapsedMillisPerFrame: elapsedMillisPerFrame,
								collisionBoxes: this.bulletCollisionBoxes,
								danmaku2Image: Danmaku2Image.MediumEnemyBullet1,
								numPixelsOffscreenBeforeDespawn: 100,
								xVelocityOffsetPerFrameMillis: this.xVelocityOffsetPerFrameMillis,
								yVelocityOffsetPerFrameMillis: this.yVelocityOffsetPerFrameMillis);
								enemies.Add(bullet);
									
							bulletDegrees2 += 72 * 128;
						}

						break;
					}
					default:
						throw new Exception();
				}
				return new Result(
					enemies: enemies,
					sounds: this.bulletShootSound);
			}

			return null;
		}
	}
}

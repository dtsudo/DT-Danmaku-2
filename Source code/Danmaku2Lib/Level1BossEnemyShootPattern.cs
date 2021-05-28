
namespace Danmaku2Lib
{
	using System;
	using System.Collections.Generic;

	public class Level1BossEnemyShootPattern
	{
		public enum Phase
		{
			Phase1,
			Phase2
		}

		private Phase phase;

		private IEnemy enemy;

		private List<ObjectBox> bulletCollisionBoxes;
		
		private Difficulty difficulty;
		private IDTDeterministicRandom rng;

		private List<Danmaku2Sound> bulletShootSound;
		
		private int currentShootCooldownMillis;
		private int currentShootDirectionScaled1;
		private int currentShootDirectionScaled2;
		private bool shouldDirection1RotateClockwise;

		private static int GetShootCooldownMillis(Difficulty difficulty, Phase phase)
		{
			switch (difficulty)
			{
				case Difficulty.Easy:
					switch (phase)
					{
						case Phase.Phase1:
							return 950;
						case Phase.Phase2:
							return 950 / 2;
						default:
							throw new Exception();
					}
				case Difficulty.Normal:
					switch (phase)
					{
						case Phase.Phase1:
							return 750;
						case Phase.Phase2:
							return 750 / 2;
						default:
							throw new Exception();
					}
				case Difficulty.Hard:
					switch (phase)
					{
						case Phase.Phase1:
							return 250;
						case Phase.Phase2:
							return 160;
						default:
							throw new Exception();
					}
				default:
					throw new Exception();
			}
		}

		public Level1BossEnemyShootPattern(
			IEnemy enemy,
			Difficulty difficulty,
			IDTDeterministicRandom rng,
			Phase phase)
		{
			this.enemy = enemy;

			this.phase = phase;

			this.bulletCollisionBoxes = new List<ObjectBox>()
			{
				new ObjectBox(-4 * 1024, 4 * 1024, -4 * 1024, 4 * 1024)
			};
			
			this.difficulty = difficulty;
			
			this.rng = rng;

			this.bulletShootSound = new List<Danmaku2Sound>() { Danmaku2Sound.Shoot23 };

			this.currentShootCooldownMillis = rng.NextInt(GetShootCooldownMillis(difficulty: difficulty, phase: phase));

			this.currentShootDirectionScaled1 = rng.NextInt(360 * 128);
			this.currentShootDirectionScaled2 = rng.NextInt(360 * 128);
			this.shouldDirection1RotateClockwise = rng.NextBool();
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
		/// Can return a null value
		/// </summary>
		public Result ProcessFrame(int elapsedMillisPerFrame)
		{
			this.currentShootCooldownMillis = this.currentShootCooldownMillis - elapsedMillisPerFrame;

			int directionScaledDelta;

			switch (this.difficulty)
			{
				case Difficulty.Easy:
					directionScaledDelta = elapsedMillisPerFrame >> 1;
					break;
				case Difficulty.Normal:
					directionScaledDelta = (elapsedMillisPerFrame << 1) / 3;
					break;
				case Difficulty.Hard:
					directionScaledDelta = elapsedMillisPerFrame << 1;
					break;
				default:
					throw new Exception();
			}

			if (this.shouldDirection1RotateClockwise)
			{
				this.currentShootDirectionScaled1 = this.currentShootDirectionScaled1 + directionScaledDelta;
				this.currentShootDirectionScaled2 = this.currentShootDirectionScaled2 - directionScaledDelta;

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
				this.currentShootDirectionScaled1 = this.currentShootDirectionScaled1 - directionScaledDelta;
				this.currentShootDirectionScaled2 = this.currentShootDirectionScaled2 + directionScaledDelta;

				while (this.currentShootDirectionScaled1 < 0)
				{
					this.currentShootDirectionScaled1 += 360 * 128;
				}
				while (this.currentShootDirectionScaled2 >= 360 * 128)
				{
					this.currentShootDirectionScaled2 -= 360 * 128;
				}
			}
				
			if (this.currentShootCooldownMillis <= 0)
			{
				this.currentShootCooldownMillis += GetShootCooldownMillis(difficulty: this.difficulty, phase: this.phase);
				
				if (this.currentShootCooldownMillis <= 0)
					this.currentShootCooldownMillis = 0;
				
				List<IEnemy> enemies = new List<IEnemy>();

				int xMillis = this.enemy.GetXMillis();
				int yMillis = this.enemy.GetYMillis() - 25 * 1024;
				
				int bulletDegrees1 = this.currentShootDirectionScaled1;

				int numBullets;
				int bulletSpeedInMillipixelsPerMilliseconds;

				switch (this.difficulty)
				{
					case Difficulty.Easy:
						numBullets = 3;
						bulletSpeedInMillipixelsPerMilliseconds = 65;
						break;
					case Difficulty.Normal:
						numBullets = 5;
						bulletSpeedInMillipixelsPerMilliseconds = 85;
						break;
					case Difficulty.Hard:
						numBullets = 10;
						bulletSpeedInMillipixelsPerMilliseconds = 110;
						break;
					default:
						throw new Exception();
				}

				if (this.phase == Phase.Phase2)
					bulletSpeedInMillipixelsPerMilliseconds = bulletSpeedInMillipixelsPerMilliseconds * 2;

				int degreesScaledBetweenBullets = 360 * 128 / numBullets;

				Danmaku2Image bulletImage;
				switch (this.phase)
				{
					case Phase.Phase1:
						bulletImage = Danmaku2Image.Level1BossEnemyBullet1;
						break;
					case Phase.Phase2:
						bulletImage = Danmaku2Image.Level1BossEnemyBullet2;
						break;
					default:
						throw new Exception();
				}

				for (int i = 0; i < numBullets; i++)
				{
					var bullet = new SimpleEnemyBulletEnemy(
						xMillis: xMillis,
						yMillis: yMillis,
						degreesScaled: bulletDegrees1,
						speedInMillipixelsPerMillisecond: bulletSpeedInMillipixelsPerMilliseconds,
						elapsedMillisPerFrame: elapsedMillisPerFrame,
						collisionBoxes: this.bulletCollisionBoxes,
						danmaku2Image: bulletImage,
						numPixelsOffscreenBeforeDespawn: 100);
					
					enemies.Add(bullet);
									
					bulletDegrees1 += degreesScaledBetweenBullets;
				}

				int bulletDegrees2 = this.currentShootDirectionScaled2;
								
				for (int i = 0; i < numBullets; i++)
				{
					var bullet = new SimpleEnemyBulletEnemy(
						xMillis: xMillis,
						yMillis: yMillis,
						degreesScaled: bulletDegrees2,
						speedInMillipixelsPerMillisecond: bulletSpeedInMillipixelsPerMilliseconds,
						elapsedMillisPerFrame: elapsedMillisPerFrame,
						collisionBoxes: this.bulletCollisionBoxes,
						danmaku2Image: bulletImage,
						numPixelsOffscreenBeforeDespawn: 100);
					
					enemies.Add(bullet);
									
					bulletDegrees2 += degreesScaledBetweenBullets;
				}
					
				return new Result(
					enemies: enemies,
					sounds: this.bulletShootSound);
			}

			return null;
		}
	}
}

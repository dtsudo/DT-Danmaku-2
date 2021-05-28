
namespace Danmaku2Lib
{
	using System;
	using System.Collections.Generic;

	public class Level1Phase1EnemyGeneratorEnemy : IEnemy
	{
		private Difficulty difficulty;
		private SniperEnemyCreator sniperEnemyCreator;
		private BasicEnemyCreator basicEnemyCreator;
		private MediumEnemyCreator mediumEnemyCreator;

		private int elapsedMillisPerFrame;
		private int numMillisElapsed;

		private HashSet<string> spawnedEnemies;

		private IDTDeterministicRandom rng;

		private bool hasPhase1Finished;
		
		private EnemyFrameResult enemyFrameResult;

		public Level1Phase1EnemyGeneratorEnemy(
			Difficulty difficulty,
			int elapsedMillisPerFrame,
			IDTDeterministicRandom rng)
		{
			this.difficulty = difficulty;
			this.sniperEnemyCreator = new SniperEnemyCreator(rng: rng);
			this.basicEnemyCreator = new BasicEnemyCreator(rng: rng, elapsedMillisPerFrame: elapsedMillisPerFrame);
			this.mediumEnemyCreator = new MediumEnemyCreator(rng: rng);

			this.elapsedMillisPerFrame = elapsedMillisPerFrame;

			this.rng = rng;

			this.numMillisElapsed = 0;

			this.spawnedEnemies = new HashSet<string>();

			this.hasPhase1Finished = false;
			
			this.enemyFrameResult = new EnemyFrameResult(
				enemies: DTImmutableList<IEnemy>.AsImmutableList(new List<IEnemy>() { this }),
				sounds: null,
				bossHealthMeterNumber: null,
				bossHealthMeterMilliPercentage: null,
				shouldEndLevel: false);
		}

		public int GetXMillis()
		{
			return 0;
		}

		public int GetYMillis()
		{
			return 0;
		}

		public List<ObjectBox> GetCollisionBoxes()
		{
			return null;
		}

		public List<ObjectBox> GetDamageBoxes()
		{
			return null;
		}

		private int? basicEnemyWave1XMillis = null;
		private void TrySpawnBasicEnemyWave1(List<IEnemy> enemies)
		{
			if (!this.spawnedEnemies.Contains("TrySpawnBasicEnemyWave1_4"))
			{
				for (int i = 0; i < 5; i++)
				{
					if (this.numMillisElapsed > 2000 + i * 2000 && !this.spawnedEnemies.Contains(StringConcatenation.Concat("TrySpawnBasicEnemyWave1_", i)))
					{
						if (this.basicEnemyWave1XMillis == null)
							this.basicEnemyWave1XMillis = (50 + this.rng.NextInt(400)) << 10;

						this.spawnedEnemies.Add(StringConcatenation.Concat("TrySpawnBasicEnemyWave1_", i));

						enemies.Add(this.basicEnemyCreator.CreateBasicEnemy(xMillis: this.basicEnemyWave1XMillis.Value, difficulty: this.difficulty));
					}
				}
			}
		}

		private int? basicEnemyWave2XMillis = null;
		private void TrySpawnBasicEnemyWave2(List<IEnemy> enemies)
		{
			if (!this.spawnedEnemies.Contains("TrySpawnBasicEnemyWave2_4"))
			{
				for (int i = 0; i < 5; i++)
				{
					if (this.numMillisElapsed > 5500 + i * 2000 && !this.spawnedEnemies.Contains(StringConcatenation.Concat("TrySpawnBasicEnemyWave2_", i)))
					{
						this.spawnedEnemies.Add(StringConcatenation.Concat("TrySpawnBasicEnemyWave2_", i));

						if (this.basicEnemyWave2XMillis == null)
						{
							this.basicEnemyWave2XMillis = (50 + this.rng.NextInt(400)) << 10;

							int count = 0;
							while (count < 50 && this.basicEnemyWave1XMillis != null && Math.Abs(this.basicEnemyWave1XMillis.Value - this.basicEnemyWave2XMillis.Value) < 150 * 1024)
							{
								this.basicEnemyWave2XMillis = (50 + this.rng.NextInt(400)) << 10;
								count++;
							}
						}

						enemies.Add(this.basicEnemyCreator.CreateBasicEnemy(xMillis: this.basicEnemyWave2XMillis.Value, difficulty: this.difficulty));
					}
				}
			}
		}

		private int? mediumEnemyWave1XMillis = null;
		private void TrySpawnMediumEnemyWave1(List<IEnemy> enemies)
		{
			if (!this.spawnedEnemies.Contains("TrySpawnMediumEnemyWave1"))
			{
				if (this.numMillisElapsed > 13500)
				{
					this.spawnedEnemies.Add("TrySpawnMediumEnemyWave1");

					this.mediumEnemyWave1XMillis = 100 * 1024;

					if (this.basicEnemyWave2XMillis != null && this.basicEnemyWave2XMillis.Value < 250 * 1024)
						this.mediumEnemyWave1XMillis = 400 * 1024;
					
					enemies.Add(this.mediumEnemyCreator.CreateMediumEnemy(
						xMillis: this.mediumEnemyWave1XMillis.Value,
						bulletsRotateClockwise: this.rng.NextBool(),
						shootDirectionScaled1: this.rng.NextInt(360 * 128),
						shootDirectionScaled2: this.rng.NextInt(360 * 128),
						difficulty: this.difficulty));
				}
			}
		}

		private List<int> sniperEnemyWave1XMillis = null;
		private void TrySpawnSniperEnemyWave1(List<IEnemy> enemies)
		{
			if (!this.spawnedEnemies.Contains("TrySpawnSniperEnemyWave1_2"))
			{
				for (int i = 0; i < 3; i++)
				{
					if (this.numMillisElapsed > 14 * 1000 + i * 2000 && !this.spawnedEnemies.Contains(StringConcatenation.Concat("TrySpawnSniperEnemyWave1_", i)))
					{
						this.spawnedEnemies.Add(StringConcatenation.Concat("TrySpawnSniperEnemyWave1_", i));

						if (this.sniperEnemyWave1XMillis == null)
							this.sniperEnemyWave1XMillis = new List<int>();

						bool onRightSide = this.mediumEnemyWave1XMillis != null && this.mediumEnemyWave1XMillis.Value < 250 * 1024;

						int xMillis = (this.rng.NextInt(200) + (onRightSide ? 250 : 50)) << 10;
						
						int count = 0;
						while (true)
						{
							count++;
							if (count >= 100)
								break;

							bool isValid = true;

							for (int x = 0; x < this.sniperEnemyWave1XMillis.Count; x++)
							{
								var otherXMillis = this.sniperEnemyWave1XMillis[x];
								if (Math.Abs(otherXMillis - xMillis) < 70 * 1024)
								{
									isValid = false;
									break;
								}
							}

							if (isValid)
								break;

							xMillis = (this.rng.NextInt(200) + (onRightSide ? 250 : 50)) << 10;
						}

						this.sniperEnemyWave1XMillis.Add(xMillis);

						enemies.Add(this.sniperEnemyCreator.CreateSniperEnemy(xMillis: xMillis, difficulty: this.difficulty));
					}
				}
			}
		}
		
		private int? mediumEnemyWave2XMillis = null;
		private void TrySpawnMediumEnemyWave2(List<IEnemy> enemies)
		{
			if (!this.spawnedEnemies.Contains("TrySpawnMediumEnemyWave2"))
			{
				if (this.numMillisElapsed > 28500)
				{
					this.spawnedEnemies.Add("TrySpawnMediumEnemyWave2");

					this.mediumEnemyWave2XMillis = 100 * 1024;

					if (this.mediumEnemyWave1XMillis != null && this.mediumEnemyWave1XMillis.Value < 250 * 1024)
						this.mediumEnemyWave2XMillis = 400 * 1024;

					enemies.Add(this.mediumEnemyCreator.CreateMediumEnemy(
						xMillis: this.mediumEnemyWave2XMillis.Value,
						bulletsRotateClockwise: this.rng.NextBool(),
						shootDirectionScaled1: this.rng.NextInt(360 * 128),
						shootDirectionScaled2: this.rng.NextInt(360 * 128),
						difficulty: this.difficulty));
				}
			}
		}

		private List<int> sniperEnemyWave2XMillis = null;
		private void TrySpawnSniperEnemyWave2(List<IEnemy> enemies)
		{
			if (!this.spawnedEnemies.Contains("TrySpawnSniperEnemyWave2_2"))
			{
				for (int i = 0; i < 3; i++)
				{
					if (this.numMillisElapsed > 29 * 1000 + i * 2000 && !this.spawnedEnemies.Contains(StringConcatenation.Concat("TrySpawnSniperEnemyWave2_", i)))
					{
						this.spawnedEnemies.Add(StringConcatenation.Concat("TrySpawnSniperEnemyWave2_", i));

						if (this.sniperEnemyWave2XMillis == null)
							this.sniperEnemyWave2XMillis = new List<int>();

						bool onRightSide = this.mediumEnemyWave2XMillis != null && this.mediumEnemyWave2XMillis.Value < 250 * 1024;

						int xMillis = (this.rng.NextInt(200) + (onRightSide ? 250 : 50)) << 10;

						int count = 0;
						while (true)
						{
							count++;
							if (count >= 100)
								break;

							bool isValid = true;

							for (int x = 0; x < this.sniperEnemyWave2XMillis.Count; x++)
							{
								var otherXMillis = this.sniperEnemyWave2XMillis[x];
								if (Math.Abs(otherXMillis - xMillis) < 70 * 1024)
								{
									isValid = false;
									break;
								}
							}

							if (isValid)
								break;

							xMillis = (this.rng.NextInt(200) + (onRightSide ? 250 : 50)) << 10;
						}

						this.sniperEnemyWave2XMillis.Add(xMillis);

						enemies.Add(this.sniperEnemyCreator.CreateSniperEnemy(xMillis: xMillis, difficulty: this.difficulty));
					}
				}
			}
		}

		private int? mediumEnemyWave3XMillis = null;
		private void TrySpawnMediumEnemyWave3(List<IEnemy> enemies)
		{
			if (!this.spawnedEnemies.Contains("TrySpawnMediumEnemyWave3"))
			{
				if (this.numMillisElapsed > 43500)
				{
					this.spawnedEnemies.Add("TrySpawnMediumEnemyWave3");

					this.mediumEnemyWave3XMillis = 100 * 1024;

					if (this.mediumEnemyWave2XMillis != null && this.mediumEnemyWave2XMillis.Value < 250 * 1024)
						this.mediumEnemyWave3XMillis = 400 * 1024;

					enemies.Add(this.mediumEnemyCreator.CreateMediumEnemy(
						xMillis: this.mediumEnemyWave3XMillis.Value,
						bulletsRotateClockwise: this.rng.NextBool(),
						shootDirectionScaled1: this.rng.NextInt(360 * 128),
						shootDirectionScaled2: this.rng.NextInt(360 * 128),
						difficulty: this.difficulty));
				}
			}
		}

		private List<int> sniperEnemyWave3XMillis = null;
		private void TrySpawnSniperEnemyWave3(List<IEnemy> enemies)
		{
			if (!this.spawnedEnemies.Contains("TrySpawnSniperEnemyWave3_2"))
			{
				for (int i = 0; i < 3; i++)
				{
					if (this.numMillisElapsed > 44 * 1000 + i * 2000 && !this.spawnedEnemies.Contains(StringConcatenation.Concat("TrySpawnSniperEnemyWave3_", i)))
					{
						this.spawnedEnemies.Add(StringConcatenation.Concat("TrySpawnSniperEnemyWave3_", i));

						if (this.sniperEnemyWave3XMillis == null)
							this.sniperEnemyWave3XMillis = new List<int>();

						bool onRightSide = this.mediumEnemyWave3XMillis != null && this.mediumEnemyWave3XMillis.Value < 250 * 1024;

						int xMillis = (this.rng.NextInt(200) + (onRightSide ? 250 : 50)) << 10;

						int count = 0;
						while (true)
						{
							count++;
							if (count >= 100)
								break;

							bool isValid = true;

							for (int x = 0; x < this.sniperEnemyWave3XMillis.Count; x++)
							{
								var otherXMillis = this.sniperEnemyWave3XMillis[x];
								if (Math.Abs(otherXMillis - xMillis) < 70 * 1024)
								{
									isValid = false;
									break;
								}
							}

							if (isValid)
								break;

							xMillis = (this.rng.NextInt(200) + (onRightSide ? 250 : 50)) << 10;
						}

						this.sniperEnemyWave3XMillis.Add(xMillis);

						enemies.Add(this.sniperEnemyCreator.CreateSniperEnemy(xMillis: xMillis, difficulty: this.difficulty));
					}
				}
			}
		}

		private List<MediumEnemyCreator.MediumEnemy> mediumEnemyWave4Enemies = null;
		private void TrySpawnMediumEnemyWave4(List<IEnemy> enemies)
		{
			if (!this.spawnedEnemies.Contains("TrySpawnMediumEnemyWave4"))
			{
				if (this.numMillisElapsed > 58500)
				{
					this.spawnedEnemies.Add("TrySpawnMediumEnemyWave4");
					
					bool bulletRotation = this.rng.NextBool();

					int shootDirectionScaled1 = this.rng.NextInt(360 * 128);
					int shootDirectionScaled2 = this.rng.NextInt(360 * 128);
					
					MediumEnemyCreator.MediumEnemy enemy1 = this.mediumEnemyCreator.CreateMediumEnemy(
						xMillis: 100 * 1024,
						bulletsRotateClockwise: bulletRotation,
						shootDirectionScaled1: shootDirectionScaled1,
						shootDirectionScaled2: shootDirectionScaled2,
						difficulty: this.difficulty);
					MediumEnemyCreator.MediumEnemy enemy2 = this.mediumEnemyCreator.CreateMediumEnemy(
						xMillis: 400 * 1024,
						bulletsRotateClockwise: !bulletRotation,
						shootDirectionScaled1: -shootDirectionScaled1,
						shootDirectionScaled2: -shootDirectionScaled2,
						difficulty: this.difficulty);

					enemies.Add(enemy1);
					enemies.Add(enemy2);

					if (this.mediumEnemyWave4Enemies == null)
						this.mediumEnemyWave4Enemies = new List<MediumEnemyCreator.MediumEnemy>();

					this.mediumEnemyWave4Enemies.Add(enemy1);
					this.mediumEnemyWave4Enemies.Add(enemy2);
				}
			}
		}

		public bool HasPhase1Finished()
		{
			return this.hasPhase1Finished;
		}

		public EnemyFrameResult ProcessFrame(
			int elapsedMillisPerFrame,
			int playerXMillis,
			int playerYMillis)
		{
			this.numMillisElapsed += elapsedMillisPerFrame;

			List<IEnemy> enemies = new List<IEnemy>();
			
			this.TrySpawnBasicEnemyWave1(enemies: enemies);
			this.TrySpawnBasicEnemyWave2(enemies: enemies);
			this.TrySpawnMediumEnemyWave1(enemies: enemies);
			this.TrySpawnSniperEnemyWave1(enemies: enemies);
			this.TrySpawnMediumEnemyWave2(enemies: enemies);
			this.TrySpawnSniperEnemyWave2(enemies: enemies);
			this.TrySpawnMediumEnemyWave3(enemies: enemies);
			this.TrySpawnSniperEnemyWave3(enemies: enemies);
			this.TrySpawnMediumEnemyWave4(enemies: enemies);

			if (this.mediumEnemyWave4Enemies != null)
			{
				bool areAllWave4EnemiesDead = true;

				for (int i = 0; i < this.mediumEnemyWave4Enemies.Count; i++)
				{
					if (this.mediumEnemyWave4Enemies[i].IsDestroyedOrDespawned() == false)
						areAllWave4EnemiesDead = false;
				}

				if (areAllWave4EnemiesDead)
				{
					this.hasPhase1Finished = true;
				}
			}

			if (!this.hasPhase1Finished)
			{
				if (enemies.Count == 0)
					return this.enemyFrameResult;
				
				enemies.Add(this);
			}

			return new EnemyFrameResult(
				enemies: DTImmutableList<IEnemy>.AsImmutableList(enemies),
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
		}

		public ZIndex GetZIndex()
		{
			return ZIndex.Background;
		}

		public void Render(IDisplay<Danmaku2Assets> display)
		{
		}
	}
}

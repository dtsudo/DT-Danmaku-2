
namespace Danmaku2Lib
{
	using System;
	using System.Collections.Generic;

	public class Level1Phase2EnemyGeneratorEnemy : IEnemy
	{
		private Difficulty difficulty;
		private SniperEnemyCreator sniperEnemyCreator;
		private BasicEnemyCreator basicEnemyCreator;
		private MediumEnemyCreator mediumEnemyCreator;
		private CircleEnemyCreator circleEnemyCreator;

		private int elapsedMillisPerFrame;
		private int numMillisElapsed;

		private HashSet<string> spawnedEnemies;

		private IDTDeterministicRandom rng;

		private bool hasPhase2Finished;
		
		private EnemyFrameResult enemyFrameResult;

		public Level1Phase2EnemyGeneratorEnemy(
			Difficulty difficulty,
			int elapsedMillisPerFrame,
			IDTDeterministicRandom rng)
		{
			this.difficulty = difficulty;
			this.sniperEnemyCreator = new SniperEnemyCreator(rng: rng);
			this.basicEnemyCreator = new BasicEnemyCreator(rng: rng, elapsedMillisPerFrame: elapsedMillisPerFrame);
			this.mediumEnemyCreator = new MediumEnemyCreator(rng: rng);
			this.circleEnemyCreator = new CircleEnemyCreator(rng: rng, elapsedMillisPerFrame: elapsedMillisPerFrame);

			this.elapsedMillisPerFrame = elapsedMillisPerFrame;

			this.rng = rng;

			this.numMillisElapsed = 0;

			this.spawnedEnemies = new HashSet<string>();

			this.hasPhase2Finished = false;
			
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

		private int? circleEnemyWave1YMillis = null;
		private bool? circleEnemyWave1IsComingFromLeftSide = null;
		private void TrySpawnCircleEnemyWave1(List<IEnemy> enemies)
		{
			if (!this.spawnedEnemies.Contains("TrySpawnCircleEnemyWave1_4"))
			{
				for (int i = 0; i < 5; i++)
				{
					if (this.numMillisElapsed > i * 2000 && !this.spawnedEnemies.Contains(StringConcatenation.Concat("TrySpawnCircleEnemyWave1_", i)))
					{
						if (this.circleEnemyWave1YMillis == null)
							this.circleEnemyWave1YMillis = (160 + this.rng.NextInt(300)) << 10;

						if (this.circleEnemyWave1IsComingFromLeftSide == null)
							this.circleEnemyWave1IsComingFromLeftSide = this.rng.NextBool();

						this.spawnedEnemies.Add(StringConcatenation.Concat("TrySpawnCircleEnemyWave1_", i));

						enemies.Add(this.circleEnemyCreator.CreateCircleEnemy(
							yMillis: this.circleEnemyWave1YMillis.Value,
							isComingFromLeftSide: this.circleEnemyWave1IsComingFromLeftSide.Value,
							difficulty: this.difficulty));
					}
				}
			}
		}

		private int? circleEnemyWave2YMillis = null;
		private bool? circleEnemyWave2IsComingFromLeftSide = null;
		private void TrySpawnCircleEnemyWave2(List<IEnemy> enemies)
		{
			if (!this.spawnedEnemies.Contains("TrySpawnCircleEnemyWave2_4"))
			{
				for (int i = 0; i < 5; i++)
				{
					if (this.numMillisElapsed > 1000 + i * 2000 && !this.spawnedEnemies.Contains(StringConcatenation.Concat("TrySpawnCircleEnemyWave2_", i)))
					{
						this.spawnedEnemies.Add(StringConcatenation.Concat("TrySpawnCircleEnemyWave2_", i));

						if (this.circleEnemyWave2YMillis == null)
						{
							this.circleEnemyWave2YMillis = (160 + this.rng.NextInt(300)) << 10;

							int count = 0;
							while (count < 50 && this.circleEnemyWave1YMillis != null && Math.Abs(this.circleEnemyWave1YMillis.Value - this.circleEnemyWave2YMillis.Value) < 95 * 1024)
							{
								this.circleEnemyWave2YMillis = (160 + this.rng.NextInt(300)) << 10;
								count++;
							}
						}

						if (this.circleEnemyWave2IsComingFromLeftSide == null)
							this.circleEnemyWave2IsComingFromLeftSide = this.circleEnemyWave1IsComingFromLeftSide.HasValue && !this.circleEnemyWave1IsComingFromLeftSide.Value;

						enemies.Add(this.circleEnemyCreator.CreateCircleEnemy(
							yMillis: this.circleEnemyWave2YMillis.Value,
							isComingFromLeftSide: this.circleEnemyWave2IsComingFromLeftSide.Value,
							difficulty: this.difficulty));
					}
				}
			}
		}

		private List<int> sniperEnemyWave1XMillis = null;
		private List<SniperEnemyCreator.SniperEnemy> sniperEnemiesWave1 = null;
		private void TrySpawnSniperEnemyWave1(List<IEnemy> enemies)
		{
			if (!this.spawnedEnemies.Contains("TrySpawnSniperEnemyWave1_10"))
			{
				for (int i = 0; i < 11; i++)
				{
					if (this.numMillisElapsed > 3000 + i * 3000 && !this.spawnedEnemies.Contains(StringConcatenation.Concat("TrySpawnSniperEnemyWave1_", i)))
					{
						this.spawnedEnemies.Add(StringConcatenation.Concat("TrySpawnSniperEnemyWave1_", i));

						if (this.sniperEnemyWave1XMillis == null)
							this.sniperEnemyWave1XMillis = new List<int>();
						
						int xMillis = (this.rng.NextInt(400) + 50) << 10;

						int count = 0;
						while (true)
						{
							count++;
							if (count >= 100)
								break;

							bool isValid = true;

							for (int x = Math.Max(this.sniperEnemyWave1XMillis.Count - 3, 0); x < this.sniperEnemyWave1XMillis.Count; x++)
							{
								var otherXMillis = this.sniperEnemyWave1XMillis[x];
								if (Math.Abs(otherXMillis - xMillis) < 90 * 1024)
								{
									isValid = false;
									break;
								}
							}

							if (isValid)
								break;

							xMillis = (this.rng.NextInt(400) + 50) << 10;
						}

						this.sniperEnemyWave1XMillis.Add(xMillis);

						if (this.sniperEnemiesWave1 == null)
							this.sniperEnemiesWave1 = new List<SniperEnemyCreator.SniperEnemy>();

						SniperEnemyCreator.SniperEnemy sniperEnemy = this.sniperEnemyCreator.CreateSniperEnemy(xMillis: xMillis, difficulty: this.difficulty);

						this.sniperEnemiesWave1.Add(sniperEnemy);
						enemies.Add(sniperEnemy);
					}
				}
			}
		}
		
		private int? circleEnemyWave3YMillis = null;
		private bool? circleEnemyWave3IsComingFromLeftSide = null;
		private void TrySpawnCircleEnemyWave3(List<IEnemy> enemies)
		{
			if (!this.spawnedEnemies.Contains("TrySpawnCircleEnemyWave3_4"))
			{
				for (int i = 0; i < 5; i++)
				{
					if (this.numMillisElapsed > 15000 + i * 2000 && !this.spawnedEnemies.Contains(StringConcatenation.Concat("TrySpawnCircleEnemyWave3_", i)))
					{
						this.spawnedEnemies.Add(StringConcatenation.Concat("TrySpawnCircleEnemyWave3_", i));

						if (this.circleEnemyWave3YMillis == null)
						{
							this.circleEnemyWave3YMillis = (160 + this.rng.NextInt(300)) << 10;

							int count = 0;
							while (count < 50 && this.circleEnemyWave1YMillis != null && this.circleEnemyWave2YMillis != null
								&&
								(Math.Abs(this.circleEnemyWave2YMillis.Value - this.circleEnemyWave3YMillis.Value) < 95 * 1024
									||
								count < 25 && Math.Abs(this.circleEnemyWave1YMillis.Value - this.circleEnemyWave3YMillis.Value) < 95 * 1024))
							{
								this.circleEnemyWave3YMillis = (160 + this.rng.NextInt(300)) << 10;
								count++;
							}
						}

						if (this.circleEnemyWave3IsComingFromLeftSide == null)
							this.circleEnemyWave3IsComingFromLeftSide = this.circleEnemyWave2IsComingFromLeftSide.HasValue && !this.circleEnemyWave2IsComingFromLeftSide.Value;

						enemies.Add(this.circleEnemyCreator.CreateCircleEnemy(
							yMillis: this.circleEnemyWave3YMillis.Value,
							isComingFromLeftSide: this.circleEnemyWave3IsComingFromLeftSide.Value,
							difficulty: this.difficulty));
					}
				}
			}
		}
		
		private int? circleEnemyWave4YMillis = null;
		private bool? circleEnemyWave4IsComingFromLeftSide = null;
		private void TrySpawnCircleEnemyWave4(List<IEnemy> enemies)
		{
			if (!this.spawnedEnemies.Contains("TrySpawnCircleEnemyWave4_4"))
			{
				for (int i = 0; i < 5; i++)
				{
					if (this.numMillisElapsed > 16000 + i * 2000 && !this.spawnedEnemies.Contains(StringConcatenation.Concat("TrySpawnCircleEnemyWave4_", i)))
					{
						this.spawnedEnemies.Add(StringConcatenation.Concat("TrySpawnCircleEnemyWave4_", i));

						if (this.circleEnemyWave4YMillis == null)
						{
							this.circleEnemyWave4YMillis = (160 + this.rng.NextInt(300)) << 10;

							int count = 0;
							while (count < 50 && this.circleEnemyWave2YMillis != null && this.circleEnemyWave3YMillis != null
								&&
								(Math.Abs(this.circleEnemyWave3YMillis.Value - this.circleEnemyWave4YMillis.Value) < 95 * 1024
									||
								count < 25 && Math.Abs(this.circleEnemyWave2YMillis.Value - this.circleEnemyWave4YMillis.Value) < 95 * 1024))
							{
								this.circleEnemyWave4YMillis = (160 + this.rng.NextInt(300)) << 10;
								count++;
							}
						}

						if (this.circleEnemyWave4IsComingFromLeftSide == null)
							this.circleEnemyWave4IsComingFromLeftSide = this.circleEnemyWave3IsComingFromLeftSide.HasValue && !this.circleEnemyWave3IsComingFromLeftSide.Value;

						enemies.Add(this.circleEnemyCreator.CreateCircleEnemy(
							yMillis: this.circleEnemyWave4YMillis.Value,
							isComingFromLeftSide: this.circleEnemyWave4IsComingFromLeftSide.Value,
							difficulty: this.difficulty));
					}
				}
			}
		}

		private int? circleEnemyWave5YMillisA = null;
		private int? circleEnemyWave5YMillisB = null;
		private bool? circleEnemyWave5IsComingFromLeftSide = null;
		private List<CircleEnemyCreator.CircleEnemy> circleEnemiesWave5 = null;
		private void TrySpawnCircleEnemyWave5(List<IEnemy> enemies)
		{
			if (!this.spawnedEnemies.Contains("TrySpawnCircleEnemyWave5_4"))
			{
				for (int i = 0; i < 5; i++)
				{
					if (this.numMillisElapsed > 30000 + i * 2000 && !this.spawnedEnemies.Contains(StringConcatenation.Concat("TrySpawnCircleEnemyWave5_", i)))
					{
						this.spawnedEnemies.Add(StringConcatenation.Concat("TrySpawnCircleEnemyWave5_", i));

						if (this.circleEnemyWave5YMillisA == null)
						{
							this.circleEnemyWave5YMillisA = (160 + this.rng.NextInt(300)) << 10;

							int count = 0;
							while (count < 50 && this.circleEnemyWave4YMillis != null && Math.Abs(this.circleEnemyWave4YMillis.Value - this.circleEnemyWave5YMillisA.Value) < 95 * 1024)
							{
								this.circleEnemyWave5YMillisA = (160 + this.rng.NextInt(300)) << 10;
								count++;
							}

							this.circleEnemyWave5YMillisB = (160 + this.rng.NextInt(300)) << 10;

							count = 0;
							while (count < 50 && this.circleEnemyWave4YMillis != null
								&&
								(count < 25 && Math.Abs(this.circleEnemyWave4YMillis.Value - this.circleEnemyWave5YMillisB.Value) < 50 * 1024
									||
								Math.Abs(this.circleEnemyWave5YMillisA.Value - this.circleEnemyWave5YMillisB.Value) < 50 * 1024))
							{
								this.circleEnemyWave5YMillisB = (160 + this.rng.NextInt(300)) << 10;
								count++;
							}

						}

						if (this.circleEnemyWave5IsComingFromLeftSide == null)
							this.circleEnemyWave5IsComingFromLeftSide = this.circleEnemyWave4IsComingFromLeftSide.HasValue && !this.circleEnemyWave4IsComingFromLeftSide.Value;

						if (this.circleEnemiesWave5 == null)
							this.circleEnemiesWave5 = new List<CircleEnemyCreator.CircleEnemy>();

						CircleEnemyCreator.CircleEnemy circleEnemyA = this.circleEnemyCreator.CreateCircleEnemy(
							yMillis: this.circleEnemyWave5YMillisA.Value,
							isComingFromLeftSide: this.circleEnemyWave5IsComingFromLeftSide.Value,
							difficulty: this.difficulty);
						CircleEnemyCreator.CircleEnemy circleEnemyB = this.circleEnemyCreator.CreateCircleEnemy(
							yMillis: this.circleEnemyWave5YMillisB.Value,
							isComingFromLeftSide: this.circleEnemyWave5IsComingFromLeftSide.Value,
							difficulty: this.difficulty);

						this.circleEnemiesWave5.Add(circleEnemyA);
						this.circleEnemiesWave5.Add(circleEnemyB);
						enemies.Add(circleEnemyA);
						enemies.Add(circleEnemyB);
					}
				}
			}
		}
		
		private int? circleEnemyWave6YMillisA = null;
		private int? circleEnemyWave6YMillisB = null;
		private bool? circleEnemyWave6IsComingFromLeftSide = null;
		private List<CircleEnemyCreator.CircleEnemy> circleEnemiesWave6 = null;
		private bool hasSpawnedLastEnemy = false;
		private void TrySpawnCircleEnemyWave6(List<IEnemy> enemies)
		{
			if (!this.spawnedEnemies.Contains("TrySpawnCircleEnemyWave6_4"))
			{
				for (int i = 0; i < 5; i++)
				{
					if (this.numMillisElapsed > 31000 + i * 2000 && !this.spawnedEnemies.Contains(StringConcatenation.Concat("TrySpawnCircleEnemyWave6_", i)))
					{
						this.spawnedEnemies.Add(StringConcatenation.Concat("TrySpawnCircleEnemyWave6_", i));

						if (i == 4)
							this.hasSpawnedLastEnemy = true;

						if (this.circleEnemyWave6YMillisA == null)
						{
							this.circleEnemyWave6YMillisA = (160 + this.rng.NextInt(300)) << 10;

							int count = 0;
							while (count < 50 && this.circleEnemyWave5YMillisA != null && this.circleEnemyWave5YMillisB != null
								&&
								(Math.Abs(this.circleEnemyWave5YMillisA.Value - this.circleEnemyWave6YMillisA.Value) < 50 * 1024
									||
								Math.Abs(this.circleEnemyWave5YMillisB.Value - this.circleEnemyWave6YMillisA.Value) < 50 * 1024))
							{
								this.circleEnemyWave6YMillisA = (160 + this.rng.NextInt(300)) << 10;
								count++;
							}

							this.circleEnemyWave6YMillisB = (160 + this.rng.NextInt(300)) << 10;

							count = 0;
							while (count < 50 && this.circleEnemyWave5YMillisA != null && this.circleEnemyWave5YMillisB != null
								&&
								(Math.Abs(this.circleEnemyWave5YMillisA.Value - this.circleEnemyWave6YMillisB.Value) < 45 * 1024
									||
								Math.Abs(this.circleEnemyWave5YMillisB.Value - this.circleEnemyWave6YMillisB.Value) < 45 * 1024
									||
								Math.Abs(this.circleEnemyWave6YMillisA.Value - this.circleEnemyWave6YMillisB.Value) < 45 * 1024))
							{
								this.circleEnemyWave6YMillisB = (160 + this.rng.NextInt(300)) << 10;
								count++;
							}

						}

						if (this.circleEnemyWave6IsComingFromLeftSide == null)
							this.circleEnemyWave6IsComingFromLeftSide = this.circleEnemyWave5IsComingFromLeftSide.HasValue && !this.circleEnemyWave5IsComingFromLeftSide.Value;

						if (this.circleEnemiesWave6 == null)
							this.circleEnemiesWave6 = new List<CircleEnemyCreator.CircleEnemy>();

						CircleEnemyCreator.CircleEnemy circleEnemyA = this.circleEnemyCreator.CreateCircleEnemy(
							yMillis: this.circleEnemyWave6YMillisA.Value,
							isComingFromLeftSide: this.circleEnemyWave6IsComingFromLeftSide.Value,
							difficulty: this.difficulty);
						CircleEnemyCreator.CircleEnemy circleEnemyB = this.circleEnemyCreator.CreateCircleEnemy(
							yMillis: this.circleEnemyWave6YMillisB.Value,
							isComingFromLeftSide: this.circleEnemyWave6IsComingFromLeftSide.Value,
							difficulty: this.difficulty);

						this.circleEnemiesWave6.Add(circleEnemyA);
						this.circleEnemiesWave6.Add(circleEnemyB);
						enemies.Add(circleEnemyA);
						enemies.Add(circleEnemyB);
					}
				}
			}
		}
		
		public bool HasPhase2Finished()
		{
			return this.hasPhase2Finished;
		}

		public EnemyFrameResult ProcessFrame(
			int elapsedMillisPerFrame,
			int playerXMillis,
			int playerYMillis)
		{
			this.numMillisElapsed += elapsedMillisPerFrame;

			List<IEnemy> enemies = new List<IEnemy>();
			
			this.TrySpawnCircleEnemyWave1(enemies: enemies);
			this.TrySpawnCircleEnemyWave2(enemies: enemies);
			this.TrySpawnSniperEnemyWave1(enemies: enemies);
			this.TrySpawnCircleEnemyWave3(enemies: enemies);
			this.TrySpawnCircleEnemyWave4(enemies: enemies);
			this.TrySpawnCircleEnemyWave5(enemies: enemies);
			this.TrySpawnCircleEnemyWave6(enemies: enemies);

			if (this.hasSpawnedLastEnemy && this.sniperEnemiesWave1 != null && this.circleEnemiesWave5 != null && this.circleEnemiesWave6 != null)
			{
				bool areAllDead = true;

				for (int i = 0; i < this.sniperEnemiesWave1.Count; i++)
				{
					if (this.sniperEnemiesWave1[i].IsDestroyedOrDespawned() == false)
						areAllDead = false;
				}
				for (int i = 0; i < this.circleEnemiesWave5.Count; i++)
				{
					if (this.circleEnemiesWave5[i].IsDestroyedOrDespawned() == false)
						areAllDead = false;
				}
				for (int i = 0; i < this.circleEnemiesWave6.Count; i++)
				{
					if (this.circleEnemiesWave6[i].IsDestroyedOrDespawned() == false)
						areAllDead = false;
				}

				if (areAllDead)
				{
					this.hasPhase2Finished = true;
				}
			}

			if (!this.hasPhase2Finished)
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

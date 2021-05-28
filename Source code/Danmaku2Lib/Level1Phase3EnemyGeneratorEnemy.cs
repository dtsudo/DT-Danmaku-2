
namespace Danmaku2Lib
{
	using System;
	using System.Collections.Generic;

	public class Level1Phase3EnemyGeneratorEnemy : IEnemy
	{
		private Difficulty difficulty;
		private SniperEnemyCreator sniperEnemyCreator;
		private BasicEnemyCreator basicEnemyCreator;
		private MediumEnemyCreator mediumEnemyCreator;
		private MediumDescendingEnemyCreator mediumDescendingEnemyCreator;
		private CircleEnemyCreator circleEnemyCreator;

		private int elapsedMillisPerFrame;
		private int numMillisElapsed;

		private HashSet<string> spawnedEnemies;

		private IDTDeterministicRandom rng;

		private bool hasPhase3Finished;
		
		private EnemyFrameResult enemyFrameResult;

		public Level1Phase3EnemyGeneratorEnemy(
			Difficulty difficulty,
			int elapsedMillisPerFrame,
			IDTDeterministicRandom rng)
		{
			this.difficulty = difficulty;
			this.sniperEnemyCreator = new SniperEnemyCreator(rng: rng);
			this.basicEnemyCreator = new BasicEnemyCreator(rng: rng, elapsedMillisPerFrame: elapsedMillisPerFrame);
			this.mediumEnemyCreator = new MediumEnemyCreator(rng: rng);
			this.mediumDescendingEnemyCreator = new MediumDescendingEnemyCreator(rng: rng, elapsedMillisPerFrame: elapsedMillisPerFrame);
			this.circleEnemyCreator = new CircleEnemyCreator(rng: rng, elapsedMillisPerFrame: elapsedMillisPerFrame);

			this.elapsedMillisPerFrame = elapsedMillisPerFrame;

			this.rng = rng;

			this.numMillisElapsed = 0;

			this.spawnedEnemies = new HashSet<string>();

			this.hasPhase3Finished = false;
			
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

		private List<BasicEnemyCreator.BasicEnemy> basicEnemies = null;
		private void TrySpawnBasicEnemy(
			int xMillis,
			int numMillisElapsed,
			string enemyName,
			List<IEnemy> enemies)
		{
			if (!this.spawnedEnemies.Contains(enemyName))
			{
				if (this.numMillisElapsed > numMillisElapsed)
				{
					this.spawnedEnemies.Add(enemyName);

					BasicEnemyCreator.BasicEnemy enemy = this.basicEnemyCreator.CreateBasicEnemy(
						xMillis: xMillis,
						difficulty: this.difficulty);

					enemies.Add(enemy);

					if (this.basicEnemies == null)
						this.basicEnemies = new List<BasicEnemyCreator.BasicEnemy>();
					this.basicEnemies.Add(enemy);
				}
			}
		}

		private List<MediumDescendingEnemyCreator.MediumDescendingEnemy> mediumDescendingEnemies = null;
		private void TrySpawnMediumDescendingEnemy(
			int xMillis,
			int numMillisElapsed,
			string enemyName,
			List<IEnemy> enemies,
			bool bulletsRotateClockwise,
			int shootDirectionScaled1,
			int shootDirectionScaled2)
		{
			if (!this.spawnedEnemies.Contains(enemyName))
			{
				if (this.numMillisElapsed > numMillisElapsed)
				{
					this.spawnedEnemies.Add(enemyName);

					MediumDescendingEnemyCreator.MediumDescendingEnemy enemy = this.mediumDescendingEnemyCreator.CreateMediumDescendingEnemy(
						xMillis: xMillis,
						bulletsRotateClockwise: bulletsRotateClockwise,
						shootDirectionScaled1: shootDirectionScaled1,
						shootDirectionScaled2: shootDirectionScaled2,
						difficulty: this.difficulty);

					enemies.Add(enemy);

					if (this.mediumDescendingEnemies == null)
						this.mediumDescendingEnemies = new List<MediumDescendingEnemyCreator.MediumDescendingEnemy>();
					this.mediumDescendingEnemies.Add(enemy);
				}
			}
		}

		public bool HasPhase3Finished()
		{
			return this.hasPhase3Finished;
		}

		public EnemyFrameResult ProcessFrame(
			int elapsedMillisPerFrame,
			int playerXMillis,
			int playerYMillis)
		{
			this.numMillisElapsed += elapsedMillisPerFrame;

			List<IEnemy> enemies = new List<IEnemy>();

			bool rotateClockwise = this.rng.NextBool();
			int shootDirectionScaled1 = this.rng.NextInt(360 * 128);
			int shootDirectionScaled2 = this.rng.NextInt(360 * 128);

			this.TrySpawnBasicEnemy(           xMillis: 150 * 1024, numMillisElapsed:  2000, enemyName:  "1", enemies: enemies);
			this.TrySpawnBasicEnemy(           xMillis: 350 * 1024, numMillisElapsed:  2000, enemyName:  "2", enemies: enemies);
			this.TrySpawnBasicEnemy(           xMillis:  50 * 1024, numMillisElapsed:  7000, enemyName:  "3", enemies: enemies);
			this.TrySpawnBasicEnemy(           xMillis: 250 * 1024, numMillisElapsed:  7000, enemyName:  "4", enemies: enemies);
			this.TrySpawnBasicEnemy(           xMillis: 450 * 1024, numMillisElapsed:  7000, enemyName:  "5", enemies: enemies);
			this.TrySpawnBasicEnemy(           xMillis: 150 * 1024, numMillisElapsed: 12000, enemyName:  "6", enemies: enemies);
			this.TrySpawnBasicEnemy(           xMillis: 350 * 1024, numMillisElapsed: 12000, enemyName:  "7", enemies: enemies);
			this.TrySpawnBasicEnemy(           xMillis:  50 * 1024, numMillisElapsed: 17000, enemyName:  "8", enemies: enemies);
			this.TrySpawnMediumDescendingEnemy(xMillis: 250 * 1024, numMillisElapsed: 17000, enemyName:  "9", enemies: enemies,
				bulletsRotateClockwise: this.rng.NextBool(), shootDirectionScaled1: this.rng.NextInt(360 * 128), shootDirectionScaled2: this.rng.NextInt(360 * 128));
			this.TrySpawnBasicEnemy(           xMillis: 450 * 1024, numMillisElapsed: 17000, enemyName: "10", enemies: enemies);
			this.TrySpawnBasicEnemy(           xMillis: 150 * 1024, numMillisElapsed: 22000, enemyName: "11", enemies: enemies);
			this.TrySpawnBasicEnemy(           xMillis: 350 * 1024, numMillisElapsed: 22000, enemyName: "12", enemies: enemies);
			this.TrySpawnBasicEnemy(           xMillis:  50 * 1024, numMillisElapsed: 27000, enemyName: "13", enemies: enemies);
			this.TrySpawnBasicEnemy(           xMillis: 250 * 1024, numMillisElapsed: 27000, enemyName: "14", enemies: enemies);
			this.TrySpawnBasicEnemy(           xMillis: 450 * 1024, numMillisElapsed: 27000, enemyName: "15", enemies: enemies);
			this.TrySpawnMediumDescendingEnemy(xMillis: 150 * 1024, numMillisElapsed: 32000, enemyName: "16", enemies: enemies,
				bulletsRotateClockwise: rotateClockwise, shootDirectionScaled1: shootDirectionScaled1, shootDirectionScaled2: shootDirectionScaled2);
			this.TrySpawnMediumDescendingEnemy(xMillis: 350 * 1024, numMillisElapsed: 32000, enemyName: "17", enemies: enemies,
				bulletsRotateClockwise: !rotateClockwise, shootDirectionScaled1: -shootDirectionScaled1, shootDirectionScaled2: -shootDirectionScaled2);
			this.TrySpawnBasicEnemy(           xMillis:  50 * 1024, numMillisElapsed: 37000, enemyName: "18", enemies: enemies);
			this.TrySpawnBasicEnemy(           xMillis: 250 * 1024, numMillisElapsed: 37000, enemyName: "19", enemies: enemies);
			this.TrySpawnBasicEnemy(           xMillis: 450 * 1024, numMillisElapsed: 37000, enemyName: "20", enemies: enemies);

			bool hasSpawnedLastEnemy = this.numMillisElapsed > 38000;

			if (hasSpawnedLastEnemy && this.basicEnemies != null && this.mediumDescendingEnemies != null)
			{
				bool areAllDead = true;

				for (int i = 0; i < this.basicEnemies.Count; i++)
				{
					if (this.basicEnemies[i].IsDestroyedOrDespawned() == false)
						areAllDead = false;
				}
				for (int i = 0; i < this.mediumDescendingEnemies.Count; i++)
				{
					if (this.mediumDescendingEnemies[i].IsDestroyedOrDespawned() == false)
						areAllDead = false;
				}

				if (areAllDead)
				{
					this.hasPhase3Finished = true;
				}
			}
			
			if (!this.hasPhase3Finished)
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


namespace Danmaku2Lib
{
	using System;
	using System.Collections.Generic;

	public class Level1EnemyGeneratorEnemy : IEnemy
	{
		private Level1Phase1EnemyGeneratorEnemy level1Phase1EnemyGeneratorEnemy;
		private bool hasSpawnedPhase1EnemyGeneratorEnemy;

		private Level1MidbossEnemy midbossEnemy;
		private int midbossSpawnTimerInMillis;
		private bool hasSpawnedMidboss;
		
		private Level1Phase2EnemyGeneratorEnemy level1Phase2EnemyGeneratorEnemy;
		private int phase2SpawnTimerInMillis;
		private bool hasSpawnedPhase2EnemyGeneratorEnemy;
		
		private Level1Phase3EnemyGeneratorEnemy level1Phase3EnemyGeneratorEnemy;
		private int phase3SpawnTimerInMillis;
		private bool hasSpawnedPhase3EnemyGeneratorEnemy;

		private int bossSpawnTimerInMillis;
		private bool hasSpawnedBoss;
		private Level1BossPhase1Enemy level1BossPhase1Enemy;

		private IDTDeterministicRandom rng;
		private Difficulty difficulty;

		private EnemyFrameResult enemyFrameResult;

		public Level1EnemyGeneratorEnemy(
			Difficulty difficulty,
			int elapsedMillisPerFrame,
			IDTDeterministicRandom rng)
		{
			this.level1Phase1EnemyGeneratorEnemy = new Level1Phase1EnemyGeneratorEnemy(
				difficulty: difficulty,
				elapsedMillisPerFrame: elapsedMillisPerFrame,
				rng: rng);

			this.rng = rng;
			this.difficulty = difficulty;

			this.hasSpawnedPhase1EnemyGeneratorEnemy = false;

			this.midbossEnemy = null;
			this.midbossSpawnTimerInMillis = 0;
			this.hasSpawnedMidboss = false;
			
			this.level1Phase2EnemyGeneratorEnemy = null;
			this.phase2SpawnTimerInMillis = 0;
			this.hasSpawnedPhase2EnemyGeneratorEnemy = false;
			
			this.level1Phase3EnemyGeneratorEnemy = null;
			this.phase3SpawnTimerInMillis = 0;
			this.hasSpawnedPhase3EnemyGeneratorEnemy = false;

			this.level1BossPhase1Enemy = null;
			this.bossSpawnTimerInMillis = 0;
			this.hasSpawnedBoss = false;

			this.enemyFrameResult = new EnemyFrameResult(
				enemies: DTImmutableList<IEnemy>.AsImmutableList(new List<IEnemy>() { this }),
				sounds: null,
				bossHealthMeterNumber: null,
				bossHealthMeterMilliPercentage: null,
				shouldEndLevel: false);
		}

		public List<ObjectBox> GetCollisionBoxes()
		{
			return null;
		}

		public List<ObjectBox> GetDamageBoxes()
		{
			return null;
		}

		public int GetXMillis()
		{
			return 0;
		}

		public int GetYMillis()
		{
			return 0;
		}

		public ZIndex GetZIndex()
		{
			return ZIndex.Background;
		}

		public void HandleCollisionWithPlayer()
		{
		}

		public void HandleCollisionWithPlayerBullet(PlayerBulletStrength bulletStrength)
		{
		}

		public EnemyFrameResult ProcessFrame(int elapsedMillisPerFrame, int playerXMillis, int playerYMillis)
		{
			List<IEnemy> enemies = new List<IEnemy>();

			if (!this.hasSpawnedPhase1EnemyGeneratorEnemy)
			{
				this.hasSpawnedPhase1EnemyGeneratorEnemy = true;
				enemies.Add(this.level1Phase1EnemyGeneratorEnemy);
			}

			if (!this.hasSpawnedMidboss && this.level1Phase1EnemyGeneratorEnemy.HasPhase1Finished())
			{
				this.midbossSpawnTimerInMillis += elapsedMillisPerFrame;

				if (this.midbossSpawnTimerInMillis >= 5000)
				{
					this.hasSpawnedMidboss = true;
					this.midbossEnemy = new Level1MidbossEnemy(
						rng: this.rng,
						difficulty: this.difficulty,
						elapsedMillisPerFrame: elapsedMillisPerFrame);
					enemies.Add(this.midbossEnemy);
				}
			}

			if (!this.hasSpawnedPhase2EnemyGeneratorEnemy)
			{
				if (this.hasSpawnedMidboss && this.midbossEnemy.HasDespawned())
				{
					this.phase2SpawnTimerInMillis += elapsedMillisPerFrame;

					if (this.phase2SpawnTimerInMillis >= 10)
					{
						this.hasSpawnedPhase2EnemyGeneratorEnemy = true;
						this.level1Phase2EnemyGeneratorEnemy = new Level1Phase2EnemyGeneratorEnemy(
							difficulty: this.difficulty,
							elapsedMillisPerFrame: elapsedMillisPerFrame,
							rng: this.rng);
						enemies.Add(this.level1Phase2EnemyGeneratorEnemy);
					}
				}
			}

			if (!this.hasSpawnedPhase3EnemyGeneratorEnemy)
			{
				if (this.hasSpawnedPhase2EnemyGeneratorEnemy && this.level1Phase2EnemyGeneratorEnemy.HasPhase2Finished())
				{
					this.phase3SpawnTimerInMillis += elapsedMillisPerFrame;

					if (this.phase3SpawnTimerInMillis >= 1000)
					{
						this.hasSpawnedPhase3EnemyGeneratorEnemy = true;
						this.level1Phase3EnemyGeneratorEnemy = new Level1Phase3EnemyGeneratorEnemy(
							difficulty: this.difficulty,
							elapsedMillisPerFrame: elapsedMillisPerFrame,
							rng: this.rng);
						enemies.Add(this.level1Phase3EnemyGeneratorEnemy);
					}
				}
			}

			if (!this.hasSpawnedBoss && this.hasSpawnedPhase3EnemyGeneratorEnemy && this.level1Phase3EnemyGeneratorEnemy.HasPhase3Finished())
			{
				this.bossSpawnTimerInMillis += elapsedMillisPerFrame;

				if (this.bossSpawnTimerInMillis >= 5000)
				{
					this.hasSpawnedBoss = true;
					this.level1BossPhase1Enemy = new Level1BossPhase1Enemy(
						rng: this.rng,
						difficulty: this.difficulty);
					enemies.Add(this.level1BossPhase1Enemy);
				}
			}

			if (enemies.Count == 0 && !this.hasSpawnedBoss)
				return this.enemyFrameResult;

			if (!this.hasSpawnedBoss)
				enemies.Add(this);

			return new EnemyFrameResult(
				enemies: DTImmutableList<IEnemy>.AsImmutableList(enemies),
				sounds: null,
				bossHealthMeterNumber: null,
				bossHealthMeterMilliPercentage: null,
				shouldEndLevel: false);
		}

		public void Render(IDisplay<Danmaku2Assets> display)
		{
		}
	}
}

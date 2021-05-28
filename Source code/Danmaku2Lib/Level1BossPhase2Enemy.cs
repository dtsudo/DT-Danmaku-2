
namespace Danmaku2Lib
{
	using System;
	using System.Collections.Generic;

	public class Level1BossPhase2Enemy : IEnemy
	{
		private enum Status
		{
			InCombat,
			Leaving,
			Despawned
		}

		private int xMillis;
		private int yMillis;

		private List<ObjectBox> collisionBoxes;
		private List<ObjectBox> damageBoxes;

		private StationaryEnemyMovement stationaryEnemyMovement;

		private Status status;

		private const int INITIAL_MILLI_HP = 40000 / 25;
		private int milliHp;

		private int timerCountdownInMillis;

		private bool hasSpawnedOrbiters;

		private Difficulty difficulty;

		private IDTDeterministicRandom rng;

		private Level1BossEnemyShootPattern level1BossEnemyShootPattern;

		public Level1BossPhase2Enemy(
			StationaryEnemyMovement stationaryEnemyMovement,
			int xMillis,
			int yMillis,
			IDTDeterministicRandom rng,
			Difficulty difficulty,
			int elapsedMillisPerFrame)
		{
			this.xMillis = xMillis;
			this.yMillis = yMillis;

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
			
			this.difficulty = difficulty;

			this.rng = rng;

			this.stationaryEnemyMovement = stationaryEnemyMovement;

			this.status = Status.InCombat;

			this.milliHp = INITIAL_MILLI_HP;

			this.timerCountdownInMillis = 80 * 1000;

			this.hasSpawnedOrbiters = false;

			this.level1BossEnemyShootPattern = new Level1BossEnemyShootPattern(
				enemy: this,
				difficulty: difficulty,
				rng: rng,
				phase: Level1BossEnemyShootPattern.Phase.Phase2);
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
		
		public EnemyFrameResult ProcessFrame(int elapsedMillisPerFrame, int playerXMillis, int playerYMillis)
		{
			List<IEnemy> enemies = new List<IEnemy>();
			List<Danmaku2Sound> sounds = null;

			if (!this.hasSpawnedOrbiters)
			{
				this.hasSpawnedOrbiters = true;

				bool shouldRotateClockwise = this.rng.NextBool();

				int initialDegreesScaled = this.rng.NextInt(360 * 128);

				List<int> orbiterNum = new List<int>() { 1, 2, 3, 4, 5 };

				var orbiterCreator = new Level1BossPhase2OrbiterEnemyCreator(rng: this.rng, elapsedMillisPerFrame: elapsedMillisPerFrame);
				for (int i = 0; i < 5; i++)
				{
					int random = this.rng.NextInt(orbiterNum.Count);
					int orbiterEnemyNumber = orbiterNum[random];
					orbiterNum.RemoveAt(random);

					enemies.Add(orbiterCreator.CreateBossPhase2OrbiterEnemy(
						bossEnemy: this,
						initialDegreesScaled: initialDegreesScaled + (360 * 128 / 5) * i,
						shouldRotateClockwise: shouldRotateClockwise,
						orbiterEnemyNumber: orbiterEnemyNumber,
						difficulty: this.difficulty));
				}
			}

			this.stationaryEnemyMovement.ProcessNextFrame(elapsedMillisPerFrame: elapsedMillisPerFrame);

			this.xMillis = this.stationaryEnemyMovement.GetXMillis();
			this.yMillis = this.stationaryEnemyMovement.GetYMillis();

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

			if (this.milliHp <= 0)
			{
				return new EnemyFrameResult(
					enemies: DTImmutableList<IEnemy>.AsImmutableList(
						new List<IEnemy>() { new StandardExplosionEnemy(
								xMillis: this.xMillis,
								yMillis: this.yMillis,
								rng: this.rng,
								scalingFactorScaled: 350) }),
					sounds: null,
					bossHealthMeterNumber: null,
					bossHealthMeterMilliPercentage: null,
					shouldEndLevel: true);
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

			if (this.status == Status.InCombat)
			{				
				Level1BossEnemyShootPattern.Result shootPatternResult = this.level1BossEnemyShootPattern.ProcessFrame(elapsedMillisPerFrame: elapsedMillisPerFrame);

				if (shootPatternResult != null)
				{
					if (shootPatternResult.Enemies != null)
					{
						foreach (IEnemy enemy in shootPatternResult.Enemies)
							enemies.Add(enemy);
					}

					if (shootPatternResult.Sounds != null)
					{
						if (sounds == null)
							sounds = new List<Danmaku2Sound>();

						foreach (Danmaku2Sound sound in shootPatternResult.Sounds)
							sounds.Add(sound);
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

			return new EnemyFrameResult(
				enemies: DTImmutableList<IEnemy>.AsImmutableList(enemies),
				sounds: sounds != null ? DTImmutableList<Danmaku2Sound>.AsImmutableList(sounds) : null,
				bossHealthMeterNumber: bossHealthMeterNumber,
				bossHealthMeterMilliPercentage: bossHealthMeterMilliPercentage,
				shouldEndLevel: this.status == Status.Despawned);
		}

		public bool IsInCombat()
		{
			return this.status == Status.InCombat;
		}

		public bool HasDespawned()
		{
			return this.status == Status.Despawned;
		}

		public bool IsDead()
		{
			return this.milliHp <= 0;
		}

		public void Render(IDisplay<Danmaku2Assets> display)
		{
			var assets = display.GetAssets();

			Danmaku2Image image = Danmaku2Image.Level1Boss;

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

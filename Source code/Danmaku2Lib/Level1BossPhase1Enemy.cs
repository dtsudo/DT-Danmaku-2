
namespace Danmaku2Lib
{
	using System;
	using System.Collections.Generic;

	public class Level1BossPhase1Enemy : IEnemy
	{
		private enum Status
		{
			Approaching,
			InCombat,
			TransitioningToPhase2
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

		private int transitionToPhase2CountdownInMillis;

		public Level1BossPhase1Enemy(
			IDTDeterministicRandom rng,
			Difficulty difficulty)
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

			this.transitionToPhase2CountdownInMillis = 5 * 1000;

			this.level1BossEnemyShootPattern = new Level1BossEnemyShootPattern(
				enemy: this,
				difficulty: difficulty,
				rng: rng,
				phase: Level1BossEnemyShootPattern.Phase.Phase1);
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

				Level1BossPhase1OrbiterEnemyCreator orbiterCreator = new Level1BossPhase1OrbiterEnemyCreator(rng: this.rng, elapsedMillisPerFrame: elapsedMillisPerFrame);
				for (int i = 0; i < 5; i++)
				{
					int random = this.rng.NextInt(orbiterNum.Count);
					int orbiterEnemyNumber = orbiterNum[random];
					orbiterNum.RemoveAt(random);

					enemies.Add(orbiterCreator.CreateBossPhase1OrbiterEnemy(
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

			if (this.status == Status.Approaching && this.stationaryEnemyMovement.HasStopped())
				this.status = Status.InCombat;

			int? bossHealthMeterNumber;
			int? bossHealthMeterMilliPercentage;

			if (this.status == Status.InCombat || this.status == Status.TransitioningToPhase2)
			{
				bossHealthMeterNumber = 2;
				bossHealthMeterMilliPercentage = (this.milliHp << 17) / INITIAL_MILLI_HP;
			}
			else
			{
				bossHealthMeterNumber = null;
				bossHealthMeterMilliPercentage = null;
			}

			if (this.status == Status.InCombat && this.milliHp <= 0)
			{
				this.status = Status.TransitioningToPhase2;
			}

			if (this.status == Status.InCombat)
			{
				this.timerCountdownInMillis -= elapsedMillisPerFrame;

				if (this.timerCountdownInMillis <= 0)
				{
					this.timerCountdownInMillis = 0;
					this.status = Status.TransitioningToPhase2;
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

			if (this.status == Status.TransitioningToPhase2)
			{
				this.transitionToPhase2CountdownInMillis -= elapsedMillisPerFrame;

				if (this.transitionToPhase2CountdownInMillis <= 0)
					this.transitionToPhase2CountdownInMillis = 0;
			}

			if (this.transitionToPhase2CountdownInMillis == 0)
				enemies.Add(new Level1BossPhase2Enemy(
					stationaryEnemyMovement: this.stationaryEnemyMovement,
					xMillis: this.xMillis,
					yMillis: this.yMillis,
					rng: this.rng,
					difficulty: this.difficulty,
					elapsedMillisPerFrame: elapsedMillisPerFrame));
			else
				enemies.Add(this);

			return new EnemyFrameResult(
				enemies: DTImmutableList<IEnemy>.AsImmutableList(enemies),
				sounds: sounds != null ? DTImmutableList<Danmaku2Sound>.AsImmutableList(sounds) : null,
				bossHealthMeterNumber: bossHealthMeterNumber,
				bossHealthMeterMilliPercentage: bossHealthMeterMilliPercentage,
				shouldEndLevel: false);
		}

		public bool IsInCombat()
		{
			return this.status == Status.InCombat;
		}

		public bool ShouldDespawnOrbiters()
		{
			return this.status == Status.TransitioningToPhase2;
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

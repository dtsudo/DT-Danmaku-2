
namespace Danmaku2Lib
{
	using System;
	using System.Collections.Generic;

	public class CircleEnemyCreator
	{
		private List<ObjectBox> collisionBoxes;
		private List<ObjectBox> damageBoxes;
		private List<ObjectBox> bulletCollisionBoxes;

		private int elapsedMillisPerFrame;
		private IDTDeterministicRandom rng;

		public CircleEnemyCreator(
			IDTDeterministicRandom rng,
			int elapsedMillisPerFrame)
		{
			this.collisionBoxes = new List<ObjectBox>()
			{
				new ObjectBox(-8000, 8000, -12000, -4000),
				new ObjectBox(-16000, 16000, -4000, 12000)
			};

			this.damageBoxes = new List<ObjectBox>()
			{
				new ObjectBox(-26 * 1024, 26 * 1024, 10 * 1024, 21 * 1024),
				new ObjectBox(-17 * 1024, 17 * 1024, -5 * 1024, 21 * 1024),
				new ObjectBox(-13 * 1024, 13 * 1024, -21 * 1024, 21 * 1024)
			};

			this.bulletCollisionBoxes = new List<ObjectBox>()
			{
				new ObjectBox(-3 * 1024, 3 * 1024, -3 * 1024, 3 * 1024)
			};

			this.elapsedMillisPerFrame = elapsedMillisPerFrame;
			this.rng = rng;
		}

		public CircleEnemy CreateCircleEnemy(
			int yMillis,
			bool isComingFromLeftSide,
			Difficulty difficulty)
		{
			return new CircleEnemy(
				yMillis: yMillis,
				isComingFromLeftSide: isComingFromLeftSide,
				collisionBoxes: this.collisionBoxes,
				damageBoxes: this.damageBoxes,
				bulletCollisionBoxes: this.bulletCollisionBoxes,
				difficulty: difficulty,
				elapsedMillisPerFrame: this.elapsedMillisPerFrame,
				rng: this.rng);
		}

		public class CircleEnemy : IEnemy
		{
			private List<ObjectBox> collisionBoxes;
			private List<ObjectBox> damageBoxes;
			private List<ObjectBox> bulletCollisionBoxes;
			private Difficulty difficulty;
			
			private EnemyFrameResult enemyFrameResult;

			private int xMillis;
			private int yMillis;
			private int milliHp;

			private bool isComingFromLeftSide;
			private int deltaXPerFrame;

			private int initialBulletDegreesScaled;
			private int deltaBulletDegreesScaled;

			private IDTDeterministicRandom rng;

			private int currentShootCooldownMillis;
			private int shootCooldownMillis;

			private bool isDestroyedOrDespawned;

			public CircleEnemy(
				int yMillis,
				bool isComingFromLeftSide,
				List<ObjectBox> collisionBoxes,
				List<ObjectBox> damageBoxes,
				List<ObjectBox> bulletCollisionBoxes,
				Difficulty difficulty,
				int elapsedMillisPerFrame,
				IDTDeterministicRandom rng)
			{
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
				
				this.xMillis = isComingFromLeftSide
					? -100 * 1024
					: (GameLogic.GAME_WINDOW_WIDTH_IN_PIXELS + 100) * 1024;

				this.yMillis = yMillis;

				this.isComingFromLeftSide = isComingFromLeftSide;
				this.deltaXPerFrame = isComingFromLeftSide
					? 90 * elapsedMillisPerFrame
					: -90 * elapsedMillisPerFrame;

				this.rng = rng;

				switch (difficulty)
				{
					case Difficulty.Easy:
						this.shootCooldownMillis = 12500;
						break;
					case Difficulty.Normal:
						this.shootCooldownMillis = 9500;
						break;
					case Difficulty.Hard:
						this.shootCooldownMillis = 1000;
						break;
					default:
						throw new Exception();
				}

				this.currentShootCooldownMillis = rng.NextInt(this.shootCooldownMillis);

				this.milliHp = 200;
				
				switch (difficulty)
				{
					case Difficulty.Easy:
						this.deltaBulletDegreesScaled = 360 * 128 / 6;
						break;
					case Difficulty.Normal:
						this.deltaBulletDegreesScaled = 360 * 128 / 9;
						break;
					case Difficulty.Hard:
						this.deltaBulletDegreesScaled = 360 * 128 / 12;
						break;
					default:
						throw new Exception();
				}
				this.initialBulletDegreesScaled = rng.NextInt(this.deltaBulletDegreesScaled);

				this.isDestroyedOrDespawned = false;
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
					this.isDestroyedOrDespawned = true;
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

				this.xMillis += this.deltaXPerFrame;

				if (this.isComingFromLeftSide && this.xMillis > (GameLogic.GAME_WINDOW_WIDTH_IN_PIXELS + 100) * 1024)
				{
					this.isDestroyedOrDespawned = true;
					return null;
				}
				if (!this.isComingFromLeftSide && this.xMillis < -100 * 1024)
				{
					this.isDestroyedOrDespawned = true;
					return null;
				}

				this.currentShootCooldownMillis = this.currentShootCooldownMillis - elapsedMillisPerFrame;
				if (this.currentShootCooldownMillis <= 0)
				{
					this.currentShootCooldownMillis += this.shootCooldownMillis;
					
					if (this.currentShootCooldownMillis <= 0)
						this.currentShootCooldownMillis = 0;

					List<IEnemy> enemies = new List<IEnemy>();
					enemies.Add(this);

					int degreesScaled = this.initialBulletDegreesScaled;

					int bulletXMillis = this.isComingFromLeftSide
						? this.xMillis + 25 * 1024
						: this.xMillis - 25 * 1024;

					while (true)
					{
						if (degreesScaled >= 360 * 128)
							break;

						int speedInMillipixelsPerMillisecond;
						switch (this.difficulty)
						{
							case Difficulty.Easy:
								speedInMillipixelsPerMillisecond = 50;
								break;
							case Difficulty.Normal:
								speedInMillipixelsPerMillisecond = 100;
								break;
							case Difficulty.Hard:
								speedInMillipixelsPerMillisecond = 125;
								break;
							default:
								throw new Exception();
						}

						enemies.Add(new SimpleEnemyBulletEnemy(
							xMillis: bulletXMillis,
							yMillis: this.yMillis,
							degreesScaled: degreesScaled,
							speedInMillipixelsPerMillisecond: speedInMillipixelsPerMillisecond,
							elapsedMillisPerFrame: elapsedMillisPerFrame,
							collisionBoxes: this.bulletCollisionBoxes,
							danmaku2Image: Danmaku2Image.CircleEnemyBullet,
							numPixelsOffscreenBeforeDespawn: 50));
						
						degreesScaled += this.deltaBulletDegreesScaled;
					}
					
					return new EnemyFrameResult(
						DTImmutableList<IEnemy>.AsImmutableList(enemies),
						sounds: DTImmutableList<Danmaku2Sound>.AsImmutableList(
							new List<Danmaku2Sound> { Danmaku2Sound.EnemyShoot }),
						bossHealthMeterNumber: null,
						bossHealthMeterMilliPercentage: null,
						shouldEndLevel: false);
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

			public bool IsDestroyedOrDespawned()
			{
				return this.isDestroyedOrDespawned;
			}

			public void Render(IDisplay<Danmaku2Assets> display)
			{
				var assets = display.GetAssets();

				Danmaku2Image image = Danmaku2Image.CircleEnemyShip;

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

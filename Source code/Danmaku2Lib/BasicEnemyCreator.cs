
namespace Danmaku2Lib
{
	using System;
	using System.Collections.Generic;

	public class BasicEnemyCreator
	{
		private List<ObjectBox> collisionBoxes;
		private List<ObjectBox> damageBoxes;
		private List<ObjectBox> bulletCollisionBoxes;

		private int elapsedMillisPerFrame;
		private IDTDeterministicRandom rng;

		public BasicEnemyCreator(
			IDTDeterministicRandom rng,
			int elapsedMillisPerFrame)
		{
			this.collisionBoxes = new List<ObjectBox>()
			{
				new ObjectBox(-8000, 8000, -12000, 20000),
				new ObjectBox(-16000, 16000, -4000, 12000)
			};

			this.damageBoxes = new List<ObjectBox>()
			{
				new ObjectBox(-26 * 1024, 26 * 1024, -8 * 1024, 21 * 1024),
				new ObjectBox(-17 * 1024, 17 * 1024, -21 * 1024, 21 * 1024)
			};

			this.bulletCollisionBoxes = new List<ObjectBox>()
			{
				new ObjectBox(-4 * 1024, 4 * 1024, -4 * 1024, 4 * 1024)
			};

			this.elapsedMillisPerFrame = elapsedMillisPerFrame;
			this.rng = rng;
		}

		public BasicEnemy CreateBasicEnemy(
			int xMillis,
			Difficulty difficulty)
		{
			return new BasicEnemy(
				xMillis: xMillis,
				collisionBoxes: this.collisionBoxes,
				damageBoxes: this.damageBoxes,
				bulletCollisionBoxes: this.bulletCollisionBoxes,
				difficulty: difficulty,
				elapsedMillisPerFrame: this.elapsedMillisPerFrame,
				rng: this.rng);
		}

		public class BasicEnemy : IEnemy
		{
			private List<ObjectBox> collisionBoxes;
			private List<ObjectBox> damageBoxes;
			private List<ObjectBox> bulletCollisionBoxes;
			private Difficulty difficulty;
			
			private EnemyFrameResult enemyFrameResult;

			private int xMillis;
			private int yMillis;
			private int milliHp;

			private int deltaYPerFrame;
			
			private IDTDeterministicRandom rng;

			private int currentShootCooldownMillis;
			private const int SHOOT_COOLDOWN_MILLIS_EASY = 2 * 1024;
			private const int SHOOT_COOLDOWN_MILLIS_NORMAL = 4 * 1024;
			private const int SHOOT_COOLDOWN_MILLIS_HARD = 2 * 1024;
			
			private bool isDestroyedOrDespawned;

			public BasicEnemy(
				int xMillis,
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

				this.xMillis = xMillis;
				this.yMillis = (GameLogic.GAME_WINDOW_HEIGHT_IN_PIXELS + 50) * 1024;

				this.deltaYPerFrame = -90 * elapsedMillisPerFrame;

				this.rng = rng;
				
				switch (difficulty)
				{
					case Difficulty.Easy:
						this.currentShootCooldownMillis = rng.NextInt(SHOOT_COOLDOWN_MILLIS_EASY);
						break;
					case Difficulty.Normal:
						this.currentShootCooldownMillis = rng.NextInt(SHOOT_COOLDOWN_MILLIS_NORMAL);
						break;
					case Difficulty.Hard:
						this.currentShootCooldownMillis = rng.NextInt(SHOOT_COOLDOWN_MILLIS_HARD);
						break;
					default:
						throw new Exception();
				}

				switch (difficulty)
				{
					case Difficulty.Easy:
						this.milliHp = 600;
						break;
					case Difficulty.Normal:
						this.milliHp = 600;
						break;
					case Difficulty.Hard:
						this.milliHp = 600;
						break;
					default:
						throw new Exception();
				}

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

				this.yMillis += this.deltaYPerFrame;
				
				if (this.yMillis < -50 * 1024)
				{
					this.isDestroyedOrDespawned = true;
					return null;
				}

				this.currentShootCooldownMillis = this.currentShootCooldownMillis - elapsedMillisPerFrame;
				if (this.currentShootCooldownMillis <= 0)
				{
					switch (this.difficulty)
					{
						case Difficulty.Easy:
							this.currentShootCooldownMillis += SHOOT_COOLDOWN_MILLIS_EASY;
							break;
						case Difficulty.Normal:
							this.currentShootCooldownMillis += SHOOT_COOLDOWN_MILLIS_NORMAL;
							break;
						case Difficulty.Hard:
							this.currentShootCooldownMillis += SHOOT_COOLDOWN_MILLIS_HARD;
							break;
						default:
							throw new Exception();
					}

					if (this.currentShootCooldownMillis <= 0)
						this.currentShootCooldownMillis = 0;

					List<IEnemy> enemies = new List<IEnemy>();
					enemies.Add(this);

					var bulletYMillis = this.yMillis - 25 * 1024;

					enemies.Add(new BasicEnemyBulletEnemy(
						xMillis: this.xMillis,
						yMillis: bulletYMillis,
						degreesScaled: 180 * 128, 
						elapsedMillisPerFrame: elapsedMillisPerFrame,
						difficulty: this.difficulty,
						collisionBoxes: this.bulletCollisionBoxes));
					
					if (this.difficulty == Difficulty.Normal || this.difficulty == Difficulty.Hard)
					{
						enemies.Add(new BasicEnemyBulletEnemy(
							xMillis: this.xMillis,
							yMillis: bulletYMillis,
							degreesScaled: 170 * 128,
							elapsedMillisPerFrame: elapsedMillisPerFrame,
							difficulty: this.difficulty,
							collisionBoxes: this.bulletCollisionBoxes));
						enemies.Add(new BasicEnemyBulletEnemy(
							xMillis: this.xMillis,
							yMillis: bulletYMillis,
							degreesScaled: 190 * 128,
							elapsedMillisPerFrame: elapsedMillisPerFrame,
							difficulty: this.difficulty,
							collisionBoxes: this.bulletCollisionBoxes));

						if (this.difficulty == Difficulty.Hard)
						{
							enemies.Add(new BasicEnemyBulletEnemy(
								xMillis: this.xMillis,
								yMillis: bulletYMillis,
								degreesScaled: 160 * 128,
								elapsedMillisPerFrame: elapsedMillisPerFrame,
								difficulty: this.difficulty,
								collisionBoxes: this.bulletCollisionBoxes));
							enemies.Add(new BasicEnemyBulletEnemy(
								xMillis: this.xMillis,
								yMillis: bulletYMillis,
								degreesScaled: 200 * 128,
								elapsedMillisPerFrame: elapsedMillisPerFrame,
								difficulty: this.difficulty,
								collisionBoxes: this.bulletCollisionBoxes));
						}
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

				Danmaku2Image image = Danmaku2Image.BasicEnemyShip;

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

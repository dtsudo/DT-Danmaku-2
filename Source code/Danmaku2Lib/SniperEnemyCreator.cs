
namespace Danmaku2Lib
{
	using System;
	using System.Collections.Generic;

	public class SniperEnemyCreator
	{
		private List<ObjectBox> collisionBoxes;
		private List<ObjectBox> damageBoxes;
		private List<ObjectBox> bulletCollisionBoxes;

		private IDTDeterministicRandom rng;

		public SniperEnemyCreator(IDTDeterministicRandom rng)
		{
			this.collisionBoxes = new List<ObjectBox>()
			{
				new ObjectBox(-8000, 8000, -16000, 16000),
				new ObjectBox(-16000, 16000, -8000, 8000)
			};

			this.damageBoxes = new List<ObjectBox>()
			{
				new ObjectBox(-26 * 1024, 26 * 1024, -8 * 1024, 21 * 1024),
				new ObjectBox(-17 * 1024, 17 * 1024, -21 * 1024, 21 * 1024)
			};

			this.bulletCollisionBoxes = new List<ObjectBox>()
			{
				new ObjectBox(-3 * 1024, 3 * 1024, -3 * 1024, 3 * 1024)
			};

			this.rng = rng;
		}

		public SniperEnemy CreateSniperEnemy(
			int xMillis,
			Difficulty difficulty)
		{
			return new SniperEnemy(
				xMillis: xMillis,
				collisionBoxes: this.collisionBoxes,
				damageBoxes: this.damageBoxes,
				bulletCollisionBoxes: this.bulletCollisionBoxes,
				difficulty: difficulty,
				rng: this.rng);
		}

		public class SniperEnemy : IEnemy
		{
			private enum Status
			{
				Approaching,
				StoppedAndFiring,
				Leaving
			}

			private Status status;

			private List<ObjectBox> collisionBoxes;
			private List<ObjectBox> damageBoxes;
			private List<ObjectBox> bulletCollisionBoxes;
			private Difficulty difficulty;

			private EnemyFrameResult enemyFrameResult;

			private int xMillis;
			private int yMillis;
			private int milliHp;

			private int elapsedTimeMillis;

			private int speed;
			private int facingDirectionInDegreesScaled;

			private IDTDeterministicRandom rng;

			private int currentShootCooldownMillis;
			private const int SHOOT_COOLDOWN_MILLIS_EASY = 5 * 1024;
			private const int SHOOT_COOLDOWN_MILLIS_NORMAL = 3 * 1024;
			private const int SHOOT_COOLDOWN_MILLIS_HARD = 400;

			private bool isDestroyedOrDespawned;

			public SniperEnemy(
				int xMillis,
				List<ObjectBox> collisionBoxes,
				List<ObjectBox> damageBoxes,
				List<ObjectBox> bulletCollisionBoxes,
				Difficulty difficulty,
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
				this.yMillis = (GameLogic.GAME_WINDOW_HEIGHT_IN_PIXELS + 100) * 1024;

				this.elapsedTimeMillis = 0;

				this.rng = rng;

				this.speed = 6000;
				this.status = Status.Approaching;

				this.facingDirectionInDegreesScaled = 180 * 128;

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
						this.milliHp = 1700;
						break;
					case Difficulty.Normal:
						this.milliHp = 1700;
						break;
					case Difficulty.Hard:
						this.milliHp = 1700;
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

				this.elapsedTimeMillis += elapsedMillisPerFrame;

				if (this.status == Status.Approaching)
				{
					this.speed -= elapsedMillisPerFrame;
					if (this.speed <= 0)
					{
						this.speed = 0;
						this.status = Status.StoppedAndFiring;
					}
				}
				else if (this.status == Status.StoppedAndFiring)
				{
					if (this.elapsedTimeMillis > 9000)
						this.status = Status.Leaving;
				}
				else if (this.status == Status.Leaving)
				{
					this.speed += elapsedMillisPerFrame;
				}
				else
				{
					throw new Exception();
				}

				int degreesScaled = DanmakuMath.GetMovementDirectionInDegreesScaled(
					currentX: this.xMillis,
					currentY: this.yMillis,
					desiredX: playerXMillis,
					desiredY: playerYMillis);

				if (degreesScaled < 120 * 128)
					degreesScaled = 120 * 128;
				else if (degreesScaled > 240 * 128)
					degreesScaled = 240 * 128;

				if (degreesScaled < this.facingDirectionInDegreesScaled)
				{
					if (this.facingDirectionInDegreesScaled - degreesScaled < (elapsedMillisPerFrame << 4))
						this.facingDirectionInDegreesScaled = degreesScaled;
					else
						this.facingDirectionInDegreesScaled = this.facingDirectionInDegreesScaled - (elapsedMillisPerFrame << 4);
				}
				else if (degreesScaled > this.facingDirectionInDegreesScaled)
				{
					if (degreesScaled - this.facingDirectionInDegreesScaled < (elapsedMillisPerFrame << 4))
						this.facingDirectionInDegreesScaled = degreesScaled;
					else
						this.facingDirectionInDegreesScaled = this.facingDirectionInDegreesScaled + (elapsedMillisPerFrame << 4);
				}

				var offset = DanmakuMath.GetOffset(
					speedInMillipixelsPerMillisecond: this.speed >> 6,
					movementDirectionInDegreesScaled: this.facingDirectionInDegreesScaled,
					elapsedMillisecondsPerIteration: elapsedMillisPerFrame);

				if (this.status == Status.Approaching)
				{
					this.xMillis += offset.DeltaXInMillipixels;
					this.yMillis += offset.DeltaYInMillipixels;
				}
				else if (this.status == Status.Leaving)
				{
					this.xMillis -= offset.DeltaXInMillipixels;
					this.yMillis -= offset.DeltaYInMillipixels;
				}

				if (this.status == Status.Leaving && (this.yMillis >> 10) > GameLogic.GAME_WINDOW_HEIGHT_IN_PIXELS + 100)
				{
					this.isDestroyedOrDespawned = true;
					return null;
				}

				this.currentShootCooldownMillis = this.currentShootCooldownMillis - elapsedMillisPerFrame;
				if (this.currentShootCooldownMillis <= 0)
				{
					switch (difficulty)
					{
						case Difficulty.Easy:
							this.currentShootCooldownMillis = SHOOT_COOLDOWN_MILLIS_EASY;
							break;
						case Difficulty.Normal:
							this.currentShootCooldownMillis = SHOOT_COOLDOWN_MILLIS_NORMAL;
							break;
						case Difficulty.Hard:
							this.currentShootCooldownMillis = SHOOT_COOLDOWN_MILLIS_HARD;
							break;
						default:
							throw new Exception();
					}

					var bulletOffset = DanmakuMath.GetOffset(
						speedInMillipixelsPerMillisecond: 30,
						movementDirectionInDegreesScaled: this.facingDirectionInDegreesScaled,
						elapsedMillisecondsPerIteration: 1000);

					var bullet = new SniperEnemyBulletEnemy(
						xMillis: this.xMillis + bulletOffset.DeltaXInMillipixels,
						yMillis: this.yMillis + bulletOffset.DeltaYInMillipixels,
						degreesScaled: this.facingDirectionInDegreesScaled,
						elapsedMillisPerFrame: elapsedMillisPerFrame,
						difficulty: this.difficulty,
						image: Danmaku2Image.SniperEnemyBullet,
						collisionBoxes: this.bulletCollisionBoxes,
						rng: this.rng);
					
					return new EnemyFrameResult(
						DTImmutableList<IEnemy>.AsImmutableList(
							new List<IEnemy>() { this, bullet }),
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

				var halfWidth = assets.GetWidth(Danmaku2Image.SniperEnemyShip) >> 1;
				var halfHeight = assets.GetHeight(Danmaku2Image.SniperEnemyShip) >> 1;

				display.GetAssets().DrawImageRotatedCounterclockwise(
					image: Danmaku2Image.SniperEnemyShip,
					x: (this.xMillis >> 10) - halfWidth,
					y: GameLogic.GAME_WINDOW_HEIGHT_IN_PIXELS - (this.yMillis >> 10) - halfHeight,
					degreesScaled: -this.facingDirectionInDegreesScaled);
			}
		}
	}
}

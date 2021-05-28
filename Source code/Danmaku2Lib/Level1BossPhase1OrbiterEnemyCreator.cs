
namespace Danmaku2Lib
{
	using System;
	using System.Collections.Generic;

	public class Level1BossPhase1OrbiterEnemyCreator
	{
		private List<ObjectBox> collisionBoxes;
		private List<ObjectBox> damageBoxes;
		private List<ObjectBox> bulletCollisionBoxes;

		private int elapsedMillisPerFrame;
		private IDTDeterministicRandom rng;

		public Level1BossPhase1OrbiterEnemyCreator(
			IDTDeterministicRandom rng,
			int elapsedMillisPerFrame)
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

			this.elapsedMillisPerFrame = elapsedMillisPerFrame;
			this.rng = rng;
		}

		public IEnemy CreateBossPhase1OrbiterEnemy(
			Level1BossPhase1Enemy bossEnemy,
			int initialDegreesScaled,
			bool shouldRotateClockwise,
			int orbiterEnemyNumber,
			Difficulty difficulty)
		{
			return new BossPhase1OrbiterEnemy(
				bossEnemy: bossEnemy,
				initialDegreesScaled: initialDegreesScaled,
				shouldRotateClockwise: shouldRotateClockwise,
				orbiterEnemyNumber: orbiterEnemyNumber,
				collisionBoxes: this.collisionBoxes,
				damageBoxes: this.damageBoxes,
				bulletCollisionBoxes: this.bulletCollisionBoxes,
				difficulty: difficulty,
				elapsedMillisPerFrame: this.elapsedMillisPerFrame,
				rng: this.rng);
		}

		private class BossPhase1OrbiterEnemy : IEnemy
		{
			private List<ObjectBox> collisionBoxes;
			private List<ObjectBox> damageBoxes;
			private List<ObjectBox> bulletCollisionBoxes;
			private Difficulty difficulty;

			private EnemyFrameResult enemyFrameResult;

			private int xMillis;
			private int yMillis;
			private int milliHp;

			private IDTDeterministicRandom rng;

			private Level1BossPhase1Enemy bossEnemy;

			private int degreesScaled;
			private bool shouldRotateClockwise;

			private int currentShootCooldownMillis;
			private int shootCooldownMillis;

			private int deltaDegreesScaled;

			private Danmaku2Image orbiterEnemyBulletImage;

			private int facingDirectionInDegreesScaled;

			private int elapsedMillisPerFrame;

			private static int ComputeXMillis(IEnemy bossEnemy, int locationInDegreesScaled)
			{
				return bossEnemy.GetXMillis() + 140 * DTMath.SineScaled(locationInDegreesScaled);
			}

			private static int ComputeYMillis(IEnemy bossEnemy, int locationInDegreesScaled)
			{
				return bossEnemy.GetYMillis() + 140 * DTMath.CosineScaled(locationInDegreesScaled);
			}

			public BossPhase1OrbiterEnemy(
				Level1BossPhase1Enemy bossEnemy,
				int initialDegreesScaled,
				bool shouldRotateClockwise,
				int orbiterEnemyNumber,
				List<ObjectBox> collisionBoxes,
				List<ObjectBox> damageBoxes,
				List<ObjectBox> bulletCollisionBoxes,
				Difficulty difficulty,
				int elapsedMillisPerFrame,
				IDTDeterministicRandom rng)
			{
				initialDegreesScaled = DanmakuMath.NormalizeAngleInDegreesScaled(angleInDegreesScaled: initialDegreesScaled);

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

				this.xMillis = ComputeXMillis(bossEnemy: bossEnemy, locationInDegreesScaled: initialDegreesScaled);
				this.yMillis = ComputeYMillis(bossEnemy: bossEnemy, locationInDegreesScaled: initialDegreesScaled);

				this.bossEnemy = bossEnemy;
				this.degreesScaled = initialDegreesScaled;
				this.shouldRotateClockwise = shouldRotateClockwise;

				this.deltaDegreesScaled = 2 * elapsedMillisPerFrame;

				this.rng = rng;
				
				switch (difficulty)
				{
					case Difficulty.Easy:
						this.shootCooldownMillis = 5 * 1024;
						break;
					case Difficulty.Normal:
						this.shootCooldownMillis = 3 * 1024;
						break;
					case Difficulty.Hard:
						this.shootCooldownMillis = 400;
						break;
					default:
						throw new Exception();
				}

				this.currentShootCooldownMillis = this.shootCooldownMillis * orbiterEnemyNumber / 5;

				this.milliHp = 5600;

				switch (orbiterEnemyNumber)
				{
					case 1:
						this.orbiterEnemyBulletImage = Danmaku2Image.Level1BossPhase1OrbiterEnemyBullet1;
						break;
					case 2:
						this.orbiterEnemyBulletImage = Danmaku2Image.Level1BossPhase1OrbiterEnemyBullet2;
						break;
					case 3:
						this.orbiterEnemyBulletImage = Danmaku2Image.Level1BossPhase1OrbiterEnemyBullet3;
						break;
					case 4:
						this.orbiterEnemyBulletImage = Danmaku2Image.Level1BossPhase1OrbiterEnemyBullet4;
						break;
					case 5:
						this.orbiterEnemyBulletImage = Danmaku2Image.Level1BossPhase1OrbiterEnemyBullet5;
						break;
					default:
						throw new Exception();
				}

				this.facingDirectionInDegreesScaled = 180 * 128;

				this.elapsedMillisPerFrame = elapsedMillisPerFrame;
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

			private void UpdateFacingDirection(
				int playerXMillis,
				int playerYMillis)
			{
				while (this.facingDirectionInDegreesScaled >= 360 * 128)
					this.facingDirectionInDegreesScaled = this.facingDirectionInDegreesScaled - 360 * 128;
				while (this.facingDirectionInDegreesScaled < 0)
					this.facingDirectionInDegreesScaled = this.facingDirectionInDegreesScaled + 360 * 128;
				
				int desiredDegreesScaled = DanmakuMath.GetMovementDirectionInDegreesScaled(
					currentX: this.xMillis,
					currentY: this.yMillis,
					desiredX: playerXMillis,
					desiredY: playerYMillis);

				int deltaDegreesScaled = Math.Abs(desiredDegreesScaled - this.facingDirectionInDegreesScaled);
				if (deltaDegreesScaled > 180 * 128)
					deltaDegreesScaled = 360 * 128 - deltaDegreesScaled;

				int maxDegreesScaledPerFrame = this.elapsedMillisPerFrame << 5;

				if (deltaDegreesScaled <= maxDegreesScaledPerFrame)
				{
					this.facingDirectionInDegreesScaled = desiredDegreesScaled;
					return;
				}

				bool shouldIncreaseDegree;

				if (desiredDegreesScaled > this.facingDirectionInDegreesScaled)
					shouldIncreaseDegree = desiredDegreesScaled - this.facingDirectionInDegreesScaled < 180 * 128;
				else
					shouldIncreaseDegree = this.facingDirectionInDegreesScaled - desiredDegreesScaled >= 180 * 128;

				if (shouldIncreaseDegree)
					this.facingDirectionInDegreesScaled += maxDegreesScaledPerFrame;
				else
					this.facingDirectionInDegreesScaled -= maxDegreesScaledPerFrame;

			}

			public EnemyFrameResult ProcessFrame(
				int elapsedMillisPerFrame,
				int playerXMillis,
				int playerYMillis)
			{
				if (this.milliHp <= 0 || this.bossEnemy.ShouldDespawnOrbiters())
				{
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

				if (this.shouldRotateClockwise)
				{
					this.degreesScaled += this.deltaDegreesScaled;
					while (this.degreesScaled >= 360 * 128)
						this.degreesScaled -= 360 * 128;
				}
				else
				{
					this.degreesScaled -= this.deltaDegreesScaled;
					while (this.degreesScaled < 0)
						this.degreesScaled += 360 * 128;
				}

				this.xMillis = ComputeXMillis(bossEnemy: this.bossEnemy, locationInDegreesScaled: this.degreesScaled);
				this.yMillis = ComputeYMillis(bossEnemy: this.bossEnemy, locationInDegreesScaled: this.degreesScaled);

				this.UpdateFacingDirection(playerXMillis: playerXMillis, playerYMillis: playerYMillis);

				this.currentShootCooldownMillis = this.currentShootCooldownMillis - elapsedMillisPerFrame;
				if (this.currentShootCooldownMillis <= 0)
				{
					this.currentShootCooldownMillis += this.shootCooldownMillis;

					if (this.currentShootCooldownMillis <= 0)
						this.currentShootCooldownMillis = 0;

					if (this.bossEnemy.IsInCombat())
					{
						List<IEnemy> enemies = new List<IEnemy>();
						enemies.Add(this);

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
							image: this.orbiterEnemyBulletImage,
							collisionBoxes: this.bulletCollisionBoxes,
							rng: this.rng);

						enemies.Add(bullet);

						return new EnemyFrameResult(
							DTImmutableList<IEnemy>.AsImmutableList(enemies),
							sounds: DTImmutableList<Danmaku2Sound>.AsImmutableList(
								new List<Danmaku2Sound> { Danmaku2Sound.EnemyShoot }),
							bossHealthMeterNumber: null,
							bossHealthMeterMilliPercentage: null,
							shouldEndLevel: false);
					}
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

			public void Render(IDisplay<Danmaku2Assets> display)
			{
				var assets = display.GetAssets();

				Danmaku2Image image = Danmaku2Image.Level1BossPhase1OrbiterEnemyShip;

				var halfWidth = assets.GetWidth(image) >> 1;
				var halfHeight = assets.GetHeight(image) >> 1;

				assets.DrawImageRotatedCounterclockwise(
					image: image,
					x: (this.xMillis >> 10) - halfWidth,
					y: GameLogic.GAME_WINDOW_HEIGHT_IN_PIXELS - (this.yMillis >> 10) - halfHeight,
					degreesScaled: -this.facingDirectionInDegreesScaled);
			}
		}
	}
}

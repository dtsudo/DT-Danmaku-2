
namespace Danmaku2Lib
{
	using System;
	using System.Collections.Generic;

	public class PlayerBulletsUtil
	{
		private int elapsedMillisPerFrame;
		public PlayerBulletSpreadLevel playerBulletSpreadLevel;
		public PlayerBulletStrength playerBulletStrength;

		// Amount of milliseconds before player can shoot again
		private int playerBulletCooldownInMillis;

		private List<ObjectBox> mainBulletCollisionBoxes;
		private List<ObjectBox> side1LeftBulletCollisionBoxes;
		private List<ObjectBox> side2LeftBulletCollisionBoxes;
		private List<ObjectBox> side3LeftBulletCollisionBoxes;
		private List<ObjectBox> side1RightBulletCollisionBoxes;
		private List<ObjectBox> side2RightBulletCollisionBoxes;
		private List<ObjectBox> side3RightBulletCollisionBoxes;

		private DTImmutableList<Danmaku2Sound> playerShootSound;

		public PlayerBulletsUtil(
			int elapsedMillisPerFrame,
			PlayerBulletSpreadLevel playerBulletSpreadLevel,
			PlayerBulletStrength playerBulletStrength)
		{
			this.elapsedMillisPerFrame = elapsedMillisPerFrame;
			this.playerBulletSpreadLevel = playerBulletSpreadLevel;
			this.playerBulletStrength = playerBulletStrength;

			this.playerBulletCooldownInMillis = 0;

			this.mainBulletCollisionBoxes = new List<ObjectBox>()
			{
				new ObjectBox(-4 * 1024, 4 * 1024, -4 * 1024, 4 * 1024),
				new ObjectBox(-4 * 1024, 4 * 1024, -25 * 1024, -17 * 1024)
			};

			this.side1LeftBulletCollisionBoxes = new List<ObjectBox>()
			{
				new ObjectBox(-4 * 1024, 4 * 1024, -4 * 1024, 4 * 1024),
				new ObjectBox(-1 * 1024, 7 * 1024, -24 * 1024, -16 * 1024)
			};

			this.side1RightBulletCollisionBoxes = new List<ObjectBox>()
			{
				new ObjectBox(-4 * 1024, 4 * 1024, -4 * 1024, 4 * 1024),
				new ObjectBox(-7 * 1024, 1 * 1024, -24 * 1024, -16 * 1024)
			};

			this.side2LeftBulletCollisionBoxes = new List<ObjectBox>()
			{
				new ObjectBox(-4 * 1024, 4 * 1024, -4 * 1024, 4 * 1024),
				new ObjectBox(3 * 1024, 11 * 1024, -23 * 1024, -15 * 1024)
			};

			this.side2RightBulletCollisionBoxes = new List<ObjectBox>()
			{
				new ObjectBox(-4 * 1024, 4 * 1024, -4 * 1024, 4 * 1024),
				new ObjectBox(-11 * 1024, -3 * 1024, -23 * 1024, -15 * 1024)
			};

			this.side3LeftBulletCollisionBoxes = new List<ObjectBox>()
			{
				new ObjectBox(-4 * 1024, 4 * 1024, -4 * 1024, 4 * 1024),
				new ObjectBox(7 * 1024, 15 * 1024, -22 * 1024, -14 * 1024)
			};

			this.side3RightBulletCollisionBoxes = new List<ObjectBox>()
			{
				new ObjectBox(-4 * 1024, 4 * 1024, -4 * 1024, 4 * 1024),
				new ObjectBox(-15 * 1024, -7 * 1024, -22 * 1024, -14 * 1024)
			};

			this.playerShootSound = new DTImmutableList<Danmaku2Sound>(
				list: new List<Danmaku2Sound>() { Danmaku2Sound.PlayerShoot });
		}

		public class FrameResult
		{
			public FrameResult(
				List<PlayerBullet> playerBullets,
				DTImmutableList<Danmaku2Sound> sounds)
			{
				this.PlayerBullets = playerBullets;
				this.Sounds = sounds;
			}

			public List<PlayerBullet> PlayerBullets { get; private set; }
			public DTImmutableList<Danmaku2Sound> Sounds { get; private set; }
		}

		public FrameResult ProcessFrame(
			List<PlayerBullet> playerBullets,
			IKeyboard keyboardInput,
			int playerXMillis,
			int playerYMillis,
			bool isPlayerDead,
			IDisplay<Danmaku2Assets> display)
		{
			List<PlayerBullet> newList = this.ProcessPlayerBulletMovement(
				playerBullets: playerBullets,
				display: display);

			var result = this.ProcessSpawningNewPlayerBullets(
				playerBullets: newList,
				keyboardInput: keyboardInput,
				playerXMillis: playerXMillis,
				playerYMillis: playerYMillis,
				isPlayerDead: isPlayerDead,
				display: display);

			return result;
		}

		private FrameResult ProcessSpawningNewPlayerBullets(
			List<PlayerBullet> playerBullets,
			IKeyboard keyboardInput,
			int playerXMillis,
			int playerYMillis,
			bool isPlayerDead,
			IDisplay<Danmaku2Assets> display)
		{
			this.playerBulletCooldownInMillis = this.playerBulletCooldownInMillis - this.elapsedMillisPerFrame;

			if (isPlayerDead)
			{
				if (this.playerBulletCooldownInMillis < 0)
					this.playerBulletCooldownInMillis = 0;

				return new FrameResult(
					playerBullets: playerBullets,
					sounds: null);
			}

			bool didShoot;

			if (this.playerBulletCooldownInMillis <= 0 && keyboardInput.IsPressed(Key.Z))
			{
				this.playerBulletCooldownInMillis += 200;
				didShoot = true;

				var assets = display.GetAssets();
				int playerHalfHeightMillis = (assets.GetHeight(Danmaku2Image.PlayerShip) >> 1) << 10;

				int playerBulletHalfHeightMillis = (assets.GetHeight(Danmaku2Image.PlayerBullet1) >> 1) << 10;

				int xMillis = playerXMillis;
				int yMillis = playerYMillis + playerHalfHeightMillis + playerBulletHalfHeightMillis;
				playerBullets.Add(new PlayerBullet(
					xMillis: xMillis,
					yMillis: yMillis,
					bulletType: PlayerBulletType.Main,
					bulletStrength: this.playerBulletStrength,
					collisionBoxes: this.mainBulletCollisionBoxes));

				// sin 10 degrees = 0.1736
				int side1XOffset = playerBulletHalfHeightMillis * 1736 / 10000;
				// 1 - (cos 10 degrees) = 0.0152
				int side1YOffset = playerBulletHalfHeightMillis * 152 / 10000;
				playerBullets.Add(
					new PlayerBullet(
						xMillis: xMillis - side1XOffset,
						yMillis: yMillis - side1YOffset,
						bulletType: PlayerBulletType.Side1Left,
						bulletStrength: this.playerBulletStrength,
						collisionBoxes: this.side1LeftBulletCollisionBoxes));
				playerBullets.Add(
					new PlayerBullet(
						xMillis: xMillis + side1XOffset,
						yMillis: yMillis - side1YOffset,
						bulletType: PlayerBulletType.Side1Right,
						bulletStrength: this.playerBulletStrength,
						collisionBoxes: this.side1RightBulletCollisionBoxes));

				if (this.playerBulletSpreadLevel == PlayerBulletSpreadLevel.FiveBullets || this.playerBulletSpreadLevel == PlayerBulletSpreadLevel.SevenBullets)
				{
					// sin 20 degrees = 0.3420
					int side2XOffset = playerBulletHalfHeightMillis * 3420 / 10000;
					// 1 - (cos 20 degrees) = 0.0603
					int side2YOffset = playerBulletHalfHeightMillis * 603 / 10000;
					playerBullets.Add(
						new PlayerBullet(
							xMillis: xMillis - side2XOffset,
							yMillis: yMillis - side2YOffset,
							bulletType: PlayerBulletType.Side2Left,
							bulletStrength: this.playerBulletStrength,
							collisionBoxes: this.side2LeftBulletCollisionBoxes));
					playerBullets.Add(
						new PlayerBullet(
							xMillis: xMillis + side2XOffset,
							yMillis: yMillis - side2YOffset,
							bulletType: PlayerBulletType.Side2Right,
							bulletStrength: this.playerBulletStrength,
							collisionBoxes: this.side2RightBulletCollisionBoxes));

					if (this.playerBulletSpreadLevel == PlayerBulletSpreadLevel.SevenBullets)
					{
						// sin 30 degrees = 0.5
						int side3XOffset = playerBulletHalfHeightMillis * 5000 / 10000;
						// 1 - (cos 30 degrees) = 0.1340
						int side3YOffset = playerBulletHalfHeightMillis * 1340 / 10000;
						playerBullets.Add(
							new PlayerBullet(
								xMillis: xMillis - side3XOffset,
								yMillis: yMillis - side3YOffset,
								bulletType: PlayerBulletType.Side3Left,
							bulletStrength: this.playerBulletStrength,
								collisionBoxes: this.side3LeftBulletCollisionBoxes));
						playerBullets.Add(
							new PlayerBullet(
								xMillis: xMillis + side3XOffset,
								yMillis: yMillis - side3YOffset,
								bulletType: PlayerBulletType.Side3Right,
							bulletStrength: this.playerBulletStrength,
								collisionBoxes: this.side3RightBulletCollisionBoxes));
					}
				}
			}
			else
			{
				didShoot = false;
			}

			if (this.playerBulletCooldownInMillis < 0)
				this.playerBulletCooldownInMillis = 0;

			return new FrameResult(
				playerBullets: playerBullets,
				sounds: didShoot ? this.playerShootSound : null);
		}

		private List<PlayerBullet> ProcessPlayerBulletMovement(
			List<PlayerBullet> playerBullets,
			IDisplay<Danmaku2Assets> display)
		{
			List<PlayerBullet> returnList = null;
			int bulletHeightInPixels = display.GetAssets().GetHeight(Danmaku2Image.PlayerBullet1);

			for (int i = 0; i < playerBullets.Count; i++)
			{
				PlayerBullet bullet = playerBullets[i];

				int deltaMilliPixels = 700 * this.elapsedMillisPerFrame;

				if (bullet.BulletType == PlayerBulletType.Main)
				{
					bullet.YMillis += deltaMilliPixels;
				}
				else if (bullet.BulletType == PlayerBulletType.Side1Left)
				{
					// sin 10 degrees = 0.1736
					bullet.XMillis -= deltaMilliPixels * 174 / 1000;
					// cos 10 degrees = 0.9848
					bullet.YMillis += deltaMilliPixels * 985 / 1000;
				}
				else if (bullet.BulletType == PlayerBulletType.Side1Right)
				{
					// sin 10 degrees = 0.1736
					bullet.XMillis += deltaMilliPixels * 174 / 1000;
					// cos 10 degrees = 0.9848
					bullet.YMillis += deltaMilliPixels * 985 / 1000;
				}
				else if (bullet.BulletType == PlayerBulletType.Side2Left)
				{
					// sin 20 degrees = 0.3420
					bullet.XMillis -= deltaMilliPixels * 342 / 1000;
					// cos 20 degrees = 0.9397
					bullet.YMillis += deltaMilliPixels * 940 / 1000;
				}
				else if (bullet.BulletType == PlayerBulletType.Side2Right)
				{
					// sin 20 degrees = 0.3420
					bullet.XMillis += deltaMilliPixels * 342 / 1000;
					// cos 20 degrees = 0.9397
					bullet.YMillis += deltaMilliPixels * 940 / 1000;
				}
				else if (bullet.BulletType == PlayerBulletType.Side3Left)
				{
					// sin 30 degrees = 0.5
					bullet.XMillis -= deltaMilliPixels * 500 / 1000;
					// cos 30 degrees = 0.8660
					bullet.YMillis += deltaMilliPixels * 866 / 1000;
				}
				else if (bullet.BulletType == PlayerBulletType.Side3Right)
				{
					// sin 30 degrees = 0.5
					bullet.XMillis += deltaMilliPixels * 500 / 1000;
					// cos 30 degrees = 0.8660
					bullet.YMillis += deltaMilliPixels * 866 / 1000;
				}
				else
				{
					throw new Exception();
				}

				if (this.IsOutOfBounds(bullet: bullet, bulletHeightInPixels: bulletHeightInPixels))
				{
					if (returnList == null)
					{
						returnList = new List<PlayerBullet>(capacity: playerBullets.Count + 14);
						for (int j = 0; j < i; j++)
						{
							returnList.Add(playerBullets[j]);
						}
					}
				}
				else
				{
					if (returnList != null)
					{
						returnList.Add(bullet);
					}
				}
			}

			return returnList != null ? returnList : playerBullets;
		}

		public void RenderPlayerBullets(
			List<PlayerBullet> playerBullets,
			IDisplay<Danmaku2Assets> display)
		{
			Danmaku2Assets assets = display.GetAssets();

			int halfWidth = assets.GetWidth(Danmaku2Image.PlayerBullet1) >> 1;
			int halfHeight = assets.GetHeight(Danmaku2Image.PlayerBullet1) >> 1;
			
			for (int i = 0; i < playerBullets.Count; i++)
			{
				PlayerBullet bullet = playerBullets[i];

				int degreesScaled;

				switch (bullet.BulletType)
				{
					case PlayerBulletType.Main:
						degreesScaled = 0;
						break;
					case PlayerBulletType.Side1Left:
						degreesScaled = 10 * 128;
						break;
					case PlayerBulletType.Side2Left:
						degreesScaled = 20 * 128;
						break;
					case PlayerBulletType.Side3Left:
						degreesScaled = 30 * 128;
						break;
					case PlayerBulletType.Side1Right:
						degreesScaled = -10 * 128;
						break;
					case PlayerBulletType.Side2Right:
						degreesScaled = -20 * 128;
						break;
					case PlayerBulletType.Side3Right:
						degreesScaled = -30 * 128;
						break;
					default:
						throw new Exception();
				}

				Danmaku2Image playerBulletImage;
				switch (bullet.BulletLevel)
				{
					case PlayerBulletStrength.Level1:
						playerBulletImage = Danmaku2Image.PlayerBullet1;
						break;
					case PlayerBulletStrength.Level2:
						playerBulletImage = Danmaku2Image.PlayerBullet2;
						break;
					case PlayerBulletStrength.Level3:
						playerBulletImage = Danmaku2Image.PlayerBullet3;
						break;
					default:
						throw new Exception();
				}

				assets.DrawImageRotatedCounterclockwise(
					image: playerBulletImage,
					x: (bullet.XMillis >> 10) - halfWidth,
					y: GameLogic.GAME_WINDOW_HEIGHT_IN_PIXELS - (bullet.YMillis >> 10) - halfHeight,
					degreesScaled: degreesScaled);
			}
		}

		private bool IsOutOfBounds(PlayerBullet bullet, int bulletHeightInPixels)
		{
			return (bullet.YMillis >> 10) > GameLogic.GAME_WINDOW_HEIGHT_IN_PIXELS + bulletHeightInPixels;
		}
	}
}

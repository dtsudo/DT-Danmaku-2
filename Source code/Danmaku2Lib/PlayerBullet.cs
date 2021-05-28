
namespace Danmaku2Lib
{
	using System;
	using System.Collections.Generic;

	public enum PlayerBulletType
	{
		Main,
		Side1Left,
		Side2Left,
		Side3Left,
		Side1Right,
		Side2Right,
		Side3Right
	}

	public enum PlayerBulletSpreadLevel
	{
		ThreeBullets,
		FiveBullets,
		SevenBullets
	}

	public enum PlayerBulletStrength
	{
		Level1,
		Level2,
		Level3
	}

	public class PlayerBullet
	{
		public PlayerBullet(
			int xMillis,
			int yMillis,
			PlayerBulletType bulletType,
			PlayerBulletStrength bulletStrength,
			List<ObjectBox> collisionBoxes)
		{
			this.XMillis = xMillis;
			this.YMillis = yMillis;
			this.BulletType = bulletType;
			this.BulletLevel = bulletStrength;
			this.CollisionBoxes = collisionBoxes;
		}

		public int XMillis;
		public int YMillis;
		public PlayerBulletType BulletType;
		public PlayerBulletStrength BulletLevel;
		public List<ObjectBox> CollisionBoxes;

		public static int GetBulletDamage(PlayerBulletStrength playerBulletStrength)
		{
			switch (playerBulletStrength)
			{
				case PlayerBulletStrength.Level1:
					return 100;
				case PlayerBulletStrength.Level2:
					return 150;
				case PlayerBulletStrength.Level3:
					return 225;
				default:
					throw new Exception();
			}
		}
	}
}


namespace Danmaku2Lib
{
	using System;
	using System.Collections.Generic;

	public class HitboxRenderer
	{
		public static void Render(
			IDisplay<Danmaku2Assets> display,
			List<PlayerBullet> playerBullets,
			List<IEnemy> enemies,
			bool shouldShowCollisionBoxes,
			bool shouldShowDamageBoxes)
		{
			if (shouldShowCollisionBoxes)
			{
				foreach (IEnemy enemy in enemies)
				{
					var collisionBoxes = enemy.GetCollisionBoxes();

					if (collisionBoxes != null)
					{
						foreach (var collisionBox in collisionBoxes)
						{
							display.DrawRectangle(
								x: (enemy.GetXMillis() + collisionBox.LowerXMillis) >> 10,
								y: GameLogic.GAME_WINDOW_HEIGHT_IN_PIXELS - ((enemy.GetYMillis() + collisionBox.UpperYMillis) >> 10),
								width: ((collisionBox.UpperXMillis - collisionBox.LowerXMillis) >> 10) + 1,
								height: ((collisionBox.UpperYMillis - collisionBox.LowerYMillis) >> 10) + 1,
								color: new DTColor(r: 255, g: 0, b: 0, alpha: 50),
								fill: true);
						}
					}
				}
			}

			if (shouldShowDamageBoxes)
			{
				foreach (PlayerBullet playerBullet in playerBullets)
				{
					for (int i = 0; i < playerBullet.CollisionBoxes.Count; i++)
					{
						ObjectBox playerBulletObjectBox = playerBullet.CollisionBoxes[i];
						display.DrawRectangle(
							x: (playerBullet.XMillis + playerBulletObjectBox.LowerXMillis) >> 10,
							y: GameLogic.GAME_WINDOW_HEIGHT_IN_PIXELS - ((playerBullet.YMillis + playerBulletObjectBox.UpperYMillis) >> 10),
							width: ((playerBulletObjectBox.UpperXMillis - playerBulletObjectBox.LowerXMillis) >> 10) + 1,
							height: ((playerBulletObjectBox.UpperYMillis - playerBulletObjectBox.LowerYMillis) >> 10) + 1,
							color: new DTColor(r: 255, g: 0, b: 0, alpha: 50),
							fill: true);
					}
				}

				foreach (IEnemy enemy in enemies)
				{
					var damageBoxes = enemy.GetDamageBoxes();

					if (damageBoxes != null)
					{
						foreach (var damageBox in damageBoxes)
						{
							display.DrawRectangle(
								x: (enemy.GetXMillis() + damageBox.LowerXMillis) >> 10,
								y: GameLogic.GAME_WINDOW_HEIGHT_IN_PIXELS - ((enemy.GetYMillis() + damageBox.UpperYMillis) >> 10),
								width: ((damageBox.UpperXMillis - damageBox.LowerXMillis) >> 10) + 1,
								height: ((damageBox.UpperYMillis - damageBox.LowerYMillis) >> 10) + 1,
								color: new DTColor(r: 255, g: 0, b: 0, alpha: 50),
								fill: true);
						}
					}
				}
			}
		}
	}
}


namespace Danmaku2Lib
{
	using System.Collections.Generic;

	public class CollisionDetector
	{
		private List<ObjectBox> playerCollisionBoxes;

		public CollisionDetector()
		{
			this.playerCollisionBoxes = new List<ObjectBox>()
			{
				new ObjectBox(lowerXMillis: -1 * 1024, upperXMillis: 1 * 1024, lowerYMillis: -1 * 1024, upperYMillis: 1 * 1024)
			};
		}

		public List<PlayerBullet> HandleCollisionBetweenPlayerBulletsAndEnemyObjects(
			List<PlayerBullet> playerBullets,
			List<IEnemy> enemies)
		{
			List<PlayerBullet> newPlayerBullets = new List<PlayerBullet>(capacity: playerBullets.Count + 14);

			foreach (PlayerBullet playerBullet in playerBullets)
			{
				bool hasCollidedWithAnyEnemy = false;

				foreach (IEnemy enemyObject in enemies)
				{
					bool hasCollided = this.HasCollided(
						object1XMillis: playerBullet.XMillis,
						object1YMillis: playerBullet.YMillis,
						object1Boxes: playerBullet.CollisionBoxes,
						object2XMillis: enemyObject.GetXMillis(),
						object2YMillis: enemyObject.GetYMillis(),
						object2Boxes: enemyObject.GetDamageBoxes());

					if (hasCollided)
					{
						enemyObject.HandleCollisionWithPlayerBullet(bulletStrength: playerBullet.BulletLevel);
						hasCollidedWithAnyEnemy = true;
						break;
					}
				}

				if (!hasCollidedWithAnyEnemy)
					newPlayerBullets.Add(playerBullet);
			}

			return newPlayerBullets;
		}

		// Returns whether the player collided with any enemy objects
		public bool HandleCollisionBetweenPlayerAndEnemyObjects(
			int playerXMillis,
			int playerYMillis,
			bool isPlayerDead,
			bool isPlayerInvulnerable,
			List<IEnemy> enemies)
		{
			if (isPlayerDead)
				return false;

			if (isPlayerInvulnerable)
				return false;
			
			foreach (IEnemy enemyObj in enemies)
			{
				bool hasCollided = this.HasCollided(
					object1XMillis: playerXMillis,
					object1YMillis: playerYMillis,
					object1Boxes: this.playerCollisionBoxes,
					object2XMillis: enemyObj.GetXMillis(),
					object2YMillis: enemyObj.GetYMillis(),
					object2Boxes: enemyObj.GetCollisionBoxes());

				if (hasCollided)
				{
					enemyObj.HandleCollisionWithPlayer();
					return true;
				}
			}

			return false;
		}

		public bool HasCollided(
			int object1XMillis,
			int object1YMillis,
			List<ObjectBox> object1Boxes, /* nullable */
			int object2XMillis,
			int object2YMillis,
			List<ObjectBox> object2Boxes /* nullable */)
		{
			if (object1Boxes == null || object2Boxes == null)
				return false;
			if (object1Boxes.Count == 0 || object2Boxes.Count == 0)
				return false;

			for (int i = 0; i < object1Boxes.Count; i++)
			{
				ObjectBox obj1Box = object1Boxes[i];
				for (int j = 0; j < object2Boxes.Count; j++)
				{
					ObjectBox obj2Box = object2Boxes[j];

					int obj1Left = object1XMillis + obj1Box.LowerXMillis;
					int obj1Right = object1XMillis + obj1Box.UpperXMillis;

					int obj2Left = object2XMillis + obj2Box.LowerXMillis;
					int obj2Right = object2XMillis + obj2Box.UpperXMillis;

					bool noXCollision = obj1Right < obj2Left || obj2Right < obj1Left;

					if (!noXCollision)
					{
						int obj1Bottom = object1YMillis + obj1Box.LowerYMillis;
						int obj1Top = object1YMillis + obj1Box.UpperYMillis;

						int obj2Bottom = object2YMillis + obj2Box.LowerYMillis;
						int obj2Top = object2YMillis + obj2Box.UpperYMillis;

						bool noYCollision = obj1Top < obj2Bottom || obj2Top < obj1Bottom;

						if (!noYCollision)
							return true;
					}
				}
			}

			return false;
		}
	}
}

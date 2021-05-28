
namespace Danmaku2Lib
{
	using System.Collections.Generic;

	public class EnemyRenderer
	{
		public static void RenderEnemiesOfSpecificZIndex(
			List<IEnemy> enemies,
			ZIndex enemyZIndex,
			IDisplay<Danmaku2Assets> display)
		{
			for (int i = 0; i < enemies.Count; i++)
			{
				IEnemy enemy = enemies[i];

				if (enemy.GetZIndex() == enemyZIndex)
					enemy.Render(display: display);
			}
		}
	}
}


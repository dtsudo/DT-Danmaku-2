
namespace Danmaku2Lib
{
	using System.Collections.Generic;

	public interface IEnemy
	{
		int GetXMillis();

		int GetYMillis();

		List<ObjectBox> GetCollisionBoxes();

		List<ObjectBox> GetDamageBoxes();

		EnemyFrameResult ProcessFrame(
			int elapsedMillisPerFrame,
			int playerXMillis,
			int playerYMillis);

		void HandleCollisionWithPlayer();

		void HandleCollisionWithPlayerBullet(PlayerBulletStrength bulletStrength);

		ZIndex GetZIndex();

		void Render(IDisplay<Danmaku2Assets> display);
	}
}

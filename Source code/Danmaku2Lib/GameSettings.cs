
namespace Danmaku2Lib
{
	public class GameSettings
	{
		public GameSettings(
			PlayerBulletSpreadLevel playerBulletSpreadLevel,
			PlayerBulletStrength playerBulletStrength,
			Difficulty difficulty,
			int numLivesRemaining)
		{
			this.playerBulletSpreadLevel = playerBulletSpreadLevel;
			this.playerBulletStrength = playerBulletStrength;
			this.difficulty = difficulty;
			this.numLivesRemaining = numLivesRemaining;
		}

		public PlayerBulletSpreadLevel playerBulletSpreadLevel { get; private set; }
		public PlayerBulletStrength playerBulletStrength { get; private set; }
		public Difficulty difficulty { get; private set; }
		public int numLivesRemaining { get; private set; }
	}
}

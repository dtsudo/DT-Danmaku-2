
namespace Danmaku2Lib
{
	public class EnemyFrameResult
	{
		public EnemyFrameResult(
			DTImmutableList<IEnemy> enemies,
			DTImmutableList<Danmaku2Sound> sounds,
			int? bossHealthMeterNumber,
			int? bossHealthMeterMilliPercentage,
			bool shouldEndLevel)
		{
			this.Enemies = enemies;
			this.Sounds = sounds;
			this.BossHealthMeterNumber = bossHealthMeterNumber;
			this.BossHealthMeterMilliPercentage = bossHealthMeterMilliPercentage;
			this.ShouldEndLevel = shouldEndLevel;
		}

		/// <summary>
		/// Can be null or empty
		/// </summary>
		public DTImmutableList<IEnemy> Enemies { get; private set; }

		/// <summary>
		/// Can be null or empty
		/// </summary>
		public DTImmutableList<Danmaku2Sound> Sounds { get; private set; }

		/// <summary>
		/// One-indexed (so a boss's last health bar has BossHealthMeterNumber = 1)
		/// </summary>
		public int? BossHealthMeterNumber { get; private set; }

		/// <summary>
		/// 100% is 128 * 1024
		/// </summary>
		public int? BossHealthMeterMilliPercentage { get; private set; }

		public bool ShouldEndLevel { get; private set; }
	}
}

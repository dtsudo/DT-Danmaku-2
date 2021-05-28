
namespace Danmaku2Lib
{
	public class GlobalState
	{
		public const int DEFAULT_VOLUME = 50;

		public GlobalState(
			int fps,
			IDTRandom nonGameLogicRng,
			GuidGenerator guidGenerator,
			bool isWebBrowserVersion,
			// Or null for default
			PlayerBulletSpreadLevel? initialPlayerBulletSpreadLevel,
			// Or null for default
			PlayerBulletStrength? initialPlayerBulletStrength,
			// Or null for default
			int? initialNumLives,
			bool debugMode,
			int? initialSoundVolume,
			int? initialMusicVolume)
		{
			this.Fps = fps;
			this.NonGameLogicRng = nonGameLogicRng;
			this.GuidGenerator = guidGenerator;
			this.IsWebBrowserVersion = isWebBrowserVersion;
			this.DebugMode = debugMode;
			this.SoundVolume = initialSoundVolume ?? GlobalState.DEFAULT_VOLUME;
			this.MusicVolume = initialMusicVolume ?? GlobalState.DEFAULT_VOLUME;
			this.BackgroundRenderer = new BackgroundRenderer(nonGameLogicRng: nonGameLogicRng, fps: fps);

			int elapsedMillisPerFrame = 1000 / fps;

			this.MusicPlayer = new MusicPlayer(elapsedMillisPerFrame: elapsedMillisPerFrame);
			this.ElapsedMillisPerFrame = elapsedMillisPerFrame;

			this.InitialPlayerBulletSpreadLevel = initialPlayerBulletSpreadLevel;
			this.InitialPlayerBulletStrength = initialPlayerBulletStrength;
			this.InitialNumLives = initialNumLives;
		}

		public PlayerBulletSpreadLevel? InitialPlayerBulletSpreadLevel { get; private set; }
		public PlayerBulletStrength? InitialPlayerBulletStrength { get; private set; }
		public int? InitialNumLives { get; private set; }

		public int Fps { get; private set; }
		public IDTRandom NonGameLogicRng { get; private set; }
		public GuidGenerator GuidGenerator { get; private set; }
		public bool IsWebBrowserVersion { get; private set; }
		public bool DebugMode { get; private set; }

		// Null means no default value specified
		public int SoundVolume { get; set; }

		// Null means no default value specified
		public int MusicVolume { get; set; }
		
		public BackgroundRenderer BackgroundRenderer { get; set; }

		public MusicPlayer MusicPlayer { get; private set; }

		public int ElapsedMillisPerFrame { get; private set; }

		public void ProcessMusic()
		{
			this.MusicPlayer.ProcessFrame();
		}

		public void RenderMusic(IDisplay<Danmaku2Assets> display)
		{
			this.MusicPlayer.RenderMusic(assets: display.GetAssets(), userVolume: this.MusicVolume);
		}
	}
}

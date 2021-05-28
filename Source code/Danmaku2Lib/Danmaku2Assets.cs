
namespace Danmaku2Lib
{
	using System;
	using System.Collections.Generic;

	public enum Danmaku2Image
	{
		TitleScreen,
		SoundOn,
		SoundOff,
		MusicOn,
		MusicOff,
		Arrow,
		Version,
		Credits,
		Programming,
		Images,
		Font,
		Sound,
		Music,
		CreditsProgrammingDesktopVersion,
		CreditsImages,
		CreditsFont,
		CreditsSound,
		CreditsMusic,
		Paused,
		ContinueReplay,
		GameOver,
		Continue,
		Start,
		Instructions,
		InstructionsLarge,
		HowToPlay,
		Back,
		Quit,
		ChooseDifficulty,
		Easy,
		Normal,
		Hard,
		Restart,
		YouWin,
		WatchReplay,
		Retry,
		PlayAgain,
		ReturnToTitle,
		TileGrass10,
		TileGrass11,
		TileGrass12,
		TileDirt06,
		TileDirt11,
		TileDirt15,
		PlayerShip,
		PlayerLifeIcon,
		PlayerShipInvulnerable,
		PowerUp,
		PlayerBullet1,
		PlayerBullet2,
		PlayerBullet3,
		Explosion1,
		Explosion2,
		Explosion3,
		Explosion4,
		Explosion5,
		Explosion6,
		Explosion7,
		Explosion8,
		Explosion9,
		SniperEnemyShip,
		SniperEnemyBullet,
		BasicEnemyShip,
		BasicEnemyBullet,
		MediumEnemyShip,
		MediumEnemyBullet1,
		MediumEnemyBullet2,
		Level1Midboss,
		Level1MidbossEnemyBullet,
		Level1MidbossEnemyAnticampingBullet,
		Level1MidbossOrbiterEnemyShip,
		Level1MidbossOrbiterEnemyBullet1,
		Level1MidbossOrbiterEnemyBullet2,
		Level1MidbossOrbiterEnemyBullet3,
		Level1MidbossOrbiterEnemyBullet4,
		Level1MidbossOrbiterEnemyBullet5,
		Level1Boss,
		Level1BossEnemyBullet1,
		Level1BossEnemyBullet2,
		Level1BossPhase1OrbiterEnemyShip,
		Level1BossPhase2OrbiterEnemyShip,
		Level1BossPhase1OrbiterEnemyBullet1,
		Level1BossPhase1OrbiterEnemyBullet2,
		Level1BossPhase1OrbiterEnemyBullet3,
		Level1BossPhase1OrbiterEnemyBullet4,
		Level1BossPhase1OrbiterEnemyBullet5,
		Level1BossPhase2OrbiterEnemyBullet1,
		Level1BossPhase2OrbiterEnemyBullet2,
		Level1BossPhase2OrbiterEnemyBullet3,
		Level1BossPhase2OrbiterEnemyBullet4,
		Level1BossPhase2OrbiterEnemyBullet5,
		CircleEnemyShip,
		CircleEnemyBullet,
		Line,
		CloseIcon
	}

	public enum Danmaku2Sound
	{
		PlayerShoot,
		StandardDeath,
		EnemyShoot,
		LargeShoot,
		ShootNoise, 
		Shoot23
	}

	public enum Danmaku2Music
	{
		Xeon6
	}

	public class ImageInfo
	{
		public ImageInfo(
			string imageName,
			int width,
			int height)
		{
			this.ImageName = imageName;
			this.Width = width;
			this.Height = height;
		}

		// e.g. "TitleScreen.png"
		public string ImageName { get; private set; }

		public int Width { get; private set; }

		public int Height { get; private set; }
	}

	public class SoundInfo
	{
		public SoundInfo(string soundName, int volume)
		{
			this.SoundName = soundName;
			this.Volume = volume;
		}

		// e.g. "PlayerShoot.ogg"
		public string SoundName { get; private set; }

		// From 0 to 100 (both inclusive)
		public int Volume { get; private set; }
	}

	public class MusicInfo
	{
		public MusicInfo(string musicName, int volume)
		{
			this.MusicName = musicName;
			this.Volume = volume;
		}

		// e.g. "Change.ogg"
		public string MusicName { get; private set; }

		// From 0 to 100 (both inclusive)
		public int Volume { get; private set; }
	}

	public abstract class Danmaku2Assets : IAssets
	{
		protected Dictionary<Danmaku2Image, ImageInfo> imageDictionary;
		protected Dictionary<Danmaku2Sound, SoundInfo> soundDictionary;
		protected Dictionary<Danmaku2Music, MusicInfo> musicDictionary;

		public Danmaku2Assets()
		{
			this.imageDictionary = new Dictionary<Danmaku2Image, ImageInfo>();

			this.imageDictionary.Add(
				Danmaku2Image.TitleScreen,
				new ImageInfo(
					imageName: "Metaflop/TitleScreen.png",
					width: 366,
					height: 152));
			this.imageDictionary.Add(
				Danmaku2Image.Arrow,
				new ImageInfo(
					imageName: "Other/Arrow.png",
					width: 37,
					height: 23));
			this.imageDictionary.Add(
				Danmaku2Image.SoundOn,
				new ImageInfo(
					imageName: "Kenney/SoundOn.png",
					width: 50,
					height: 50));
			this.imageDictionary.Add(
				Danmaku2Image.SoundOff,
				new ImageInfo(
					imageName: "Kenney/SoundOff.png",
					width: 50,
					height: 50));
			this.imageDictionary.Add(
				Danmaku2Image.MusicOn,
				new ImageInfo(
					imageName: "Kenney/MusicOn.png",
					width: 50,
					height: 50));
			this.imageDictionary.Add(
				Danmaku2Image.MusicOff,
				new ImageInfo(
					imageName: "Kenney/MusicOff.png",
					width: 50,
					height: 50));
			this.imageDictionary.Add(
				Danmaku2Image.Version,
				new ImageInfo(
					imageName: "Metaflop/Version.png",
					width: 47,
					height: 16));
			this.imageDictionary.Add(
				Danmaku2Image.Credits,
				new ImageInfo(
					imageName: "Metaflop/Credits.png",
					width: 66,
					height: 16));
			this.imageDictionary.Add(
				Danmaku2Image.Programming,
				new ImageInfo(
					imageName: "Metaflop/Programming.png",
					width: 106,
					height: 18));
			this.imageDictionary.Add(
				Danmaku2Image.Images,
				new ImageInfo(
					imageName: "Metaflop/Images.png",
					width: 57,
					height: 18));
			this.imageDictionary.Add(
				Danmaku2Image.Font,
				new ImageInfo(
					imageName: "Metaflop/Font.png",
					width: 32,
					height: 14));
			this.imageDictionary.Add(
				Danmaku2Image.Sound,
				new ImageInfo(
					imageName: "Metaflop/Sound.png",
					width: 48,
					height: 14));
			this.imageDictionary.Add(
				Danmaku2Image.Music,
				new ImageInfo(
					imageName: "Metaflop/Music.png",
					width: 48,
					height: 14));
			this.imageDictionary.Add(
				Danmaku2Image.CreditsProgrammingDesktopVersion,
				new ImageInfo(
					imageName: "Metaflop/CreditsProgrammingDesktopVersion.png",
					width: 388,
					height: 389));
			this.imageDictionary.Add(
				Danmaku2Image.CreditsImages,
				new ImageInfo(
					imageName: "Metaflop/CreditsImages.png",
					width: 364,
					height: 129));
			this.imageDictionary.Add(
				Danmaku2Image.CreditsFont,
				new ImageInfo(
					imageName: "Metaflop/CreditsFont.png",
					width: 365,
					height: 157));
			this.imageDictionary.Add(
				Danmaku2Image.CreditsSound,
				new ImageInfo(
					imageName: "Metaflop/CreditsSound.png",
					width: 372,
					height: 550));
			this.imageDictionary.Add(
				Danmaku2Image.CreditsMusic,
				new ImageInfo(
					imageName: "Metaflop/CreditsMusic.png",
					width: 379,
					height: 245));
			this.imageDictionary.Add(
				Danmaku2Image.Paused,
				new ImageInfo(
					imageName: "Metaflop/Paused.png",
					width: 183,
					height: 46));
			this.imageDictionary.Add(
				Danmaku2Image.GameOver,
				new ImageInfo(
					imageName: "Metaflop/GameOver.png",
					width: 307,
					height: 44));
			this.imageDictionary.Add(
				Danmaku2Image.Continue,
				new ImageInfo(
					imageName: "Metaflop/Continue.png",
					width: 117,
					height: 23));
			this.imageDictionary.Add(
				Danmaku2Image.Start,
				new ImageInfo(
					imageName: "Metaflop/Start.png",
					width: 60,
					height: 23));
			this.imageDictionary.Add(
				Danmaku2Image.Instructions,
				new ImageInfo(
					imageName: "Metaflop/Instructions.png",
					width: 146,
					height: 23));
			this.imageDictionary.Add(
				Danmaku2Image.InstructionsLarge,
				new ImageInfo(
					imageName: "Metaflop/Instructions_Large.png",
					width: 197,
					height: 31));
			this.imageDictionary.Add(
				Danmaku2Image.HowToPlay,
				new ImageInfo(
					imageName: "Metaflop/HowToPlay.png",
					width: 460,
					height: 228));
			this.imageDictionary.Add(
				Danmaku2Image.Back,
				new ImageInfo(
					imageName: "Metaflop/Back.png",
					width: 61,
					height: 23));
			this.imageDictionary.Add(
				Danmaku2Image.Quit,
				new ImageInfo(
					imageName: "Metaflop/Quit.png",
					width: 53,
					height: 23));
			this.imageDictionary.Add(
				Danmaku2Image.ChooseDifficulty,
				new ImageInfo(
					imageName: "Metaflop/ChooseDifficulty.png",
					width: 298,
					height: 40));
			this.imageDictionary.Add(
				Danmaku2Image.Easy,
				new ImageInfo(
					imageName: "Metaflop/Easy.png",
					width: 56,
					height: 29));
			this.imageDictionary.Add(
				Danmaku2Image.Normal,
				new ImageInfo(
					imageName: "Metaflop/Normal.png",
					width: 90,
					height: 23));
			this.imageDictionary.Add(
				Danmaku2Image.Hard,
				new ImageInfo(
					imageName: "Metaflop/Hard.png",
					width: 58,
					height: 23));
			this.imageDictionary.Add(
				Danmaku2Image.Restart,
				new ImageInfo(
					imageName: "Metaflop/Restart.png",
					width: 89,
					height: 23));
			this.imageDictionary.Add(
				Danmaku2Image.ContinueReplay,
				new ImageInfo(
					imageName: "Metaflop/ContinueReplay.png",
					width: 207,
					height: 30));
			this.imageDictionary.Add(
				Danmaku2Image.YouWin,
				new ImageInfo(
					imageName: "Metaflop/YouWin.png",
					width: 224,
					height: 44));
			this.imageDictionary.Add(
				Danmaku2Image.WatchReplay,
				new ImageInfo(
					imageName: "Metaflop/WatchReplay.png",
					width: 170,
					height: 30));
			this.imageDictionary.Add(
				Danmaku2Image.Retry,
				new ImageInfo(
					imageName: "Metaflop/Retry.png",
					width: 65,
					height: 30));
			this.imageDictionary.Add(
				Danmaku2Image.PlayAgain,
				new ImageInfo(
					imageName: "Metaflop/PlayAgain.png",
					width: 130,
					height: 30));
			this.imageDictionary.Add(
				Danmaku2Image.ReturnToTitle,
				new ImageInfo(
					imageName: "Metaflop/ReturnToTitle.png",
					width: 174,
					height: 23));
			this.imageDictionary.Add(
				Danmaku2Image.TileGrass10,
				new ImageInfo(
					imageName: "Kenney/grass_10.png",
					width: 120,
					height: 140));
			this.imageDictionary.Add(
				Danmaku2Image.TileGrass11,
				new ImageInfo(
					imageName: "Kenney/grass_11.png",
					width: 120,
					height: 140));
			this.imageDictionary.Add(
				Danmaku2Image.TileGrass12,
				new ImageInfo(
					imageName: "Kenney/grass_12.png",
					width: 120,
					height: 140));
			this.imageDictionary.Add(
				Danmaku2Image.TileDirt06,
				new ImageInfo(
					imageName: "Kenney/dirt_06.png",
					width: 120,
					height: 140));
			this.imageDictionary.Add(
				Danmaku2Image.TileDirt11,
				new ImageInfo(
					imageName: "Kenney/dirt_11.png",
					width: 120,
					height: 140));
			this.imageDictionary.Add(
				Danmaku2Image.TileDirt15,
				new ImageInfo(
					imageName: "Kenney/dirt_15.png",
					width: 120,
					height: 140));
			this.imageDictionary.Add(
				Danmaku2Image.PlayerShip,
				new ImageInfo(
					imageName: "Kenney/PlayerShip.png",
					width: 50,
					height: 38));
			this.imageDictionary.Add(
				Danmaku2Image.PlayerLifeIcon,
				new ImageInfo(
					imageName: "Kenney/PlayerLifeIcon.png",
					width: 25,
					height: 20));
			this.imageDictionary.Add(
				Danmaku2Image.PlayerShipInvulnerable,
				new ImageInfo(
					imageName: "Kenney/PlayerShipInvulnerable.png",
					width: 50,
					height: 38));
			this.imageDictionary.Add(
				Danmaku2Image.PowerUp,
				new ImageInfo(
					imageName: "Kenney/PowerUp.png",
					width: 34,
					height: 33));
			this.imageDictionary.Add(
				Danmaku2Image.PlayerBullet1,
				new ImageInfo(
					imageName: "Kenney/PlayerBullet1.png",
					width: 5,
					height: 27));
			this.imageDictionary.Add(
				Danmaku2Image.PlayerBullet2,
				new ImageInfo(
					imageName: "Kenney/PlayerBullet2.png",
					width: 5,
					height: 27));
			this.imageDictionary.Add(
				Danmaku2Image.PlayerBullet3,
				new ImageInfo(
					imageName: "Kenney/PlayerBullet3.png",
					width: 5,
					height: 27));
			this.imageDictionary.Add(
				Danmaku2Image.Explosion1,
				new ImageInfo(
					imageName: "Kenney/Explosion1.png",
					width: 96,
					height: 96));
			this.imageDictionary.Add(
				Danmaku2Image.Explosion2,
				new ImageInfo(
					imageName: "Kenney/Explosion2.png",
					width: 76,
					height: 75));
			this.imageDictionary.Add(
				Danmaku2Image.Explosion3,
				new ImageInfo(
					imageName: "Kenney/Explosion3.png",
					width: 41,
					height: 46));
			this.imageDictionary.Add(
				Danmaku2Image.Explosion4,
				new ImageInfo(
					imageName: "Kenney/Explosion4.png",
					width: 46,
					height: 51));
			this.imageDictionary.Add(
				Danmaku2Image.Explosion5,
				new ImageInfo(
					imageName: "Kenney/Explosion5.png",
					width: 60,
					height: 62));
			this.imageDictionary.Add(
				Danmaku2Image.Explosion6,
				new ImageInfo(
					imageName: "Kenney/Explosion6.png",
					width: 67,
					height: 67));
			this.imageDictionary.Add(
				Danmaku2Image.Explosion7,
				new ImageInfo(
					imageName: "Kenney/Explosion7.png",
					width: 69,
					height: 70));
			this.imageDictionary.Add(
				Danmaku2Image.Explosion8,
				new ImageInfo(
					imageName: "Kenney/Explosion8.png",
					width: 72,
					height: 72));
			this.imageDictionary.Add(
				Danmaku2Image.Explosion9,
				new ImageInfo(
					imageName: "Kenney/Explosion9.png",
					width: 75,
					height: 76));
			this.imageDictionary.Add(
				Danmaku2Image.SniperEnemyShip,
				new ImageInfo(
					imageName: "Kenney/SniperEnemyShip.png",
					width: 52,
					height: 42));
			this.imageDictionary.Add(
				Danmaku2Image.SniperEnemyBullet,
				new ImageInfo(
					imageName: "Kenney/SniperEnemyBullet.png",
					width: 32,
					height: 30));
			this.imageDictionary.Add(
				Danmaku2Image.BasicEnemyShip,
				new ImageInfo(
					imageName: "Kenney/BasicEnemyShip.png",
					width: 50,
					height: 45));
			this.imageDictionary.Add(
				Danmaku2Image.BasicEnemyBullet,
				new ImageInfo(
					imageName: "Kenney/BasicEnemyBullet.png",
					width: 9,
					height: 9));
			this.imageDictionary.Add(
				Danmaku2Image.MediumEnemyShip,
				new ImageInfo(
					imageName: "Kenney/MediumEnemyShip.png",
					width: 143,
					height: 147));
			this.imageDictionary.Add(
				Danmaku2Image.MediumEnemyBullet1,
				new ImageInfo(
					imageName: "Kenney/MediumEnemyBullet1.png",
					width: 9,
					height: 17));
			this.imageDictionary.Add(
				Danmaku2Image.MediumEnemyBullet2,
				new ImageInfo(
					imageName: "Kenney/MediumEnemyBullet2.png",
					width: 9,
					height: 17));
			this.imageDictionary.Add(
				Danmaku2Image.Level1Midboss,
				new ImageInfo(
					imageName: "Kenney/Level1Midboss.png",
					width: 208,
					height: 168));
			this.imageDictionary.Add(
				Danmaku2Image.Level1MidbossEnemyBullet,
				new ImageInfo(
					imageName: "Kenney/Level1MidbossEnemyBullet.png",
					width: 9,
					height: 17));
			this.imageDictionary.Add(
				Danmaku2Image.Level1MidbossEnemyAnticampingBullet,
				new ImageInfo(
					imageName: "Kenney/Level1MidbossEnemyAnticampingBullet.png",
					width: 9,
					height: 9));
			this.imageDictionary.Add(
				Danmaku2Image.Level1MidbossOrbiterEnemyShip,
				new ImageInfo(
					imageName: "Kenney/Level1MidbossOrbiterEnemyShip.png",
					width: 50,
					height: 45));
			this.imageDictionary.Add(
				Danmaku2Image.Level1MidbossOrbiterEnemyBullet1,
				new ImageInfo(
					imageName: "Kenney/Level1MidbossOrbiterEnemyBullet1.png",
					width: 9,
					height: 9));
			this.imageDictionary.Add(
				Danmaku2Image.Level1MidbossOrbiterEnemyBullet2,
				new ImageInfo(
					imageName: "Kenney/Level1MidbossOrbiterEnemyBullet2.png",
					width: 9,
					height: 9));
			this.imageDictionary.Add(
				Danmaku2Image.Level1MidbossOrbiterEnemyBullet3,
				new ImageInfo(
					imageName: "Kenney/Level1MidbossOrbiterEnemyBullet3.png",
					width: 9,
					height: 9));
			this.imageDictionary.Add(
				Danmaku2Image.Level1MidbossOrbiterEnemyBullet4,
				new ImageInfo(
					imageName: "Kenney/Level1MidbossOrbiterEnemyBullet4.png",
					width: 9,
					height: 9));
			this.imageDictionary.Add(
				Danmaku2Image.Level1MidbossOrbiterEnemyBullet5,
				new ImageInfo(
					imageName: "Kenney/Level1MidbossOrbiterEnemyBullet5.png",
					width: 9,
					height: 9));
			this.imageDictionary.Add(
				Danmaku2Image.CircleEnemyShip,
				new ImageInfo(
					imageName: "Kenney/CircleEnemyShip.png",
					width: 49,
					height: 42));
			this.imageDictionary.Add(
				Danmaku2Image.CircleEnemyBullet,
				new ImageInfo(
					imageName: "Kenney/CircleEnemyBullet.png",
					width: 9,
					height: 20));
			this.imageDictionary.Add(
				Danmaku2Image.Level1Boss,
				new ImageInfo(
					imageName: "Kenney/Level1Boss.png",
					width: 208,
					height: 168));
			this.imageDictionary.Add(
				Danmaku2Image.Level1BossEnemyBullet1,
				new ImageInfo(
					imageName: "Kenney/Level1BossEnemyBullet1.png",
					width: 9,
					height: 17));
			this.imageDictionary.Add(
				Danmaku2Image.Level1BossEnemyBullet2,
				new ImageInfo(
					imageName: "Kenney/Level1BossEnemyBullet2.png",
					width: 9,
					height: 17));
			this.imageDictionary.Add(
				Danmaku2Image.Level1BossPhase1OrbiterEnemyShip,
				new ImageInfo(
					imageName: "Kenney/Level1BossPhase1OrbiterEnemyShip.png",
					width: 52,
					height: 42));
			this.imageDictionary.Add(
				Danmaku2Image.Level1BossPhase2OrbiterEnemyShip,
				new ImageInfo(
					imageName: "Kenney/Level1BossPhase2OrbiterEnemyShip.png",
					width: 49,
					height: 42));
			this.imageDictionary.Add(
				Danmaku2Image.Line,
				new ImageInfo(
					imageName: "Other/Line.bmp",
					width: 5,
					height: 20));
			this.imageDictionary.Add(
				Danmaku2Image.Level1BossPhase1OrbiterEnemyBullet1,
				new ImageInfo(
					imageName: "Kenney/Level1BossPhase1OrbiterEnemyBullet1.png",
					width: 32,
					height: 30));
			this.imageDictionary.Add(
				Danmaku2Image.Level1BossPhase1OrbiterEnemyBullet2,
				new ImageInfo(
					imageName: "Kenney/Level1BossPhase1OrbiterEnemyBullet2.png",
					width: 32,
					height: 30));
			this.imageDictionary.Add(
				Danmaku2Image.Level1BossPhase1OrbiterEnemyBullet3,
				new ImageInfo(
					imageName: "Kenney/Level1BossPhase1OrbiterEnemyBullet3.png",
					width: 32,
					height: 30));
			this.imageDictionary.Add(
				Danmaku2Image.Level1BossPhase1OrbiterEnemyBullet4,
				new ImageInfo(
					imageName: "Kenney/Level1BossPhase1OrbiterEnemyBullet4.png",
					width: 32,
					height: 30));
			this.imageDictionary.Add(
				Danmaku2Image.Level1BossPhase1OrbiterEnemyBullet5,
				new ImageInfo(
					imageName: "Kenney/Level1BossPhase1OrbiterEnemyBullet5.png",
					width: 32,
					height: 30));
			this.imageDictionary.Add(
				Danmaku2Image.Level1BossPhase2OrbiterEnemyBullet1,
				new ImageInfo(
					imageName: "Kenney/Level1BossPhase2OrbiterEnemyBullet1.png",
					width: 9,
					height: 20));
			this.imageDictionary.Add(
				Danmaku2Image.Level1BossPhase2OrbiterEnemyBullet2,
				new ImageInfo(
					imageName: "Kenney/Level1BossPhase2OrbiterEnemyBullet2.png",
					width: 9,
					height: 20));
			this.imageDictionary.Add(
				Danmaku2Image.Level1BossPhase2OrbiterEnemyBullet3,
				new ImageInfo(
					imageName: "Kenney/Level1BossPhase2OrbiterEnemyBullet3.png",
					width: 9,
					height: 20));
			this.imageDictionary.Add(
				Danmaku2Image.Level1BossPhase2OrbiterEnemyBullet4,
				new ImageInfo(
					imageName: "Kenney/Level1BossPhase2OrbiterEnemyBullet4.png",
					width: 9,
					height: 20));
			this.imageDictionary.Add(
				Danmaku2Image.Level1BossPhase2OrbiterEnemyBullet5,
				new ImageInfo(
					imageName: "Kenney/Level1BossPhase2OrbiterEnemyBullet5.png",
					width: 9,
					height: 20));
			this.imageDictionary.Add(
				Danmaku2Image.CloseIcon,
				new ImageInfo(
					imageName: "Kenney/cross.png",
					width: 50,
					height: 50));
						
			this.soundDictionary = new Dictionary<Danmaku2Sound, SoundInfo>();

			this.soundDictionary.Add(
				Danmaku2Sound.PlayerShoot,
				new SoundInfo(
					soundName: "Kenney/PlayerShoot_Modified.ogg",
					volume: 20));
			this.soundDictionary.Add(
				Danmaku2Sound.StandardDeath,
				new SoundInfo(
					soundName: "Kenney/StandardDeath.ogg",
					volume: 20));
			this.soundDictionary.Add(
				Danmaku2Sound.EnemyShoot,
				new SoundInfo(
					soundName: "Kenney/EnemyShoot_Modified.ogg",
					volume: 20));
			this.soundDictionary.Add(
				Danmaku2Sound.LargeShoot,
				new SoundInfo(
					soundName: "jeckkech/projectile_Modified.ogg",
					volume: 100));
			this.soundDictionary.Add(
				Danmaku2Sound.ShootNoise,
				new SoundInfo(
					soundName: "josepharaoh99/shot_noise.ogg",
					volume: 60));
			this.soundDictionary.Add(
				Danmaku2Sound.Shoot23,
				new SoundInfo(
					soundName: "jalastram/Shooting_Sounds_023_Modified.ogg",
					volume: 20));

			this.musicDictionary = new Dictionary<Danmaku2Music, MusicInfo>();
			
			this.musicDictionary.Add(
				Danmaku2Music.Xeon6,
				new MusicInfo(
					musicName: "bart/xeon6.ogg",
					volume: 40));
		}

		/// <summary>
		/// Once this.DisposeImages() is invoked, this function can no longer be called.
		/// </summary>
		public abstract void DrawInitialLoadingScreen();

		/// <summary>
		/// Must be repeatedly invoked until it returns true before invoking DrawImage(), DrawImageRotatedCounterclockwise(),
		/// GetWidth(), GetHeight(), or DrawCharacter()
		/// </summary>
		public abstract bool LoadImages();

		public void DrawImage(Danmaku2Image image, int x, int y)
		{
			this.DrawImageRotatedCounterclockwise(image: image, x: x, y: y, degreesScaled: 0, scalingFactorScaled: 128);
		}

		// Degrees = DegreesScaled / 128.0
		public void DrawImageRotatedCounterclockwise(Danmaku2Image image, int x, int y, int degreesScaled)
		{
			this.DrawImageRotatedCounterclockwise(
				image: image,
				x: x,
				y: y,
				degreesScaled: degreesScaled,
				scalingFactorScaled: 128);
		}

		// Degrees = DegreesScaled / 128.0
		// ScalingFactor = scalingFactorScaled / 128.0
		public abstract void DrawImageRotatedCounterclockwise(Danmaku2Image image, int x, int y, int degreesScaled, int scalingFactorScaled);
		
		public int GetWidth(Danmaku2Image image)
		{
			return this.imageDictionary[image].Width;
		}

		public int GetHeight(Danmaku2Image image)
		{
			return this.imageDictionary[image].Height;
		}
		
		/// <summary>
		/// Note that DisposeImages() must be called, and it gets called even
		/// if LoadImages() is never invoked (or was invoked but never returned true)
		/// 
		/// This function must be idempotent (and not fail if called multiple times).
		/// </summary>
		public abstract void DisposeImages();

		/// <summary>
		/// Must be repeatedly invoked until it returns true before invoking PlaySound()
		/// </summary>
		public abstract bool LoadSounds();

		/// <summary>
		/// Plays the specified sound.
		/// Volume ranges from 0 to 100 (both inclusive)
		/// </summary>
		public abstract void PlaySound(Danmaku2Sound sound, int volume);

		/// <summary>
		/// Note that DisposeSounds() must be called, and it gets called even
		/// if LoadSounds() is never invoked (or was invoked but never returned true)
		/// 
		/// This function must be idempotent (and not fail if called multiple times).
		/// </summary>
		public abstract void DisposeSounds();

		/// <summary>
		/// Must be repeatedly invoked until it returns true before invoking PlayMusic() or StopMusic()
		/// </summary>
		public abstract bool LoadMusic();

		/// <summary>
		/// Volume ranges from 0 to 100 (both inclusive)
		/// </summary>
		public abstract void PlayMusic(Danmaku2Music music, int volume);

		public abstract void StopMusic();

		/// <summary>
		/// Note that DisposeMusic() must be called, and it gets called even
		/// if LoadMusic() is never invoked (or was invoked but never returned true)
		/// 
		/// This function must be idempotent (and not fail if called multiple times).
		/// </summary>
		public abstract void DisposeMusic();
	}
}

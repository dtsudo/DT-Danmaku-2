
namespace Danmaku2
{
	using AgateLib.AudioLib;
	using AgateLib.DisplayLib;
	using AgateLib.Geometry;
	using Danmaku2Lib;
	using System;
	using System.Collections.Generic;
	using System.IO;
	
	public abstract class AgateLibDisplay<T> : IDisplay<T> where T : class, IAssets
	{
		private FontSurface debugFontSurface;

		public AgateLibDisplay()
		{
			this.debugFontSurface = null;
		}

		public void DrawRectangle(int x, int y, int width, int height, DTColor color, bool fill)
		{
			Color agateLibColor = Color.FromArgb(color.Alpha, color.R, color.G, color.B);

			if (fill)
			{
				Display.FillRect(x, y, width, height, agateLibColor);
			}
			else
			{
				// Unclear why the "- 1" are necessary, but it seems to make the display render better.
				Display.DrawRect(x, y, width - 1, height - 1, agateLibColor);
			}
		}

		public void DebugPrint(int x, int y, string debugText)
		{
			if (debugFontSurface == null)
			{
				debugFontSurface = new FontSurface("Arial", 12);
				debugFontSurface.Color = Color.White;
			}
			debugFontSurface.DrawText(destX: x, destY: y, text: debugText);
		}

		public abstract T GetAssets();
	}

	public class Danmaku2AgateLibDisplay : AgateLibDisplay<Danmaku2Assets>
	{
		private AgateLibDanmaku2Assets assets;
		
		private class AgateLibDanmaku2Assets : Danmaku2Assets
		{
			private Surface loadingImage;

			private Dictionary<Danmaku2Image, Surface> imageToSurfaceMapping;
			private Dictionary<Danmaku2Sound, SoundBuffer> soundToSoundBufferMapping;
			private Dictionary<Danmaku2Music, Music> musicMapping;
			private List<SoundBuffer> disposedSoundBuffers;
			private List<Music> disposedMusic;

			private bool canSuccessfullyPlayAudio;

			private static string GetImagesDirectory()
			{
				string path = Util.GetExecutablePath();

				// The images are expected to be in the /Data/Images/ folder
				// relative to the executable.
				if (Directory.Exists(path + "/Data/Images"))
					return path + "/Data/Images" + "/";

				// However, if the folder doesn't exist, search for the /Data/Images folder
				// using some heuristic.
				while (true)
				{
					int i = Math.Max(path.LastIndexOf("/", StringComparison.Ordinal), path.LastIndexOf("\\", StringComparison.Ordinal));

					if (i == -1)
						throw new Exception("Cannot find images directory");

					path = path.Substring(0, i);

					if (Directory.Exists(path + "/Data/Images"))
						return path + "/Data/Images" + "/";
				}
			}
			
			private static string GetAudioDirectory()
			{
				string path = Util.GetExecutablePath();

				// The images are expected to be in the /Data/Audio/ folder
				// relative to the executable.
				if (Directory.Exists(path + "/Data/Audio"))
					return path + "/Data/Audio" + "/";

				// However, if the folder doesn't exist, search for the /Data/Audio folder
				// using some heuristic.
				while (true)
				{
					int i = Math.Max(path.LastIndexOf("/", StringComparison.Ordinal), path.LastIndexOf("\\", StringComparison.Ordinal));

					if (i == -1)
						throw new Exception("Cannot find audio directory");

					path = path.Substring(0, i);

					if (Directory.Exists(path + "/Data/Audio"))
						return path + "/Data/Audio" + "/";
				}
			}

			public AgateLibDanmaku2Assets(bool canSuccessfullyPlayAudio)
			{
				this.canSuccessfullyPlayAudio = canSuccessfullyPlayAudio;

				string imagesDirectory = GetImagesDirectory();

				this.loadingImage = new Surface(imagesDirectory + "Metaflop/LoadingImage.png");
				
				this.imageToSurfaceMapping = new Dictionary<Danmaku2Image, Surface>();
				this.soundToSoundBufferMapping = new Dictionary<Danmaku2Sound, SoundBuffer>();
				this.musicMapping = new Dictionary<Danmaku2Music, Music>();
				this.disposedSoundBuffers = new List<SoundBuffer>();
				this.disposedMusic = new List<Music>();
			}

			private Surface GetSurface(Danmaku2Image image)
			{
				return this.imageToSurfaceMapping[image];
			}

			public override void DrawInitialLoadingScreen()
			{
				int x = 0;
				int y = 0;
				// Unclear why the "- 1" is necessary, but it seems to make the image render better.
				this.loadingImage.Draw(x - 1, y);
			}
			
			public override bool LoadImages()
			{
				string imagesDirectory = GetImagesDirectory();

				foreach (Danmaku2Image image in this.imageDictionary.Keys)
				{
					if (!this.imageToSurfaceMapping.ContainsKey(image))
					{
						string filename = this.imageDictionary[image].ImageName;
						this.imageToSurfaceMapping.Add(image, new Surface(imagesDirectory + filename));
						return false;
					}
				}
				
				return true;
			}

			public override void DisposeImages()
			{
				if (!this.loadingImage.IsDisposed)
					this.loadingImage.Dispose();

				foreach (Surface surface in this.imageToSurfaceMapping.Values)
				{
					if (!surface.IsDisposed)
					{
						surface.Dispose();
					}
				}
			}

			public override void DrawImageRotatedCounterclockwise(Danmaku2Image image, int x, int y, int degreesScaled, int scalingFactorScaled)
			{
				Surface surface = this.GetSurface(image);
				surface.RotationCenter = OriginAlignment.Center;
				surface.RotationAngle = degreesScaled / 128.0 * 2.0 * Math.PI / 360.0;

				double scalingFactor = scalingFactorScaled / 128.0;

				surface.SetScale(
					width: scalingFactor,
					height: scalingFactor);

				// Unclear why the "- 1" is necessary, but it seems to make the image render better.
				surface.Draw(x - 1, y);
			}
			
			public override bool LoadSounds()
			{
				if (!this.canSuccessfullyPlayAudio)
					return true;
				
				string audioDirectory = GetAudioDirectory();

				foreach (Danmaku2Sound sound in this.soundDictionary.Keys)
				{
					if (!this.soundToSoundBufferMapping.ContainsKey(sound))
					{
						string filename = this.soundDictionary[sound].SoundName;
						this.soundToSoundBufferMapping.Add(sound, new SoundBuffer(audioDirectory + filename));
						return false;
					}
				}

				return true;
			}

			public override void PlaySound(Danmaku2Sound sound, int volume)
			{
				if (!this.canSuccessfullyPlayAudio)
					return;

				double finalVolume = (this.soundDictionary[sound].Volume / 100.0) * (volume / 100.0);
				if (finalVolume > 1.0)
					finalVolume = 1.0;
				if (finalVolume < 0.0)
					finalVolume = 0.0;
				if (finalVolume > 0.0)
				{
					SoundBuffer soundBuffer = this.soundToSoundBufferMapping[sound];
					soundBuffer.Volume = finalVolume;
					soundBuffer.Play();
				}
			}

			public override void DisposeSounds()
			{
				if (!this.canSuccessfullyPlayAudio)
					return;
				
				foreach (SoundBuffer soundBuffer in this.soundToSoundBufferMapping.Values)
				{
					bool alreadyDisposed = false;

					foreach (SoundBuffer disposedSoundBuffer in this.disposedSoundBuffers)
					{
						if (disposedSoundBuffer == soundBuffer)
							alreadyDisposed = true;
					}

					if (!alreadyDisposed)
					{
						soundBuffer.Stop();
						soundBuffer.Dispose();

						this.disposedSoundBuffers.Add(soundBuffer);
					}
				}
			}

			public override bool LoadMusic()
			{
				if (!this.canSuccessfullyPlayAudio)
					return true;

				string audioDirectory = GetAudioDirectory();

				foreach (Danmaku2Music music in this.musicDictionary.Keys)
				{
					if (!this.musicMapping.ContainsKey(music))
					{
						string filename = this.musicDictionary[music].MusicName;
						Music musicObj = new Music(audioDirectory + filename);
						musicObj.IsLooping = true;
						this.musicMapping.Add(music, musicObj);
						return false;
					}
				}

				return true;
			}

			public override void PlayMusic(Danmaku2Music music, int volume)
			{
				if (!this.canSuccessfullyPlayAudio)
					return;

				Music m = this.musicMapping[music];

				foreach (Music otherMusic in this.musicMapping.Values)
				{
					if (otherMusic != m && otherMusic.IsPlaying)
					{
						/*
							For some reason, this isn't necessary and also doesn't work.
							otherMusic.IsPlaying appears to be true if any music is playing,
							and invoking Stop() causes whatever music is currently playing to stop.
						*/
						//otherMusic.Stop();
					}
				}

				double finalVolume = (this.musicDictionary[music].Volume / 100.0) * (volume / 100.0);
				if (finalVolume > 1.0)
					finalVolume = 1.0;
				if (finalVolume < 0.0)
					finalVolume = 0.0;
				m.Volume = finalVolume;

				if (!m.IsPlaying)
				{
					m.Play();
				}
			}

			public override void StopMusic()
			{
				if (!this.canSuccessfullyPlayAudio)
					return;

				foreach (Music music in this.musicMapping.Values)
				{
					if (music.IsPlaying)
						music.Stop();
				}
			}

			public override void DisposeMusic()
			{
				if (!this.canSuccessfullyPlayAudio)
					return;
				
				foreach (Music music in this.musicMapping.Values)
				{
					bool alreadyDisposed = false;

					foreach (Music disposedMusic in this.disposedMusic)
					{
						if (disposedMusic == music)
							alreadyDisposed = true;
					}

					if (!alreadyDisposed)
					{
						music.Stop();
						music.Dispose();

						this.disposedMusic.Add(music);
					}
				}
			}
		}

		public Danmaku2AgateLibDisplay(bool canSuccessfullyPlayAudio)
		{
			this.assets = new AgateLibDanmaku2Assets(canSuccessfullyPlayAudio: canSuccessfullyPlayAudio);
		}

		public override Danmaku2Assets GetAssets()
		{
			return this.assets;
		}
	}
}

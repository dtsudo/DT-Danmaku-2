
namespace Danmaku2Lib
{
	public class MusicPlayer
	{
		/// <summary>
		/// The current music being played, or null if no music is playing.
		/// 
		/// This may not be the same as intendedMusic since it takes a while
		/// to fade out an existing music and fade in a new one
		/// </summary>
		private Danmaku2Music? currentMusic;

		/// <summary>
		/// The intended music that should eventually play, or null if we should fade out all music
		/// </summary>
		private Danmaku2Music? intendedMusic;
		
		/// <summary>
		/// From 0 to 100 * 1024 (both inclusive)
		/// 
		/// Normally, this value is 100 * 1024.
		/// However, when fading in/out, this value will decrease to represent the drop in music volume.
		/// </summary>
		private int currentFadeInAndOutVolumeMillis;
		
		/// <summary>
		/// From 0 to 100.
		/// 
		/// For this.currentMusic, the intended volume at which the music should be played.
		/// We allow this to be set since we might want to play a particular music at a different
		/// volume depending on circumstances (e.g. maybe the music should be played softer when
		/// the game is paused)
		/// </summary>
		private int currentMusicVolume;

		/// <summary>
		/// From 0 to 100.
		/// 
		/// For this.intendedMusic, the intended volume at which the music should be played.
		/// </summary>
		private int intendedMusicVolume;
		
		private int elapsedMillisPerFrame;

		public MusicPlayer(int elapsedMillisPerFrame)
		{
			this.currentMusic = null;
			this.intendedMusic = null;
			this.currentFadeInAndOutVolumeMillis = 0;
			this.currentMusicVolume = 0;
			this.intendedMusicVolume = 0;

			this.elapsedMillisPerFrame = elapsedMillisPerFrame;
		}

		private void DecreaseCurrentFadeInAndOutVolumeMillis()
		{
			this.currentFadeInAndOutVolumeMillis = this.currentFadeInAndOutVolumeMillis - this.elapsedMillisPerFrame * 100;
			if (this.currentFadeInAndOutVolumeMillis < 0)
				this.currentFadeInAndOutVolumeMillis = 0;
		}

		private void IncreaseCurrentFadeInAndOutVolumeMillis()
		{
			this.currentFadeInAndOutVolumeMillis = this.currentFadeInAndOutVolumeMillis + this.elapsedMillisPerFrame * 100;
			if (this.currentFadeInAndOutVolumeMillis > 100 * 1024)
				this.currentFadeInAndOutVolumeMillis = 100 * 1024;
		}

		public void ProcessFrame()
		{
			if (this.intendedMusic == null)
			{
				if (this.currentMusic != null)
				{
					this.DecreaseCurrentFadeInAndOutVolumeMillis();
					if (this.currentFadeInAndOutVolumeMillis == 0)
						this.currentMusic = null;
				}

				return;
			}

			if (this.currentMusic == null)
			{
				this.currentMusic = this.intendedMusic;
				this.currentFadeInAndOutVolumeMillis = 0;
				this.currentMusicVolume = this.intendedMusicVolume;
				return;
			}

			if (this.currentMusic.Value != this.intendedMusic.Value)
			{
				this.DecreaseCurrentFadeInAndOutVolumeMillis();
				if (this.currentFadeInAndOutVolumeMillis == 0)
					this.currentMusic = null;
				return;
			}

			if (this.currentMusicVolume < this.intendedMusicVolume)
			{
				this.currentMusicVolume = this.currentMusicVolume + this.elapsedMillisPerFrame / 5;
				if (this.currentMusicVolume > this.intendedMusicVolume)
					this.currentMusicVolume = this.intendedMusicVolume;
			}

			if (this.currentMusicVolume > this.intendedMusicVolume)
			{
				this.currentMusicVolume = this.currentMusicVolume - this.elapsedMillisPerFrame / 5;
				if (this.currentMusicVolume < this.intendedMusicVolume)
					this.currentMusicVolume = this.intendedMusicVolume;
			}

			this.IncreaseCurrentFadeInAndOutVolumeMillis();
		}

		public void SetMusic(Danmaku2Music music, int volume)
		{
			this.intendedMusic = music;
			this.intendedMusicVolume = volume;
		}

		public void StopMusic()
		{
			this.intendedMusic = null;
		}
		
		public void RenderMusic(
			Danmaku2Assets assets,
			// From 0 to 100
			int userVolume)
		{
			if (this.currentMusic != null)
				assets.PlayMusic(
					music: this.currentMusic.Value,
					volume: ((this.currentFadeInAndOutVolumeMillis * this.currentMusicVolume / 100) >> 10) * userVolume / 100);
			else
				assets.StopMusic();
		}
	}
}

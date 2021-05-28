
namespace Danmaku2Lib
{
	using System;

	public class SoundAndMusicVolumePicker
	{
		private SoundVolumePicker soundVolumePicker;
		private MusicVolumePicker musicVolumePicker;

		private int currentSoundVolume;
		private int currentMusicVolume;

		private int maxChangePerFrame;

		public SoundAndMusicVolumePicker(int xPos, int yPos, int initialSoundVolume, int initialMusicVolume, int elapsedMillisPerFrame)
		{
			this.soundVolumePicker = new SoundVolumePicker(xPos: xPos, yPos: yPos, initialVolume: initialSoundVolume);
			this.musicVolumePicker = new MusicVolumePicker(xPos: xPos, yPos: yPos + 50, initialVolume: initialMusicVolume);

			this.currentSoundVolume = this.soundVolumePicker.GetCurrentSoundVolume();
			this.currentMusicVolume = this.musicVolumePicker.GetCurrentMusicVolume();

			this.maxChangePerFrame = elapsedMillisPerFrame / 5;
		}

		public void ProcessFrame(
			IMouse mouseInput,
			IMouse previousMouseInput)
		{
			this.soundVolumePicker.ProcessFrame(mouseInput: mouseInput, previousMouseInput: previousMouseInput);
			this.musicVolumePicker.ProcessFrame(mouseInput: mouseInput, previousMouseInput: previousMouseInput);

			this.currentSoundVolume = this.ComputeNextVolume(currentVolume: this.currentSoundVolume, desiredVolume: this.soundVolumePicker.GetCurrentSoundVolume());
			this.currentMusicVolume = this.ComputeNextVolume(currentVolume: this.currentMusicVolume, desiredVolume: this.musicVolumePicker.GetCurrentMusicVolume());
		}

		private int ComputeNextVolume(int currentVolume, int desiredVolume)
		{
			if (Math.Abs(desiredVolume - currentVolume) <= this.maxChangePerFrame)
				return desiredVolume;
			else if (desiredVolume > currentVolume)
				return currentVolume + this.maxChangePerFrame;
			else
				return currentVolume - this.maxChangePerFrame;
		}

		/// <summary>
		/// Returns a number from 0 to 100 (both inclusive)
		/// 
		/// The value is smoothed such that the sound volume doesn't change abruptly.
		/// </summary>
		public int GetCurrentSoundVolumeSmoothed()
		{
			return this.currentSoundVolume;
		}

		/// <summary>
		/// Returns a number from 0 to 100 (both inclusive)
		/// 
		/// The value is smoothed such that the music volume doesn't change abruptly.
		/// </summary>
		public int GetCurrentMusicVolumeSmoothed()
		{
			return this.currentMusicVolume;
		}

		public void Render(IDisplay<Danmaku2Assets> display)
		{
			this.soundVolumePicker.Render(display: display);
			this.musicVolumePicker.Render(display: display);
		}
	}
}


namespace Danmaku2Lib
{
	using System;
	using System.Collections.Generic;

	public class ChooseDifficultyScreenFrame : IFrame<Danmaku2Assets>
	{
		private GlobalState globalState;
		private SoundAndMusicVolumePicker soundAndMusicVolumePicker;
		private OptionChooser optionChooser;

		public ChooseDifficultyScreenFrame(GlobalState globalState)
		{
			this.globalState = globalState;
			
			this.soundAndMusicVolumePicker = new SoundAndMusicVolumePicker(
				xPos: 0,
				yPos: 600,
				initialSoundVolume: globalState.SoundVolume,
				initialMusicVolume: globalState.MusicVolume,
				elapsedMillisPerFrame: globalState.ElapsedMillisPerFrame);

			this.optionChooser = new OptionChooser(new List<OptionChooser.Option>()
				{
					new OptionChooser.Option(image: Danmaku2Image.Easy, x: 175, y: 325, arrowX: 125, arrowY: 325),
					new OptionChooser.Option(image: Danmaku2Image.Normal, x: 175, y: 400, arrowX: 125, arrowY: 400),
					new OptionChooser.Option(image: Danmaku2Image.Hard, x: 175, y: 475, arrowX: 125, arrowY: 475)
				},
				initiallySelectedOption: null);
		}

		public IFrame<Danmaku2Assets> GetNextFrame(
			IKeyboard keyboardInput,
			IMouse mouseInput,
			IKeyboard previousKeyboardInput,
			IMouse previousMouseInput,
			IDisplay<Danmaku2Assets> display)
		{
			this.globalState.BackgroundRenderer.ProcessFrame();
			this.globalState.BackgroundRenderer.ProcessFrame();

			this.soundAndMusicVolumePicker.ProcessFrame(
				mouseInput: mouseInput,
				previousMouseInput: previousMouseInput);
			
			this.globalState.SoundVolume = this.soundAndMusicVolumePicker.GetCurrentSoundVolumeSmoothed();
			this.globalState.MusicVolume = this.soundAndMusicVolumePicker.GetCurrentMusicVolumeSmoothed();
			
			int? selectedOption = this.optionChooser.ProcessFrame(
				keyboardInput: keyboardInput,
				previousKeyboardInput: previousKeyboardInput);

			if (selectedOption != null)
			{
				if (selectedOption.Value == 0)
					return new Level1Frame(globalState: this.globalState, difficulty: Difficulty.Easy);

				if (selectedOption.Value == 1)
					return new Level1Frame(globalState: this.globalState, difficulty: Difficulty.Normal);

				if (selectedOption.Value == 2)
					return new Level1Frame(globalState: this.globalState, difficulty: Difficulty.Hard);

				throw new Exception();
			}

			if (keyboardInput.IsPressed(Key.Esc) && !previousKeyboardInput.IsPressed(Key.Esc))
			{
				return new TitleScreenFrame(globalState: this.globalState, initiallySelectedOption: TitleScreenFrame.Option.Start);
			}

			return this;
		}

		public void ProcessMusic()
		{
			this.globalState.ProcessMusic();
		}

		public void Render(IDisplay<Danmaku2Assets> display)
		{
			var assets = display.GetAssets();

			this.globalState.BackgroundRenderer.Render(display: display);

			// Draw background
			display.DrawRectangle(
				x: 0,
				y: 0,
				width: 500,
				height: 700,
				color: new DTColor(r: 0, g: 0, b: 0, alpha: 150),
				fill: true);

			assets.DrawImage(Danmaku2Image.ChooseDifficulty, 120, 202);

			this.optionChooser.Render(display: display);

			this.soundAndMusicVolumePicker.Render(display: display);
		}

		public void RenderMusic(IDisplay<Danmaku2Assets> display)
		{
			this.globalState.RenderMusic(display: display);
		}
	}
}

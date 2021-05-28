
namespace Danmaku2Lib
{
	using System;
	using System.Collections.Generic;

	public class ReplayPauseScreenFrame : IFrame<Danmaku2Assets>
	{
		private GlobalState globalState;
		private ReplayFrame replayFrame;

		private OptionChooser optionChooser;

		private SoundAndMusicVolumePicker soundAndMusicVolumePicker;

		public ReplayPauseScreenFrame(
			GlobalState globalState,
			ReplayFrame replayFrame)
		{
			this.globalState = globalState;
			this.replayFrame = replayFrame;

			this.soundAndMusicVolumePicker = new SoundAndMusicVolumePicker(
				xPos: 0,
				yPos: 600,
				initialSoundVolume: globalState.SoundVolume,
				initialMusicVolume: globalState.MusicVolume,
				elapsedMillisPerFrame: globalState.ElapsedMillisPerFrame);

			this.optionChooser = new OptionChooser(new List<OptionChooser.Option>()
				{
					new OptionChooser.Option(image: Danmaku2Image.ContinueReplay, x: 150, y: 300, arrowX: 100, arrowY: 300),
					new OptionChooser.Option(image: Danmaku2Image.PlayAgain, x: 150, y: 400, arrowX: 100, arrowY: 400),
					new OptionChooser.Option(image: Danmaku2Image.ReturnToTitle, x: 150, y: 500, arrowX: 100, arrowY: 500)
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
			this.soundAndMusicVolumePicker.ProcessFrame(
				mouseInput: mouseInput,
				previousMouseInput: previousMouseInput);

			this.globalState.SoundVolume = this.soundAndMusicVolumePicker.GetCurrentSoundVolumeSmoothed();
			this.globalState.MusicVolume = this.soundAndMusicVolumePicker.GetCurrentMusicVolumeSmoothed();

			int? selectedOption = this.optionChooser.ProcessFrame(keyboardInput: keyboardInput, previousKeyboardInput: previousKeyboardInput);

			if (selectedOption != null)
			{
				if (selectedOption.Value == 0)
					return this.replayFrame;

				if (selectedOption.Value == 1)
					return new Level1Frame(globalState: this.globalState, difficulty: this.replayFrame.GetGameLogic().GetDifficulty());

				if (selectedOption.Value == 2)
					return new TitleScreenFrame(globalState: this.globalState, initiallySelectedOption: null);

				throw new Exception();
			}

			return this;
		}

		public void ProcessMusic()
		{
			this.globalState.ProcessMusic();
		}

		public void Render(IDisplay<Danmaku2Assets> display)
		{
			this.globalState.BackgroundRenderer.Render(display: display);

			this.replayFrame.GetGameLogic().Render(display: display);

			display.DrawRectangle(
				x: 0,
				y: 0,
				width: 500,
				height: 700,
				color: new DTColor(r: 0, g: 0, b: 0, alpha: 150),
				fill: true);

			display.GetAssets().DrawImage(
				image: Danmaku2Image.Paused,
				x: 150,
				y: 150);

			this.optionChooser.Render(display: display);

			this.soundAndMusicVolumePicker.Render(display: display);
		}

		public void RenderMusic(IDisplay<Danmaku2Assets> display)
		{
			this.globalState.RenderMusic(display: display);
		}
	}
}

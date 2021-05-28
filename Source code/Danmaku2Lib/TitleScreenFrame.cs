
namespace Danmaku2Lib
{
	using System;
	using System.Collections.Generic;

	public class TitleScreenFrame : IFrame<Danmaku2Assets>
	{
		private GlobalState globalState;

		private SoundAndMusicVolumePicker soundAndMusicVolumePicker;

		private int? creditXStart;
		private int? creditYStart;
		private int? creditXEnd;
		private int? creditYEnd;
		private bool isHoveringOverCredits;
		private bool isTryingToOpenCredits;

		private OptionChooser optionChooser;

		private CreditsDisplay creditsPopup;

		public enum Option
		{
			Start,
			Instructions,
			Quit
		}

		public TitleScreenFrame(GlobalState globalState, Option? initiallySelectedOption)
		{
			this.globalState = globalState;
			
			this.soundAndMusicVolumePicker = new SoundAndMusicVolumePicker(
				xPos: 0,
				yPos: 600,
				initialSoundVolume: globalState.SoundVolume,
				initialMusicVolume: globalState.MusicVolume,
				elapsedMillisPerFrame: globalState.ElapsedMillisPerFrame);

			this.creditXStart = null;
			this.creditYStart = null;
			this.creditXEnd = null;
			this.creditYEnd = null;
			this.isHoveringOverCredits = false;
			this.isTryingToOpenCredits = false;

			this.creditsPopup = new CreditsDisplay(isWebBrowserVersion: globalState.IsWebBrowserVersion);

			List<OptionChooser.Option> options = new List<OptionChooser.Option>();
			options.Add(new OptionChooser.Option(image: Danmaku2Image.Start, x: 175, y: 325, arrowX: 125, arrowY: 325));
			options.Add(new OptionChooser.Option(image: Danmaku2Image.Instructions, x: 175, y: 400, arrowX: 125, arrowY: 400));
			if (globalState.IsWebBrowserVersion == false)
				options.Add(new OptionChooser.Option(image: Danmaku2Image.Quit, x: 175, y: 475, arrowX: 125, arrowY: 475));

			int? initiallySelectedOptionInt;

			if (initiallySelectedOption == null)
				initiallySelectedOptionInt = null;
			else
			{
				switch (initiallySelectedOption.Value)
				{
					case Option.Start:
						initiallySelectedOptionInt = 0;
						break;
					case Option.Instructions:
						initiallySelectedOptionInt = 1;
						break;
					case Option.Quit:
						if (globalState.IsWebBrowserVersion)
							throw new Exception();
						initiallySelectedOptionInt = 2;
						break;
					default:
						throw new Exception();
				}
			}

			this.optionChooser = new OptionChooser(options: options, initiallySelectedOption: initiallySelectedOptionInt);
		}

		public IFrame<Danmaku2Assets> GetNextFrame(
			IKeyboard keyboardInput,
			IMouse mouseInput,
			IKeyboard previousKeyboardInput,
			IMouse previousMouseInput,
			IDisplay<Danmaku2Assets> display)
		{
			this.globalState.MusicPlayer.SetMusic(music: Danmaku2Music.Xeon6, volume: 100);
			
			this.globalState.BackgroundRenderer.ProcessFrame();
			this.globalState.BackgroundRenderer.ProcessFrame();

			this.soundAndMusicVolumePicker.ProcessFrame(
				mouseInput: this.creditsPopup.IsOpen() ? (new EmptyMouse()) : mouseInput,
				previousMouseInput: this.creditsPopup.IsOpen() ? (new EmptyMouse()) : previousMouseInput);

			this.globalState.SoundVolume = this.soundAndMusicVolumePicker.GetCurrentSoundVolumeSmoothed();
			this.globalState.MusicVolume = this.soundAndMusicVolumePicker.GetCurrentMusicVolumeSmoothed();
			
			int? selectedOption = this.optionChooser.ProcessFrame(
				keyboardInput: this.creditsPopup.IsOpen() ? (new EmptyKeyboard()) : keyboardInput, 
				previousKeyboardInput: this.creditsPopup.IsOpen() ? (new EmptyKeyboard()) : previousKeyboardInput);

			if (selectedOption != null)
			{
				if (selectedOption.Value == 0)
					return new ChooseDifficultyScreenFrame(globalState: this.globalState);

				if (selectedOption.Value == 1)
					return new InstructionScreenFrame(globalState: this.globalState);

				if (selectedOption.Value == 2)
					return null;

				throw new Exception();
			}
			
			if (this.creditXStart == null)
			{
				var assets = display.GetAssets();
				this.creditXStart = 500 - assets.GetWidth(Danmaku2Image.Credits) - 15;
				this.creditYStart = 700 - assets.GetHeight(Danmaku2Image.Credits) - 15;
				this.creditXEnd = 500 - 5;
				this.creditYEnd = 700 - 5;
			}

			this.isHoveringOverCredits = this.creditsPopup.IsOpen() == false
				&& mouseInput.GetX() >= this.creditXStart.Value
				&& mouseInput.GetX() <= this.creditXEnd.Value
				&& mouseInput.GetY() >= this.creditYStart.Value
				&& mouseInput.GetY() <= this.creditYEnd.Value;

			if (mouseInput.IsLeftMouseButtonPressed() && !previousMouseInput.IsLeftMouseButtonPressed() && this.isHoveringOverCredits)
				this.isTryingToOpenCredits = true;
			
			if (!mouseInput.IsLeftMouseButtonPressed() && previousMouseInput.IsLeftMouseButtonPressed())
			{
				if (this.isTryingToOpenCredits && this.isHoveringOverCredits)
					this.creditsPopup.Open();
				this.isTryingToOpenCredits = false;
			}

			this.creditsPopup.ProcessFrame(
				keyboardInput: keyboardInput,
				previousKeyboardInput: previousKeyboardInput,
				mouseInput: mouseInput,
				previousMouseInput: previousMouseInput,
				display: display);

			if (this.globalState.DebugMode)
			{
				if (this.creditsPopup.IsOpen() == false && keyboardInput.IsPressed(Key.D) && !previousKeyboardInput.IsPressed(Key.D))
					return new Level1Demo(globalState: this.globalState).GetReplayFrame();
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
			
			display.DrawRectangle(
				x: 0,
				y: 0,
				width: 500,
				height: 700,
				color: new DTColor(r: 0, g: 0, b: 0, alpha: 150),
				fill: true);

			var assets = display.GetAssets();

			assets.DrawImage(Danmaku2Image.TitleScreen, 67, 189);

			this.optionChooser.Render(display: display);

			assets.DrawImage(Danmaku2Image.Version, 500 - assets.GetWidth(Danmaku2Image.Version) - 10, 670 - assets.GetHeight(Danmaku2Image.Version) - 5);

			if (this.creditXStart.HasValue)
			{
				assets.DrawImage(Danmaku2Image.Credits, this.creditXStart.Value + 5, this.creditYStart.Value + 5);

				display.DrawRectangle(
					x: this.creditXStart.Value,
					y: this.creditYStart.Value,
					width: this.creditXEnd.Value - this.creditXStart.Value,
					height: this.creditYEnd.Value - this.creditYStart.Value,
					color: DTColor.White(),
					fill: false);

				if (this.isHoveringOverCredits)
				{
					display.DrawRectangle(
						x: this.creditXStart.Value,
						y: this.creditYStart.Value,
						width: this.creditXEnd.Value - this.creditXStart.Value,
						height: this.creditYEnd.Value - this.creditYStart.Value,
						color: new DTColor(r: 255, g: 255, b: 255, alpha: 128),
						fill: true);
				}
			}

			this.soundAndMusicVolumePicker.Render(display: display);
			
			this.creditsPopup.Render(display: display);
		}

		public void RenderMusic(IDisplay<Danmaku2Assets> display)
		{
			this.globalState.RenderMusic(display: display);
		}
	}
}

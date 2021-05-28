
namespace Danmaku2Lib
{
	using System;
	using System.Collections.Generic;

	public class GameOverScreenFrame : IFrame<Danmaku2Assets>
	{
		private GlobalState globalState;
		private GameLogic gameLogic;

		private OptionChooser optionChooser;

		public GameOverScreenFrame(GlobalState globalState, GameLogic gameLogic)
		{
			this.globalState = globalState;
			this.gameLogic = gameLogic;

			this.optionChooser = new OptionChooser(new List<OptionChooser.Option>()
				{
					new OptionChooser.Option(image: Danmaku2Image.WatchReplay, x: 150, y: 300, arrowX: 100, arrowY: 300),
					new OptionChooser.Option(image: Danmaku2Image.Retry, x: 150, y: 400, arrowX: 100, arrowY: 400),
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
			this.globalState.BackgroundRenderer.ProcessFrame();
			this.globalState.BackgroundRenderer.ProcessFrame();

			this.gameLogic.ProcessFrame(
				keyboardInput: new EmptyKeyboard(),
				mouseInput: new EmptyMouse(),
				display: display);

			int? selectedOption = this.optionChooser.ProcessFrame(keyboardInput: keyboardInput, previousKeyboardInput: previousKeyboardInput);

			if (selectedOption != null)
			{
				if (selectedOption.Value == 0)
					return new ReplayFrame(
						globalState: this.globalState,
						difficulty: this.gameLogic.GetDifficulty(),
						moveRecorder: this.gameLogic.GetSnapshotOfMoves());

				if (selectedOption.Value == 1)
					return new Level1Frame(
						globalState: this.globalState,
						difficulty: this.gameLogic.GetDifficulty());

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

			this.gameLogic.Render(display: display);

			display.DrawRectangle(
				x: 0,
				y: 0,
				width: 500,
				height: 700,
				color: new DTColor(0, 0, 0, alpha: 150),
				fill: true);

			var assets = display.GetAssets();

			assets.DrawImage(
				image: Danmaku2Image.GameOver,
				x: 100,
				y: 150);

			this.optionChooser.Render(display: display);
		}

		public void RenderMusic(IDisplay<Danmaku2Assets> display)
		{
			this.globalState.RenderMusic(display: display);
		}
	}
}


namespace Danmaku2Lib
{
	using System;

	public class ReplayFrame : IFrame<Danmaku2Assets>
	{
		private GlobalState globalState;
		private GameLogic gameLogic;
		private MoveRecorder moveRecorder;

		public GameLogic GetGameLogic()
		{
			return this.gameLogic;
		}

		public ReplayFrame(
			GlobalState globalState,
			Difficulty difficulty,
			MoveRecorder moveRecorder)
		{
			this.globalState = globalState;
			this.moveRecorder = moveRecorder;
			moveRecorder.BeginPlayback();

			this.gameLogic = new GameLogic(
				xOffsetInPixels: 0,
				yOffsetInPixels: 0,
				globalState: globalState,
				gameSettings: moveRecorder.GetGameSettings());
		}

		private enum ReplayStatus
		{
			InProgress,
			GameOver,
			Victory
		}

		private ReplayStatus ProcessGameFrame(IDisplay<Danmaku2Assets> display)
		{
			var simulatedKeyboardInput = this.moveRecorder.GetNextInput();

			var result = this.gameLogic.ProcessFrameHelper(
				keyboardInput: simulatedKeyboardInput,
				mouseInput: new EmptyMouse(),
				display: display);

			if (result.TransitionToGameOver)
				return ReplayStatus.GameOver;

			if (result.TransitionToNextLevel)
				return ReplayStatus.Victory;

			return ReplayStatus.InProgress;
		}

		public IFrame<Danmaku2Assets> GetNextFrame(
			IKeyboard keyboardInput,
			IMouse mouseInput,
			IKeyboard previousKeyboardInput,
			IMouse previousMouseInput,
			IDisplay<Danmaku2Assets> display)
		{
			if (keyboardInput.IsPressed(Key.Esc) && !previousKeyboardInput.IsPressed(Key.Esc)
					||
				keyboardInput.IsPressed(Key.P) && !previousKeyboardInput.IsPressed(Key.P))
			{
				return new ReplayPauseScreenFrame(globalState: this.globalState, replayFrame: this);
			}

			int numTimes;

			if (keyboardInput.IsPressed(Key.Shift))
				numTimes = 1;
			else if (keyboardInput.IsPressed(Key.Z))
				numTimes = 8;
			else
				numTimes = 2;

			for (int i = 0; i < numTimes; i++)
			{
				this.globalState.BackgroundRenderer.ProcessFrame();

				var replayStatus = this.ProcessGameFrame(display: display);

				switch (replayStatus)
				{
					case ReplayStatus.InProgress:
						// do nothing
						break;
					case ReplayStatus.GameOver:
						return new GameOverScreenFrame(
							globalState: this.globalState,
							gameLogic: this.gameLogic);
					case ReplayStatus.Victory:
						return new VictoryScreenFrame(
							globalState: this.globalState,
							gameLogic: this.gameLogic);
					default:
						throw new Exception();
				}
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
			
			this.gameLogic.Render(display: display);
		}

		public void RenderMusic(IDisplay<Danmaku2Assets> display)
		{
			this.globalState.RenderMusic(display: display);
		}
	}
}

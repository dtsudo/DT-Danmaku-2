
namespace Danmaku2Lib
{
	public class Level1Frame : IFrame<Danmaku2Assets>
	{
		private GlobalState globalState;
		private GameLogic gameLogic;

		public GameLogic GetGameLogic()
		{
			return this.gameLogic;
		}

		public Level1Frame(
			GlobalState globalState,
			Difficulty difficulty)
		{
			this.globalState = globalState;

			var gameSettings = new GameSettings(
				playerBulletSpreadLevel: globalState.InitialPlayerBulletSpreadLevel != null ? globalState.InitialPlayerBulletSpreadLevel.Value : PlayerBulletSpreadLevel.ThreeBullets,
				playerBulletStrength: globalState.InitialPlayerBulletStrength != null ? globalState.InitialPlayerBulletStrength.Value :  PlayerBulletStrength.Level1,
				difficulty: difficulty,
				numLivesRemaining: globalState.InitialNumLives != null ? globalState.InitialNumLives.Value : 3);

			this.gameLogic = new GameLogic(
				xOffsetInPixels: 0,
				yOffsetInPixels: 0,
				globalState: globalState,
				gameSettings: gameSettings);
		}

		public IFrame<Danmaku2Assets> GetNextFrame(
			IKeyboard keyboardInput,
			IMouse mouseInput,
			IKeyboard previousKeyboardInput,
			IMouse previousMouseInput,
			IDisplay<Danmaku2Assets> display)
		{
			GameLogic.FrameResult frameResult = this.gameLogic.ProcessFrame(
				keyboardInput: keyboardInput,
				mouseInput: mouseInput,
				display: display);

			this.globalState.BackgroundRenderer.ProcessFrame();

			if (frameResult.IsDoubleFrame)
				this.globalState.BackgroundRenderer.ProcessFrame();

			bool transitionToGameOver = frameResult.TransitionToGameOver;
			bool transitionToNextLevel = frameResult.TransitionToNextLevel;

			if (this.globalState.DebugMode)
			{
				if (keyboardInput.IsPressed(Key.Nine))
				{
					for (int i = 0; i < 7; i++)
					{
						frameResult = this.gameLogic.ProcessFrame(
							keyboardInput: keyboardInput,
							mouseInput: mouseInput,
							display: display);
						if (frameResult.TransitionToGameOver)
							transitionToGameOver = true;
						if (frameResult.TransitionToNextLevel)
							transitionToNextLevel = true;
					}
				}
				if (keyboardInput.IsPressed(Key.Zero))
				{
					for (int i = 0; i < 31; i++)
					{
						frameResult = this.gameLogic.ProcessFrame(
							keyboardInput: keyboardInput,
							mouseInput: mouseInput,
							display: display);
						if (frameResult.TransitionToGameOver)
							transitionToGameOver = true;
						if (frameResult.TransitionToNextLevel)
							transitionToNextLevel = true;
					}
				}
			}

			if (transitionToGameOver)
				return new GameOverScreenFrame(
					globalState: this.globalState,
					gameLogic: this.gameLogic);

			if (transitionToNextLevel)
				return new VictoryScreenFrame(
					globalState: this.globalState,
					gameLogic: this.gameLogic);

			if (keyboardInput.IsPressed(Key.Esc) && !previousKeyboardInput.IsPressed(Key.Esc)
					||
				keyboardInput.IsPressed(Key.P) && !previousKeyboardInput.IsPressed(Key.P))
			{
				return new PauseScreenFrame(
					globalState: this.globalState,
					level1Frame: this);
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

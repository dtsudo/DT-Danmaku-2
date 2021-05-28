
namespace Danmaku2Lib
{	
	public class InitialLoadingScreenFrame : IFrame<Danmaku2Assets>
	{
		private GlobalState globalState;
		private bool skipToLevel1HardDifficulty;
		private bool isDemo;
		private bool hasInitializedDemoFrame;

		public InitialLoadingScreenFrame(GlobalState globalState, bool skipToLevel1HardDifficulty, bool isDemo)
		{
			this.globalState = globalState;
			this.skipToLevel1HardDifficulty = skipToLevel1HardDifficulty;
			this.isDemo = isDemo;
			this.hasInitializedDemoFrame = false;
		}
		
		public IFrame<Danmaku2Assets> GetNextFrame(
			IKeyboard keyboardInput,
			IMouse mouseInput,
			IKeyboard previousKeyboardInput,
			IMouse previousMouseInput,
			IDisplay<Danmaku2Assets> display)
		{
			var returnValue = this.GetNextFrameHelper(display: display);

			if (returnValue != null)
				return returnValue;

			returnValue = this.GetNextFrameHelper(display: display);

			if (returnValue != null)
				return returnValue;

			return this;
		}

		private IFrame<Danmaku2Assets> GetNextFrameHelper(IDisplay<Danmaku2Assets> display)
		{
			bool isDoneLoadingImages = display.GetAssets().LoadImages();

			if (!isDoneLoadingImages)
				return null;
			
			bool isDoneLoadingSounds = display.GetAssets().LoadSounds();

			if (!isDoneLoadingSounds)
				return null;
						
			bool isDoneLoadingMusic = display.GetAssets().LoadMusic();

			if (!isDoneLoadingMusic)
				return null;

			if (!this.hasInitializedDemoFrame)
			{
				this.hasInitializedDemoFrame = true;
				(new Level1DemoMoves()).GetMoveRecorder();
				return null;
			}

			if (this.skipToLevel1HardDifficulty)
				return new Level1Frame(globalState: this.globalState, difficulty: Difficulty.Hard);

			if (this.isDemo)
				return (new Level1Demo(globalState: this.globalState)).GetReplayFrame();

			return new TitleScreenFrame(globalState: this.globalState, initiallySelectedOption: null);
		}

		public void ProcessMusic()
		{
		}

		public void Render(IDisplay<Danmaku2Assets> display)
		{
			display.GetAssets().DrawInitialLoadingScreen();
		}

		public void RenderMusic(IDisplay<Danmaku2Assets> display)
		{
		}
	}
}

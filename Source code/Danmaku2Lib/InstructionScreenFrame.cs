
namespace Danmaku2Lib
{
	public class InstructionScreenFrame : IFrame<Danmaku2Assets>
	{
		private GlobalState globalState;

		public InstructionScreenFrame(GlobalState globalState)
		{
			this.globalState = globalState;
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

			if (keyboardInput.IsPressed(Key.Z) && !previousKeyboardInput.IsPressed(Key.Z)
					||
				keyboardInput.IsPressed(Key.Enter) && !previousKeyboardInput.IsPressed(Key.Enter)
					||
				keyboardInput.IsPressed(Key.Space) && !previousKeyboardInput.IsPressed(Key.Space)
					||
				keyboardInput.IsPressed(Key.Esc) && !previousKeyboardInput.IsPressed(Key.Esc))
			{
				return new TitleScreenFrame(globalState: this.globalState, initiallySelectedOption: TitleScreenFrame.Option.Instructions);
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

			assets.DrawImage(
				image: Danmaku2Image.InstructionsLarge,
				x: 150,
				y: 50);

			assets.DrawImage(
				image: Danmaku2Image.HowToPlay,
				x: 15,
				y: 150);

			assets.DrawImage(
				image: Danmaku2Image.Arrow,
				x: 150,
				y: 450);

			assets.DrawImage(
				image: Danmaku2Image.Back,
				x: 200,
				y: 450);
		}

		public void RenderMusic(IDisplay<Danmaku2Assets> display)
		{
			this.globalState.RenderMusic(display: display);
		}
	}
}


namespace Danmaku2Lib
{
	public class Level1Demo
	{
		private GlobalState globalState;

		public Level1Demo(GlobalState globalState)
		{
			this.globalState = globalState;
		}

		public ReplayFrame GetReplayFrame()
		{
			MoveRecorder moveRecorder = (new Level1DemoMoves()).GetMoveRecorder();

			return new ReplayFrame(
				globalState: this.globalState,
				difficulty: moveRecorder.GetGameSettings().difficulty,
				moveRecorder: moveRecorder);
		}
	}
}

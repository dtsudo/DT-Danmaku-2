
namespace Danmaku2Lib
{
	public class Danmaku2
	{
		public static IFrame<Danmaku2Assets> GetFirstFrame(GlobalState globalState, bool skipToLevel1HardDifficulty, bool isDemo)
		{
			var frame = new InitialLoadingScreenFrame(globalState: globalState, skipToLevel1HardDifficulty: skipToLevel1HardDifficulty, isDemo: isDemo);
			return frame;
		}
	}
}

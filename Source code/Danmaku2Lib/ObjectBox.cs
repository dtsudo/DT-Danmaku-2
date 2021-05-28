
namespace Danmaku2Lib
{
	public class ObjectBox
	{
		// relative to the object's center; i.e. object's center is (0,0)
		public int LowerXMillis { get; private set; }
		public int UpperXMillis { get; private set; }
		public int LowerYMillis { get; private set; }
		public int UpperYMillis { get; private set; }

		public ObjectBox(int lowerXMillis, int upperXMillis, int lowerYMillis, int upperYMillis)
		{
			this.LowerXMillis = lowerXMillis;
			this.UpperXMillis = upperXMillis;
			this.LowerYMillis = lowerYMillis;
			this.UpperYMillis = upperYMillis;
		}
	}
}

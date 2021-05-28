
namespace Danmaku2
{
	using Danmaku2Lib;
	using System;

	public class Program
	{
		[STAThread]
		static void Main(string[] args)
		{
			bool debugMode = false;

			Initializer.Start(debugMode: debugMode);
		}
	}
}

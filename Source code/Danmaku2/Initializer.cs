
namespace Danmaku2
{
	using AgateLib;
	using AgateLib.DisplayLib;
	using AgateLib.Geometry;
	using Danmaku2Lib;
	using System;
	using System.Globalization;

	public class Initializer
	{
		public static void Start(bool debugMode)
		{			
			Core.CrossPlatformDebugLevel = CrossPlatformDebugLevel.None;

			using (AgateSetup setup = new AgateSetup())
			{
				setup.InitializeDisplay();
				setup.InitializeInput();
				bool canSuccessfullyPlayAudio;
				try
				{
					setup.InitializeAudio();
					canSuccessfullyPlayAudio = true;
				}
				catch (Exception)
				{
					canSuccessfullyPlayAudio = false;
				}

				if (setup.WasCanceled)
					return;

				DisplayWindow window = DisplayWindow.CreateWindowed("DT Danmaku 2", 500, 700);
				Display.VSync = false;

				int fps = 60;

				IFrame<Danmaku2Assets> frame = Danmaku2.GetFirstFrame(
					globalState: new GlobalState(
						fps: fps,
						nonGameLogicRng: new DTRandom(),
						guidGenerator: new GuidGenerator(guidString: "347354247161643025"),
						isWebBrowserVersion: false,
						initialPlayerBulletSpreadLevel: null,
						initialPlayerBulletStrength: null,
						initialNumLives: null,
						debugMode: debugMode,
						initialSoundVolume: null,
						initialMusicVolume: null),
					skipToLevel1HardDifficulty: false,
					isDemo: false);

				IKeyboard agateLibKeyboard = new AgateLibKeyboard();
				IMouse agateLibMouse = new AgateLibMouse();
				IDisplay<Danmaku2Assets> display = new Danmaku2AgateLibDisplay(canSuccessfullyPlayAudio: canSuccessfullyPlayAudio);
				IKeyboard prevKeyboard = new EmptyKeyboard();
				IMouse prevMouse = new EmptyMouse();

				double elapsedTimeMs = 0.0;

				long timeForFpsCounter = DateTime.Now.Ticks;
				int currentDisplayFpsCount = 0;
				int displayFpsSnapshotValue = 0;
				
				int debugSlowDown = 0;
				int debugNumCyclesToSkip = 0;
				
				int numTimesFramesDropped = 0;

				int hasReached300FramesCount = 0;

				while (Display.CurrentWindow.IsClosed == false && frame != null)
				{
					Display.BeginFrame();

					Display.Clear(Color.White);

					if (frame != null)
					{
						frame.Render(display);
						frame.RenderMusic(display);
					}

					elapsedTimeMs += Display.DeltaTime;

					// Run at 60 frames per second.

					// If for whatever reason, we're really behind, we'll try to catch up,
					// but only for a maximum of 5 consecutive frames.
					if (elapsedTimeMs > 1000.0 / fps * 5.0)
					{
						elapsedTimeMs = 1000.0 / fps * 5.0;
						
						if (hasReached300FramesCount >= 300)
							numTimesFramesDropped++;
					}

					if (elapsedTimeMs > 1000.0 / fps)
					{						
						elapsedTimeMs = elapsedTimeMs - 1000.0 / fps;
						IKeyboard currentKeyboard = new CopiedKeyboard(agateLibKeyboard);
						IMouse currentMouse = new CopiedMouse(agateLibMouse);

						if (hasReached300FramesCount < 300)
							hasReached300FramesCount++;

						if (debugMode)
						{
							if (debugNumCyclesToSkip == 0)
							{
								if (agateLibKeyboard.IsPressed(Key.Six) && !prevKeyboard.IsPressed(Key.Six))
									debugSlowDown = (debugSlowDown + 1) % 4;

								if (frame != null)
									frame = frame.GetNextFrame(currentKeyboard, currentMouse, prevKeyboard, prevMouse, display);
								if (frame != null)
									frame.ProcessMusic();
								prevKeyboard = new CopiedKeyboard(currentKeyboard);
								prevMouse = new CopiedMouse(currentMouse);
							}
						}
						else
						{
							if (frame != null)
								frame = frame.GetNextFrame(currentKeyboard, currentMouse, prevKeyboard, prevMouse, display);
							if (frame != null)
								frame.ProcessMusic();
							prevKeyboard = new CopiedKeyboard(currentKeyboard);
							prevMouse = new CopiedMouse(currentMouse);
						}
						
						if (debugMode)
						{
							if (debugSlowDown > 0)
							{
								if (debugNumCyclesToSkip > 0)
									debugNumCyclesToSkip--;
								else
								{
									if (debugSlowDown == 1)
										debugNumCyclesToSkip = 1;
									if (debugSlowDown == 2)
										debugNumCyclesToSkip = 7;
									if (debugSlowDown == 3)
										debugNumCyclesToSkip = 31;
								}
							}
							else
							{
								debugNumCyclesToSkip = 0;
							}
						}
					}
					else
					{
						System.Threading.Thread.Sleep(millisecondsTimeout: 5);
					}

					if (debugMode)
					{
						currentDisplayFpsCount++;
						long milliSecondsElapsedForFpsCounter = (DateTime.Now.Ticks - timeForFpsCounter) / 10000L;
						if (milliSecondsElapsedForFpsCounter > 1000)
						{
							timeForFpsCounter += 10000L * 1000L;
							displayFpsSnapshotValue = currentDisplayFpsCount;
							currentDisplayFpsCount = 0;
						}

						string fpsDisplay = "fps: " + displayFpsSnapshotValue.ToString(CultureInfo.InvariantCulture);

						if (numTimesFramesDropped > 0)
							fpsDisplay = fpsDisplay + " | Number of dropped frames: " + numTimesFramesDropped.ToString(CultureInfo.InvariantCulture);

						display.DebugPrint(x: 10, y: 10, debugText: fpsDisplay);
					}

					Display.EndFrame();

					Core.KeepAlive();
				}

				if (frame == null)
				{
					if (window.IsClosed == false)
						window.Dispose();
				}

				display.GetAssets().DisposeImages();
				display.GetAssets().DisposeSounds();
				display.GetAssets().DisposeMusic();
			}
		}
	}
}

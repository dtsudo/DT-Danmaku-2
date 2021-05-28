
namespace Danmaku2Lib
{
	using System;
	using System.Collections.Generic;
	using System.Text;

	public class MoveRecorder
	{
		private class FrameInput
		{
			public bool IsUpPressed;
			public bool IsDownPressed;
			public bool IsLeftPressed;
			public bool IsRightPressed;
			public bool IsShootPressed;
			public bool IsSlowdownPressed;
		}

		private class SimulatedKeyboard : IKeyboard
		{
			private FrameInput frameInput;

			public SimulatedKeyboard(FrameInput frameInput)
			{
				this.frameInput = frameInput;
			}

			public bool IsPressed(Key key)
			{
				switch (key)
				{
					case Key.UpArrow: return this.frameInput.IsUpPressed;
					case Key.DownArrow: return this.frameInput.IsDownPressed;
					case Key.LeftArrow: return this.frameInput.IsLeftPressed;
					case Key.RightArrow: return this.frameInput.IsRightPressed;
					case Key.Z: return this.frameInput.IsShootPressed;
					case Key.Shift: return this.frameInput.IsSlowdownPressed;
					default: return false;
				}
			}
		}

		private LinkedList<FrameInput> inputList;
		private LinkedList<FrameInput>.Enumerator enumerator;

		private GameSettings gameSettings;

		private bool hasFinishedRecording;

		public MoveRecorder(GameSettings gameSettings)
		{
			this.inputList = new LinkedList<FrameInput>();
			this.gameSettings = gameSettings;
			this.hasFinishedRecording = false;
		}

		public string SerializeToString()
		{
			StringBuilder str = new StringBuilder();

			str.Append(this.hasFinishedRecording ? "true" : "false");
			str.Append(" ");

			switch (this.gameSettings.playerBulletSpreadLevel)
			{
				case PlayerBulletSpreadLevel.ThreeBullets:
					str.Append("3");
					break;
				case PlayerBulletSpreadLevel.FiveBullets:
					str.Append("5");
					break;
				case PlayerBulletSpreadLevel.SevenBullets:
					str.Append("7");
					break;
				default:
					throw new Exception();
			}
			str.Append(" ");

			switch (this.gameSettings.playerBulletStrength)
			{
				case PlayerBulletStrength.Level1:
					str.Append("1");
					break;
				case PlayerBulletStrength.Level2:
					str.Append("2");
					break;
				case PlayerBulletStrength.Level3:
					str.Append("3");
					break;
				default:
					throw new Exception();
			}
			str.Append(" ");

			switch (this.gameSettings.difficulty)
			{
				case Difficulty.Easy:
					str.Append("easy");
					break;
				case Difficulty.Normal:
					str.Append("normal");
					break;
				case Difficulty.Hard:
					str.Append("hard");
					break;
				default:
					throw new Exception();
			}
			str.Append(" ");

			str.Append(Util.IntToString(this.gameSettings.numLivesRemaining));

			foreach (FrameInput input in this.inputList)
			{
				str.Append("\n");
				str.Append(input.IsUpPressed ? "1" : "0");
				str.Append(input.IsDownPressed ? "1" : "0");
				str.Append(input.IsLeftPressed ? "1" : "0");
				str.Append(input.IsRightPressed ? "1" : "0");
				str.Append(input.IsShootPressed ? "1" : "0");
				str.Append(input.IsSlowdownPressed ? "1" : "0");
			}

			return str.ToString();
		}

		public static MoveRecorder DeserializeFromString(string str)
		{
			int index = str.IndexOf(' ');

			string hasFinishedRecordingString = str.Substring(0, index);
			str = str.Substring(index + 1);

			bool hasFinishedRecording;
			if (hasFinishedRecordingString == "true")
				hasFinishedRecording = true;
			else if (hasFinishedRecordingString == "false")
				hasFinishedRecording = false;
			else
				throw new Exception();

			index = str.IndexOf(' ');

			string playerBulletSpreadLevelString = str.Substring(0, index);
			str = str.Substring(index + 1);

			PlayerBulletSpreadLevel playerBulletSpreadLevel;
			if (playerBulletSpreadLevelString == "3")
				playerBulletSpreadLevel = PlayerBulletSpreadLevel.ThreeBullets;
			else if (playerBulletSpreadLevelString == "5")
				playerBulletSpreadLevel = PlayerBulletSpreadLevel.FiveBullets;
			else if (playerBulletSpreadLevelString == "7")
				playerBulletSpreadLevel = PlayerBulletSpreadLevel.SevenBullets;
			else
				throw new Exception();

			index = str.IndexOf(' ');

			string playerBulletStrengthString = str.Substring(0, index);
			str = str.Substring(index + 1);

			PlayerBulletStrength playerBulletStrength;
			if (playerBulletStrengthString == "1")
				playerBulletStrength = PlayerBulletStrength.Level1;
			else if (playerBulletStrengthString == "2")
				playerBulletStrength = PlayerBulletStrength.Level2;
			else if (playerBulletStrengthString == "3")
				playerBulletStrength = PlayerBulletStrength.Level3;
			else
				throw new Exception();

			index = str.IndexOf(' ');

			string difficultyString = str.Substring(0, index);
			str = str.Substring(index + 1);

			Difficulty difficulty;
			if (difficultyString == "easy")
				difficulty = Difficulty.Easy;
			else if (difficultyString == "normal")
				difficulty = Difficulty.Normal;
			else if (difficultyString == "hard")
				difficulty = Difficulty.Hard;
			else
				throw new Exception();

			index = str.IndexOf('\n');

			string numLivesRemainingString = index == -1 ? str : str.Substring(0, index);
			str = index == -1 ? "" : str.Substring(index + 1);

			int numLivesRemaining = Util.StringToInt(numLivesRemainingString);

			string[] array = str.Split('\n');

			MoveRecorder moveRecorder = new MoveRecorder(gameSettings: new GameSettings(
				playerBulletSpreadLevel: playerBulletSpreadLevel,
				playerBulletStrength: playerBulletStrength,
				difficulty: difficulty,
				numLivesRemaining: numLivesRemaining));

			moveRecorder.hasFinishedRecording = hasFinishedRecording;

			foreach (string input in array)
			{
				if (input.Length == 0)
					continue;

				FrameInput frameInput = new FrameInput
				{
					IsUpPressed = input[0] == '1',
					IsDownPressed = input[1] == '1',
					IsLeftPressed = input[2] == '1',
					IsRightPressed = input[3] == '1',
					IsShootPressed = input[4] == '1',
					IsSlowdownPressed = input[5] == '1'
				};

				moveRecorder.inputList.AddLast(frameInput);
			}

			return moveRecorder;
		}

		public MoveRecorder MakeCopyOfMoveRecorder()
		{
			var copyRecorder = new MoveRecorder(gameSettings: this.gameSettings);

			foreach (var frameInput in this.inputList)
			{
				copyRecorder.inputList.AddLast(frameInput);
			}

			copyRecorder.hasFinishedRecording = this.hasFinishedRecording;

			return copyRecorder;
		}

		public GameSettings GetGameSettings()
		{
			return this.gameSettings;
		}

		public void FinishRecording()
		{
			this.hasFinishedRecording = true;
		}

		public void RecordMove(IKeyboard keyboardInput)
		{
			if (this.hasFinishedRecording)
				return;

			FrameInput input = new FrameInput();

			input.IsUpPressed = keyboardInput.IsPressed(Key.UpArrow);
			input.IsDownPressed = keyboardInput.IsPressed(Key.DownArrow);
			input.IsLeftPressed = keyboardInput.IsPressed(Key.LeftArrow);
			input.IsRightPressed = keyboardInput.IsPressed(Key.RightArrow);
			input.IsShootPressed = keyboardInput.IsPressed(Key.Z);
			input.IsSlowdownPressed = keyboardInput.IsPressed(Key.Shift);

			this.inputList.AddLast(input);
		}

		public void BeginPlayback()
		{
			this.enumerator = this.inputList.GetEnumerator();
		}

		public IKeyboard GetNextInput()
		{
			var hasNext = this.enumerator.MoveNext();

			if (!hasNext)
				return new EmptyKeyboard();

			return new SimulatedKeyboard(frameInput: this.enumerator.Current);
		}
	}
}

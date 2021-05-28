
namespace Danmaku2Lib
{
	using System;
	using System.Collections.Generic;

	public class GameLogic
	{
		public const int GAME_WINDOW_HEIGHT_IN_PIXELS = 700;
		public const int GAME_WINDOW_WIDTH_IN_PIXELS = 500;

		private int xOffsetInPixels;
		private int yOffsetInPixels;

		private Player player;
		private List<PlayerBullet> playerBullets;
		private PlayerBulletsUtil playerBulletsUtil;
		private CollisionDetector collisionDetector;

		private Difficulty difficulty;

		private GlobalState globalState;

		private bool showCollisionBoxes;
		private bool showDamageBoxes;

		private List<Danmaku2Sound> soundsToPlay;

		private List<IEnemy> enemies;

		private int numLivesRemaining;
		private int? millisToGameOver;

		private int? millisToNextLevel;

		private IDTDeterministicRandom rng;

		private BossHealthBar bossHealthBar;

		private MoveRecorder moveRecorder;

		private GameSettings initialGameSettings;

		private IKeyboard previousKeyboardInput;
		private IMouse previousMouseInput;

		public Difficulty GetDifficulty()
		{
			return this.difficulty;
		}

		public MoveRecorder GetSnapshotOfMoves()
		{
			return this.moveRecorder.MakeCopyOfMoveRecorder();
		}

		public GameSettings GetGameSettings()
		{
			return this.initialGameSettings;
		}

		public GameLogic(
			int xOffsetInPixels,
			int yOffsetInPixels,
			GlobalState globalState,
			GameSettings gameSettings)
		{
			this.initialGameSettings = gameSettings;

			var rng = new DTDeterministicRandom(seed: 7239);

			this.xOffsetInPixels = xOffsetInPixels;
			this.yOffsetInPixels = yOffsetInPixels;
			this.globalState = globalState;

			this.difficulty = gameSettings.difficulty;

			this.showCollisionBoxes = false;
			this.showDamageBoxes = false;

			this.numLivesRemaining = gameSettings.numLivesRemaining;

			this.millisToGameOver = null;
			this.millisToNextLevel = null;

			this.player = new Player(
				elapsedMillisPerFrame: globalState.ElapsedMillisPerFrame,
				rng: rng);
			this.playerBullets = new List<PlayerBullet>();
			this.playerBulletsUtil = new PlayerBulletsUtil(
				elapsedMillisPerFrame: globalState.ElapsedMillisPerFrame,
				playerBulletSpreadLevel: gameSettings.playerBulletSpreadLevel,
				playerBulletStrength: gameSettings.playerBulletStrength);
			this.collisionDetector = new CollisionDetector();

			this.soundsToPlay = new List<Danmaku2Sound>();

			this.rng = rng;

			this.enemies = new List<IEnemy>();
			this.enemies.Add(new Level1EnemyGeneratorEnemy(
				difficulty: gameSettings.difficulty,
				elapsedMillisPerFrame: globalState.ElapsedMillisPerFrame,
				rng: rng));

			this.moveRecorder = new MoveRecorder(gameSettings: gameSettings);

			this.bossHealthBar = new BossHealthBar(elapsedMillisPerFrame: globalState.ElapsedMillisPerFrame);

			this.previousKeyboardInput = new EmptyKeyboard();
			this.previousMouseInput = new EmptyMouse();
		}

		public class FrameResult
		{
			public FrameResult(bool isDoubleFrame, bool transitionToGameOver, bool transitionToNextLevel)
			{
				this.IsDoubleFrame = isDoubleFrame;
				this.TransitionToGameOver = transitionToGameOver;
				this.TransitionToNextLevel = transitionToNextLevel;
			}

			public bool IsDoubleFrame { get; private set; }
			public bool TransitionToGameOver { get; private set; }
			public bool TransitionToNextLevel { get; private set; }
		}

		public FrameResult ProcessFrame(
			IKeyboard keyboardInput,
			IMouse mouseInput,
			IDisplay<Danmaku2Assets> display)
		{
			keyboardInput = new CopiedKeyboard(keyboardInput);
			mouseInput = new CopiedMouse(mouseInput);

			var result1 = this.ProcessFrameHelper(
				keyboardInput: keyboardInput,
				mouseInput: mouseInput,
				display: display);

			bool isDoubleFrame = !keyboardInput.IsPressed(Key.Shift) || player.IsDead;

			if (isDoubleFrame)
			{
				var result2 = this.ProcessFrameHelper(
					keyboardInput: keyboardInput,
					mouseInput: mouseInput,
					display: display);

				bool transitionToGameOver = result1.TransitionToGameOver || result2.TransitionToGameOver;

				return new FrameResult(
					isDoubleFrame: true,
					transitionToGameOver: transitionToGameOver, 
					transitionToNextLevel: !transitionToGameOver && (result1.TransitionToNextLevel || result2.TransitionToNextLevel));
			}
			else
			{
				return new FrameResult(isDoubleFrame: false, transitionToGameOver: result1.TransitionToGameOver, transitionToNextLevel: result1.TransitionToNextLevel);
			}
		}

		public class FrameHelperResult
		{
			public FrameHelperResult(bool transitionToGameOver, bool transitionToNextLevel)
			{
				this.TransitionToGameOver = transitionToGameOver;
				this.TransitionToNextLevel = transitionToNextLevel;
			}

			public bool TransitionToGameOver { get; private set; }
			public bool TransitionToNextLevel { get; private set; }
		}

		public FrameHelperResult ProcessFrameHelper(
			IKeyboard keyboardInput,
			IMouse mouseInput,
			IDisplay<Danmaku2Assets> display)
		{
			keyboardInput = new CopiedKeyboard(keyboardInput);
			mouseInput = new CopiedMouse(mouseInput);

			IKeyboard previousKeyboardInput = this.previousKeyboardInput;
			IMouse previousMouseInput = this.previousMouseInput;

			this.previousKeyboardInput = keyboardInput;
			this.previousMouseInput = mouseInput;

			IDisplay<Danmaku2Assets> translatedDisplay = new TranslatedDisplay(
				display: display,
				xOffsetInPixels: this.xOffsetInPixels,
				yOffsetInPixels: this.yOffsetInPixels);

			this.player.ProcessFrame(
				keyboardInput: keyboardInput,
				previousKeyboardInput: previousKeyboardInput,
				display: translatedDisplay,
				isDebugMode: this.globalState.DebugMode);

			 var playerBulletsUtilResult = this.playerBulletsUtil.ProcessFrame(
				playerBullets: this.playerBullets,
				keyboardInput: keyboardInput,
				playerXMillis: this.player.XMillis,
				playerYMillis: this.player.YMillis,
				isPlayerDead: this.player.IsDead,
				display: translatedDisplay);
			this.playerBullets = playerBulletsUtilResult.PlayerBullets;
			if (playerBulletsUtilResult.Sounds != null)
			{
				for (int i = 0; i < playerBulletsUtilResult.Sounds.Count; i++)
					this.soundsToPlay.Add(playerBulletsUtilResult.Sounds[i]);
			}

			int? bossHealthMeterNumber = null;
			int? bossHealthMeterMilliPercentage = null;

			List<IEnemy> updatedEnemies = new List<IEnemy>();
			for (int i = 0; i < this.enemies.Count; i++)
			{
				IEnemy enemy = this.enemies[i];

				var result = enemy.ProcessFrame(
					elapsedMillisPerFrame: this.globalState.ElapsedMillisPerFrame,
					playerXMillis: this.player.XMillis,
					playerYMillis: this.player.YMillis);

				if (result != null)
				{
					if (result.Enemies != null)
					{
						for (int j = 0; j < result.Enemies.Count; j++)
							updatedEnemies.Add(result.Enemies[j]);
					}

					if (result.Sounds != null)
					{
						for (int j = 0; j < result.Sounds.Count; j++)
							this.soundsToPlay.Add(result.Sounds[j]);
					}


					if (result.BossHealthMeterNumber != null)
						bossHealthMeterNumber = result.BossHealthMeterNumber.Value;
					if (result.BossHealthMeterMilliPercentage != null)
						bossHealthMeterMilliPercentage = result.BossHealthMeterMilliPercentage.Value;

					if (result.ShouldEndLevel && this.millisToNextLevel == null)
						this.millisToNextLevel = 5000;
				}
			}

			this.enemies = updatedEnemies;

			this.bossHealthBar.ProcessBossHealthBar(
				bossHealthMeterNumber: bossHealthMeterNumber,
				bossHealthMeterMilliPercentage: bossHealthMeterMilliPercentage);

			var hasPlayerCollided = this.collisionDetector.HandleCollisionBetweenPlayerAndEnemyObjects(
				playerXMillis: this.player.XMillis,
				playerYMillis: this.player.YMillis,
				isPlayerDead: this.player.IsDead,
				isPlayerInvulnerable: this.player.IsInvulnerable(),
				enemies: this.enemies);

			if (hasPlayerCollided)
			{
				bool shouldRespawn;
				if (this.numLivesRemaining == 0)
				{
					shouldRespawn = false;
					this.millisToGameOver = 5000;
				}
				else
				{
					shouldRespawn = true;
					this.numLivesRemaining--;
				}
				var enemy = this.player.DestroyPlayer(shouldRespawn: shouldRespawn);

				this.enemies.Add(enemy);
			}

			if (this.millisToGameOver != null)
			{
				this.millisToGameOver = this.millisToGameOver.Value - this.globalState.ElapsedMillisPerFrame;
				if (this.millisToGameOver.Value < 0)
					this.millisToGameOver = 0;
			}

			if (this.millisToNextLevel != null)
			{
				this.millisToNextLevel = this.millisToNextLevel.Value - this.globalState.ElapsedMillisPerFrame;
				if (this.millisToNextLevel.Value < 0)
					this.millisToNextLevel = 0;
			}

			this.playerBullets = this.collisionDetector.HandleCollisionBetweenPlayerBulletsAndEnemyObjects(
				playerBullets: this.playerBullets,
				enemies: this.enemies);

			if (this.globalState.DebugMode)
			{
				if (keyboardInput.IsPressed(Key.K) && !previousKeyboardInput.IsPressed(Key.K) && !this.player.IsDead)
				{
					bool shouldRespawn;
					if (this.numLivesRemaining == 0)
					{
						shouldRespawn = false;
						this.millisToGameOver = 5000;
					}
					else
					{
						shouldRespawn = true;
						this.numLivesRemaining--;
					}
					var enemy = this.player.DestroyPlayer(shouldRespawn: shouldRespawn);
					this.enemies.Add(enemy);
				}

				if (keyboardInput.IsPressed(Key.D) && !previousKeyboardInput.IsPressed(Key.D))
				{
					this.showDamageBoxes = !this.showDamageBoxes;
				}

				if (keyboardInput.IsPressed(Key.Y) && !previousKeyboardInput.IsPressed(Key.Y))
				{
					switch (this.playerBulletsUtil.playerBulletSpreadLevel)
					{
						case PlayerBulletSpreadLevel.ThreeBullets:
							this.playerBulletsUtil.playerBulletSpreadLevel = PlayerBulletSpreadLevel.FiveBullets;
							break;
						case PlayerBulletSpreadLevel.FiveBullets:
							this.playerBulletsUtil.playerBulletSpreadLevel = PlayerBulletSpreadLevel.SevenBullets;
							break;
						case PlayerBulletSpreadLevel.SevenBullets:
							this.playerBulletsUtil.playerBulletSpreadLevel = PlayerBulletSpreadLevel.ThreeBullets;
							break;
						default:
							throw new Exception();
					}
				}

				if (keyboardInput.IsPressed(Key.U) && !previousKeyboardInput.IsPressed(Key.U))
				{
					switch (this.playerBulletsUtil.playerBulletStrength)
					{
						case PlayerBulletStrength.Level1:
							this.playerBulletsUtil.playerBulletStrength = PlayerBulletStrength.Level2;
							break;
						case PlayerBulletStrength.Level2:
							this.playerBulletsUtil.playerBulletStrength = PlayerBulletStrength.Level3;
							break;
						case PlayerBulletStrength.Level3:
							this.playerBulletsUtil.playerBulletStrength = PlayerBulletStrength.Level1;
							break;
						default:
							throw new Exception();
					}
				}

				if (keyboardInput.IsPressed(Key.H) && !previousKeyboardInput.IsPressed(Key.H))
				{
					this.showCollisionBoxes = !this.showCollisionBoxes;
				}
			}

			bool transitionToGameOver = this.millisToGameOver != null && this.millisToGameOver.Value <= 0;
			bool transitionToNextLevel = this.millisToGameOver == null && this.millisToNextLevel != null && this.millisToNextLevel.Value <= 0;

			this.moveRecorder.RecordMove(keyboardInput: keyboardInput);

			if (transitionToGameOver || transitionToNextLevel)
				this.moveRecorder.FinishRecording();

			return new FrameHelperResult(transitionToGameOver: transitionToGameOver, transitionToNextLevel: transitionToNextLevel);
		}

		public void Render(IDisplay<Danmaku2Assets> display)
		{
			IDisplay<Danmaku2Assets> translatedDisplay = new TranslatedDisplay(
				display: display,
				xOffsetInPixels: this.xOffsetInPixels,
				yOffsetInPixels: this.yOffsetInPixels);
			
			EnemyRenderer.RenderEnemiesOfSpecificZIndex(
				enemies: this.enemies,
				enemyZIndex: ZIndex.Background,
				display: translatedDisplay);

			if (this.soundsToPlay.Count > 0)
			{
				for (int i = 0; i < this.soundsToPlay.Count; i++)
					translatedDisplay.GetAssets().PlaySound(
						sound: this.soundsToPlay[i],
						volume: this.globalState.SoundVolume);

				this.soundsToPlay.Clear();
			}

			EnemyRenderer.RenderEnemiesOfSpecificZIndex(
				enemies: this.enemies,
				enemyZIndex: ZIndex.Enemy,
				display: translatedDisplay);

			EnemyRenderer.RenderEnemiesOfSpecificZIndex(
				enemies: this.enemies,
				enemyZIndex: ZIndex.DeathAnimation,
				display: translatedDisplay);

			this.player.Render(display: translatedDisplay);

			this.playerBulletsUtil.RenderPlayerBullets(
				playerBullets: this.playerBullets,
				display: translatedDisplay);

			EnemyRenderer.RenderEnemiesOfSpecificZIndex(
				enemies: this.enemies,
				enemyZIndex: ZIndex.EnemyBullet,
				display: translatedDisplay);

			this.bossHealthBar.RenderBossHealthBar(display: translatedDisplay);

			NumLivesDisplay.Render(display: translatedDisplay, numLivesRemaining: this.numLivesRemaining);

			if (this.globalState.DebugMode)
			{
				HitboxRenderer.Render(
					display: translatedDisplay,
					playerBullets: this.playerBullets,
					enemies: this.enemies,
					shouldShowCollisionBoxes: this.showCollisionBoxes,
					shouldShowDamageBoxes: this.showDamageBoxes);
			}
		}
	}
}

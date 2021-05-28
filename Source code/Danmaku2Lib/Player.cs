
namespace Danmaku2Lib
{
	public class Player
	{
		private const int SPAWN_LOCATION_X_MILLIS = (GameLogic.GAME_WINDOW_WIDTH_IN_PIXELS / 2) * 1024;
		private const int SPAWN_LOCATION_Y_MILLIS = 50 * 1024;

		public int XMillis;
		public int YMillis;

		public bool IsDead;
		private int? millisUntilRespawn;
		private int? millisUntilNotInvulnerable;

		private bool forcePlayerInvulnerable;
		
		private int elapsedMillisPerFrame;

		private IDTDeterministicRandom rng;

		private int speedInOneDirection;
		private int speedInTwoDirections;

		public Player(
			int elapsedMillisPerFrame,
			IDTDeterministicRandom rng)
		{
			this.XMillis = SPAWN_LOCATION_X_MILLIS;
			this.YMillis = SPAWN_LOCATION_Y_MILLIS;
			this.IsDead = false;
			this.millisUntilRespawn = null;
			this.millisUntilNotInvulnerable = null;

			this.elapsedMillisPerFrame = elapsedMillisPerFrame;

			this.rng = rng;

			this.speedInOneDirection = 200 * elapsedMillisPerFrame;
			// 1 / sqrt(2) = 0.707
			this.speedInTwoDirections = this.speedInOneDirection * 707 / 1000;

			this.forcePlayerInvulnerable = false;
		}
		
		public void ProcessFrame(
			IKeyboard keyboardInput,
			IKeyboard previousKeyboardInput,
			IDisplay<Danmaku2Assets> display,
			bool isDebugMode)
		{
			if (isDebugMode)
			{
				if (keyboardInput.IsPressed(Key.I) && !previousKeyboardInput.IsPressed(Key.I))
				{
					this.forcePlayerInvulnerable = !this.forcePlayerInvulnerable;
				}
			}

			if (!this.IsDead)
			{
				var assets = display.GetAssets();
				int playerHalfWidthMillis = (assets.GetWidth(Danmaku2Image.PlayerShip) << 10) >> 1;
				int playerHalfHeightMillis = (assets.GetHeight(Danmaku2Image.PlayerShip) << 10) >> 1;

				bool isLeft = keyboardInput.IsPressed(Key.LeftArrow) && !keyboardInput.IsPressed(Key.RightArrow);
				bool isRight = !keyboardInput.IsPressed(Key.LeftArrow) && keyboardInput.IsPressed(Key.RightArrow);
				bool isUp = keyboardInput.IsPressed(Key.UpArrow) && !keyboardInput.IsPressed(Key.DownArrow);
				bool isDown = !keyboardInput.IsPressed(Key.UpArrow) && keyboardInput.IsPressed(Key.DownArrow);

				bool isTwoDirections = isLeft && isUp || isUp && isRight || isRight && isDown || isDown && isLeft;
				int speed = isTwoDirections ? this.speedInTwoDirections : this.speedInOneDirection;

				if (isLeft)
				{
					this.XMillis = this.XMillis - speed;

					int minXMillis = playerHalfWidthMillis;
					if (this.XMillis < minXMillis)
						this.XMillis = minXMillis;
				}

				if (isRight)
				{
					this.XMillis = this.XMillis + speed;

					int maxXMillis = (GameLogic.GAME_WINDOW_WIDTH_IN_PIXELS << 10) - playerHalfWidthMillis;
					if (this.XMillis > maxXMillis)
						this.XMillis = maxXMillis;
				}

				if (isDown)
				{
					this.YMillis = this.YMillis - speed;

					int minYMillis = playerHalfHeightMillis;
					if (this.YMillis < minYMillis)
						this.YMillis = minYMillis;
				}

				if (isUp)
				{
					this.YMillis = this.YMillis + speed;

					int maxYMillis = (GameLogic.GAME_WINDOW_HEIGHT_IN_PIXELS << 10) - playerHalfHeightMillis;
					if (this.YMillis > maxYMillis)
						this.YMillis = maxYMillis;
				}

				if (isLeft)
					this.rng.AddSeed(150 /* arbitrarily chosen */);
				if (isRight)
					this.rng.AddSeed(161 /* arbitrarily chosen */);
				if (isDown)
					this.rng.AddSeed(172 /* arbitrarily chosen */);
				if (isUp)
					this.rng.AddSeed(183 /* arbitrarily chosen */);
			}

			if (this.millisUntilNotInvulnerable != null)
			{
				this.millisUntilNotInvulnerable = this.millisUntilNotInvulnerable.Value - this.elapsedMillisPerFrame;
				if (this.millisUntilNotInvulnerable.Value <= 0)
				{
					this.millisUntilNotInvulnerable = null;
				}
			}

			if (this.millisUntilRespawn != null)
			{
				this.millisUntilRespawn = this.millisUntilRespawn.Value - this.elapsedMillisPerFrame;
				if (this.millisUntilRespawn.Value <= 0)
				{
					this.millisUntilRespawn = null;
					this.IsDead = false;

					this.XMillis = SPAWN_LOCATION_X_MILLIS;
					this.YMillis = SPAWN_LOCATION_Y_MILLIS;
					this.millisUntilNotInvulnerable = 6000;
				}
			}
		}

		public bool IsInvulnerable()
		{
			return this.millisUntilNotInvulnerable != null || this.forcePlayerInvulnerable;
		}

		public IEnemy DestroyPlayer(bool shouldRespawn)
		{
			this.IsDead = true;

			if (shouldRespawn)
				this.millisUntilRespawn = 3000;

			return new StandardExplosionEnemy(
				xMillis: this.XMillis,
				yMillis: this.YMillis,
				rng: this.rng,
				scalingFactorScaled: 128);
		}

		public void Render(IDisplay<Danmaku2Assets> display)
		{
			if (this.IsDead)
				return;

			var assets = display.GetAssets();

			assets.DrawImage(
				this.IsInvulnerable() ? Danmaku2Image.PlayerShipInvulnerable : Danmaku2Image.PlayerShip,
				(this.XMillis >> 10) - (assets.GetWidth(Danmaku2Image.PlayerShip) >> 1),
				GameLogic.GAME_WINDOW_HEIGHT_IN_PIXELS - (this.YMillis >> 10) - (assets.GetHeight(Danmaku2Image.PlayerShip) >> 1));
		}
	}
}

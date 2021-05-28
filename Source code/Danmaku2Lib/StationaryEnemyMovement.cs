
namespace Danmaku2Lib
{
	using System;

	public class StationaryEnemyMovement
	{
		private enum Status
		{
			Approaching,
			Stopped,
			Leaving
		}

		private Status status;

		private int xMillis;
		private int yMillis;

		private int initialYMillis;
		private int destinationYMillis;

		private int? stoppedDurationInMillis;
		private int? currentStoppedDurationInMillis;
		private bool hasLeftScreen;

		private bool forceLeave;

		public StationaryEnemyMovement(
			int xMillis, 
			int initialYMillis, 
			int destinationYMillis,
			int? stoppedDurationInMillis)
		{
			this.xMillis = xMillis;
			this.yMillis = initialYMillis;
			this.initialYMillis = initialYMillis;
			this.destinationYMillis = destinationYMillis;
			this.status = Status.Approaching;
			this.stoppedDurationInMillis = stoppedDurationInMillis;
			if (this.stoppedDurationInMillis == null)
				this.currentStoppedDurationInMillis = null;
			else
				this.currentStoppedDurationInMillis = 0;

			this.hasLeftScreen = false;

			this.forceLeave = false;
		}

		public void ProcessNextFrame(int elapsedMillisPerFrame)
		{
			if (this.status == Status.Approaching)
			{
				int distance = this.yMillis - this.destinationYMillis;

				if (distance > 90 * 1024)
				{
					this.yMillis -= elapsedMillisPerFrame * 200;
				}
				else if (distance > 60 * 1024)
				{
					this.yMillis -= elapsedMillisPerFrame * 175;
				}
				else if (distance > 40 * 1024)
				{
					this.yMillis -= elapsedMillisPerFrame * 150;
				}
				else if (distance > 25 * 1024)
				{
					this.yMillis -= elapsedMillisPerFrame * 125;
				}
				else
				{
					this.yMillis -= elapsedMillisPerFrame * 100;
				}

				if (this.yMillis <= this.destinationYMillis)
				{
					this.yMillis = this.destinationYMillis;
					this.status = Status.Stopped;
				}
			}
			else if (this.status == Status.Stopped)
			{
				if (this.stoppedDurationInMillis != null)
				{
					this.currentStoppedDurationInMillis = this.currentStoppedDurationInMillis.Value + elapsedMillisPerFrame;

					if (this.currentStoppedDurationInMillis.Value >= this.stoppedDurationInMillis.Value)
						this.status = Status.Leaving;
				}
				else
				{
					if (this.forceLeave)
						this.status = Status.Leaving;
				}
			}
			else if (this.status == Status.Leaving)
			{
				int distance = this.yMillis - this.destinationYMillis;

				if (distance > 90 * 1024)
				{
					this.yMillis += elapsedMillisPerFrame * 130;
				}
				else if (distance > 60 * 1024)
				{
					this.yMillis += elapsedMillisPerFrame * 100;
				}
				else if (distance > 40 * 1024)
				{
					this.yMillis += elapsedMillisPerFrame * 80;
				}
				else if (distance > 25 * 1024)
				{
					this.yMillis += elapsedMillisPerFrame * 60;
				}
				else
				{
					this.yMillis += elapsedMillisPerFrame * 40;
				}

				if (this.yMillis >= this.initialYMillis)
				{
					this.yMillis = this.initialYMillis;
					this.hasLeftScreen = true;
				}
			}
			else
				throw new Exception();
		}

		public int GetXMillis()
		{
			return this.xMillis;
		}

		public int GetYMillis()
		{
			return this.yMillis;
		}

		public bool HasStopped()
		{
			return this.status == Status.Stopped;
		}

		public bool HasLeftScreen()
		{
			return this.hasLeftScreen;
		}

		public void ForceLeave()
		{
			this.forceLeave = true;	
		}
	}
}

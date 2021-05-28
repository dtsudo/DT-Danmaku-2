
namespace Danmaku2Lib
{
	using System;
	using System.Collections.Generic;

	public class BossHealthBar
	{
		private class DisplayedHealthBar : IComparable<DisplayedHealthBar>
		{
			public int MeterNumber;
			public int MilliPercentage;

			public int CompareTo(DisplayedHealthBar other)
			{
				if (this.MeterNumber < other.MeterNumber)
					return -1;
				if (this.MeterNumber > other.MeterNumber)
					return 1;
				return 0;
			}
		}

		private int? bossHealthMeterNumber;
		private int? bossHealthMeterMilliPercentage;

		private int numMilliPercentPerFrame;

		private List<DisplayedHealthBar> displayedHealthBars;

		public BossHealthBar(int elapsedMillisPerFrame)
		{
			this.bossHealthMeterNumber = null;
			this.bossHealthMeterMilliPercentage = null;

			int numPercentPerSecond = 70 * 1024 / 1000;
			this.numMilliPercentPerFrame = numPercentPerSecond * elapsedMillisPerFrame;

			this.displayedHealthBars = new List<DisplayedHealthBar>();
		}

		public void ProcessBossHealthBar(
			int? bossHealthMeterNumber,
			int? bossHealthMeterMilliPercentage)
		{
			if (bossHealthMeterNumber == null && bossHealthMeterMilliPercentage != null)
				throw new Exception();
			if (bossHealthMeterNumber != null && bossHealthMeterMilliPercentage == null)
				throw new Exception();

			this.bossHealthMeterNumber = bossHealthMeterNumber;
			this.bossHealthMeterMilliPercentage = bossHealthMeterMilliPercentage;

			if (bossHealthMeterNumber == null)
			{
				this.displayedHealthBars = new List<DisplayedHealthBar>();
			}
			else
			{
				List<DisplayedHealthBar> newList = new List<DisplayedHealthBar>();

				bool[] isAlreadyIncluded = new bool[bossHealthMeterNumber.Value];
				for (int i = 0; i < isAlreadyIncluded.Length; i++)
					isAlreadyIncluded[i] = false;

				foreach (DisplayedHealthBar existingHealthBar in this.displayedHealthBars)
				{
					if (existingHealthBar.MeterNumber - 1 < isAlreadyIncluded.Length)
						isAlreadyIncluded[existingHealthBar.MeterNumber - 1] = true;

					if (existingHealthBar.MeterNumber < bossHealthMeterNumber.Value)
						existingHealthBar.MilliPercentage = Math.Min(existingHealthBar.MilliPercentage + numMilliPercentPerFrame, 128 * 1024);
					else if (existingHealthBar.MeterNumber > bossHealthMeterNumber.Value)
						existingHealthBar.MilliPercentage = Math.Max(existingHealthBar.MilliPercentage - numMilliPercentPerFrame, 0);
					else
					{
						if (existingHealthBar.MilliPercentage < bossHealthMeterMilliPercentage.Value)
							existingHealthBar.MilliPercentage = Math.Min(existingHealthBar.MilliPercentage + numMilliPercentPerFrame, bossHealthMeterMilliPercentage.Value);
						else if (existingHealthBar.MilliPercentage > bossHealthMeterMilliPercentage.Value)
							existingHealthBar.MilliPercentage = Math.Max(existingHealthBar.MilliPercentage - numMilliPercentPerFrame, bossHealthMeterMilliPercentage.Value);
					}

					if (existingHealthBar.MeterNumber <= bossHealthMeterNumber.Value || existingHealthBar.MilliPercentage > 0)
						newList.Add(existingHealthBar);
				}

				for (int i = 0; i < isAlreadyIncluded.Length; i++)
				{
					if (!isAlreadyIncluded[i])
					{
						DisplayedHealthBar newHealthBar = new DisplayedHealthBar();
						newHealthBar.MeterNumber = i + 1;
						newHealthBar.MilliPercentage = 0;
						newList.Add(newHealthBar);
					}
				}

				this.displayedHealthBars = newList;
			}
		}

		public void RenderBossHealthBar(IDisplay<Danmaku2Assets> display)
		{
			if (this.bossHealthMeterNumber == null)
				return;

			List<DisplayedHealthBar> list = new List<DisplayedHealthBar>();
			foreach (DisplayedHealthBar healthBar in this.displayedHealthBars)
			{
				if (healthBar.MeterNumber >= this.bossHealthMeterNumber.Value - 1)
					list.Add(healthBar);
			}

			list.Sort();

			for (int i = 0; i < list.Count; i++)
			{
				DisplayedHealthBar healthBar = list[i];

				int alpha = healthBar.MeterNumber == this.bossHealthMeterNumber.Value - 1
					? 40
					: 255;
				
				const int maxWidthInPixels = GameLogic.GAME_WINDOW_WIDTH_IN_PIXELS - 100;

				int desiredWidth = (maxWidthInPixels * healthBar.MilliPercentage) >> 17;

				display.DrawRectangle(
					x: 50,
					y: 40,
					width: desiredWidth,
					height: 7,
					color: GetColor(bossHealthMeterNumber: healthBar.MeterNumber, alpha: alpha),
					fill: true);
			}
		}

		private static DTColor GetColor(int bossHealthMeterNumber, int alpha)
		{
			long barNum = (bossHealthMeterNumber - 1) % 3;

			if (barNum == 0)
				return new DTColor(r: 255, g: 67, b: 38, alpha: alpha);
			if (barNum == 1)
				return new DTColor(r: 255, g: 127, b: 39, alpha: alpha);

			return new DTColor(r: 255, g: 190, b: 38, alpha: alpha);
		}
	}
}

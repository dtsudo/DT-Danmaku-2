
namespace Danmaku2Lib
{
	using System;
	using System.Collections.Generic;

	public class CreditsDisplay
	{
		private enum Tab
		{
			Programming,
			Images,
			Font,
			Sound,
			Music
		}

		private bool isOpen;
		private bool isTryingToClose;
		private Tab selectedTab;

		private bool areXCoordinatesInitialized;
		private int xCoordinateOfProgrammingTab;
		private int xCoordinateOfImagesTab;
		private int xCoordinateOfFontTab;
		private int xCoordinateOfSoundTab;
		private int xCoordinateOfMusicTab;
		private int xCoordinateOfEndOfTabStrip;

		private Tab? isTryingToSelectTab;
		private Tab? mouseHoverTab;

		private bool isWebBrowserVersion;

		public CreditsDisplay(bool isWebBrowserVersion)
		{
			this.isOpen = false;
			this.isTryingToClose = false;
			this.selectedTab = Tab.Programming;
			this.isTryingToSelectTab = null;

			this.areXCoordinatesInitialized = false;

			this.mouseHoverTab = null;

			this.isWebBrowserVersion = isWebBrowserVersion;
		}

		public bool IsOpen()
		{
			return this.isOpen;
		}

		public void Open()
		{
			this.isOpen = true;
			this.isTryingToClose = false;
			this.isTryingToSelectTab = null;
			this.mouseHoverTab = null;
		}

		private bool IsMouseOverCloseIcon(IMouse mouseInput)
		{
			if (this.isOpen == false)
				return false;
		
			return 425 <= mouseInput.GetX()
				&& mouseInput.GetX() <= 450
				&& 50 <= mouseInput.GetY()
				&& mouseInput.GetY() <= 75;
		}

		private Tab? IsMouseOverTab(IMouse mouseInput)
		{
			if (!this.areXCoordinatesInitialized)
				return null;

			if (this.isOpen == false)
				return null;

			int mouseX = mouseInput.GetX();
			int mouseY = mouseInput.GetY();

			if (mouseY < 50 || mouseY > 80)
				return null;

			if (this.xCoordinateOfProgrammingTab <= mouseX && mouseX < this.xCoordinateOfImagesTab)
				return Tab.Programming;
			if (this.xCoordinateOfImagesTab <= mouseX && mouseX < this.xCoordinateOfFontTab)
				return Tab.Images;
			if (this.xCoordinateOfFontTab <= mouseX && mouseX < this.xCoordinateOfSoundTab)
				return Tab.Font;
			if (this.xCoordinateOfSoundTab <= mouseX && mouseX < this.xCoordinateOfMusicTab)
				return Tab.Sound;
			if (this.xCoordinateOfMusicTab <= mouseX && mouseX < this.xCoordinateOfEndOfTabStrip)
				return Tab.Music;

			return null;
		}

		public void ProcessFrame(
			IKeyboard keyboardInput,
			IKeyboard previousKeyboardInput,
			IMouse mouseInput,
			IMouse previousMouseInput,
			IDisplay<Danmaku2Assets> display)
		{
			var assets = display.GetAssets();

			if (!this.areXCoordinatesInitialized)
			{
				this.areXCoordinatesInitialized = true;
				int widthOfProgrammingText = assets.GetWidth(Danmaku2Image.Programming);
				int widthOfImagesText = assets.GetWidth(Danmaku2Image.Images);
				int widthOfFontText = assets.GetWidth(Danmaku2Image.Font);
				int widthOfSoundText = assets.GetWidth(Danmaku2Image.Sound);
				int widthOfMusicText = assets.GetWidth(Danmaku2Image.Music);

				this.xCoordinateOfProgrammingTab = 50;
				this.xCoordinateOfImagesTab = 50 + widthOfProgrammingText + 5 * 2;
				this.xCoordinateOfFontTab = this.xCoordinateOfImagesTab + widthOfImagesText + 5 * 2;
				this.xCoordinateOfSoundTab = this.xCoordinateOfFontTab + widthOfFontText + 5 * 2;
				this.xCoordinateOfMusicTab = this.xCoordinateOfSoundTab + widthOfSoundText + 5 * 2;
				this.xCoordinateOfEndOfTabStrip = this.xCoordinateOfMusicTab + widthOfMusicText + 5 * 2;
			}

			if (this.isOpen
				&& mouseInput.IsLeftMouseButtonPressed()
				&& !previousMouseInput.IsLeftMouseButtonPressed())
			{
				if (this.IsMouseOverCloseIcon(mouseInput: mouseInput))
					this.isTryingToClose = true;

				this.isTryingToSelectTab = this.IsMouseOverTab(mouseInput: mouseInput);
			}

			this.mouseHoverTab = this.IsMouseOverTab(mouseInput: mouseInput);

			if (this.isOpen && !mouseInput.IsLeftMouseButtonPressed() & previousMouseInput.IsLeftMouseButtonPressed())
			{
				if (this.isTryingToClose && this.IsMouseOverCloseIcon(mouseInput: mouseInput))
					this.isOpen = false;
				this.isTryingToClose = false;

				Tab? isMouseOverTab = this.IsMouseOverTab(mouseInput: mouseInput);
				if (isMouseOverTab.HasValue && this.isTryingToSelectTab.HasValue && isMouseOverTab.Value == this.isTryingToSelectTab.Value)
					this.selectedTab = isMouseOverTab.Value;
				this.isTryingToSelectTab = null;
			}
		}

		public void Render(IDisplay<Danmaku2Assets> display)
		{
			if (!this.areXCoordinatesInitialized)
				return;

			var assets = display.GetAssets();

			DTColor grey = new DTColor(r: 128, g: 128, b: 128);
			DTColor lightGrey = new DTColor(r: 180, g: 180, b: 180);

			if (this.isOpen)
			{
				display.DrawRectangle(
					x: 50,
					y: 50,
					width: 400,
					height: 600,
					color: DTColor.White(),
					fill: true);

				display.DrawThickRectangle(
					x: 50,
					y: 50,
					width: 400,
					height: 600,
					additionalThickness: 1,
					color: DTColor.Black(),
					fill: false);

				if (this.selectedTab != Tab.Programming)
				{
					display.DrawRectangle(
						x: this.xCoordinateOfProgrammingTab,
						y: 50,
						width: this.xCoordinateOfImagesTab - this.xCoordinateOfProgrammingTab,
						height: 30,
						color: this.mouseHoverTab == Tab.Programming ? lightGrey : grey,
						fill: true);
				}
				if (this.selectedTab != Tab.Images)
				{
					display.DrawRectangle(
						x: this.xCoordinateOfImagesTab,
						y: 50,
						width: this.xCoordinateOfFontTab - this.xCoordinateOfImagesTab,
						height: 30,
						color: this.mouseHoverTab == Tab.Images ? lightGrey : grey,
						fill: true);
				}
				if (this.selectedTab != Tab.Font)
				{
					display.DrawRectangle(
						x: this.xCoordinateOfFontTab,
						y: 50,
						width: this.xCoordinateOfSoundTab - this.xCoordinateOfFontTab,
						height: 30,
						color: this.mouseHoverTab == Tab.Font ? lightGrey : grey,
						fill: true);
				}
				if (this.selectedTab != Tab.Sound)
				{
					display.DrawRectangle(
						x: this.xCoordinateOfSoundTab,
						y: 50,
						width: this.xCoordinateOfMusicTab - this.xCoordinateOfSoundTab,
						height: 30,
						color: this.mouseHoverTab == Tab.Sound ? lightGrey : grey,
						fill: true);
				}
				if (this.selectedTab != Tab.Music)
				{
					display.DrawRectangle(
						x: this.xCoordinateOfMusicTab,
						y: 50,
						width: this.xCoordinateOfEndOfTabStrip - this.xCoordinateOfMusicTab,
						height: 30,
						color: this.mouseHoverTab == Tab.Music ? lightGrey : grey,
						fill: true);
				}

				display.DrawThickRectangle(
					x: this.xCoordinateOfProgrammingTab,
					y: 50,
					width: this.xCoordinateOfImagesTab - this.xCoordinateOfProgrammingTab,
					height: 30,
					additionalThickness: 1,
					color: DTColor.Black(),
					fill: false);

				display.DrawThickRectangle(
					x: this.xCoordinateOfImagesTab,
					y: 50,
					width: this.xCoordinateOfFontTab - this.xCoordinateOfImagesTab,
					height: 30,
					additionalThickness: 1,
					color: DTColor.Black(),
					fill: false);

				display.DrawThickRectangle(
					x: this.xCoordinateOfFontTab,
					y: 50,
					width: this.xCoordinateOfSoundTab - this.xCoordinateOfFontTab,
					height: 30,
					additionalThickness: 1,
					color: DTColor.Black(),
					fill: false);

				display.DrawThickRectangle(
					x: this.xCoordinateOfSoundTab,
					y: 50,
					width: this.xCoordinateOfMusicTab - this.xCoordinateOfSoundTab,
					height: 30,
					additionalThickness: 1,
					color: DTColor.Black(),
					fill: false);

				display.DrawThickRectangle(
					x: this.xCoordinateOfMusicTab,
					y: 50,
					width: this.xCoordinateOfEndOfTabStrip - this.xCoordinateOfMusicTab,
					height: 30,
					additionalThickness: 1,
					color: DTColor.Black(),
					fill: false);

				display.DrawThickRectangle(
					x: this.xCoordinateOfEndOfTabStrip,
					y: 50,
					width: 450 - this.xCoordinateOfEndOfTabStrip,
					height: 30,
					additionalThickness: 1,
					color: DTColor.Black(),
					fill: false);

				assets.DrawImage(
					image: Danmaku2Image.Programming,
					x: this.xCoordinateOfProgrammingTab + 5,
					y: 55);
				
				assets.DrawImage(
					image: Danmaku2Image.Images,
					x: this.xCoordinateOfImagesTab + 5,
					y: 55);
				
				assets.DrawImage(
					image: Danmaku2Image.Font,
					x: this.xCoordinateOfFontTab + 5,
					y: 55);

				assets.DrawImage(
					image: Danmaku2Image.Sound,
					x: this.xCoordinateOfSoundTab + 5,
					y: 55);

				assets.DrawImage(
					image: Danmaku2Image.Music,
					x: this.xCoordinateOfMusicTab + 5,
					y: 55);
				
				if (selectedTab == Tab.Programming)
					display.DrawRectangle(
						x: this.xCoordinateOfProgrammingTab + 2,
						y: 78,
						width: this.xCoordinateOfImagesTab - this.xCoordinateOfProgrammingTab - 4,
						height: 5,
						color: DTColor.White(),
						fill: true);
				if (selectedTab == Tab.Images)
					display.DrawRectangle(
						x: this.xCoordinateOfImagesTab + 2,
						y: 78,
						width: this.xCoordinateOfFontTab - this.xCoordinateOfImagesTab - 4,
						height: 5,
						color: DTColor.White(),
						fill: true);
				if (selectedTab == Tab.Font)
					display.DrawRectangle(
						x: this.xCoordinateOfFontTab + 2,
						y: 78,
						width: this.xCoordinateOfSoundTab - this.xCoordinateOfFontTab - 4,
						height: 5,
						color: DTColor.White(),
						fill: true);
				if (selectedTab == Tab.Sound)
					display.DrawRectangle(
						x: this.xCoordinateOfSoundTab + 2,
						y: 78,
						width: this.xCoordinateOfMusicTab - this.xCoordinateOfSoundTab - 4,
						height: 5,
						color: DTColor.White(),
						fill: true);
				if (selectedTab == Tab.Music)
					display.DrawRectangle(
						x: this.xCoordinateOfMusicTab + 2,
						y: 78,
						width: this.xCoordinateOfEndOfTabStrip - this.xCoordinateOfMusicTab - 4,
						height: 5,
						color: DTColor.White(),
						fill: true);

				if (selectedTab == Tab.Programming)
					CreditsProgrammingDisplay.Render(
						x: 50,
						y: 80,
						isWebBrowserVersion: this.isWebBrowserVersion,
						display: display);
				if (selectedTab == Tab.Images)
					CreditsImagesDisplay.Render(
						x: 50,
						y: 80,
						display: display);
				if (selectedTab == Tab.Font)
					CreditsFontDisplay.Render(
						x: 50,
						y: 80,
						display: display);
				if (selectedTab == Tab.Sound)
					CreditsSoundDisplay.Render(
						x: 50,
						y: 80,
						display: display);
				if (selectedTab == Tab.Music)
					CreditsMusicDisplay.Render(
						x: 50,
						y: 80,
						display: display);

				assets.DrawImageRotatedCounterclockwise(
					image: Danmaku2Image.CloseIcon,
					x: 425,
					y: 50,
					degreesScaled: 0,
					scalingFactorScaled: 64);
			}
		}
	}
}

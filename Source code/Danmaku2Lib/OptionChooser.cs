
namespace Danmaku2Lib
{
	using System;
	using System.Collections.Generic;

    public class OptionChooser
    {
        public class Option
        {
            public Option(
                Danmaku2Image image,
                int x,
                int y,
                int arrowX,
                int arrowY)
            {
                this.Image = image;
                this.X = x;
                this.Y = y;
                this.ArrowX = arrowX;
                this.ArrowY = arrowY;
            }

            public Danmaku2Image Image { get; private set; }
            public int X { get; private set; }
            public int Y { get; private set; }
            public int ArrowX { get; private set; }
            public int ArrowY { get; private set; }
        }

        private List<Option> options;
        private int selectedOption;

        public OptionChooser(List<Option> options, int? initiallySelectedOption)
        {
            this.options = options;
			if (initiallySelectedOption != null)
				this.selectedOption = initiallySelectedOption.Value;
			else
				this.selectedOption = 0;

			if (this.selectedOption >= options.Count)
				throw new Exception();
        }

        /// <summary>
        /// Returns null if the user didn't pick anything; otherwise,
        /// returns the index of the selected option.
        /// </summary>
        public int? ProcessFrame(
            IKeyboard keyboardInput,
            IKeyboard previousKeyboardInput)
        {
            if (keyboardInput.IsPressed(Key.DownArrow) && !previousKeyboardInput.IsPressed(Key.DownArrow))
            {
                this.selectedOption = this.selectedOption + 1;
                if (this.selectedOption == this.options.Count)
                    this.selectedOption = 0;
            }

            if (keyboardInput.IsPressed(Key.UpArrow) && !previousKeyboardInput.IsPressed(Key.UpArrow))
            {
                this.selectedOption = this.selectedOption - 1;
                if (this.selectedOption == -1)
                    this.selectedOption = this.options.Count - 1;
            }

            if (keyboardInput.IsPressed(Key.Z) && !previousKeyboardInput.IsPressed(Key.Z)
                    ||
                keyboardInput.IsPressed(Key.Enter) && !previousKeyboardInput.IsPressed(Key.Enter)
                    ||
                keyboardInput.IsPressed(Key.Space) && !previousKeyboardInput.IsPressed(Key.Space))
            {
                return this.selectedOption;
            }

            return null;
        }

        public void Render(IDisplay<Danmaku2Assets> display)
        {
            var assets = display.GetAssets();

            for (int i = 0; i < this.options.Count; i++)
            {
                var option = this.options[i];

                assets.DrawImage(
                    image: option.Image,
                    x: option.X,
                    y: option.Y);

                if (this.selectedOption == i)
                    assets.DrawImage(
                        image: Danmaku2Image.Arrow,
                        x: option.ArrowX,
                        y: option.ArrowY);
            }
        }
    }
}

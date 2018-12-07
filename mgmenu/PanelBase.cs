// COPYRIGHT 2018 Roberto Ceccarelli - Casasoft.
// 
// This file is part of OR Tools.
// 
// OR Tools is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// OR Tools is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with OR Tools.  If not, see <http://www.gnu.org/licenses/>.

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.BitmapFonts;
using System.Collections.Generic;

namespace Casasoft.MgMenu
{
    public class PanelBase
    {
        protected enum FontSizes { Normal, Subtitle, Title, Header }
        protected Dictionary<FontSizes, BitmapFont> fonts;

        protected Texture2D boxBackground;
        protected Texture2D boxBackgroundInactive;

        protected int maxItems;

        protected int screenX;
        protected int screenY;
        protected Rectangle textBox;
        protected Rectangle leftBox;

        public string Caption { get; set; }
        public int Selected { get; set; }

        /// <summary>
        /// Defines a row of text for scrollers
        /// </summary>
        protected class TextRow
        {
            public string Text { get; set; }
            public FontSizes FontSize { get; set; }

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="text"></param>
            /// <param name="size"></param>
            public TextRow(string text, FontSizes size)
            {
                Text = text;
                FontSize = size;
            }

            /// <summary>
            /// Constructor with default text width
            /// </summary>
            /// <param name="text"></param>
            public TextRow(string text) : this(text, FontSizes.Normal) { }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="game"></param>
        public PanelBase( Game game)
        {
            screenX = game.GraphicsDevice.DisplayMode.Width;
            screenY = game.GraphicsDevice.DisplayMode.Height;

            fonts = new Dictionary<FontSizes, BitmapFont>
            {
                [FontSizes.Normal] = game.Content.Load<BitmapFont>("NormalText"),
                [FontSizes.Title] = game.Content.Load<BitmapFont>("TitleText"),
                [FontSizes.Subtitle] = game.Content.Load<BitmapFont>("SubtitleText"),
                [FontSizes.Header] = game.Content.Load<BitmapFont>("HeaderText")
            };

            boxBackground = new Texture2D(game.GraphicsDevice, 1, 1);
            Color[] colorData = new Color[1];
            colorData[0] = Color.WhiteSmoke;
            boxBackground.SetData<Color>(colorData);

            boxBackgroundInactive = new Texture2D(game.GraphicsDevice, 1, 1);
            colorData = new Color[1];
            colorData[0] = Color.LightGray;
            boxBackgroundInactive.SetData<Color>(colorData);

            Caption = "Selector";
            maxItems = 0;

            ReInit();
        }

        /// <summary>
        /// Resets panel data
        /// </summary>
        public virtual void Clear()
        {
            Selected = 0;
            maxItems = 0;
        }


        #region input check
        protected MouseState oldMouseState;
        protected MouseState mouseState;
        protected KeyboardState oldKeyboardState;
        protected KeyboardState keyboardState;
        protected GamePadState oldGamePadState;
        protected GamePadState gamePadState;

        /// <summary>
        /// ReInit input devices status
        /// </summary>
        public virtual void ReInit()
        {
            oldMouseState = Mouse.GetState();
            oldKeyboardState = Keyboard.GetState();
            oldGamePadState = GamePad.GetState(PlayerIndex.One);
        }

        /// <summary>
        /// Manages keyboard and controller input
        /// </summary>
        public virtual int Update()
        {
            gamePadState = GamePad.GetState(PlayerIndex.One);
            keyboardState = Keyboard.GetState();
            mouseState = Mouse.GetState();

            int ret = CheckInput(); 

            oldMouseState = mouseState;
            oldKeyboardState = keyboardState;
            oldGamePadState = gamePadState;

            return ret;
        }

        /// <summary>
        /// specific input check
        /// </summary>
        /// <returns></returns>
        protected virtual int CheckInput()
        {
            return 0;
        }

        /// <summary>
        /// True if scroll up
        /// </summary>
        /// <returns></returns>
        protected bool MouseScrollerUp()
        {
            return mouseState.ScrollWheelValue > oldMouseState.ScrollWheelValue;
        }

        /// <summary>
        /// True if scroll down
        /// </summary>
        /// <returns></returns>
        protected bool MouseScrollerDown()
        {
            return mouseState.ScrollWheelValue < oldMouseState.ScrollWheelValue;
        }

        /// <summary>
        /// True if key switched from up to down
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected bool KeyboardPressed(Keys key)
        {
            return oldKeyboardState.IsKeyUp(key) && keyboardState.IsKeyDown(key);
        }

        /// <summary>
        /// True if button switched fron up to down
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
        protected bool GamePadPressed(Buttons button)
        {
            return oldGamePadState.IsButtonUp(button) && gamePadState.IsButtonDown(button);
        }
        #endregion

        #region draw
        /// <summary>
        /// Draws the screen
        /// </summary>
        /// <param name="sb"></param>
        public virtual void Draw(SpriteBatch sb)
        {
            if (!string.IsNullOrWhiteSpace(Caption))
                sb.DrawString(fonts[FontSizes.Header], Caption, new Vector2(20,-5), Color.WhiteSmoke);
        }

        /// <summary>
        /// Draw a resized image
        /// </summary>
        /// <param name="sb">SpriteBatch to use for drawing</param>
        /// <param name="img"></param>
        /// <param name="area">Position and size of the image</param>
        protected void DrawResized(SpriteBatch sb, Texture2D img, Rectangle area)
        {
            sb.Draw(img, area, new Rectangle(0, 0, img.Width, img.Height), Color.White);
        }
        #endregion

        #region string wrap
        /// <summary>
        /// Wrap the text inside the box
        /// </summary>
        /// <param name="text"></param>
        /// <param name="TextBox"></param>
        /// <param name="font"></param>
        /// <returns></returns>
        protected string WrapText(string text, Rectangle TextBox, BitmapFont font)
        {
            string line = string.Empty;
            string returnString = string.Empty;
            string[] wordArray = text.Split(' ');

            foreach (string word in wordArray)
            {
                if (font.MeasureString(line + word).Width > TextBox.Width - 20)
                {
                    returnString = returnString + line + '\n';
                    line = string.Empty;
                }
                line = line + word + ' ';
            }
            return returnString + line;
        }

        /// <summary>
        /// Returns wrapped text by size
        /// </summary>
        /// <param name="text"></param>
        /// <param name="TextBox"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        protected string WrapText(string text, Rectangle TextBox, FontSizes size)
        {
            return WrapText(text, TextBox, fonts[size]);
        }

        /// <summary>
        /// Returns wrapped text with Normal font
        /// </summary>
        /// <param name="text"></param>
        /// <param name="TextBox"></param>
        /// <returns></returns>
        protected string WrapText(string text, Rectangle TextBox)
        {
            return WrapText(text, TextBox, FontSizes.Normal);
        }
        #endregion

        /// <summary>
        /// Gets a list of prewrapped text
        /// </summary>
        /// <param name="text"></param>
        /// <param name="size">Font size for line rendering</param>
        /// <returns></returns>
        protected List<TextRow> GetTextRows(string text, FontSizes size)
        {
            List<TextRow> ret = new List<TextRow>();
            string[] lines = text.Split('\n');
            foreach (var l in lines)
                ret.Add(new TextRow(l, size));
            return ret;
        }
    }
}


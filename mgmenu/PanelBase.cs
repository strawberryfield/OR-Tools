﻿// COPYRIGHT 2018 Roberto Ceccarelli - Casasoft.
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
using ORTS.Menu;
using System.Collections.Generic;

namespace Casasoft.MgMenu
{
    public class PanelBase
    {
        protected SpriteFont font;
        protected SpriteFont titleFont;
        protected Texture2D boxBackground;
        
        protected int screenX;
        protected int screenY;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="game"></param>
        public PanelBase( Game game)
        {
            screenX = game.GraphicsDevice.DisplayMode.Width;
            screenY = game.GraphicsDevice.DisplayMode.Height;

            font = game.Content.Load<SpriteFont>("NormalText");
            titleFont = game.Content.Load<SpriteFont>("TitleText");

            boxBackground = new Texture2D(game.GraphicsDevice, 1, 1);
            Color[] colorData = new Color[1];
            colorData[0] = Color.WhiteSmoke;
            boxBackground.SetData<Color>(colorData);

            ReInit();
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
        public void ReInit()
        {
            oldMouseState = Mouse.GetState();
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
            // Your code
        }

        /// <summary>
        /// Wrap the text inside the box
        /// </summary>
        /// <param name="text"></param>
        /// <param name="TextBox"></param>
        /// <returns></returns>
        protected string WrapText(string text, Rectangle TextBox)
        {
            string line = string.Empty;
            string returnString = string.Empty;
            string[] wordArray = text.Split(' ');

            foreach (string word in wordArray)
            {
                if (font.MeasureString(line + word).Length() > TextBox.Width)
                {
                    returnString = returnString + line + '\n';
                    line = string.Empty;
                }
                line = line + word + ' ';
            }
            return returnString + line;
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

    }
}


// COPYRIGHT 2019 Roberto Ceccarelli - Casasoft.
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

namespace Casasoft.Panels2D
{
    public class PanelBase
    {
        protected Dictionary<FontSizes, BitmapFont> fonts;
        protected Game game;

        protected int screenX;
        protected int screenY;

        public string Caption { get; set; }
        public int Selected { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="game"></param>
        public PanelBase(Game game)
        {
            this.game = game;

            screenX = game.GraphicsDevice.DisplayMode.Width;
            screenY = game.GraphicsDevice.DisplayMode.Height;

            fonts = new Dictionary<FontSizes, BitmapFont>
            {
                [FontSizes.Normal] = game.Content.Load<BitmapFont>("NormalText"),
                [FontSizes.Title] = game.Content.Load<BitmapFont>("TitleText"),
                [FontSizes.Subtitle] = game.Content.Load<BitmapFont>("SubtitleText"),
                [FontSizes.Header] = game.Content.Load<BitmapFont>("HeaderText")
            };

            Caption = "Selector";

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
                sb.DrawString(fonts[FontSizes.Header], Caption, new Vector2(20, -5), Color.WhiteSmoke);
        }

        /// <summary>
        /// Draw a resized image
        /// </summary>
        /// <param name="sb">SpriteBatch to use for drawing</param>
        /// <param name="img"></param>
        /// <param name="area">Position and size of the image</param>
        /// <param name="color">Overlay color</param>
        protected void DrawResized(SpriteBatch sb, Texture2D img, Rectangle area, Color color)
        {
            sb.Draw(img, area, new Rectangle(0, 0, img.Width, img.Height), color);
        }

        /// <summary>
        /// Draw a resized image
        /// </summary>
        /// <param name="sb">SpriteBatch to use for drawing</param>
        /// <param name="img"></param>
        /// <param name="area">Position and size of the image</param>
        protected void DrawResized(SpriteBatch sb, Texture2D img, Rectangle area)
        {
            DrawResized(sb, img, area, Color.White);
        }
        #endregion

    }
}

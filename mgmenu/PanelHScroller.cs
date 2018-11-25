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

namespace Casasoft.MgMenu
{
    public class PanelHScroller : PanelBase
    {
        protected int maxItems;
        protected Rectangle textBox;
        protected Texture2D noImage;

        protected int thumbSizeX = 120;
        protected int thumbSizeY = 90;
        protected int thumbStep;
        protected int thumbX;
        protected int thumbY = 90;
        protected int detailSizeX = 640;
        protected int detailSizeY = 480;

        public int Selected { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="spriteBatch"></param>
        public PanelHScroller(Game game) : base(game)
        {
            maxItems = 0;

            thumbSizeX = (thumbSizeX * screenY) / 768;
            thumbSizeY = (thumbSizeY * screenY) / 768;
            thumbStep = (thumbSizeX * 115) / 100;
            thumbY = (thumbY * screenY) / 768;
            thumbX = (screenX - thumbSizeX) / 2;

            textBox = new Rectangle(detailSizeX + 40, 200, detailSizeX, detailSizeY);
            noImage = game.Content.Load<Texture2D>("no-image");
        }

        /// <summary>
        /// Manages keyboard and controller input
        /// </summary>
        public override int Update()
        {
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);
            KeyboardState keyboardState = Keyboard.GetState();
            MouseState mouseState = Mouse.GetState();

            if ((gamePadState.IsButtonDown(Buttons.LeftThumbstickLeft) || gamePadState.IsButtonDown(Buttons.DPadLeft) ||
                keyboardState.IsKeyDown(Keys.Left) ||
                mouseState.ScrollWheelValue > oldMouseState.ScrollWheelValue) &&
                Selected > 0)
                Selected--;

            if ((gamePadState.IsButtonDown(Buttons.LeftThumbstickRight) || gamePadState.IsButtonDown(Buttons.DPadRight) ||
                keyboardState.IsKeyDown(Keys.Right) ||
                mouseState.ScrollWheelValue < oldMouseState.ScrollWheelValue) &&
                Selected < maxItems - 1)
                Selected++;

            if (gamePadState.IsButtonDown(Buttons.LeftShoulder) || gamePadState.IsButtonDown(Buttons.LeftStick) ||
                keyboardState.IsKeyDown(Keys.Home))
                Selected = 0;

            if (gamePadState.IsButtonDown(Buttons.RightShoulder) || gamePadState.IsButtonDown(Buttons.RightStick) ||
                keyboardState.IsKeyDown(Keys.End))
                Selected = maxItems - 1;

            oldMouseState = mouseState;
            return 0;
        }

    }
}

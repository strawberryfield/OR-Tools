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

namespace Casasoft.Panels2D
{
    public class PanelHScroller : PanelScroller
    {
        protected Texture2D noImage;

        protected int thumbSizeX = 120;
        protected int thumbSizeY = 90;
        protected int thumbStep;
        protected int thumbX;
        protected int thumbY = 90;
        protected int detailSizeX = 640;
        protected int detailSizeY = 480;


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="spriteBatch"></param>
        public PanelHScroller(Game game) : base(game)
        {

            thumbSizeX = (thumbSizeX * screenY) / 768;
            thumbSizeY = (thumbSizeY * screenY) / 768;
            thumbStep = (thumbSizeX * 115) / 100;
            thumbY = (thumbY * screenY) / 768;
            thumbX = (screenX - thumbSizeX) / 2;

            textBox = new Rectangle(detailSizeX + 40, 200, screenX - detailSizeX - 60, detailSizeY);
            noImage = game.Content.Load<Texture2D>("no-image");

            Clear();
        }

        /// <summary>
        /// Manages keyboard and controller input
        /// </summary>
        protected override int CheckInput()
        {
            if (GamePadPressed(Buttons.Back) || KeyboardPressed(Keys.Escape))
                return -1;

            if (GamePadPressed(Buttons.Start) || KeyboardPressed(Keys.Enter))
                return 1;

            if ((GamePadPressed(Buttons.LeftThumbstickLeft) || GamePadPressed(Buttons.DPadLeft) ||
                KeyboardPressed(Keys.Left) ||
                MouseScrollerUp()) &&
                Selected > 0)
                Selected--;

            if ((GamePadPressed(Buttons.LeftThumbstickRight) || GamePadPressed(Buttons.DPadRight) ||
                KeyboardPressed(Keys.Right) ||
                MouseScrollerDown()) &&
                Selected < maxItems - 1)
                Selected++;

            if (GamePadPressed(Buttons.LeftShoulder) || GamePadPressed(Buttons.LeftStick) ||
                KeyboardPressed(Keys.Home))
                Selected = 0;

            if (GamePadPressed(Buttons.RightShoulder) || GamePadPressed(Buttons.RightStick) ||
                KeyboardPressed(Keys.End))
                Selected = maxItems - 1;

            return 0;
        }

    }
}

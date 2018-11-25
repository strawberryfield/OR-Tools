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
using ORTS.Menu;
using System.Collections.Generic;

namespace Casasoft.MgMenu
{
    public class SelRoute : PanelBase
    {
        private List<Route> routes;

        private int maxRoutes;
        private Rectangle textBox;
        private Texture2D noImage;

        private int thumbSizeX = 120;
        private int thumbSizeY = 90;
        private int thumbStep;
        private int thumbX;
        private int thumbY = 90;
        private int detailSizeX = 640;
        private int detailSizeY = 480;

        public int Selected { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="spriteBatch"></param>
        public SelRoute(List<Route> Routes, Game game) : base(game)
        {
            routes = Routes;
            maxRoutes = routes.Count;

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
        public override void Update()
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
                Selected < maxRoutes - 1)
                Selected++;

            oldMouseState = mouseState;
        }


        /// <summary>
        /// Draws the screen
        /// </summary>
        /// <param name="sb"></param>
        public override void Draw(SpriteBatch sb)
        {
            int x = thumbX - Selected * thumbStep;

            foreach (var dir in routes)
            {
                if (x + thumbSizeX >= 0 && x < screenX)
                    DrawResized(sb, dir.Texture == null ? noImage : dir.Texture, 
                        new Rectangle(x, thumbY, thumbSizeX, thumbSizeY));
                x += thumbStep;
            }

            sb.Draw(boxBackground, textBox, Color.White);

            Route current = routes[Selected];
            DrawResized(sb, current.Texture == null ? noImage : current.Texture,
                new Rectangle(20, 200, detailSizeX, detailSizeY));

            if (!string.IsNullOrWhiteSpace(current.Name))
                sb.DrawString(titleFont, current.Name, new Vector2(detailSizeX + 50, 200), Color.Black);
            if (!string.IsNullOrWhiteSpace(current.Description))
                sb.DrawString(font, this.WrapText(current.Description, textBox),
                    new Vector2(detailSizeX + 50, 250), Color.Black);
        }

     }
}

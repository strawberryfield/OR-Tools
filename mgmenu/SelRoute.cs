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
    public class SelRoute
    {
        private List<Route> routes;

        private int maxRoutes;
        private MouseState oldMouseState;
        private SpriteFont font;
        private Texture2D boxBackground;
        private Rectangle textBox;

        private int thumbSizeX = 120;
        private int thumbSizeY = 90;
        private int thumbStep;
        private int thumbX;
        private int thumbY = 90;
        private int screenX;
        private int screenY;
        private int detailSizeX = 640;
        private int detailSizeY = 480;

        public int Selected { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="spriteBatch"></param>
        public SelRoute(List<Route> Routes, Game game)
        {
            routes = Routes;
            maxRoutes = routes.Count;

            screenX = game.GraphicsDevice.DisplayMode.Width;
            screenY = game.GraphicsDevice.DisplayMode.Height;

            thumbSizeX = (thumbSizeX * screenY) / 768;
            thumbSizeY = (thumbSizeY * screenY) / 768;
            thumbStep = (thumbSizeX * 115) / 100;
            thumbY = (thumbY * screenY) / 768;
            thumbX = (screenX - thumbSizeX) / 2;

            font = game.Content.Load<SpriteFont>("NormalText");

            boxBackground = new Texture2D(game.GraphicsDevice, 1, 1);
            Color[] colorData = new Color[1];
            colorData[0] = Color.WhiteSmoke;
            boxBackground.SetData<Color>(colorData);
            textBox = new Rectangle(detailSizeX + 40, 200, detailSizeX, detailSizeY);

            ReInit();
        }

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
        public void Update()
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
        public void Draw(SpriteBatch sb)
        {
            int x = thumbX - Selected * thumbStep;

            foreach (var dir in routes)
            {
                if (x + thumbSizeX >= 0 && x < screenX)
                    if (dir.Texture != null)
                        sb.Draw(dir.Texture,
                         new Rectangle(x, thumbY, thumbSizeX, thumbSizeY),
                         new Rectangle(0, 0, dir.Texture.Width, dir.Texture.Height),
                         Color.White);
                x += thumbStep;
            }

            sb.Draw(boxBackground, textBox, Color.White);

            Route current = routes[Selected];
            if (current.Texture != null)
                sb.Draw(current.Texture, 
                    new Rectangle(20, 200, detailSizeX, detailSizeY),
                    new Rectangle(0, 0, current.Texture.Width, current.Texture.Height),
                    Color.White);

            if (!string.IsNullOrWhiteSpace(current.Name))
                sb.DrawString(font, current.Name, new Vector2(detailSizeX + 50, 200), Color.Black);
            if (!string.IsNullOrWhiteSpace(current.Description))
                sb.DrawString(font, this.WrapText(current.Description, textBox),
                    new Vector2(detailSizeX + 50, 220), Color.Black);
        }

        private string WrapText(string text, Rectangle TextBox)
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
    }
}
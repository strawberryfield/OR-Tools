using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ORTS.Menu;

namespace Casasoft.MgMenu
{
    public class SelRoute
    {
        private List<Route> routes;

        private int maxRoutes;
        private MouseState oldMouseState;

        private int thumbSizeX = 120;
        private int thumbSizeY = 90;
        private int thumbStep;
        private int thumbX;
        private int thumbY = 90;
        private int screenX;
        private int screenY;

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
                //spriteBatch.DrawString(font, string.Format("{0}: {1}",dir.Name,dir.Path), new Vector2(10, y), Color.Black);
                //y = y + 20;
                if (x + thumbSizeX >= 0 && x < screenX)
                    if (dir.Texture != null)
                        sb.Draw(dir.Texture,
                         new Rectangle(x, thumbY, thumbSizeX, thumbSizeY),
                         new Rectangle(0, 0, dir.Texture.Width, dir.Texture.Height),
                         Color.White);
                x += thumbStep;
            }

            Route current = routes[Selected];
            if (current.Texture != null)
                sb.Draw(current.Texture,
                 new Rectangle(20, 200, 640, 480),
                 new Rectangle(0, 0, current.Texture.Width, current.Texture.Height),
                 Color.White);

        }
    }
}

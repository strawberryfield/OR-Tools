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

        public int Selected { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="spriteBatch"></param>
        public SelRoute(List<Route> Routes)
        {
            routes = Routes;
            maxRoutes = routes.Count;
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

            if ((gamePadState.Buttons.LeftStick == ButtonState.Pressed ||
                keyboardState.IsKeyDown(Keys.Left) ||
                mouseState.ScrollWheelValue > oldMouseState.ScrollWheelValue) &&
                Selected > 0)
                Selected--;

            if ((gamePadState.Buttons.RightStick == ButtonState.Pressed ||
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
            int y = 10;
            int x = 300;
            int sizeX = 48;
            int sizeY = 36;
            int stepX = sizeX + 12;
            x = x - Selected * stepX;

            foreach (var dir in routes)
            {
                //spriteBatch.DrawString(font, string.Format("{0}: {1}",dir.Name,dir.Path), new Vector2(10, y), Color.Black);
                //y = y + 20;
                if (dir.Texture != null) sb.Draw(dir.Texture,
                     new Rectangle(x, y, sizeX, sizeY),
                     new Rectangle(0, 0, dir.Texture.Width, dir.Texture.Height),
                     Color.White);
                x += stepX;
            }

        }
    }
}

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
        private int selected;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="spriteBatch"></param>
        public SelRoute(List<Route> Routes)
        {
            routes = Routes;
            maxRoutes = routes.Count;
        }

        /// <summary>
        /// Manages keyboard and controller input
        /// </summary>
        public void Update()
        {
            if ((GamePad.GetState(PlayerIndex.One).Buttons.LeftStick == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Left)) &&
                selected > 0)
                selected--;

            if ((GamePad.GetState(PlayerIndex.One).Buttons.RightStick == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Right)) &&
                selected < maxRoutes - 1)
                selected++;
        }

        
        public void Draw(SpriteBatch sb)
        {
            int y = 10;
            int x = 300;
            int sizeX = 48;
            int sizeY = 36;
            int stepX = sizeX + 12;
            x = x - selected * stepX;

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

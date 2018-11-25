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
    public class SelRoute : PanelHScroller
    {
        private List<Route> routes;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="spriteBatch"></param>
        public SelRoute(List<Route> Routes, Game game) : base(game)
        {
            routes = Routes;
            maxItems = routes.Count;
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

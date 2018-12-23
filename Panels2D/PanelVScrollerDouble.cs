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
using MonoGame.Extended.BitmapFonts;

namespace Casasoft.Panels2D
{
    public class PanelVScrollerDouble : PanelVScroller
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="game"></param>
        public PanelVScrollerDouble(Game game) : base(game) { }

        /// <summary>
        /// Return the text for the right scroller item
        /// </summary>
        /// <param name="pos">line to draw</param>
        /// <returns></returns>
        protected virtual string RightScrollerItemText(int pos)
        {
            return "";
        }

        /// <summary>
        /// Panel drawing
        /// </summary>
        /// <param name="sb"></param>
        protected override void ScrollerItemDetail(SpriteBatch sb)
        {
            int y = boxesY + 5;
            for (int j = 0; j < maxItemsRight; j++)
            {
                if (maxItemsRight <= maxRows || (j >= SelectedRight && j < SelectedRight + maxRows))
                {
                    string txt = RightScrollerItemText(j);
                    if (j == SelectedRight)
                        sb.DrawString(fonts[FontSizes.Title], txt, new Vector2(textBox.Left + 40, y), Color.Blue);
                    else
                        sb.DrawString(fonts[FontSizes.Title], txt, new Vector2(textBox.Left + 5, y), Color.Black);
                    y += itemHeight;
                }
            }

         }

    }
}

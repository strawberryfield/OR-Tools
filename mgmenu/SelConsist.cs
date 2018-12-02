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
using MonoGame.Extended.BitmapFonts;
using ORTS.Menu;
using System.Collections.Generic;

namespace Casasoft.MgMenu
{
    public class SelConsist : PanelVScroller
    {
        private List<Consist> consists;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="game"></param>
        public SelConsist(Game game) : base(game)
        {
            consists = new List<Consist>();
            maxItems = consists.Count;

            Caption = "Choose a train";
        }

        /// <summary>
        /// Resets panel data
        /// </summary>
        public override void Clear()
        {
            base.Clear();
            if(consists != null) consists.Clear();
        }

        /// <summary>
        /// Assign data list
        /// </summary>
        /// <param name="Consists"></param>
        public void SetList(List<Consist> Consists)
        {
            consists = Consists;
            maxItems = consists.Count;
        }

        /// <summary>
        /// Draws the consist detail
        /// </summary>
        /// <param name="sb"></param>
        protected override void ScrollerItemDetail(SpriteBatch sb)
        {
            Consist current = consists[Selected];
            int y = boxesY + 5;
            if (!string.IsNullOrWhiteSpace(current.Name))
            {
                sb.DrawString(font, this.WrapText(current.Name, textBox),
                    new Vector2(textBox.Left + 5, y), Color.Black);
                y += (int)font.MeasureString(current.Name).Height + 5;
            }
        }

        /// <summary>
        /// Returns activity name
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        protected override string ScrollerItemText(int pos)
        {
            return consists[pos].Name;
        }

    }
}

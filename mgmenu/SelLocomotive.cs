﻿// COPYRIGHT 2018,2019 Roberto Ceccarelli - Casasoft.
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

using Casasoft.Panels2D;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ORTS.Menu;
using System.Collections.Generic;

namespace Casasoft.MgMenu
{
    public class SelLocomotive : PanelVScroller
    {
        private List<Locomotive> locomotives;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="game"></param>
        public SelLocomotive(Game game) : base(game)
        {
            locomotives = new List<Locomotive>();
            maxItems = locomotives.Count;

            Caption = "Choose a locomotive";
        }

        /// <summary>
        /// Resets panel data
        /// </summary>
        public override void Clear()
        {
            base.Clear();
            if(locomotives != null) locomotives.Clear();
        }

        /// <summary>
        /// Assign data list
        /// </summary>
        /// <param name="Locomotives"></param>
        public void SetList(List<Locomotive> Locomotives)
        {
            locomotives = Locomotives;
            maxItems = locomotives.Count;
        }

        /// <summary>
        /// Draws the loco detail
        /// </summary>
        /// <param name="sb"></param>
        protected override void ScrollerItemDetail(SpriteBatch sb)
        {
            Locomotive current = locomotives[Selected];

            TextBox detail = new TextBox(sb, fonts, textBox);
            if (!string.IsNullOrWhiteSpace(current.Description))
                detail.AddTextRowsWrapped(current.Description);

            detail.Draw();
        }

        /// <summary>
        /// Returns loco name
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        protected override string ScrollerItemText(int pos)
        {
            return locomotives[pos].Name;
        }

        /// <summary>
        /// Returns selected locomotive
        /// </summary>
        public Locomotive Locomotive
        {
            get
            {
                return locomotives.Count > 0 ? locomotives[Selected] : null;
            }
        }
    }
}

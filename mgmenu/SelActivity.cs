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
using MonoGame.Extended.BitmapFonts;
using ORTS.Menu;
using System.Collections.Generic;

namespace Casasoft.MgMenu
{
    public class SelActivity : PanelVScroller
    {
        private List<Activity> activities;
 
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="game"></param>
        public SelActivity(Game game) : base(game)
        {
            activities = new List<Activity>();
            maxItems = activities.Count;

            Caption = "Choose an activity";
        }

        /// <summary>
        /// Resets panel data
        /// </summary>
        public override void Clear()
        {
            base.Clear();
            if(activities != null) activities.Clear();
        }

        /// <summary>
        /// Assign data list
        /// </summary>
        /// <param name="Activities"></param>
        public void SetList(List<Activity> Activities)
        {
            activities = Activities;
            maxItems = activities.Count;
        }

        /// <summary>
        /// Draws the activity detail
        /// </summary>
        /// <param name="sb"></param>
        protected override void ScrollerItemDetail(SpriteBatch sb)
        {
            BitmapFont font = fonts[FontSizes.Normal];
            BitmapFont subtitleFont = fonts[FontSizes.Normal];

            Activity current = activities[Selected];
            int y = boxesY + 5;
            if (!string.IsNullOrWhiteSpace(current.Description))
            {
                sb.DrawString(font, this.WrapText(current.Description, textBox),
                    new Vector2(textBox.Left + 5, y), Color.Black);
                y += (int)font.MeasureString(current.Description).Height + 5;
            }

            if (!string.IsNullOrWhiteSpace(current.Briefing))
            {
                sb.DrawString(subtitleFont, "Briefing", new Vector2(textBox.Left + 5, y), Color.Black);
                sb.DrawString(font, this.WrapText(current.Briefing, textBox),
                    new Vector2(textBox.Left + 5, y + 25), Color.Black);
                y += (int)font.MeasureString(current.Briefing).Height + 5;
            }
        }

        /// <summary>
        /// Returns activity name
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        protected override string ScrollerItemText(int pos)
        {
            return activities[pos].Name;
        }

    }
}

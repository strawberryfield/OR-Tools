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
    public class SelActivity : PanelBase
    {
        private List<Activity> activities;
        private int maxItems;
        private Rectangle textBox;
        private Rectangle scroller;
        protected int detailSizeX = 640;
        protected int detailSizeY = 580;
        protected int itemHeight = 40;
        protected int boxesY = 100;

        public int Selected { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="game"></param>
        public SelActivity(Game game) : base(game)
        {
            activities = new List<Activity>();
            maxItems = activities.Count;

            scroller = new Rectangle(20, boxesY, detailSizeX, detailSizeY);
            textBox = new Rectangle(detailSizeX + 40, boxesY, detailSizeX, detailSizeY);
            Selected = 0;
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
        /// Manages keyboard and controller input
        /// </summary>
        protected override int CheckInput()
        {
            if (GamePadPressed(Buttons.Back) || KeyboardPressed(Keys.Escape))
                return -1;

            if (GamePadPressed(Buttons.Start) || KeyboardPressed(Keys.Enter))
                return 1;

            if ((GamePadPressed(Buttons.LeftThumbstickUp) || GamePadPressed(Buttons.DPadUp) ||
                KeyboardPressed(Keys.Up) ||
                MouseScrollerUp()) &&
                Selected > 0)
                Selected--;

            if ((GamePadPressed(Buttons.LeftThumbstickDown) || GamePadPressed(Buttons.DPadDown) ||
                KeyboardPressed(Keys.Down) ||
                MouseScrollerDown()) &&
                Selected < maxItems - 1)
                Selected++;

            if (GamePadPressed(Buttons.LeftShoulder) || GamePadPressed(Buttons.LeftStick) ||
                KeyboardPressed(Keys.Home))
                Selected = 0;

            if (GamePadPressed(Buttons.RightShoulder) || GamePadPressed(Buttons.RightStick) ||
                KeyboardPressed(Keys.End))
                Selected = maxItems - 1;

            return 0;
        }

        public override void Draw(SpriteBatch sb)
        {
            sb.Draw(boxBackground, scroller, Color.White);
            sb.Draw(boxBackground, textBox, Color.White);

            int y = boxesY + 5;
            for(int j = 0; j < maxItems; j++)
            {
                Activity item = activities[j];
                string txt = string.IsNullOrWhiteSpace(item.Name) ? "???" : item.Name;
                if (j == Selected)
                    sb.DrawString(titleFont, txt, new Vector2(scroller.Left + 40, y), Color.Blue);
                else
                    sb.DrawString(titleFont, txt, new Vector2(scroller.Left + 5, y), Color.Black);
                y += itemHeight;
            }

            Activity current = activities[Selected];
            y = boxesY + 5;
            if (!string.IsNullOrWhiteSpace(current.Description))
            {
                sb.DrawString(font, this.WrapText(current.Description, textBox),
                    new Vector2(textBox.Left + 5, y), Color.Black);
                y += (int)font.MeasureString(current.Description).Y + 5;
            }

            if (!string.IsNullOrWhiteSpace(current.Briefing))
            {
                sb.DrawString(titleFont, "Briefing", new Vector2(textBox.Left + 5, y), Color.Black);
                sb.DrawString(font, this.WrapText(current.Briefing, textBox),
                    new Vector2(textBox.Left + 5, y + 25), Color.Black);
                y += (int)font.MeasureString(current.Briefing).Y + 5;
            }

        }

    }
}

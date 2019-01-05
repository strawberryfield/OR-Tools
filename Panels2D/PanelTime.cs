// COPYRIGHT 2019 Roberto Ceccarelli - Casasoft.
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
using System;

namespace Casasoft.Panels2D
{
    public class PanelTime : PanelBase
    {
        public int Hours { get; set; }
        public int Minutes { get; set; }
        public int Seconds { get; set; }

        public DateTime Time
        {
            get
            {
                return new DateTime(1, 1, 1, Hours, Minutes, Seconds);
            }

            set
            {
                Hours = value.Hour;
                Minutes = value.Minute;
                Seconds = value.Second;
            }
        }

        protected BitmapFont font;
        protected enum FieldType { Hours, Minutes, Seconds }
        protected FieldType activeField;

        protected int itemHeight;
        protected int itemWidth;
        protected int itemY;
        protected int itemHoursX;
        protected int itemSepX;
        protected int itemMinutesX;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="game"></param>
        public PanelTime(Game game) : base(game)
        {
            Hours = 0;
            Minutes = 0;
            Seconds = 0;
            activeField = FieldType.Hours;

            font = game.Content.Load<BitmapFont>("7SegmentFont");
            var size = font.MeasureString("8");
            itemHeight = Convert.ToInt16(size.Height);
            itemWidth = Convert.ToInt16(size.Width);
            itemY = (screenY - itemHeight) / 2;
            itemSepX = (screenX - itemWidth) / 2;
            itemHoursX = itemSepX - itemWidth * 2;
            itemMinutesX = itemSepX + itemWidth;

            Caption = "Time";
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

            // Panel switching
            if ((GamePadPressed(Buttons.LeftThumbstickRight) || GamePadPressed(Buttons.DPadRight) ||
                KeyboardPressed(Keys.Right)) &&
                activeField == FieldType.Hours)
                activeField = FieldType.Minutes;

            if ((GamePadPressed(Buttons.LeftThumbstickLeft) || GamePadPressed(Buttons.DPadLeft) ||
                KeyboardPressed(Keys.Left)) &&
                activeField == FieldType.Minutes)
                activeField = FieldType.Hours;

            if (KeyboardPressed(Keys.Tab))
                activeField = (activeField == FieldType.Hours ? FieldType.Minutes : FieldType.Hours);

            // Up Down scrolling
            if (GamePadPressed(Buttons.LeftThumbstickUp) || GamePadPressed(Buttons.DPadUp) ||
                KeyboardPressed(Keys.Up) || MouseScrollerUp())
            {
                if (activeField == FieldType.Hours && Hours > 0)
                    Hours--;
                if (activeField == FieldType.Minutes && Minutes > 0)
                    Minutes--;
            }

            if (GamePadPressed(Buttons.LeftThumbstickDown) || GamePadPressed(Buttons.DPadDown) ||
                KeyboardPressed(Keys.Down) || MouseScrollerDown())
            {
                if (activeField == FieldType.Hours && Hours < 23)
                    Hours++;
                if (activeField == FieldType.Minutes && Minutes < 59)
                    Minutes++;
            }

            if (GamePadPressed(Buttons.LeftShoulder) || GamePadPressed(Buttons.LeftStick) ||
                KeyboardPressed(Keys.Home))
            {
                if (activeField == FieldType.Hours)
                    Hours = 0;
                if (activeField == FieldType.Minutes)
                    Minutes = 0;
            }

            if (GamePadPressed(Buttons.RightShoulder) || GamePadPressed(Buttons.RightStick) ||
                KeyboardPressed(Keys.End))
            {
                if (activeField == FieldType.Hours)
                    Hours = 23;
                if (activeField == FieldType.Minutes)
                    Minutes = 59;
            }

            if (KeyboardPressed(Keys.PageDown))
            {
                if (activeField == FieldType.Hours)
                {
                    Hours += 6;
                    if (Hours > 23) Hours = 23;
                }
                if (activeField == FieldType.Minutes)
                {
                    Minutes += 15;
                    if (Minutes > 59) Minutes = 59;
                }

            }

            if (KeyboardPressed(Keys.PageUp))
            {
                if (activeField == FieldType.Hours)
                {
                    Hours -= 6;
                    if (Hours < 0) Hours = 0;
                }
                if (activeField == FieldType.Minutes)
                {
                    Minutes -= 15;
                    if (Minutes < 0) Minutes = 0;
                }

            }

            return 0;
        }

        /// <summary>
        /// Draws the screen
        /// </summary>
        /// <param name="sb"></param>
        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);

            string text = string.Format("{0,2}:{1:D2}:{2:D2}", Hours, Minutes, Seconds);

            Color hoursColor = activeField == FieldType.Hours ? Color.WhiteSmoke : Color.LightGray;
            if (Hours < 10)
                sb.DrawString(font, text.Substring(1, 1), new Vector2(itemHoursX + itemWidth, itemY), hoursColor);
            else
                sb.DrawString(font, text.Substring(0, 2), new Vector2(itemHoursX, itemY), hoursColor);
            sb.DrawString(font, ":", new Vector2(itemSepX, itemY), Color.LightGray);
            sb.DrawString(font, text.Substring(3, 2), new Vector2(itemMinutesX, itemY),
                activeField == FieldType.Minutes ? Color.WhiteSmoke : Color.LightGray);
        }
    }
}

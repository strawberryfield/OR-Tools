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
using System.Collections.Generic;

namespace Casasoft.Panels2D
{
    public class PanelButtons : PanelBase
    {
        protected class ButtonData
        {
            public Texture2D Picture;
            public string Caption;

            public ButtonData(Texture2D picture, string caption)
            {
                Picture = picture;
                Caption = caption;
            }
        }

        protected List<ButtonData> buttonList;

        protected FontSizes FontSize { get; set; }
        protected int ImageSize { get; set; }
        protected int ButtonsY;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="game"></param>
        public PanelButtons(Game game) : base(game)
        {
            buttonList = new List<ButtonData>();
            FontSize = FontSizes.Subtitle;
            ImageSize = 200;
            ButtonsY = (screenY - ImageSize) / 2;
        }

        /// <summary>
        /// Draws the panel
        /// </summary>
        /// <param name="sb"></param>
        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
            int nButtons = buttonList.Count;
            int x = (screenX - ImageSize * nButtons - ImageSize / 10 * (nButtons - 1)) / 2;
            Rectangle buttonSize = new Rectangle(x, ButtonsY, ImageSize, ImageSize);
            Vector2 textPos = new Vector2(x, ButtonsY + ImageSize + ImageSize / 10);

            for(int j=0; j<nButtons; j++)
            {
                DrawResized(sb, buttonList[j].Picture, buttonSize, j == Selected ? Color.White : Color.Gray);

                int textWidth = Convert.ToInt16(fonts[FontSize].MeasureString(buttonList[j].Caption).Width);
                textPos.X = (ImageSize - textWidth) / 2 + buttonSize.X;
                sb.DrawString(fonts[FontSize], buttonList[j].Caption, textPos, j == Selected ? Color.WhiteSmoke : Color.Black);

                buttonSize.X += ImageSize + ImageSize / 10;
            }
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

            int maxItems = buttonList.Count;

            if ((GamePadPressed(Buttons.LeftThumbstickLeft) || GamePadPressed(Buttons.DPadLeft) ||
                KeyboardPressed(Keys.Left)) &&
                Selected > 0)
                Selected--;

            if ((GamePadPressed(Buttons.LeftThumbstickRight) || GamePadPressed(Buttons.DPadRight) ||
                KeyboardPressed(Keys.Right)) &&
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

    }
}

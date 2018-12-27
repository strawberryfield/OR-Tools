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
    public class PanelVScroller : PanelBase
    {
        protected int detailSizeX;
        protected int detailSizeY;
        protected int itemHeight = 40;
        protected int boxesY = 100;
        protected int maxRows;
        protected int maxItemsRight;
        protected int maxRowsRight;

        protected enum Boxes { Left, Right }
        protected Boxes currentBox;

        public int SelectedRight { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="game"></param>
        public PanelVScroller(Game game) : base(game)
        {
            detailSizeX = screenX / 2 - 30;
            detailSizeY = screenY - boxesY * 2;
            leftBox = new Rectangle(20, boxesY, detailSizeX, detailSizeY);
            textBox = new Rectangle(detailSizeX + 40, boxesY, screenX - detailSizeX - 60, detailSizeY);
            maxRows = (detailSizeY-10) / itemHeight;
            Clear();
        }

        /// <summary>
        /// Reset the panel
        /// </summary>
        public override void Clear()
        {
            base.Clear();
            currentBox = Boxes.Left;
            SelectedRight = 0;
            maxItemsRight = 0;
        }

        /// <summary>
        /// Called when Enter is pressed
        /// </summary>
        /// <returns></returns>
        protected virtual int EnterPressed()
        {
            return 1;
        }

        /// <summary>
        /// Called when enter key is pressed with right panel active
        /// </summary>
        /// <returns></returns>
        protected virtual int EnterPressedOnRight()
        {
            return 0;
        }

        /// <summary>
        /// Called on left panel selection changed
        /// </summary>
        protected virtual void OnSelectedChanged() { }

        /// <summary>
        /// Called on right panel selection changed
        /// </summary>
        protected virtual void OnSelectedRightChanged() { }

        /// <summary>
        /// Manages keyboard and controller input
        /// </summary>
        protected override int CheckInput()
        {
            int oldSelected = Selected;
            int oldSelectedRight = SelectedRight;

            if (GamePadPressed(Buttons.Back) || KeyboardPressed(Keys.Escape))
                return -1;

            if (GamePadPressed(Buttons.Start) || KeyboardPressed(Keys.Enter))
            {
                if (currentBox == Boxes.Left)
                    return EnterPressed();
                else
                    return EnterPressedOnRight();
            }

            // Panel switching
            if ((GamePadPressed(Buttons.LeftThumbstickRight) || GamePadPressed(Buttons.DPadRight) ||
                KeyboardPressed(Keys.Right)) &&
                currentBox == Boxes.Left)
                currentBox = Boxes.Right;

            if ((GamePadPressed(Buttons.LeftThumbstickLeft) || GamePadPressed(Buttons.DPadLeft) ||
                KeyboardPressed(Keys.Left)) &&
                currentBox == Boxes.Right)
                currentBox = Boxes.Left;

            if (KeyboardPressed(Keys.Tab))
                currentBox = (currentBox == Boxes.Left ? Boxes.Right : Boxes.Left);

            // Up Down scrolling
            if (GamePadPressed(Buttons.LeftThumbstickUp) || GamePadPressed(Buttons.DPadUp) ||
                KeyboardPressed(Keys.Up) || MouseScrollerUp())
            {
                if (currentBox == Boxes.Left && Selected > 0)
                    Selected--;
                if (currentBox == Boxes.Right && SelectedRight > 0)
                    SelectedRight--;
            }

            if (GamePadPressed(Buttons.LeftThumbstickDown) || GamePadPressed(Buttons.DPadDown) ||
                KeyboardPressed(Keys.Down) || MouseScrollerDown())
            {
                if (currentBox == Boxes.Left && Selected < maxItems - 1)
                    Selected++;
                if (currentBox == Boxes.Right && SelectedRight < maxItemsRight - 1)
                    SelectedRight++;
            }

            if (GamePadPressed(Buttons.LeftShoulder) || GamePadPressed(Buttons.LeftStick) ||
                KeyboardPressed(Keys.Home))
            {
                if (currentBox == Boxes.Left)
                    Selected = 0;
                if (currentBox == Boxes.Right)
                    SelectedRight = 0;
            }

            if (GamePadPressed(Buttons.RightShoulder) || GamePadPressed(Buttons.RightStick) ||
                KeyboardPressed(Keys.End))
            {
                if (currentBox == Boxes.Left)
                    Selected = maxItems - 1;
                if (currentBox == Boxes.Right)
                    SelectedRight = maxItemsRight - 1;
            }

            if(KeyboardPressed(Keys.PageDown))
            {
                if (currentBox == Boxes.Left)
                {
                    Selected += (maxRows - 1);
                    if (Selected > maxItems - 1) Selected = maxItems - 1;
                }
                if (currentBox == Boxes.Right)
                {
                    SelectedRight += (maxRowsRight - 1);
                    if (SelectedRight > maxItemsRight - 1) SelectedRight = maxItemsRight - 1;
                }

            }

            if (KeyboardPressed(Keys.PageUp))
            {
                if (currentBox == Boxes.Left)
                {
                    Selected -= (maxRows - 1);
                    if (Selected < 0) Selected = 0;
                }
                if (currentBox == Boxes.Right)
                {
                    SelectedRight -= (maxRowsRight - 1);
                    if (SelectedRight < 0) SelectedRight = 0;
                }

            }

            if (Selected != oldSelected) OnSelectedChanged();
            if (SelectedRight != oldSelectedRight) OnSelectedRightChanged();
            return 0;
        }

        /// <summary>
        /// Panel drawing
        /// </summary>
        /// <param name="sb"></param>
        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);

            sb.Draw(currentBox == Boxes.Left ? boxBackground : boxBackgroundInactive, leftBox, Color.White);
            sb.Draw(currentBox == Boxes.Right ? boxBackground : boxBackgroundInactive, textBox, Color.White);

            int y = boxesY + 5;
            for (int j = 0; j < maxItems; j++)
            {
                if (maxItems <= maxRows || (j >= Selected && j < Selected + maxRows)) {
                    string txt = ScrollerItemText(j);
                    if (j == Selected)
                        sb.DrawString(fonts[FontSizes.Title], txt, new Vector2(leftBox.Left + 40, y), Color.Blue);
                    else
                        sb.DrawString(fonts[FontSizes.Title], txt, new Vector2(leftBox.Left + 5, y), Color.Black);
                    y += itemHeight;
                }
            }

            if (maxItems > 0)
                ScrollerItemDetail(sb);
        }

        /// <summary>
        /// Return the text for the scroller item
        /// </summary>
        /// <param name="pos">line to draw</param>
        /// <returns></returns>
        protected virtual string ScrollerItemText(int pos)
        {
            return "";
        }

        /// <summary>
        /// Draw the selected item detail
        /// </summary>
        /// <param name="sb"></param>
        protected virtual void ScrollerItemDetail(SpriteBatch sb)
        {
            // virtual
        }
    }
}

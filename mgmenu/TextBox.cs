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
using System.Collections.Generic;

namespace Casasoft.MgMenu
{
    public enum FontSizes { Normal, Subtitle, Title, Header }

    /// <summary>
    /// Defines a row of text for scrollers
    /// </summary>
    public class TextRow
    {
        public string Text { get; set; }
        public FontSizes FontSize { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="text"></param>
        /// <param name="size"></param>
        public TextRow(string text, FontSizes size)
        {
            Text = text;
            FontSize = size;
        }

        /// <summary>
        /// Constructor with default text width
        /// </summary>
        /// <param name="text"></param>
        public TextRow(string text) : this(text, FontSizes.Normal) { }

        /// <summary>
        /// Draws the row
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="fonts"></param>
        /// <param name="pos"></param>
        /// <param name="color"></param>
        public void Draw(SpriteBatch sb, Dictionary<FontSizes, BitmapFont> fonts, Vector2 pos, Color color)
        {
            sb.DrawString(fonts[this.FontSize], this.Text, pos, Color.Black);
        }

        /// <summary>
        /// Returns the height of the row
        /// </summary>
        /// <param name="fonts"></param>
        /// <returns></returns>
        public int Height(Dictionary<FontSizes, BitmapFont> fonts)
        {
            return (int)fonts[this.FontSize].MeasureString(
                string.IsNullOrWhiteSpace(this.Text) ? "@" : this.Text).Height;
        }
    }

    /// <summary>
    /// List of text rows
    /// </summary>
    public class TextBox : List<TextRow>
    {
        private SpriteBatch sb;
        private Rectangle box;
        private Dictionary<FontSizes, BitmapFont> fonts;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="fonts"></param>
        /// <param name="box"></param>
        public TextBox(SpriteBatch sb, Dictionary<FontSizes, BitmapFont> fonts, Rectangle box) : base()
        {
            this.sb = sb;
            this.fonts = fonts;
            this.box = box;
        }

        /// <summary>
        /// Draws the entire box
        /// </summary>
        public void Draw() 
        {
            Draw(0);
        }

        /// <summary>
        /// Draws the text starting at line 
        /// </summary>
        /// <param name="line">line to start from</param>
        public void Draw(int line)
        {
            if (line < 0 || line > this.Count)
                return;

            // If the box can contain the whole text I draw all
            if (Height() < box.Height - 10)
                line = 0;

            int y = box.Top + 5;
            for(int r = line; r < this.Count; r++)
            {
                int h = this[r].Height(fonts);
                if (y + h > box.Height - 5)
                    break;

                this[r].Draw(sb, fonts, new Vector2(box.Left + 10, y), Color.Black);
                y += h;
            }
        }

        /// <summary>
        /// Total height of the text
        /// </summary>
        /// <returns></returns>
        public int Height()
        {
            int ret = 0;
            foreach (var row in this)
                ret += row.Height(fonts);
            return ret;
        }

        #region add
        /// <summary>
        /// Adds lines of text
        /// </summary>
        /// <param name="text"></param>
        /// <param name="size"></param>
        public void AddTextRows(string text, FontSizes size)
        {
            string[] lines = text.Split('\n');
            foreach (var l in lines)
                this.Add(new TextRow(l, size));
        }

        /// <summary>
        /// Wraps and adds lines of text 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="size"></param>
        public void AddTextRowsWrapped(string text, FontSizes size)
        {
            AddTextRows(WrapText(text, size), size);
        }

        /// <summary>
        /// Wraps and adds lines of text  with normal font
        /// </summary>
        /// <param name="text"></param>
        public void AddTextRowsWrapped(string text)
        {
            AddTextRowsWrapped(text, FontSizes.Normal);
        }
        #endregion

        #region string wrap
        /// <summary>
        /// Wrap the text inside the box
        /// </summary>
        /// <param name="text"></param>
        /// <param name="TextBox"></param>
        /// <param name="font"></param>
        /// <returns></returns>
        public string WrapText(string text, Rectangle TextBox, BitmapFont font)
        {
            string line = string.Empty;
            string returnString = string.Empty;
            string[] wordArray = text.Split(' ');

            foreach (string word in wordArray)
            {
                if (font.MeasureString(line + word).Width > TextBox.Width - 20)
                {
                    returnString = returnString + line + '\n';
                    line = string.Empty;
                }
                line = line + word + ' ';
            }
            return returnString + line;
        }

        /// <summary>
        /// Returns wrapped text by size
        /// </summary>
        /// <param name="text"></param>
        /// <param name="TextBox"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public string WrapText(string text, Rectangle TextBox, FontSizes size)
        {
            return WrapText(text, TextBox, fonts[size]);
        }

        /// <summary>
        /// Returns wrapped text with Normal font
        /// </summary>
        /// <param name="text"></param>
        /// <param name="TextBox"></param>
        /// <returns></returns>
        public string WrapText(string text, Rectangle TextBox)
        {
            return WrapText(text, TextBox, FontSizes.Normal);
        }

        /// <summary>
        /// Returns wrapped text in object's block
        /// </summary>
        /// <param name="text"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public string WrapText(string text, FontSizes size)
        {
            return WrapText(text, this.box, size);
        }

        /// <summary>
        /// Returns wrapped text in object's block with normal font
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public string WrapText(string text)
        {
            return WrapText(text, FontSizes.Normal);
        }
        #endregion

    }

}

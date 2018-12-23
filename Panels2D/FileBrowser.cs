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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Casasoft.Panels2D
{
    class FileBrowser : PanelVScroller
    {
        public string CurrentPath;
        public string CurrentFile;

        protected DriveInfo[] drives;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="game"></param>
        public FileBrowser(Game game) : base(game)
        {
            CurrentPath = string.Empty;
            CurrentFile = string.Empty;

            drives = DriveInfo.GetDrives();
            maxItems = drives.Length;

            Caption = "File browser";
        }

        /// <summary>
        /// Resets panel data
        /// </summary>
        public override void Clear()
        {
            base.Clear();
            CurrentPath = string.Empty;
            CurrentFile = string.Empty;
        }

        /// <summary>
        /// Draws the file list
        /// </summary>
        /// <param name="sb"></param>
        protected override void ScrollerItemDetail(SpriteBatch sb)
        {
            TextBox detail = new TextBox(sb, fonts, textBox);
            CurrentPath = drives[Selected].Name;

            foreach (var f in Directory.GetFiles(CurrentPath))
            {
                detail.AddTextRows(f, FontSizes.Subtitle);
            }

            detail.Draw();
        }

        /// <summary>
        /// Directory name
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        protected override string ScrollerItemText(int pos)
        {
            return drives[pos].Name;
        }
    }
}

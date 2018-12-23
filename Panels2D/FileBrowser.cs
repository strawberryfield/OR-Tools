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
using System;
using System.IO;

namespace Casasoft.Panels2D
{
    public class FileBrowser : PanelVScrollerDouble
    {
        public string CurrentPath;
        public string CurrentFile;

        protected DriveInfo[] drives;
        protected string[] files;
        protected string[] dirs;
        protected bool isTopLevel;

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
            isTopLevel = true;
            OnSelectedChanged();

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
        /// Directory selection
        /// </summary>
        /// <returns></returns>
        protected override int EnterPressed()
        {
            try
            {
                string[] d = Directory.GetDirectories(CurrentPath);
                if(d.Length > 0)
                {
                    isTopLevel = false;
                    dirs = d;
                    maxItems = dirs.Length;
                    Selected = 0;
                    OnSelectedChanged();
                }

            }
            catch (Exception)
            {
            }
            return 0;
        }

        /// <summary>
        /// File selection
        /// </summary>
        /// <returns></returns>
        protected override int EnterPressedOnRight()
        {
            if (maxItemsRight > 0)
            {
                CurrentFile = files[Selected];
                return 1;
            }
            else
                return 0;
        }

        protected override void OnSelectedChanged()
        {
            if (isTopLevel)
                CurrentPath = drives[Selected].Name;
            else
                CurrentPath = dirs[Selected];

            try
            {
                files = Directory.GetFiles(CurrentPath);
                maxItemsRight = files.Length;
            }
            catch(Exception)
            {
                maxItemsRight = 0;
            }
            for (int j = 0; j < maxItemsRight; j++)
                files[j] = Path.GetFileName(files[j]);

            SelectedRight = 0;
        }

        /// <summary>
        /// Directory name
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        protected override string ScrollerItemText(int pos)
        {
            return isTopLevel ? drives[pos].Name : dirs[pos];
        }

        /// <summary>
        /// File name
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        protected override string RightScrollerItemText(int pos)
        {
            return files[pos];
        }
    }
}

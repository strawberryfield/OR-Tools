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
using System;
using System.Collections.Generic;
using System.IO;

namespace Casasoft.Panels2D
{
    public class FileBrowser : PanelVScrollerDouble
    {
        public string CurrentPath;
        public string CurrentFile;
        protected string ActiveDir;
        protected string StartDir;
        protected string[] Filters;

        protected DriveInfo[] drives;
        protected string[] files;
        protected List<string> dirs;
        protected bool isTopLevel;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="game"></param>
        public FileBrowser(Game game) : this(game, string.Empty) { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="game"></param>
        /// <param name="startDir"></param>
        public FileBrowser(Game game, string startDir) : this(game, startDir, "*") { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="game"></param>
        /// <param name="startDir"></param>
        /// <param name="filter">Files listing filter</param>
        public FileBrowser(Game game, string startDir, string filter) : base(game)
        {
            CurrentPath = string.Empty;
            CurrentFile = string.Empty;
            ActiveDir = string.Empty;
            StartDir = startDir;
            if(string.IsNullOrWhiteSpace(filter))
            {
                Filters = new string[] { "*" };
            }
            else
            {
                Filters = filter.Split('|');
            }

            dirs = new List<string>();
            drives = DriveInfo.GetDrives();
            if (string.IsNullOrWhiteSpace(StartDir))
            {
                maxItems = drives.Length;
                isTopLevel = true;
                OnSelectedChanged();
            }
            else
            {
                CurrentPath = StartDir;
                isTopLevel = false;
                EnterPressed();
            }

            detailSizeY = screenY - boxesY * 2 - 20;
            boxesY += 20;
            leftBox = new Rectangle(20, boxesY, detailSizeX, detailSizeY);
            textBox = new Rectangle(detailSizeX + 40, boxesY, screenX - detailSizeX - 60, detailSizeY);
            maxRows = (detailSizeY - 10) / itemHeight;
            maxRowsRight = maxRows;

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
            ActiveDir = CurrentPath;
            isTopLevel = false;
            if (ActiveDir == string.Empty)
                isTopLevel = true;

            if(!isTopLevel)
            {
                string[] dirarray = { };
                try
                {
                    dirarray = Directory.GetDirectories(CurrentPath);
                }
                catch (Exception)
                {
                } 
                
                dirs.Clear();
                dirs.Add(CurrentPath);
                string parent = Path.GetDirectoryName(CurrentPath);
                if (string.IsNullOrWhiteSpace(parent)) parent = string.Empty;
                dirs.Add(parent);
                dirs.AddRange(dirarray);
                maxItems = dirs.Count;
            }
            else
            {
                maxItems = drives.Length;
            }

            Selected = 0;
            OnSelectedChanged();
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
                CurrentFile = files[SelectedRight];
                return 1;
            }
            else
                return 0;
        }

        /// <summary>
        /// Called when the dir changes
        /// </summary>
        protected override void OnSelectedChanged()
        {
            if (isTopLevel)
                CurrentPath = drives[Selected].Name;
            else
                CurrentPath = dirs[Selected];

            FilesList();
        }

        /// <summary>
        /// Updates the right panel list
        /// </summary>
        protected void FilesList()
        { 
            try
            {
                List<string> fl = new List<string>();
                foreach (var filter in Filters)
                    fl.AddRange(Directory.EnumerateFiles(CurrentPath, filter));
                fl.Sort();
                files = fl.ToArray();
                maxItemsRight = files.Length;
            }
            catch(Exception)
            {
                maxItemsRight = 0;
            }

            SelectedRight = 0;
        }

        /// <summary>
        /// Directory name
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        protected override string ScrollerItemText(int pos)
        {
            if(isTopLevel)
            {
                return drives[pos].Name;
            }
            else
            {
                switch (pos)
                {
                    case 0:
                        return ".";
                    case 1:
                        return "..";
                    default:
                        return Path.GetFileName(dirs[pos]);
                }
            }           
        }

        /// <summary>
        /// File name
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        protected override string RightScrollerItemText(int pos)
        {
            return Path.GetFileName(files[pos]);
        }

        /// <summary>
        /// Draws the current path
        /// </summary>
        /// <param name="sb"></param>
        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);

            if (!string.IsNullOrWhiteSpace(ActiveDir))
            {
                sb.DrawString(fonts[FontSizes.Subtitle], ActiveDir, new Vector2(leftBox.Left, boxesY - 35), Color.WhiteSmoke);
            }
        }
    }
}

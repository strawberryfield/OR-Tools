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

using Casasoft.Panels2D;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ORTS.Menu;
using System.Collections.Generic;
using System.Linq;

namespace Casasoft.MgMenu
{
    public class SelPath : PanelVScrollerDouble
    {
        private List<Path> pathList;
        private List<string> startAt;
        private List<Path> headTo;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="game"></param>
        public SelPath(Game game) : base(game)
        {
            pathList = new List<Path>();
            startAt = new List<string>();
            headTo = new List<Path>();

            maxItems = startAt.Count;
            maxItemsRight = headTo.Count;

            Caption = "Choose a trip";
        }

        /// <summary>
        /// List of starting locations
        /// </summary>
        private void FillStartAt()
        {
           startAt.Clear();
            foreach (var place in pathList.Select(p => p.Start).Distinct().OrderBy(s => s.ToString()))
                startAt.Add(place);
            maxItems = startAt.Count;
        }

        /// <summary>
        /// Fills destinations list
        /// </summary>
        private void FillHeadTo()
        {
            headTo.Clear();
            foreach (var path in pathList.Where(p => p.Start == startAt[Selected]))
                headTo.Add(path);
            maxItemsRight = headTo.Count;
        }

        /// <summary>
        /// Inits panel data
        /// </summary>
        /// <param name="paths"></param>
        public void SetList(List<Path> paths)
        {
            pathList = paths;
            FillStartAt();
            Selected = 0;
            FillHeadTo();
            SelectedRight = 0;
        }

        /// <summary>
        /// Called after start selection
        /// </summary>
        /// <returns></returns>
        protected override void OnSelectedChanged()
        {
            FillHeadTo();
            SelectedRight = 0;
        }

        /// <summary>
        /// If there only a path with that start I select it immediately
        /// </summary>
        /// <returns></returns>
        protected override int EnterPressed()
        {
            return (maxItemsRight == 1 ? 1 : 0);
        }

        /// <summary>
        /// Path selection
        /// </summary>
        /// <returns></returns>
        protected override int EnterPressedOnRight()
        {
            return 1;
        }

        /// <summary>
        /// StartAt description
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        protected override string ScrollerItemText(int pos)
        {
            return startAt[pos];
        }

        /// <summary>
        /// HeadTo description
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        protected override string RightScrollerItemText(int pos)
        {
            return headTo[pos].End;
        }
    }
}

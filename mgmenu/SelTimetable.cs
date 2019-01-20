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
using Orts.Formats.OR;

namespace Casasoft.MgMenu
{
    class SelTimetable : PanelVScrollerDouble
    {
        private TimetableInfo timeTable;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="game"></param>
        public SelTimetable(Game game) : base(game)
        {
            maxItems = 0;
            maxItemsRight = 0;

            Caption = "Choose a Timetable and Train";
        }

        /// <summary>
        /// Init panel data
        /// </summary>
        /// <param name="timetableInfo"></param>
        public void SetList(TimetableInfo timetableInfo)
        {
            timeTable = timetableInfo;
            Selected = 0;
            maxItems = timeTable.ORTTList.Count;
            OnSelectedChanged();
        }

        /// <summary>
        /// On timetable selection changed
        /// </summary>
        protected override void OnSelectedChanged()
        {
            SelectedRight = 0;
            maxItemsRight = timeTable.ORTTList[Selected].Trains.Count;
        }

        /// <summary>
        /// Timetable item description
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        protected override string ScrollerItemText(int pos)
        {
            return timeTable != null ? timeTable.ORTTList[pos].Description : "";
        }

        /// <summary>
        /// Train description
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        protected override string RightScrollerItemText(int pos)
        {
            return timeTable != null ? timeTable.ORTTList[Selected].Trains[pos].Train : "";
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
        /// Selected timetable
        /// </summary>
        public TimetableFileLite Timetable
        {
            get
            {
                return timeTable.ORTTList[Selected];
            }
        }

        /// <summary>
        /// Selected train
        /// </summary>
        public TimetableFileLite.TrainInformation Train
        {
            get
            {
                return timeTable.ORTTList[Selected].Trains[SelectedRight];
            }
        }
    }
}

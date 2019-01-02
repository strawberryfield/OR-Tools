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
using System.IO;

namespace Casasoft.MgMenu
{
    public class SelSeason : PanelButtons
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="game"></param>
        public SelSeason(Game game) : base(game)
        {
            Caption = "Choose the season";
            string imgPath = Path.GetFullPath(@".\Content\Buttons");

            using (var fileStream = new FileStream(Path.Combine(imgPath, "summer.png"), FileMode.Open))
                buttonList.Add(new ButtonData(Texture2D.FromStream(game.GraphicsDevice, fileStream), "Summer"));
            using (var fileStream = new FileStream(Path.Combine(imgPath, "fall.png"), FileMode.Open))
                buttonList.Add(new ButtonData(Texture2D.FromStream(game.GraphicsDevice, fileStream), "Fall"));
            using (var fileStream = new FileStream(Path.Combine(imgPath, "winter.png"), FileMode.Open))
                buttonList.Add(new ButtonData(Texture2D.FromStream(game.GraphicsDevice, fileStream), "Winter"));
            using (var fileStream = new FileStream(Path.Combine(imgPath, "spring.png"), FileMode.Open))
                buttonList.Add(new ButtonData(Texture2D.FromStream(game.GraphicsDevice, fileStream), "Spring"));

        }
    }
}

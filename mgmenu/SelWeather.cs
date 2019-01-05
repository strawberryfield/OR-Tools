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
using Orts.Formats.Msts;
using System.IO;

namespace Casasoft.MgMenu
{
    public class SelWeather : PanelButtons
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="game"></param>
        public SelWeather(Game game) : base(game)
        {
            Caption = "Choose the weather";
            string imgPath = Path.GetFullPath(@".\Content\Buttons");

            using (var fileStream = new FileStream(Path.Combine(imgPath, "sun.jpg"), FileMode.Open))
                buttonList.Add(new ButtonData(Texture2D.FromStream(game.GraphicsDevice, fileStream), "Clear"));
            using (var fileStream = new FileStream(Path.Combine(imgPath, "rain.jpg"), FileMode.Open))
                buttonList.Add(new ButtonData(Texture2D.FromStream(game.GraphicsDevice, fileStream), "Rain"));
            using (var fileStream = new FileStream(Path.Combine(imgPath, "snow.jpg"), FileMode.Open))
                buttonList.Add(new ButtonData(Texture2D.FromStream(game.GraphicsDevice, fileStream), "Snow"));
        }

        /// <summary>
        /// Weather <-> Selected conversion
        /// </summary>
        public WeatherType Weather
        {
            get
            {
                switch (Selected)
                {
                    case 0:
                        return WeatherType.Clear;
                    case 1:
                        return WeatherType.Rain;
                    case 2:
                        return WeatherType.Snow;
                    default:
                        return WeatherType.Clear;
                }
            }

            set
            {
                switch (value)
                {
                    case WeatherType.Clear:
                        Selected = 0;
                        break;
                    case WeatherType.Snow:
                        Selected = 1;
                        break;
                    case WeatherType.Rain:
                        Selected = 2;
                        break;
                    default:
                        Selected = 0;
                        break;
                }
            }
        }
    }
}

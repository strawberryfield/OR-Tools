// COPYRIGHT 2018,2019 Roberto Ceccarelli - Casasoft.
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

namespace Casasoft.Panels2D
{
    public class PanelScroller : PanelBase
    {

        protected Texture2D boxBackground;
        protected Texture2D boxBackgroundInactive;

        protected int maxItems;

        protected Rectangle textBox;
        protected Rectangle leftBox;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="game"></param>
        public PanelScroller( Game game) : base(game)
        {
            boxBackground = new Texture2D(game.GraphicsDevice, 1, 1);
            Color[] colorData = new Color[1];
            colorData[0] = Color.WhiteSmoke;
            boxBackground.SetData<Color>(colorData);

            boxBackgroundInactive = new Texture2D(game.GraphicsDevice, 1, 1);
            colorData = new Color[1];
            colorData[0] = Color.LightGray;
            boxBackgroundInactive.SetData<Color>(colorData);

            maxItems = 0;
        }

        /// <summary>
        /// Resets panel data
        /// </summary>
        public virtual void Clear()
        {
            Selected = 0;
            maxItems = 0;
        }

    }
}


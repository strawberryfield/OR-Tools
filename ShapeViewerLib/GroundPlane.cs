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

namespace Casasoft.ShapeViewerLib
{
    class GroundPlane
    {
        protected VertexPositionNormalTexture[] floorVerts;
        protected Matrix worldMatrix;
        public Texture2D FloorTexture { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="size"></param>
        /// <param name="rep"></param>
        public GroundPlane(int size, int rep) : this(size, rep, null) { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="size"></param>
        /// <param name="rep"></param>
        /// <param name="texture"></param>
        public GroundPlane(int size, int rep, Texture2D texture)
        {
            FloorTexture = texture;
            DefineFloor(size, rep);

            Vector3 camTarget = new Vector3(0f, 0f, 0f);
            worldMatrix = Matrix.CreateWorld(camTarget, Vector3.
              Forward, Vector3.Up);
        }

        /// <summary>
        /// Vertices definition
        /// </summary>
        /// <param name="size"></param>
        /// <param name="rep"></param>
        protected void DefineFloor(int size, int rep)
        {
            floorVerts = new VertexPositionNormalTexture[6];
            floorVerts[2].Position = new Vector3(-size, 0, -size);
            floorVerts[1].Position = new Vector3(-size, 0, size);
            floorVerts[0].Position = new Vector3(size, 0, -size);

            floorVerts[4].Position = floorVerts[1].Position;
            floorVerts[3].Position = new Vector3(size, 0, size);
            floorVerts[5].Position = floorVerts[0].Position;

            floorVerts[2].TextureCoordinate = new Vector2(0, 0);
            floorVerts[1].TextureCoordinate = new Vector2(0, rep);
            floorVerts[0].TextureCoordinate = new Vector2(rep, 0);

            floorVerts[4].TextureCoordinate = floorVerts[1].TextureCoordinate;
            floorVerts[3].TextureCoordinate = new Vector2(rep, rep);
            floorVerts[5].TextureCoordinate = floorVerts[0].TextureCoordinate;

            // Reset normals
            for (int j = 0; j < floorVerts.Length; j++) floorVerts[j].Normal = Vector3.Up;

            //Vector3 v1 = floorVerts[1].Position - floorVerts[0].Position;
            //Vector3 v2 = floorVerts[0].Position - floorVerts[2].Position;
            //Vector3 n = Vector3.Cross(v1, v2);
            //n.Normalize();
        }

        /// <summary>
        /// Draws the floor
        /// </summary>
        /// <param name="frame"></param>
        public void Draw(RenderFrame frame)
        {
            // Draws the Floor
            BasicEffect basicEffect = new BasicEffect(frame.Game.GraphicsDevice);
            basicEffect.Alpha = 1f;
            basicEffect.TextureEnabled = true;
            basicEffect.Texture = FloorTexture;
            basicEffect.Projection = frame.XNACameraProjection;
            basicEffect.View = frame.XNACameraView;
            basicEffect.World = worldMatrix;

            foreach (var pass in basicEffect.CurrentTechnique.Passes)
                pass.Apply();

            frame.Game.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, floorVerts, 0, 2);
        }
    }
}

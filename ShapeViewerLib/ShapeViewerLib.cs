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
using Orts.Formats.Msts;
using ORTS.Settings;
using System;
using System.IO;
using System.Linq;

namespace Casasoft.ShapeViewerLib
{
    public class ShapeViewerLib
    {
        //Camera
        public float cameraHeight { get; set; }
        public float cameraDistance { get; set; }
        public float cameraAngle { get; set; }

protected Vector3 camTarget;
        protected Vector3 camPosition;
        protected Matrix projectionMatrix;
        protected Matrix worldMatrix;

        //BasicEffect for rendering
        BasicEffect basicEffect;
        Viewer viewer;
        Game Game;
        public Simulator sim;

        // Floor
        VertexPositionTexture[] floorVerts;
        public Texture2D floorTexture;

        public SharedShape Shape { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="game"></param>
        public ShapeViewerLib(Game game)
        {
            Game = game;
            Game.Content.RootDirectory = "Content";
            DefineFloor(50, 10);
        }

        public void CameraSetup()
        {
            camTarget = new Vector3(0f, 0f, 0f);
            cameraDistance = 30f;
            cameraHeight = 2f;
            cameraAngle = 0f;
            camPosition = GetCameraPosition();
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                               MathHelper.ToRadians(45f),
                               Game.GraphicsDevice.DisplayMode.AspectRatio, 1f, 1000f);
            worldMatrix = Matrix.CreateWorld(camTarget, Vector3.
                          Forward, Vector3.Up);
        }

        public Vector3 GetCameraPosition()
        {
            return new Vector3((float)(cameraDistance * Math.Sin(cameraAngle)),
                            cameraHeight, -cameraDistance * (float)Math.Cos(cameraAngle));
        }

        public void BasicEffectSetup()
        {
            //BasicEffect
            basicEffect = new BasicEffect(Game.GraphicsDevice);
            basicEffect.Alpha = 1f;

            //Lighting requires normal information which VertexPositionColor does not have
            //If you want to use lighting and VPC you need to create a custom def
            basicEffect.LightingEnabled = false;

            basicEffect.TextureEnabled = true;
            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            Game.GraphicsDevice.RasterizerState = rasterizerState;
            Game.GraphicsDevice.BlendState = BlendState.NonPremultiplied;
        }

        private void DefineFloor(int size, int rep)
        {
            floorVerts = new VertexPositionTexture[6];
            floorVerts[0].Position = new Vector3(-size, 0, -size);
            floorVerts[1].Position = new Vector3(-size, 0, size);
            floorVerts[2].Position = new Vector3(size, 0, -size);

            floorVerts[3].Position = floorVerts[1].Position;
            floorVerts[4].Position = new Vector3(size, 0, size);
            floorVerts[5].Position = floorVerts[2].Position;

            floorVerts[0].TextureCoordinate = new Vector2(0, 0);
            floorVerts[1].TextureCoordinate = new Vector2(0, rep);
            floorVerts[2].TextureCoordinate = new Vector2(rep, 0);

            floorVerts[3].TextureCoordinate = floorVerts[1].TextureCoordinate;
            floorVerts[4].TextureCoordinate = new Vector2(rep, rep);
            floorVerts[5].TextureCoordinate = floorVerts[2].TextureCoordinate;
        }

        public void LoadShape(string filename)
        {
            sim = new Simulator();
            sim.RoutePath = Path.Combine(Path.GetDirectoryName(filename), "..");
            sim.Season = SeasonType.Summer;
            sim.WeatherType = WeatherType.Clear;
            var options = Environment.GetCommandLineArgs().Where(a => (a.StartsWith("-") || a.StartsWith("/"))).Select(a => a.Substring(1));
            sim.Settings = new UserSettings(options);

            viewer = new Viewer(Game.GraphicsDevice, sim);

            Shape = new SharedShape(viewer, filename);
        }

        public void Update()
        {
            bool recalcXZ = false;

            // TODO: Add your update logic here
            if (Keyboard.GetState().IsKeyDown(Keys.PageDown))
            {
                cameraHeight -= 0.1f;
                camPosition.Y = cameraHeight;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.PageUp))
            {
                cameraHeight += 0.1f;
                camPosition.Y = cameraHeight;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                cameraDistance += 0.1f;
                recalcXZ = true;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                cameraDistance -= 0.1f;
                recalcXZ = true;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                cameraAngle += 0.01f;
                recalcXZ = true;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                cameraAngle -= 0.01f;
                recalcXZ = true;
            }

            if (recalcXZ)
            {
                camPosition = GetCameraPosition();
            }

        }

        public void Draw(int lod, int distanceLevel)
        {
            Matrix viewMatrix = Matrix.CreateLookAt(camPosition, camTarget, Vector3.UnitY);

            basicEffect.Projection = projectionMatrix;
            basicEffect.View = viewMatrix;
            basicEffect.World = worldMatrix;

            // Draws the Floor
            basicEffect.Texture = floorTexture;
            foreach (var pass in basicEffect.CurrentTechnique.Passes)
                pass.Apply();

            Game.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, floorVerts, 0, 2);

            // Draws the shape
            SharedShape.SubObject[] subs = Shape.LodControls[lod].DistanceLevels[distanceLevel].SubObjects;
            foreach (var so in subs)
            {
                foreach (var pr in so.ShapePrimitives)
                {
                    basicEffect.Texture = pr.Material.GetShadowTexture();
                    foreach (var pass in basicEffect.CurrentTechnique.Passes)
                        pass.Apply();

                    pr.Draw(Game.GraphicsDevice);
                }
            }

        }

        public void Draw()
        {
            Draw(0, 0);
        }
    }
}

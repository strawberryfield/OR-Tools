using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Orts.Formats.Msts;
using Orts.Viewer3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ORTS.Settings;

namespace Casasoft.ShapeViewerLib
{
    public class ShapeViewerLib
    {
        //Camera
        public Vector3 camTarget;
        public Vector3 camPosition;
        public Matrix projectionMatrix;
        public Matrix worldMatrix;

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
            camPosition = new Vector3(0f, 12f, -40f);
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                               MathHelper.ToRadians(45f),
                               Game.GraphicsDevice.DisplayMode.AspectRatio,
                1f, 1000f);
            worldMatrix = Matrix.CreateWorld(camTarget, Vector3.
                          Forward, Vector3.Up);
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
                    Texture2D texture = AceFile.Texture2DFromFile(Game.GraphicsDevice, ((SceneryMaterial)pr.Material).TexturePath);
                    basicEffect.Texture = texture;

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

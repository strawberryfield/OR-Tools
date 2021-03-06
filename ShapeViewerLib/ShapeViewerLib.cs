﻿// COPYRIGHT 2018 Roberto Ceccarelli - Casasoft.
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
using Orts.Simulation.RollingStocks;
using System;
using System.IO;
using System.Linq;
using Orts.Simulation;

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

        protected Game Game;
        protected RenderFrame frame;
        protected Simulator sim;
        protected Viewer viewer;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="game"></param>
        public ShapeViewerLib(Game game)
        {
            Game = game;
            Game.Content.RootDirectory = "Content";
            frame = new RenderFrame(Game);
            var options = Environment.GetCommandLineArgs().Where(a => (a.StartsWith("-") || a.StartsWith("/"))).Select(a => a.Substring(1));

            sim = new Simulator(new UserSettings(options), Path.GetFullPath(@".\Content\DummyRoute\Activities\DummyActivity.act"), false);
            sim.Season = SeasonType.Summer;
            sim.WeatherType = WeatherType.Clear;
            viewer = new Viewer(Game.GraphicsDevice, sim);
        }

        /// <summary>
        /// Camera init
        /// </summary>
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
        }

        /// <summary>
        /// Returns a Vector3 camera position from polar coords
        /// </summary>
        /// <returns></returns>
        public Vector3 GetCameraPosition()
        {
            return new Vector3((float)(cameraDistance * Math.Sin(cameraAngle)),
                            cameraHeight, -cameraDistance * (float)Math.Cos(cameraAngle));
        }

        /// <summary>
        /// Sets the texture of the floor
        /// </summary>
        /// <param name="texture"></param>
        public void SetFloorTexture(Texture2D texture)
        {
            frame.SetFloorTexture(texture);
        }

        /// <summary>
        /// Clears the renderframe
        /// </summary>
        public void Clear()
        {
            frame.Clear();
        }

        /// <summary>
        /// Loads en entire object
        /// </summary>
        /// <param name="filename"></param>
        public void LoadItem(string filename)
        {
            string ext = Path.GetExtension(filename).ToLower();
            if (ext == ".s")
            {
                LoadShape(filename);
            }
            else if (ext == ".wag" || ext == ".eng")
            {
                MSTSWagon wag = new MSTSWagon(sim, filename);
                wag.Load();
                string wagDir = Path.GetDirectoryName(filename);
                if(!string.IsNullOrWhiteSpace(wag.MainShapeFileName))
                    LoadShape(Path.Combine(wagDir, wag.MainShapeFileName));
                if (!string.IsNullOrWhiteSpace(wag.FreightShapeFileName))
                    LoadShape(Path.Combine(wagDir, wag.FreightShapeFileName));
            }
        }

        /// <summary>
        /// Loads a shape
        /// </summary>
        /// <param name="filename"></param>
        public void LoadShape(string filename)
        {
            sim.RoutePath = Path.Combine(Path.GetDirectoryName(filename), "..");

            string dir = Path.GetDirectoryName(filename);
            string dirTextures = Path.Combine(dir, "..\\TEXTURES");
            string fileref = filename + '\0';
            if (Directory.Exists(dirTextures))
            {
                fileref += dirTextures;
            }
            else
            {
                fileref += dir;
            }

            SharedShape Shape = new SharedShape(viewer, fileref);
            Shape.PrepareFrame(frame, ShapeFlags.AutoZBias);
        }

        /// <summary>
        /// Changes the season
        /// </summary>
        /// <param name="season"></param>
        public void SetSeason(SeasonType season)
        {
            sim.Season = season;
        }

        /// <summary>
        /// Changes the wheather
        /// </summary>
        /// <param name="weather"></param>
        public void SetWeather(WeatherType weather)
        {
            sim.WeatherType = weather;
        }

        /// <summary>
        /// Handles user input
        /// </summary>
        /// <returns></returns>
        public int Update()
        {
            bool recalcXZ = false;
            GamePadState gps = GamePad.GetState(PlayerIndex.One);
            KeyboardState kbs = Keyboard.GetState();

            if (gps.IsButtonDown(Buttons.Back) || kbs.IsKeyDown(Keys.Escape))
                return -1;

            // TODO: Add your update logic here
            if (kbs.IsKeyDown(Keys.PageDown) || gps.IsButtonDown(Buttons.RightThumbstickDown) )
            {
                cameraHeight -= 0.1f;
                camPosition.Y = cameraHeight;
            }

            if (kbs.IsKeyDown(Keys.PageUp) || gps.IsButtonDown(Buttons.RightThumbstickUp))
            {
                cameraHeight += 0.1f;
                camPosition.Y = cameraHeight;
            }

            if (kbs.IsKeyDown(Keys.Down) || gps.IsButtonDown(Buttons.LeftThumbstickDown))
            {
                cameraDistance += 0.1f;
                recalcXZ = true;
            }

            if (kbs.IsKeyDown(Keys.Up) || gps.IsButtonDown(Buttons.LeftThumbstickUp))
            {
                cameraDistance -= 0.1f;
                recalcXZ = true;
            }

            if (kbs.IsKeyDown(Keys.Left) || gps.IsButtonDown(Buttons.LeftThumbstickLeft))
            {
                cameraAngle += 0.01f;
                recalcXZ = true;
            }

            if (kbs.IsKeyDown(Keys.Right) || gps.IsButtonDown(Buttons.LeftThumbstickRight))
            {
                cameraAngle -= 0.01f;
                recalcXZ = true;
            }

            //if (kbs.IsKeyDown(Keys.W)) sim.Season = SeasonType.Winter;
            //if (kbs.IsKeyDown(Keys.S)) sim.Season = SeasonType.Summer;

            if (recalcXZ)
            {
                camPosition = GetCameraPosition();
            }
            return 0;

        }

        /// <summary>
        /// Draws the scene
        /// </summary>
        public void Draw()
        {
            Matrix viewMatrix = Matrix.CreateLookAt(camPosition, camTarget, Vector3.UnitY);

            frame.XNACameraProjection = projectionMatrix;
            frame.XNACameraView = viewMatrix;

            frame.Draw(Game.GraphicsDevice);
        }

    }
}

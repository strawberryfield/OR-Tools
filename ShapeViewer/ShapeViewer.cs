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

#define WINDOWED

using Casasoft.Panels2D;
using Casasoft.ShapeViewerLib;
using GNU.Gettext;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Orts.Formats.Msts;
using ORTS.Settings;
using System;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace ShapeViewer
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class ShapeViewer : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private ShapeViewerLib sv;

        private UserSettings Settings;
        private GettextResourceManager catalog = new GettextResourceManager("Menu");

        private enum LoopStatus { SelShape, ShowShape }
        private LoopStatus loopStatus;

        private FileBrowser fileBrowser;

        //string examples = "..\\ORTools\\ShapeViewer\\samples\\";
        string floorTexturePath = "Content\\TSF_BCT_margherite.ace";

        /// <summary>
        /// Constructor
        /// </summary>
        public ShapeViewer()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            this.IsFixedTimeStep = true;
            this.graphics.SynchronizeWithVerticalRetrace = true;

        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            sv = new ShapeViewerLib(this);

            base.Initialize();

            var options = Environment.GetCommandLineArgs().Where(a => (a.StartsWith("-") || a.StartsWith("/"))).Select(a => a.Substring(1));
            Settings = new UserSettings(options);
            LoadLanguage();

            string startDir = ConfigurationManager.AppSettings["StartDir"];
            fileBrowser = new FileBrowser(this, startDir, "*.s|*.eng|*.wag");

            sv.CameraSetup();
            sv.SetFloorTexture(AceFile.Texture2DFromFile(GraphicsDevice, floorTexturePath));
#if WINDOWED
            graphics.PreferredBackBufferWidth = 1366;
            graphics.PreferredBackBufferHeight = 768;
            graphics.IsFullScreen = false;
#else
            graphics.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width;
            graphics.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height;
            graphics.IsFullScreen = true;
#endif
            graphics.ApplyChanges();

            loopStatus = LoopStatus.SelShape;
        }

        #region read OR data
        /// <summary>
        /// Loads OR language settings
        /// </summary>
        private void LoadLanguage()
        {
            if (Settings.Language.Length > 0)
            {
                try
                {
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo(Settings.Language);
                }
                catch { }
            }
        }
        #endregion

        #region MG content mamagement
        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }
        #endregion


        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            switch (loopStatus)
            {
                case LoopStatus.SelShape:
                    switch (fileBrowser.Update())
                    {
                        case -1:
                            Exit();
                            break;
                        case 1:
                            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
                            sv.Clear();
                            sv.LoadItem(fileBrowser.CurrentFile);
                            loopStatus = LoopStatus.ShowShape;
                            break;
                        default:
                            break;
                    }
                    break;
                case LoopStatus.ShowShape:
                    switch (sv.Update())
                    {
                        case -1:
                            fileBrowser.ReInit();
                            loopStatus = LoopStatus.SelShape;
                            break;
                        default:
                            break;
                    }                  
                    break;
                default:
                    break;
            }
            

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            switch (loopStatus)
            {
                case LoopStatus.SelShape:
                    GraphicsDevice.Clear(Color.CornflowerBlue);
                    spriteBatch.Begin();
                    fileBrowser.Draw(spriteBatch);
                    spriteBatch.End();
                    break;
                case LoopStatus.ShowShape:
                    GraphicsDevice.Clear(Color.WhiteSmoke);
                    sv.Draw();
                    break;
                default:
                    break;
            }
                     
            base.Draw(gameTime);
        }
    }


}

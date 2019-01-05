﻿// COPYRIGHT 2018,2019 Roberto Ceccarelli - Casasoft.
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

using GNU.Gettext;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Orts.Formats.Msts;
using ORTS.Menu;
using ORTS.Settings;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Path = ORTS.Menu.Path;

namespace Casasoft.MgMenu
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class MgMenu : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        // OR data and settings
        private UserSettings Settings;
        private GettextResourceManager catalog = new GettextResourceManager("Menu");
        private List<Folder> Folders = new List<Folder>();
        private List<Route> Routes = new List<Route>();
        private List<Activity> Activities = new List<Activity>();
        private List<TimetableInfo> TimetableSets = new List<TimetableInfo>();
        private List<Consist> Consists = new List<Consist>();
        private List<Locomotive> Locos = new List<Locomotive>();
        private List<Path> Paths = new List<Path>();

        public Folder SelectedFolder { get; private set; }
        public Route SelectedRoute { get; private set; }
        public Activity SelectedActivity { get; private set; }
        public Path SelectedPath { get; private set; }
        public Consist SelectedConsist { get; private set; }
        public Locomotive SelectedLocomotive { get; private set; }
        public SeasonType SelectedSeason { get; private set; }
        public WeatherType SelectedWeather { get; private set; }

        private enum LoopStatus { SelRoute, SelActivity, SelLoco, SelConsist, SelPath, SelTime, SelSeason, SelWeather }
        private LoopStatus loopStatus;

        // Panels
        private SelRoute selRoute;
        private SelActivity selActivity;
        private SelLocomotive selLocomotive;
        private SelConsist selConsist;
        private SelPath selPath;
        private SelSeason selSeason;
        private SelWeather selWeather;

        /// <summary>
        /// Constructor
        /// </summary>
        public MgMenu()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            this.IsFixedTimeStep = true;
            this.graphics.SynchronizeWithVerticalRetrace = true;
//           this.TargetElapsedTime = new System.TimeSpan(0, 0, 0, 0, 100); // 100ms = 10fps

            this.IsMouseVisible = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            var options = Environment.GetCommandLineArgs().Where(a => (a.StartsWith("-") || a.StartsWith("/"))).Select(a => a.Substring(1));
            Settings = new UserSettings(options);
            LoadLanguage();
            LoadFolderList();
            LoadRouteList();

            selRoute = new SelRoute(Routes, this);
            selActivity = new SelActivity(this);
            selConsist = new SelConsist(this);
            selLocomotive = new SelLocomotive(this);
            selPath = new SelPath(this);
            selSeason = new SelSeason(this);
            selWeather = new SelWeather(this);

            SelectedFolder = null;
            SelectedRoute = null;
            SelectedActivity = null;
            SelectedPath = null;
            SelectedConsist = null;
            SelectedLocomotive = null;
            SelectedSeason = SeasonType.Summer;
            SelectedWeather = WeatherType.Clear;

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

            loopStatus = LoopStatus.SelRoute;
//            loopStatus = LoopStatus.SelWeather;

            base.Initialize();
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

        /// <summary>
        /// Gets a list of assets folders for OR
        /// </summary>
        private void LoadFolderList()
        {
            Folders = Folder.GetFolders(Settings).OrderBy(f => f.Name).ToList();
        }

        /// <summary>
        /// Gets all the routes in all folders
        /// </summary>
        private void LoadRouteList()
        {
            foreach (var f in Folders)
                Routes.AddRange(Route.GetRoutes(f, this));
            Routes = Routes.OrderBy(n => n.Name).ToList();
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

        #region panels data management
        /// <summary>
        /// Sets the profile folder from route path  
        /// </summary>
        /// <param name="route"></param>
        private void selectFolder(Route route)
        {
            Folder oldFolder = SelectedFolder;

            string p = Directory.GetParent(Directory.GetParent(route.Path).FullName).FullName;
            SelectedFolder = Folders.Where(i => i.Path == p).FirstOrDefault();

            if(oldFolder == null || SelectedFolder.Path != oldFolder.Path)
            {
                Task load = new Task(() => loadFolderData());
                load.Start();
            }
        }

        /// <summary>
        /// Loads activities and paths for current route
        /// </summary>
        private void loadRouteData()
        {
            Activities = Activity.GetActivities(SelectedFolder, SelectedRoute).OrderBy(l => l.Name).ToList();
            selActivity.SetList(Activities);
            Paths = Path.GetPaths(SelectedRoute, true);
            selPath.SetList(Paths);
        }

        /// <summary>
        /// Loads consists and locos from current profile
        /// </summary>
        private void loadFolderData()
        {
            selConsist.Clear();
            selLocomotive.Clear();
            Consists = Consist.GetConsists(SelectedFolder);
            Locos.Add(new Locomotive());
            foreach (var loco in Consists.Where(c => c.Locomotive != null).Select(c => c.Locomotive).Distinct().OrderBy(l => l.ToString()))
                Locos.Add(loco);
            selLocomotive.SetList(Locos);
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
                case LoopStatus.SelRoute:
                    switch (selRoute.Update())
                    {
                        case -1:
                            Exit();
                            break;
                        case 1:
                            loopStatus = LoopStatus.SelActivity;
                            SelectedRoute = Routes[selRoute.Selected];
                            selectFolder(SelectedRoute);
                            selActivity.ReInit();
                            selActivity.Clear();
                            Task loadActivities = new Task(() => loadRouteData());
                            loadActivities.Start();
                            break;
                        default:
                            break;
                    }
                    break;
                case LoopStatus.SelActivity:
                    switch (selActivity.Update())
                    {
                        case -1:
                            loopStatus = LoopStatus.SelRoute;
                            selRoute.ReInit();
                            break;
                        case 1:
                            loopStatus = LoopStatus.SelLoco;
                            SelectedActivity = Activities[selActivity.Selected];
                            selLocomotive.ReInit();
                            break;
                        default:
                            break;
                    }
                    break;
                case LoopStatus.SelLoco:
                    switch(selLocomotive.Update())
                    {
                        case -1:
                            loopStatus = LoopStatus.SelActivity;
                            selActivity.ReInit();
                            break;
                        case 1:
                            loopStatus = LoopStatus.SelConsist;
                            SelectedLocomotive = Locos[selLocomotive.Selected];
                            selConsist.ReInit();
                            selConsist.Clear();
                            List<Consist> lc = new List<Consist>();
                            foreach (var consist in Consists.Where(c => SelectedLocomotive.Equals(c.Locomotive)).OrderBy(c => c.Name))
                                lc.Add(consist);
                            selConsist.SetList(lc);
                            break;
                        default:
                            break;
                    }
                    break;
                case LoopStatus.SelConsist:
                    switch (selConsist.Update())
                    {
                        case -1:
                            loopStatus = LoopStatus.SelLoco;
                            selLocomotive.ReInit();
                            break;
                        case 1:
                            loopStatus = LoopStatus.SelPath;
                            SelectedConsist = Consists[selConsist.Selected];
                            selPath.ReInit();
                            break;
                        default:
                            break;
                    }
                    break;
                case LoopStatus.SelPath:
                    switch (selPath.Update())
                    {
                        case -1:
                            loopStatus = LoopStatus.SelConsist;
                            selConsist.ReInit();
                            break;
                        case 1:
                            loopStatus = LoopStatus.SelSeason;
                            SelectedPath = selPath.Path;
                            selSeason.ReInit();
                            break;
                        default:
                            break;
                    }
                    break;
                case LoopStatus.SelSeason:
                    switch (selSeason.Update())
                    {
                        case -1:
                            loopStatus = LoopStatus.SelPath;
                            selPath.ReInit();
                            break;
                        case 1:
                            loopStatus = LoopStatus.SelWeather;
                            SelectedSeason = selSeason.Season;
                            selWeather.ReInit();
                            break;
                        default:
                            break;
                    }
                    break;
                case LoopStatus.SelWeather:
                    switch (selWeather.Update())
                    {
                        case -1:
                            loopStatus = LoopStatus.SelSeason;
                            selSeason.ReInit();
                            break;
                        default:
                            break;
                    }
                    break;
                case LoopStatus.SelTime:
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
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            switch (loopStatus)
            {
                case LoopStatus.SelRoute:
                    selRoute.Draw(spriteBatch);
                    break;
                case LoopStatus.SelActivity:
                    selActivity.Draw(spriteBatch);
                    break;
                case LoopStatus.SelLoco:
                    selLocomotive.Draw(spriteBatch);
                    break;
                case LoopStatus.SelConsist:
                    selConsist.Draw(spriteBatch);
                    break;
                case LoopStatus.SelPath:
                    selPath.Draw(spriteBatch);
                    break;
                case LoopStatus.SelTime:
                    break;
                case LoopStatus.SelSeason:
                    selSeason.Draw(spriteBatch);
                    break;
                case LoopStatus.SelWeather:
                    selWeather.Draw(spriteBatch);
                    break;
                default:
                    break;
            }
            
            spriteBatch.End();


            base.Draw(gameTime);
        }

        
    }
}

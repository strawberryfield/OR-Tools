﻿using GNU.Gettext;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ORTS.Settings;
using System;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace mgmenu
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class MgMenu : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont font;

        // OR data and settings
        bool Initialized;
        UserSettings Settings;
        GettextResourceManager catalog = new GettextResourceManager("Menu");

        public MgMenu()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
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

            base.Initialize();
        }

        void LoadLanguage()
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
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            font = Content.Load<SpriteFont>("NormalText");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

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

            int y = 100;
            foreach(var dir in Settings.Folders.Folders)
            {
                spriteBatch.DrawString(font, dir.Value, new Vector2(100, y), Color.Black);
                y = y + 20;
            }
            

            spriteBatch.End();


            base.Draw(gameTime);
        }
    }
}
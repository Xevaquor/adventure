using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace dev_adventure
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class DevAdventure : Microsoft.Xna.Framework.Game
    {
        public const int FRAMES_PER_SECOND = 25;

        private Vector2 desiredResolution = new Vector2(1280, 720);
        private Vector2 realResolution = Vector2.Zero;
        private Matrix projectionMatrix = Matrix.Identity;
        private Viewport gameViewport = new Viewport();

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public DevAdventure()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            this.IsFixedTimeStep = true;
            this.TargetElapsedTime = TimeSpan.FromMilliseconds(1000 / FRAMES_PER_SECOND);

            SetGraphicMode();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {

            base.Initialize();
        }

        private void SetGraphicMode()
        {
            bool fulscreen = false;   
            string[] args = Environment.GetCommandLineArgs();            
            try
            {
                realResolution.X = int.Parse(args[1]);
                realResolution.Y = int.Parse(args[2]);
                fulscreen = bool.Parse(args[3]);
            }
            catch (Exception ex)
            {
                logger.Warn("Invalid arguments given. Setting to default.", realResolution.X, realResolution.Y, fulscreen);
                realResolution.X = 1024;
                realResolution.Y = 768;
                fulscreen = false;
            }

            graphics.PreferredBackBufferWidth = (int) realResolution.X;
            graphics.PreferredBackBufferHeight = (int) realResolution.Y;
            graphics.IsFullScreen = fulscreen;
            graphics.ApplyChanges();

            Window.Title = string.Format("{0}x{1}", realResolution.X, realResolution.Y);

            float scale = realResolution.X / desiredResolution.X;
            float aspect = desiredResolution.X / desiredResolution.Y;
            float final_y = realResolution.X / aspect;

            int margin = (int)(Math.Abs(realResolution.Y - final_y) / 2);
            gameViewport = new Viewport(0, margin, (int)realResolution.X, (int)final_y);

            projectionMatrix = Matrix.CreateScale(scale, scale, 1);
            
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            FloatText.Create(spriteBatch, Content.Load<SpriteFont>("default"));
            
            bg = Content.Load<Texture2D>("bg");

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {

        }

        Random rnd = new Random();

        protected override void Update(GameTime gameTime)
        {
            FloatText.UpdateAll();

            base.Update(gameTime);
        }

        Texture2D bg;
        protected override void Draw(GameTime gameTime)
        {
            Viewport full_view = new Viewport(0,0, (int)realResolution.X, (int)realResolution.Y);
            GraphicsDevice.Viewport = full_view;
            GraphicsDevice.Clear(Color.Gray);
            GraphicsDevice.Viewport = gameViewport;

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise, null, projectionMatrix);

            spriteBatch.Draw(bg, new Vector2(0, 0), Color.White);

            FloatText.DrawAll();

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}

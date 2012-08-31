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
using System.Diagnostics;
using System.Threading.Tasks;

namespace dev_adventure
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class DevAdventure : Microsoft.Xna.Framework.Game
    {
        private Matrix projectionMatrix = Matrix.Identity;
        private Viewport gameViewport = new Viewport();

        private Dictionary<string, IGameState> gameStates = null;
        private string currentState, previousState;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public static float Scale;
        public static int Margin;

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        
        public DevAdventure()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            this.IsFixedTimeStep = true;
            this.TargetElapsedTime = TimeSpan.FromMilliseconds(1000 / Settings.FramesPerSecond);

            this.IsMouseVisible = true;

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
            ResMan.Create(Content);
            base.Initialize();
        }

        private void SetGraphicMode()
        {
            graphics.PreferredBackBufferWidth = (int)Settings.Resolution.X;
            graphics.PreferredBackBufferHeight = (int)Settings.Resolution.Y;
            graphics.IsFullScreen = Settings.Fullscreen;
            graphics.ApplyChanges();

            Window.Title = string.Format("{0}x{1}x{2}", Settings.Resolution.X, Settings.Resolution.Y, Settings.FramesPerSecond);

            float scale = Settings.Resolution.X / Settings.DesiredResolution.X;
            float aspect = Settings.DesiredResolution.X / Settings.DesiredResolution.Y;
            float final_y = Settings.Resolution.X / aspect;

            Scale = scale;


            int margin = (int)(Math.Abs(Settings.Resolution.Y - final_y) / 2);
            gameViewport = new Viewport(0, margin, (int)Settings.Resolution.X, (int)final_y);
            Margin = margin;
            projectionMatrix = Matrix.CreateScale(scale, scale, 1);

            logger.Info("Created window {0}x{1} with {2} frames per second, windowed: {3}", Settings.Resolution.X, Settings.Resolution.Y, Settings.FramesPerSecond, !Settings.Fullscreen);

        }
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            ResMan.LoadDefaultContent();
            gameStates = new Dictionary<string, IGameState>();


            gameStates.Add("menu", new MenuGameState());
            gameStates.Add("pause", new PauseGameState());
            gameStates.Add("demo", new DemoGameState());


            foreach (var item in gameStates)
            {
                item.Value.RequestingResources += new ResourceRequestDelegate(Value_RequestingResources);
                item.Value.RequestingStateChange += new StateChangeRequestDelegate(Value_RequestingStateChange);
            }

            currentState = "demo";

            gameStates[currentState].Activate("default");
                    }


        void Value_RequestingStateChange(string name, object obj)
        {
            if (name == null)
            {
                currentState = previousState;
            }
            else
            {
                previousState = currentState;
                currentState = name;
            }
            gameStates[currentState].Activate(obj);

        }

        private string caller = "";
        bool IsBeingLoading = false;
        Task async;

        void Value_RequestingResources(IEnumerable<ResMan.Asset> res_list)
        {
            caller = currentState;
            IsBeingLoading = true;

            Action<object> omg = LoadResourcesAsync;
            async = new Task(omg, res_list);
            async.Start();

            //LoadResourcesAsync(res_list);
        }

        private void LoadResourcesAsync(object res_list)
        {
            progress = 0;
            IEnumerable<ResMan.Asset> request = res_list as IEnumerable<ResMan.Asset>;
            try
            {
                foreach (var item in request)
                {
                    System.Threading.Interlocked.Increment(ref progress);
                    switch (item.Type)
                    {
                        case ResMan.Asset.AssetType.SPRITE_FONT:
                            ResMan.LoadResource<SpriteFont>(item.Name);
                            break;
                        case ResMan.Asset.AssetType.TEXTURE_2D:
                            ResMan.LoadResource<Texture2D>(item.Name);
                            break;
                        default:
                            logger.Error("Unknown resource type: {0}. Resource name: {1}", item.Type, item.Name);
                            break;
                    }

                }
            }
            catch
            {
                return;
            }
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
            InMan.Update();

            if (IsBeingLoading)
            {
                if (async.IsCompleted)
                {
                    IsBeingLoading = false;
                    SuppressDraw();
                    gameStates[currentState].Activate(null);
                }
            }
            else
                gameStates[currentState].Update();

            base.Update(gameTime);
        }

        private int progress = 0;

        protected override void Draw(GameTime gameTime)
        {

            Viewport full_view = new Viewport(0, 0, (int)Settings.Resolution.X, (int)Settings.Resolution.Y);
           // GraphicsDevice.Viewport = full_view;
            GraphicsDevice.Clear(Color.Black);
            try
            {
                GraphicsDevice.Viewport = gameViewport;
            }
            catch
            {
                ;//I really don't know waht's going on. It crashes when unminimizing
            }

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise, null, projectionMatrix);
            if (IsBeingLoading)
            {
                spriteBatch.DrawString(ResMan.GetResource<SpriteFont>("default"), "LOADING>>>>>>" + progress.ToString(), new Vector2(400, 400), Color.Yellow);
            }
            else
            {
                gameStates[currentState].Draw(spriteBatch);
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}

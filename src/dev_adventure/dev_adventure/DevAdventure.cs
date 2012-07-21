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
        private static int frames_per_second = 30;
        public static int FRAMES_PER_SECOND { get { return frames_per_second; } }

        private Vector2 desiredResolution = new Vector2(1920, 1080);
        private Vector2 realResolution = Vector2.Zero;
        private Matrix projectionMatrix = Matrix.Identity;
        private Viewport gameViewport = new Viewport();

        private Dictionary<string, IGameState> gameStates = null;
        private string currentState, previousState;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public static float Scale;
        public static int Margin;

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        Texture2D bugTex = null;

        public DevAdventure()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            this.IsFixedTimeStep = true;
            this.TargetElapsedTime = TimeSpan.FromMilliseconds(1000 / FRAMES_PER_SECOND);

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
            bool fulscreen = false;
            string[] args = Environment.GetCommandLineArgs();
            try
            {
                frames_per_second = int.Parse(args[1]);
                realResolution.X = int.Parse(args[2]);
                realResolution.Y = int.Parse(args[3]);
                fulscreen = bool.Parse(args[4]);
            }
            catch (Exception ex)
            {
                logger.Warn("Invalid arguments given. Setting to default.", realResolution.X, realResolution.Y, fulscreen);
                realResolution.X = 1024;
                realResolution.Y = 768;
                fulscreen = false;
                frames_per_second = 30;
            }

            graphics.PreferredBackBufferWidth = (int)realResolution.X;
            graphics.PreferredBackBufferHeight = (int)realResolution.Y;
            graphics.IsFullScreen = fulscreen;
            graphics.ApplyChanges();

            Window.Title = string.Format("{0}x{1}x{2}", realResolution.X, realResolution.Y, FRAMES_PER_SECOND);

            float scale = realResolution.X / desiredResolution.X;
            float aspect = desiredResolution.X / desiredResolution.Y;
            float final_y = realResolution.X / aspect;

            Scale = scale;


            int margin = (int)(Math.Abs(realResolution.Y - final_y) / 2);
            gameViewport = new Viewport(0, margin, (int)realResolution.X, (int)final_y);
            Margin = margin;
            projectionMatrix = Matrix.CreateScale(scale, scale, 1);

            logger.Info("Created window {0}x{1} with {2} frames per second, windowed: {3}", realResolution.X, realResolution.Y, FRAMES_PER_SECOND, !fulscreen);

        }
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            ResMan.LoadDefaultContent();
            gameStates = new Dictionary<string, IGameState>();


            gameStates.Add("menu", new MenuGameState());
            gameStates.Add("pause", new PauseGameState());


            foreach (var item in gameStates)
            {
                item.Value.RequestingResources += new ResourceRequestDelegate(Value_RequestingResources);
                item.Value.RequestingStateChange += new StateChangeRequestDelegate(Value_RequestingStateChange);
            }

            currentState = "menu";

            gameStates[currentState].Activate("default");

            ResMan.LoadResource<Texture2D>("bug");
            bugTex = ResMan.GetResource<Texture2D>("bug");

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

            bugOrigin = new Vector2(bugTex.Width / 2, bugTex.Height / 2);

            if (InMan.KeyDown(Keys.Right))
                bugAngle += 0.1f;
            else if (InMan.KeyDown(Keys.Left))
                bugAngle -= 0.1f;

            int modifier = 0;
            if (InMan.KeyDown(Keys.Up))
                modifier--;
            if (InMan.KeyDown(Keys.Down))
                modifier++;

            if (InMan.KeyDown(Keys.W))
                camera.Y-= 10;
            if(InMan.KeyDown(Keys.S))
                camera.Y+= 10;
            if (InMan.KeyDown(Keys.A))
                camera.X-= 10;
            if (InMan.KeyDown(Keys.D))
                camera.X+= 10;

            Vector2 v = new Vector2(-r * (float)Math.Sin(bugAngle), r * (float)Math.Cos(bugAngle)) * modifier;
            bugPos += v;

            relativePos = bugPos - camera;

            camera = bugPos - desiredResolution / 2;

            base.Update(gameTime);
        }

        private int progress = 0;

        Vector2 bugPos = new Vector2(120, 120) * 5;
        float bugAngle = MathHelper.ToRadians(30f);
        Vector2 bugOrigin = Vector2.Zero;
        Vector2 camera = Vector2.Zero;
        Vector2 relativePos = Vector2.Zero;
        float r = 10f;


        protected override void Draw(GameTime gameTime)
        {


            Viewport full_view = new Viewport(0, 0, (int)realResolution.X, (int)realResolution.Y);
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

                spriteBatch.Draw(ResMan.GetResource<Texture2D>("checkboard"), Vector2.Zero - camera, Color.Red);

                //spriteBatch.Draw(bugTex, bugPos, Color.White);
                spriteBatch.Draw(bugTex, relativePos, null, Color.White, bugAngle, bugOrigin, 1.0f, SpriteEffects.None, 0);
                spriteBatch.DrawString(ResMan.GetResource<SpriteFont>("default"), "Absolute bug pos: " + bugPos.ToString(), Vector2.Zero, Color.Purple);
                spriteBatch.DrawString(ResMan.GetResource<SpriteFont>("default"), "Camera pos: " + camera.ToString(), new Vector2(0, 100), Color.Purple);
                spriteBatch.DrawString(ResMan.GetResource<SpriteFont>("default"), "relative pos: " + relativePos.ToString(), new Vector2(0, 200), Color.Purple);

                
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}

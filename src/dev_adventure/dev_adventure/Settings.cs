using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace DevAdventure
{
    /// <summary>
    /// Ne tak miało wyglądać moje życie ale .settings nie ma zamiaru działać a ja nie mam czasu zeby się z tym użerać :/
    /// </summary>
    public static class Settings
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public static Vector2 Resolution { get; private set; }
        public static bool Fullscreen { get; private set; }
        public static int FramesPerSecond { get; private set; }
        public static Vector2 DesiredResolution { get; private set; }
        public static float FrameTime { get; private set; }

        private static void SetDefault()
        {
            FramesPerSecond = 60;
            Resolution = new Vector2(1024, 768);
            Fullscreen = false;
            logger.Warn("Invalid arguments given. Setting to default.", Resolution.X, Resolution.Y, Fullscreen);
        }
        static Settings()
        {
            DesiredResolution = new Vector2(1920,1080);
            string[] args = Environment.GetCommandLineArgs();

            try
            {
                FramesPerSecond = int.Parse(args[1]);
                Resolution = new Vector2(int.Parse(args[2]), int.Parse(args[3]));
                Fullscreen = bool.Parse(args[4]);
            }
            catch (ArgumentNullException)
            {
                SetDefault();
            }
            catch (FormatException)
            {
                SetDefault();
            }
            catch (IndexOutOfRangeException)
            {
                SetDefault();
            }

            FrameTime = 1.0f / FramesPerSecond;
        }
    }
}

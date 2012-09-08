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
using FarseerPhysics.Dynamics;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Factories;
using FarseerPhysics.SamplesFramework;

namespace DevAdventure
{
    class Level
    {
        public HashSet<GameObject> Obstacles { get; private set;}

        public string Name { get; private set; }
        public HashSet<ResMan.Asset> RequiredResources {get; private set; }

        private AnimatedSprite background;

        public Level()
        {
            Obstacles = new HashSet<GameObject>();
        }

        public void Update()
        {
            ;
        }
        public void Draw(SpriteBatch batch, Vector2 camera)
        {
            //draw background
            int timesX = (int) Settings.DesiredResolution.X / background.Area.Width;
            int timesY = (int) Settings.DesiredResolution.Y / background.Area.Height;

            float offsetX = -((int)camera.X % background.Area.Width);
            float offsetY = -((int)camera.Y % background.Area.Height);

            for (int X = -1; X <= timesX; X++)
            {
                for (int Y = -1; Y <= timesY; Y++)
                {
                    batch.Draw(background.Sprite,
                        new Vector2(X * background.Area.Width + offsetX, Y * background.Area.Height + offsetY),
                        background.Area, Color.White);                            

                }
            }
        }
        public HashSet<ResMan.Asset> GetRequiredResources()
        {
            var set = new HashSet<ResMan.Asset>();
            set.Add(ResMan.NewTexture2D("bg_a"));
            set.Add(ResMan.NewTexture2D("obstacle_1_a"));
            set.Add(ResMan.NewTexture2D("obstacle_2_a"));

            return set;
        }
        public void LoadFromFile(string filename)
        {
            background = new AnimatedSprite(ResMan.Get<Texture2D>("bg_a"));

            GameObject hole = GameObject.CreateCircular(
                new AnimatedSprite(ResMan.Get<Texture2D>("obstacle_1_a")),
                Vector2.Zero);
            GameObject hole2 = GameObject.CreateRectangular(
                new AnimatedSprite(ResMan.Get<Texture2D>("obstacle_2_a"),4,1,"none"),
                new Vector2(512,512));

            Obstacles.Add(hole);
            Obstacles.Add(hole2);

            //TODO: mapka kafelkowa jednak
        }
    }
}

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
using System.Xml.Linq;
using System.Diagnostics;

namespace DevAdventure
{
    public class Level
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public HashSet<GameObject> Obstacles { get; private set; }

        public string Name { get; private set; }
        
        public HashSet<ResMan.Asset> RequiredResources { get; private set; }
        public List<AnimatedSprite> Textures = new List<AnimatedSprite>();
        
        
        private List<string> textureMap = new List<string>();
        private Dictionary<string, List<Vector2>> dirtyObstacles = new Dictionary<string, List<Vector2>>();
        private Dictionary<int, List<Vector2>> mapObstacles = new Dictionary<int, List<Vector2>>();


        private int[,] tileMap;
        private int rows, cols;



        public Level()
        {
            Obstacles = new HashSet<GameObject>();
            RequiredResources = new HashSet<ResMan.Asset>();
        }

        public void Update()
        {
            ;
        }
        public void Draw(SpriteBatch batch, Vector2 camera)
        {
            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    var buffer = Textures[tileMap[y,x]];
                    batch.Draw(
                        buffer.Sprite, new Vector2(256 * x, 256 * y) - camera, buffer.Area, Color.White);
                }
            }
        }
        public static Level LoadFromFile(string filename)
        {
            Level lvl = new Level();

            var res_list = new List<string>();

            XDocument doc = XDocument.Load(filename);
            var raw = (from x in doc.Descendants() select x.Value).First();


            res_list = (from x in doc.Root.Attributes("backgrounds") select x.Value).First().Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();

            var additional_resources = (from x in doc.Root.Attributes("images") select x.Value).First().Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();

            lvl.textureMap = res_list;
            foreach (var item in res_list)
            {
                lvl.RequiredResources.Add(ResMan.NewTexture2D(item));
            }
            foreach (var item in additional_resources)
            {
                lvl.RequiredResources.Add(ResMan.NewTexture2D(item));
            }
            string[] lines = raw.Split('\n');
            lvl.rows = lines.Length;
            lvl.cols = lines[0].Split(',').Length;
            lvl.tileMap = new int[lvl.rows, lvl.cols];

            for (int y = 0; y < lvl.rows; y++)
            {
                string[] entries = lines[y].Split(',');
                for (int x = 0; x < lvl.cols; x++)
                {
                    lvl.tileMap[y, x] = int.Parse(entries[x]);
                }
            }
            var obstacles = from x in doc.Descendants("Obstacles").Descendants("tile")
                            select new
                            {
                                tileIndex = int.Parse(x.Attribute("tx").Value.ToString()) + 1,
                                Position = new Vector2(
                                    int.Parse(x.Attribute("x").Value.ToString()) * 256 + 128,
                                    int.Parse(x.Attribute("y").Value.ToString()) * 256 + 128)
                            };

            foreach (var obstacle in obstacles)
            {
                if(lvl.mapObstacles.Keys.Contains(obstacle.tileIndex))
                {
                    lvl.mapObstacles[obstacle.tileIndex].Add(obstacle.Position);
                }
                else
                {
                    lvl.mapObstacles[obstacle.tileIndex] = new List<Vector2>();
                    lvl.mapObstacles[obstacle.tileIndex].Add(obstacle.Position);
                }
            }

            foreach (var key in lvl.mapObstacles.Keys)
            {
                lvl.RequiredResources.Add(ResMan.NewTexture2D("obstacle_" + key.ToString()));
            }

            var stones = from x in doc.Root.Descendants("Entities").Descendants("Stone")
                         select new Vector2(float.Parse(x.Attribute("x").Value),
                                 float.Parse(x.Attribute("y").Value));

            lvl.dirtyObstacles["Stone"] = stones.ToList();

            var fires = from x in doc.Root.Descendants("Entities").Descendants("Fire")
                         select new Vector2(float.Parse(x.Attribute("x").Value),
                                 float.Parse(x.Attribute("y").Value));

            lvl.dirtyObstacles["Fire"] = fires.ToList();

            lvl.Filename = filename;
            return lvl;
        }

        public void AssignResources()
        {
            //mamy zgarnięte obrazki, teraz podbindować je
            for (int i = 0; i < textureMap.Count; i++)
            {
                Textures.Add(new AnimatedSprite(ResMan.Get<Texture2D>(textureMap[i])));
            }
            foreach (var outer in mapObstacles)
            {
                string resource_name = "obstacle_" + outer.Key.ToString();
                var img = ResMan.Get<Texture2D>(resource_name);
                int frames = img.Width / 256;
                foreach(var inner in outer.Value)
                {
                    Obstacles.Add(
                        GameObject.CreateRectangular(
                        new AnimatedSprite(img, frames,20f,"being"),
                            inner));
                }
            }

            foreach (var obstacle in dirtyObstacles["Stone"])
            {
                Obstacles.Add(GameObject.CreateStone(obstacle));
            }
            foreach (var obstacle in dirtyObstacles["Fire"])
            {
                Obstacles.Add(GameObject.CreateFire(obstacle));
            }
        }

        public object Filename { get; set; }
    }
}

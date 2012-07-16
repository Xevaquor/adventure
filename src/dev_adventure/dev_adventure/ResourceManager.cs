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
using System.Diagnostics;

namespace dev_adventure
{
    public static class ResMan
    {
        public struct Asset
        {
            public string Name;
            public enum AssetType { SPRITE_FONT, TEXTURE_2D };
            public AssetType Type;
        }

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private static ContentManager loader = null;

        private static Dictionary<string, object> assets = null;

        public static void Create(ContentManager content)
        {
            loader = content;
            assets = new Dictionary<string, object>();
        }

        public static void LoadDefaultContent()
        {
            logger.Info("Loading default content...");

            LoadAsset<SpriteFont>("default");

            logger.Info("Default content loaded");
        }

        public static T GetAsset<T>(string name)
        {
            Debug.Assert(assets.ContainsKey(name));
            object asset = assets[name];
            Debug.Assert(asset is T);

            return (T)asset;
        }

        public static void LoadAsset<T>(string name)
        {
            if (assets.ContainsKey(name))
            {
                logger.Warn("Asset {0} - (requested type - {1}) already loaded.", name, typeof(T).Name);
                return;
            }
            logger.Info("Loading {0}: {1}...", typeof(T).Name, name);
            assets.Add(name, loader.Load<T>(name));
            System.Threading.Thread.Sleep(1000);
            logger.Info("Loaded  {0}: {1}",typeof(T).Name,  name);
        }

        public static bool ContentLoaded(IEnumerable<string> demand)
        {
            var intersecion = demand.Intersect(assets.Keys);
            return intersecion.Count() == demand.Count();
        }
    }
}

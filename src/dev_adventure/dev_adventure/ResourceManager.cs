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

namespace DevAdventure
{
    public static class ResMan
    {
        public struct Asset
        {
            public string Name;
            public enum AssetType { SPRITE_FONT, TEXTURE_2D, LEVEL };
            public AssetType Type;
        }

        public static Asset NewTexture2D(string name)
        {
            return new Asset() { Name = name, Type = Asset.AssetType.TEXTURE_2D };
        }

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private static ContentManager loader;

        private static Dictionary<string, object> assets;

        public static void Create(ContentManager content)
        {
            loader = content;
            assets = new Dictionary<string, object>();
        }

        public static void LoadDefaultContent()
        {
            logger.Info("Loading default content...");

            LoadResource<SpriteFont>("default");

            logger.Info("Default content loaded");
        }

        public static T Get<T>(string name)
        {
            return GetResource<T>(name);
        }

        public static T GetResource<T>(string name)
        {
            Debug.Assert(assets.ContainsKey(name));
            object asset = assets[name];
            Debug.Assert(asset is T);

            return (T) asset;
        }

        public static void LoadResource<T>(string name)
        {
            if (assets.ContainsKey(name))
            {
                logger.Warn("Asset {0} - (requested type - {1}) already loaded.", name, typeof(T).Name);
                return;
            }
            logger.Info("Loading {0}: {1}...", typeof(T).Name, name);
            try
            {
                assets.Add(name, loader.Load<T>(name));
            }
            catch (ContentLoadException ex)
            {
                logger.Error("Failed to load resource. {0}", ex.Message);
                throw;
            }
            logger.Info("Loaded  {0}: {1}",typeof(T).Name,  name);
        }

        public static bool ResourcesLoaded(IEnumerable<ResMan.Asset> demand)
        {
            var names = from x in demand select x.Name;

            var intersecion = names.Intersect(assets.Keys);
            return intersecion.Count() == demand.Count();
        }
    }
}

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
    /// <summary>
    /// Requires used content ALREADY LOADED
    /// </summary>
    class LoadingGameState : GameState
    {
        private delegate bool LoadAsyncDelegate(IEnumerable<ResMan.Asset> request);

        volatile int progress = 0;
        string msg;

        SpriteFont font = null;

        public LoadingGameState()
        {
            font = ResMan.GetResource<SpriteFont>("default");
        }
        public override void Initialize()
        {
            ;
        }
        public override void Draw()
        {
            spriteBatch.DrawString(font, msg, new Vector2(500, 500), Color.Red);
        }
        IAsyncResult async = null;
        LoadAsyncDelegate asyncCall = null;

        public override void Update()
        {
            msg = "Loading: " + progress.ToString();
            if (async == null || async.IsCompleted)
            {
                if (!asyncCall.EndInvoke(async))
                    throw new Exception("Failed to load resources");
                RaiseStateChangeRequest(null);
            }

        }

        public override bool Activate(object request)
        {
            //TODO check if has requred content else - error
            if (request == null)
            {
                return true;
            }
            asyncCall = LoadAsync;
            
            async = asyncCall.BeginInvoke(request as IEnumerable<ResMan.Asset> , null, null);

            return true;
        }

        private bool LoadAsync(IEnumerable<ResMan.Asset> request)
        {
            try
            {
                var omg = request as IEnumerable<ResMan.Asset>;
                foreach (var item in omg)
                {
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
                            return false;
                            break;
                    }
                    System.Threading.Interlocked.Increment(ref progress);
                }
            }
            catch
            {
                return false;
            }
            return true;

        }
    }
}

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
    class LoadingGameState : IGameState
    {
        private delegate void LoadAsyncDelegate(IEnumerable<ResMan.Asset> request);

        volatile int progress = 0;
        string msg;

        SpriteFont font = null;

        public LoadingGameState()
        {
            font = ResMan.GetAsset<SpriteFont>("default");
        }

        public void Draw(SpriteBatch batch)
        {
            batch.DrawString(font, msg, new Vector2(500, 500), Color.Red);
        }
        IAsyncResult async = null;
        public void Update()
        {
            msg = "Loading: " + progress.ToString();
            if (async == null || async.IsCompleted)
                if (StateChangeRequested != null)
                    StateChangeRequested(this, null);

        }

        public void Activate(bool contentLoaded,object request)
        {
            if (request == null)
            {
                return;
            }
            LoadAsyncDelegate asyncCall = LoadAsync;

            async = asyncCall.BeginInvoke(request as IEnumerable<ResMan.Asset> , null, null);
            
        }

        private void LoadAsync(IEnumerable<ResMan.Asset> request)
        {
            var omg = request as IEnumerable<ResMan.Asset>;
            foreach (var item in omg)
            {
                switch (item.Type)
                {
                    case ResMan.Asset.AssetType.SPRITE_FONT:
                        ResMan.LoadAsset<SpriteFont>(item.Name);
                        break;
                    case ResMan.Asset.AssetType.TEXTURE_2D:
                        ResMan.LoadAsset<Texture2D>(item.Name);
                        break;
                    default:
                        break;
                }
                System.Threading.Interlocked.Increment(ref progress);
            }


        }

        public event RequestStateChangeDelegate StateChangeRequested;
        public event RequestContent ContentRequested;


    }
}

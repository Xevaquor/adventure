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
    class PauseGameState : IGameState
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        SpriteFont fnt;
        Vector2 pos = new Vector2(100, 100);
        public PauseGameState()
        {
            requiredAssets.Add(new ResMan.Asset() { Name = "default", Type = ResMan.Asset.AssetType.SPRITE_FONT });
        }

        HashSet<ResMan.Asset> requiredAssets = new HashSet<ResMan.Asset>();
        public void Draw(SpriteBatch batch)
        {
            batch.DrawString(fnt, "PAUSE", pos, Color.Red);
        }
        MouseState ms, pms;
        public void Update()
        {
            pos += Vector2.One;

            pms = ms;
            ms = Mouse.GetState();

            if (ms.LeftButton == ButtonState.Pressed && pms.LeftButton == ButtonState.Released)
                if (StateChangeRequested != null)
                {
                    StateChangeRequested(this, "menu");
                    return;
                }
        }

        public event RequestStateChangeDelegate StateChangeRequested;

     
        public void Activate( object obj)
        {
            if (ResMan.ContentLoaded(requiredAssets))
            {
                fnt = ResMan.GetAsset<SpriteFont>("default");
            }
            else
            {
                if (ContentRequested != null)
                {
                    ContentRequested(this, requiredAssets);
                    return;
                }
                else
                    logger.Warn("ContentRequest event is not handled");
            }
        }

        public event RequestContent ContentRequested;
    }

    public class MenuGameState : IGameState
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        SpriteFont font;
        Texture2D img;
        MouseState ms, pms;

        public MenuGameState()
        {

            requiredAssets.Add(new ResMan.Asset() { Name = "default", Type = ResMan.Asset.AssetType.SPRITE_FONT });
            requiredAssets.Add(new ResMan.Asset() { Name = "bg", Type = ResMan.Asset.AssetType.TEXTURE_2D });
        }

        public void Draw(SpriteBatch batch)
        {
            batch.Draw(img, Vector2.Zero, Color.White);
            batch.DrawString(font, "MENU", new Vector2(500), Color.Purple);
        }

        public void Update()
        {
            pms = ms;
            ms = Mouse.GetState();

            if (ms.LeftButton == ButtonState.Pressed && pms.LeftButton == ButtonState.Released)
                if (StateChangeRequested != null)
                {
                    StateChangeRequested(this, "pause");
                    return;
                }

        }

        public event RequestStateChangeDelegate StateChangeRequested;

        HashSet<ResMan.Asset> requiredAssets = new HashSet<ResMan.Asset>();

        public void Activate(object obj)
        {
            if (ResMan.ContentLoaded(requiredAssets))
            {
                font = ResMan.GetAsset<SpriteFont>("default");
                img = ResMan.GetAsset<Texture2D>("bg");
            }
            else
            {
                if (ContentRequested != null)
                {
                    ContentRequested(this, requiredAssets);
                    return;
                }
                else
                    logger.Warn("ContentRequest event is not handled");
            }
            
        }
        public event RequestContent ContentRequested;


        public HashSet<string> RequiredContent
        {
            get { throw new NotImplementedException(); }
        }
    }

}

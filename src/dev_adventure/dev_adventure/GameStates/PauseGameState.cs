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
        SpriteFont fnt;
        Vector2 pos = new Vector2(100, 100);
        public PauseGameState(SpriteFont font)
        {
            fnt = font;
        }

        public void Draw(SpriteBatch batch)
        {
            pos += Vector2.One;
            batch.DrawString(fnt, "PAUSE", pos, Color.Red);
        }

        public void Update()
        {
            ;
        }

        public void Activate(object obj)
        {
            ;
        }

        public event RequestStateChangeDelegate StateChangeRequested;


        public HashSet<string> RequiredContent
        {
            get { throw new NotImplementedException(); }
        }


        public void Activate()
        {
            throw new NotImplementedException();
        }


        public void Activate(bool contentLoaded, object obj)
        {
            throw new NotImplementedException();
        }

        public event RequestContent ContentRequested;
    }

    public class MenuGameState : IGameState
    {
        SpriteFont font;
        Texture2D img;
        MouseState ms, pms;
        bool hasContent = false;

        public MenuGameState()
        {
            
        }

        public void Draw(SpriteBatch batch)
        {
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

        public void Activate(bool contentLoaded, object obj)
        {
            if (contentLoaded)
            {
                font = ResMan.GetAsset<SpriteFont>("default");
                img = ResMan.GetAsset<Texture2D>("bg");
            }
            else
            {
                HashSet<ResMan.Asset> requiredAssets = new HashSet<ResMan.Asset>();
                requiredAssets.Add(new ResMan.Asset() { Name = "default", Type = ResMan.Asset.AssetType.SPRITE_FONT });
                requiredAssets.Add(new ResMan.Asset() { Name = "bg", Type = ResMan.Asset.AssetType.TEXTURE_2D });
                requiredAssets.Add(new ResMan.Asset() { Name = "anim", Type = ResMan.Asset.AssetType.TEXTURE_2D });
                requiredAssets.Add(new ResMan.Asset() { Name = "bg2", Type = ResMan.Asset.AssetType.TEXTURE_2D });

                if (ContentRequested != null)
                {
                    ContentRequested(this, requiredAssets);
                    return;
                }
            }
            
        }
        public event RequestContent ContentRequested;


        public HashSet<string> RequiredContent
        {
            get { throw new NotImplementedException(); }
        }
    }

}

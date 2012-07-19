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
        FloatText ft = null;
        string msg = "DEFAUT";
        bool omg = false;

        public override void Initialize()
        {
            fnt = ResMan.GetResource<SpriteFont>("default");
        }

        protected override void SetRequiredResources()
        {
            requiredResources.Add(new ResMan.Asset() { Name = "default", Type = ResMan.Asset.AssetType.SPRITE_FONT });
        }

        public override void Draw()
        {
            spriteBatch.DrawString(fnt, msg , pos, Color.Red);
        }
        MouseState ms, pms;
        public override void Update()
        {
            pos += Vector2.One;

            pms = ms;
            ms = Mouse.GetState();

            if (InMan.LeftPressed)
                RaiseStateChangeRequest(null, null);
        }

        public override bool Activate(object obj)
        {
            if (obj is string)
            {
                requiredResources.Add(new ResMan.Asset() { Name = "tahoma", Type = ResMan.Asset.AssetType.SPRITE_FONT });
                //RaiseContentRequest();
            }
            if (!base.Activate(obj))
                return false;



            return true;

        }
    }
}
/*
    public class MenuGameState : GameState
    {
        SpriteFont font;
        Texture2D img;
        MouseState ms, pms;

        FloatText ft;

        public MenuGameState()
        {
            requiredResources.Add(new ResMan.Asset() { Name = "default", Type = ResMan.Asset.AssetType.SPRITE_FONT });
            requiredResources.Add(new ResMan.Asset() { Name = "bg", Type = ResMan.Asset.AssetType.TEXTURE_2D });

           
        }
        Random rnd = new Random();
        public override void Draw()
        {
            spriteBatch.Draw(img, Vector2.Zero, Color.White);
            spriteBatch.DrawString(font, "MENU", new Vector2(500), Color.Purple);

            ft.DrawAll();
        }

        public override void Update()
        {
            pms = ms;
            ms = Mouse.GetState();

            if (ms.LeftButton == ButtonState.Pressed && pms.LeftButton == ButtonState.Released)
            {
                RaiseStateChangeRequest("pause");
                return;
            }

            if (InMan.RightPressed)
            {
                ft.Add("centryfuga", new Vector2(400, 400), Color.LightGreen);
            }
            ft.UpdateAll();

        }
        public override void Activate(object obj)
        {
            //base.Activate
            if (HandleResources())
            {
                if (!initialized)
                {
                    font = ResMan.GetResource<SpriteFont>("default");
                    img = ResMan.GetResource<Texture2D>("bg");
                    ft = new FloatText(spriteBatch, font);
                    initialized = true;
                }              
            }
                        
        }
    }

}
*/
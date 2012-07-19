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
    class DemoGameState : IGameState
    {
        SpriteFont font = null;
        Texture2D checkboard = null;
        string msg = "";

        protected override void SetRequiredResources()
        {
            requiredResources.Add(new ResMan.Asset() { Name = "default", Type = ResMan.Asset.AssetType.SPRITE_FONT });
            requiredResources.Add(new ResMan.Asset() { Name = "checkboard", Type = ResMan.Asset.AssetType.TEXTURE_2D });
        }

        public override void Draw()
        {
            spriteBatch.Draw(checkboard, Vector2.Zero, Color.White);
            spriteBatch.DrawString(font, msg, Vector2.Zero, Color.Tomato);
        }

        public override void Update()
        {
            msg = string.Format("Mouse position: {0}", InMan.MousePosition);
            if (InMan.LeftPressed)
            {
                RaiseStateChangeRequest("pause", "omg");
                //RaiseStateChangeRequest("pause");
            }
        }

        public override bool Activate(object obj)
        {
            if (!base.Activate(obj))
                return false;

            if (obj is string)
            {
                requiredResources.Add(new ResMan.Asset() { Name = "bg", Type = ResMan.Asset.AssetType.TEXTURE_2D });
                RaiseContentRequest();
            }

            return true;
        }
        public override void Initialize()
        {
            font = ResMan.GetResource<SpriteFont>("default");
            checkboard = ResMan.GetResource<Texture2D>("checkboard");
        }
    }
}

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

namespace DevAdventure
{
    class PauseGameState : IGameState
    {
        private SpriteFont font;
        string msg = "PAUSE";

        public PauseGameState()
        {
            requiredResources.Add(new ResMan.Asset() { Name = "default", Type = ResMan.Asset.AssetType.SPRITE_FONT });
        }

        public override void Draw(SpriteBatch batch)
        {
            batch.DrawString(font, msg, new Vector2(400, 400), Color.Red);
        }
        public override void Update()
        {
            if (InMan.LeftPressed)
            {
                RaiseStateChangeRequest(null, "HEHEHE GMOCH");
                return;
            }
        }
        public override void Activate(object obj)
        {
            if (obj is string)
            {
                msg = obj as string;
                requiredResources.Add(new ResMan.Asset() { Name = "bg", Type = ResMan.Asset.AssetType.TEXTURE_2D });
                requiredResources.Add(new ResMan.Asset() { Name = "bg2", Type = ResMan.Asset.AssetType.TEXTURE_2D });
                requiredResources.Add(new ResMan.Asset() { Name = "checkboard", Type = ResMan.Asset.AssetType.TEXTURE_2D });
                requiredResources.Add(new ResMan.Asset() { Name = "tahoma", Type = ResMan.Asset.AssetType.SPRITE_FONT });
                requiredResources.Add(new ResMan.Asset() { Name = "default", Type = ResMan.Asset.AssetType.SPRITE_FONT });
            }

            if(!HandleResources())
             return;
        }

        protected override void AssignResources()
        {
            font = ResMan.GetResource<SpriteFont>("default");
        }
    }
}

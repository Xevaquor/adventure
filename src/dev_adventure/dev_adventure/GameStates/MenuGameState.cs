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

namespace dev_adventure
{
    class MenuGameState : IGameState
    {
        Vector2 pos = Vector2.Zero;
        private SpriteFont fontType = null;
        string fontName = "default";
        string msg = "MENU";
        Texture2D checkboard;

        public MenuGameState()
        {
            requiredResources.Add(new ResMan.Asset() { Name = "checkboard", Type = ResMan.Asset.AssetType.TEXTURE_2D });
        }

        public override void Draw(SpriteBatch batch)
        {
            batch.Draw(checkboard, Vector2.Zero, Color.White);
            batch.DrawString(fontType, msg, pos, Color.Red);
        }

        public override void Update()
        {
            if (InMan.LeftPressed)
            {
                RaiseStateChangeRequest("pause", "CALLED");
                return;
            }

            pos += Vector2.One;
        }

        public override void Activate(object obj)
        {
            if (obj is string)
            {
                msg = obj as string;
                fontName = "tahoma";
                requiredResources.Add(new ResMan.Asset() { Name = fontName, Type = ResMan.Asset.AssetType.SPRITE_FONT });
            }

            //zakładamy że przed base.Activate są ustawione aktualne zasoby
            if (!HandleResources())
                return;

        }

        protected override void AssignResources()
        {
            fontType = ResMan.GetResource<SpriteFont>(fontName);
            checkboard = ResMan.GetResource<Texture2D>("checkboard");
        }


    }
}

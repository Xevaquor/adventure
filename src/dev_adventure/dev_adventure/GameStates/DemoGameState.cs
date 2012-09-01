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
    class DemoGameState : IGameState
    {
        private GameObject bug;
        private GameObject bg;

        private FloatText floatingText;
        public DemoGameState()
        {
            //TODO: ResMan.AddTeture2D
            requiredResources.Add(new ResMan.Asset() { Name = "bug", Type = ResMan.Asset.AssetType.TEXTURE_2D });
            requiredResources.Add(new ResMan.Asset() { Name = "checkboard", Type = ResMan.Asset.AssetType.TEXTURE_2D });
        }

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch batch)
        {
            DrawGameObject(batch, bg);
            DrawGameObject(batch, bug);
            
            floatingText.DrawAll(batch);
        }

        public override void Update()
        {
            if (InMan.KeyDown(Keys.Left))
                bug.Rotate(-90);
            if (InMan.KeyDown(Keys.Right))
                bug.Rotate(+90);
            if (InMan.KeyDown(Keys.Up))
                bug.MoveStraight(+120);
            if (InMan.KeyDown(Keys.Down))
                bug.MoveStraight(-120);

            if (InMan.LeftPressed)
            {
                floatingText.Add("Q33NY", InMan.MousePosition, Color.Red);
            }

            floatingText.UpdateAll();
            LookAt(bg);
        }

        protected override void AssignResources()
        {
            bug = new GameObject(new AnimatedSprite(ResMan.Get<Texture2D>("bug")), Vector2.Zero);
            bg = new GameObject(new AnimatedSprite(ResMan.Get<Texture2D>("checkboard")), Vector2.Zero);
            floatingText = new FloatText(ResMan.GetResource<SpriteFont>("default"));
        }

        public override void Activate(object obj)
        {
            if (!HandleResources())
                return;
        }
    }
}

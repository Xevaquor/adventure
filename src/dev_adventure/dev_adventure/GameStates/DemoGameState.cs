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
using FarseerPhysics.Dynamics;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Factories;
using FarseerPhysics.SamplesFramework;
using FarseerPhysics.Common;

namespace DevAdventure
{
    class DemoGameState : IGameState
    {
        private GameObject bug;
        private GameObject bg;
        private GameObject feature;

        private FloatText floatingText;

        World world = new World(Vector2.Zero);
        Body boundary;

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
            DrawGameObject(batch, feature);

            batch.DrawString(ResMan.Get<SpriteFont>("default"), bug.Position.ToString(), Vector2.Zero, Color.Red);
            batch.DrawString(ResMan.Get<SpriteFont>("default"), feature.Position.ToString(), Vector2.Zero, Color.Red);

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

            world.Step(Settings.FrameTime);

            bug.Update();
            feature.Update();

            floatingText.UpdateAll();
            LookAt(bug);
        }

        protected override void AssignResources()
        {
            // bug = new GameObject(new AnimatedSprite(ResMan.Get<Texture2D>("bug")), Vector2.Zero);
            bg = new GameObject(new AnimatedSprite(ResMan.Get<Texture2D>("checkboard")), Vector2.Zero);
            floatingText = new FloatText(ResMan.GetResource<SpriteFont>("default"));

            world = new World(Vector2.Zero);
            bug = CreatePhysicsObject(new AnimatedSprite(ResMan.Get<Texture2D>("bug")), Vector2.Zero);
            feature = CreatePhysicsObject(new AnimatedSprite(ResMan.Get<Texture2D>("bug")), new Vector2(400, 400));

            bug.PhysicsBody.Mass = 5;
            feature.PhysicsBody.Mass = 10;

            bug.PhysicsBody.OnCollision += new OnCollisionEventHandler(PhysicsBody_OnCollision);
            /*
            var bounds = new Vertices(4);
            bounds.Add(ConvertUnits.ToSimUnits(new Vector2(-500, -500)));
            bounds.Add(ConvertUnits.ToSimUnits(new Vector2(1920, 0)));
            bounds.Add(ConvertUnits.ToSimUnits(new Vector2(1920, 1080)));
            bounds.Add(ConvertUnits.ToSimUnits(new Vector2(0, 1080)));*/
            /*
            boundary = new Body(world);
            boundary.CreateFixture(new CircleShape(ConvertUnits.ToSimUnits(10), 0));
            boundary.Position = ConvertUnits.ToSimUnits(new Vector2(400, 400));
          /*  boundary.CollisionCategories = Category.All;
            boundary.CollidesWith = Category.All;
            boundary.BodyType = BodyType.Dynamic;*/
        }

        bool PhysicsBody_OnCollision(Fixture fixtureA, Fixture fixtureB, FarseerPhysics.Dynamics.Contacts.Contact contact)
        {
            /* System.Windows.Forms.MessageBox.Show(feature.Position.ToString() + " " + bug.Position.ToString());
             System.Windows.Forms.MessageBox.Show(feature.PhysicsBody.Position.ToString() + " " + bug.PhysicsBody.Position.ToString());
             */
            return true;
        }

        public override void Activate(object obj)
        {
            if (!HandleResources())
                return;
        }

        private GameObject CreatePhysicsObject(AnimatedSprite animated_sprite, Vector2 pos, float rot = 0.0f)
        {
            GameObject obj = new GameObject(animated_sprite, pos, rot);
            obj.PhysicsBody = BodyFactory.CreateBody(world);
            //obj.PhysicsFixture = obj.PhysicsBody.CreateFixture(new CircleShape(ConvertUnits.ToSimUnits(animated_sprite.Area.Width / 2), 0f));
            /*obj.PhysicsFixture = obj.PhysicsBody.CreateFixture(
                new FarseerPhysics.Collision.Shapes.LoopShape(
                new Vertices(
                    new Vector2[] { new Vector2(0,0),
                        new Vector2(0, animated_sprite.Area.Width),
                        new Vector2(animated_sprite.Area.Height),
                        new Vector2(animated_sprite.Area.Width),
                        new Vector2(animated_sprite.Area.Width, 0)})));*/

            var q = new PolygonShape(1);/*
            q.SetAsBox(ConvertUnits.ToSimUnits(animated_sprite.Area.Width / 2),
                ConvertUnits.ToSimUnits( animated_sprite.Area.Height / 2),
                ConvertUnits.ToSimUnits(new Vector2(animated_sprite.Area.Height / 2,
                    animated_sprite.Area.Height / 2)), 0);*/

            q.SetAsBox(ConvertUnits.ToSimUnits(animated_sprite.Area.Width / 2),
                ConvertUnits.ToSimUnits(animated_sprite.Area.Height / 2));
            obj.PhysicsFixture = obj.PhysicsBody.CreateFixture(q);

            obj.PhysicsBody.BodyType = BodyType.Dynamic;
            obj.PhysicsBody.Position = ConvertUnits.ToSimUnits(pos);
            obj.PhysicsBody.Rotation = MathHelper.ToRadians(rot);
            obj.PhysicsBody.Friction = 50;
            return obj;
        }
    }
}

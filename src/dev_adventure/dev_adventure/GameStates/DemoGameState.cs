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

        private FloatText floatingText;

        World world = new World(Vector2.Zero);

        IEnumerable<GameObject> Obstacles;

        List<Character> enemies = new List<Character>();

        Level level;
        public Character Player;

        public DemoGameState()
        {
            requiredResources.Add(new ResMan.Asset() { Name = "bug2", Type = ResMan.Asset.AssetType.TEXTURE_2D });
        }

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch batch)
        {
           /*rawGameObject(batch, bg);
            DrawGameObject(batch, bug);*/
            level.Draw(batch, camera);

            foreach (var item in Obstacles)
            {
                DrawGameObject(batch, item);
            }

            enemies.RemoveAll((e) => !e.Alive);

            foreach (var item in enemies)
            {
                DrawGameObject(batch, item);
                foreach (var inner in item.Bullets)
                {
                    DrawGameObject(batch, inner);
                }
            }
            foreach (var item in Player.Bullets)
            {
                DrawGameObject(batch, item);
            }

           // DrawGameObject(batch, bg);
            DrawGameObject(batch, bug);
            DrawGameObject(batch, Player);
            
            floatingText.DrawAll(batch);

            batch.DrawString(ResMan.Get<SpriteFont>("default"), Player.Health.ToString(), Vector2.Zero, Color.Red);
        }

        public override void Update()
        {
            

            if (InMan.LeftPressed)
            {
                var enemy = Character.CreateBug(InMan.MousePosition + camera);
                enemy.PhysicsBody.OnCollision += new OnCollisionEventHandler(PhysicsBody_OnCollision);
                enemy.PhysicsBody.OnSeparation += new OnSeparationEventHandler(PhysicsBody_OnSeparation);
                enemies.Add(enemy);
            }
            if (InMan.RightPressed)
            {
                RaiseStateChangeRequest("demo", "showcase2.oel");
                return;
            }

            world.Step(Settings.FrameTime);
            bug.Update();


            foreach (var item in Obstacles)
            {
                item.Update();
            }
            foreach (var item in enemies)
            {
                item.Update();
            }
            Player.Update();

            if (!Player.Alive)
            {
                System.Windows.Forms.MessageBox.Show("LOOSER");
                RaiseStateChangeRequest("demo", level.Filename);
            }

            floatingText.UpdateAll();
            LookAt(Player);
        }

        void PhysicsBody_OnSeparation(Fixture fixtureA, Fixture fixtureB)
        {
            Character player = fixtureB.Body.UserData as Character;
            Character bug = fixtureA.Body.UserData as Character;
            if (player != null && bug != null)
            {
                if (bug.Relations != player.Relations)
                {
                    player.TakingTouchDamage -= 1;
                }
            }
        }

        bool PhysicsBody_OnCollision(Fixture fixtureA, Fixture fixtureB, FarseerPhysics.Dynamics.Contacts.Contact contact)
        {
            Character player = fixtureB.Body.UserData as Character;
            Character bug = fixtureA.Body.UserData as Character;
            if (player != null && bug != null)
            {
                if (bug.Relations != player.Relations)
                {
                    player.TakingTouchDamage += 1;
                }
            }
            return true;
        }


        protected override void AssignResources()
        {
            world = new World(Vector2.Zero);
            GameObject.World = world;

            bug = GameObject.CreateCircular(new AnimatedSprite(ResMan.Get<Texture2D>("bug2"),4,1,"walk"), new Vector2(500, 500), BodyType.Dynamic);
            Player = Character.CreatePlayer(new Vector2(500, 500));
           
            floatingText = new FloatText(ResMan.GetResource<SpriteFont>("default"));

            //załozenie że są zasoby
            level.AssignResources();
            Obstacles = level.Obstacles;
        }


        public override void Activate(object obj)
        {
            if (obj != null)
            {
                level = Level.LoadFromFile(obj as string);

                foreach (var res in level.RequiredResources)
                {
                    requiredResources.Add(res);
                }
                requiredResources.Add(ResMan.NewTexture2D("bug"));
                requiredResources.Add(ResMan.NewTexture2D("bullet"));
            }
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

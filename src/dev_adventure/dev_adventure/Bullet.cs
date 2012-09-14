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
using System.Diagnostics;

namespace DevAdventure
{
    class Bullet : GameObject
    {
        public Vector2 Velocity { get; set; }
        public int Damage { get; set; }

        public Bullet(AnimatedSprite sprite, Vector2 pos, float angle = 0.0f)
            : base(sprite, pos, angle)
        {
            PhysicsBody.OnCollision += new OnCollisionEventHandler(PhysicsBody_OnCollision);
        }

        public override void Update()
        {
            base.Update();
            this.PhysicsBody.LinearVelocity = Velocity;
        }

        public static Bullet CreateBullet(Vector2 pos, float angle)
        {
            var sprite = new AnimatedSprite(ResMan.Get<Texture2D>("bullet"));
            Bullet obj = new Bullet(sprite, pos, angle);
            obj.Velocity = new Vector2((float)(100 * Math.Sin(obj.Rotation)), (float)(-100 * Math.Cos(obj.Rotation))) / Settings.FramesPerSecond;
            obj.Damage = 10;

            obj.PhysicsBody.CreateFixture(new CircleShape(ConvertUnits.ToSimUnits(obj.Size.X / 2), 0));
            obj.PhysicsBody.BodyType = BodyType.Dynamic;
            obj.PhysicsBody.Mass = 10000;

            obj.PhysicsBody.CollisionCategories = (Category) 0x02;
            obj.PhysicsBody.CollidesWith = (Category) (0x1 + 0x4);

            obj.PhysicsBody.UserData = obj;
            obj.PhysicsBody.OnCollision += new OnCollisionEventHandler(PhysicsBody_OnCollision);

            return obj;
        }

        static bool PhysicsBody_OnCollision(Fixture fixtureA, Fixture fixtureB, FarseerPhysics.Dynamics.Contacts.Contact contact)
        {
            //make it dead
            Bullet b = null;
            b = fixtureA.Body.UserData as Bullet;
            Debug.Assert(b != null);

            //hit entity
            Character c = fixtureB.Body.UserData as Character;
            if (c != null)
            {
                c.Kill();
            }
            b.Kill();

            return true;
        }
    }
}

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

namespace DevAdventure
{
    interface IMovementStyle
    {
        /// <summary>
        /// Force to apply
        /// </summary>
        Vector2 Force { get; }
        /// <summary>
        /// Manual movement
        /// </summary>
        Vector2 Offset { get; }
        /// <summary>
        /// Rotate manually, in radians
        /// </summary>
        float Angle { get; }
        void Think(Character c);
    }

    class PatrolMovementStyle : IMovementStyle
    {
        private Vector2 velocity;

        public void Think(Character c)
        {
            velocity = new Vector2(10, 0) * c.PhysicsBody.Mass;
        }

        public Vector2 Force
        {
            get { return velocity; }
        }

        public Vector2 Offset
        {
            get { return Vector2.Zero; }
        }

        public float Angle
        {
            get { return 0.0f; }
        }
    }
    class ManualMovementStyle : IMovementStyle
    {
        protected Vector2 force, offset;
        protected float angle;

        public void Think(Character c)
        {
            offset = Vector2.Zero;
            force = Vector2.Zero;
            angle = 0.0f;

            if (InMan.KeyDown(Keys.Left))
                angle = MathHelper.ToRadians(-240) / Settings.FramesPerSecond;
            if (InMan.KeyDown(Keys.Right))
                angle = MathHelper.ToRadians(+240) / Settings.FramesPerSecond;

            if (InMan.KeyDown(Keys.Up))
                force = new Vector2((float)(c.MOVEMENT_SPEED * Math.Sin(c.Rotation)), (float)(-c.MOVEMENT_SPEED * Math.Cos(c.Rotation))) * c.PhysicsBody.Mass;/// Settings.FramesPerSecond;

            if (InMan.KeyPressed(Keys.Space))
            {
                //var v = c.Position + 
                c.Bullets.Add(Bullet.CreateBullet(c.Position + c.GetDirectionalVector(100), c.Rotation));

            }


            //c.MoveStraight(+220);
            /*if (InMan.KeyDown(Keys.Down))
                c.MoveStraight(-220);*/
        }

        public Vector2 Force
        {
            get { return force; }
        }

        public Vector2 Offset
        {
            get { return offset; }
        }

        public float Angle
        {
            get { return angle; }
        }
    }


    class Character : GameObject
    {
        public Character(AnimatedSprite sprite, Vector2 pos, float angle = 0.0f)
            : base(sprite, pos, angle)
        {
            ai = new PatrolMovementStyle();
            Health = 0;
            MOVEMENT_SPEED = 120;
            PhysicsBody.UserData = this;
        }

        public int MOVEMENT_SPEED { get; set; }

        public List<Bullet> Bullets = new List<Bullet>();

        public override void Update()
        {
            base.Update();

            ai.Think(this);
            this.PhysicsBody.ApplyForce(ai.Force);
            this.Position += ai.Offset;
            this.Rotation += ai.Angle;

            Bullets.RemoveAll((b) => !b.Alive);
            
            foreach (var item in Bullets)
            {
                item.Update();
                item.PhysicsBody.OnCollision += new OnCollisionEventHandler(PhysicsBody_OnCollision);
            } 
            Health -= TakingTouchDamage;
            if (Health <= 0)
                Kill();
        }

        bool PhysicsBody_OnCollision(Fixture fixtureA, Fixture fixtureB, FarseerPhysics.Dynamics.Contacts.Contact contact)
        {
           // System.Windows.Forms.MessageBox.Show("Test");
            return true;
        }

        protected IMovementStyle ai;

        public int Health { get; set; }

        public static Character CreateBug(Vector2 pos)
        {
            AnimatedSprite sprite = new AnimatedSprite(ResMan.Get<Texture2D>("bug"), 1, 1, "none");

            Character obj = new Character(sprite, pos);
            obj.PhysicsBody.CreateFixture(new CircleShape(ConvertUnits.ToSimUnits(obj.Size.X / 2), 0));
            obj.PhysicsBody.BodyType = BodyType.Dynamic;
            obj.Health = 100;
            obj.PhysicsBody.Mass = 200;
            obj.PhysicsBody.UserData = obj;
            obj.Relations = Relationship.Enemy;
            obj.PhysicsBody.CollisionCategories = (Category) 0x04;
            obj.PhysicsBody.CollidesWith = (Category.All);
            return obj;
        }

        public static Character CreatePlayer(Vector2 pos)
        {
            AnimatedSprite sprite = new AnimatedSprite(ResMan.Get<Texture2D>("bug2"), 4, 1, "walk");

            Character obj = new Character(sprite, pos);
            obj.PhysicsBody.CreateFixture(new CircleShape(ConvertUnits.ToSimUnits(obj.Size.X / 2), 0));
            obj.PhysicsBody.BodyType = BodyType.Dynamic;
            obj.Health = 1000;
            obj.ai = new ManualMovementStyle();
            obj.MOVEMENT_SPEED = 80;
            obj.PhysicsBody.Mass = 100;
            obj.PhysicsBody.UserData = obj;
            obj.Relations = GameObject.Relationship.Friendly;
            obj.PhysicsBody.CollisionCategories = (Category) 0x04;
            obj.PhysicsBody.CollidesWith = (Category.All);
            return obj;
        }
        public override void Kill()
        {
            base.Kill();
            foreach (var item in Bullets)
            {
                item.Kill();
            }
        }

        public int TakingTouchDamage { get; set; }
    }
}

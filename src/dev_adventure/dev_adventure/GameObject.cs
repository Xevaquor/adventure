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
    public class GameObject
    {
        /// <summary>
        /// Position in game units. Autoassign physics units.
        /// </summary>
        public Vector2 Position
        {
            get { return ConvertUnits.ToDisplayUnits(PhysicsBody.Position); }
            set { PhysicsBody.Position = ConvertUnits.ToSimUnits(value); }
        }
        /// <summary>
        /// In radians
        /// </summary>
        public float Rotation
        {
            get { return PhysicsBody.Rotation; }
            set { PhysicsBody.Rotation = value; }
        }
        public float DegreesRotation
        {
            get { return MathHelper.ToDegrees(Rotation); }
            set { Rotation = MathHelper.ToRadians(value); }
        }
       // public Vector2 Velocity;
        public Vector2 Origin;

        public Vector2 Size { get { return new Vector2(Sprite.Area.Width, Sprite.Area.Height); } }

        public AnimatedSprite Sprite;

        public Body PhysicsBody;
        public Fixture PhysicsFixture;

        public static World World;

        public GameObject(AnimatedSprite animated_sprite, Vector2 pos, float rot, Vector2 ori, Vector2 vel)
        {
            PhysicsBody = BodyFactory.CreateBody(World);
            Sprite = animated_sprite;
            Position = pos;
            Rotation = rot;
            PhysicsBody.Rotation = Rotation;
            Origin = ori;
        }
        public GameObject(AnimatedSprite animated_sprite, Vector2 pos, float rot = 0.0f)
            : this(animated_sprite, pos, rot, Vector2.Zero, Vector2.Zero)
        {
            Origin = new Vector2(Sprite.Area.Width / 2, Sprite.Area.Height / 2);
        }

        /// <summary>
        /// Call AFTER physics step
        /// </summary>
        public virtual void Update()
        {
            Sprite.Update();
            PhysicsBody.LinearVelocity = Vector2.Zero;
            PhysicsBody.AngularVelocity = 0;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="angle">Angle in degrees</param>
        public void Rotate(float angle)
        {
            Rotation += MathHelper.ToRadians(angle) / Settings.FramesPerSecond;
            PhysicsBody.Rotation = Rotation;
        }
        public void MoveStraight(float distance)
        {
          //  distance /= Settings.FramesPerSecond;
            Vector2 vel = new Vector2((float)(distance * Math.Sin(Rotation)), (float)(-distance * Math.Cos(Rotation)));
            //vel /= Settings.FramesPerSecond;
            PhysicsBody.LinearVelocity = ConvertUnits.ToSimUnits(vel);
             

           // PhysicsBody.LinearVelocity = new Vector2(0, 1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tex">Sprite</param>
        /// <param name="pos">Postion</param>
        /// <param name="angle">Angle in radians</param>
        /// <returns></returns>
        public static GameObject CreateCircular(AnimatedSprite sprite, Vector2 pos, BodyType type = BodyType.Static, float angle = 0f)
        {
            GameObject obj = new GameObject(sprite, pos, angle);
            obj.PhysicsBody.CreateFixture(new CircleShape(ConvertUnits.ToSimUnits(obj.Size.X / 2), 0));
            obj.PhysicsBody.BodyType = type;
            return obj;
        }
        public static GameObject CreateStone(Vector2 pos)
        {
            AnimatedSprite sprite = new AnimatedSprite(ResMan.Get<Texture2D>("stone"));
            GameObject obj = CreateCircular(sprite, pos);
            return obj;
        }
        public static GameObject CreateNonPhysics(AnimatedSprite sprite, Vector2 pos, float angle = 0f)
        {
            GameObject obj = new GameObject(sprite, pos, angle);
            //obj.PhysicsBody.IsSensor = true;
            return obj;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sprite"></param>
        /// <param name="pos"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static GameObject CreateRectangular(AnimatedSprite sprite, Vector2 pos, BodyType type = BodyType.Static)
        {
            GameObject obj = new GameObject(sprite, pos);
            var q = new PolygonShape(1);
            q.SetAsBox(ConvertUnits.ToSimUnits(obj.Size.X / 2),
                ConvertUnits.ToSimUnits(obj.Size.Y / 2));

            obj.PhysicsBody.CreateFixture(q);
            obj.PhysicsBody.BodyType = type;
            return obj;
        }


        public static GameObject CreateFire(Vector2 obstacle)
        {
            AnimatedSprite sprite = new AnimatedSprite(ResMan.Get<Texture2D>("fire"), 8, 0.5f, "burning");
            return GameObject.CreateCircular(sprite, obstacle);
        }
    }
}

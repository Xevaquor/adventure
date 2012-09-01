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
    class GameObject
    {
        public Vector2 Position;
        /// <summary>
        /// In radians
        /// </summary>
        public float Rotation;
        public Vector2 Velocity;
        public Vector2 Origin;

        public AnimatedSprite Sprite;

        public Body PhysicsBody;
        public Fixture PhysicsFixture;
        
        public GameObject(AnimatedSprite animated_sprite, Vector2 pos, float rot, Vector2 ori, Vector2 vel, Body body)
        {
            Sprite = animated_sprite;
            Position = pos;
            Rotation = rot;
            Velocity = vel;
            Origin = ori;
            PhysicsBody = body;
        }
        public GameObject(AnimatedSprite animated_sprite, Vector2 pos, float rot = 0.0f)
            : this(animated_sprite, pos, rot, Vector2.Zero, Vector2.Zero, null)
        {
            Origin = new Vector2(Sprite.Area.Width / 2, Sprite.Area.Height / 2);
        }

        /// <summary>
        /// Call AFTER physics step
        /// </summary>
        public void Update()
        {
            Position += Velocity / Settings.FramesPerSecond;
            Sprite.Update();
            Position = ConvertUnits.ToDisplayUnits(PhysicsBody.Position);
            Rotation = PhysicsBody.Rotation;
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
            Vector2 vel = new Vector2((float) (distance * Math.Sin(Rotation)),(float) (-distance * Math.Cos(Rotation)));
            vel /= Settings.FramesPerSecond;
            PhysicsBody.LinearVelocity = vel;
        }
    }
}

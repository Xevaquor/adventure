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
        Vector2 Movement { get; }
        void Think();
    }

    class PatrolMovementStyle : IMovementStyle
    {
        private Vector2 velocity;

        public void Think()
        {
            velocity = new Vector2(10, 0);
        }

        Vector2 IMovementStyle.Movement
        {
            get { return velocity;}
        }
    }

    class Enemy : GameObject
    {
        public Enemy(AnimatedSprite sprite, Vector2 pos, float angle = 0.0f)
            : base(sprite, pos, angle)
        {
            ai = new PatrolMovementStyle();
        }
        
        public override void Update()
        {
            base.Update();
            ai.Think();
            this.PhysicsBody.ApplyForce(ai.Movement);
        }

        protected IMovementStyle ai;

        public static Enemy CreateBug(Vector2 pos)
        {
            AnimatedSprite sprite = new AnimatedSprite(ResMan.Get<Texture2D>("bug"), 1, 1, "none");

            Enemy obj = new Enemy(sprite, pos);
            obj.PhysicsBody.CreateFixture(new CircleShape(ConvertUnits.ToSimUnits(obj.Size.X / 2), 0));
            obj.PhysicsBody.BodyType = BodyType.Dynamic;
            return obj;

        }
    }
}

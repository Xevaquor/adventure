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
    class GameObject
    {
        public Vector2 Position;
        public float Rotation;
        public Vector2 Velocity;
        public Vector2 Origin;

        public AnimatedSprite Sprite;
        
        public GameObject(AnimatedSprite animated_sprite, Vector2 pos, float rot, Vector2 ori, Vector2 vel)
        {
            Sprite = animated_sprite;
            Position = pos;
            Rotation = rot;
            Velocity = vel;
            Origin = ori;
        }
        public GameObject(AnimatedSprite animated_sprite, Vector2 pos, float rot = 0.0f)
            : this(animated_sprite, pos, rot, Vector2.Zero, Vector2.Zero)
        {
            Origin = new Vector2(Sprite.Area.Width / 2, Sprite.Area.Height / 2);
        }

        public void Update()
        {
            Position += Velocity / Settings.FramesPerSecond;
            Sprite.Update();
        }
        public void Rotate(float angle)
        {
            Rotation += MathHelper.ToRadians(angle) / Settings.FramesPerSecond;
        }
        public void MoveStraight(float distance)
        {
            Vector2 vel = new Vector2((float) (distance * Math.Sin(Rotation)),(float) (-distance * Math.Cos(Rotation)));
            Position += vel / Settings.FramesPerSecond;            
        }
    }
}

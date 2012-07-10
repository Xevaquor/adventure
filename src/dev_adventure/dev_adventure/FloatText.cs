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
using System.Diagnostics;

namespace dev_adventure
{
    class FloatText
    {
        private static SpriteBatch spriteBatch = null;
        private static SpriteFont font = null;
        private static Vector2 velocity = Vector2.Zero;             //px/sec
        private static LinkedList<FloatText> collection = null;

        private static float delta = 0;

        private const float FADE_SPEED = 1.1f;
        private const float TIME_TO_LIVE = 1.1f;


        private string message;
        private Vector2 position;
        private Color color;
        private float livingTime;

        public static void Create(SpriteBatch batch, SpriteFont spriteFont)
        {
            spriteBatch = batch;
            font = spriteFont;
            velocity = new Vector2(0, -120);
            collection = new LinkedList<FloatText>();
        }
        public static void UpdateAll(float deltaTime)
        {
            delta = deltaTime;
            LinkedListNode<FloatText> node = collection.First;
            while (node != null)
            {
                if (!node.Value.Update())
                    collection.Remove(node);
                node = node.Next;
            }
        }
        public static void DrawAll()
        {
            foreach (var item in collection)
            {
                item.Draw();
            }
        }
        public static void Add(string msg, Vector2 pos, Color c)
        {
            FloatText ft = new FloatText();
            ft.message = msg;
            ft.position = pos;
            ft.color = c;
            ft.livingTime = 0.0f;

            collection.AddFirst(ft);
        }

        /// <returns>False if dead, true if alive</returns>
        public bool Update()
        {
            livingTime += delta;
            if (livingTime > TIME_TO_LIVE)
                return false;
            position += velocity * delta;
            checked
            {
                try
                {
                    color.A -= (byte)(delta * FADE_SPEED * 255);
                }
                catch (OverflowException ex)
                {
                    color.A = 0;
                }                
            }

            return true;
        }

        public void Draw()
        {
            spriteBatch.DrawString(font, message, position, color);
        }
    }
}

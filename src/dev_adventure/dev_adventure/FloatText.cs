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
        private static Vector2 velocity = Vector2.Zero;             
        private static LinkedList<FloatText> collection = null;

        private const float TIME_TO_LIVE = DevAdventure.FRAMES_PER_SECOND;


        private string message;
        private Vector2 position;
        private Color color;
        private float livingTime;

        public static void Create(SpriteBatch batch, SpriteFont spriteFont)
        {
            spriteBatch = batch;
            font = spriteFont;
            velocity = new Vector2(0, -1);
            collection = new LinkedList<FloatText>();
        }
        public static void UpdateAll(float deltaTime)
        {
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
            if (livingTime++ >= TIME_TO_LIVE)
                return false;
            position += velocity;
                    color.A -= (byte)(10);
                
            return true;
        }

        public void Draw()
        {
            spriteBatch.DrawString(font, message, position, color);
        }
    }
}

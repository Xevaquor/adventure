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

        private float TIME_TO_LIVE = DevAdventure.FRAMES_PER_SECOND;
        
        private string message;
        private Vector2 position;
        private Color color;
        private float livingTime;
        private Vector2 origin;

        /// <summary>
        /// Init quas-singleton. Has to be called after SpriteBatch created.
        /// </summary>
        /// <param name="batch"></param>
        /// <param name="spriteFont"></param>
        public static void Create(SpriteBatch batch, SpriteFont spriteFont)
        {
            Debug.Assert(batch != null);
            spriteBatch = batch;
            font = spriteFont;
            velocity = new Vector2(0, -80.0f / DevAdventure.FRAMES_PER_SECOND);
            collection = new LinkedList<FloatText>();
        }
        /// <summary>
        /// Updates all text objects. Call once a frame.
        /// </summary>
        public static void UpdateAll()
        {
            LinkedListNode<FloatText> node = collection.First;
            while (node != null)
            {
                var next = node.Next;
                if (!node.Value.Update())
                    collection.Remove(node);
                node = next;
            }
        }
        /// <summary>
        /// Draw all floating texts.
        /// </summary>
        public static void DrawAll()
        {
            foreach (var item in collection)
            {
                item.Draw();
            }
        }
        /// <summary>
        /// Adds new floating text. Destruction is automatic
        /// </summary>
        /// <param name="msg">Message to be shown.</param>
        /// <param name="pos">Position of center of the message.</param>
        /// <param name="c">Message colour.</param>
        public static void Add(string msg, Vector2 pos, Color c)
        {
            FloatText ft = new FloatText();
            ft.message = msg;
            ft.position = pos;
            ft.color = c;
            ft.livingTime = 0.0f;
            ft.origin = font.MeasureString(ft.message) / 2;

            collection.AddFirst(ft);
        }

        /// <returns>False if dead, true if alive</returns>
        public bool Update()
        {
            if (livingTime++ >= TIME_TO_LIVE)
                return false;
            position += velocity;
                    color.A -= (byte)(255 / TIME_TO_LIVE);
                
            return true;
        }

        public void Draw()
        {
            spriteBatch.DrawString(font, message, position, color, 0.0f, origin, 1f, SpriteEffects.None, 0);
        }
    }
}

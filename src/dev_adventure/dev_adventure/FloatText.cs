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
        private class FloatTextInstance
        {
            public string message;
            public Vector2 position;
            public Color color;
            public float livingTime;
            public Vector2 origin;
        }

        private SpriteBatch spriteBatch = null;
        private SpriteFont font = null;
        private Vector2 velocity = Vector2.Zero;             
        private LinkedList<FloatTextInstance> collection = null;

        private float TIME_TO_LIVE = DevAdventure.FRAMES_PER_SECOND;
        
        /// <summary>
        /// Init quas-singleton. Has to be called after SpriteBatch created.
        /// </summary>
        /// <param name="batch"></param>
        /// <param name="spriteFont"></param>
        public FloatText(SpriteBatch batch, SpriteFont spriteFont)
        {
            Debug.Assert(batch != null);
            spriteBatch = batch;
            font = spriteFont;
            velocity = new Vector2(0, -80.0f / DevAdventure.FRAMES_PER_SECOND);
            collection = new LinkedList<FloatTextInstance>();
        }
        /// <summary>
        /// Updates all text objects. Call once a frame.
        /// </summary>
        public void UpdateAll()
        {
            LinkedListNode<FloatTextInstance> node = collection.First;
            while (node != null)
            {
                var next = node.Next;
                if (!Update(node.Value))
                    collection.Remove(node);
                node = next;
            }
        }
        /// <summary>
        /// Draw all floating texts.
        /// </summary>
        public void DrawAll()
        {
            foreach (var item in collection)
            {
                Draw(item);
            }
        }
        /// <summary>
        /// Adds new floating text. Destruction is automatic
        /// </summary>
        /// <param name="msg">Message to be shown.</param>
        /// <param name="pos">Position of center of the message.</param>
        /// <param name="c">Message colour.</param>
        public void Add(string msg, Vector2 pos, Color c)
        {
            FloatTextInstance ft = new FloatTextInstance();
            ft.message = msg;
            ft.position = pos;
            ft.color = c;
            ft.livingTime = 0.0f;
            ft.origin = font.MeasureString(ft.message) / 2;

            collection.AddFirst(ft);
        }

        /// <returns>False if dead, true if alive</returns>
        private bool Update(FloatTextInstance inst)
        {
            if (inst.livingTime++ >= TIME_TO_LIVE)
                return false;
            inst.position += velocity;
            inst.color.A -= (byte)(255 / TIME_TO_LIVE);

            return true;
        }

        private void Draw(FloatTextInstance inst)
        {
            spriteBatch.DrawString(font, inst.message, inst.position, inst.color, 0.0f, inst.origin, 1f, SpriteEffects.None, 0);
        }
    }
}

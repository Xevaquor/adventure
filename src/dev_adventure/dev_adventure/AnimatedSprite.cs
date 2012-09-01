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

namespace DevAdventure
{
    /// <summary>
    /// Animated sprite - generates part of image wchich contains frame animation.
    /// </summary>
    class AnimatedSprite
    {
        private Dictionary<string, int> animations = new Dictionary<string, int>();
        private int currentAnim;
        private int currentFrame;
        private int frames;

        private readonly int framesInAnim;
        private readonly int animTime;

        Texture2D img;
        private Vector2 frameSize = Vector2.Zero;
        private Rectangle clipRectangle = new Rectangle();

        /// <summary>
        /// Image for SpriteBatch
        /// </summary>
        public Texture2D Sprite { get { return img; } }
        /// <summary>
        /// Clip area for SpriteBatch
        /// </summary>
        public Rectangle Area { get { return clipRectangle; } }

        /// <summary>
        /// Creates animated sprite.
        /// </summary>
        /// <param name="tex">Source image</param>
        /// <param name="frames_in_anim">Frames in single animation</param>
        /// <param name="anim_time">Time for full animation pass in seconds</param>
        /// <param name="names">Names for each animation eg. walk, shoot, etc.</param>
        public AnimatedSprite(Texture2D tex, int frames_in_anim, int anim_time, params string[] names)
        {
            Debug.Assert(names.Length > 0);
            Debug.Assert(frames_in_anim > 0);

            img = tex;
            int rows = names.Length;
            int cols = frames_in_anim;

            for (int i = 0; i < rows; i++)
            {
                animations.Add(names[i], i);
            }
            frameSize.X = tex.Width / cols;
            frameSize.Y = tex.Height / rows;

            framesInAnim = anim_time * Settings.FramesPerSecond / frames_in_anim;
            animTime = frames_in_anim;

            clipRectangle = new Rectangle(
                (int)(currentFrame * frameSize.X),
                (int)(currentAnim * frameSize.Y),
                (int)frameSize.X,
                (int)frameSize.Y);
        }

        public AnimatedSprite(Texture2D tex)
            : this(tex, 1, 1, new string[] { "none" })
        { }
        /// <summary>
        /// Sets animation and resets frames counter
        /// </summary>
        /// <param name="name"></param>
        public void SetAnim(string name)
        {
            Debug.Assert(animations.ContainsKey(name));

            currentAnim = animations[name];
            frames = 0;
            currentFrame = 0;
        }
        /// <summary>
        /// Updates animation if required. Has to be called once a frame.
        /// </summary>
        public void Update()
        {
            if (frames++ >= framesInAnim)
            {
                currentFrame++;
                frames = 1;
            }

            if (currentFrame >= animTime)
            {
                currentFrame = 0;
            }

            clipRectangle = new Rectangle(
                    (int)(currentFrame * frameSize.X),
                    (int)(currentAnim * frameSize.Y),
                    (int)frameSize.X,
                    (int)frameSize.Y);
        }

    }
}

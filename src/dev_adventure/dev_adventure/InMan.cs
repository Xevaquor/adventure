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
    static class InMan
    {
        private static MouseState mouseState, prevMouseState;

        public static void Update()
        {
            prevMouseState = mouseState;
            mouseState = Mouse.GetState();
        }

        public static bool LeftDown
        { get { return mouseState.LeftButton == ButtonState.Pressed; } }

        public static bool LeftPressed
        { get { return LeftDown && (prevMouseState.LeftButton == ButtonState.Released); } }

        public static bool RightDown
        { get { return mouseState.RightButton == ButtonState.Pressed; } }

        public static bool RightPressed
        { get { return RightDown && (prevMouseState.RightButton == ButtonState.Released); } }

        public static Vector2 MousePosition
        {
            get
            {
                return new Vector2(mouseState.X, mouseState.Y - DevAdventure.Margin) * (1 / DevAdventure.Scale);
            }
        }
    }
}

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
    static class InMan
    {
        private static MouseState mouseState, prevMouseState;
        private static KeyboardState keyState, prevKeyState;

        public static void Update()
        {
            prevMouseState = mouseState;
            mouseState = Mouse.GetState();

            prevKeyState = keyState;
            keyState = Keyboard.GetState();
        }

        public static bool LeftDown
        { get { return mouseState.LeftButton == ButtonState.Pressed; } }

        public static bool LeftPressed
        { get { return LeftDown && (prevMouseState.LeftButton == ButtonState.Released); } }

        public static bool RightDown
        { get { return mouseState.RightButton == ButtonState.Pressed; } }

        public static bool RightPressed
        { get { return RightDown && (prevMouseState.RightButton == ButtonState.Released); } }

        public static bool KeyDown(Keys key)
        {
            return keyState.IsKeyDown(key);
        }
        public static bool KeyPressed(Keys key)
        {
            return keyState.IsKeyDown(key) && prevKeyState.IsKeyUp(key);
        }

        public static Vector2 MousePosition
        {
            get
            {
                return new Vector2(mouseState.X, mouseState.Y - DevAdventure.Margin) * (1 / DevAdventure.Scale);
            }
        }
    }
}

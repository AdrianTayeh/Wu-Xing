using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Wu_Xing
{
    class Mouse
    {
        private enum State { None, Pressed, Held, Released}
        private State leftState;
        private State rightState;

        private Point position;
        private float multiplier;
        private Point offset;

        private MouseState previousMouseState;
        private MouseState currentMouseState;

        public Mouse(Rectangle window, Rectangle resolution, float windowScale)
        {
            UpdateSettings(window, resolution, windowScale);
        }

        public Point Position { get { return position; } }

        public bool LeftIsNone { get { return leftState == State.None; } }
        public bool LeftIsPressed { get { return leftState == State.Pressed; } }
        public bool LeftIsHeld { get { return leftState == State.Held; } }
        public bool LeftIsReleased { get { return leftState == State.Released; } }

        public bool RightIsNone { get { return rightState == State.None; } }
        public bool RightIsPressed { get { return rightState == State.Pressed; } }
        public bool RightIsHeld { get { return rightState == State.Held; } }
        public bool RightIsReleased { get { return rightState == State.Released; } }

        public void UpdateSettings(Rectangle window, Rectangle resolution, float windowScale)
        {
            if (resolution == window)
            {
                multiplier = 1;
                offset = Point.Zero;
            }

            else if ((float)resolution.Height / resolution.Width >= (float)window.Height / window.Width)
            {
                multiplier = (float)window.Width / resolution.Width;
                offset.Y = (int)((resolution.Height - (window.Height * windowScale)) / 2);
            }

            else
            {
                multiplier = (float)window.Height / resolution.Height;
                offset.X = (int)((resolution.Width - (window.Width * windowScale)) / 2);
            }
        }

        public void Update()
        {
            previousMouseState = currentMouseState;
            currentMouseState = Microsoft.Xna.Framework.Input.Mouse.GetState();

            CalculatePosition();
            DetermineLeftState();
            DetermineRightState();
        }

        private void CalculatePosition()
        {
            position.X = (int)((currentMouseState.Position.X - offset.X) * multiplier);
            position.Y = (int)((currentMouseState.Position.Y - offset.Y) * multiplier);
        }

        private void DetermineLeftState()
        {
            if (currentMouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Released)
                leftState = State.Pressed;

            else if ((leftState == State.Pressed || leftState == State.Held) && currentMouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Pressed)
                leftState = State.Held;

            else if ((leftState == State.Pressed || leftState == State.Held) && currentMouseState.LeftButton == ButtonState.Released && previousMouseState.LeftButton == ButtonState.Pressed)
                leftState = State.Released;

            else
                leftState = State.None;
        }

        private void DetermineRightState()
        {
            if (currentMouseState.RightButton == ButtonState.Pressed && previousMouseState.RightButton == ButtonState.Released)
                rightState = State.Pressed;

            else if ((rightState == State.Pressed || rightState == State.Held) && currentMouseState.RightButton == ButtonState.Pressed && previousMouseState.RightButton == ButtonState.Pressed)
                rightState = State.Held;

            else if ((rightState == State.Pressed || rightState == State.Held) && currentMouseState.RightButton == ButtonState.Released && previousMouseState.RightButton == ButtonState.Pressed)
                rightState = State.Released;

            else
                rightState = State.None;
        }
    }
}
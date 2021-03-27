using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace iOSgame
{
    class Button2
    {
        public enum State { None, Hover, Pressed, Released }
        private State state;

        private Vector2 labelOrigin;
        private bool active;

        private Rectangle rectangle;
        private string label;
        private SpriteFont font;
        private Texture2D background;
        private Texture2D icon;
        private Dictionary<State, Color> backgroundColor;
        private Dictionary<State, Color> labelColor;

        /// <summary>
        /// Assign the button's top left position with width and height.
        /// </summary>

        public Button2(Rectangle rectangle, string label, SpriteFont font, Texture2D background, Texture2D icon, Dictionary<State, Color> backgroundColor, Dictionary<State, Color> labelColor)
        {
            this.rectangle = rectangle;

            CommonConstructor(label, font, background, icon, backgroundColor, labelColor);
        }

        /// <summary>
        /// Assign the button's center point with width and height.
        /// </summary>

        public Button2(Point center, Point size, string label, SpriteFont font, Texture2D background, Texture2D icon, Dictionary<State, Color> backgroundColor, Dictionary<State, Color> labelColor)
        {
            rectangle.X = center.X - size.X / 2;
            rectangle.Y = center.Y - size.Y / 2;
            rectangle.Width = size.X;
            rectangle.Height = size.Y;

            CommonConstructor(label, font, background, icon, backgroundColor, labelColor);
        }

        private void CommonConstructor(string label, SpriteFont font, Texture2D background, Texture2D icon, Dictionary<State, Color> backgroundColor, Dictionary<State, Color> labelColor)
        {
            this.label = label;
            this.font = font;
            this.background = background;
            this.icon = icon;
            this.backgroundColor = backgroundColor;
            this.labelColor = labelColor;
            active = true;

            if (font != null)
                labelOrigin = font.MeasureString(label) / 2;
            /*
            backgroundColor = new Dictionary<State, Color> {
                { State.None, Color.White },
                { State.Hover, Color.FromNonPremultiplied(185, 215, 255, 255) },
                { State.Pressed, Color.FromNonPremultiplied(125, 160, 210, 255) },
                { State.Released, Color.White } };

            labelColor = new Dictionary<State, Color> {
                { State.None, Color.Black },
                { State.Hover, Color.Black },
                { State.Pressed, Color.Black },
                { State.Released, Color.Black } };*/
        }

        public bool IsReleased { get { return state == State.Released; } }
        public bool Active { get { return active; } set { active = value; } }

        public Rectangle Rectangle { get { return rectangle; } }
        public Point Location { get { return rectangle.Location; } set { rectangle.Location = value; } }
        public string Label { get { return label; } set { label = value; } }

        public void UpdateLabelOrigin()
        {
            labelOrigin = font.MeasureString(label) / 2;
        }

        public void Update(MouseState currentMouseState, MouseState previousMouseState)
        {
            if (!active)
                return;

            if (rectangle.Contains(currentMouseState.Position))
            {
                if (currentMouseState.LeftButton == ButtonState.Released && previousMouseState.LeftButton == ButtonState.Released)
                    state = State.Hover;

                if (currentMouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Released)
                    state = State.Pressed;

                else if (state == State.Pressed && currentMouseState.LeftButton == ButtonState.Released && previousMouseState.LeftButton == ButtonState.Pressed)
                    state = State.Released;

                else
                    state = State.None;
            }

            else
            {
                state = State.None;
            }
        }

        public void Draw(SpriteBatch spriteBatch, Theme theme)
        {
            if (!active)
                return;

            if (background != null)
                spriteBatch.Draw(background, rectangle, backgroundColor[state]);

            if (icon != null)
                spriteBatch.Draw(icon, rectangle.Center.ToVector2(), null, Color.White, 0, icon.Bounds.Center.ToVector2(), 1, SpriteEffects.None, 0);

            if (label != "" && labelColor != null)
                spriteBatch.DrawString(font, label, rectangle.Center.ToVector2(), labelColor[state], 0, labelOrigin, 1, SpriteEffects.None, 0);
        }
    }
}

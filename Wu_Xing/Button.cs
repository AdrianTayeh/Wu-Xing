using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wu_Xing
{
    class Button
    {
        public enum State { None, Hover, Pressed, Held, Released }
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

        public Button(Rectangle rectangle, string label, SpriteFont font, Texture2D background, Texture2D icon, Dictionary<State, Color> backgroundColor, Dictionary<State, Color> labelColor)
        {
            this.rectangle = rectangle;

            CommonConstructor(label, font, background, icon, backgroundColor, labelColor);
        }

        /// <summary>
        /// Assign the button's center point with width and height.
        /// </summary>

        public Button(Point center, Point size, string label, SpriteFont font, Texture2D background, Texture2D icon, Dictionary<State, Color> backgroundColor, Dictionary<State, Color> labelColor)
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
        }

        public bool IsReleased { get { return state == State.Released; } }
        public bool IsHoveredOn { get { return state != State.None; } }
        public bool Active { get { return active; } set { active = value; } }

        public Rectangle Rectangle { get { return rectangle; } }
        public Point Location { get { return rectangle.Location; } }
        public string Label { get { return label; } set { label = value; } }
        public Dictionary<State, Color> BackgroundColor { get { return backgroundColor; } set { backgroundColor = value; } }
        public Texture2D Background { get { return background; } set { background = value; } }

        public void UpdateLabelOrigin()
        {
            labelOrigin = font.MeasureString(label) / 2;
        }

        public void Update(Mouse mouse)
        {
            if (!active)
                return;

            if (rectangle.Contains(mouse.Position))
            {
                if (mouse.LeftIsPressed)
                    state = State.Pressed;

                else if ((state == State.Pressed || state == State.Held) && mouse.LeftIsHeld)
                    state = State.Held;

                else if ((state == State.Pressed || state == State.Held) && mouse.LeftIsReleased)
                    state = State.Released;

                else
                    state = State.Hover;
            }

            else
            {
                state = State.None;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!active)
                return;

            if (background != null)
                spriteBatch.Draw(background, rectangle, backgroundColor[state == State.Pressed || state == State.Released ? State.Held : state]);

            if (icon != null)
                spriteBatch.Draw(icon, rectangle.Center.ToVector2(), null, Color.White, 0, icon.Bounds.Center.ToVector2(), 1, SpriteEffects.None, 0);

            if (label != "" && labelColor != null)
                spriteBatch.DrawString(font, label, rectangle.Center.ToVector2(), labelColor[state == State.Pressed || state == State.Released ? State.Held : state], 0, labelOrigin, 1, SpriteEffects.None, 0);
        }
    }
}

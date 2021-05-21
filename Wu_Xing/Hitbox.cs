using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Wu_Xing
{
    class Hitbox
    {
        private Rectangle rectangle;
        private Rectangle oldRectangle;
        private bool colliding;

        public enum HitboxType { Flat, OnGround, Flying }
        private HitboxType type;

        /// <summary> Initialize a Hitbox by prodiving its center position and size.</summary>
        public Hitbox(HitboxType type, bool colliding, Vector2 position, Point size)
        {
            this.type = type;
            this.colliding = colliding;

            //Adds 0.5 to position to round up if above .5
            rectangle.X = (int)(position.X + 0.5 - size.X / 2);
            rectangle.Y = (int)(position.Y + 0.5 - size.Y / 2);
            rectangle.Size = size;

            oldRectangle = rectangle;
        }

        /// <summary> Initialize a Hitbox by prodiving a Rectangle.</summary>
        public Hitbox(HitboxType type, bool colliding, Rectangle rectangle)
        {
            this.type = type;
            this.colliding = colliding;
            this.rectangle = rectangle;
            oldRectangle = rectangle;
        }

        #region Properties

        /// <summary>Whether or not the hitbox can collide with other hitboxes.</summary>
        public bool Colliding { get { return colliding; } set { colliding = value; } }
        public HitboxType Type { get { return type; } }

        //Used to access the whole rectangle
        public Rectangle Rectangle { get { return rectangle; } }

        //Used to shorten Hitbox properties
        public int Width { get { return rectangle.Width; } }
        public int Height { get { return rectangle.Height; } }
        public Vector2 Center { get { return rectangle.Center.ToVector2(); } }

        //Gets the sides or moves the entire hitbox by setting one of the sides
        public int Top { get { return rectangle.Top; } private set { rectangle.Y = value; } }
        public int Bottom { get { return rectangle.Bottom; } private set { rectangle.Y = value - rectangle.Height; } }
        public int Left { get { return rectangle.Left; } private set { rectangle.X = value; } }
        public int Right { get { return rectangle.Right; } private set { rectangle.X = value - rectangle.Width; } }

        //Gets the old sides
        public int OldTop { get { return oldRectangle.Top; } }
        public int OldBottom { get { return oldRectangle.Bottom; } }
        public int OldLeft { get { return oldRectangle.Left; } }
        public int OldRight { get { return oldRectangle.Right; } }

        #endregion

        /// <summary>Gets whether or not the other Hitbox intersects with this Hitbox.</summary>
        public bool Intersects(Hitbox obstacle)
        {
            return rectangle.Intersects(obstacle.Rectangle);
        }

        /// <summary>Gets whether or not the provided Vector2 lies within the bounds of this Hitbox.</summary>
        public bool Contains(Vector2 position)
        {
            return rectangle.Contains(position);
        }

        /// <summary>Moves the Hitbox witout checking for collisions.</summary>
        public void Move(Vector2 newPosition)
        {
            oldRectangle.Location = rectangle.Location;

            //Adds 0.5 to position to round up if above .5
            rectangle.X = (int)(newPosition.X + 0.5 - rectangle.Width / 2);
            rectangle.Y = (int)(newPosition.Y + 0.5 - rectangle.Width / 2);
        }

        /// <summary>Checks for a collision with another hitbox, modifies its location, before returning its final center position.</summary>
        public bool MoveOutOfCollision(Hitbox obstacle)
        {
            //If any of these conditions is true, there is no collision, return false
            if (Bottom < obstacle.Top || Top > obstacle.Bottom || Left > obstacle.Right || Right < obstacle.Left)
                return false;

            //Check collision with the obstacle's top side
            if (Bottom >= obstacle.Top && OldBottom < obstacle.OldTop)
                Bottom = obstacle.Top - 1;

            //Check collision with the obstacle's bottom side
            else if (Top <= obstacle.Bottom && OldTop > obstacle.OldBottom)
                Top = obstacle.Bottom + 1;

            //Check collision with the obstacle's left side
            else if (Right >= obstacle.Left && OldRight < obstacle.OldLeft)
                Right = obstacle.Left - 1;

            //Check collision with the obstacle's right side
            else if (Left <= obstacle.Right && OldLeft > obstacle.OldRight)
                Left = obstacle.Right + 1;

            //A collision was found and this Hitbox was moved, return true
            return true;
        }

        public void Draw(SpriteBatch spriteBatch, float layerDepth)
        {
            spriteBatch.Draw(TextureLibrary.Hitbox, rectangle, null, colliding ? Color.White : Color.Orange, 0, Vector2.Zero, SpriteEffects.None, layerDepth + 0.001f);
        }

    }
}

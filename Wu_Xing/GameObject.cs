﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wu_Xing
{
    abstract class GameObject
    {
        // Initialized in constructor
        protected Element? element;
        protected Color color;

        // Will be initialized in subclass constructor
        protected Vector2 position;
        protected Texture2D texture;
        protected Rectangle source;
        protected Rectangle hitbox;
        protected Vector2 origin;
        protected float layerDepth;

        // May be initialized in subclass constructor
        protected float animationFPS;
        protected float rotation;

        // Not initialized
        protected float animationTimer;
        protected bool dead;

        public GameObject(Vector2 position, Element? element, Random random)
        {
            this.element = element;
            color = Color.White;

            //No use of calling MoveTo, since hitboxes havn't been initialized at this point
        }

        public Texture2D Texture { get { return texture; } }
        public Vector2 Position { get { return position; } }
        public Rectangle Hitbox { get { return hitbox; } }
        public Rectangle Source { get { return source; } }
        public Element? Element { get { return element; } }
        public bool IsDead { get { return dead; } }

        public virtual void Update(float elapsedSeconds, List<GameObject> gameObjects, Adam adam, KeyboardState currentKeyboard, MapManager mapManager, Random random)
        {
            if (animationFPS != 0)
                UpdateAnimation(elapsedSeconds);
        }

        public void MoveTo(Vector2 newPosition)
        {
            position = newPosition;
            hitbox.Location = (position - hitbox.Size.ToVector2() / 2).ToPoint();
        }

        protected void UpdateAnimation(float elapsedSeconds)
        {
            animationTimer += elapsedSeconds;
            if (animationTimer >= 1 / animationFPS)
            {
                animationTimer -= 1 / animationFPS;
                source.X += source.Width;

                if (source.X >= texture.Bounds.Width)
                {
                    source.X = 0;
                    source.Y += source.Height;

                    if (source.Y >= texture.Bounds.Height)
                        source.Y = 0;
                }
            }
        }

        public void RandomSourceLocation(Random random)
        {
            //Using the size of an objects texture and source,
            //a new source location can be randomized, assuming the texture consists of a grid of same sized sources
            //Does not fail even if the texture only holds one source
            source.Location = new Point(random.Next(texture.Width / source.Width) * source.Width, random.Next(texture.Height / source.Height) * source.Height);
        }

        public void RandomRotation(Random random, int steps)
        {
            rotation = (float)(random.Next(steps) * Math.PI * 2 / steps);
        }

        public virtual void Draw(SpriteBatch spriteBatch, Vector2 roomPosition, bool drawHitbox)
        {
            spriteBatch.Draw(texture, roomPosition + position, source, color, rotation, origin, 1, SpriteEffects.None, layerDepth);

            if (drawHitbox)
                spriteBatch.Draw(TextureLibrary.Hitbox, hitbox, null, Color.White, 0, Vector2.Zero, SpriteEffects.None, layerDepth + 0.001f);
        }

    }
}

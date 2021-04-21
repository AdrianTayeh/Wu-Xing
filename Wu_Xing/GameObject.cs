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
    abstract class GameObject
    {
        // Initialized in constructor head
        protected Vector2 position;
        protected int size;
        protected Rectangle source;
        protected float layerDepth;

        // Initialized in constructor body
        protected Rectangle hitbox;
        protected Vector2 origin;
        protected Color color;

        // To Be initialized in subclass constructor body
        protected Texture2D texture;

        // Not initialized
        protected SpriteEffects spriteEffect;
        protected float rotation;
        protected bool toBeRemoved;
        protected Vector2 centerPosition;

        public GameObject(Vector2 position, int size, Rectangle source, float layerDepth)
        {
            this.position = position;
            this.size = size;
            this.source = source;
            this.layerDepth = layerDepth;

            hitbox.X = (int)position.X;
            hitbox.Y = (int)position.Y;
            hitbox.Width = size;
            hitbox.Height = size;

            origin.X = size / 2;
            origin.Y = size / 2;

            color = Color.White;
        }

        public virtual void Update(GameTime gameTime, Rectangle window, List<GameObject> gameObjectList, KeyboardState currentKeyboardState)
        {
            CalculateCenterPosition();
        }

        private void CalculateCenterPosition()
        {
            centerPosition.X = hitbox.X + size;
            centerPosition.Y = hitbox.Y + size;
        }

        public Texture2D Texture { get { return texture; } }

        public Vector2 CenterPosition { get { return centerPosition; } }

        public Vector2 Position { get { return position; } }

        public Rectangle Hitbox { get { return hitbox; } }

        public Rectangle Source { get { return source; } }

        public int Size { get { return size; } }

        public bool ToBeRemoved { get { return toBeRemoved; } set { toBeRemoved = value; } }

        public virtual void Draw(SpriteBatch spriteBatch, SpriteFont spriteFont)
        {
            spriteBatch.Draw(texture, new Rectangle((int)position.X + (int)origin.X, (int)position.Y + (int)origin.Y, size, size), source, color, rotation, origin, spriteEffect, layerDepth);
        }

    }
}

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
    abstract class GameObject : ICloneable
    {
        // Initialized in constructor head
        protected Vector2 position;
        protected Rectangle source;

        // To be initialized in subclass constructor
        protected Texture2D texture;
        protected Rectangle hitbox;
        protected Vector2 origin;
        protected float layerDepth;
        protected Element? element;

        public GameObject(Vector2 position, Element? element)
        {
            MoveTo(position);
            this.element = element;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public Texture2D Texture { get { return texture; } }
        public Vector2 Position { get { return position; } }
        public Rectangle Hitbox { get { return hitbox; } }
        public Rectangle Source { get { return source; } }
        public Element? Element { get { return element; } }

        public virtual void Update(float elapsedSeconds, List<GameObject> gameObjects, Adam adam, KeyboardState currentKeyboard, MapManager mapManager)
        {
            
        }

        public void MoveTo(Vector2 newPosition)
        {
            position = newPosition;
            hitbox.Location = (position - hitbox.Size.ToVector2() / 2).ToPoint();
        }

        //Used by Tile, overidden by Character
        public virtual void Draw(SpriteBatch spriteBatch, Vector2 roomPosition, bool drawHitbox)
        {
            spriteBatch.Draw(texture, roomPosition + position, source, Color.White, 0, origin, 1, SpriteEffects.None, layerDepth);

            if (drawHitbox)
                DrawHitbox(spriteBatch);
        }

        public void DrawHitbox(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(TextureLibrary.Hitbox, hitbox, null, Color.White, 0, Vector2.Zero, SpriteEffects.None, layerDepth + 0.001f);
        }

    }
}

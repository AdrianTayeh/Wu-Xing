using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Wu_Xing
{
    abstract class Character : GameObject
    {
        // Initialized in constructor
        protected static float rotationSpeed;

        // Will be initialized in subclass constructor
        protected int maxHealth;
        protected int health;
        protected float movingSpeed;
        protected int damage;
        protected int range;
        protected int shadowSize;

        // Not initialized
        protected float rotationTarget;
        protected Vector2 movingDirection;
        protected Vector2 aimingDirection;

        public Character(Vector2 position, Element? element, Random random) : base(position, element, random)
        {
            //GameObject
            layerDepth = 0.5f;

            //Character
            rotationSpeed = 0.3f;
        }

        public override void Update(float elapsedSeconds, List<GameObject> gameObjects, Adam adam, KeyboardState currentKeyboard, MapManager mapManager, Random random)
        {
            MoveTo(position + (movingDirection * 600 * elapsedSeconds * movingSpeed));

            base.Update(elapsedSeconds, gameObjects, adam, currentKeyboard, mapManager, random);
        }

        public void Heal(int heal)
        {
            health += heal;
            health = health > maxHealth ? maxHealth : health;
        }

        public void TakeDamage(int damage)
        {
            health -= damage;
            if (health <= 0)
                Kill();
        }

        public void Kill()
        {
            health = 0;
            dead = true;
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 roomPosition, bool drawHitbox)
        {
            if (shadowSize != 0)
                spriteBatch.Draw(TextureLibrary.Shadow, roomPosition + position, null, Color.White, 0, TextureLibrary.Shadow.Bounds.Size.ToVector2() / 2, (float)shadowSize / TextureLibrary.Shadow.Bounds.Width, SpriteEffects.None, 0.4f);
            base.Draw(spriteBatch, roomPosition, drawHitbox);
        }
    }
}

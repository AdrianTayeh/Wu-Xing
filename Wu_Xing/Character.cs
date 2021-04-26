using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Wu_Xing
{
    abstract class Character : GameObject
    {
        // Initialized in constructor body
        protected Color color;
        protected static float rotationSpeed;

        // To be initialized in subclass constructor
        protected int maxHealth;
        protected int health;
        protected float movingSpeed;

        // Not initialized in constructor
        protected float rotation;
        protected float rotationTarget;
        protected bool dead;
        protected Vector2 movingDirection;
        protected Vector2 aimingDirection;

        public Character(Vector2 position, Element? element, Random random) : base(position, element, random)
        {
            //GameObject
            layerDepth = 0.5f;

            //Character
            color = Color.White;
            rotationSpeed = 0.3f;
        }

        public bool IsDead { get { return dead; } }

        public override void Update(float elapsedSeconds, List<GameObject> gameObjects, Adam adam, KeyboardState currentKeyboard, MapManager mapManager)
        {
            MoveTo(position + (movingDirection * 600 * elapsedSeconds * movingSpeed));

            base.Update(elapsedSeconds, gameObjects, adam, currentKeyboard, mapManager);
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
            spriteBatch.Draw(texture, roomPosition + position, source, color, rotation, origin, 1, SpriteEffects.None, layerDepth);

            if (drawHitbox)
                DrawHitbox(spriteBatch);
        }
    }
}

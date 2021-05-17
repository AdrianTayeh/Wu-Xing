using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Wu_Xing
{
    abstract class Character : GameObject
    {
        // Initialized in fields
        private static float maximumOffsetAngle = (float)Math.PI / 8; //22.5 degrees

        // Will be initialized in subclass constructor
        protected int maxHealth;
        protected float health;
        protected float speed;
        protected int shadowSize;

        // May be initialized in subclass constructor
        protected float rotationSpeed;
        protected ProjectileAttributes projectileAttributes;
        protected float shotsPerSecond;
        protected float shotCooldown;
        protected float accuracy; //1 = perfect aim

        // Not initialized
        protected float rotationTarget;
        protected Vector2 movingDirection;
        protected float flashTimer;

        public Character(Vector2 position, Element? element, Random random) : base(position, element, random)
        {
            //GameObject
            layerDepth = 0.5f;
        }

        public override void Update(float elapsedSeconds, List<GameObject> gameObjects, Adam adam, KeyboardState currentKeyboard, MapManager mapManager, Random random)
        {
            RotateTowardTarget();
            base.Update(elapsedSeconds, gameObjects, adam, currentKeyboard, mapManager, random);
        }

        protected float AccuracyOffset(Random random)
        {
            return (float)((random.NextDouble() - 0.5) * 2 * maximumOffsetAngle * Math.Abs(accuracy - 1));
        }

        public void Heal(float heal)
        {
            health += heal;
            health = health > maxHealth ? maxHealth : health;
        }

        public virtual void TakeDamage(float damage)
        {
            health -= damage;

            if (health <= 0)
                Kill();

            else
                Flash();
        }

        public async void TakeKnockback(Vector2 direction, float knockback)
        {
            float duration = 0.05f;
            int fps = 60;
            int frames = (int)(duration * fps);

            for (int i = 0; i < frames; i++)
            {
                MoveTo(position + direction * knockback * 4);
                await Task.Delay(1000 / fps);
            }
        }

        private async void Flash()
        {
            float flashDurantion = 0.06f;

            //If already flashing, reset timer
            if (flashTimer > 0)
            {
                flashTimer = flashDurantion;
                return;
            }

            //If not already flashing, reset and start decreasing timer
            else
            {
                flashTimer = flashDurantion;
                Color original = color;
                color = Color.FromNonPremultiplied(255, 100, 100, 200);

                //Decrease timer by 1/20, untill only 1/20 is left
                while (flashTimer > flashDurantion / 20)
                {
                    await Task.Delay((int)(flashDurantion / 20 * 1000));
                    flashTimer -= flashDurantion / 20;
                }

                //Restore original color before ending timer
                await Task.Delay((int)(flashDurantion / 20 * 1000));
                color = original;
                flashTimer = 0;
            } 
        }

        private void RotateTowardTarget()
        {
            if (rotation == rotationTarget)
                return;

            //Find closest rotation path
            float larger = rotationTarget > rotation ? rotationTarget : rotation;
            float smaller = rotationTarget > rotation ? rotation : rotationTarget;

            float distanceWithoutCrossingZero = larger - smaller;
            float distanceCrossingZero = smaller + ((float)Math.PI * 2f - larger);

            //Rotate
            if (distanceWithoutCrossingZero < distanceCrossingZero)
                rotation += rotation == smaller ? rotationSpeed : -rotationSpeed;

            else
                rotation += rotation == smaller ? -rotationSpeed : rotationSpeed;

            rotation %= (float)Math.PI * 2;

            //Check if rotation is complete
            float larger2 = rotationTarget > rotation ? rotationTarget : rotation;
            float smaller2 = rotationTarget > rotation ? rotation : rotationTarget;

            float distanceWithoutCrossingZero2 = larger - smaller;
            float distanceCrossingZero2 = smaller + ((float)Math.PI * 2f - larger);

            if (distanceWithoutCrossingZero2 <= rotationSpeed || distanceCrossingZero2 <= rotationSpeed)
                rotation = rotationTarget;
        }


        private void Kill()
        {
            health = 0;
            dead = true;
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 roomPosition, bool drawHitbox)
        {
            if (shadowSize != 0)
                spriteBatch.Draw(TextureLibrary.Shadow, roomPosition + position, null, Color.White, 0, TextureLibrary.Shadow.Bounds.Size.ToVector2() / 2, (float)shadowSize / TextureLibrary.Shadow.Bounds.Width, SpriteEffects.None, 0.3f);
            base.Draw(spriteBatch, roomPosition, drawHitbox);
        }
    }
}

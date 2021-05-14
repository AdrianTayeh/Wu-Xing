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
        // Initialized in constructor
        protected static float rotationSpeed;

        // Will be initialized in subclass constructor
        protected int maxHealth;
        protected float health;
        protected float speed;
        protected int shadowSize;

        // May be initialized in subclass constructor
        protected ProjectileAttributes projectileAttributes;
        protected float shotsPerSecond;
        protected float shotCooldown;
        protected float accuracy; //1 = perfect aim

        // Not initialized
        protected float rotationTarget;
        protected Vector2 movingDirection;
        protected Vector2 aimingDirection;
        protected float flashTimer;

        public Character(Vector2 position, Element? element, Random random) : base(position, element, random)
        {
            //GameObject
            layerDepth = 0.5f;

            //Character
            rotationSpeed = 0.3f;
        }

        public override void Update(float elapsedSeconds, List<GameObject> gameObjects, Adam adam, KeyboardState currentKeyboard, MapManager mapManager, Random random)
        {
            MoveTo(position + (movingDirection * 600 * elapsedSeconds * speed));
            Shoot(elapsedSeconds, gameObjects, random);

            base.Update(elapsedSeconds, gameObjects, adam, currentKeyboard, mapManager, random);
        }

        private void Shoot(float elapsedSeconds, List<GameObject> gameObjects, Random random)
        {
            //If character can shoot
            if (shotsPerSecond > 0)
            {
                //Decrease cooldown
                if (shotCooldown > 0)
                    shotCooldown -= elapsedSeconds;

                //If ready to shoot, is aiming, and is facing the right way
                if (shotCooldown <= 0 && aimingDirection != Vector2.Zero && rotation == rotationTarget)
                {
                    //Reset cooldown
                    shotCooldown += 1 / shotsPerSecond;

                    //Create projectile
                    CreateProjectile(gameObjects, random);
                }
            }
        }

        private void CreateProjectile(List<GameObject> gameObjects, Random random)
        {
            Vector2 position = this.position + Rotate.PointAroundZero(Vector2.UnitY, rotation) * hitbox.Width * 0.7f;

            ProjectileAttributes attributes = new ProjectileAttributes(projectileAttributes);
            attributes.Speed *= movingDirection == aimingDirection ? 1.25f : 1;
            attributes.Speed *= movingDirection == -aimingDirection ? 0.75f : 1;

            float maximumOffsetAngle = (float)Math.PI / 8; //22.5 degrees
            float accuracyOffset = (float)((random.NextDouble() - 0.5) * 2 * maximumOffsetAngle * Math.Abs(accuracy - 1));

            //For demonstrative purpose only
            Element element = (Element)this.element;
            if (this is Adam && ((Adam)this).RandomElement)
            {
                Array elements = Enum.GetValues(typeof(Element));
                element = (Element)elements.GetValue(random.Next(elements.Length));
            }

            gameObjects.Add(new Projectile(position, element, random, Projectile.Type.MagicBall, attributes, rotation + accuracyOffset, this is Adam));
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

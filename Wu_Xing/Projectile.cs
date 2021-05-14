using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Wu_Xing
{
    class ProjectileAttributes
    {
        public float Scale;
        public float Damage;
        public float Range;
        public float Speed;
        public float Knockback { get; private set; }

        /// <summary>Create a set of projectile attributes with custom scale.</summary>
        /// <param name="range">How far the projectile will travel, measured in tiles.</param>
        /// <param name="speed">How fast the projectile will travel, 1 being 100%.</param>
        public ProjectileAttributes(float damage, float range, float speed, float scale)
        {
            Scale = scale;
            CommonConstructor(damage, range, speed);
        }

        /// <summary>Create a set of projectile attributes with scale proportionate to damage.</summary>
        /// <param name="range">How far the projectile will travel, measured in tiles.</param>
        /// <param name="speed">How fast the projectile will travel, 1 being 100%.</param>
        public ProjectileAttributes(float damage, float range, float speed)
        {
            //Scale = 1 at 10 damage
            Scale = damage * 0.1f;
            CommonConstructor(damage, range, speed);
        }

        /// <summary>Create a set of projectile attributes using an existing set.</summary>
        public ProjectileAttributes(ProjectileAttributes projectileAttributes)
        {
            Scale = projectileAttributes.Scale;
            CommonConstructor(projectileAttributes.Damage, projectileAttributes.Range, projectileAttributes.Speed);
        }

        private void CommonConstructor(float damage, float range, float speed)
        {
            Damage = damage;
            Range = range;
            Speed = speed;
            Knockback = Speed * Scale;
        }
    }

    class Projectile : GameObject
    {
        private ProjectileAttributes attributes;
        private float tilesTraveled;
        private Vector2 direction;
        public enum Type { MagicBall }
        private bool shotByAdam;

        public Projectile(Vector2 position, Element? element, Random random, Type type, ProjectileAttributes attributes, float rotation, bool shotByAdam) : base(position, element, random)
        {
            //Projectile
            this.shotByAdam = shotByAdam;
            this.attributes = attributes;
            this.rotation = rotation;
            direction = Rotate.PointAroundZero(Vector2.UnitY, rotation);

            //GameObject
            if (type == Type.MagicBall)
            {
                texture = TextureLibrary.MagicBall;
                animationFPS = 60;
                color = ColorLibrary.Element[(Element)element];
                hitbox.Size = new Point((int)(40 * attributes.Scale));
                source.Size = new Point(200, 320);
                origin = new Vector2(source.Width / 2, source.Height * 0.6f);
            }

            layerDepth = 0.4f;
            RandomSourceLocation(random);
            MoveTo(position);
        }

        public override void Update(float elapsedSeconds, List<GameObject> gameObjects, Adam adam, KeyboardState currentKeyboard, MapManager mapManager, Random random)
        {
            Vector2 previousPosition = position;
            MoveTo(position + (direction * 600 * elapsedSeconds * attributes.Speed));
            tilesTraveled += Vector2.Distance(previousPosition, position) / 100;
            dead = tilesTraveled >= attributes.Range ? true : false;
            FadeOut();

            foreach (GameObject gameObject in gameObjects)
            {
                if (((gameObject is Enemy && shotByAdam) || (gameObject is Adam && !shotByAdam)) && hitbox.Intersects(gameObject.Hitbox))
                {
                    ((Character)gameObject).TakeDamage(attributes.Damage);
                    dead = true;
                    break;
                }
            }
                
            base.Update(elapsedSeconds, gameObjects, adam, currentKeyboard, mapManager, random);
        }

        private void FadeOut()
        {
            //If the projectile has traveled at least 80% of its range
            float percentageTraveled = tilesTraveled / attributes.Range;
            if (percentageTraveled >= 0.8f)
            {
                //Calculate opacity and apply to alpha value. Alpha is 255 at 80% and 0 at 100%
                float opacity = Math.Abs(percentageTraveled - 1) * 5;
                color.A = (byte)(255 * opacity);
            }
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 roomPosition, bool drawHitbox)
        {
            spriteBatch.Draw(texture, roomPosition + position, source, Color.FromNonPremultiplied(color.R, color.G, color.B, color.A), rotation, origin, attributes.Scale, SpriteEffects.None, layerDepth);

            if (texture == TextureLibrary.MagicBall)
                spriteBatch.Draw(TextureLibrary.MagicBallCenter, roomPosition + position, source, Color.FromNonPremultiplied(255, 255, 255, color.A), rotation, origin, attributes.Scale, SpriteEffects.None, layerDepth + 0.001f);

            if (drawHitbox)
                spriteBatch.Draw(TextureLibrary.Hitbox, hitbox, null, Color.White, 0, Vector2.Zero, SpriteEffects.None, layerDepth + 0.001f);
        }
    }
}
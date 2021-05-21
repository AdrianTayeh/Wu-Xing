using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Wu_Xing
{
    class ProjectileAttributes
    {
        public int CriticalChance;
        public float Damage;
        public float Range;
        public float Speed;
        public float Scale { get; private set; }
        public float Knockback { get; private set; }

        /// <summary>Create a set of projectile attributes with custom scale.</summary>
        /// <param name="range">How far the projectile will travel, measured in tiles.</param>
        /// <param name="speed">How fast the projectile will travel, 1 being 100%.</param>
        public ProjectileAttributes(float damage, int criticalChance, float range, float speed, float scale)
        {
            Scale = scale;
            CommonConstructor(damage, criticalChance, range, speed);
        }

        /// <summary>Create a set of projectile attributes with scale proportionate to damage.</summary>
        /// <param name="range">How far the projectile will travel, measured in tiles.</param>
        /// <param name="speed">How fast the projectile will travel, 1 being 100%.</param>
        public ProjectileAttributes(float damage, int criticalChance, float range, float speed)
        {
            Scale = damage * 0.1f;
            CommonConstructor(damage, criticalChance, range, speed);
        }

        /// <summary>Create a set of projectile attributes using an existing set.</summary>
        public ProjectileAttributes(ProjectileAttributes projectileAttributes)
        {
            Scale = projectileAttributes.Scale;
            CommonConstructor(projectileAttributes.Damage, projectileAttributes.CriticalChance, projectileAttributes.Range, projectileAttributes.Speed);
        }

        private void CommonConstructor(float damage, int criticalChance, float range, float speed)
        {
            Damage = damage;
            CriticalChance = criticalChance;
            Range = range;
            Speed = speed;
            Knockback = Speed * Scale;
        }

        /// <summary>Should be called after modifying Damage or Speed.</summary>
        public void UpdateScaleAndKnockback()
        {
            Scale = Damage * 0.1f;
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
                hitbox = new Hitbox(Hitbox.HitboxType.Flying, true, position, new Point((int)(40 * attributes.Scale)));
                source.Size = new Point(200, 320);
                origin = new Vector2(source.Width / 2, source.Height * 0.6f);
            }

            layerDepth = 0.4f;
            RandomSourceLocation(random);
        }

        public override void Move(Vector2 newPosition, List<GameObject> gameObjects, List<Hitbox> roomHitboxes)
        {
            position = newPosition;
            hitbox.Move(newPosition);

            //Return if this is non-colliding
            if (!hitbox.Colliding)
                return;

            //Check all gameObjects and die upon intersection
            for (int i = gameObjects.Count - 1; i >= 0; i--)
            {
                //Ignore if non-colliding
                if (!gameObjects[i].Hitbox.Colliding)
                    continue;

                //Ignore if not Tile
                if (!(gameObjects[i] is Tile))
                    continue;

                //Ignore if HitboxType.Flat
                if (gameObjects[i].Hitbox.Type == Hitbox.HitboxType.Flat)
                    continue;

                //Check for intersection
                if (hitbox.Intersects(gameObjects[i].Hitbox))
                {
                    dead = true;
                    return;
                }
            }

            foreach (Hitbox roomHitbox in roomHitboxes)
            {
                //Check for intersection
                if (roomHitbox.Colliding && hitbox.Intersects(roomHitbox))
                {
                    dead = true;
                    return;
                }
            }
        }

        public override void Update(float elapsedSeconds, List<GameObject> gameObjects, Adam adam, KeyboardState currentKeyboard, MapManager mapManager, Random random)
        {
            Vector2 previousPosition = position;
            Move(position + (direction * 600 * elapsedSeconds * attributes.Speed), gameObjects, mapManager.CurrentRoom.Hitboxes);
            tilesTraveled += Vector2.Distance(previousPosition, position) / 100;

            //If traveled too far, or outside of the room's bounds, or already dead from intersection with tile
            if (tilesTraveled >= attributes.Range || !mapManager.CurrentRoom.Bounds.Contains(hitbox.Center) || dead)
            {
                dead = true;
                return;
            }

            FadeOut();
            HitGameObjects(gameObjects, adam, random, mapManager.CurrentRoom.Hitboxes);
            base.Update(elapsedSeconds, gameObjects, adam, currentKeyboard, mapManager, random);
        }

        private void HitGameObjects(List<GameObject> gameObjects, Adam adam, Random random, List<Hitbox> roomHitboxes)
        {
            if (shotByAdam)
            {
                foreach (GameObject gameObject in gameObjects)
                {
                    if (gameObject is Character && hitbox.Intersects(gameObject.Hitbox))
                    {
                        int criticalChance = 3;
                        int criticalDamageMultiplier = random.Next(100) < criticalChance ? 2 : 1;

                        ((Character)gameObject).TakeDamage(attributes.Damage * criticalDamageMultiplier);
                        ((Character)gameObject).TakeKnockback(direction, attributes.Knockback, gameObjects, roomHitboxes);
                        dead = true;
                        break;
                    }
                }
            }

            else
            {
                if (hitbox.Intersects(adam.Hitbox))
                {
                    adam.TakeDamage(attributes.Damage);
                    dead = true;
                }
            }
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
                hitbox.Draw(spriteBatch, layerDepth);
        }
    }
}
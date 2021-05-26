using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Wu_Xing
{
    class Adam : Character
    {
        private Rectangle aimingArmSource;
        private Rectangle restingArmSource;

        private Vector2 leftArmPosition;
        private Vector2 rightArmPosition;

        private float invulnerableTimer;
        private Vector2 aimingDirection;

        //For video
        private int shootCounter;

        //For demonstrative purpose only
        private bool randomElement;

        public Adam(Vector2 position, Element element, Random random) : base(position, element, random)
        {
            //GameObject
            texture = TextureLibrary.Adam;
            source = new Rectangle(140, 0, 140, 140);
            origin = source.Size.ToVector2() / 2;
            hitbox = new Hitbox(Hitbox.HitboxType.OnGround, true, position, new Point(80));

            //Character
            rotationSpeed = 0.3f;
            speed = 1;
            health = maxHealth = 6;
            shadowSize = 100;
            projectileAttributes = new ProjectileAttributes(5, 3, 7, 1.25f);
            shotsPerSecond = 1.4f;
            accuracy = 0.6f;

            //Adam
            aimingArmSource = new Rectangle(0, 140, 40, 70);
            restingArmSource = new Rectangle(40, 140, 40, 70);
            leftArmPosition = new Vector2(28, 16);
            rightArmPosition = new Vector2(-28, 16);
        }

        public override void Update(float elapsedSeconds, List<GameObject> gameObjects, Adam adam, KeyboardState currentKeyboard, MapManager mapManager, Random random)
        {
            CheckCheatInput(currentKeyboard);

            DetermineMovingDirection(currentKeyboard);
            DetermineAimingDirection(currentKeyboard);
            DetermineRotationTarget();

            if (invulnerableTimer > 0)
                invulnerableTimer -= elapsedSeconds;

            Move(position + (movingDirection * 600 * elapsedSeconds * speed), gameObjects, mapManager.CurrentRoom.Hitboxes);
            Shoot(elapsedSeconds, gameObjects, random, mapManager.CurrentRoom.Hitboxes);
            base.Update(elapsedSeconds, gameObjects, adam, currentKeyboard, mapManager, random);

            CheckDoors(mapManager, gameObjects);
        }

        private void CheckCheatInput(KeyboardState currentKeyboard)
        {
            //Base
            if (currentKeyboard.IsKeyDown(Keys.D1))
            {
                projectileAttributes = new ProjectileAttributes(5, 3, 7, 1.25f);
                shotsPerSecond = 1.4f;
                health = maxHealth = 6;
                speed = 1;
                accuracy = 0.6f;
                randomElement = false;
            }

            //Upgraded
            else if (currentKeyboard.IsKeyDown(Keys.D2))
            {
                projectileAttributes = new ProjectileAttributes(7, 3, 10, 1.5f);
                shotsPerSecond = 4;
                health = maxHealth = 8;
                speed = 1.2f;
                accuracy = 0.7f;
                randomElement = false;
            }

            //Slow
            else if (currentKeyboard.IsKeyDown(Keys.D3))
            {
                projectileAttributes = new ProjectileAttributes(15, 3, 7, 0.8f);
                shotsPerSecond = 1;
                health = maxHealth = 12;
                speed = 0.9f;
                accuracy = 0.8f;
                randomElement = false;
            }

            //Sniper
            else if (currentKeyboard.IsKeyDown(Keys.D4))
            {
                projectileAttributes = new ProjectileAttributes(10, 5, 12, 5f);
                shotsPerSecond = 2;
                health = maxHealth = 14;
                speed = 1f;
                accuracy = 1;
                randomElement = false;
            }

            //Laser
            else if (currentKeyboard.IsKeyDown(Keys.D5))
            {
                projectileAttributes = new ProjectileAttributes(15, 2, 14, 3f);
                shotsPerSecond = 60;
                health = maxHealth = 6;
                speed = 1.5f;
                accuracy = 1f;
                randomElement = false;
            }

            //Rainbow laser
            else if (currentKeyboard.IsKeyDown(Keys.D6))
            {
                projectileAttributes = new ProjectileAttributes(15, 2, 14, 3f);
                shotsPerSecond = 60;
                health = maxHealth = 6;
                speed = 1.5f;
                accuracy = 1f;
                randomElement = true;
            }

            //Small machinegun
            else if (currentKeyboard.IsKeyDown(Keys.D7))
            {
                projectileAttributes = new ProjectileAttributes(2, 5, 14, 1.75f);
                shotsPerSecond = 60;
                health = maxHealth = 4;
                speed = 1;
                accuracy = 0.3f;
                randomElement = false;
            }

            //Big machine gun
            else if (currentKeyboard.IsKeyDown(Keys.D8))
            {
                projectileAttributes = new ProjectileAttributes(8, 3, 14, 1.75f);
                shotsPerSecond = 60;
                health = maxHealth = 2;
                speed = 1;
                accuracy = 0f;
                randomElement = false;
            }

            //Big rainbow machine gun
            else if (currentKeyboard.IsKeyDown(Keys.D9))
            {
                projectileAttributes = new ProjectileAttributes(12, 2, 14, 1.5f);
                shotsPerSecond = 60;
                health = maxHealth = 2;
                speed = 1;
                accuracy = 0f;
                randomElement = true;
            }

            //Chaos
            else if (currentKeyboard.IsKeyDown(Keys.D0))
            {
                projectileAttributes = new ProjectileAttributes(30, 10, 20, 3f);
                shotsPerSecond = 60;
                health = maxHealth = 16;
                speed = 3;
                accuracy = -3f;
                randomElement = true;
            }
        }

        private void CheckDoors(MapManager mapManager, List<GameObject> gameObjects)
        {
            foreach (Door door in mapManager.Rooms[mapManager.CurrentRoomLocation.X, mapManager.CurrentRoomLocation.Y].Doors)
            {
                if (door.EntranceArea.Contains(position))
                {
                    position = door.TransitionExitPosition;
                    mapManager.StartRoomTransition(door, gameObjects);
                    break;
                }
            }
        }

        private void DetermineMovingDirection(KeyboardState currentKeyboard)
        {
            movingDirection = Vector2.Zero;

            if (currentKeyboard.IsKeyDown(Keys.W))
                movingDirection.Y -= 1;

            if (currentKeyboard.IsKeyDown(Keys.S))
                movingDirection.Y += 1;

            if (currentKeyboard.IsKeyDown(Keys.A))
                movingDirection.X -= 1;

            if (currentKeyboard.IsKeyDown(Keys.D))
                movingDirection.X += 1;

            if (movingDirection != Vector2.Zero)
                movingDirection.Normalize();
        }

        private void DetermineAimingDirection(KeyboardState currentKeyboard)
        {
            aimingDirection = Vector2.Zero;

            if (currentKeyboard.IsKeyDown(Keys.Up))
                aimingDirection.Y -= 1;

            if (currentKeyboard.IsKeyDown(Keys.Down))
                aimingDirection.Y += 1;

            if (currentKeyboard.IsKeyDown(Keys.Left))
                aimingDirection.X -= 1;

            if (currentKeyboard.IsKeyDown(Keys.Right))
                aimingDirection.X += 1;

            if (aimingDirection != Vector2.Zero)
                aimingDirection.Normalize();
        }

        private void DetermineRotationTarget()
        {
            if (aimingDirection != Vector2.Zero)
                rotationTarget = (float)Math.Atan2(-aimingDirection.X, aimingDirection.Y);

            else if (movingDirection != Vector2.Zero)
                rotationTarget = (float)Math.Atan2(-movingDirection.X, movingDirection.Y);

            rotationTarget %= (float)Math.PI * 2;
        }

        private void Shoot(float elapsedSeconds, List<GameObject> gameObjects, Random random, List<Hitbox> roomHitboxes)
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
                CreateProjectile(gameObjects, random, roomHitboxes);
            }
        }

        private void CreateProjectile(List<GameObject> gameObjects, Random random, List<Hitbox> roomHitboxes)
        {
            Vector2 position = this.position + Rotate.PointAroundZero(Vector2.UnitY, rotation) * hitbox.Width * 0.7f;

            ProjectileAttributes attributes = new ProjectileAttributes(projectileAttributes);
            attributes.Speed *= movingDirection == aimingDirection ? 1.25f : 1;
            attributes.Speed *= movingDirection == -aimingDirection ? 0.75f : 1;

            //For demonstrative purpose only
            Element element = (Element)this.element;
            if (randomElement)
            {
                Array elements = Enum.GetValues(typeof(Element));
                element = (Element)elements.GetValue(random.Next(elements.Length));
            }

            gameObjects.Add(new Projectile(position, element, random, Projectile.Type.MagicBall, attributes, rotation + AccuracyOffset(random), true));
            gameObjects[gameObjects.Count - 1].Move(gameObjects[gameObjects.Count - 1].Position + Rotate.PointAroundZero(Vector2.UnitY, rotation) * gameObjects[gameObjects.Count - 1].Hitbox.Width * 0.5f, gameObjects, roomHitboxes);

            float volume = projectileAttributes.Damage / 5f >= 1 ? 1 : projectileAttributes.Damage / 5f;

            if (shotsPerSecond >= 40)
            {
                shootCounter += 1;
                if (shootCounter == 6)
                {
                    shootCounter = 0;
                    SoundLibrary.FireAttack.Play(volume, 0, 0);
                }
            }

            else
            {
                SoundLibrary.FireAttack.Play(volume, 0, 0);
            }
        }

        public override void TakeDamage(float damage)
        {
            if (invulnerableTimer <= 0)
            {
                invulnerableTimer = 1.2f;
                base.TakeDamage(damage);
            }   
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 roomPosition, bool drawHitbox)
        {
            //Arms
            spriteBatch.Draw(texture, roomPosition + position + Rotate.PointAroundZero(leftArmPosition, rotation), aimingDirection == Vector2.Zero ? restingArmSource : aimingArmSource, color, rotation + (aimingDirection == Vector2.Zero ? -0.3f : 0.12f), new Vector2(aimingArmSource.Width / 2, aimingArmSource.Height / 4), 1, SpriteEffects.None, layerDepth - 0.001f);
            spriteBatch.Draw(texture, roomPosition + position + Rotate.PointAroundZero(rightArmPosition, rotation), aimingDirection == Vector2.Zero ? restingArmSource : aimingArmSource, color, rotation + (aimingDirection == Vector2.Zero ? 0.3f : -0.12f), new Vector2(aimingArmSource.Width / 2, aimingArmSource.Height / 4), 1, SpriteEffects.None, layerDepth - 0.001f);

            //Head
            base.Draw(spriteBatch, roomPosition, drawHitbox);
        }

        public void DrawHearts(SpriteBatch spriteBatch)
        {
            for (int i = 1; i <= maxHealth / 2; i++)
                spriteBatch.Draw(TextureLibrary.Heart, new Vector2(175 + i * 65, 65), new Rectangle(health >= i * 2 ? 0 : health == i * 2 - 1 ? 60 : 120, 0, 60, 60), Color.White);
        }
    }
}

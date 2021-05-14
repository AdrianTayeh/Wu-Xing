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

        //For demonstrative purpose only
        private bool randomElement;
        public bool RandomElement { get { return randomElement; } }

        public Adam(Vector2 position, Element element, Random random) : base(position, element, random)
        {
            //GameObject
            texture = TextureLibrary.Adam;
            source = new Rectangle(140, 0, 140, 140);
            origin = source.Size.ToVector2() / 2;
            hitbox.Size = new Point(86);
            MoveTo(position);

            //Character
            speed = 1;
            health = maxHealth = 6;
            shadowSize = 100;
            projectileAttributes = new ProjectileAttributes(5, 7, 1.25f);
            shotsPerSecond = 1.2f;
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
            RotateTowardTarget();

            if (invulnerableTimer > 0)
                invulnerableTimer -= elapsedSeconds;

            base.Update(elapsedSeconds, gameObjects, adam, currentKeyboard, mapManager, random);

            CheckDoors(mapManager);
        }

        private void CheckCheatInput(KeyboardState currentKeyboard)
        {
            //Base
            if (currentKeyboard.IsKeyDown(Keys.D1))
            {
                projectileAttributes = new ProjectileAttributes(5, 7, 1.25f);
                shotsPerSecond = 1.2f;
                health = maxHealth = 6;
                speed = 1;
                accuracy = 0.6f;
                randomElement = false;
            }

            //Upgraded
            else if (currentKeyboard.IsKeyDown(Keys.D2))
            {
                projectileAttributes = new ProjectileAttributes(7, 10, 1.5f);
                shotsPerSecond = 4;
                health = maxHealth = 8;
                speed = 1.2f;
                accuracy = 0.7f;
                randomElement = false;
            }

            //Slow
            else if (currentKeyboard.IsKeyDown(Keys.D3))
            {
                projectileAttributes = new ProjectileAttributes(15, 7, 0.8f);
                shotsPerSecond = 1;
                health = maxHealth = 12;
                speed = 0.9f;
                accuracy = 0.8f;
                randomElement = false;
            }

            //Sniper
            else if (currentKeyboard.IsKeyDown(Keys.D4))
            {
                projectileAttributes = new ProjectileAttributes(10, 12, 5f);
                shotsPerSecond = 2;
                health = maxHealth = 14;
                speed = 1f;
                accuracy = 1;
                randomElement = false;
            }

            //Laser
            else if (currentKeyboard.IsKeyDown(Keys.D5))
            {
                projectileAttributes = new ProjectileAttributes(15, 14, 3f);
                shotsPerSecond = 60;
                health = maxHealth = 6;
                speed = 1.5f;
                accuracy = 1f;
                randomElement = false;
            }

            //Rainbow laser
            else if (currentKeyboard.IsKeyDown(Keys.D6))
            {
                projectileAttributes = new ProjectileAttributes(15, 14, 5f);
                shotsPerSecond = 60;
                health = maxHealth = 6;
                speed = 1.5f;
                accuracy = 1f;
                randomElement = true;
            }

            //Small machinegun
            else if (currentKeyboard.IsKeyDown(Keys.D7))
            {
                projectileAttributes = new ProjectileAttributes(3, 14, 1.75f);
                shotsPerSecond = 60;
                health = maxHealth = 4;
                speed = 1;
                accuracy = 0.3f;
                randomElement = false;
            }

            //Big machine gun
            else if (currentKeyboard.IsKeyDown(Keys.D8))
            {
                projectileAttributes = new ProjectileAttributes(8, 14, 1.75f);
                shotsPerSecond = 60;
                health = maxHealth = 2;
                speed = 1;
                accuracy = 0f;
                randomElement = false;
            }

            //Big rainbow machine gun
            else if (currentKeyboard.IsKeyDown(Keys.D9))
            {
                projectileAttributes = new ProjectileAttributes(12, 14, 1.5f);
                shotsPerSecond = 60;
                health = maxHealth = 2;
                speed = 1;
                accuracy = 0f;
                randomElement = true;
            }
        }

        private void CheckDoors(MapManager mapManager)
        {
            foreach (Door door in mapManager.Rooms[mapManager.CurrentRoomLocation.X, mapManager.CurrentRoomLocation.Y].Doors)
            {
                if (door.EntranceArea.Contains(position))
                {
                    position = door.TransitionExitPosition;
                    mapManager.StartRoomTransition(door);
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

        public override void TakeDamage(float damage)
        {
            if (invulnerableTimer <= 0)
            {
                invulnerableTimer = 2;
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

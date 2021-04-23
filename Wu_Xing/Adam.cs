using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Wu_Xing
{
    class Adam
    {
        private Vector2 position;
        private Vector2 exitPosition;
        private float movingSpeed;

        private Vector2 movingDirection;
        private Vector2 aimingDirection;

        private Rectangle headSource;
        private Rectangle aimingArmSource;
        private Rectangle restingArmSource;
        private float rotation;
        private float rotationTarget;
        private float rotationSpeed;
        private Vector2 leftArmPosition;
        private Vector2 rightArmPosition;

        public Adam(Rectangle window)
        {
            headSource = new Rectangle(140, 0, 140, 140);
            aimingArmSource = new Rectangle(0, 140, 40, 70);
            restingArmSource = new Rectangle(40, 140, 40, 70);

            leftArmPosition = new Vector2(28, 22);
            rightArmPosition = new Vector2(-28, 22);

            rotationSpeed = 0.3f;
            position = window.Size.ToVector2() / 2;
            movingSpeed = 10;
        }

        public Vector2 Position { get { return position; } set { position = value; } }
        public Vector2 ExitPosition { get { return exitPosition; } }

        public void Update(KeyboardState currentKeyboard, MapManager mapManager, Rectangle window)
        {
            DetermineMovingDirection(currentKeyboard);
            DetermineAimingDirection(currentKeyboard);
            DetermineRotationTarget();
            RotateTowardTarget();
            Move(currentKeyboard);

            CheckDoors(mapManager, window);
        }

        private void CheckDoors(MapManager mapManager, Rectangle window)
        {
            foreach (Door door in mapManager.Rooms[mapManager.CurrentRoomLocation.X, mapManager.CurrentRoomLocation.Y].Doors)
            {
                if (door.EntranceArea.Contains(position))
                {
                    position = door.TransitionExitPosition;
                    exitPosition = door.ExitPosition;
                    mapManager.StartRoomTransition(door, window);
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

        private void Move(KeyboardState currentKeyboard)
        {
            if (movingDirection != Vector2.Zero)
                movingDirection.Normalize();
            position += movingDirection * movingSpeed * (currentKeyboard.IsKeyDown(Keys.LeftShift) ? 3 : 1);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(TextureLibrary.Adam, position + Rotate.PointAroundZero(leftArmPosition, rotation), aimingDirection == Vector2.Zero ? restingArmSource : aimingArmSource, Color.White, rotation + (aimingDirection == Vector2.Zero ? -0.3f : 0), new Vector2(aimingArmSource.Width / 2, aimingArmSource.Height / 4), 1, SpriteEffects.None, 0.5f);
            spriteBatch.Draw(TextureLibrary.Adam, position + Rotate.PointAroundZero(rightArmPosition, rotation), aimingDirection == Vector2.Zero ? restingArmSource : aimingArmSource, Color.White, rotation + (aimingDirection == Vector2.Zero ? 0.3f : 0), new Vector2(aimingArmSource.Width / 2, aimingArmSource.Height / 4), 1, SpriteEffects.None, 0.5f);
            spriteBatch.Draw(TextureLibrary.Adam, position, headSource, Color.White, rotation, new Vector2(headSource.Width / 2, headSource.Height / 2), 1, SpriteEffects.None, 0.51f);
        }

        public void DrawHearts(SpriteBatch spriteBatch)
        {
            int maxHealth = 6;
            int health = 5;

            for (int i = 1; i <= maxHealth / 2; i++)
                spriteBatch.Draw(TextureLibrary.Heart, new Vector2(175 + i * 65, 65), new Rectangle(health >= i * 2 ? 0 : health == i * 2 - 1 ? 60 : 120, 0, 60, 60), Color.White);
        }

    }
}

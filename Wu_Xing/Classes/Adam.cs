using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Monolights;

namespace Wu_Xing
{
    class Adam
    {
        private Vector2 position;
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

        private PointLight _pointlight;


        public Adam(Rectangle window)
        {
            headSource = new Rectangle(140, 0, 140, 140);
            aimingArmSource = new Rectangle(0, 140, 40, 70);
            restingArmSource = new Rectangle(40, 140, 40, 70);

            leftArmPosition = new Vector2(28, 22);
            rightArmPosition = new Vector2(-28, 22);

            rotationSpeed = 0.3f;
            position = window.Size.ToVector2() / 2;
            movingSpeed = 8;

            _pointlight = new PointLight()
            {
                Color = Color.White,
                Power = 0.5f,
                LightDecay = 300,
                Position = new Vector3(position.X, position.Y, 0),
                IsEnabled = true
            };


        }

        public void Update(KeyboardState currentKeyboard)
        {
            _pointlight.Position = new Vector3(position.X, position.Y, 0);
            DetermineMovingDirection(currentKeyboard);
            DetermineAimingDirection(currentKeyboard);
            DetermineRotationTarget();
            RotateTowardTarget();
            Move();
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

        private void Move()
        {
            if (movingDirection != Vector2.Zero)
                movingDirection.Normalize();
            position += movingDirection * movingSpeed;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //Left arm
            spriteBatch.Draw(TextureLibrary.Adam, position + Rotate.PointAroundZero(leftArmPosition, rotation), aimingDirection == Vector2.Zero ? restingArmSource : aimingArmSource, Color.White, rotation + (aimingDirection == Vector2.Zero ? -0.3f : 0), new Vector2(aimingArmSource.Width / 2, aimingArmSource.Height / 4), 1, SpriteEffects.None, 0);

            //Right arm
            spriteBatch.Draw(TextureLibrary.Adam, position + Rotate.PointAroundZero(rightArmPosition, rotation), aimingDirection == Vector2.Zero ? restingArmSource : aimingArmSource, Color.White, rotation + (aimingDirection == Vector2.Zero ? 0.3f : 0), new Vector2(aimingArmSource.Width / 2, aimingArmSource.Height / 4), 1, SpriteEffects.None, 0);

            //Head
            spriteBatch.Draw(TextureLibrary.Adam, position, headSource, Color.White, rotation, new Vector2(headSource.Width / 2, headSource.Height / 2), 1, SpriteEffects.None, 0);
        }

    }
}

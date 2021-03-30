using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Wu_Xing
{
    class Adam
    {
        private enum Direction { None, Down, Up, Left, Right }
        private Direction facingDirection;
        private Direction aimingDirection;

        private Vector2 position;
        private Vector2 velocity;
        private float speed;

        private Dictionary<Direction, Rectangle> headSource;
        private Dictionary<Direction, Rectangle> torsoSource;
        private Dictionary<Direction, Rectangle> footSource;
        private Rectangle armSource;
        private Rectangle leftLegSource;
        private Rectangle rightLegSource;

        private Dictionary<Direction, float> armRotation;
        private Dictionary<Direction, Vector2> leftArmPosition;
        private Dictionary<Direction, Vector2> rightArmPosition;
        private Dictionary<Direction, Vector2> leftLegPosition;
        private Dictionary<Direction, Vector2> rightLegPosition;
        private Dictionary<Direction, Vector2> leftFootPosition;
        private Dictionary<Direction, Vector2> rightFootPosition;
        private Vector2 torsoPosition;

        public Adam()
        {
            facingDirection = Direction.Down;
            aimingDirection = Direction.None;

            headSource = new Dictionary<Direction, Rectangle> {
                { Direction.Down, new Rectangle(0, 0, 160, 160) },
                { Direction.Up, new Rectangle(160, 0, 160, 160) },
                { Direction.Left, new Rectangle(320, 0, 160, 160) },
                { Direction.Right, new Rectangle(480, 0, 160, 160) } };

            torsoSource = new Dictionary<Direction, Rectangle> {
                { Direction.Down, new Rectangle(0, 160, 70, 50) },
                { Direction.Up, new Rectangle(70, 160, 70, 50) },
                { Direction.Left, new Rectangle(140, 160, 70, 50) },
                { Direction.Right, new Rectangle(210, 160, 70, 50) } };

            footSource = new Dictionary<Direction, Rectangle> {
                { Direction.Down, new Rectangle(360, 160, 40, 40) },
                { Direction.Up, new Rectangle(360, 160, 40, 40) },
                { Direction.Left, new Rectangle(400, 160, 40, 40) },
                { Direction.Right, new Rectangle(400, 160, 40, 40) } };

            armSource = new Rectangle(440, 160, 40, 80);
            leftLegSource = new Rectangle(320, 160, 40, 50);
            rightLegSource = new Rectangle(280, 160, 40, 50);

            armRotation = new Dictionary<Direction, float> {
                { Direction.Down, 0 },
                { Direction.Up, (float)Math.PI },
                { Direction.Left, (float)Math.PI * 0.5f },
                { Direction.Right, (float)Math.PI * 1.5f },
                { Direction.None, (float)Math.PI * 0.07f } };

            leftArmPosition = new Dictionary<Direction, Vector2> {
                { Direction.Down, new Vector2(34, -70) },
                { Direction.Up, new Vector2(34, -70) },
                { Direction.Left, new Vector2(0, -60) },
                { Direction.Right, new Vector2(0, -80) } };

            rightArmPosition = new Dictionary<Direction, Vector2> {
                { Direction.Down, new Vector2(-34, -70) },
                { Direction.Up, new Vector2(-34, -70) },
                { Direction.Left, new Vector2(0, -80) },
                { Direction.Right, new Vector2(0, -60) } };

            leftLegPosition = new Dictionary<Direction, Vector2> {
                { Direction.Down, new Vector2(12, -40) },
                { Direction.Up, new Vector2(12, -36) },
                { Direction.Left, new Vector2(0, -30) },
                { Direction.Right, new Vector2(0, -50) } };

            rightLegPosition = new Dictionary<Direction, Vector2> {
                { Direction.Down, new Vector2(-12, -40) },
                { Direction.Up, new Vector2(-12, -36) },
                { Direction.Left, new Vector2(0, -50) },
                { Direction.Right, new Vector2(0, -30) } };

            leftFootPosition = new Dictionary<Direction, Vector2> {
                { Direction.Down, new Vector2(14, -22) },
                { Direction.Up, new Vector2(14, -22) },
                { Direction.Left, new Vector2(0, -12) },
                { Direction.Right, new Vector2(0, -38) } };

            rightFootPosition = new Dictionary<Direction, Vector2> {
                { Direction.Down, new Vector2(-14, -22) },
                { Direction.Up, new Vector2(-14, -22) },
                { Direction.Left, new Vector2(0, -38) },
                { Direction.Right, new Vector2(0, -12) } };

            torsoPosition = new Vector2(0, -65);

            position = new Vector2(200, 200);
            speed = 10;
        }

        public void Update(KeyboardState currentKeyboard, KeyboardState previousKeyboard)
        {
            //Determine velocity
            velocity = Vector2.Zero;

            if (currentKeyboard.IsKeyDown(Keys.W))
                velocity.Y = -1;

            else if (currentKeyboard.IsKeyDown(Keys.S))
                velocity.Y = 1;

            if (currentKeyboard.IsKeyDown(Keys.A))
                velocity.X = -1;

            else if (currentKeyboard.IsKeyDown(Keys.D))
                velocity.X = 1;

            //Determine facing direction
            if (velocity.Y == -1)
                facingDirection = Direction.Up;

            else if (velocity.Y == 1)
                facingDirection = Direction.Down;

            else if (velocity.X == -1)
                facingDirection = Direction.Left;

            else if (velocity.X == 1)
                facingDirection = Direction.Right;

            //Move
            if (velocity != Vector2.Zero)
                velocity.Normalize();
            position += velocity * speed;

            //Determine aiming direction
            if (currentKeyboard.IsKeyDown(Keys.Up))
                aimingDirection = Direction.Up;

            else if (currentKeyboard.IsKeyDown(Keys.Down))
                aimingDirection = Direction.Down;

            else if (currentKeyboard.IsKeyDown(Keys.Left))
                aimingDirection = Direction.Left;

            else if (currentKeyboard.IsKeyDown(Keys.Right))
                aimingDirection = Direction.Right;

            else
                aimingDirection = Direction.None;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //Left foot
            spriteBatch.Draw(TextureLibrary.Adam, position + leftFootPosition[facingDirection], footSource[facingDirection], Color.White, 0, new Vector2(footSource[facingDirection].Width / 2, footSource[facingDirection].Height / 2), 1, SpriteEffects.None, 0);

            //Right foot
            spriteBatch.Draw(TextureLibrary.Adam, position + rightFootPosition[facingDirection], footSource[facingDirection], Color.White, 0, new Vector2(footSource[facingDirection].Width / 2, footSource[facingDirection].Height / 2), 1, SpriteEffects.None, 0);

            //Left leg
            spriteBatch.Draw(TextureLibrary.Adam, position + leftLegPosition[facingDirection], facingDirection == Direction.Right ? rightLegSource : leftLegSource, Color.White, 0, new Vector2(leftLegSource.Width / 2, leftLegSource.Height / 2), 1, SpriteEffects.None, 0);

            //Right leg
            spriteBatch.Draw(TextureLibrary.Adam, position + rightLegPosition[facingDirection], facingDirection == Direction.Left ? leftLegSource : rightLegSource, Color.White, 0, new Vector2(rightLegSource.Width / 2, rightLegSource.Height / 2), 1, SpriteEffects.None, 0);

            //Torso
            spriteBatch.Draw(TextureLibrary.Adam, position + torsoPosition, torsoSource[facingDirection], Color.White, 0, new Vector2(torsoSource[facingDirection].Width / 2, torsoSource[facingDirection].Height / 2), 1, SpriteEffects.None, 0);

            //Left arm
            spriteBatch.Draw(TextureLibrary.Adam, position + leftArmPosition[facingDirection], armSource, Color.White, aimingDirection == Direction.None ? -armRotation[aimingDirection] : armRotation[aimingDirection], new Vector2(armSource.Width / 2, armSource.Height / 2), 1, SpriteEffects.None, 0);

            //Right arm
            spriteBatch.Draw(TextureLibrary.Adam, position + rightArmPosition[facingDirection], armSource, Color.White, armRotation[aimingDirection], new Vector2(armSource.Width / 2, armSource.Height / 2), 1, SpriteEffects.None, 0);

            



        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Wu_Xing
{
    class Soul : Enemy
    {
        private Vector2? destination;
        private bool attack;
        private static float maximumMovingDistance = 200;

        public Soul(Vector2 position, Element element, Random random) : base(position, element, random)
        {
            //GameObject
            texture = TextureLibrary.Soul;
            color = ColorLibrary.Element[element];
            source.Size = new Point(100, 130);
            hitbox.Size = new Point(60);
            origin = source.Size.ToVector2() / 2 + new Vector2(0, 30);
            animationFPS = 60;
            MoveTo(position);
            RandomSourceLocation(random);

            //Character
            health = maxHealth = 30;
            speed = random.Next(5, 16) / 10f; //0.5-1.5
            shadowSize = 80;
            projectileAttributes = new ProjectileAttributes(1, 4, 0.7f, 0.5f);
            shotsPerSecond = random.Next(4, 8) / 10f; //0.4-0.7
            accuracy = 0.9f;

            //Enemy
            detectionRange = 12;
        }

        public override void Update(float elapsedSeconds, List<GameObject> gameObjects, Adam adam, KeyboardState currentKeyboard, MapManager mapManager, Random random)
        {
            //Decrease cooldown
            if (shotCooldown > 0)
                shotCooldown -= elapsedSeconds;

            //If ready, not moving, and within detectionRange
            if (shotCooldown <= 0 && destination == null && Vector2.Distance(position, adam.Position) <= detectionRange * 100)
            {
                //Reset cooldown
                shotCooldown += 1 / shotsPerSecond;

                DetermineMovingDirection(adam, random);
            }

            //If has destination, move
            else if (destination != null)
            {
                MoveToDestination(elapsedSeconds, gameObjects, adam, random);
            }

            base.Update(elapsedSeconds, gameObjects, adam, currentKeyboard, mapManager, random);
        }

        private void MoveToDestination(float elapsedSeconds, List<GameObject> gameObjects, Adam adam, Random random)
        {
            Vector2 positionRelativeToDestination = position - (Vector2)destination;
            float angleBefore = (float)Math.Atan2(-positionRelativeToDestination.X, positionRelativeToDestination.Y);

            MoveTo(position + (movingDirection * 600 * elapsedSeconds * speed));

            positionRelativeToDestination = position - (Vector2)destination;
            float angleAfter = (float)Math.Atan2(-positionRelativeToDestination.X, positionRelativeToDestination.Y);

            //If the destination was surpassed
            if (Math.Abs(angleBefore - angleAfter) > Math.PI / 2)
            {
                position = (Vector2)destination;
                destination = null;

                if (attack)
                    CreateProjectile(adam, gameObjects, random);
            }
        }

        private void DetermineMovingDirection(Adam adam, Random random)
        {
            Vector2 positionRelativeToAdam = position - adam.Position;
            float angleRelativeToAdam = (float)Math.Atan2(positionRelativeToAdam.X, -positionRelativeToAdam.Y);

            float idealDistance = detectionRange * 100 * 0.3f;
            Vector2 idealPosition = adam.Position + Rotate.PointAroundZero(new Vector2(0, -idealDistance), angleRelativeToAdam);
            float distanceToIdealPosition = Vector2.Distance(position, idealPosition);

            Vector2 positionRelativeToIdealPosition = position - idealPosition;
            float angleRelativeToIdealPosition = (float)Math.Atan2(positionRelativeToIdealPosition.X, -positionRelativeToIdealPosition.Y);

            //If more than one move away from ideal position, move closer to ideal position
            if (distanceToIdealPosition > maximumMovingDistance)
            {
                destination = position + Rotate.PointAroundZero(new Vector2(0, -maximumMovingDistance), angleRelativeToIdealPosition + (float)Math.PI);
                attack = false;
            }

            //If less than half a move away from ideal position, mode sideways and attack
            else if (distanceToIdealPosition < maximumMovingDistance / 2)
            {
                destination = Rotate.PointAroundCenter(idealPosition, adam.Position, random.Next(2) == 0 ? 0.7f : -0.7f);
                attack = true;
            }

            //If one move away from ideal position, move to ideal position and attack
            else
            {
                destination = idealPosition;
                attack = true;
            }

            movingDirection = (Vector2)destination - position;
            movingDirection.Normalize();
        }

        private void CreateProjectile(Adam adam, List<GameObject> gameObjects, Random random)
        {
            for (int i = -1; i <= 1; i += 2)
            {
                Vector2 aimingDirection = adam.Position - this.position;
                aimingDirection.Normalize();

                float rotation = (float)Math.Atan2(-aimingDirection.X, aimingDirection.Y);
                Vector2 position = this.position + aimingDirection * hitbox.Width * 0.7f;

                gameObjects.Add(new Projectile(position, element, random, Projectile.Type.MagicBall, projectileAttributes, rotation + 0.3f * i + AccuracyOffset(random), false));
                gameObjects[gameObjects.Count - 1].MoveTo(gameObjects[gameObjects.Count - 1].Position + Rotate.PointAroundZero(Vector2.UnitY, rotation) * gameObjects[gameObjects.Count - 1].Hitbox.Width * 0.5f);
            }
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 roomPosition, bool drawHitbox)
        {
            base.Draw(spriteBatch, roomPosition, drawHitbox);
            spriteBatch.Draw(TextureLibrary.ElementalEyes, roomPosition + position + new Vector2(0, 5), null, Color.White, 0, origin, 1, SpriteEffects.None, layerDepth + 0.001f);
        }
    }
}

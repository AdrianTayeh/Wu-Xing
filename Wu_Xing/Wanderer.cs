using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Wu_Xing
{
    class Wanderer : Enemy
    {
        public Wanderer(Vector2 position, Element element, Random random) : base(position, element, random)
        {
            //GameObject
            texture = TextureLibrary.Wanderer;
            color = ColorLibrary.Element[element];
            source.Size = new Point(150, 195);
            hitbox.Size = new Point(70);
            origin = source.Size.ToVector2() / 2;
            animationFPS = 60;
            MoveTo(position);
            RandomSourceLocation(random);
            RandomRotation(random, 8);

            //Character
            health = maxHealth = 40;
            speed = 0.2f;
            shadowSize = 80;

            //Enemy
            detectionRange = 10;
        }

        public override void Update(float elapsedSeconds, List<GameObject> gameObjects, Adam adam, KeyboardState currentKeyboard, MapManager mapManager, Random random)
        {
            DetermineMovingDirection(adam);

            if (movingDirection != Vector2.Zero)
                MoveTo(position + (Rotate.PointAroundZero(Vector2.UnitY, rotation) * 600 * elapsedSeconds * speed));

            base.Update(elapsedSeconds, gameObjects, adam, currentKeyboard, mapManager, random);
        }

        private void DetermineMovingDirection(Adam adam)
        {
            float distanceToAdam = Vector2.Distance(position, adam.Position);

            if (distanceToAdam < detectionRange * 100 && distanceToAdam > hitbox.Width * 0.75)
            {
                movingDirection = adam.Position - position;
                movingDirection.Normalize();
                rotationTarget = (float)Math.Atan2(-movingDirection.X, movingDirection.Y);
            }

            else
            {
                movingDirection = Vector2.Zero;
            }
        }

    }
}

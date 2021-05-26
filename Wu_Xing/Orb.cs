using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Wu_Xing
{
    class Orb : Enemy
    {
        public Orb(Vector2 position, Element element, Random random) : base(position, element, random)
        {
            //GameObject
            texture = TextureLibrary.Orb;
            color = ColorLibrary.Element[element];
            source.Size = new Point(100, 130);
            hitbox = new Hitbox(Hitbox.HitboxType.OnGround, true, position, new Point(60));
            origin = source.Size.ToVector2() / 2 + new Vector2(0, 15);
            animationFPS = 60;
            RandomSourceLocation(random);

            //Character
            health = maxHealth = 20;
            speed = 0.4f;
            shadowSize = 80;

            //Enemy
            detectionRange = 15;
        }

        public override void Update(float elapsedSeconds, List<GameObject> gameObjects, Adam adam, KeyboardState currentKeyboard, MapManager mapManager, Random random)
        {
            DetermineMovingDirection(adam);
            Move(position + (movingDirection * 600 * elapsedSeconds * speed), gameObjects, mapManager.CurrentRoom.Hitboxes);
            base.Update(elapsedSeconds, gameObjects, adam, currentKeyboard, mapManager, random);
        }

        private void DetermineMovingDirection(Adam adam)
        {
            float distanceToAdam = Vector2.Distance(position, adam.Position);

            if (distanceToAdam < detectionRange * 100 && distanceToAdam > hitbox.Width * 0.75)
            {
                movingDirection = adam.Position - position;
                movingDirection.Normalize();
            }

            else
            {
                movingDirection = Vector2.Zero;
            }
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 roomPosition, bool drawHitbox)
        {
            base.Draw(spriteBatch, roomPosition, drawHitbox);
            spriteBatch.Draw(TextureLibrary.ElementalEyes, roomPosition + position + new Vector2(0, 35), null, Color.White, 0, origin, 1, SpriteEffects.None, layerDepth + 0.001f);
        }
    }
}

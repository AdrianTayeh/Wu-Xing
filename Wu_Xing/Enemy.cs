using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Wu_Xing
{
    class Enemy : Character
    {
        // Initialized in constructor
        protected float meleeDamage;

        // Will be initialized in subclass constructor
        protected float detectionRange;

        public Enemy(Vector2 position, Element? element, Random random) : base(position, element, random)
        {
            //Character
            rotationSpeed = 0.05f;

            //Enemy
            meleeDamage = 1;
        }

        public override void Update(float elapsedSeconds, List<GameObject> gameObjects, Adam adam, KeyboardState currentKeyboard, MapManager mapManager, Random random)
        {
            DealMeleeDamage(adam, random);
            base.Update(elapsedSeconds, gameObjects, adam, currentKeyboard, mapManager, random);
        }

        private void DealMeleeDamage(Adam adam, Random random)
        {
            if (hitbox.Intersects(adam.Hitbox))
                adam.TakeDamage(meleeDamage, random);
        }
    }
}

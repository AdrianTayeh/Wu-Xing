using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Wu_Xing
{
    class Enemy : Character
    {
        public Enemy(Vector2 position, Element? element, Random random) : base(position, element, random)
        {

        }

        public override void Update(float elapsedSeconds, List<GameObject> gameObjects, Adam adam, KeyboardState currentKeyboard, MapManager mapManager)
        {
            base.Update(elapsedSeconds, gameObjects, adam, currentKeyboard, mapManager);
        }
    }
}

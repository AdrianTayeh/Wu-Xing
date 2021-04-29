using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Wu_Xing
{
    class Tile : GameObject
    {
        public Tile(Vector2 position, Element? element, Random random) : base(position, element, random)
        {
            //GameObject
            layerDepth = 0.11f;
            source.Size = new Point(100, 100);
            origin = source.Size.ToVector2() / 2;
        }

        public override void Update(float elapsedSeconds, List<GameObject> gameObjects, Adam adam, KeyboardState currentKeyboard, MapManager mapManager, Random random)
        {
            base.Update(elapsedSeconds, gameObjects, adam, currentKeyboard, mapManager, random);
        }
    }
}

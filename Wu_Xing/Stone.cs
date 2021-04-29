using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Wu_Xing
{
    class Stone : Tile
    {
        public Stone(Vector2 position, Element? element, Random random) : base(position, element, random)
        {
            //GameObject
            texture = TextureLibrary.Stone;
            hitbox.Size = new Point(100);
            MoveTo(position);
            RandomSourceLocation(random);
            RandomRotation(random, 4);
        }
    }
}
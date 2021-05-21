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
            hitbox = new Hitbox(Hitbox.HitboxType.OnGround, true, position, new Point(100));
            RandomSourceLocation(random);
            RandomRotation(random, 4);
        }
    }
}
using System;
using Microsoft.Xna.Framework;

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
            NewRandomSourceLocation(random);
        }
    }
}
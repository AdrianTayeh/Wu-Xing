using System;
using Microsoft.Xna.Framework;

namespace Wu_Xing
{
    class Stone : Tile
    {
        public Stone(Vector2 position, Element? element) : base(position, element)
        {
            source.Location = new Point(100, 0);
            hitbox.Size = new Point(100, 100);
        }
    }
}

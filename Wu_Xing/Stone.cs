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
            source.Location = new Point(random.Next(3) * 100, random.Next(3) * 100);
            hitbox.Size = new Point(100, 100);

            //Tile
            NewRandomSourceLocation(random);
        }
    }
}

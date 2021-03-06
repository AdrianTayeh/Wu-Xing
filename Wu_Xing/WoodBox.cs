using System;
using Microsoft.Xna.Framework;

namespace Wu_Xing
{
    class WoodBox : Tile
    {
        public WoodBox(Vector2 position, Element? element, Random random) : base(position, element, random)
        {
            //GameObject
            texture = TextureLibrary.WoodBox;
            hitbox = new Hitbox(Hitbox.HitboxType.OnGround, true, position, new Point(100));
            RandomSourceLocation(random);
            RandomRotation(random, 4);
        }
    }
}

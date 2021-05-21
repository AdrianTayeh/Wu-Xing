using System;
using Microsoft.Xna.Framework;

namespace Wu_Xing
{
    class MetalBox : Tile
    {
        public MetalBox(Vector2 position, Element? element, Random random) : base(position, element, random)
        {
            //GameObject
            texture = TextureLibrary.MetalBox;
            hitbox = new Hitbox(Hitbox.HitboxType.OnGround, true, position, new Point(100));
            RandomSourceLocation(random);
        }
    }
}

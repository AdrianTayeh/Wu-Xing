using System;
using Microsoft.Xna.Framework;

namespace Wu_Xing
{
    class Fire : Tile
    {
        public Fire(Vector2 position, Element? element, Random random) : base(position, element, random)
        {
            //GameObject
            texture = TextureLibrary.Fire;
            hitbox.Size = new Point(60);
            source.Size = new Point(150, 200);
            RandomSourceLocation(random);
            MoveTo(position);
            animationFPS = 60;
            origin = new Vector2(source.Width / 2, source.Height * 0.6f);
        }
    }
}

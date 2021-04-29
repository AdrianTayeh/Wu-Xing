using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Wu_Xing
{
    class Wanderer : Enemy
    {
        public Wanderer(Vector2 position, Element element, Random random) : base(position, element, random)
        {
            //GameObject
            texture = TextureLibrary.Wanderer;
            color = ColorLibrary.Element[element];
            source.Size = new Point(150, 195);
            hitbox.Size = new Point(70);
            origin = source.Size.ToVector2() / 2;
            animationFPS = 60;
            MoveTo(position);
            NewRandomSourceLocation(random);
        }
    }
}

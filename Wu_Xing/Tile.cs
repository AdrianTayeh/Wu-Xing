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

            //No use of calling NewRandomSourceLocation, since sources havn't been initialized at this point
        }

        public void NewRandomSourceLocation(Random random)
        {
            //Using the size of an objects texture and source,
            //a new source location can be randomized, assuming the texture consists of a grid of same sized sources
            //Does not fail even if the texture only holds one source
            source.Location = new Point(random.Next(texture.Width / source.Width) * source.Width, random.Next(texture.Height / source.Height) * source.Height);
        }
    }
}

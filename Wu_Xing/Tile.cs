using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Wu_Xing
{
    class Tile : GameObject
    {
        public Tile(Vector2 position, Element? element) : base(position, element)
        {
            layerDepth = 0.11f;
            texture = TextureLibrary.Tiles;
            source.Size = new Point(100, 100);
            origin = source.Size.ToVector2() / 2;
        }
    }
}

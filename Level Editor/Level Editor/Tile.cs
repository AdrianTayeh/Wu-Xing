using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Level_Editor
{
    abstract class Tile
    {
        protected Rectangle source;
        protected Vector2 origin;
        protected float layerDepth;
        public Tile(Vector2 position)
        {
            source.Size = new Point(100, 100);
            origin = source.Size.ToVector2() / 2;
            layerDepth = 0.11f;
        }
    }
}

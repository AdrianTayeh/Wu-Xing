using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prototype_Room
{
    class Tile
    {
        public Vector2 pos;
        public bool wall;
        Rectangle rec;
        public Texture2D tex;

        public Tile(Vector2 pos, bool wall, Rectangle rec, Texture2D tex)
        {
            this.pos = pos;
            this.wall = wall;
            this.tex = tex;
            this.rec = rec;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //spriteBatch.Draw(tex, pos, Color.White);
            spriteBatch.Draw(tex, new Rectangle((int)pos.X, (int)pos.Y, 50, 50), rec, Color.White);
        }
    }
}


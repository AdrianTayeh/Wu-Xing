using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prototype_Room
{
    class Door
    {
        Vector2 pos;
        Texture2D tex;
        public Rectangle hitbox;
        bool locked;
        LevelManager level1;

        public Door(Vector2 pos, Texture2D tex, Rectangle hitbox, LevelManager level1)
        {
            this.pos = pos;
            this.tex = tex;
            this.hitbox = hitbox;
            this.level1 = level1;
        }

        public void draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(tex, hitbox, Color.White);
        }
    }
}

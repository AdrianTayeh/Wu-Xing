using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prototype_Room
{
    class GameObject
    {
        protected Vector2 position;
        protected Texture2D texture;

        public GameObject(Vector2 pos, Texture2D tex)
        {
            this.position = pos;
            this.texture = tex;
        }
    }
}

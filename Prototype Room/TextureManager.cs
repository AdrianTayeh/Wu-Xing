using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prototype_Room
{
    class TextureManager
    {
        public static Texture2D PacMan;
        public static Texture2D TileSheet;
        public static Texture2D empty;
        public static Texture2D dot;
        public static Texture2D door1;


        public static void LoadTextures(ContentManager cm)
        {
            PacMan = cm.Load<Texture2D>("pacman");
            TileSheet = cm.Load<Texture2D>("Tileset");
            empty = cm.Load<Texture2D>("empty");
            dot = cm.Load<Texture2D>("centerDot");
            door1 = cm.Load<Texture2D>("door");
        }
    }
}

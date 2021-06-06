using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Level_Editor
{
    static class FontLibrary
    {
        public static SpriteFont Normal { get; private set; }
        public static SpriteFont Big { get; private set; }
        public static SpriteFont Huge { get; private set; }

        public static void Load(ContentManager Content)
        {
            Normal = Content.Load<SpriteFont>("Normal");
            Big = Content.Load<SpriteFont>("Big");
            Huge = Content.Load<SpriteFont>("Huge");
        }
    }
}

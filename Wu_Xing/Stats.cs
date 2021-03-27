using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wu_Xing
{
    class Stats
    {
        public Stats(Rectangle window)
        {

        }

        public void Update(ref Screen screen, Mouse mouse, KeyboardState currentKeyboard, KeyboardState previousKeyboard)
        {
            if (currentKeyboard.IsKeyUp(Keys.Escape) && previousKeyboard.IsKeyDown(Keys.Escape))
                screen = Screen.Pregame;
        }

        public void Draw(SpriteBatch spriteBatch, Rectangle window)
        {
            spriteBatch.Draw(TextureLibrary.BackgroundGray, window, Color.White);
        }
    }
}

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
    class Running
    {
        private Adam adam;

        public Running(Rectangle window)
        {
            adam = new Adam(window);
        }

        public void Update(ref Screen screen, Mouse mouse, KeyboardState currentKeyboard, KeyboardState previousKeyboard)
        {
            if (currentKeyboard.IsKeyUp(Keys.Escape) && previousKeyboard.IsKeyDown(Keys.Escape))
                screen = Screen.Pregame;

            adam.Update(currentKeyboard);
        }

        public void Draw(SpriteBatch spriteBatch, Rectangle window)
        {
            spriteBatch.Draw(TextureLibrary.RoomBlack1x1, window.Center.ToVector2(), null, Color.White, 0, TextureLibrary.RoomBlack1x1.Bounds.Size.ToVector2() / 2, 1, SpriteEffects.None, 0);
            adam.Draw(spriteBatch);
        }
    }
}

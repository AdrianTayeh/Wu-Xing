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
    class Start
    {
        public Start()
        {

        }

        public void Update(ref Screen screen, Mouse mouse, KeyboardState currentKeyboardState, KeyboardState previousKeyboardState)
        {
            if ((currentKeyboardState.GetPressedKeys().Count() > 0 && currentKeyboardState.IsKeyUp(Keys.Escape)) || mouse.LeftIsPressed || mouse.RightIsPressed)
                screen = Screen.Menu;
            //SoundLibrary.StartingScreenInstance.Play();
        }

        public void Draw(SpriteBatch spriteBatch, Rectangle window)
        {
            spriteBatch.Draw(TextureLibrary.BackgroundRed, window, Color.White);
            spriteBatch.Draw(TextureLibrary.WXLogoDarkDots, new Vector2(window.Width / 2, 475), null, Color.White, 0, TextureLibrary.WXLogoDarkDots.Bounds.Size.ToVector2() / 2, 0.6f, SpriteEffects.None, 0);
            spriteBatch.DrawString(FontLibrary.Normal, "PRESS ANY KEY TO START", new Vector2(window.Width / 2, 720), Color.White, 0, FontLibrary.Normal.MeasureString("PRESS ANY KEY TO START") / 2, 1, SpriteEffects.None, 0);
        }
    }
}

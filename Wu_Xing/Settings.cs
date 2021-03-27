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
    class Settings
    {
        private Dictionary<string, Button> button = new Dictionary<string, Button>();

        public Settings(Rectangle window)
        {
            button.Add("Default", new Button(
                new Point(window.Width / 2 - 140, 860),
                new Point(260, 70),
                "DEFAULT", FontLibrary.Normal,
                TextureLibrary.WhitePixel, null,
                ColorLibrary.WhiteButtonBackgroundColor,
                ColorLibrary.WhiteButtonLabelColor
                ));

            button.Add("Back", new Button(
                new Point(window.Width / 2 + 140, 860),
                new Point(260, 70),
                "BACK", FontLibrary.Normal,
                TextureLibrary.WhitePixel, null,
                ColorLibrary.WhiteButtonBackgroundColor,
                ColorLibrary.WhiteButtonLabelColor
                ));
        }

        public void Update(ref Screen screen, Mouse mouse, KeyboardState currentKeyboard, KeyboardState previousKeyboard)
        {
            if (currentKeyboard.IsKeyUp(Keys.Escape) && previousKeyboard.IsKeyDown(Keys.Escape))
                screen = Screen.Menu;

            foreach (KeyValuePair<string, Button> item in button)
                item.Value.Update(mouse);

            if (button["Default"].IsReleased) { }
                //Apply default settings

            else if (button["Back"].IsReleased)
                screen = Screen.Menu;
        }

        public void Draw(SpriteBatch spriteBatch, Rectangle window)
        {
            spriteBatch.Draw(TextureLibrary.BackgroundGray, window, Color.White);
            spriteBatch.DrawString(FontLibrary.Normal, "SETTINGS", new Vector2(window.Width / 2, 150), Color.White, 0, FontLibrary.Normal.MeasureString("SETTINGS") / 2, 1, SpriteEffects.None, 0);

            foreach (KeyValuePair<string, Button> item in button)
                item.Value.Draw(spriteBatch);
        }
    }
}

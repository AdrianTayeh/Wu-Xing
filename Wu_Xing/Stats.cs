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
        private Button back;

        public Stats(Rectangle window)
        {
            back = new Button(
                new Point(window.Width / 2, 770),
                new Point(260, 70),
                "BACK", FontLibrary.Normal,
                TextureLibrary.WhitePixel, null,
                ColorLibrary.WhiteButtonBackgroundColor,
                ColorLibrary.WhiteButtonLabelColor
                );
        }

        public void Update(ref Screen screen, Mouse mouse, KeyboardState currentKeyboard, KeyboardState previousKeyboard)
        {
            if (currentKeyboard.IsKeyUp(Keys.Escape) && previousKeyboard.IsKeyDown(Keys.Escape))
                screen = Screen.Pregame;

            back.Update(mouse);

            if (back.IsReleased)
                screen = Screen.Pregame;
        }

        public void Draw(SpriteBatch spriteBatch, Rectangle window)
        {
            spriteBatch.Draw(TextureLibrary.BackgroundGray, window, Color.White);
            spriteBatch.DrawString(FontLibrary.Normal, "STATISTICS", new Vector2(window.Width / 2, 350), Color.White, 0, FontLibrary.Normal.MeasureString("STATISTICS") / 2, 1, SpriteEffects.None, 0);

            int start = 450;
            int spacing = 50;

            spriteBatch.DrawString(FontLibrary.Normal, "RUNS STARTED", new Vector2(760, start), Color.White);
            spriteBatch.DrawString(FontLibrary.Normal, "DEATHS", new Vector2(760, start + spacing), Color.White);
            spriteBatch.DrawString(FontLibrary.Normal, "ENEMIES KILLED", new Vector2(760, start + spacing * 2), Color.White);
            spriteBatch.DrawString(FontLibrary.Normal, "BOSSES DEFEATED", new Vector2(760, start + spacing * 3), Color.White);

            spriteBatch.DrawString(FontLibrary.Normal, "99", new Vector2(1110, start), Color.White);
            spriteBatch.DrawString(FontLibrary.Normal, "77", new Vector2(1110, start + spacing), Color.White);
            spriteBatch.DrawString(FontLibrary.Normal, "555", new Vector2(1110, start + spacing * 2), Color.White);
            spriteBatch.DrawString(FontLibrary.Normal, "66", new Vector2(1110, start + spacing * 3), Color.White);

            back.Draw(spriteBatch);
        }
    }
}

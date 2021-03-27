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
    class Pregame
    {
        private Dictionary<string, Button> button = new Dictionary<string, Button>();

        public Pregame(Rectangle window)
        {
            button.Add("Continue", new Button(
                new Point(window.Width / 2, 405),
                new Point(260, 70),
                "CONTINUE", FontLibrary.Normal,
                TextureLibrary.WhitePixel, null,
                ColorLibrary.WhiteButtonBackgroundColor,
                ColorLibrary.WhiteButtonLabelColor
                ));

            button.Add("New game", new Button(
                new Point(window.Width / 2, 405 + 70 + 20),
                new Point(260, 70),
                "NEW GAME", FontLibrary.Normal,
                TextureLibrary.WhitePixel, null,
                ColorLibrary.WhiteButtonBackgroundColor,
                ColorLibrary.WhiteButtonLabelColor
                ));

            button.Add("Stats", new Button(
                new Point(window.Width / 2, 405 + 140 + 40),
                new Point(260, 70),
                "STATS", FontLibrary.Normal,
                TextureLibrary.WhitePixel, null,
                ColorLibrary.WhiteButtonBackgroundColor,
                ColorLibrary.WhiteButtonLabelColor
                ));

            button.Add("Back", new Button(
                new Point(window.Width / 2, 405 + 210 + 60),
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

            if (button["Continue"].IsReleased)
            {
                screen = Screen.Running;
                //Load existing run
            }

            else if (button["New game"].IsReleased)
            {
                screen = Screen.NewGame;
            }

            else if (button["Stats"].IsReleased)
            {
                screen = Screen.Stats;
            }

            else if (button["Back"].IsReleased)
            {
                screen = Screen.Menu;
            }
        }

        public void Draw(SpriteBatch spriteBatch, Rectangle window)
        {
            spriteBatch.Draw(TextureLibrary.BackgroundGray, window, Color.White);

            foreach (KeyValuePair<string, Button> item in button)
                item.Value.Draw(spriteBatch);
        }
    }
}

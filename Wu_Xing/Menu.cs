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
    class Menu
    {
        private Dictionary<string, Button> button = new Dictionary<string, Button>();

        public Menu(Rectangle window)
        {
            button.Add("Settings", new Button(
                new Point(window.Width / 2, 650),
                new Point(260, 70),
                "SETTINGS", FontLibrary.Normal,
                TextureLibrary.WhitePixel, null,
                new Dictionary<Button.State, Color> {
                    { Button.State.None, Color.White },
                    { Button.State.Hover, Color.FromNonPremultiplied(185, 215, 255, 255) },
                    { Button.State.Down, Color.FromNonPremultiplied(125, 160, 210, 255) },
                    { Button.State.Released, Color.White } },
                new Dictionary<Button.State, Color> {
                    { Button.State.None, Color.Black },
                    { Button.State.Hover, Color.Black },
                    { Button.State.Down, Color.Black },
                    { Button.State.Released, Color.Black } }
                ));
            
            button.Add("Back", new Button(
                new Point(window.Width / 2, 740),
                new Point(260, 70),
                "BACK", FontLibrary.Normal,
                TextureLibrary.WhitePixel, null,
                new Dictionary<Button.State, Color> {
                    { Button.State.None, Color.White },
                    { Button.State.Hover, Color.FromNonPremultiplied(185, 215, 255, 255) },
                    { Button.State.Down, Color.FromNonPremultiplied(125, 160, 210, 255) },
                    { Button.State.Released, Color.White } },
                new Dictionary<Button.State, Color> {
                    { Button.State.None, Color.Black },
                    { Button.State.Hover, Color.Black },
                    { Button.State.Down, Color.Black },
                    { Button.State.Released, Color.Black } }
                ));
        }

        public void Update(ref Screen screen, MouseState currentMouseState, MouseState previousMouseState, KeyboardState currentKeyboardState, KeyboardState previousKeyboardState)
        {
            foreach (KeyValuePair<string, Button> item in button)
                item.Value.Update(currentMouseState, previousMouseState);

            if (button["Settings"].IsReleased)
                screen = Screen.Settings;

            else if (button["Back"].IsReleased)
                screen = Screen.Start;
        }

        public void Draw(SpriteBatch spriteBatch, Rectangle window)
        {
            spriteBatch.Draw(TextureLibrary.BackgroundGray, window, Color.White);
            spriteBatch.DrawString(FontLibrary.Normal, "SELECT A SAVE FILE", new Vector2(window.Width / 2, 300), Color.White, 0, FontLibrary.Normal.MeasureString("SELECT A SAVE FILE") / 2, 1, SpriteEffects.None, 0);

            foreach (KeyValuePair<string, Button> item in button)
                item.Value.Draw(spriteBatch);
        }
    }
}

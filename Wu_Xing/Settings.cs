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
            button.Add("Resolution", new Button(
                new Point(window.Width / 2 + 140, window.Height / 2 - 270),
                new Point(260, 70),
                "1080p", FontLibrary.Normal,
                TextureLibrary.WhitePixel, null,
                ColorLibrary.WhiteButtonBackgroundColor,
                ColorLibrary.WhiteButtonLabelColor
                ));

            button.Add("Window", new Button(
                new Point(window.Width / 2 + 140, window.Height / 2 - 180),
                new Point(260, 70),
                "FULLSCREEN", FontLibrary.Normal,
                TextureLibrary.WhitePixel, null,
                ColorLibrary.WhiteButtonBackgroundColor,
                ColorLibrary.WhiteButtonLabelColor
                ));

            button.Add("Music", new Button(
                new Point(window.Width / 2 + 140, window.Height / 2 - 90),
                new Point(260, 70),
                "70%", FontLibrary.Normal,
                TextureLibrary.WhitePixel, null,
                ColorLibrary.WhiteButtonBackgroundColor,
                ColorLibrary.WhiteButtonLabelColor
                ));

            button.Add("Sound", new Button(
                new Point(window.Width / 2 + 140, window.Height / 2),
                new Point(260, 70),
                "100%", FontLibrary.Normal,
                TextureLibrary.WhitePixel, null,
                ColorLibrary.WhiteButtonBackgroundColor,
                ColorLibrary.WhiteButtonLabelColor
                ));

            button.Add("Map", new Button(
                new Point(window.Width / 2 + 140, window.Height / 2 + 90),
                new Point(260, 70),
                "70%", FontLibrary.Normal,
                TextureLibrary.WhitePixel, null,
                ColorLibrary.WhiteButtonBackgroundColor,
                ColorLibrary.WhiteButtonLabelColor
                ));

            button.Add("Default", new Button(
               new Point(window.Width / 2 - 140, window.Height / 2 + 270),
               new Point(260, 70),
               "DEFAULT", FontLibrary.Normal,
               TextureLibrary.WhitePixel, null,
               ColorLibrary.WhiteButtonBackgroundColor,
               ColorLibrary.WhiteButtonLabelColor
               ));

            button.Add("Back", new Button(
                new Point(window.Width / 2 + 140, window.Height / 2 + 270),
                new Point(260, 70),
                "BACK", FontLibrary.Normal,
                TextureLibrary.WhitePixel, null,
                ColorLibrary.WhiteButtonBackgroundColor,
                ColorLibrary.WhiteButtonLabelColor
                ));
        }

        public void Update(ref Screen screen, Screen previousScreen, Mouse mouse, KeyboardState currentKeyboard, KeyboardState previousKeyboard, GraphicsDeviceManager graphics)
        {
            if (currentKeyboard.IsKeyUp(Keys.Escape) && previousKeyboard.IsKeyDown(Keys.Escape))
                screen = previousScreen;

            foreach (KeyValuePair<string, Button> item in button)
                item.Value.Update(mouse);

            if (button["Resolution"].IsReleased)
            {
                if (button["Resolution"].Label == "1080p")
                {
                    //Change resolution to 2560 x 1440
                    button["Resolution"].Label = "1440p";
                }

                else if (button["Resolution"].Label == "1440p")
                {
                    //Change resolution to 3840 x 2160 (4K)
                    button["Resolution"].Label = "4K";
                }

                else if (button["Resolution"].Label == "4K")
                {
                    //Change resolution to 1920 x 1080
                    button["Resolution"].Label = "1080p";
                }

                button["Resolution"].UpdateLabelOrigin();
            }

            else if (button["Window"].IsReleased)
            {
                graphics.IsFullScreen = !graphics.IsFullScreen;
                graphics.ApplyChanges();
                button["Window"].Label = graphics.IsFullScreen ? "FULLSCREEN" : "WINDOWED";
                button["Window"].UpdateLabelOrigin();
                
            }

            else if (button["Music"].IsReleased)
            {
                int percentage = int.Parse(button["Music"].Label.Replace("%", ""));
                percentage = percentage == 100 ? 0 : percentage + 10;
                button["Music"].Label = percentage + "%";
                button["Music"].UpdateLabelOrigin();
                //Set music volume to percentage
            }

            else if (button["Sound"].IsReleased)
            {
                int percentage = int.Parse(button["Sound"].Label.Replace("%", ""));
                percentage = percentage == 100 ? 0 : percentage + 10;
                button["Sound"].Label = percentage + "%";
                button["Sound"].UpdateLabelOrigin();
                //Set sound volume to percentage
            }

            else if (button["Map"].IsReleased)
            {
                int percentage = int.Parse(button["Map"].Label.Replace("%", ""));
                percentage = percentage == 100 ? 0 : percentage + 10;
                button["Map"].Label = percentage + "%";
                button["Map"].UpdateLabelOrigin();
                //Set map opacity to percentage
            }

            else if (button["Default"].IsReleased)
            {
                //Apply default settings:
                //Set resolution to 1080p
                //Set window to fullscreen
                //Set music volume to 70%
                //Set sound volume to 100%
                //Set map opacity to 70%

                graphics.IsFullScreen = true;
                graphics.ApplyChanges();

                button["Resolution"].Label = "1080p";
                button["Resolution"].UpdateLabelOrigin();
                button["Window"].Label = "FULLSCREEN";
                button["Window"].UpdateLabelOrigin();
                button["Music"].Label = "70%";
                button["Music"].UpdateLabelOrigin();
                button["Sound"].Label = "100%";
                button["Sound"].UpdateLabelOrigin();
                button["Map"].Label = "70%";
                button["Map"].UpdateLabelOrigin();
            }

            else if (button["Back"].IsReleased)
            {
                screen = previousScreen;
            }  
        }

        public void Draw(SpriteBatch spriteBatch, Rectangle window)
        {
            spriteBatch.Draw(TextureLibrary.BackgroundGray, window, Color.White);
            spriteBatch.DrawString(FontLibrary.Normal, "SETTINGS", new Vector2(window.Width / 2, 150), Color.White, 0, FontLibrary.Normal.MeasureString("SETTINGS") / 2, 1, SpriteEffects.None, 0);

            spriteBatch.DrawString(FontLibrary.Normal, "RESOLUTION", new Vector2(window.Width / 2 - 270, button["Resolution"].Rectangle.Center.Y), Color.White, 0, new Vector2(0, FontLibrary.Normal.MeasureString("E").Y / 2), 1, SpriteEffects.None, 0);
            spriteBatch.DrawString(FontLibrary.Normal, "WINDOW", new Vector2(window.Width / 2 - 270, button["Window"].Rectangle.Center.Y), Color.White, 0, new Vector2(0, FontLibrary.Normal.MeasureString("E").Y / 2), 1, SpriteEffects.None, 0);
            spriteBatch.DrawString(FontLibrary.Normal, "MUSIC", new Vector2(window.Width / 2 - 270, button["Music"].Rectangle.Center.Y), Color.White, 0, new Vector2(0, FontLibrary.Normal.MeasureString("E").Y / 2), 1, SpriteEffects.None, 0);
            spriteBatch.DrawString(FontLibrary.Normal, "SOUND", new Vector2(window.Width / 2 - 270, button["Sound"].Rectangle.Center.Y), Color.White, 0, new Vector2(0, FontLibrary.Normal.MeasureString("E").Y / 2), 1, SpriteEffects.None, 0);
            spriteBatch.DrawString(FontLibrary.Normal, "MAP OPACITY", new Vector2(window.Width / 2 - 270, button["Map"].Rectangle.Center.Y), Color.White, 0, new Vector2(0, FontLibrary.Normal.MeasureString("E").Y / 2), 1, SpriteEffects.None, 0);

            foreach (KeyValuePair<string, Button> item in button)
                item.Value.Draw(spriteBatch);
        }
    }
}

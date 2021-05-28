using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
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
        private static float minimapOpacity;
        private static float masterVolume;
        private static float musicVolume;

        public static float MinimapOpacity { get { return minimapOpacity; } }
        public static float MasterVolume { get { return masterVolume; } }
        public static float MusicVolume { get { return musicVolume; } }

        public Settings(Rectangle window)
        {
            minimapOpacity = 0.7f;
            masterVolume = 1;
            musicVolume = 1;

            SoundEffect.MasterVolume = masterVolume;
            SoundLibrary.BackgroundMusicInstance.Volume = musicVolume;

            button.Add("Scaling", new Button(
                new Point(window.Width / 2 + 140, window.Height / 2 - 270),
                new Point(260, 70),
                "AUTOMATIC", FontLibrary.Normal,
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
                "100%", FontLibrary.Normal,
                TextureLibrary.WhitePixel, null,
                ColorLibrary.WhiteButtonBackgroundColor,
                ColorLibrary.WhiteButtonLabelColor
                ));

            button.Add("Master volume", new Button(
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

        public void Update(ref Screen screen, Screen previousScreen, Mouse mouse, KeyboardState currentKeyboard, KeyboardState previousKeyboard, GraphicsDeviceManager graphics, Rectangle window, ref Rectangle resolution, ref float windowScale)
        {
            if (currentKeyboard.IsKeyUp(Keys.Escape) && previousKeyboard.IsKeyDown(Keys.Escape))
                screen = previousScreen;

            foreach (KeyValuePair<string, Button> item in button)
                item.Value.Update(mouse);

            if (button["Scaling"].IsReleased)
            {
                if (button["Scaling"].Label == "AUTOMATIC")
                {
                    button["Scaling"].Label = "NONE";
                    resolution = window;
                }

                else if (button["Scaling"].Label == "NONE")
                {
                    button["Scaling"].Label = "AUTOMATIC";
                    resolution.Width = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                    resolution.Height = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                }

                graphics.PreferredBackBufferWidth = resolution.Width;
                graphics.PreferredBackBufferHeight = resolution.Height;
                graphics.ApplyChanges();

                windowScale = (float)resolution.Height / resolution.Width >= (float)window.Height / window.Width ? (float)resolution.Width / window.Width : (float)resolution.Height / window.Height;

                button["Scaling"].UpdateLabelOrigin();
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
                IncrementPercentage(button["Music"], ref musicVolume);
                SoundLibrary.BackgroundMusicInstance.Volume = musicVolume;
            }

            else if (button["Master volume"].IsReleased)
            {
                IncrementPercentage(button["Master volume"], ref musicVolume);
                SoundEffect.MasterVolume = masterVolume;
            }

            else if (button["Map"].IsReleased)
            {
                IncrementPercentage(button["Map"], ref musicVolume);
            }

            else if (button["Default"].IsReleased)
            {
                //Resets buttons
                button["Scaling"].Label = "AUTOMATIC";
                button["Scaling"].UpdateLabelOrigin();
                button["Window"].Label = "FULLSCREEN";
                button["Window"].UpdateLabelOrigin();
                button["Music"].Label = "100%";
                button["Music"].UpdateLabelOrigin();
                button["Master volume"].Label = "100%";
                button["Master volume"].UpdateLabelOrigin();
                button["Map"].Label = "70%";
                button["Map"].UpdateLabelOrigin();

                //Resets Scaling
                resolution.Width = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                resolution.Height = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                graphics.PreferredBackBufferWidth = resolution.Width;
                graphics.PreferredBackBufferHeight = resolution.Height;
                windowScale = (float)resolution.Height / resolution.Width >= (float)window.Height / window.Width ? (float)resolution.Width / window.Width : (float)resolution.Height / window.Height;

                //Resets Window
                graphics.IsFullScreen = true;
                graphics.ApplyChanges();

                //Resets Music
                musicVolume = 1;
                SoundLibrary.BackgroundMusicInstance.Volume = musicVolume;

                //Resets SoundVolume
                masterVolume = 1;
                SoundEffect.MasterVolume = masterVolume;
            }

            else if (button["Back"].IsReleased)
            {
                screen = previousScreen;
            }  
        }

        private void IncrementPercentage(Button button, ref float percentageVariable)
        {
            int percentage = int.Parse(button.Label.Replace("%", ""));
            percentage = percentage == 100 ? 0 : percentage + 10;
            button.Label = percentage + "%";
            button.UpdateLabelOrigin();
            percentageVariable = percentage / 100f;
        }

        public void Draw(SpriteBatch spriteBatch, Rectangle window)
        {
            spriteBatch.Draw(TextureLibrary.BackgroundGray, window, Color.White);
            spriteBatch.DrawString(FontLibrary.Normal, "SETTINGS", new Vector2(window.Width / 2, 150), Color.White, 0, FontLibrary.Normal.MeasureString("SETTINGS") / 2, 1, SpriteEffects.None, 0);

            spriteBatch.DrawString(FontLibrary.Normal, "SCALING", new Vector2(window.Width / 2 - 270, button["Scaling"].Rectangle.Center.Y), Color.White, 0, new Vector2(0, FontLibrary.Normal.MeasureString("E").Y / 2), 1, SpriteEffects.None, 0);
            spriteBatch.DrawString(FontLibrary.Normal, "WINDOW", new Vector2(window.Width / 2 - 270, button["Window"].Rectangle.Center.Y), Color.White, 0, new Vector2(0, FontLibrary.Normal.MeasureString("E").Y / 2), 1, SpriteEffects.None, 0);
            spriteBatch.DrawString(FontLibrary.Normal, "MUSIC", new Vector2(window.Width / 2 - 270, button["Music"].Rectangle.Center.Y), Color.White, 0, new Vector2(0, FontLibrary.Normal.MeasureString("E").Y / 2), 1, SpriteEffects.None, 0);
            spriteBatch.DrawString(FontLibrary.Normal, "MASTER VOLUME", new Vector2(window.Width / 2 - 270, button["Master volume"].Rectangle.Center.Y), Color.White, 0, new Vector2(0, FontLibrary.Normal.MeasureString("E").Y / 2), 1, SpriteEffects.None, 0);
            spriteBatch.DrawString(FontLibrary.Normal, "MAP OPACITY", new Vector2(window.Width / 2 - 270, button["Map"].Rectangle.Center.Y), Color.White, 0, new Vector2(0, FontLibrary.Normal.MeasureString("E").Y / 2), 1, SpriteEffects.None, 0);

            foreach (KeyValuePair<string, Button> item in button)
                item.Value.Draw(spriteBatch);
        }
    }
}

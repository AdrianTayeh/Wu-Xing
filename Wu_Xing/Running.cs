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
        private bool paused;

        private float seconds;
        private int minutes;
        private int hours;

        private Dictionary<string, Button> button = new Dictionary<string, Button>();

        private Map map;
        private Point room;
        //private List<GameObject> gameObjects = new List<GameObject>();

        public Running(Rectangle window)
        {
            adam = new Adam(window);

            button.Add("Continue", new Button(
                new Point(window.Width / 2, window.Height / 2 - 90),
                new Point(260, 70),
                "CONTINUE", FontLibrary.Normal,
                TextureLibrary.WhitePixel, null,
                ColorLibrary.WhiteButtonBackgroundColor,
                ColorLibrary.WhiteButtonLabelColor
                ));

            button.Add("Settings", new Button(
                new Point(window.Width / 2, window.Height / 2),
                new Point(260, 70),
                "SETTINGS", FontLibrary.Normal,
                TextureLibrary.WhitePixel, null,
                ColorLibrary.WhiteButtonBackgroundColor,
                ColorLibrary.WhiteButtonLabelColor
                ));

            button.Add("Menu", new Button(
                new Point(window.Width / 2, window.Height / 2 + 90),
                new Point(260, 70),
                "MENU", FontLibrary.Normal,
                TextureLibrary.WhitePixel, null,
                ColorLibrary.WhiteButtonBackgroundColor,
                ColorLibrary.WhiteButtonLabelColor
                ));
        }

        public bool MapInitialized { get { return map != null; } }

        public void InitializeNewMap(Random random, int size, Element element)
        {
            map = new Map(random, size, element);
        }

        public void Update(ref Screen screen, ref Screen previousScreen, Mouse mouse, KeyboardState currentKeyboard, KeyboardState previousKeyboard, GameTime gameTime, Random random)
        {
            if (currentKeyboard.IsKeyUp(Keys.Escape) && previousKeyboard.IsKeyDown(Keys.Escape))
                paused = !paused;

            //Running
            if (!paused)
            {
                adam.Update(currentKeyboard);
                UpdateTimer(gameTime);
            }

            //Paused
            else
            {
                foreach (KeyValuePair<string, Button> item in button)
                    item.Value.Update(mouse);

                if (button["Continue"].IsReleased)
                {
                    paused = false;
                }
                    
                else if (button["Settings"].IsReleased)
                {
                    previousScreen = screen;
                    screen = Screen.Settings;
                }
                    
                else if (button["Menu"].IsReleased)
                {
                    screen = Screen.Pregame;
                }  
            }
        }

        private void UpdateTimer(GameTime gameTime)
        {
            seconds += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (seconds >= 60)
            {
                seconds %= 60;
                minutes += 1;

                if (minutes == 60)
                {
                    minutes = 0;
                    hours += 1;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, Rectangle window)
        {
            map.DrawWorld(spriteBatch);
            adam.Draw(spriteBatch);
            map.DrawFullMap(spriteBatch);
                
            if (paused)
            {
                spriteBatch.Draw(TextureLibrary.WhitePixel, window, Color.FromNonPremultiplied(0, 0, 0, 150));
                spriteBatch.DrawString(FontLibrary.Normal, "PAUSED", new Vector2(window.Width / 2, 190), Color.White, 0, FontLibrary.Normal.MeasureString("PAUSED") / 2, 1, SpriteEffects.None, 0);

                string time = hours + ":" + minutes.ToString("00") + ":" + seconds.ToString("00");
                spriteBatch.DrawString(FontLibrary.Huge, time, new Vector2(window.Width / 2, 250), Color.White, 0, FontLibrary.Huge.MeasureString(time) / 2, 1, SpriteEffects.None, 0);

                foreach (KeyValuePair<string, Button> item in button)
                    item.Value.Draw(spriteBatch);
            }
        }
    }
}

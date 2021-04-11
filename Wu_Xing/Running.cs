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

        private Room[,] rooms;
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

        public bool MapInitialized { get { return rooms != null; } }

        public void InitializeNewMap(Random random, int size)
        {
            rooms = new Map().GenerateNewMap(random, size);
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
            spriteBatch.Draw(TextureLibrary.RoomBlack1x1, window.Center.ToVector2(), null, Color.White, 0, TextureLibrary.RoomBlack1x1.Bounds.Size.ToVector2() / 2, 1, SpriteEffects.None, 0);
            adam.Draw(spriteBatch);

            for (int y = 0; y < rooms.GetLength(0); y++)
            {
                for (int x = 0; x < rooms.GetLength(0); x++)
                {
                    if (rooms[x, y] != null)
                    {
                        spriteBatch.Draw(TextureLibrary.WhitePixel, new Rectangle(50 + x * 30, 50 + y * 30, 30 * rooms[x, y].Size.X - 4, 30 * rooms[x, y].Size.Y - 4), rooms[x, y].RoomType == Room.Type.Normal ? Color.White : rooms[x, y].RoomType == Room.Type.Boss ? Color.Red : Color.Blue);
                    }
                }
            }
                
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

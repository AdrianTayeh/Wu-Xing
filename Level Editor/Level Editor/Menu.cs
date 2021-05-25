using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Level_Editor
{
    class Menu
    {
        private Dictionary<string, Button> button = new Dictionary<string, Button>();

        public Menu(Rectangle window)
        {
            button.Add("1x1", new Button(
                new Point(window.Width / 3 + 70, window.Height / 2),
                new Point(90, 90),
                "1x1", FontLibrary.Big,
                TextureLibrary.WhitePixel, null,
                ColorLibrary.WhiteButtonBackgroundColor,
                ColorLibrary.WhiteButtonLabelColor
                ));

            button.Add("2x1", new Button(
                new Point(window.Width / 3 + 100*1 + 70, window.Height / 2),
                new Point(90, 90),
                "2x1", FontLibrary.Big,
                TextureLibrary.WhitePixel, null,
                ColorLibrary.WhiteButtonBackgroundColor,
                ColorLibrary.WhiteButtonLabelColor
                ));

            button.Add("3x1", new Button(
                new Point(window.Width / 3 + 100 * 2 + 70, window.Height / 2),
                new Point(90, 90),
                "3x1", FontLibrary.Big,
                TextureLibrary.WhitePixel, null,
                ColorLibrary.WhiteButtonBackgroundColor,
                ColorLibrary.WhiteButtonLabelColor
                ));

            button.Add("1x2", new Button(
                new Point(window.Width / 3 + 100 * 3 + 70, window.Height / 2),
                new Point(90, 90),
                "1x2", FontLibrary.Big,
                TextureLibrary.WhitePixel, null,
                ColorLibrary.WhiteButtonBackgroundColor,
                ColorLibrary.WhiteButtonLabelColor
                ));

            button.Add("1x3", new Button(
                new Point(window.Width / 3 + 100 * 4 + 70, window.Height / 2),
                new Point(90, 90),
                "1x3", FontLibrary.Big,
                TextureLibrary.WhitePixel, null,
                ColorLibrary.WhiteButtonBackgroundColor,
                ColorLibrary.WhiteButtonLabelColor
                ));

            button.Add("2x2", new Button(
                new Point(window.Width / 3 + 100 * 5 + 70, window.Height / 2),
                new Point(90, 90),  
                "2x2", FontLibrary.Big,
                TextureLibrary.WhitePixel, null,
                ColorLibrary.WhiteButtonBackgroundColor,
                ColorLibrary.WhiteButtonLabelColor
                ));
        }

        public void Update(ref Game1.RoomSize roomSize, Mouse mouse, ref Game1.Screen screen)
        {
            foreach (KeyValuePair<string, Button> item in button)
                item.Value.Update(mouse);

            if (button["1x1"].IsReleased)
            {
                roomSize = Game1.RoomSize.OneXOne;
                screen = Game1.Screen.Map;
            }
            else if (button["2x1"].IsReleased)
            {
                roomSize = Game1.RoomSize.TwoXOne;
                screen = Game1.Screen.Map;
            }
            else if (button["3x1"].IsReleased)
            {
                roomSize = Game1.RoomSize.ThreeXOne;
                screen = Game1.Screen.Map;
            }
            else if (button["1x2"].IsReleased)
            {
                roomSize = Game1.RoomSize.OneXTwo;
                screen = Game1.Screen.Map;
            }
            else if (button["1x3"].IsReleased)
            {
                roomSize = Game1.RoomSize.OneXThree;
                screen = Game1.Screen.Map;
            }
            else if (button["2x2"].IsReleased)
            {
                roomSize = Game1.RoomSize.TwoXTwo;
                screen = Game1.Screen.Map;
            }
        }

        public void Draw(SpriteBatch spriteBatch, Rectangle window)
        {
            spriteBatch.Draw(TextureLibrary.BackgroundGray, window, Color.White);
            spriteBatch.DrawString(FontLibrary.Normal, "SELECT A ROOMSIZE", new Vector2(window.Width / 2, 300), Color.White, 0, FontLibrary.Normal.MeasureString("SELECT A ROOMSIZE") / 2, 1, SpriteEffects.None, 0);

            foreach (KeyValuePair<string, Button> item in button)
                item.Value.Draw(spriteBatch);
        }

    }
}

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
                new Point(window.Width / 3, window.Height / 2),
                new Point(260, 70),
                "1x1", FontLibrary.Normal,
                TextureLibrary.WhitePixel, null,
                ColorLibrary.WhiteButtonBackgroundColor,
                ColorLibrary.WhiteButtonLabelColor
                ));

            button.Add("2x1", new Button(
                new Point(2 * window.Width / 3, window.Height / 2),
                new Point(260, 70),
                "2x1", FontLibrary.Normal,
                TextureLibrary.WhitePixel, null,
                ColorLibrary.WhiteButtonBackgroundColor,
                ColorLibrary.WhiteButtonLabelColor
                ));

            button.Add("3x1", new Button(
                new Point(window.Width, window.Height / 2),
                new Point(260, 70),
                "3x1", FontLibrary.Normal,
                TextureLibrary.WhitePixel, null,
                ColorLibrary.WhiteButtonBackgroundColor,
                ColorLibrary.WhiteButtonLabelColor
                ));

            button.Add("1x2", new Button(
                new Point(window.Width / 3, 2* window.Height / 3),
                new Point(260, 70),
                "1x2", FontLibrary.Normal,
                TextureLibrary.WhitePixel, null,
                ColorLibrary.WhiteButtonBackgroundColor,
                ColorLibrary.WhiteButtonLabelColor
                ));

            button.Add("1x3", new Button(
                new Point(2*window.Width / 3, 2* window.Height/ 3),
                new Point(260, 70),
                "1x3", FontLibrary.Normal,
                TextureLibrary.WhitePixel, null,
                ColorLibrary.WhiteButtonBackgroundColor,
                ColorLibrary.WhiteButtonLabelColor
                ));

            button.Add("2x2", new Button(
                new Point(window.Width, 2 * window.Height/3),
                new Point(260, 70),
                "2x2", FontLibrary.Normal,
                TextureLibrary.WhitePixel, null,
                ColorLibrary.WhiteButtonBackgroundColor,
                ColorLibrary.WhiteButtonLabelColor
                ));
        }

        public void Update(Mouse mouse)
        {
            foreach (KeyValuePair<string, Button> item in button)
                item.Value.Update(mouse);

            if (button["1x1"].IsReleased)
            {

            }
            else if (button["2x1"].IsReleased)
            {

            }
            else if (button["3x1"].IsReleased)
            {

            }
            else if (button["1x2"].IsReleased)
            {

            }
            else if (button["1x3"].IsReleased)
            {

            }
            else if (button["2x2"].IsReleased)
            {

            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (KeyValuePair<string, Button> item in button)
                item.Value.Draw(spriteBatch);
        }

    }
}

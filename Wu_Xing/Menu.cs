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
        private bool deleteMode;

        public Menu(Rectangle window)
        {
            button.Add("Settings", new Button(
                new Point(window.Width / 2, 650),
                new Point(260, 70),
                "SETTINGS", FontLibrary.Normal,
                TextureLibrary.WhitePixel, null,
                ColorLibrary.WhiteButtonBackgroundColor,
                ColorLibrary.WhiteButtonLabelColor
                ));
            
            button.Add("Back", new Button(
                new Point(window.Width / 2, 740),
                new Point(260, 70),
                "BACK", FontLibrary.Normal,
                TextureLibrary.WhitePixel, null,
                ColorLibrary.WhiteButtonBackgroundColor,
                ColorLibrary.WhiteButtonLabelColor
                ));

            //Look for save files
            int nrOfSaveFiles = 3;

            //Add a button for each save file
            for (int i = 0; i < nrOfSaveFiles; i++)
            {
                button.Add((i + 1).ToString(), new Button(
                new Point(window.Width / 2 - 55 * (nrOfSaveFiles + 1) + 110 * i, 425),
                new Point(90, 90),
                (i + 1).ToString(), FontLibrary.Big,
                TextureLibrary.WhitePixel, null,
                ColorLibrary.WhiteButtonBackgroundColor,
                ColorLibrary.WhiteButtonLabelColor
                ));
            }
            
            button.Add("Plus", new Button(
                new Point(window.Width / 2 - 55 * (nrOfSaveFiles + 1) + 110 * nrOfSaveFiles, 425),
                new Point(90, 90),
                "", null,
                TextureLibrary.WhitePixel, TextureLibrary.IconPlus,
                ColorLibrary.GreenButtonBackgroundColor,
                null
                ));

            button.Add("Delete", new Button(
                new Point(window.Width / 2 - 55 * (nrOfSaveFiles + 1) + 110 * (nrOfSaveFiles + 1), 425),
                new Point(90, 90),
                "", null,
                TextureLibrary.WhitePixel, TextureLibrary.IconDelete,
                ColorLibrary.RedButtonBackgroundColor,
                null
                ));
        }

        public void Update(ref Screen screen, ref Screen previousScreen, Mouse mouse, KeyboardState currentKeyboard, KeyboardState previousKeyboard)
        {
            if (currentKeyboard.IsKeyUp(Keys.Escape) && previousKeyboard.IsKeyDown(Keys.Escape))
                screen = Screen.Start;

            foreach (KeyValuePair<string, Button> item in button)
                item.Value.Update(mouse);

            if (button["Settings"].IsReleased)
            {
                previousScreen = screen;
                screen = Screen.Settings;
            }   

            else if (button["Back"].IsReleased)
            {
                screen = Screen.Start;
            }   

            else if (button["Plus"].IsReleased)
            {
                //Add new save file
            }  

            else if (button["Delete"].IsReleased)
            {
                deleteMode = !deleteMode;
                button["Delete"].BackgroundColor = deleteMode ? ColorLibrary.WhiteButtonBackgroundColor : ColorLibrary.RedButtonBackgroundColor;

                for (int i = 1; i < button.Count - 3; i++)
                    button[i.ToString()].BackgroundColor = deleteMode ? ColorLibrary.RedButtonBackgroundColor : ColorLibrary.WhiteButtonBackgroundColor;
            }
            
            for (int i = 1; i < button.Count - 3; i++)
            {
                if (button[i.ToString()].IsReleased)
                {
                    if (deleteMode)
                    {
                        //Delete save file "i"
                    }

                    else
                    {
                        //Load save file "i"
                        screen = Screen.Pregame;
                    }
                }
            }
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

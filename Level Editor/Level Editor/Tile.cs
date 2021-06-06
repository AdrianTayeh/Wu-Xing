using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Level_Editor
{
    class Tile
    {
        private List<string> tileList;
        private int mouseClicks;
        private string tileString;
        private bool checkClick;
        private Vector2 mousePosition;
        private Vector2 worldPos;
        private string tileIndex;
        public List<Tuple<string, Vector2, string>> tilePosList;
        public List<int> tilePosX;
        private KeyboardState currentKeyboardState;

        public Tile()
        {
            tileList = new List<string>() { "Conveyor", "Fire", "Metal Box", "Spikes", "Stone", "Water Hole", "Wood Box", "Hole", "Orb", "Soul", "Wanderer" };
            tilePosList = new List<Tuple<string, Vector2, string>>();
            tilePosX = new List<int>();
            currentKeyboardState = Microsoft.Xna.Framework.Input.Keyboard.GetState();
            mouseClicks = 0;
        }
        
        public void Update(ref Game1.Screen screen, Mouse mouse, Camera camera)
        {
            mousePosition = mouse.Position.ToVector2();
            worldPos = camera.MousePositionInWorld(mouse);
            if (mouse.RightIsPressed)
                mouseClicks += 1;

            switch (mouseClicks)
            {
                case 0:
                    tileString = "None";
                    break;

                case 1:
                    tileString = "Conveyor";
                    tileIndex = "C";
                    break;

                case 2:
                    tileString = "Fire";
                    tileIndex = "F";
                    break;

                case 3:
                    tileString = "Metal Box";
                    tileIndex = "M";
                    break;

                case 4:
                    tileString = "Spikes";
                    tileIndex = "SP";
                    break;

                case 5:
                    tileString = "Stone";
                    tileIndex = "S";
                    break;

                case 6:
                    tileString = "Water Hole";
                    tileIndex = "WH";
                    break;

                case 7:
                    tileString = "Wood Box";
                    tileIndex = "W";
                    break;

                case 8:
                    tileString = "Hole";
                    tileIndex = "H";
                    break;

                case 9:
                    tileString = "Orb";
                    tileIndex = "O";
                    break;

                case 10:
                    tileString = "Soul";
                    tileIndex = "S";
                    break;

                case 11:
                    tileString = "Wanderer";
                    tileIndex = "W";
                    break;
            }
            checkClick = mouse.LeftIsPressed;

            if (checkClick)
                tilePosList.Add(new Tuple<string, Vector2, string>(tileString, worldPos, tileIndex));

            mouseClicks = mouseClicks >= 12 ? 0 : mouseClicks;
        }

        //Uses a camera matrix
        public void DrawWorld(SpriteBatch spriteBatch)
        {
            foreach (Tuple<string, Vector2, string> item in tilePosList)
            {
                spriteBatch.DrawString(FontLibrary.Big, item.Item1, item.Item2, Color.White);
                tilePosX.Add((int)(item.Item2.X - 190) / 100);
            }
        }

        //Does not use camera metrix
        public void DrawHUD(SpriteBatch spriteBatch)
        {
            if (mouseClicks > 0)
                spriteBatch.DrawString(FontLibrary.Big, tileString, mousePosition + new Vector2(20, -8), Color.White);

            spriteBatch.DrawString(FontLibrary.Big, "Press Ctrl + S to initiate Saving", new Vector2(50, 0), Color.White);
            spriteBatch.DrawString(FontLibrary.Big, "Ctrl + 1 = Save to Center Room (No enemies)", new Vector2(50, 50), Color.White);
            spriteBatch.DrawString(FontLibrary.Big, "Ctrl + 2 = Save to Normal Room", new Vector2(50, 100), Color.White);
            spriteBatch.DrawString(FontLibrary.Big, "Ctrl + 3 = Save to Boss Room (Only 1x1 rooms)", new Vector2(50, 150), Color.White);
        }
    }
}

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
        private int mouseClick;
        private string tileString;
        private bool checkClick;
        private MouseState mousePos;
        private Vector2 mousePosition;


        public Tile()
        {
            tileList = new List<string>() { "Conveyor", "Fire", "Metal Box", "Spikes", "Stone", "Water Hole", "Wood Box", "Hole", "Orb", "Soul", "Wanderer" };
            mouseClick = 0;
        }
        
        public void Update(ref Game1.Screen screen, Mouse mouse)
        {
            mousePos = Microsoft.Xna.Framework.Input.Mouse.GetState();
            mousePosition = new Vector2(mousePos.X, mousePos.Y);
            if (mouse.RightIsPressed)
                mouseClick += 1;


            switch (mouseClick)
            {
                case 0:
                    tileString = string.Empty;
                    break;

                case 1:
                    tileString = "Conveyor";
                    break;

                case 2:
                    tileString = "Fire";
                    break;

                case 3:
                    tileString = "Metal Box";
                    break;

                case 4:
                    tileString = "Spikes";
                    break;

                case 5:
                    tileString = "Stone";
                    break;

                case 6:
                    tileString = "Water Hole";
                    break;

                case 7:
                    tileString = "Wood Box";
                    break;

                case 8:
                    tileString = "Hole";
                    break;

                case 9:
                    tileString = "Orb";
                    break;

                case 10:
                    tileString = "Soul";
                    break;

                case 11:
                    tileString = "Wanderer";
                    break;
            }

            if (mouseClick >= 12)
                mouseClick = 0;

            if (mouse.LeftIsPressed)
            {
                checkClick = true;
            }
            else
            {
                checkClick = false;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if(checkClick)
                spriteBatch.DrawString(FontLibrary.Big, tileString, mousePosition, Color.White);
        }
    }
}

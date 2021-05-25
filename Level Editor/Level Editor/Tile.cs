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

        public Tile()
        {
            tileList = new List<string>() { "Conveyor", "Fire", "Metal Box", "Spikes", "Stone", "Water Hole", "Wood Box", "Hole", "Orb", "Soul", "Wanderer" };
            mouseClicks = 0;
        }
        
        public void Update(ref Game1.Screen screen, Mouse mouse)
        {
            mousePosition = mouse.Position.ToVector2();

            if (mouse.RightIsPressed)
                mouseClicks += 1;

            switch (mouseClicks)
            {
                case 0:
                    tileString = "None";
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

            mouseClicks = mouseClicks >= 12 ? 0 : mouseClicks;
            checkClick = mouse.LeftIsPressed || mouse.LeftIsHeld;
        }

        //Uses a camera matrix
        public void DrawWorld(SpriteBatch spriteBatch)
        {

        }

        //Does not use camera metrix
        public void DrawHUD(SpriteBatch spriteBatch)
        {
            if (checkClick)
                spriteBatch.DrawString(FontLibrary.Big, tileString, mousePosition + new Vector2(20, -20), Color.White);
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Level_Editor
{
    class SaveRoom
    {
        private string roomSize;
        private string x;
        private string y;
        private List <int> tilePosX;
        private List <int> tilePosY;
        private List<string> tileIndex;
        private List<string> tileName;
        private Tile tile;
        private int length;
        private StreamWriter sw;

        public SaveRoom(ref Game1.RoomSize roomSize)
        {
            this.roomSize = roomSize.ToString();
            tilePosX = new List<int>();
            tilePosY = new List<int>();
            tileName = new List<string>();
            tileIndex = new List<string>();
            length = 1;
            tile = new Tile();
            
        }

        public void Save(KeyboardState currentKeyboard)
        {
            foreach(Tuple<string, Vector2, string> item in tile.tilePosList)
            {
                if(item.Item1 != "None")
                {
                    tilePosX.Add((int)(item.Item2.X - 190) / 100);
                    tilePosY.Add((int)(item.Item2.Y - 190) / 100);
                    tileIndex.Add(item.Item3);
                    tileName.Add(item.Item1);
                }

            }

            for (int i = 0; i < tileIndex.Count; i++)
            {
                if (tilePosY[i] == tilePosY[i + 1])
                    length += 1;
                else
                    length = 1;

                if (currentKeyboard.IsKeyDown(Keys.LeftControl) && currentKeyboard.IsKeyDown(Keys.D1) && !tileName.Contains("Orb") || !tileName.Contains("Soul") || !tileName.Contains("Wanderer"))
                    sw = File.AppendText("Center " + roomSize + ".txt");

                else if (currentKeyboard.IsKeyDown(Keys.LeftControl) && currentKeyboard.IsKeyDown(Keys.D2))
                    sw = File.AppendText("Normal " + roomSize + ".txt");

                else if (currentKeyboard.IsKeyDown(Keys.LeftControl) && currentKeyboard.IsKeyDown(Keys.D3) && roomSize == "1x1")
                    sw = File.AppendText("Boss " + roomSize + ".txt");

                if(!tileName.Contains("Orb") || !tileName.Contains("Soul") || !tileName.Contains("Wanderer"))
                {
                    sw.Write(tilePosX[i] + "," + tilePosY[i] + "," + tileIndex[i] + "," + length + ";");
                }

                else
                {

                }
            }
        }
    }
}

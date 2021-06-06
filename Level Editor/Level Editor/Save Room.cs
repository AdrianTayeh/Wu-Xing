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
        private string x;
        private string y;
        private List <int> tilePosX;
        private List <int> tilePosY;
        private List<string> tileIndex;
        private List<string> tileName;
        private Tile tile;
        private int length;
        private StreamWriter sw;
        private string size;
        private bool writeCenter;
        private bool writeNormal;
        private bool writeBoss;

        public SaveRoom()
        {
            tilePosX = new List<int>();
            tilePosY = new List<int>();
            tileName = new List<string>();
            tileIndex = new List<string>();
            length = 1;
            tile = new Tile();
            
        }

        public void Save(KeyboardState currentKeyboard, ref List<Tuple<string, Vector2, string>> tilePosList, ref Game1.RoomSize roomSize)
        {
            if (roomSize == Game1.RoomSize.OneXOne)
                size = "1x1";
            else if (roomSize == Game1.RoomSize.OneXTwo)
                size = "1x2";
            else if (roomSize == Game1.RoomSize.OneXThree)
                size = "1x3";
            else if (roomSize == Game1.RoomSize.TwoXOne)
                size = "2x1";
            else if (roomSize == Game1.RoomSize.TwoXTwo)
                size = "2x2";
            else if (roomSize == Game1.RoomSize.ThreeXOne)
                size = "3x1";
            foreach(Tuple<string, Vector2, string> item in tilePosList.Where(item => item.Item1 != "None"))
            {
                tilePosX.Add((int)(item.Item2.X - 190) / 100);
                tilePosY.Add((int)(item.Item2.Y - 190) / 100);
                tileIndex.Add(item.Item3);
                tileName.Add(item.Item1);
            }

            for (int i = 0; i < tileIndex.Count - 1; i++)
            {
                if (tileName[i] == tileName[i + 1] && tilePosY[i] == tilePosY[i + 1])
                    length++;
                if (currentKeyboard.IsKeyDown(Keys.LeftControl) && currentKeyboard.IsKeyDown(Keys.D1) && (!tileName.Contains("Orb") || !tileName.Contains("Soul") || !tileName.Contains("Wanderer")))
                {
                    sw = File.AppendText("Center " + size + ".txt");
                    sw.Write(tilePosX[i] + "," + tilePosY[i] + "," + tileIndex[i] + "," + length + ";");
                    sw.Close();
                }
                else if (currentKeyboard.IsKeyDown(Keys.LeftControl) && currentKeyboard.IsKeyDown(Keys.D2))
                {
                    sw = File.AppendText("Normal " + size + ".txt");
                    if(tileName.Contains("Orb") || tileName.Contains("Soul") || tileName.Contains("Wanderer"))
                    {
                        sw.Write(tilePosX[i] + "," + tilePosY[i] + "," + tileIndex[i] + ";");
                    }
                    else
                    {
                        sw.Write(tilePosX[i] + "," + tilePosY[i] + "," + tileIndex[i] + "," + length + ";");
                    }
                    sw.Close();
                }

                else if (currentKeyboard.IsKeyDown(Keys.LeftControl) && currentKeyboard.IsKeyDown(Keys.D3) && size == "1x1")
                {
                    sw = File.AppendText("Boss " + size + ".txt");
                    if (tileName.Contains("Orb") || tileName.Contains("Soul") || tileName.Contains("Wanderer"))
                    {
                        sw.Write(tilePosX[i] + "," + tilePosY[i] + "," + tileIndex[i] + ";");
                    }
                    else
                    {
                        sw.Write(tilePosX[i] + "," + tilePosY[i] + "," + tileIndex[i] + "," + length + ";");
                    }
                        sw.Close();
                }

            }
        }
    }
}

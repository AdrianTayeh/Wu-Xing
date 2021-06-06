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
        private int length;
        private StreamWriter sw;
        private string size;
        private int i;

        public SaveRoom()
        {
            length = 1;            
        }

        public void Save(KeyboardState currentKeyboard, ref List<Tuple<string, Vector2, string>> tilePosList, ref Game1.RoomSize roomSize, ref List<int>tilePosX)
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

            i = 0;
            foreach(Tuple<string, Vector2, string> item in tilePosList.Where(item => item.Item1 != "None"))
            {
                if (i + 1 < tilePosList.Count && item.Item1[i] == item.Item1[i + 1] && tilePosX[i] == tilePosX[i + 1])
                    length++;

                if (currentKeyboard.IsKeyDown(Keys.LeftControl) && currentKeyboard.IsKeyDown(Keys.D1) && (!item.Item1.Contains("Orb") || !item.Item1.Contains("Soul") || !item.Item1.Contains("Wanderer")))
                {
                    sw = File.AppendText("Center " + size + ".txt");
                    sw.Write((int)((item.Item2.X - 190)/100) + "," + (int)((item.Item2.Y - 190)/100) + "," + item.Item3 + "," + length + ";");
                    sw.Close();
                }
                else if (currentKeyboard.IsKeyDown(Keys.LeftControl) && currentKeyboard.IsKeyDown(Keys.D2))
                {
                    sw = File.AppendText("Normal " + size + ".txt");
                    if (item.Item1.Contains("Orb") || item.Item1.Contains("Soul") || item.Item1.Contains("Wanderer"))
                    {
                        sw.Write((int)((item.Item2.X - 190) / 100) + "," + (int)((item.Item2.Y - 190) / 100) + "," + item.Item3 + ";");
                    }
                    else
                    {
                        sw.Write((int)((item.Item2.X - 190) / 100) + "," + (int)((item.Item2.Y - 190) / 100) + "," + item.Item3 + "," + length + ";");
                    }
                    sw.Close();
                }

                else if (currentKeyboard.IsKeyDown(Keys.LeftControl) && currentKeyboard.IsKeyDown(Keys.D3) && size == "1x1")
                {
                    sw = File.AppendText("Boss " + size + ".txt");
                    if (item.Item1.Contains("Orb") || item.Item1.Contains("Soul") || item.Item1.Contains("Wanderer"))
                    {
                        sw.Write((int)((item.Item2.X - 190) / 100) + "," + (int)((item.Item2.Y - 190) / 100) + "," + item.Item3 + ";");
                    }
                    else
                    {
                        sw.Write((int)((item.Item2.X - 190) / 100) + "," + (int)((item.Item2.Y - 190) / 100) + "," + item.Item3 + "," + length + ";");
                    }
                    sw.Close();
                }
                i++;
            }

        }
    }
}

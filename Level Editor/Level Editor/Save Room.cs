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

            
            //Create a local list of Tuples from tilePosList, excluding "None"s
            List<Tuple<string, Vector2, string>> positions = new List<Tuple<string, Vector2, string>>();

            foreach (Tuple<string, Vector2, string> item in tilePosList)
                if (item.Item1 != "None")
                    positions.Add(item);

            for(int i = 0; i < positions.Count; i++)
            {
                for(int j = 1; i + j < positions.Count; j++)
                {
                    if (positions[i].Item1 == positions[i+j].Item1 && (int)((positions[i].Item2.X - 190) / 100) + j == (int)((positions[i + j].Item2.X - 190) / 100))
                        length += 1;
                    else
                        break;
                }
                if (currentKeyboard.IsKeyDown(Keys.LeftControl) && currentKeyboard.IsKeyDown(Keys.D1) && (!positions[i].Item1.Contains("Orb") || !positions[i].Item1.Contains("Soul") || !positions[i].Item1.Contains("Wanderer")))
                {
                    sw = File.AppendText("Center " + size + ".txt");
                    sw.Write((int)((positions[i].Item2.X - 190) / 100) + "," + (int)((positions[i].Item2.Y - 190) / 100) + "," + positions[i].Item3 + "," + length + ";");
                    sw.Close();
                    tilePosList.Clear();
                }
                else if (currentKeyboard.IsKeyDown(Keys.LeftControl) && currentKeyboard.IsKeyDown(Keys.D2))
                {
                    sw = File.AppendText("Normal " + size + ".txt");
                    if (positions[i].Item1.Contains("Orb") || positions[i].Item1.Contains("Soul") || positions[i].Item1.Contains("Wanderer"))
                    {
                        sw.Write((int)((positions[i].Item2.X - 190) / 100) + "," + (int)((positions[i].Item2.Y - 190) / 100) + "," + positions[i].Item3 + ";");
                    }
                    else
                    {
                        sw.Write((int)((positions[i].Item2.X - 190) / 100) + "," + (int)((positions[i].Item2.Y - 190) / 100) + "," + positions[i].Item3 + "," + length + ";");
                    }
                    sw.Close();
                    tilePosList.Clear();
                }

                else if (currentKeyboard.IsKeyDown(Keys.LeftControl) && currentKeyboard.IsKeyDown(Keys.D3) && size == "1x1")
                {
                    sw = File.AppendText("Boss " + size + ".txt");
                    if (positions[i].Item1.Contains("Orb") || positions[i].Item1.Contains("Soul") || positions[i].Item1.Contains("Wanderer"))
                    {
                        sw.Write((int)((positions[i].Item2.X - 190) / 100) + "," + (int)((positions[i].Item2.Y - 190) / 100) + "," + positions[i].Item3 + ";");
                    }
                    else
                    {
                        sw.Write((int)((positions[i].Item2.X - 190) / 100) + "," + (int)((positions[i].Item2.Y - 190) / 100) + "," + positions[i].Item3 + "," + length + ";");
                    }
                    sw.Close();
                    tilePosList.Clear();
                }
                i += length - 1;
            }

        }
    }
}

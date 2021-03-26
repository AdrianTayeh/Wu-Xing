using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prototype_Room
{
    class LevelManager
    {
        public Tile[,] tileArray;
        public static int TileSize = 50;
        Player Player;
        Door door;
    
        public bool playerMeetPauline = false;
        Rectangle Empty;
        Rectangle square1_1, square1_2, square1_3, square1_4;
        Rectangle square2_1, square2_2, square2_3, square2_4;
        Rectangle square3_1, square3_2, square3_3, square3_4;
        Rectangle square4_1, square4_2, square4_3, square4_4;


        public LevelManager(string fileName)
        {
            Createlevel(fileName);
        }

        public bool GetTileAtPosition(Vector2 vec)
        {
            return tileArray[(int)vec.X / TileSize, (int)vec.Y / TileSize].wall;
        }

        public void Createlevel(string fileName)
        {
            List<string> strings = ReadFromFile(fileName);
            tileArray = new Tile[strings[0].Length, strings.Count];

            Empty = new Rectangle(0, 0, 30, 30);
            //Rectanglar för olika tilsen så at de vissar olika bilder
            square1_1 = new Rectangle(0, 0, 32, 32);
            square1_2 = new Rectangle(33, 0, 32, 32);
            square1_3 = new Rectangle(66, 0, 32, 32);
            square1_4 = new Rectangle(99, 0, 32, 32);

            square2_1 = new Rectangle(0, 33, 32, 32);
            square2_2 = new Rectangle(33, 33, 32, 32);
            square2_3 = new Rectangle(66, 33, 32, 32);
            square2_4 = new Rectangle(99, 33, 32, 32);

            square3_1 = new Rectangle(0, 66, 32, 32);
            square3_2 = new Rectangle(33, 66, 32, 32);
            square3_3 = new Rectangle(66, 66, 32, 32);
            square3_4 = new Rectangle(99, 66, 32, 32);

            square4_1 = new Rectangle(0, 99, 32, 32);
            square4_2 = new Rectangle(33, 99, 32, 32);
            square4_3 = new Rectangle(66, 99, 32, 32);
            square4_4 = new Rectangle(99, 99, 32, 32);

            for (int j = 0; j < tileArray.GetLength(0); j++)
            {
                for (int i = 0; i < tileArray.GetLength(1); i++)
                {
                    // Texturerna för tiles
                    if (strings[i][j] == 'w')
                    {
                        tileArray[j, i] = new Tile(new Vector2(TileSize * j, TileSize * i), true, square1_1, TextureManager.TileSheet);
                    }
                    else if (strings[i][j] == '-')
                    {
                        tileArray[j, i] = new Tile(new Vector2(TileSize * j, TileSize * i), false, Empty, TextureManager.empty);
                    }
                    else if (strings[i][j] == 'a')
                    {
                        tileArray[j, i] = new Tile(new Vector2(TileSize * j, TileSize * i), true, square3_3, TextureManager.TileSheet);
                    }
                    else if (strings[i][j] == 'c')
                    {
                        tileArray[j, i] = new Tile(new Vector2(TileSize * j, TileSize * i), true, square1_4, TextureManager.TileSheet);
                    }
                    else if (strings[i][j] == 'd')
                    {
                        tileArray[j, i] = new Tile(new Vector2(TileSize * j, TileSize * i), true, square2_3, TextureManager.TileSheet);
                    }
                    else if (strings[i][j] == 'e')
                    {
                        tileArray[j, i] = new Tile(new Vector2(TileSize * j, TileSize * i), true, square2_2, TextureManager.TileSheet);
                    }
                    else if (strings[i][j] == 'b')
                    {
                        tileArray[j, i] = new Tile(new Vector2(TileSize * j, TileSize * i), true, square4_1, TextureManager.TileSheet);
                    }
                    else if (strings[i][j] == 'f')
                    {
                        tileArray[j, i] = new Tile(new Vector2(TileSize * j, TileSize * i), true, square3_2, TextureManager.TileSheet);
                    }
                    else if (strings[i][j] == 'g')
                    {
                        tileArray[j, i] = new Tile(new Vector2(TileSize * j, TileSize * i), true, square2_1, TextureManager.TileSheet);
                    }
                    else if (strings[i][j] == 'h')
                    {
                        tileArray[j, i] = new Tile(new Vector2(TileSize * j, TileSize * i), true, square1_2, TextureManager.TileSheet);
                    }
                    else if (strings[i][j] == 'j')
                    {
                        tileArray[j, i] = new Tile(new Vector2(TileSize * j, TileSize * i), true, square4_2, TextureManager.TileSheet);
                    }
                    else if (strings[i][j] == 'k')
                    {
                        tileArray[j, i] = new Tile(new Vector2(TileSize * j, TileSize * i), true, square1_3, TextureManager.TileSheet);
                    }
                    else if (strings[i][j] == 'l')
                    {
                        tileArray[j, i] = new Tile(new Vector2(TileSize * j, TileSize * i), true, square2_4, TextureManager.TileSheet);
                    }
                    else if (strings[i][j] == 'm')
                    {
                        tileArray[j, i] = new Tile(new Vector2(TileSize * j, TileSize * i), true, square3_1, TextureManager.TileSheet);
                    }
                    //Texturerna för spelaren
                    else if (strings[i][j] == 'p')
                    {
                        tileArray[j, i] = new Tile(new Vector2(TileSize * j, TileSize * i), false, new Rectangle(4, 4, 30, 30), TextureManager.empty);
                        Player = new Player(new Vector2(TileSize * j, TileSize * i), TextureManager.PacMan, new Rectangle(0, 0, TextureManager.PacMan.Width / 4, TextureManager.PacMan.Height), this);
                    }
                    else if (strings[i][j] == '+')
                    {
                        tileArray[j, i] = new Tile(new Vector2(TileSize * j, TileSize * i), false, Empty, TextureManager.empty);
                    }
                    else if (strings[i][j] == 'z')
                    {
                        tileArray[j, i] = new Tile(new Vector2(TileSize * j, TileSize * i), true, new Rectangle(4, 4, 30, 30), TextureManager.empty);
                        door = new Door(new Vector2(TileSize * j, TileSize * i), TextureManager.door1, new Rectangle(0, 0, TextureManager.door1.Width / 16, TextureManager.door1.Height / 16), this);
                    }
                }
            }
        }
        public List<string> ReadFromFile(string fileName)
        {
            StreamReader sr = new StreamReader(fileName);
            List<string> strings = new List<string>();
            while (!sr.EndOfStream)
            {
                string s = sr.ReadLine();
                strings.Add(s);
                Console.WriteLine(s);
            }            sr.Close();
            return strings;
        }
        public void Update(GameTime gameTime)
        {
            //upptarterar spelaren
            Player.Update(gameTime);
        }

        public static bool TilePos(Vector2 vec)
        {
            return true;
        }
        //ritar utt tiles
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Tile tile in tileArray)
            {
                if (tile != null)
                {
                    tile.Draw(spriteBatch);
                }
            }
            Player.Draw(spriteBatch);
            door.draw(spriteBatch);
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Level_Editor
{
    class TextureLibrary
    {
        public enum RoomType { Normal, Center, Boss };
        private RoomType roomType;


        public static Dictionary<RoomType, Texture2D> DoorBottoms { get; private set; }
        public static Dictionary<RoomType, Texture2D> DoorTops { get; private set; }
        public static Dictionary<RoomType, Texture2D> DoorFronts { get; private set; }

        public static Texture2D Shadow { get; private set; }
        public static Texture2D Stone { get; private set; }
        public static Texture2D Tiles { get; private set; }
        public static Texture2D BackgroundGray { get; private set; }

        public static Dictionary<string, Texture2D> Rooms { get; private set; }

        public static Texture2D WhitePixel { get; private set; }


        public static void Load(ContentManager Content, GraphicsDevice GraphicsDevice)
        {
            BackgroundGray = Content.Load<Texture2D>("Background Gray");

            //Folder Doors
            DoorBottoms = new Dictionary<RoomType, Texture2D>();
            DoorBottoms.Add(RoomType.Normal, Content.Load<Texture2D>("Doors\\Door Bottom Normal"));
            DoorBottoms.Add(RoomType.Boss, Content.Load<Texture2D>("Doors\\Door Bottom Boss"));

            DoorTops = new Dictionary<RoomType, Texture2D>();
            DoorTops.Add(RoomType.Normal, Content.Load<Texture2D>("Doors\\Door Top Normal"));
            DoorTops.Add(RoomType.Boss, Content.Load<Texture2D>("Doors\\Door Top Boss"));

            DoorFronts = new Dictionary<RoomType, Texture2D>();
            DoorFronts.Add(RoomType.Normal, Content.Load<Texture2D>("Doors\\Door Front Normal"));
            DoorFronts.Add(RoomType.Boss, Content.Load<Texture2D>("Doors\\Door Front Normal"));

            Shadow = Content.Load<Texture2D>("Miscellaneous\\Shadow");
            Stone = Content.Load<Texture2D>("Miscellaneous\\Stone");
            Tiles = Content.Load<Texture2D>("Miscellaneous\\Tiles");

            //Folder Rooms
            Rooms = new Dictionary<string, Texture2D>();
            Rooms.Add("1x1", Content.Load<Texture2D>("Rooms\\Room 1x1"));
            Rooms.Add("1x2", Content.Load<Texture2D>("Rooms\\Room 1x2"));
            Rooms.Add("1x3", Content.Load<Texture2D>("Rooms\\Room 1x3"));
            Rooms.Add("2x1", Content.Load<Texture2D>("Rooms\\Room 2x1"));
            Rooms.Add("3x1", Content.Load<Texture2D>("Rooms\\Room 3x1"));
            Rooms.Add("2x2", Content.Load<Texture2D>("Rooms\\Room 2x2"));

            WhitePixel = new Texture2D(GraphicsDevice, 2, 2);
            Color[] c = new Color[4];
            c[0] = c[1] = c[2] = c[3] = Color.White;
            WhitePixel.SetData(c);
        }

    }
}

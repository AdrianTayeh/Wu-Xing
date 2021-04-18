using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Wu_Xing
{
    public class Room
    {
        private Tile[,] tiles;
        private List<Door> doors;

        private Point size;

        private bool discovered;
        private bool cleared;

        public enum Type { Normal, Center, Boss}
        private Type roomType;

        public Room(Point size, Type roomType, List<Door> doors)
        {
            this.size = size;
            this.roomType = roomType;
            this.doors = doors;
            tiles = new Tile[15 * size.X, 7 * size.Y];
        }

        public Point Size { get { return size; } }
        public Type RoomType { get { return roomType; } }
        public List<Door> Doors { get { return doors; } }

        public void Draw(SpriteBatch spriteBatch, Element element, Vector2 position)
        {
            spriteBatch.Draw(TextureLibrary.Rooms[size.X + "x" + size.Y], position, null, ColorLibrary.Room[element], 0, Vector2.Zero, 1, SpriteEffects.None, 0.1f);
            
            foreach (Door door in doors)
                door.Draw(spriteBatch, position);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Wu_Xing
{
    class Room
    {
        private List<Tile> tiles;
        private List<Door> doors;

        private Point size;

        public enum State { Unknown, Discovered, Cleared }
        private State roomState;

        //State       Desciption                                        Minimap appearence
        
        //Unknown     Unknown to the player                             Hidden
        //Discovered  At any point entered or adjacent to entered room  Dark
        //Cleared     Has been discovered, no enemies present           Light

        public enum Type { Normal, Center, Boss}
        private Type roomType;

        public Room(Point size, Type roomType, List<Door> doors, List<Tile> tiles)
        {
            this.size = size;
            this.roomType = roomType;
            this.doors = doors;
            this.tiles = tiles;
            roomState = State.Unknown;
        }

        public Point Size { get { return size; } }
        public Type RoomType { get { return roomType; } }
        public State RoomState { get { return roomState; } set { roomState = value; } }
        public List<Door> Doors { get { return doors; } }

        public void IsEntered(Room[,] rooms)
        {
            if (roomState == State.Unknown)
            {
                roomState = State.Discovered;      
            }

            if (roomState == State.Discovered)
            {
                foreach (Door door in doors)
                    if (rooms[door.LeadsToRoom.X, door.LeadsToRoom.Y].RoomState == State.Unknown)
                        rooms[door.LeadsToRoom.X, door.LeadsToRoom.Y].RoomState = State.Discovered;

                if (LookForEnemies() == false)
                    roomState = State.Cleared;

                else
                    foreach (Door door in doors)
                        door.Close();
            }
        }

        private bool LookForEnemies()
        {
            //Check for enemies

            //No enemy found
            return false;          
        }

        public void Draw(SpriteBatch spriteBatch, Element element, Vector2 position, bool drawHitbox)
        {
            spriteBatch.Draw(TextureLibrary.Rooms[size.X + "x" + size.Y], position, null, Color.FromNonPremultiplied(60, 60, 60, 255), 0, Vector2.Zero, 1, SpriteEffects.None, 0.1f);

            foreach (Tile tile in tiles)
                tile.Draw(spriteBatch, position, drawHitbox);

            foreach (Door door in doors)
                door.Draw(spriteBatch, position);
        }
    }
}

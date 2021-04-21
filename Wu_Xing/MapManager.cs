using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Wu_Xing
{
    class MapManager
    {
        private Room[,] rooms;
        private Room currentRoom;

        private Point currentRoomLocation;
        private Point transitionRoom;
        private Vector2 transitionPosition;
        private Vector2 transitionRoomPosition;

        private Element element;

        private Dictionary<Room.State, Color> minimapColor;

        public MapManager()
        {
            transitionRoom = new Point(-1, -1);

            minimapColor = new Dictionary<Room.State, Color>();
            minimapColor.Add(Room.State.Unknown, Color.FromNonPremultiplied(0, 100, 100, 255));
            minimapColor.Add(Room.State.Discovered, Color.FromNonPremultiplied(80, 80, 80, 255));
            minimapColor.Add(Room.State.Cleared, Color.FromNonPremultiplied(150, 150, 150, 255));
        }

        public Element Element { get { return element; } }
        public int Size { get { return rooms.GetLength(0); } }
        public Room[,] Rooms { get { return rooms; } }
        public Point CurrentRoomLocation { get { return currentRoomLocation; } }
        public Room CurrentRoom { get { return currentRoom; } }
        public bool Transition { get { return transitionPosition != Vector2.Zero; } }
        public Vector2 TransitionPosition { get { return transitionPosition; } }
        public Vector2 CenterOfCenterRoom
        {
            get
            {
                foreach (Room room in rooms)
                    if (room != null && room.RoomType == Room.Type.Center)
                        return TextureLibrary.Rooms[room.Size.X + "x" + room.Size.Y].Bounds.Size.ToVector2() / 2;

                return Vector2.Zero;
            }
        }

        public void GenerateNewMap(Random random, int size, Element element)
        {
            this.element = element;
            rooms = new MapGenerator(random).NewMap(size, element);
            SetCurrentRoomToCenter();
        }

        private void SetCurrentRoomToCenter()
        {
            //Find the room with type Center and set currentRoom to the rooms location in the map grid

            for (int y = 0; y < rooms.GetLength(0); y++)
            {
                for (int x = 0; x < rooms.GetLength(0); x++)
                {
                    if (rooms[x, y] != null && rooms[x, y].RoomType == Room.Type.Center)
                    {
                        currentRoomLocation = new Point(x, y);
                        currentRoom = rooms[x, y];
                        rooms[x, y].IsEntered(rooms);
                        break;
                    }
                }
            }
        }

        public async void StartRoomTransition(Door door, Rectangle window)
        {
            transitionRoom = door.LeadsToRoom;
            rooms[transitionRoom.X, transitionRoom.Y].IsEntered(rooms);

            //Calculate transitionStart and transitionEnd
            Vector2 transitionStart = door.EntranceArea.Center.ToVector2();
            Vector2 transitionEnd = door.TransitionExitPosition;

            //North door
            if (door.Rotation == 0)
            {
                transitionStart.Y += 500;
                transitionEnd.Y -= 300;
            }

            //East door
            else if (door.Rotation == (float)Math.PI / 2)
            {
                transitionStart.X -= 900;
                transitionEnd.X += 700;
            }

            //South door
            else if (door.Rotation == (float)Math.PI)
            {
                transitionStart.Y -= 500;
                transitionEnd.Y += 300;
            }

            //West door
            else
            {
                transitionStart.X += 900;
                transitionEnd.X -= 700;
            }

            transitionPosition = transitionStart;

            //Calculate transitionRoomPosition
            transitionRoomPosition.X = (transitionRoom.X - currentRoomLocation.X) * 1500;
            transitionRoomPosition.Y = (transitionRoom.Y - currentRoomLocation.Y) * 700;
            transitionRoomPosition += Rotate.PointAroundZero(new Vector2(0, -380), door.Rotation);

            //Start transition
            int duration = 400;
            DateTime startTime = DateTime.Now;
            TimeSpan elapsedTime = TimeSpan.Zero;

            while (elapsedTime.TotalMilliseconds < duration)
            {
                elapsedTime = DateTime.Now - startTime;

                //x goes from (-1)-1 linearly
                float x = (float)elapsedTime.TotalMilliseconds / duration * 2f - 1f;

                //y goes from 0-1 exponentially (sigmoid curve)
                float y = (float)(-1f / (1f + (float)Math.Pow(2, x * 8f)) + 1f);

                transitionPosition = transitionStart + ((transitionEnd - transitionStart) * y);

                //A 1ms delay is necessary in order for transitionPosition to be read correctly
                await Task.Delay(1);
            }

            //Transition finished
            currentRoomLocation = door.LeadsToRoom;
            currentRoom = rooms[currentRoomLocation.X, currentRoomLocation.Y];
            transitionPosition = Vector2.Zero;
            transitionRoom = new Point(-1, -1);
        }

        public void DrawFullMap(SpriteBatch spriteBatch)
        {
            for (int y = 0; y < rooms.GetLength(0); y++)
                for (int x = 0; x < rooms.GetLength(0); x++)
                    if (rooms[x, y] != null)
                        spriteBatch.Draw(TextureLibrary.WhitePixel, new Rectangle(100 + x * 30, 200 + y * 30, 30 * rooms[x, y].Size.X - 4, 30 * rooms[x, y].Size.Y - 4), new Point(x, y) == currentRoomLocation ? Color.White : minimapColor[rooms[x, y].RoomState]);
        }

        public void DrawMinimap(SpriteBatch spriteBatch)
        {
            for (int y = 0; y < rooms.GetLength(0); y++)
                for (int x = 0; x < rooms.GetLength(0); x++)
                    if (rooms[x, y] != null)
                        spriteBatch.Draw(TextureLibrary.WhitePixel, new Rectangle(100 + x * 30, 200 + y * 30, 30 * rooms[x, y].Size.X - 4, 30 * rooms[x, y].Size.Y - 4), new Point(x, y) == currentRoomLocation ? Color.White : minimapColor[rooms[x, y].RoomState]);
        }

        public void DrawWorld(SpriteBatch spriteBatch)
        {
            currentRoom.Draw(spriteBatch, element, Vector2.Zero);

            if (transitionRoom != new Point(-1, -1))
                rooms[transitionRoom.X, transitionRoom.Y].Draw(spriteBatch, element, transitionRoomPosition);
        }
    }
}

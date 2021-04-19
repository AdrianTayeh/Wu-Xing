using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Wu_Xing
{
    class Map
    {
        class MapTile
        {
            private Point createdFrom;
            public enum TileState { New, Ready, Finished }
            TileState state;
            public Point Size;
            public Room.Type Type;

            public MapTile(int X, int Y)
            {
                createdFrom.X = X;
                createdFrom.Y = Y;
                Size = new Point(1, 1);
            }

            public TileState State { get { return state; } set { state = value; } }

            public void Spread(Random random, MapTile[,] mapTile, int X, int Y)
            {
                //50% chance to create two new rooms forward
                if (random.Next(0, 2) == 0)
                    for (int i = 1; i < 3; i++)
                        if (X - createdFrom.X * i >= 0 && X - createdFrom.X * i < mapTile.GetLength(0) && Y - createdFrom.Y * i >= 0 && Y - createdFrom.Y * i < mapTile.GetLength(1))
                            if (mapTile[X - createdFrom.X * i, Y - createdFrom.Y * i] == null)
                                mapTile[X - createdFrom.X * i, Y - createdFrom.Y * i] = new MapTile(createdFrom.X, createdFrom.Y);

                //10% chance to create a new room to the left
                if (random.Next(0, 10) == 0)
                    for (int i = 1; i < 3; i++)
                        if (X - createdFrom.Y * i >= 0 && X - createdFrom.Y * i < mapTile.GetLength(0) && Y - createdFrom.X * i >= 0 && Y - createdFrom.X * i < mapTile.GetLength(1))
                            if (mapTile[X - createdFrom.Y * i, Y - createdFrom.X * i] == null)
                                mapTile[X - createdFrom.Y * i, Y - createdFrom.X * i] = new MapTile(-createdFrom.Y, -createdFrom.X);

                //10% chance to create a new room to the right
                if (random.Next(0, 10) == 0)
                    for (int i = 1; i < 3; i++)
                        if (X + createdFrom.Y * i >= 0 && X + createdFrom.Y * i < mapTile.GetLength(0) && Y + createdFrom.X * i >= 0 && Y + createdFrom.X * i < mapTile.GetLength(1))
                            if (mapTile[X + createdFrom.Y * i, Y + createdFrom.X * i] == null)
                                mapTile[X + createdFrom.Y * i, Y + createdFrom.X * i] = new MapTile(createdFrom.Y, createdFrom.X);

                state = TileState.Finished;
            }
        }

        private Room[,] rooms;

        private Point currentRoom;
        private Point transitionRoom;
        private Vector2 transitionPosition;
        private Vector2 transitionRoomPosition;

        private Element element;
        
        private int gridOffset;
        public static int GridOffset { get; private set; }

        private Dictionary<Room.State, Color> minimapColor;

        /// <summary>
        /// Size must be at least 5, and should not be larger than 25.
        /// </summary>
        public Map(Random random, int size, Element element)
        {
            this.element = element;
            gridOffset = GridOffset = 190;
            transitionRoom = new Point(-1, -1);
            minimapColor = new Dictionary<Room.State, Color>();
            minimapColor.Add(Room.State.Discovered, Color.FromNonPremultiplied(80, 80, 80, 255));
            minimapColor.Add(Room.State.Cleared, Color.FromNonPremultiplied(150, 150, 150, 255));
            MapTile[,] mapTile;

            //For debugging
            int tries = 0;
            DateTime startTime = DateTime.Now;

            //Generation
            while (true)
            {
                tries += 1;
                mapTile = new MapTile[size, size];
                GenerateFirstFiveRooms(mapTile);
                GenerateAllRooms(random, mapTile);

                if (CheckIfMapFulfillsRequirements(mapTile) == true)
                    break;
            }

            CreateBossRooms(mapTile, random);
            CreateLargeRooms(mapTile, random);
            rooms = new Room[size, size];
            ConvertToRoomArray(mapTile, rooms);
            SwapDoorExitPositions(rooms);
            SetCurrentRoomToCenter(rooms);

            Debug.WriteLine("Map successfully generated after " + tries + " tries and " + (DateTime.Now - startTime).TotalMilliseconds + " milliseconds.");
        }

        public Element Element { get { return element; } }
        public int Size { get { return rooms.GetLength(0); } }
        public Room[,] Rooms { get { return rooms; } }
        public Point CurrentRoom { get { return currentRoom; } }
        public bool Transition { get { return transitionPosition != Vector2.Zero; } }
        public Vector2 TransitionPosition { get { return transitionPosition; } }

        public Vector2 CenterOfCenterRoom
        {
            get
            {
                foreach (Room room in rooms)
                    if (room != null && room.RoomType == Room.Type.Center)
                        return new Vector2(gridOffset + room.Size.X * 15 * 100 / 2, gridOffset + room.Size.Y * 7 * 100 / 2);

                return Vector2.Zero;
            }
        }

        private void GenerateFirstFiveRooms(MapTile[,] mapTile)
        {
            mapTile[mapTile.GetLength(0) / 2, mapTile.GetLength(0) / 2] = new MapTile(0, 0);
            mapTile[mapTile.GetLength(0) / 2, mapTile.GetLength(0) / 2].Type = Room.Type.Center;
            mapTile[mapTile.GetLength(0) / 2, mapTile.GetLength(0) / 2 - 1] = new MapTile(0, 1);
            mapTile[mapTile.GetLength(0) / 2, mapTile.GetLength(0) / 2 + 1] = new MapTile(0, -1);
            mapTile[mapTile.GetLength(0) / 2 - 1, mapTile.GetLength(0) / 2] = new MapTile(1, 0);
            mapTile[mapTile.GetLength(0) / 2 + 1, mapTile.GetLength(0) / 2] = new MapTile(-1, 0);
        }

        private void GenerateAllRooms(Random random, MapTile[,] mapTile)
        {
            while (true)
            {
                SpreadTiles(mapTile, random);

                if (CheckForAliveTiles(mapTile) == false)
                    break;
            }
        }

        private void SpreadTiles(MapTile[,] mapTile, Random random)
        {
            //Spread tiles that are ready
            for (int y = 0; y < mapTile.GetLength(0); y++)
                for (int x = 0; x < mapTile.GetLength(0); x++)
                    if (mapTile[x, y] != null && mapTile[x, y].State == MapTile.TileState.Ready)
                        mapTile[x, y].Spread(random, mapTile, x, y);

            //Make new tiles ready
            for (int y = 0; y < mapTile.GetLength(0); y++)
                for (int x = 0; x < mapTile.GetLength(0); x++)
                    if (mapTile[x, y] != null && mapTile[x, y].State == MapTile.TileState.New)
                        mapTile[x, y].State = MapTile.TileState.Ready;
        }

        private bool CheckForAliveTiles(MapTile[,] mapTile)
        {
            for (int y = 0; y < mapTile.GetLength(0); y++)
                for (int x = 0; x < mapTile.GetLength(0); x++)
                    if (mapTile[x, y] != null && mapTile[x, y].State != MapTile.TileState.Finished)
                        return true;

            return false;
        }

        private bool CheckIfMapFulfillsRequirements(MapTile[,] mapTile)
        {
            int totalRooms = 0;

            bool north = false;
            bool south = false;
            bool west = false;
            bool east = false;

            bool northWest = false;
            bool northEast = false;
            bool southWest = false;
            bool southEast = false;

            for (int y = 0; y < mapTile.GetLength(0); y++)
                for (int x = 0; x < mapTile.GetLength(0); x++)
                    if (mapTile[x, y] != null)
                        totalRooms += 1;

            for (int y = 0; y < 2; y++)
                for (int x = 2; x < mapTile.GetLength(0) - 2; x++)
                    if (mapTile[x, y] != null)
                        north = true;

            for (int y = mapTile.GetLength(0) - 2; y < mapTile.GetLength(0); y++)
                for (int x = 2; x < mapTile.GetLength(0) - 2; x++)
                    if (mapTile[x, y] != null)
                        south = true;

            for (int y = 2; y < mapTile.GetLength(0) - 2; y++)
                for (int x = 0; x < 2; x++)
                    if (mapTile[x, y] != null)
                        west = true;

            for (int y = 2; y < mapTile.GetLength(0) - 2; y++)
                for (int x = mapTile.GetLength(0) - 2; x < mapTile.GetLength(0); x++)
                    if (mapTile[x, y] != null)
                        east = true;

            for (int y = 0; y < mapTile.GetLength(0) * 0.3; y++)
                for (int x = 0; x < mapTile.GetLength(0) * 0.3; x++)
                    if (mapTile[x, y] != null)
                        northWest = true;

            for (int y = 0; y < mapTile.GetLength(0) * 0.3; y++)
                for (int x = (int)(mapTile.GetLength(0) * 0.7); x < mapTile.GetLength(0); x++)
                    if (mapTile[x, y] != null)
                        northEast = true;

            for (int y = (int)(mapTile.GetLength(0) * 0.7); y < mapTile.GetLength(0); y++)
                for (int x = 0; x < mapTile.GetLength(0) * 0.3; x++)
                    if (mapTile[x, y] != null)
                        southWest = true;

            for (int y = (int)(mapTile.GetLength(0) * 0.7); y < mapTile.GetLength(0); y++)
                for (int x = (int)(mapTile.GetLength(0) * 0.7); x < mapTile.GetLength(0); x++)
                    if (mapTile[x, y] != null)
                        southEast = true;

            if (totalRooms > mapTile.GetLength(0) * 3 && totalRooms < mapTile.GetLength(0) * 6)
                if (north && south && west && east)
                    if ((northWest && southEast) || (northEast && southWest))
                        return true;

            return false;
        }

        private void CreateBossRooms(MapTile[,] mapTile, Random random)
        {
            List<Point> start = new List<Point>();
            List<Point> end = new List<Point>();

            start.Add(new Point(2, 0));
            end.Add(new Point(mapTile.GetLength(0) - 3, 1));

            start.Add(new Point(2, mapTile.GetLength(0) - 2));
            end.Add(new Point(mapTile.GetLength(0) - 3, mapTile.GetLength(0) - 1));

            start.Add(new Point(0, 2));
            end.Add(new Point(1, mapTile.GetLength(0) - 3));

            start.Add(new Point(mapTile.GetLength(0) - 2, 2));
            end.Add(new Point(mapTile.GetLength(0) - 1, mapTile.GetLength(0) - 3));

            for (int i = 0; i < 4; i++)
            {
                List<Point> badBossRoomLocations = new List<Point>();
                List<Point> edgeBossRoomLocations = new List<Point>();
                List<Point> perfectBossRoomLocations = new List<Point>();

                for (int y = start[i].Y; y <= end[i].Y; y++)
                {
                    for (int x = start[i].X; x <= end[i].X; x++)
                    {
                        if (mapTile[x, y] != null)
                        {
                            int connections = 0;

                            if (y - 1 >= 0 && mapTile[x, y - 1] != null)
                                connections += 1;

                            if (y + 1 < mapTile.GetLength(0) && mapTile[x, y + 1] != null)
                                connections += 1;

                            if (x - 1 >= 0 && mapTile[x - 1, y] != null)
                                connections += 1;

                            if (x + 1 < mapTile.GetLength(0) && mapTile[x + 1, y] != null)
                                connections += 1;

                            if (connections == 1)
                                perfectBossRoomLocations.Add(new Point(x, y));

                            else if (x == 0 || x == mapTile.GetLength(0) - 1 || y == 0 || y == mapTile.GetLength(0) - 1)
                                edgeBossRoomLocations.Add(new Point(x, y));

                            else
                                badBossRoomLocations.Add(new Point(x, y));
                        }
                    }
                }

                Point location;

                if (perfectBossRoomLocations.Count >= 1)
                    location = perfectBossRoomLocations[random.Next(0, perfectBossRoomLocations.Count)];

                else if (edgeBossRoomLocations.Count >= 1)
                    location = edgeBossRoomLocations[random.Next(0, edgeBossRoomLocations.Count)];

                else
                    location = badBossRoomLocations[random.Next(0, badBossRoomLocations.Count)];

                mapTile[location.X, location.Y].Type = Room.Type.Boss;
            }
        }

        private void CreateLargeRooms(MapTile[,] mapTile, Random random)
        {
            List<Point> roomSizes = new List<Point>();
            roomSizes.Add(new Point(2, 2));

            if (random.Next(0, 2) == 0)
            {
                roomSizes.Add(new Point(3, 1));
                roomSizes.Add(new Point(1, 3));
                roomSizes.Add(new Point(2, 1));
                roomSizes.Add(new Point(1, 2));
            }

            else
            {
                roomSizes.Add(new Point(1, 3));
                roomSizes.Add(new Point(3, 1));
                roomSizes.Add(new Point(1, 2));
                roomSizes.Add(new Point(2, 1));
            }
            
            //For all room sizes
            foreach (Point roomSize in roomSizes)
            {
                //Check all tiles
                for (int y = 0; y < mapTile.GetLength(0); y++)
                {
                    for (int x = 0; x < mapTile.GetLength(0); x++)
                    {
                        //Check if tile can become current room size
                        bool possible = true;
                        for (int b = y; b < y + roomSize.Y; b++)
                            for (int a = x; a < x + roomSize.X; a++)
                                if (a >= mapTile.GetLength(0) || b >= mapTile.GetLength(0) || mapTile[a, b] == null || mapTile[a, b].Size != new Point(1, 1) || mapTile[a, b].Type == Room.Type.Boss)
                                    possible = false;

                        //If possible, 25% chance to implement
                        if (possible && random.Next(0, 4) == 0)
                        {
                            mapTile[x, y].Size = roomSize;

                            for (int b = y; b < y + roomSize.Y; b++)
                            {
                                for (int a = x; a < x + roomSize.X; a++)
                                {
                                    if (new Point(a, b) != new Point(x, y))
                                    {
                                        if (mapTile[a, b].Type == Room.Type.Center)
                                            mapTile[x, y].Type = Room.Type.Center;

                                        //Make size negative, makes it read as a pointer
                                        mapTile[a, b].Size = new Point(x - a, y - b);
                                    }
                                }
                            }             
                        }
                    }
                }
            }
        }

        private void ConvertToRoomArray(MapTile[,] mapTile, Room[,] rooms)
        {
            for (int y = 0; y < mapTile.GetLength(0); y++)
            {
                for (int x = 0; x < mapTile.GetLength(0); x++)
                {
                    //If the tile exists, and its size is positive
                    if (mapTile[x, y] != null && mapTile[x, y].Size.X > 0 && mapTile[x, y].Size.Y > 0)
                    {
                        List<Door> doors = new List<Door>();

                        if (y > 0)
                            for (int a = x; a < x + mapTile[x, y].Size.X; a++)
                                if (mapTile[a, y - 1] != null)
                                    doors.Add(NewDoor(mapTile, x, y, a, y - 1, 0));

                        if (x + mapTile[x, y].Size.X - 1 < mapTile.GetLength(0) - 1)
                            for (int b = y; b < y + mapTile[x, y].Size.Y; b++)
                                if (mapTile[x + mapTile[x, y].Size.X, b] != null)
                                    doors.Add(NewDoor(mapTile, x, y, x + mapTile[x, y].Size.X, b, (float)Math.PI / 2));

                        if (y + mapTile[x, y].Size.Y - 1 < mapTile.GetLength(0) - 1)
                            for (int a = x; a < x + mapTile[x, y].Size.X; a++)
                                if (mapTile[a, y + mapTile[x, y].Size.Y] != null)
                                    doors.Add(NewDoor(mapTile, x, y, a, y + mapTile[x, y].Size.Y, (float)Math.PI));

                        if (x > 0)
                            for (int b = y; b < y + mapTile[x, y].Size.Y; b++)
                                if (mapTile[x - 1, b] != null)
                                    doors.Add(NewDoor(mapTile, x, y, x - 1, b, (float)Math.PI * 1.5f));

                        rooms[x, y] = new Room(mapTile[x, y].Size, mapTile[x, y].Type, doors);
                    }
                }
            }
        }

        private Door NewDoor(MapTile[,] mapTile, int x, int y, int a, int b, float rotation)
        {
            Point exitTile = Point.Zero;
            Point entranceTile = Point.Zero;
            Vector2 position = Vector2.Zero;

            if (rotation == 0)
            {
                exitTile.X = 7 + 15 * (a - x);
                entranceTile = new Point(exitTile.X, exitTile.Y - 2);
                position.X = gridOffset + exitTile.X * 100 + 50;
            }

            else if (rotation == (float)Math.PI / 2)
            {
                exitTile.X = 15 * mapTile[x, y].Size.X - 1;
                exitTile.Y = 3 + 7 * (b - y);
                entranceTile = new Point(exitTile.X + 2, exitTile.Y);
                position.X = gridOffset + exitTile.X * 100 + 100 + gridOffset;
                position.Y = gridOffset + exitTile.Y * 100 + 50;
            }

            else if (rotation == (float)Math.PI)
            {
                exitTile.X = 7 + 15 * (a - x);
                exitTile.Y = 7 * mapTile[x, y].Size.Y - 1;
                entranceTile = new Point(exitTile.X, exitTile.Y + 2);
                position.X = gridOffset + exitTile.X * 100 + 50;
                position.Y = gridOffset + exitTile.Y * 100 + 100 + gridOffset;
            }

            else
            {
                exitTile.Y = 3 + 7 * (b - y);
                entranceTile = new Point(exitTile.X - 2, exitTile.Y);
                position.Y = gridOffset + exitTile.Y * 100 + 50;
            }

            Vector2 exitPosition = new Vector2(gridOffset + exitTile.X * 100 + 50, gridOffset + exitTile.Y * 100 + 50);
            Rectangle entranceArea = new Rectangle(gridOffset + entranceTile.X * 100, gridOffset + entranceTile.Y * 100, 100, 100);
            Point leadsToRoom = mapTile[a, b].Size.X > 0 ? new Point(a, b) : new Point(a + mapTile[a, b].Size.X, b + mapTile[a, b].Size.Y);
            Room.Type doorType = mapTile[x, y].Type == Room.Type.Boss || mapTile[leadsToRoom.X, leadsToRoom.Y].Type == Room.Type.Boss ? Room.Type.Boss : Room.Type.Normal;
            
            return new Door(position, exitPosition, entranceArea, rotation, leadsToRoom, doorType);
        }

        private void SwapDoorExitPositions(Room[,] rooms)
        {
            List<Door> swappedDoors = new List<Door>();

            for (int y = 0; y < rooms.GetLength(0); y++)
            {
                for (int x = 0; x < rooms.GetLength(0); x++)
                {
                    if (rooms[x, y] != null)
                    {
                        foreach (Door door in rooms[x, y].Doors)
                        {
                            foreach (Door correspondingDoor in rooms[door.LeadsToRoom.X, door.LeadsToRoom.Y].Doors)
                            {
                                if (correspondingDoor.LeadsToRoom == new Point(x, y) && !swappedDoors.Contains(correspondingDoor))
                                {
                                    Vector2 swappedPosition = door.ExitPosition;
                                    door.ExitPosition = correspondingDoor.ExitPosition;
                                    correspondingDoor.ExitPosition = swappedPosition;

                                    swappedDoors.Add(door);
                                    swappedDoors.Add(correspondingDoor);

                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void SetCurrentRoomToCenter(Room[,] rooms)
        {
            for (int y = 0; y < rooms.GetLength(0); y++)
            {
                for (int x = 0; x < rooms.GetLength(0); x++)
                {
                    if (rooms[x, y] != null && rooms[x, y].RoomType == Room.Type.Center)
                    {
                        currentRoom = new Point(x, y);
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

            if (door.Rotation == 0)
            {
                transitionStart.Y += 500;
                transitionEnd.Y -= 300;
            }

            else if (door.Rotation == (float)Math.PI / 2)
            {
                transitionStart.X -= 900;
                transitionEnd.X += 700;
            }

            else if (door.Rotation == (float)Math.PI)
            {
                transitionStart.Y -= 500;
                transitionEnd.Y += 300;
            }

            else
            {
                transitionStart.X += 900;
                transitionEnd.X -= 700;
            }

            transitionPosition = transitionStart;

            //Calculate transitionRoomPosition
            transitionRoomPosition.X = (transitionRoom.X - currentRoom.X) * 1500;
            transitionRoomPosition.Y = (transitionRoom.Y - currentRoom.Y) * 700;
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
            currentRoom = door.LeadsToRoom;
            transitionPosition = Vector2.Zero;
            transitionRoom = new Point(-1, -1);
        }

        public void DrawMinimap(SpriteBatch spriteBatch)
        {
            for (int y = 0; y < rooms.GetLength(0); y++)
            {
                for (int x = 0; x < rooms.GetLength(0); x++)
                {
                    if (rooms[x, y] != null && rooms[x, y].RoomState != Room.State.Unknown)
                    {
                        spriteBatch.Draw(TextureLibrary.WhitePixel, new Rectangle(50 + x * 30, 50 + y * 30, 30 * rooms[x, y].Size.X - 4, 30 * rooms[x, y].Size.Y - 4), new Point(x, y) == currentRoom ? Color.White : minimapColor[rooms[x, y].RoomState]);
                    }
                }
            }           
        }

        public void DrawWorld(SpriteBatch spriteBatch)
        {
            rooms[currentRoom.X, currentRoom.Y].Draw(spriteBatch, element, Vector2.Zero);

            if (transitionRoom != new Point(-1, - 1))
                rooms[transitionRoom.X, transitionRoom.Y].Draw(spriteBatch, element, transitionRoomPosition);
        }

    }
}

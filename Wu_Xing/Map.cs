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
            minimapColor.Add(Room.State.Unknown, Color.FromNonPremultiplied(0, 100, 100, 255));
            minimapColor.Add(Room.State.Discovered, Color.FromNonPremultiplied(80, 80, 80, 255));
            minimapColor.Add(Room.State.Cleared, Color.FromNonPremultiplied(150, 150, 150, 255));

            //For debugging
            int tries = 0;
            DateTime startTime = DateTime.Now;

            //MapTile matrix used to generate the map
            MapTile[,] mapTile;

            //Main generation loop, each loop generates a completely new map
            while (true)
            {
                tries += 1;
                mapTile = new MapTile[size, size];
                GenerateFirstFiveRooms(mapTile);
                GenerateAllRooms(random, mapTile);

                if (CheckIfMapFulfillsRequirements(mapTile) == true)
                    break;
            }

            //Further develop the successfully generated map
            CreateBossRooms(mapTile, random);
            CreateLargeRooms(mapTile, random);
            rooms = new Room[size, size];
            ConvertToRoomArray(mapTile, rooms, random);
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
            //Returns true if an alive tile was found
            for (int y = 0; y < mapTile.GetLength(0); y++)
                for (int x = 0; x < mapTile.GetLength(0); x++)
                    if (mapTile[x, y] != null && mapTile[x, y].State != MapTile.TileState.Finished)
                        return true;

            return false;
        }

        private bool CheckIfMapFulfillsRequirements(MapTile[,] mapTile)
        {
            int totalRooms = 0;

            //Count total rooms
            for (int y = 0; y < mapTile.GetLength(0); y++)
                for (int x = 0; x < mapTile.GetLength(0); x++)
                    if (mapTile[x, y] != null)
                        totalRooms += 1;

            //If a reasonable amount of rooms were not created
            //The map does not fullfill the requirements, returns false to generate new map
            if (totalRooms <= mapTile.GetLength(0) * 3 || totalRooms > mapTile.GetLength(0) * 6)
                return false;

            //Create areas
            Rectangle[] areas = new Rectangle[8];
            bool[] roomsFound = new bool[8];
            int cornerLength = (int)(mapTile.GetLength(0) * 0.3);

            //North
            areas[0] = new Rectangle(2, 0, mapTile.GetLength(0) - 4, 2);
            //South
            areas[1] = new Rectangle(2, mapTile.GetLength(0) - 2, mapTile.GetLength(0) - 4, 2);
            //West
            areas[2] = new Rectangle(0, 2, 2, mapTile.GetLength(0) - 4);
            //East
            areas[3] = new Rectangle(mapTile.GetLength(0) - 2, 2, 2, mapTile.GetLength(0) - 4);
            //North west
            areas[4] = new Rectangle(0, 0, cornerLength, cornerLength);
            //North east
            areas[5] = new Rectangle(mapTile.GetLength(0) - cornerLength, 0, cornerLength, cornerLength);
            //South west
            areas[6] = new Rectangle(0, mapTile.GetLength(0) - cornerLength, cornerLength, cornerLength);
            //South east
            areas[7] = new Rectangle(mapTile.GetLength(0) - cornerLength, mapTile.GetLength(0) - cornerLength, cornerLength, cornerLength);

            //For each area, check all tiles to find room
            for (int i = 0; i < areas.Length; i++)
                for (int y = areas[i].Y; y < areas[i].Y + areas[i].Height; y++)
                    for (int x = areas[i].X; x < areas[i].X + areas[i].Width; x++)
                        if (mapTile[x, y] != null)
                            roomsFound[i] = true;

            //If no room was found in either north, south, west or east area
            //The map does not fullfill the requirements, returns false to generate new map
            if (!roomsFound[0] || !roomsFound[1] || !roomsFound[2] || !roomsFound[3])
                return false;

            //If no room was found in either north west or south east area
            //And no room was found in either north east or south west area
            //The map does not fullfill the requirements, returns false to generate new map
            if ((!roomsFound[4] || !roomsFound[7]) && (!roomsFound[5] || !roomsFound[6]))
                return false;

            //If the method has not yet returned false, the map fullfills the requirements
            //Returns true to further develop this map
            return true;
        }

        private void CreateBossRooms(MapTile[,] mapTile, Random random)
        {
            Rectangle[] areas = new Rectangle[4];

            //North
            areas[0] = new Rectangle(2, 0, mapTile.GetLength(0) - 4, 2);
            //South
            areas[1] = new Rectangle(2, mapTile.GetLength(0) - 2, mapTile.GetLength(0) - 4, 2);
            //West
            areas[2] = new Rectangle(0, 2, 2, mapTile.GetLength(0) - 4);
            //East
            areas[3] = new Rectangle(mapTile.GetLength(0) - 2, 2, 2, mapTile.GetLength(0) - 4);

            for (int i = 0; i < areas.Length; i++)
            {
                List<Point> badBossRoomLocations = new List<Point>();
                List<Point> edgeBossRoomLocations = new List<Point>();
                List<Point> perfectBossRoomLocations = new List<Point>();

                for (int y = areas[i].Y; y < areas[i].Y + areas[i].Height; y++)
                {
                    for (int x = areas[i].X; x < areas[i].X + areas[i].Width; x++)
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

                            //Determine if the boss room location is perfect, edge, or bad
                            if (connections == 1)
                                perfectBossRoomLocations.Add(new Point(x, y));

                            else if (x == 0 || x == mapTile.GetLength(0) - 1 || y == 0 || y == mapTile.GetLength(0) - 1)
                                edgeBossRoomLocations.Add(new Point(x, y));

                            else
                                badBossRoomLocations.Add(new Point(x, y));
                        }
                    }
                }

                //After all possible boss room locations for this area has been located
                //Choose a random one out of the best category
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

            //Two possible orders, prioritizing horizontal or vertical rooms
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

        private void ConvertToRoomArray(MapTile[,] mapTile, Room[,] rooms, Random random)
        {
            //The dictionary rows pairs a room type with a dictionary
            //which pairs a point (room size) to a list of strings (rows)
            //Each string holds tiles and enemies for a room
            Dictionary<Room.Type, Dictionary<Point, List<string>>> rows = GetDictionaryWithAllRooms();

            //Incomplete
            Dictionary<string, Type> gameObjects = GetDictionaryWithAllGameObjects();

            //Go through all tiles
            for (int y = 0; y < mapTile.GetLength(0); y++)
            {
                for (int x = 0; x < mapTile.GetLength(0); x++)
                {
                    //If the tile exists, and its size is positive
                    //Positive means it it a room and not a pointer
                    if (mapTile[x, y] != null && mapTile[x, y].Size.X > 0 && mapTile[x, y].Size.Y > 0)
                    {
                        //Get doors
                        List<Door> doors = GetListOfDoors(mapTile, x, y);

                        //Get tiles and characters from random string
                        Tile[,] tiles = new Tile[mapTile[x, y].Size.X * 15, mapTile[x, y].Size.Y * 7];
                        List<object> characters = new List<object>();

                        string randomString = rows[mapTile[x, y].Type][mapTile[x, y].Size][random.Next(rows[mapTile[x, y].Type][mapTile[x, y].Size].Count)];
                        if (randomString != "")
                            GetRoomContentsFromString(tiles, characters, randomString, gameObjects);

                        //Initialize room
                        rooms[x, y] = new Room(mapTile[x, y].Size, mapTile[x, y].Type, doors, tiles);
                    }
                }
            }
        }

        private Dictionary<Room.Type, Dictionary<Point, List<string>>> GetDictionaryWithAllRooms()
        {
            Dictionary<Room.Type, Dictionary<Point, List<string>>> rows = new Dictionary<Room.Type, Dictionary<Point, List<string>>>();

            rows.Add(Room.Type.Normal, new Dictionary<Point, List<string>>());
            rows.Add(Room.Type.Center, new Dictionary<Point, List<string>>());
            rows.Add(Room.Type.Boss, new Dictionary<Point, List<string>>());

            rows[Room.Type.Normal] = new Dictionary<Point, List<string>>();
            rows[Room.Type.Center] = new Dictionary<Point, List<string>>();
            rows[Room.Type.Boss] = new Dictionary<Point, List<string>>();

            //Read files and convert their rows to string lists
            List<Point> sizes = new List<Point>();
            sizes.Add(new Point(1, 1));
            sizes.Add(new Point(1, 2));
            sizes.Add(new Point(1, 3));
            sizes.Add(new Point(2, 1));
            sizes.Add(new Point(3, 1));
            sizes.Add(new Point(2, 2));

            foreach (Point size in sizes)
            {
                rows[Room.Type.Normal].Add(size, File.ReadAllLines("Room Content/Normal " + size.X + "x" + size.Y + ".txt").ToList());
                rows[Room.Type.Center].Add(size, File.ReadAllLines("Room Content/Center " + size.X + "x" + size.Y + ".txt").ToList());
            }

            rows[Room.Type.Boss].Add(new Point(1, 1), File.ReadAllLines("Room Content/Boss 1x1" + ".txt").ToList());

            return rows;
        }

        //Incomplete
        private Dictionary<string, Type> GetDictionaryWithAllGameObjects()
        {
            Dictionary<string, Type> gameObjects = new Dictionary<string, Type>();
            /*
            gameObjects.Add("ST", typeof(Tile));
            */

            return gameObjects;
        }

        private List<Door> GetListOfDoors(MapTile[,] mapTile, int x, int y)
        {
            List<Door> doors = new List<Door>();

            //Create doors leading north
            if (y > 0)
                for (int a = x; a < x + mapTile[x, y].Size.X; a++)
                    if (mapTile[a, y - 1] != null)
                        doors.Add(NewDoor(mapTile, x, y, a, y - 1, 0));

            //Create doors leading east
            if (x + mapTile[x, y].Size.X - 1 < mapTile.GetLength(0) - 1)
                for (int b = y; b < y + mapTile[x, y].Size.Y; b++)
                    if (mapTile[x + mapTile[x, y].Size.X, b] != null)
                        doors.Add(NewDoor(mapTile, x, y, x + mapTile[x, y].Size.X, b, (float)Math.PI / 2));

            //Create doors leading south
            if (y + mapTile[x, y].Size.Y - 1 < mapTile.GetLength(0) - 1)
                for (int a = x; a < x + mapTile[x, y].Size.X; a++)
                    if (mapTile[a, y + mapTile[x, y].Size.Y] != null)
                        doors.Add(NewDoor(mapTile, x, y, a, y + mapTile[x, y].Size.Y, (float)Math.PI));

            //Create doors leading west
            if (x > 0)
                for (int b = y; b < y + mapTile[x, y].Size.Y; b++)
                    if (mapTile[x - 1, b] != null)
                        doors.Add(NewDoor(mapTile, x, y, x - 1, b, (float)Math.PI * 1.5f));

            return doors;
        }

        //Incomplete
        private void GetRoomContentsFromString(Tile[,] tiles, List<object> characters, string row, Dictionary<string, Type> gameObjects)
        {
            //Row format:
            //block;block;block
            string[] blocks = row.Split(';');

            foreach (string block in blocks)
            {
                //Block formats:
                //x,y,objectID
                //x,y,objectID,length
                string[] components = block.Split(',');

                int x = int.Parse(components[0]);
                int y = int.Parse(components[1]);
                string objectID = components[2];

                //If the block consists of four components, it is a number of tiles
                if (components.Count() == 4)
                {
                    int length = int.Parse(components[3]);

                    for (int a = x; a < x + length; a++)
                    {
                        //tiles[a, y] = new 
                    }
                }

                //If the block consists of three components, it is a character
                else
                {
                    //characters.Add(gameObjects[objectID]);
                }

            }
        }

        private Door NewDoor(MapTile[,] mapTile, int x, int y, int a, int b, float rotation)
        {
            Point exitTile = Point.Zero;
            Point entranceTile = Point.Zero;
            Vector2 position = Vector2.Zero;

            //North door
            if (rotation == 0)
            {
                exitTile.X = 7 + 15 * (a - x);
                entranceTile = new Point(exitTile.X, exitTile.Y - 2);
                position.X = gridOffset + exitTile.X * 100 + 50;
            }

            //East door
            else if (rotation == (float)Math.PI / 2)
            {
                exitTile.X = 15 * mapTile[x, y].Size.X - 1;
                exitTile.Y = 3 + 7 * (b - y);
                entranceTile = new Point(exitTile.X + 2, exitTile.Y);
                position.X = gridOffset + exitTile.X * 100 + 100 + gridOffset;
                position.Y = gridOffset + exitTile.Y * 100 + 50;
            }

            //South door
            else if (rotation == (float)Math.PI)
            {
                exitTile.X = 7 + 15 * (a - x);
                exitTile.Y = 7 * mapTile[x, y].Size.Y - 1;
                entranceTile = new Point(exitTile.X, exitTile.Y + 2);
                position.X = gridOffset + exitTile.X * 100 + 50;
                position.Y = gridOffset + exitTile.Y * 100 + 100 + gridOffset;
            }

            //West door
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
            //At this point, a door's exit position is what its corresponding door's exit position should be
            //Meaning, for each door, we need to find its corresponding door, and swap their exit positions

            List<Door> swappedDoors = new List<Door>();

            for (int y = 0; y < rooms.GetLength(0); y++)
            {
                for (int x = 0; x < rooms.GetLength(0); x++)
                {
                    //Room found
                    if (rooms[x, y] != null)
                    {
                        //Check all doors in room
                        foreach (Door door in rooms[x, y].Doors)
                        {
                            //Find the corresponting door in the room the door leads to
                            foreach (Door correspondingDoor in rooms[door.LeadsToRoom.X, door.LeadsToRoom.Y].Doors)
                            {
                                //If the door leads back to the first room, it is the corresponding door
                                //If they have not yet been swapped, swap and more on to the next door in the room
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
            //Find the room with type Center and set currentRoom to the rooms location in the map grid

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
                    if (rooms[x, y] != null /*&& rooms[x, y].RoomState != Room.State.Unknown*/)
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

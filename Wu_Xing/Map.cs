using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Wu_Xing
{
    class Map
    {
        class MapTile
        {
            private Point createdFrom;
            public bool Fresh;
            public bool Finished;
            public Point Size;
            public Room.Type Type;

            public MapTile(int X, int Y)
            {
                createdFrom.X = X;
                createdFrom.Y = Y;
                Fresh = true;
                Size = new Point(1, 1);
            }

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

                Finished = true;
            }
        }

        public Map()
        {

        }

        /// <summary>
        /// Size must be at least 5, and should not be larger than 25.
        /// </summary>
        public Room[,] GenerateNewMap(Random random, int size)
        {
            //For debugging
            int tries = 0;
            DateTime startTime = DateTime.Now;

            MapTile[,] mapTile;

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
            Room[,] rooms = new Room[size, size];
            ConvertToRoomArray(mapTile, rooms);
            ConnectDoors(rooms);

            //For debugging
            DateTime finishTime = DateTime.Now;
            Debug.WriteLine("Map successfully generated after " + tries + " tries and " + (finishTime - startTime).TotalMilliseconds + " milliseconds.");

            return rooms;
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
            //Update tiles that can spread
            for (int y = 0; y < mapTile.GetLength(0); y++)
            {
                for (int x = 0; x < mapTile.GetLength(0); x++)
                {
                    if (mapTile[x, y] != null && mapTile[x, y].Fresh == false && mapTile[x, y].Finished == false)
                        mapTile[x, y].Spread(random, mapTile, x, y);
                }
            }

            //Make fresh tiles ready to spread
            int roomsCreated = 0;
            for (int y = 0; y < mapTile.GetLength(0); y++)
            {
                for (int x = 0; x < mapTile.GetLength(0); x++)
                {
                    if (mapTile[x, y] != null)
                    {
                        mapTile[x, y].Fresh = false;
                        roomsCreated += 1;
                    }
                }
            }
        }

        private bool CheckForAliveTiles(MapTile[,] mapTile)
        {
            for (int y = 0; y < mapTile.GetLength(0); y++)
                for (int x = 0; x < mapTile.GetLength(0); x++)
                    if (mapTile[x, y] != null && mapTile[x, y].Finished == false)
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

                        if (y < mapTile.GetLength(0) - 1)
                            for (int a = x; a < x + mapTile[x, y].Size.X; a++)
                                if (mapTile[a, y + 1] != null)
                                    doors.Add(NewDoor(mapTile, x, y, a, y + 1, (float)Math.PI));

                        if (x > 0)
                            for (int b = y; b < y + mapTile[x, y].Size.Y; b++)
                                if (mapTile[x - 1, b] != null)
                                    doors.Add(NewDoor(mapTile, x, y, x - 1, b, (float)Math.PI * 1.5f));

                        if (x < mapTile.GetLength(0) - 1)
                            for (int b = y; b < y + mapTile[x, y].Size.Y; b++)
                                if (mapTile[x + 1, b] != null)
                                    doors.Add(NewDoor(mapTile, x, y, x + 1, b, (float)Math.PI / 2));

                        rooms[x, y] = new Room(mapTile[x, y].Size, Point.Zero, mapTile[x, y].Type, doors);
                    }
                }
            }
        }

        private Door NewDoor(MapTile[,] mapTile, int x, int y, int a, int b, float rotation)
        {
            Point connectedToTile = Point.Zero;
            Vector2 position = Vector2.Zero;

            if (rotation == 0)
            {
                connectedToTile.X = 7 + 15 * (a - x);
                position.X = 190 + connectedToTile.X * 100 + 50;
            }

            else if (rotation == Math.PI / 2)
            {
                connectedToTile.X = 15 * mapTile[x, y].Size.X - 1;
                connectedToTile.Y = 3 + 7 * (b - y);
                position.X = 190 + connectedToTile.X * 100 + 100 + 190;
                position.Y = 190 + connectedToTile.Y * 100 + 50;
            }

            else if (rotation == Math.PI)
            {
                connectedToTile.X = 7 + 15 * (a - x);
                connectedToTile.Y = 7 * mapTile[x, y].Size.Y - 1;
                position.X = 190 + connectedToTile.X * 100 + 50;
                position.Y = 190 + connectedToTile.Y * 100 + 100 + 190;
            }

            else
            {
                connectedToTile.Y = 3 + 7 * (b - y);
                position.Y = 190 + connectedToTile.Y * 100 + 50;
            }

            Point leadsToRoom = Point.Zero;

            if (mapTile[a, b].Size.X > 0)
                leadsToRoom = new Point(a, b);

            else
                leadsToRoom = new Point(a + mapTile[a, b].Size.X, b + mapTile[a, b].Size.Y);

            Texture2D topSection = mapTile[a, b].Type == Room.Type.Normal ? TextureLibrary.DoorTopBlackNormal : TextureLibrary.DoorTopBlackBoss;

            return new Door(position, connectedToTile, rotation, leadsToRoom, topSection);
        }

        private void ConnectDoors(Room[,] rooms)
        {
            for (int y = 0; y < rooms.GetLength(0); y++)
            {
                for (int x = 0; x < rooms.GetLength(0); x++)
                {
                    if (rooms[x, y] != null)
                    {
                        List<Door> correspondingDoorsFound = new List<Door>();

                        foreach (Door door in rooms[x, y].Doors)
                        {
                            foreach (Door correspondingDoor in rooms[door.LeadsToRoom.X, door.LeadsToRoom.Y].Doors)
                            {
                                if (correspondingDoor.LeadsToRoom == new Point(x, y) && !correspondingDoorsFound.Contains(correspondingDoor))
                                {
                                    door.LeadsToTile = correspondingDoor.ConnectedToTile;
                                    correspondingDoorsFound.Add(correspondingDoor);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

    }
}

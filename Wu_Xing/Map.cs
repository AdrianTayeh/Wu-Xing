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
        /// Size must be at least 5, and at most 25.
        /// </summary>
        public Room[,] GenerateNewMap(Random random, int size)
        {
            //For debugging
            int tries = 0;
            DateTime startTime = DateTime.Now;

            //Generate a new map
            MapTile[,] mapTile;
            while (true)
            {
                tries += 1;

                //First 5 rooms
                mapTile = new MapTile[size, size];

                mapTile[size / 2, size / 2] = new MapTile(0, 0);
                mapTile[size / 2, size / 2].Type = Room.Type.Center;
                mapTile[size / 2, size / 2 - 1] = new MapTile(0, 1);
                mapTile[size / 2, size / 2 + 1] = new MapTile(0, -1);
                mapTile[size / 2 - 1, size / 2] = new MapTile(1, 0);
                mapTile[size / 2 + 1, size / 2] = new MapTile(-1, 0);

                //Generate rooms
                while (true)
                {
                    SpreadTiles(mapTile, random, size);

                    if (CheckForAliveTiles(mapTile, size) == false)
                        break;
                }

                if (CheckIfMapFulfillsRequirements(mapTile, size) == true)
                    break;
            }

            CreateBossRooms(mapTile, random, size);
            CreateLargeRooms(mapTile, random, size);

            //For debugging
            DateTime finishTime = DateTime.Now;
            Debug.WriteLine("Map successfully generated after " + tries + " tries and " + (finishTime - startTime).TotalMilliseconds + " milliseconds.");

            return ConvertToRoomArray(mapTile, size);
        }

        private void SpreadTiles(MapTile[,] mapTile, Random random, int size)
        {
            //Update tiles that can spread
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    if (mapTile[x, y] != null && mapTile[x, y].Fresh == false && mapTile[x, y].Finished == false)
                        mapTile[x, y].Spread(random, mapTile, x, y);
                }
            }

            //Make fresh tiles ready to spread
            int roomsCreated = 0;
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    if (mapTile[x, y] != null)
                    {
                        mapTile[x, y].Fresh = false;
                        roomsCreated += 1;
                    }
                }
            }
        }

        private bool CheckForAliveTiles(MapTile[,] mapTile, int size)
        {
            for (int y = 0; y < size; y++)
                for (int x = 0; x < size; x++)
                    if (mapTile[x, y] != null && mapTile[x, y].Finished == false)
                        return true;

            return false;
        }

        private void CreateBossRooms(MapTile[,] mapTile, Random random, int size)
        {
            List<Point> start = new List<Point>();
            List<Point> end = new List<Point>();

            start.Add(new Point(2, 0));
            end.Add(new Point(size - 3, 1));

            start.Add(new Point(2, size - 2));
            end.Add(new Point(size - 3, size - 1));

            start.Add(new Point(0, 2));
            end.Add(new Point(1, size - 3));

            start.Add(new Point(size - 2, 2));
            end.Add(new Point(size - 1, size - 3));

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

                            if (y + 1 < size && mapTile[x, y + 1] != null)
                                connections += 1;

                            if (x - 1 >= 0 && mapTile[x - 1, y] != null)
                                connections += 1;

                            if (x + 1 < size && mapTile[x + 1, y] != null)
                                connections += 1;

                            if (connections == 1)
                                perfectBossRoomLocations.Add(new Point(x, y));

                            else if (x == 0 || x == size - 1 || y == 0 || y == size - 1)
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

        private void CreateLargeRooms(MapTile[,] mapTile, Random random, int size)
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
                for (int y = 0; y < size; y++)
                {
                    for (int x = 0; x < size; x++)
                    {
                        //Check if tile can become current room size
                        bool possible = true;
                        for (int b = y; b < y + roomSize.Y; b++)
                            for (int a = x; a < x + roomSize.X; a++)
                                if (a >= size || b >= size || mapTile[a, b] == null || mapTile[a, b].Size != new Point(1, 1) || mapTile[a, b].Type == Room.Type.Boss)
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

        private Room[,] ConvertToRoomArray(MapTile[,] mapTile, int size)
        {
            Room[,] rooms = new Room[size, size];

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    //If the tile exists, and its size is positive
                    if (mapTile[x, y] != null && mapTile[x, y].Size.X > 0 && mapTile[x, y].Size.Y > 0)
                    {
                        List<Door> doors = new List<Door>();

                        //North side
                        /*if (y > 0)
                            for (int a = x; a < x + mapTile[x, y].Size.X; a++)
                                if (mapTile[a, y] != null)
                                    doors.Add(NewDoor(mapTile, x, y, a, y - 1, 0));*/
                        
                        rooms[x, y] = new Room(mapTile[x, y].Size, Point.Zero, mapTile[x, y].Type, doors);
                    }
                }
            }

            return rooms;
        }

        /*private Door NewDoor(MapTile[,] mapTile, int x, int y, int a, int b, float rotation)
        {
            Point connectedToTile;

            if (rotation == 0)
                connectedToTile = new Point(7 + 15 * (x - a), 0);

            else if (rotation == Math.PI / 2)
                connectedToTile = new Point(7 + 15 * , 3 + 7 * (mapTile[a, y].Size.Y - 1));


            //If the tile is a room (positive size)
            if (mapTile[a, y].Size.X > 0)
                return new Door(, 0, new Point(a, y), new Point(7, 6), mapTile[a, y].Type == Room.Type.Normal ? TextureLibrary.DoorTopBlackNormal : TextureLibrary.DoorTopBlackBoss);

            return new Door(new Point(7 + 15 * (mapTile[a, y].Size.X - 1), 0), 0, new Point(a + mapTile[a, y].Size.X, y + mapTile[a, y].Size.Y), new Point(7, 6), mapTile[a, y].Type == Room.Type.Normal ? TextureLibrary.DoorTopBlackNormal : TextureLibrary.DoorTopBlackBoss);
            
        }*/

        private bool CheckIfMapFulfillsRequirements(MapTile[,] mapTile, int size)
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

            for (int y = 0; y < size; y++)
                for (int x = 0; x < size; x++)
                    if (mapTile[x, y] != null)
                        totalRooms += 1;

            for (int y = 0; y < 2; y++)
                for (int x = 2; x < size - 2; x++)
                    if (mapTile[x, y] != null)
                        north = true;

            for (int y = size - 2; y < size; y++)
                for (int x = 2; x < size - 2; x++)
                    if (mapTile[x, y] != null)
                        south = true;

            for (int y = 2; y < size - 2; y++)
                for (int x = 0; x < 2; x++)
                    if (mapTile[x, y] != null)
                        west = true;

            for (int y = 2; y < size - 2; y++)
                for (int x = size - 2; x < size; x++)
                    if (mapTile[x, y] != null)
                        east = true;

            for (int y = 0; y < size * 0.3; y++)
                for (int x = 0; x < size * 0.3; x++)
                    if (mapTile[x, y] != null)
                        northWest = true;

            for (int y = 0; y < size * 0.3; y++)
                for (int x = (int)(size * 0.7); x < size; x++)
                    if (mapTile[x, y] != null)
                        northEast = true;

            for (int y = (int)(size * 0.7); y < size; y++)
                for (int x = 0; x < size * 0.3; x++)
                    if (mapTile[x, y] != null)
                        southWest = true;

            for (int y = (int)(size * 0.7); y < size; y++)
                for (int x = (int)(size * 0.7); x < size; x++)
                    if (mapTile[x, y] != null)
                        southEast = true;

            if (totalRooms > size * 3 && totalRooms < size * 6)
                if (north && south && west && east)
                    if ((northWest && southEast) || (northEast && southWest))
                        return true;

            return false;
        }

    }
}

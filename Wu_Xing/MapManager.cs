using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

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
        private RenderTarget2D fullMinimap;
        private Rectangle minimapSource;
        private Dictionary<Room.Type, Rectangle> minimapIconSource;
        private float minimapOpacity;
        private float minimapScale;
        private bool largeMinimap;

        public MapManager()
        {
            transitionRoom = new Point(-1, -1);
            minimapSource.Width = minimapSource.Height = 7 * 68 - 8;

            minimapIconSource = new Dictionary<Room.Type, Rectangle>();
            minimapIconSource.Add(Room.Type.Center, new Rectangle(0, 0, 60, 60));
            minimapIconSource.Add(Room.Type.Boss, new Rectangle(60, 0, 60, 60));

            minimapColor = new Dictionary<Room.State, Color>();
            minimapColor.Add(Room.State.Unknown, Color.FromNonPremultiplied(0, 100, 100, 255));
            minimapColor.Add(Room.State.Discovered, Color.FromNonPremultiplied(80, 80, 80, 255));
            minimapColor.Add(Room.State.Cleared, Color.FromNonPremultiplied(150, 150, 150, 255));

            minimapScale = 0.5f;
            minimapOpacity = Settings.MinimapOpacity;
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

        public void GenerateNewMap(GraphicsDevice GraphicsDevice, Random random, int size, Element element)
        {
            this.element = element;
            fullMinimap = new RenderTarget2D(GraphicsDevice, size * 68 - 8, size * 68 - 8);

            rooms = new MapGenerator(random).NewMap(size, element);
            MakeCenterCurrentRoom();
        }

        private void MakeCenterCurrentRoom()
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
                        minimapSource = CalculateMinimapSource(currentRoomLocation);
                        break;
                    }
                }
            }
        }

        public void Update(KeyboardState currentKeyboard, KeyboardState previousKeyboard)
        {
            UpdateMinimapOpacityAndScale(currentKeyboard, previousKeyboard);
        }

        private void UpdateMinimapOpacityAndScale(KeyboardState currentKeyboard, KeyboardState previousKeyboard)
        {
            if (currentKeyboard.IsKeyDown(Keys.Tab) && previousKeyboard.IsKeyUp(Keys.Tab))
                largeMinimap = !largeMinimap;

            if (largeMinimap)
            {
                minimapOpacity += minimapOpacity < 1 ? 0.05f : 0;
                minimapScale += minimapScale < 1 ? 0.05f : 0;
            }

            else
            {
                minimapOpacity -= minimapOpacity > Settings.MinimapOpacity ? 0.05f : 0;
                minimapScale -= minimapScale > 0.5 ? 0.05f : 0;
            }
        }

        public async void StartRoomTransition(Door door)
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

            //Get start and end positions for minimapSource
            Vector2 minimapSourceStartPosition = CalculateMinimapSource(currentRoomLocation).Location.ToVector2();
            Vector2 minimapSourceEndPosition = CalculateMinimapSource(door.LeadsToRoom).Location.ToVector2();

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
                minimapSource.Location = (minimapSourceStartPosition + ((minimapSourceEndPosition - minimapSourceStartPosition) * y)).ToPoint();

                //A 1ms delay is necessary in order for transitionPosition to be read correctly
                await Task.Delay(1);
            }

            //Transition finished
            currentRoomLocation = door.LeadsToRoom;
            currentRoom = rooms[currentRoomLocation.X, currentRoomLocation.Y];
            transitionPosition = Vector2.Zero;
            transitionRoom = new Point(-1, -1);
            minimapSource = CalculateMinimapSource(currentRoomLocation);
        }

        private Rectangle CalculateMinimapSource(Point roomLocation)
        {
            Vector2 minimapCenter = roomLocation.ToVector2() + rooms[roomLocation.X, roomLocation.Y].Size.ToVector2() / 2;
            Rectangle newMinimapSource = minimapSource;

            newMinimapSource.X = (int)((minimapCenter.X - 3.5) * 68);
            newMinimapSource.Y = (int)((minimapCenter.Y - 3.5) * 68);

            if (newMinimapSource.X < 0)
                newMinimapSource.X = 0;

            else if (newMinimapSource.X > (rooms.GetLength(0) - 7) * 68)
                newMinimapSource.X = (rooms.GetLength(0) - 7) * 68;

            if (newMinimapSource.Y < 0)
                newMinimapSource.Y = 0;

            else if (newMinimapSource.Y > (rooms.GetLength(0) - 7) * 68)
                newMinimapSource.Y = (rooms.GetLength(0) - 7) * 68;

            return newMinimapSource;
        }

        public void DrawFullMinimap(SpriteBatch spriteBatch, GraphicsDevice GraphicsDevice)
        {
            GraphicsDevice.SetRenderTarget(fullMinimap);
            GraphicsDevice.Clear(Color.Transparent);
            spriteBatch.Begin();

            for (int y = 0; y < rooms.GetLength(0); y++)
            {
                for (int x = 0; x < rooms.GetLength(0); x++)
                {
                    if (rooms[x, y] != null && rooms[x, y].RoomState != Room.State.Unknown)
                    {
                        Rectangle rectangle = new Rectangle(x * 68, y * 68, 68 * rooms[x, y].Size.X - 8, 68 * rooms[x, y].Size.Y - 8);
                        spriteBatch.Draw(TextureLibrary.WhitePixel, rectangle, new Point(x, y) == currentRoomLocation ? Color.White : minimapColor[rooms[x, y].RoomState]);

                        if (rooms[x, y].RoomType != Room.Type.Normal)
                            spriteBatch.Draw(TextureLibrary.MinimapIcons, rectangle.Center.ToVector2(), minimapIconSource[rooms[x, y].RoomType], Color.White, 0, minimapIconSource[rooms[x, y].RoomType].Size.ToVector2() / 2, 1, SpriteEffects.None, 0);
                    }
                }
            }
            
            spriteBatch.End();
        }

        public void DrawMinimap(SpriteBatch spriteBatch, Rectangle window)
        {
            spriteBatch.Draw(TextureLibrary.WhitePixel, new Rectangle(window.Width - (int)(minimapSource.Width * minimapScale) - 70, 30, (int)(minimapSource.Width * minimapScale) + 40, (int)(minimapSource.Height * minimapScale) + 40), Color.FromNonPremultiplied(0, 0, 0, (int)(120 * minimapOpacity)));
            spriteBatch.Draw(fullMinimap, new Vector2(window.Width - 50, 50), minimapSource, Color.FromNonPremultiplied(255, 255, 255, (int)(255 * minimapOpacity)), 0, new Vector2(minimapSource.Width, 0), minimapScale, SpriteEffects.None, 0);
        }

        public void DrawWorld(SpriteBatch spriteBatch, bool drawHitbox)
        {
            currentRoom.Draw(spriteBatch, element, Vector2.Zero, drawHitbox);

            if (transitionRoom != new Point(-1, -1))
                rooms[transitionRoom.X, transitionRoom.Y].Draw(spriteBatch, element, transitionRoomPosition, drawHitbox);
        }
    }
}

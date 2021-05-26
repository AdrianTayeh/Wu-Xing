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
        private Adam adam;
        private Room[,] rooms;
        private Room currentRoom;

        private Element element;
        public enum Realm { Alfheim, Asgard, Helheim, Jotunheim, Midgard, Muspelheim, Nidavellir, Niflheim, Svartalfheim, Vanaheim }
        private Realm realm;
        private Dictionary<Realm, string> realmDescription;
        private Dictionary<Realm, Element> realmElement;

        private bool drawHitboxes;
        private bool extendedUI;
        private float extendedUITransition;

        private Point currentRoomLocation;
        private Point transitionRoom;
        private Vector2 transitionPosition;
        private Vector2 transitionRoomPosition;

        private Dictionary<Room.State, Color> minimapColor;
        private RenderTarget2D fullMinimap;
        private Rectangle minimumMinimapSource;
        private Rectangle minimapSource;
        private Dictionary<Room.Type, Rectangle> minimapIconSource;
        private float minimapOpacity;

        public MapManager()
        {
            transitionRoom = new Point(-1, -1);
            minimumMinimapSource.Width = minimumMinimapSource.Height = 9 * 68 - 8;

            minimapIconSource = new Dictionary<Room.Type, Rectangle>();
            minimapIconSource.Add(Room.Type.Center, new Rectangle(0, 0, 60, 60));
            minimapIconSource.Add(Room.Type.Boss, new Rectangle(60, 0, 60, 60));

            minimapColor = new Dictionary<Room.State, Color>();
            minimapColor.Add(Room.State.Unknown, Color.FromNonPremultiplied(0, 100, 100, 255));
            minimapColor.Add(Room.State.Discovered, Color.FromNonPremultiplied(80, 80, 80, 255));
            minimapColor.Add(Room.State.Cleared, Color.FromNonPremultiplied(150, 150, 150, 255));

            minimapOpacity = Settings.MinimapOpacity;

            realmElement = new Dictionary<Realm, Element>();
            realmElement.Add(Realm.Muspelheim, Element.Fire);
            realmElement.Add(Realm.Helheim, Element.Fire);
            realmElement.Add(Realm.Svartalfheim, Element.Earth);
            realmElement.Add(Realm.Midgard, Element.Earth);
            realmElement.Add(Realm.Asgard, Element.Metal);
            realmElement.Add(Realm.Nidavellir, Element.Metal);
            realmElement.Add(Realm.Niflheim, Element.Water);
            realmElement.Add(Realm.Jotunheim, Element.Water);
            realmElement.Add(Realm.Alfheim, Element.Wood);
            realmElement.Add(Realm.Vanaheim, Element.Wood);

            realmDescription = new Dictionary<Realm, string>();
            realmDescription.Add(Realm.Muspelheim, "Realm of fire");
            realmDescription.Add(Realm.Helheim, "Hell");
            realmDescription.Add(Realm.Svartalfheim, "Realm of the Dark Elves");
            realmDescription.Add(Realm.Midgard, "Realm of the Humans");
            realmDescription.Add(Realm.Asgard, "Realm of the gods");
            realmDescription.Add(Realm.Nidavellir, "Realm of the Dwarves");
            realmDescription.Add(Realm.Niflheim, "Realm of Ice, Snow and Mist");
            realmDescription.Add(Realm.Jotunheim, "Realm of the Giants");
            realmDescription.Add(Realm.Alfheim, "Realm of the Elves");
            realmDescription.Add(Realm.Vanaheim, "Realm of the Vanir");
        }

        public Adam Adam { get { return adam; } }
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

        public void Update(float elapsedSeconds, KeyboardState currentKeyboard, KeyboardState previousKeyboard, Random random)
        {
            CheckKeyboardInput(currentKeyboard, previousKeyboard, random);
            currentRoom.Update(elapsedSeconds, currentKeyboard, adam, this, random);
            UpdateExtendedUI(currentKeyboard, previousKeyboard);
        }

        private void CheckKeyboardInput(KeyboardState currentKeyboard, KeyboardState previousKeyboard, Random random)
        {
            //H - Toggle hitboxes
            if (currentKeyboard.IsKeyDown(Keys.H) && previousKeyboard.IsKeyUp(Keys.H))
                drawHitboxes = !drawHitboxes;

            //K - Toggle extended UI
            if (currentKeyboard.IsKeyDown(Keys.Tab) && previousKeyboard.IsKeyUp(Keys.Tab))
                extendedUI = !extendedUI;
        }

        public void GenerateNewMap(GraphicsDevice GraphicsDevice, Random random, int size, Element gemToFind, Element elementToChannel)
        {
            element = gemToFind;

            if (element == Element.Fire)
                realm = Realm.Muspelheim;

            else if (element == Element.Earth)
                realm = Realm.Svartalfheim;

            else if (element == Element.Metal)
                realm = Realm.Asgard;

            else if (element == Element.Water)
                realm = Realm.Niflheim;

            else
                realm = Realm.Alfheim;

            fullMinimap = new RenderTarget2D(GraphicsDevice, size * 68 - 8, size * 68 - 8);
            rooms = new MapGenerator(random).NewMap(size, element);
            MakeCenterCurrentRoom();
            adam = new Adam(CenterOfCenterRoom, elementToChannel, random);
        }

        public void RegenerateMap(Random random)
        {
            if (element == Element.Fire)
                realm = Realm.Muspelheim;

            else if (element == Element.Earth)
                realm = Realm.Svartalfheim;

            else if (element == Element.Metal)
                realm = Realm.Asgard;

            else if (element == Element.Water)
                realm = Realm.Niflheim;

            else
                realm = Realm.Alfheim;

            rooms = new MapGenerator(random).NewMap(rooms.GetLength(0), element);
            MakeCenterCurrentRoom();
            adam = new Adam(CenterOfCenterRoom, (Element)adam.Element, random);
        }

        public void NullMap()
        {
            rooms = null;
            adam = null;
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
                        minimumMinimapSource = CalculateMinimapSource(currentRoomLocation);
                        break;
                    }
                }
            }
        }

        private void UpdateExtendedUI(KeyboardState currentKeyboard, KeyboardState previousKeyboard)
        {
            if (extendedUI)
                extendedUITransition += extendedUITransition < 1 ? 0.1f : 0;
            
            else
                extendedUITransition -= extendedUITransition > 0 ? 0.1f : 0;

            minimapOpacity = 1 - (1 - Settings.MinimapOpacity) * extendedUITransition;
        }

        public async void StartRoomTransition(Door door, List<GameObject> gameObjects)
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

            //Get start and end positions for minimumMinimapSource
            Vector2 minimumMinimapSourceStartPosition = CalculateMinimapSource(currentRoomLocation).Location.ToVector2();
            Vector2 minimumMinimapSourceEndPosition = CalculateMinimapSource(door.LeadsToRoom).Location.ToVector2();

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
                minimumMinimapSource.Location = (minimumMinimapSourceStartPosition + ((minimumMinimapSourceEndPosition - minimumMinimapSourceStartPosition) * y)).ToPoint();

                //A 1ms delay is necessary in order for transitionPosition to be read correctly
                await Task.Delay(1);
            }

            //Transition finished
            currentRoom.IsLeft();
            currentRoomLocation = door.LeadsToRoom;
            currentRoom = rooms[currentRoomLocation.X, currentRoomLocation.Y];
            adam.Move(door.ExitPosition, gameObjects, currentRoom.Hitboxes);
            transitionRoom = new Point(-1, -1);
            minimumMinimapSource.Location = minimumMinimapSourceEndPosition.ToPoint();

            //Lets running know that the transition is finished
            transitionPosition = Vector2.Zero;
        }

        private Rectangle CalculateMinimapSource(Point roomLocation)
        {
            Vector2 minimapCenter = roomLocation.ToVector2() + currentRoom.Size.ToVector2() / 2;
            Rectangle newMinimapSource = minimumMinimapSource;

            int nrOfVisibleTiles = (minimumMinimapSource.Width + 8) / 68;

            newMinimapSource.X = (int)((minimapCenter.X - nrOfVisibleTiles / 2f) * 68);
            newMinimapSource.Y = (int)((minimapCenter.Y - nrOfVisibleTiles / 2f) * 68);

            if (newMinimapSource.X < 0)
                newMinimapSource.X = 0;

            else if (newMinimapSource.X > (rooms.GetLength(0) - nrOfVisibleTiles) * 68)
                newMinimapSource.X = (rooms.GetLength(0) - nrOfVisibleTiles) * 68;

            if (newMinimapSource.Y < 0)
                newMinimapSource.Y = 0;

            else if (newMinimapSource.Y > (rooms.GetLength(0) - nrOfVisibleTiles) * 68)
                newMinimapSource.Y = (rooms.GetLength(0) - nrOfVisibleTiles) * 68;

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
            float scale = 0.5f + extendedUITransition / 2;
            int size = (int)(468 * scale);

            minimapSource.X = (int)(minimumMinimapSource.X - minimumMinimapSource.X * extendedUITransition);
            minimapSource.Y = (int)(minimumMinimapSource.Y - minimumMinimapSource.Y * extendedUITransition);
            minimapSource.Width = (int)(minimumMinimapSource.Width + (fullMinimap.Width - minimumMinimapSource.Width) * extendedUITransition);
            minimapSource.Height = (int)(minimumMinimapSource.Height + (fullMinimap.Height - minimumMinimapSource.Height) * extendedUITransition);
            
            spriteBatch.Draw(TextureLibrary.WhitePixel, new Rectangle(window.Width - 70 - size, 30, size + 40, size + 40), Color.FromNonPremultiplied(0, 0, 0, 100));
            spriteBatch.Draw(fullMinimap, new Rectangle(window.Width - 50 - size, 50, size, size), minimapSource, ColorLibrary.Opacity(Color.White, minimapOpacity));
        }

        public void DrawRealmDescription(SpriteBatch spriteBatch, Rectangle window)
        {
            spriteBatch.DrawString(FontLibrary.Huge, realm.ToString().ToUpper(), new Vector2(window.Width - 580, 90), Color.FromNonPremultiplied(255, 255, 255, (int)(255 * extendedUITransition)), 0, FontLibrary.Huge.MeasureString(realm.ToString().ToUpper()) * new Vector2(1, 0.5f), 1, SpriteEffects.None, 0);
            spriteBatch.DrawString(FontLibrary.Normal, realmDescription[realm].ToUpper(), new Vector2(window.Width - 580, 140), ColorLibrary.Opacity(ColorLibrary.Element[element], extendedUITransition), 0, FontLibrary.Normal.MeasureString(realmDescription[realm].ToUpper()) * new Vector2(1, 0.5f), 1, SpriteEffects.None, 0);
        }

        public void DrawWorld(SpriteBatch spriteBatch)
        {
            adam.Draw(spriteBatch, Vector2.Zero, drawHitboxes);
            currentRoom.Draw(spriteBatch, Vector2.Zero, drawHitboxes);

            if (transitionRoom != new Point(-1, -1))
                rooms[transitionRoom.X, transitionRoom.Y].Draw(spriteBatch, transitionRoomPosition, drawHitboxes);
        }
    }
}
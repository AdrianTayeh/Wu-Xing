using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Wu_Xing
{
    class Room
    {
        private List<GameObject> gameObjects;
        private List<Door> doors;
        private Point size;

        public enum Type { Normal, Center, Boss }
        private Type roomType;

        public enum State { Unknown, Discovered, Cleared }
        private State roomState;

        //Unknown     Unknown to the player
        //Discovered  At any point entered or adjacent to entered room
        //Cleared     Has been discovered, no enemies present

        public Room(Point size, Type roomType, List<Door> doors, List<GameObject> gameObjects)
        {
            this.size = size;
            this.roomType = roomType;
            this.doors = doors;
            this.gameObjects = gameObjects;
            roomState = State.Unknown;
        }

        public Point Size { get { return size; } }
        public Type RoomType { get { return roomType; } }
        public State RoomState { get { return roomState; } set { roomState = value; } }
        public List<Door> Doors { get { return doors; } }

        public void Update(float elapsedSeconds, KeyboardState currentKeyboard, Adam adam, MapManager mapManager)
        {
            adam.Update(elapsedSeconds, gameObjects, adam, currentKeyboard, mapManager);

            foreach (GameObject gameObject in gameObjects)
                gameObject.Update(elapsedSeconds, gameObjects, adam, currentKeyboard, mapManager);

            for (int i = gameObjects.Count - 1; i >= 0; i--)
                if (gameObjects[i] is Enemy && ((Enemy)gameObjects[i]).IsDead)
                    gameObjects.RemoveAt(i);
        }

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

                if (CheckIfCleared())
                    roomState = State.Cleared;

                else
                    foreach (Door door in doors)
                        door.Close();
            }
        }

        private bool CheckIfCleared()
        {
            foreach (GameObject gameObject in gameObjects)
                if (gameObject is Enemy)
                    return false;
            return true;
        }

        public void Draw(SpriteBatch spriteBatch, Element element, Vector2 position, bool drawHitbox)
        {
            spriteBatch.Draw(TextureLibrary.Rooms[size.X + "x" + size.Y], position, null, Color.FromNonPremultiplied(60, 60, 60, 255), 0, Vector2.Zero, 1, SpriteEffects.None, 0.1f);

            foreach (GameObject gameObject in gameObjects)
                gameObject.Draw(spriteBatch, position, drawHitbox);

            foreach (Door door in doors)
                door.Draw(spriteBatch, position);
        }
    }
}

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
        private static float layerDepth = 0.1f;
        private List<Hitbox> hitboxes;
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

        public Room(Point size, Type roomType, List<Door> doors, List<GameObject> gameObjects, List<Hitbox> hitboxes)
        {
            this.size = size;
            this.roomType = roomType;
            this.doors = doors;
            this.gameObjects = gameObjects;
            this.hitboxes = hitboxes;
            roomState = State.Unknown;
        }

        public Point Size { get { return size; } }
        public Rectangle Bounds { get { return TextureLibrary.Rooms[size.X + "x" + size.Y].Bounds; } }
        public Type RoomType { get { return roomType; } }
        public State RoomState { get { return roomState; } set { roomState = value; } }
        public List<Door> Doors { get { return doors; } }
        public List<Hitbox> Hitboxes { get { return hitboxes; } }
        public List<GameObject> GameObjects { get { return gameObjects; } }

        public void Update(float elapsedSeconds, KeyboardState currentKeyboard, Adam adam, MapManager mapManager, Random random)
        {
            UpdateGameObjects(elapsedSeconds, currentKeyboard, adam, mapManager, random);

            if (roomState == State.Discovered && CheckIfCleared())
            {
                roomState = State.Cleared;
                ToggleDoorHitboxes(false);
                foreach (Door door in doors)
                    door.Open();
            }
        }

        private void UpdateGameObjects(float elapsedSeconds, KeyboardState currentKeyboard, Adam adam, MapManager mapManager, Random random)
        {
            adam.Update(elapsedSeconds, gameObjects, adam, currentKeyboard, mapManager, random);

            for (int i = gameObjects.Count - 1; i >= 0; i--)
                if (!gameObjects[i].IsDead)
                    gameObjects[i].Update(elapsedSeconds, gameObjects, adam, currentKeyboard, mapManager, random);

            for (int i = gameObjects.Count - 1; i >= 0; i--)
                if (gameObjects[i].IsDead)
                    gameObjects.RemoveAt(i);
        }

        private void ToggleDoorHitboxes(bool colliding)
        {
            foreach (Hitbox hitbox in hitboxes)
                if (hitbox.Width == 100 || hitbox.Height == 100)
                    if (doors.FindIndex(door => hitbox.Contains(door.EntranceArea.Center.ToVector2())) != -1)
                        hitbox.Colliding = colliding;
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
                {
                    roomState = State.Cleared;
                    ToggleDoorHitboxes(false);
                }

                else
                {
                    ToggleDoorHitboxes(true);
                    foreach (Door door in doors)
                    {
                        door.Close();
                        SoundLibrary.Door.Play();
                    }       
                }  
            }
        }

        public void IsLeft()
        {
            for (int i = gameObjects.Count - 1; i >= 0; i--)
                if (gameObjects[i] is Projectile)
                    gameObjects.RemoveAt(i);
        }

        private bool CheckIfCleared()
        {
            foreach (GameObject gameObject in gameObjects)
                if (gameObject is Enemy)
                    return false;
            return true;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, bool drawHitbox)
        {
            spriteBatch.Draw(TextureLibrary.Rooms[size.X + "x" + size.Y], position, null, Color.FromNonPremultiplied(60, 60, 60, 255), 0, Vector2.Zero, 1, SpriteEffects.None, layerDepth);

            if (drawHitbox)
                foreach (Hitbox hitbox in hitboxes)
                    hitbox.Draw(spriteBatch, 0.95f);

            for (int i = gameObjects.Count - 1; i >= 0; i--)
                gameObjects[i].Draw(spriteBatch, position, drawHitbox);

            foreach (Door door in doors)
                door.Draw(spriteBatch, position);
        }
    }
}

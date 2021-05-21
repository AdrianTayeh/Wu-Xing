using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Wu_Xing
{
    class Tile : GameObject
    {
        public Tile(Vector2 position, Element? element, Random random) : base(position, element, random)
        {
            //GameObject
            layerDepth = 0.11f;
            source.Size = new Point(100, 100);
            origin = source.Size.ToVector2() / 2;
        }

        public override void Move(Vector2 newPosition, List<GameObject> gameObjects, List<Hitbox> roomHitboxes)
        {
            position = newPosition;
            hitbox.Move(newPosition);

            //Return if this is non-colliding
            if (!hitbox.Colliding)
                return;

            //Check all gameObjects and move out of collision
            for (int i = gameObjects.Count - 1; i >= 0; i--)
            {
                //Ignore if same gameObject
                if (this == gameObjects[i])
                    continue;

                //Ignore if non-colliding
                if (!gameObjects[i].Hitbox.Colliding)
                    continue;

                //Ignore if not Tile
                if (!(gameObjects[i] is Tile))
                    continue;

                //Check for collision
                //If the hitbox was moved
                if (hitbox.MoveOutOfCollision(gameObjects[i].Hitbox))
                {
                    //Adjust position to the hitbox's new center
                    position = hitbox.Center;

                    //Check all gameObjects again
                    i = gameObjects.Count;
                }
            }

            foreach (Hitbox roomHitbox in roomHitboxes)
            {
                //Check for collision
                //If the hitbox was moved
                if (hitbox.MoveOutOfCollision(roomHitbox))
                    //Adjust position to the hitbox's new center
                    position = hitbox.Center;
            }
        }
    }
}

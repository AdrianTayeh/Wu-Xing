using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Wu_Xing
{
    public class Door
    {
        private Vector2 position;
        private float rotation;

        private Point leadsToRoom;
        private Point leadsToTile;

        private Texture2D topSection;

        public Door(Point connectedToTile, float rotation, Point leadsToRoom, Point leadsToTile, Texture2D topSection)
        {
            this.rotation = rotation;
            this.leadsToRoom = leadsToRoom;
            this.leadsToTile = leadsToTile;
            this.topSection = topSection;

            if (rotation == 0)
            {
                position.X = 190 + connectedToTile.X * 100 + 50;
            }

            else if (rotation == Math.PI / 2)
            {
                position.X = 190 + connectedToTile.X * 100 + 100 + 190;
                position.Y = 190 + connectedToTile.Y * 100 + 50;
            }

            else if (rotation == Math.PI)
            {
                position.X = 190 + connectedToTile.X * 100 + 50;
                position.Y = 190 + connectedToTile.Y * 100 + 100 + 190;
            }

            else
            {
                position.Y = 190 + connectedToTile.Y * 100 + 50;
            }

        }

    }
}

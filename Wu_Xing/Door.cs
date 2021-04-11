using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Wu_Xing
{
    public class Door
    {
        private Vector2 position;
        private float rotation;

        private Point connectedToTile;
        private Point leadsToRoom;
        private Point leadsToTile;

        private Texture2D topSection;

        public Door(Vector2 position, Point connectedToTile, float rotation, Point leadsToRoom, Texture2D topSection)
        {
            this.position = position;
            this.connectedToTile = connectedToTile;
            this.rotation = rotation;
            this.leadsToRoom = leadsToRoom;
            this.topSection = topSection;
        }

        public Point LeadsToTile { get { return leadsToTile; } set { leadsToTile = value; } }
        public Point LeadsToRoom { get { return leadsToRoom; } }
        public Point ConnectedToTile { get { return connectedToTile; } }

    }
}

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Wu_Xing
{
    public class Door
    {
        private Vector2 position;
        private float rotation;
        private Vector2 origin;

        private Point connectedToTile;
        private Point leadsToRoom;
        private Point leadsToTile;

        private Room.Type doorType;
        private Rectangle frontSource;
        private Vector2 frontOrigin;

        public Door(Vector2 position, Point connectedToTile, float rotation, Point leadsToRoom, Room.Type doorType)
        {
            this.position = position;
            this.connectedToTile = connectedToTile;
            this.rotation = rotation;
            this.leadsToRoom = leadsToRoom;
            this.doorType = doorType;
            origin.X = TextureLibrary.DoorBottoms[doorType].Width / 2;
            frontSource = new Rectangle(0, 0, 130, 175);
            frontOrigin.X = frontSource.Width / 2;
        }

        public Point LeadsToTile { get { return leadsToTile; } set { leadsToTile = value; } }
        public Point LeadsToRoom { get { return leadsToRoom; } }
        public Point ConnectedToTile { get { return connectedToTile; } }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(TextureLibrary.DoorBottoms[doorType], position, null, Color.White, rotation, origin, 1, SpriteEffects.None, 0);
            spriteBatch.Draw(TextureLibrary.DoorFronts[doorType], position, frontSource, Color.White, rotation, frontOrigin, 1, SpriteEffects.None, 0);
            spriteBatch.Draw(TextureLibrary.DoorTops[doorType], position, null, Color.White, rotation, origin, 1, SpriteEffects.None, 0);
        }
    }
}

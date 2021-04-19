using System;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Wu_Xing
{
    public class Door
    {
        private Vector2 position;
        private float rotation;
        private Vector2 origin;

        private Vector2 exitPosition;
        private Rectangle entranceArea;
        private Point leadsToRoom;

        private Room.Type doorType;
        private Rectangle frontSource;
        private Vector2 frontOrigin;

        public Door(Vector2 position, Vector2 exitPosition, Rectangle entranceArea, float rotation, Point leadsToRoom, Room.Type doorType)
        {
            this.position = position;
            this.exitPosition = exitPosition;
            this.entranceArea = entranceArea;
            this.rotation = rotation;
            this.leadsToRoom = leadsToRoom;
            this.doorType = doorType;

            origin.X = TextureLibrary.DoorBottoms[doorType].Width / 2;
            frontSource.Size = new Point(130, 175);
            frontOrigin.X = frontSource.Width / 2;
            OpenInstantly();
        }

        public Point LeadsToRoom { get { return leadsToRoom; } }
        public Vector2 ExitPosition { get { return exitPosition; } set { exitPosition = value; } }
        public Rectangle EntranceArea { get { return entranceArea; } }
        public Vector2 TransitionExitPosition { get { return Rotate.PointAroundCenter(new Vector2(position.X, position.Y - Map.GridOffset - 50), position, rotation); } }
        public float Rotation { get { return rotation; } }

        public void OpenInstantly()
        {
            frontSource.Location = TextureLibrary.DoorFronts[doorType].Bounds.Size - frontSource.Size;
        }

        public void CloseInstantly()
        {
            frontSource.Location = Point.Zero;
        }

        public async void Open()
        {
            CloseInstantly();

            for (int i = 0; i < 59; i++)
            {
                frontSource.X += frontSource.Width;
                if (frontSource.X == TextureLibrary.DoorFronts[doorType].Width)
                {
                    frontSource.X = 0;
                    frontSource.Y += frontSource.Height;
                }

                await Task.Delay(1000 / 90);
            }
        }

        public async void Close()
        {
            OpenInstantly();

            for (int i = 0; i < 59; i++)
            {
                frontSource.X -= frontSource.Width;
                if (frontSource.X < 0)
                {
                    frontSource.X = TextureLibrary.DoorFronts[doorType].Bounds.Width - frontSource.Width;
                    frontSource.Y -= frontSource.Height;
                }

                await Task.Delay(1000 / 90);
            }
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 roomPosition)
        {
            spriteBatch.Draw(TextureLibrary.DoorBottoms[doorType], roomPosition + position, null, Color.White, rotation, origin, 1, SpriteEffects.None, 0.11f);
            spriteBatch.Draw(TextureLibrary.DoorFronts[doorType], roomPosition + position, frontSource, Color.White, rotation, frontOrigin, 1, SpriteEffects.None, 0.12f);
            spriteBatch.Draw(TextureLibrary.DoorTops[doorType], roomPosition + position, null, Color.White, rotation, origin, 1, SpriteEffects.None, 0.9f);

            //Temporary
            spriteBatch.Draw(TextureLibrary.WhitePixel, new Rectangle(roomPosition.ToPoint() + entranceArea.Location, entranceArea.Size), null, Color.FromNonPremultiplied(255, 255, 255, 50), 0, Vector2.Zero, SpriteEffects.None, 1f);
        }
    }
}

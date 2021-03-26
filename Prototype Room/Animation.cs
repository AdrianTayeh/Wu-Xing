using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prototype_Room
{
    class Animation
    {
        public double timeSinceLastFrame;
        public double timeBetweenFrames;
        public Point currentFrame;
        public Point sheetSize;
        public Point frameSize;
        public Texture2D Tex;
        public Rectangle hitBox;
        public LevelManager level;

        public Animation()
        {

            timeSinceLastFrame = 0;
            timeBetweenFrames = 0.1;
            sheetSize = new Point(3, 0);
            frameSize = new Point(40, 40);
            currentFrame = new Point(0, 0);
        }

        public void Update(GameTime gameTime)
        {
            timeSinceLastFrame += gameTime.ElapsedGameTime.TotalSeconds;

            //I animate the stone in the intro
            if (timeSinceLastFrame >= timeBetweenFrames)
            {
                timeSinceLastFrame -= timeBetweenFrames;
                currentFrame.X++;
                if (currentFrame.X >= sheetSize.X)
                {
                    currentFrame.X = 0;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 pos, float rotation, SpriteEffects pacFX)
        {
            Rectangle frame = new Rectangle(currentFrame.X * frameSize.X, currentFrame.Y * frameSize.Y, frameSize.X, frameSize.Y);
            spriteBatch.Draw(TextureManager.PacMan, pos + new Vector2(20, 20), frame, Color.White, rotation, new Vector2(20, 20), 1, pacFX, 0);

        }
    }
}

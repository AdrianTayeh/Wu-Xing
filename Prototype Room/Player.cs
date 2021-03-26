using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prototype_Room
{
    class Player : GameObject
    {
        Vector2 destination;
        Vector2 direction;
        float speed = 100.0f;
        public bool moving = false;
        LevelManager level1;
        public Rectangle Hitbox;
        public static int life;
        public Vector2 reStart;
        Animation ani;
        float rotation;
        SpriteEffects pacFX;
        double frameTimer;
        public Vector2 Position;
        

        public Player(Vector2 Position, Texture2D Texture, Rectangle Hitbox, LevelManager level1) : base(Position, Texture)
        {
            this.Position = position;
            ani = new Animation();
            this.level1 = level1;
            this.Hitbox = Hitbox;
            reStart = Position;
            rotation = 0;
            pacFX = SpriteEffects.None;
            pacFX = SpriteEffects.FlipHorizontally;
            frameTimer = 100;
        }

        public void ChangeDirection(Vector2 dir)
        {
            direction = dir;
            Vector2 newDestination = position + direction * 50.0f;

            if (!level1.GetTileAtPosition(newDestination))
            {
                destination = newDestination;
                moving = true;
            }
        }
        public void Update(GameTime gameTime)
        {
            //rotation += 0.1f;
            frameTimer -= gameTime.ElapsedGameTime.TotalMilliseconds;
            ani.Update(gameTime);
            if (!moving)
            {

                if (Keyboard.GetState().IsKeyDown(Keys.Left))
                {
                    pacFX = SpriteEffects.FlipHorizontally;
                    rotation = MathHelper.ToRadians(0);
                    frameTimer -= gameTime.ElapsedGameTime.TotalMilliseconds;
                    ChangeDirection(new Vector2(-1, 0));
                }
                else if (Keyboard.GetState().IsKeyDown(Keys.Right))
                {
                    ChangeDirection(new Vector2(1, 0));
                    pacFX = SpriteEffects.None;
                    
                    rotation = MathHelper.ToRadians(0);
                    frameTimer -= gameTime.ElapsedGameTime.TotalMilliseconds;
                }
                else if (Keyboard.GetState().IsKeyDown(Keys.Up))
                {
                    ChangeDirection(new Vector2(0, -1));
                    rotation = MathHelper.ToRadians(-90);
                    pacFX = SpriteEffects.None;
                    
                    frameTimer -= gameTime.ElapsedGameTime.TotalMilliseconds;

                }
                else if (Keyboard.GetState().IsKeyDown(Keys.Down))
                {
                    ChangeDirection(new Vector2(0, 1));
                    rotation = MathHelper.ToRadians(90);
                    pacFX = SpriteEffects.None;
                    
                    frameTimer -= gameTime.ElapsedGameTime.TotalMilliseconds;
                }
            }
            else
            {
                position += direction * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (Vector2.Distance(position, destination) < 1)
                {
                    position = destination;
                    moving = false;
                }
            }
            Hitbox.X = (int)position.X;
            Hitbox.Y = (int)position.Y;
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            ani.Draw(spriteBatch, position, rotation, pacFX);
        }
    }
}

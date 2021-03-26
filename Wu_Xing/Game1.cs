using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Wu_Xing
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private SpriteFont normalFont;

        private Rectangle window;
        
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            IsMouseVisible = true;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            normalFont = Content.Load<SpriteFont>("Normal");

            window.Width = 1280;
            window.Height = 720;

            graphics.PreferredBackBufferWidth = window.Width;
            graphics.PreferredBackBufferHeight = window.Height;
            graphics.ApplyChanges();
        }

        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Keyboard.GetState().IsKeyDown(Keys.Space) ? Color.DarkGray : Color.DarkCyan);
            spriteBatch.Begin();

            spriteBatch.DrawString(normalFont, "Hold space to change background color", new Vector2(100, 100), Color.White);

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}

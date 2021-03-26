using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Prototype_Room
{
    public class Game1 : Game
    {

        // sup
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        LevelManager LevelM;
        public static int life;
        public static int food;
        SpriteFont screenText;
        Player player;
        Door door;
        //public static Random rdm;
        Animation ani;

        enum GameState
        {
            Start,
            LevelOne,
            LevelTwo,
            Win,
            GameOver,
        }
        GameState currentGameState;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            //rdm = new Random();
            TextureManager.LoadTextures(Content);
            LevelM = new LevelManager("Level1.txt");
            //Level2 = new LevelManager("Level2.txt");
            screenText = Content.Load<SpriteFont>(@"SpriteFontFX");
            currentGameState = GameState.Start;
            life = 3;
            food = 138;
            graphics.PreferredBackBufferHeight = 800;
            graphics.PreferredBackBufferWidth = 1000;

        }

        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            //spelet har olika gamestates
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            switch (currentGameState)
            {
                case GameState.Start:
                    if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                    {
                        currentGameState = GameState.LevelOne;

                    }
                    break;
                case GameState.LevelOne:
                    LevelM.Update(gameTime);
                    if (life <= 0)
                    {
                        currentGameState = GameState.GameOver;
                    }
                    else if (food <= 0)
                    {
                        currentGameState = GameState.Win;
                    }
                    if (player.Hitbox.Intersects(door.hitbox))
                    {
                        currentGameState = GameState.LevelTwo;
                    }
                    break;
                case GameState.LevelTwo:
                    LevelM.Update(gameTime);
                    LevelM = new LevelManager("Level2.txt");
                    break;

                case GameState.GameOver:
                    if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                    {
                        life = 3;
                        food = 138;
                        currentGameState = GameState.LevelOne;
                        //player.Position = player.reStart;
                    }
                    break;
                case GameState.Win:
                    if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                    {
                        life = 3;
                        food = 138;
                        currentGameState = GameState.LevelOne;
                        //player.Position = player.reStart;
                    }
                    break;
                    base.Update(gameTime);
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            //ritar utt olike gamestate
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            switch (currentGameState)
            {
                case GameState.Start:


                    spriteBatch.DrawString(screenText, "Press 'Enter' to start!", new Vector2(300, 200), Color.Yellow);
                    //spriteBatch.DrawString(screenText, "Press 'Enter' to start!", new Vector2(180, (Window.ClientBounds.Height / 2 - 100)), Color.Pink);
                    break;
                case GameState.LevelOne:
                    //spriteBatch.DrawString(screenText, "Lives " + life, new Vector2(300, 670), Color.Yellow);
                    //spriteBatch.DrawString(screenText, "Food remains " + food, new Vector2(400, 670), Color.Yellow);
                    LevelM.Draw(spriteBatch);
                    break;
                case GameState.GameOver:

                    spriteBatch.DrawString(screenText, " Game Over! Press enter to play again", new Vector2(300, 200), Color.Yellow);
                    //spriteBatch.DrawString(screenText, "Press enter to play again", new Vector2(180, (Window.ClientBounds.Height / 2 - 100)), Color.Pink);
                    break;
                case GameState.Win:

                    spriteBatch.DrawString(screenText, "You Win!, Press enter to play again", new Vector2(300, 200), Color.Yellow);
                    break;
            }
            graphics.ApplyChanges();
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}

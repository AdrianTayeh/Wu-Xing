using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Wu_Xing
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private Rectangle window;

        private Start start;
        private Menu menu;
        private Settings settings;
        private Pregame pregame;
        private Stats stats;
        private NewGame newGame;
        private Running running;

        Screen screen;

        MouseState currentMouseState;
        MouseState previousMouseState;
        KeyboardState currentKeyboardState;
        KeyboardState previousKeyboardState;
        
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

            window.Width = 1920;
            window.Height = 1080;

            graphics.PreferredBackBufferWidth = window.Width;
            graphics.PreferredBackBufferHeight = window.Height;
            graphics.ApplyChanges();

            TextureLibrary.Load(Content, GraphicsDevice);
            FontLibrary.Load(Content);
            screen = Screen.Start;

            start = new Start();
            menu = new Menu(window);
            settings = new Settings(window);
            pregame = new Pregame(window);
            stats = new Stats(window);
            newGame = new NewGame(window);
            running = new Running(window);
        }

        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            previousMouseState = currentMouseState;
            previousKeyboardState = currentKeyboardState;
            currentMouseState = Mouse.GetState();
            currentKeyboardState = Keyboard.GetState();

            if (currentKeyboardState.IsKeyDown(Keys.Escape))
                Exit();

            switch (screen)
            {
                case Screen.Start:
                    start.Update(ref screen, currentMouseState, previousMouseState, currentKeyboardState, previousKeyboardState);
                    break;

                case Screen.Menu:
                    menu.Update(ref screen, currentMouseState, previousMouseState, currentKeyboardState, previousKeyboardState);
                    break;

                case Screen.Settings:
                    settings.Update(ref screen, currentMouseState, previousMouseState, currentKeyboardState, previousKeyboardState);
                    break;

                case Screen.Pregame:
                    pregame.Update(ref screen, currentMouseState, previousMouseState, currentKeyboardState, previousKeyboardState);
                    break;

                case Screen.Stats:
                    stats.Update(ref screen, currentMouseState, previousMouseState, currentKeyboardState, previousKeyboardState);
                    break;

                case Screen.NewGame:
                    newGame.Update(ref screen, currentMouseState, previousMouseState, currentKeyboardState, previousKeyboardState);
                    break;

                case Screen.Running:
                    running.Update(ref screen, currentMouseState, previousMouseState, currentKeyboardState, previousKeyboardState);
                    break;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();

            switch (screen)
            {
                case Screen.Start:
                    start.Draw(spriteBatch, window);
                    break;

                case Screen.Menu:
                    menu.Draw(spriteBatch, window);
                    break;

                case Screen.Settings:
                    settings.Draw(spriteBatch, window);
                    break;

                case Screen.Pregame:
                    pregame.Draw(spriteBatch, window);
                    break;

                case Screen.Stats:
                    stats.Draw(spriteBatch, window);
                    break;

                case Screen.NewGame:
                    newGame.Draw(spriteBatch, window);
                    break;

                case Screen.Running:
                    running.Draw(spriteBatch, window);
                    break;
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}

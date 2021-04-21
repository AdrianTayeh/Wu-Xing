using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Monolights;

namespace Wu_Xing
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private Random random;
        Monolights.Monolights _monoLights;
        private Effect _lightEffect;
        private Effect _deferrectLightEffect;

        private Rectangle window;
        private Rectangle resolution;
        private float windowScale;
        private RenderTarget2D game;

        private Start start;
        private Menu menu;
        private Settings settings;
        private Pregame pregame;
        private Stats stats;
        private NewGame newGame;
        private Running running;

        private Screen screen;
        private Screen previousScreen;

        private Mouse mouse;
        private KeyboardState currentKeyboard;
        private KeyboardState previousKeyboard;

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
            random = new Random();

            window.Width = 1920;
            window.Height = 1080;
            resolution.Width = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            resolution.Height = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

            graphics.PreferredBackBufferWidth = resolution.Width;
            graphics.PreferredBackBufferHeight = resolution.Height;
            graphics.IsFullScreen = true;
            graphics.ApplyChanges();

            game = new RenderTarget2D(GraphicsDevice, window.Width, window.Height);

            windowScale = (float)resolution.Height / resolution.Width >= (float)window.Height / window.Width ? (float)resolution.Width / window.Width : (float)resolution.Height / window.Height;

            TextureLibrary.Load(Content, GraphicsDevice);
            FontLibrary.Load(Content);
            ColorLibrary.Load();

            screen = Screen.Start;
            mouse = new Mouse(window, resolution, windowScale);

            start = new Start();
            menu = new Menu(window);
            settings = new Settings(window);
            pregame = new Pregame(window);
            stats = new Stats(window);
            newGame = new NewGame(window, GraphicsDevice);
            running = new Running(window);

            _lightEffect = Content.Load<Effect>("LightEffect");
            _deferrectLightEffect = Content.Load<Effect>("DeferredLightEffect");
            _monoLights = new Monolights.Monolights(GraphicsDevice, _lightEffect, _deferrectLightEffect);
            _monoLights.InvertYNormal = false;

        }

        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            previousKeyboard = currentKeyboard;
            currentKeyboard = Keyboard.GetState();

            mouse.Update();

            if (currentKeyboard.IsKeyUp(Keys.Escape) && previousKeyboard.IsKeyDown(Keys.Escape) && screen == Screen.Start)
                Exit();

            switch (screen)
            {
                case Screen.Start:
                    start.Update(ref screen, mouse, currentKeyboard, previousKeyboard);
                    break;

                case Screen.Menu:
                    menu.Update(ref screen, ref previousScreen, mouse, currentKeyboard, previousKeyboard);
                    break;

                case Screen.Settings:
                    settings.Update(ref screen, previousScreen, mouse, currentKeyboard, previousKeyboard, graphics);
                    break;

                case Screen.Pregame:
                    pregame.Update(ref screen, mouse, currentKeyboard, previousKeyboard, newGame, random, running.MapInitialized);
                    break;

                case Screen.Stats:
                    stats.Update(ref screen, mouse, currentKeyboard, previousKeyboard);
                    break;

                case Screen.NewGame:
                    newGame.Update(ref screen, mouse, currentKeyboard, previousKeyboard, random, running, gameTime);
                    break;

                case Screen.Running:
                    running.Update(ref screen, ref previousScreen, mouse, currentKeyboard, previousKeyboard, gameTime, random);
                    break;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            //Pre-draw to render targets

            if (screen == Screen.NewGame)
                newGame.DrawCircleToTexture(spriteBatch, GraphicsDevice);

            //Render game

            GraphicsDevice.SetRenderTarget(game);
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

            //Scale rendered game to screen

            GraphicsDevice.SetRenderTarget(_monoLights.Colormap);
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();

            spriteBatch.Draw(game, resolution.Size.ToVector2() / 2, null, Color.White, 0, window.Size.ToVector2() / 2, windowScale, SpriteEffects.None, 0);

            spriteBatch.End();

            _monoLights.Draw(null, spriteBatch, new Rectangle(resolution.X, resolution.Y, resolution.Width, resolution.Height));

            base.Draw(gameTime);
        }
    }
}

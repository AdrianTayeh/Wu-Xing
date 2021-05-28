using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace Wu_Xing
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private Random random;

        private Rectangle window;
        private Rectangle resolution;
        private float windowScale;
        private RenderTarget2D world;
        private RenderTarget2D HUD;
        private Camera camera;

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

            Window.IsBorderless = true;
            graphics.PreferredBackBufferWidth = resolution.Width;
            graphics.PreferredBackBufferHeight = resolution.Height;
            //graphics.IsFullScreen = true;
            graphics.ApplyChanges();
            
            world = new RenderTarget2D(GraphicsDevice, window.Width, window.Height);
            HUD = new RenderTarget2D(GraphicsDevice, window.Width, window.Height);

            windowScale = (float)resolution.Height / resolution.Width >= (float)window.Height / window.Width ? (float)resolution.Width / window.Width : (float)resolution.Height / window.Height;

            TextureLibrary.Load(Content, GraphicsDevice);
            FontLibrary.Load(Content);
            ColorLibrary.Load();
            SoundLibrary.Load(Content);

            screen = Screen.Start;
            mouse = new Mouse(window, resolution, windowScale);
            camera = new Camera(window);

            start = new Start();
            menu = new Menu(window);
            settings = new Settings(window);
            pregame = new Pregame(window);
            stats = new Stats(window);
            newGame = new NewGame(window, GraphicsDevice);
            running = new Running(window);
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
                    settings.Update(ref screen, previousScreen, mouse, currentKeyboard, previousKeyboard, graphics, window, ref resolution, ref windowScale);
                    mouse.UpdateSettings(window, resolution, windowScale);
                    break;

                case Screen.Pregame:
                    pregame.Update(ref screen, mouse, currentKeyboard, previousKeyboard, newGame, running.MapInitialized);
                    break;

                case Screen.Stats:
                    stats.Update(ref screen, mouse, currentKeyboard, previousKeyboard);
                    break;

                case Screen.NewGame:
                    newGame.Update(ref screen, mouse, currentKeyboard, previousKeyboard, random, running, (float)gameTime.ElapsedGameTime.TotalSeconds, GraphicsDevice);
                    break;

                case Screen.Running:
                    running.Update(ref screen, ref previousScreen, mouse, currentKeyboard, previousKeyboard, (float)gameTime.ElapsedGameTime.TotalSeconds, random, GraphicsDevice);
                    break;
            }

            if (screen == Screen.Running)
                camera.UpdateFocus(running.CameraFocus, running.CurrentRoomSize, running.LimitCameraFocusToBounds);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            //Pre-draw to render targets

            if (screen == Screen.NewGame)
                newGame.DrawCircleToTexture(spriteBatch, GraphicsDevice);

            else if (screen == Screen.Running && running.MapInitialized)
                running.DrawFullMinimap(spriteBatch, GraphicsDevice);

            //Render world

            // 0.1 Room
            // 0.11 Tiles
            // 0.12 Door Bottoms
            // 0.13 Door Fronts
            // 0.3 Character Shadows
            // 0.4 Projectiles
            // 0.5 Character Bottoms (Secondary)
            // 0.51 Character Tops (Primary)
            // 0.9 Door Tops

            if (screen == Screen.Running)
            {
                GraphicsDevice.SetRenderTarget(world);
                GraphicsDevice.Clear(Color.Transparent);
                spriteBatch.Begin(SpriteSortMode.FrontToBack, null, null, null, null, null, camera.Matrix);

                running.DrawWorld(spriteBatch);

                spriteBatch.End();
            }

            //Render HUD and menu

            GraphicsDevice.SetRenderTarget(HUD);
            GraphicsDevice.Clear(Color.Transparent);
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
                    running.DrawHUD(spriteBatch, window);
                    break;
            }

            spriteBatch.End();

            //Scale rendered game to screen

            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();

            spriteBatch.Draw(world, resolution.Size.ToVector2() / 2, null, Color.White, 0, window.Size.ToVector2() / 2, windowScale, SpriteEffects.None, 0);
            spriteBatch.Draw(HUD, resolution.Size.ToVector2() / 2, null, Color.White, 0, window.Size.ToVector2() / 2, windowScale, SpriteEffects.None, 0);

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
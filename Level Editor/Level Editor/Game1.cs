using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Level_Editor
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private Rectangle window;
        private Rectangle resolution;
        private Menu menu;
        private Mouse mouse;
        private KeyboardState currentKeyboard;
        private float windowScale;
        public enum RoomSize {Default, OneXOne, OneXTwo, OneXThree, TwoXOne, ThreeXOne, TwoXTwo }
        private RoomSize roomSize;
        private Point size;
        public enum Screen { Menu, Map }
        private Screen screen;
        private Camera camera;
        private Vector2 cameraPosition;
        private SpriteBatch cameraSpriteBatch;
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
            cameraSpriteBatch = new SpriteBatch(GraphicsDevice);
            window.Width = 1920;
            window.Height = 1080;
            resolution.Width = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            resolution.Height = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

            graphics.PreferredBackBufferWidth = resolution.Width;
            graphics.PreferredBackBufferHeight = resolution.Height;
            graphics.ApplyChanges();

            windowScale = (float)resolution.Height / resolution.Width >= (float)window.Height / window.Width ? (float)resolution.Width / window.Width : (float)resolution.Height / window.Height;


            TextureLibrary.Load(Content, GraphicsDevice);
            FontLibrary.Load(Content);
            ColorLibrary.Load();
            menu = new Menu(window);
            mouse = new Mouse(window, resolution, windowScale);
            camera = new Camera(window);
            cameraPosition = new Vector2((size.X * 1500) / 2, (size.Y * 700) / 2);


            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            mouse.Update();
            menu.Update(ref roomSize, mouse, ref screen);
            currentKeyboard = Keyboard.GetState();


            if (screen == Screen.Map)
            {
                if (currentKeyboard.IsKeyDown(Keys.W))
                    cameraPosition.Y -= 5;
                else if (currentKeyboard.IsKeyDown(Keys.S))
                    cameraPosition.Y += 5;
                else if (currentKeyboard.IsKeyDown(Keys.A))
                    cameraPosition.X -= 5;
                else if (currentKeyboard.IsKeyDown(Keys.D))
                    cameraPosition.X += 5;

                camera.UpdateFocus(cameraPosition);
            }

            switch (roomSize)
            {
                case RoomSize.OneXOne:
                    size.X = 1;
                    size.Y = 1;
                    break;

                case RoomSize.OneXTwo:
                    size.X = 1;
                    size.Y = 2;
                    break;

                case RoomSize.OneXThree:
                    size.X = 1;
                    size.Y = 3;
                    break;

                case RoomSize.TwoXOne:
                    size.X = 2;
                    size.Y = 1;
                    break;

                case RoomSize.ThreeXOne:
                    size.X = 3;
                    size.Y = 1;
                    break;

                case RoomSize.TwoXTwo:
                    size.X = 2;
                    size.Y = 2;
                    break;
            }

            


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            Vector2 position = new Vector2((-size.X * 1500) / 2, (-size.Y * 700) / 2);
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            if(screen == Screen.Menu)
                menu.Draw(spriteBatch, window);           
            spriteBatch.End();
            cameraSpriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, camera.Matrix);
            if (screen == Screen.Map)
                cameraSpriteBatch.Draw(TextureLibrary.Rooms[size.X + "x" + size.Y], position, null, Color.FromNonPremultiplied(60, 60, 60, 255), 0, Vector2.Zero, 1, SpriteEffects.None, 0.1f);
            cameraSpriteBatch.End();
            base.Draw(gameTime);
        }
    }
}

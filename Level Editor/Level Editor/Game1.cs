using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Level_Editor
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private RenderTarget2D mapRenderTarget;
        private RenderTarget2D menuRenderTarget;

        private Rectangle window;
        private Rectangle resolution;
        private float windowScale;

        private Menu menu;
        private Tile tile;

        private Mouse mouse;
        private KeyboardState currentKeyboard;

        public enum RoomSize {Default, OneXOne, OneXTwo, OneXThree, TwoXOne, ThreeXOne, TwoXTwo }
        private RoomSize roomSize;
        private Point size;

        public enum Screen { Menu, Map }
        private Screen screen;

        private Camera camera;
        private Vector2 cameraPosition;
        private SaveRoom saveRoom;

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
            resolution.Width = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            resolution.Height = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

            graphics.PreferredBackBufferWidth = resolution.Width;
            graphics.PreferredBackBufferHeight = resolution.Height;
            graphics.ApplyChanges();

            mapRenderTarget = new RenderTarget2D(GraphicsDevice, window.Width, window.Height);
            menuRenderTarget = new RenderTarget2D(GraphicsDevice, window.Width, window.Height);

            windowScale = (float)resolution.Height / resolution.Width >= (float)window.Height / window.Width ? (float)resolution.Width / window.Width : (float)resolution.Height / window.Height;

            TextureLibrary.Load(Content, GraphicsDevice);
            FontLibrary.Load(Content);
            ColorLibrary.Load();

            menu = new Menu(window);
            mouse = new Mouse(window, resolution, windowScale);
            camera = new Camera(window);
            tile = new Tile();
            saveRoom = new SaveRoom();
            cameraPosition = TextureLibrary.Rooms["1x1"].Bounds.Size.ToVector2() / 2;
        }

        protected override void Update(GameTime gameTime)
        {
            mouse.Update();
            //if (currentKeyboard.IsKeyDown(Keys.LeftControl) && currentKeyboard.IsKeyDown(Keys.S))
            saveRoom.Save(currentKeyboard, ref tile.tilePosList, ref roomSize);
            currentKeyboard = Keyboard.GetState();

            if (currentKeyboard.IsKeyDown(Keys.Escape))
                Exit();

            if (screen == Screen.Map)
            {
                if (currentKeyboard.IsKeyDown(Keys.Up))
                    cameraPosition.Y -= 5;
                else if (currentKeyboard.IsKeyDown(Keys.Down))
                    cameraPosition.Y += 5;
                if (currentKeyboard.IsKeyDown(Keys.Left))
                    cameraPosition.X -= 5;
                else if (currentKeyboard.IsKeyDown(Keys.Right   ))
                    cameraPosition.X += 5;

                camera.UpdateFocus(cameraPosition);
                tile.Update(ref screen, mouse, camera);
            }

            else
            {
                menu.Update(ref roomSize, mouse, ref screen);
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
            //Render map
            if (screen == Screen.Map)
            {
                GraphicsDevice.SetRenderTarget(mapRenderTarget);
                GraphicsDevice.Clear(Color.Transparent);
                spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, camera.Matrix);


                spriteBatch.Draw(TextureLibrary.Rooms[size.X + "x" + size.Y], Vector2.Zero, null, Color.FromNonPremultiplied(60, 60, 60, 255), 0, Vector2.Zero, 1, SpriteEffects.None, 0.1f);
                tile.DrawWorld(spriteBatch);
                spriteBatch.End();
            }

            //Render menu and HUD
            GraphicsDevice.SetRenderTarget(menuRenderTarget);
            GraphicsDevice.Clear(Color.Transparent);
            spriteBatch.Begin();

            if (screen == Screen.Menu)
                menu.Draw(spriteBatch, window);

            else
                tile.DrawHUD(spriteBatch);

            spriteBatch.End();

            //Scale rendered layer to screen
            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();

            spriteBatch.Draw(mapRenderTarget, resolution.Size.ToVector2() / 2, null, Color.White, 0, window.Size.ToVector2() / 2, windowScale, SpriteEffects.None, 0);
            spriteBatch.Draw(menuRenderTarget, resolution.Size.ToVector2() / 2, null, Color.White, 0, window.Size.ToVector2() / 2, windowScale, SpriteEffects.None, 0);

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}

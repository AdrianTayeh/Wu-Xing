using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wu_Xing
{
    class Running
    {
        private Adam adam;
        private enum State { Running, Paused, Transition }
        private State gameState;

        private float seconds;
        private int minutes;
        private int hours;

        private Dictionary<string, Button> button = new Dictionary<string, Button>();

        private MapManager mapManager;
        //private List<GameObject> gameObjects = new List<GameObject>();

        public Running(Rectangle window)
        {
            adam = new Adam(window);
            mapManager = new MapManager();

            button.Add("Continue", new Button(
                new Point(window.Width / 2, window.Height / 2 - 90),
                new Point(260, 70),
                "CONTINUE", FontLibrary.Normal,
                TextureLibrary.WhitePixel, null,
                ColorLibrary.WhiteButtonBackgroundColor,
                ColorLibrary.WhiteButtonLabelColor
                ));

            button.Add("Settings", new Button(
                new Point(window.Width / 2, window.Height / 2),
                new Point(260, 70),
                "SETTINGS", FontLibrary.Normal,
                TextureLibrary.WhitePixel, null,
                ColorLibrary.WhiteButtonBackgroundColor,
                ColorLibrary.WhiteButtonLabelColor
                ));

            button.Add("Menu", new Button(
                new Point(window.Width / 2, window.Height / 2 + 90),
                new Point(260, 70),
                "MENU", FontLibrary.Normal,
                TextureLibrary.WhitePixel, null,
                ColorLibrary.WhiteButtonBackgroundColor,
                ColorLibrary.WhiteButtonLabelColor
                ));
        }

        public Vector2 CameraFocus { get { return mapManager.TransitionPosition == Vector2.Zero ? adam.Position : mapManager.TransitionPosition; } }
        public bool LimitCameraFocusToBounds { get { return mapManager.TransitionPosition == Vector2.Zero; } }
        public Point CurrentRoomSize { get { return mapManager == null ? new Point(1, 1) : mapManager.CurrentRoom.Size; } }
        public bool MapInitialized { get { return mapManager != null; } }

        public void InitializeNewMap(Random random, int size, Element element)
        {
            mapManager.GenerateNewMap(random, size, element);
            adam.Position = mapManager.CenterOfCenterRoom;
        }

        public void Update(ref Screen screen, ref Screen previousScreen, Mouse mouse, KeyboardState currentKeyboard, KeyboardState previousKeyboard, GameTime gameTime, Random random, Rectangle window)
        {
            if (currentKeyboard.IsKeyUp(Keys.Escape) && previousKeyboard.IsKeyDown(Keys.Escape))
                gameState = gameState == State.Running ? State.Paused : State.Running;

            else if (currentKeyboard.IsKeyUp(Keys.R) && previousKeyboard.IsKeyDown(Keys.R))
                InitializeNewMap(random, mapManager.Size, mapManager.Element);

            switch (gameState)
            {
                case State.Running:
                    UpdateRunning(currentKeyboard, gameTime, window);
                    break;

                case State.Paused:
                    UpdatePaused(ref screen, ref previousScreen, mouse);
                    break;

                case State.Transition:
                    UpdateTransition();
                    break;
            }
        }

        private void UpdateRunning(KeyboardState currentKeyboard, GameTime gameTime, Rectangle window)
        {
            adam.Update(currentKeyboard, mapManager, window);
            UpdateTimer(gameTime);

            if (mapManager.Transition)
                gameState = State.Transition;
        }

        private void UpdatePaused(ref Screen screen, ref Screen previousScreen, Mouse mouse)
        {
            foreach (KeyValuePair<string, Button> item in button)
                item.Value.Update(mouse);

            if (button["Continue"].IsReleased)
            {
                gameState = State.Running;
            }

            else if (button["Settings"].IsReleased)
            {
                previousScreen = screen;
                screen = Screen.Settings;
            }

            else if (button["Menu"].IsReleased)
            {
                screen = Screen.Pregame;
            }
        }

        private void UpdateTransition()
        {
            if (mapManager.Transition)
                return;

            gameState = State.Running;
            adam.Position = adam.ExitPosition;
        }

        private void UpdateTimer(GameTime gameTime)
        {
            seconds += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (seconds >= 60)
            {
                seconds %= 60;
                minutes += 1;

                if (minutes == 60)
                {
                    minutes = 0;
                    hours += 1;
                }
            }
        }

        public void DrawWorld(SpriteBatch spriteBatch)
        {
            mapManager.DrawWorld(spriteBatch);
            adam.Draw(spriteBatch);
        }

        public void DrawHUD(SpriteBatch spriteBatch, Rectangle window)
        {
            mapManager.DrawFullMap(spriteBatch);
            adam.DrawHearts(spriteBatch);

            if (gameState == State.Paused)
            {
                spriteBatch.Draw(TextureLibrary.WhitePixel, window, Color.FromNonPremultiplied(0, 0, 0, 150));
                spriteBatch.DrawString(FontLibrary.Normal, "PAUSED", new Vector2(window.Width / 2, 190), Color.White, 0, FontLibrary.Normal.MeasureString("PAUSED") / 2, 1, SpriteEffects.None, 0);

                string time = hours + ":" + minutes.ToString("00") + ":" + seconds.ToString("00");
                spriteBatch.DrawString(FontLibrary.Huge, time, new Vector2(window.Width / 2, 250), Color.White, 0, FontLibrary.Huge.MeasureString(time) / 2, 1, SpriteEffects.None, 0);

                foreach (KeyValuePair<string, Button> item in button)
                    item.Value.Draw(spriteBatch);
            }
        }
    }
}

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
        private enum State { Running, Paused, Transition, GameOver }
        private State gameState;

        private float seconds;
        private int minutes;
        private int hours;

        private Dictionary<string, Button> button = new Dictionary<string, Button>();

        private MapManager mapManager;
        private List<GameObject> gameObjects = new List<GameObject>();
        private bool drawHitbox;
        private bool drawKeyBindings;

        public Running(Rectangle window)
        {
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
        public bool MapInitialized { get { return mapManager.Rooms != null; } }

        public void InitializeNewMap(GraphicsDevice GraphicsDevice, Random random, int size, Element gemToFind, Element elementToChannel)
        {
            mapManager.GenerateNewMap(GraphicsDevice, random, size, gemToFind);
            adam = new Adam(mapManager.CenterOfCenterRoom, elementToChannel);
        }

        public void Update(ref Screen screen, ref Screen previousScreen, Mouse mouse, KeyboardState currentKeyboard, KeyboardState previousKeyboard, float elapsedSeconds, Random random, Rectangle window, GraphicsDevice GraphicsDevice)
        {
            CheckKeyboardInput(currentKeyboard, previousKeyboard, random, GraphicsDevice);

            switch (gameState)
            {
                case State.Running:
                    UpdateRunning(currentKeyboard, previousKeyboard, elapsedSeconds, window);
                    break;

                case State.Paused:
                    UpdatePaused(ref screen, ref previousScreen, mouse);
                    break;

                case State.Transition:
                    UpdateTransition();
                    break;

                case State.GameOver:

                    break;
            }
        }

        private void CheckKeyboardInput(KeyboardState currentKeyboard, KeyboardState previousKeyboard, Random random, GraphicsDevice GraphicsDevice)
        {
            //Escape - Toggle pause
            if (currentKeyboard.IsKeyUp(Keys.Escape) && previousKeyboard.IsKeyDown(Keys.Escape))
                gameState = gameState == State.Running ? State.Paused : State.Running;

            //R - Reset run
            else if (currentKeyboard.IsKeyDown(Keys.R) && previousKeyboard.IsKeyUp(Keys.R))
                InitializeNewMap(GraphicsDevice, random, mapManager.Size, mapManager.Element, (Element)adam.Element);

            //H - Toggle draw hitbox
            else if (currentKeyboard.IsKeyDown(Keys.H) && previousKeyboard.IsKeyUp(Keys.H))
                drawHitbox = !drawHitbox;

            //T - Toggle draw tips
            else if (currentKeyboard.IsKeyDown(Keys.K) && previousKeyboard.IsKeyUp(Keys.K))
                drawKeyBindings = !drawKeyBindings;
        }

        private void UpdateRunning(KeyboardState currentKeyboard, KeyboardState previousKeyboard, float elapsedSeconds, Rectangle window)
        {
            //Update adam and other characters
            adam.Update(elapsedSeconds, gameObjects, adam, currentKeyboard, mapManager);

            foreach (GameObject gameObject in gameObjects)
                gameObject.Update(elapsedSeconds, gameObjects, adam, currentKeyboard, mapManager);

            //Remove dead characters from gameObjects list
            for (int i = gameObjects.Count - 1; i >= 0; i--)
                if (gameObjects[i] is Character)
                    if (((Character)gameObjects[i]).IsDead)
                        gameObjects.RemoveAt(i);

            mapManager.Update(currentKeyboard, previousKeyboard);
            UpdateTimer(elapsedSeconds);

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
            adam.MoveTo(adam.ExitPosition);
        }

        private void UpdateTimer(float elapsedSeconds)
        {
            seconds += elapsedSeconds;
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

        public void DrawFullMinimap(SpriteBatch spriteBatch, GraphicsDevice GraphicsDevice)
        {
            mapManager.DrawFullMinimap(spriteBatch, GraphicsDevice);
        }

        public void DrawWorld(SpriteBatch spriteBatch)
        {
            mapManager.DrawWorld(spriteBatch, drawHitbox);
            adam.Draw(spriteBatch, Vector2.Zero, drawHitbox);

            foreach (GameObject gameObject in gameObjects)
                gameObject.Draw(spriteBatch, Vector2.Zero, drawHitbox);
        }

        public void DrawHUD(SpriteBatch spriteBatch, Rectangle window)
        {
            mapManager.DrawMinimap(spriteBatch, window);
            adam.DrawHearts(spriteBatch);

            if (drawKeyBindings)
            {
                spriteBatch.DrawString(FontLibrary.Normal, "K: Show key bindings \nR: Restart \nH: Show hitboxes \nEsc: Pause \nTab: Additional UI", new Vector2(100, 250), Color.LightBlue);
                spriteBatch.DrawString(FontLibrary.Normal, "LShift: Increased speed", new Vector2(100, 250 + FontLibrary.Normal.LineSpacing * 6), Color.OrangeRed);
            }

            else
                spriteBatch.DrawString(FontLibrary.Normal, "K: Show key bindings", new Vector2(100, 250), Color.LightBlue);

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

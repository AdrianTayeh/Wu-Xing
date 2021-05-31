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
        private enum State { Running, Paused, Transition, GameOver }
        private State gameState;

        private float seconds;
        private int minutes;
        private int hours;

        private Dictionary<string, Button> button = new Dictionary<string, Button>();
        private MapManager mapManager;

        private bool drawKeyBindings;
        private bool extendedUI;
        private float extendedUITransition;

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

        public Vector2 CameraFocus { get { return mapManager.TransitionPosition == Vector2.Zero ? mapManager.Adam.Position : mapManager.TransitionPosition; } }
        public bool LimitCameraFocusToBounds { get { return mapManager.TransitionPosition == Vector2.Zero; } }
        public Point CurrentRoomSize { get { return mapManager == null ? new Point(1, 1) : mapManager.CurrentRoom.Size; } }
        public bool MapInitialized { get { return mapManager.Rooms != null; } }

        public void InitializeNewMap(GraphicsDevice GraphicsDevice, Random random, int size, Element gemToFind, Element elementToChannel)
        {
            mapManager.GenerateNewMap(GraphicsDevice, random, size, gemToFind, elementToChannel);
        }

        public void Update(ref Screen screen, ref Screen previousScreen, Mouse mouse, KeyboardState currentKeyboard, KeyboardState previousKeyboard, float elapsedSeconds, Random random)
        {
            CheckKeyboardInput(ref screen, currentKeyboard, previousKeyboard, random);
            UpdateExtendedUITransition();

            switch (gameState)
            {
                case State.Running:
                    UpdateRunning(currentKeyboard, previousKeyboard, elapsedSeconds, random);
                    break;

                case State.Paused:
                    UpdatePaused(ref screen, ref previousScreen, mouse);
                    break;

                case State.Transition:
                    if (!mapManager.Transition)
                        gameState = State.Running;
                    break;

                case State.GameOver:

                    break;
            }

            PlayingBackgroundMusic();
        }

        private void UpdateExtendedUITransition()
        {
            if (extendedUI)
                extendedUITransition += extendedUITransition < 1 ? 0.1f : 0;

            else
                extendedUITransition -= extendedUITransition > 0 ? 0.1f : 0;
        }

        private void PlayingBackgroundMusic()
        {
            if (gameState == State.Running || gameState == State.Transition)
            {
                SoundLibrary.BackgroundMusicInstance.Play();
                SoundLibrary.BackgroundMusicInstance.IsLooped = true;
            } 
            else
            {
                SoundLibrary.BackgroundMusicInstance.IsLooped = false;
                SoundLibrary.BackgroundMusicInstance.Stop();
            }
        }

        private void CheckKeyboardInput(ref Screen screen, KeyboardState currentKeyboard, KeyboardState previousKeyboard, Random random)
        {
            //Escape - Toggle pause
            if (currentKeyboard.IsKeyUp(Keys.Escape) && previousKeyboard.IsKeyDown(Keys.Escape))
            {
                if (gameState == State.Running || gameState == State.Paused)
                {
                    gameState = gameState == State.Running ? State.Paused : State.Running;
                    SoundLibrary.BackgroundMusicInstance.Pause();
                }
                    
                else if (gameState == State.GameOver)
                {
                    screen = Screen.Pregame;
                    mapManager.NullMap();
                }  
            }

            //R - Reset run
            else if (currentKeyboard.IsKeyDown(Keys.R) && previousKeyboard.IsKeyUp(Keys.R))
            {
                gameState = State.Running;
                mapManager.RegenerateMap(random);
            } 

            //T - Toggle draw tips
            else if (currentKeyboard.IsKeyDown(Keys.K) && previousKeyboard.IsKeyUp(Keys.K))
            {
                drawKeyBindings = !drawKeyBindings;
            }

            //K - Toggle extended UI
            else if (currentKeyboard.IsKeyDown(Keys.Tab) && previousKeyboard.IsKeyUp(Keys.Tab))
                extendedUI = !extendedUI;
        }

        private void UpdateRunning(KeyboardState currentKeyboard, KeyboardState previousKeyboard, float elapsedSeconds, Random random)
        {
            mapManager.Update(elapsedSeconds, currentKeyboard, previousKeyboard, random, extendedUITransition);
            UpdateTimer(elapsedSeconds);

            if (mapManager.Adam.IsDead)
                gameState = State.GameOver;

            else if (mapManager.Transition)
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
            mapManager.DrawWorld(spriteBatch);
        }

        public void DrawHUD(SpriteBatch spriteBatch, Rectangle window)
        {
            spriteBatch.Draw(TextureLibrary.Filter, Vector2.Zero, null, Color.White);
            mapManager.DrawMinimap(spriteBatch, window, extendedUITransition);
            mapManager.Adam.DrawHearts(spriteBatch);
            mapManager.DrawRealmDescription(spriteBatch, window, extendedUITransition);
            mapManager.DrawEffectiveness(spriteBatch, window, extendedUITransition);

            if (drawKeyBindings)
            {
                spriteBatch.DrawString(FontLibrary.Normal, "K: Show key bindings \nR: Restart \nH: Show hitboxes \nEsc: Pause \nTab: Extended UI", new Vector2(100, 220), Color.White);
                spriteBatch.DrawString(FontLibrary.Normal, "LShift: Increased speed \n1-9: Attributes presets \n    1: Base \n    2: Upgraded \n    3: Slow  \n    4: Sniper \n    5: Laser \n    6: Rainbow laser \n    7: Machine gun \n    8: Big machine gun \n    9: Big rainbow machinegun \n    0: Chaos", new Vector2(100, 220 + FontLibrary.Normal.LineSpacing * 6), Color.Yellow);
            }

            else
            {
                spriteBatch.DrawString(FontLibrary.Normal, "K: Show key bindings", new Vector2(100, 220), Color.FromNonPremultiplied(255, 255, 255, 60));
            } 

            if (gameState == State.Paused)
            {
                spriteBatch.Draw(TextureLibrary.WhitePixel, window, Color.FromNonPremultiplied(0, 0, 0, 150));
                spriteBatch.DrawString(FontLibrary.Normal, "PAUSED", new Vector2(window.Width / 2, 190), Color.White, 0, FontLibrary.Normal.MeasureString("PAUSED") / 2, 1, SpriteEffects.None, 0);

                string time = hours + ":" + minutes.ToString("00") + ":" + seconds.ToString("00");
                spriteBatch.DrawString(FontLibrary.Huge, time, new Vector2(window.Width / 2, 250), Color.White, 0, FontLibrary.Huge.MeasureString(time) / 2, 1, SpriteEffects.None, 0);

                foreach (KeyValuePair<string, Button> item in button)
                    item.Value.Draw(spriteBatch);
            }

            else if (gameState == State.GameOver)
            {
                spriteBatch.Draw(TextureLibrary.WhitePixel, window, Color.FromNonPremultiplied(0, 0, 0, 150));
                spriteBatch.DrawString(FontLibrary.Huge, "YOU DIED", new Vector2(window.Width / 2, 450), Color.White, 0, FontLibrary.Huge.MeasureString("YOU DIED") / 2, 1, SpriteEffects.None, 0);
                spriteBatch.DrawString(FontLibrary.Normal, "PRESS R TO RESTART", new Vector2(window.Width / 2, 530), Color.White, 0, FontLibrary.Normal.MeasureString("PRESS R TO RESTART") / 2, 1, SpriteEffects.None, 0);
                spriteBatch.DrawString(FontLibrary.Normal, "OR ESC TO RETURN TO MENU", new Vector2(window.Width / 2, 530 + FontLibrary.Normal.LineSpacing), Color.White, 0, FontLibrary.Normal.MeasureString("OR ESC TO RETURN TO MENU") / 2, 1, SpriteEffects.None, 0);
            }
        }
    }
}

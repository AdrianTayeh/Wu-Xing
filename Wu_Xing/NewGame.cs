using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Wu_Xing
{
    class NewGame
    {
        private float energyCircleRotation;
        private Vector2 energyCirclePosition;
        private RenderTarget2D energyCircle;

        private Element? elementToChannel;
        private Element? gemToFind;

        private Rectangle[] energyLineSource = new Rectangle[5];
        private Vector2[] energyLinePosition = new Vector2[5];
        private float[] energyLineRotation = new float[5];
        private bool[] energyLineVisible = new bool[5];

        bool woodGemObtained;
        bool fireGemObtained;
        bool earthGemObtained;
        bool metalGemObtained;
        bool waterGemObtained;

        private Dictionary<Element, Button> gemButtons = new Dictionary<Element, Button>();
        private Thread mapGeneratorThread;
        private float versusScreenTimer;

        private enum Stage { PickElement, PickGem, Versus }
        private Stage stage;

        private Button helpButton;
        private string helpText;
        private bool drawHelpText;

        public NewGame(Rectangle window, GraphicsDevice GraphicsDevice)
        {
            energyCirclePosition = new Vector2(window.Width / 2, window.Height / 2);
            energyCircle = new RenderTarget2D(GraphicsDevice, TextureLibrary.EnergyCircle.Width, TextureLibrary.EnergyCircle.Height);

            //Set sources for the energy lines
            for (int i = 0; i < energyLineSource.Length; i++)
            {
                energyLineSource[i].X = i * 300;
                energyLineSource[i].Width = 395;
                energyLineSource[i].Height = TextureLibrary.EnergyLine.Height;
                energyLineRotation[i] = (float)Math.PI * 2 / 5 * i;
                energyLinePosition[i] = Rotate.PointAroundZero(new Vector2(0, -62), energyLineRotation[i]);
            }

            gemButtons.Add(Element.Wood, new Button(
                (energyCirclePosition + energyLinePosition[0] * 3.5f).ToPoint(),
                new Point(150, 150),
                "", null,
                null, null,
                ColorLibrary.SolidWhiteButtonBackgroundColor,
                null
                ));

            gemButtons.Add(Element.Fire, new Button(
                (energyCirclePosition + energyLinePosition[1] * 3.5f).ToPoint(),
                new Point(150, 150),
                "", null,
                null, null,
                ColorLibrary.SolidWhiteButtonBackgroundColor,
                null
                ));

            gemButtons.Add(Element.Earth, new Button(
                (energyCirclePosition + energyLinePosition[2] * 3.5f).ToPoint(),
                new Point(150, 150),
                "", null,
                null, null,
                ColorLibrary.SolidWhiteButtonBackgroundColor,
                null
                ));

            gemButtons.Add(Element.Metal, new Button(
                (energyCirclePosition + energyLinePosition[3] * 3.5f).ToPoint(),
                new Point(150, 150),
                "", null,
                null, null,
                ColorLibrary.SolidWhiteButtonBackgroundColor,
                null
                ));

            gemButtons.Add(Element.Water, new Button(
                (energyCirclePosition + energyLinePosition[4] * 3.5f).ToPoint(),
                new Point(150, 150),
                "", null,
                null, null,
                ColorLibrary.SolidWhiteButtonBackgroundColor,
                null
                ));

            helpButton = new Button(
                new Point(60, 60),
                new Point(60, 60),
                "?", FontLibrary.Normal,
                null, null,
                null,
                ColorLibrary.BlackButtonLabelColor
                );

            string textFile = Environment.CurrentDirectory;

            //For Windows
            textFile = textFile.Replace(@"bin\DesktopGL\AnyCPU\Debug", @"Text\Effectiveness.txt");
            //For MacOS
            textFile = textFile.Replace(@"bin/DesktopGL/AnyCPU/Debug", @"Text/Effectiveness.txt");

            helpText = File.ReadAllText(textFile);
        }

        public void Refresh()
        {
            elementToChannel = null;
            gemToFind = null;
            stage = Stage.PickElement;
            drawHelpText = false;
            helpButton.Active = true;

            //Gems obtained in this save file
            woodGemObtained = true;
            fireGemObtained = true;
            earthGemObtained = true;
            metalGemObtained = true;
            waterGemObtained = true;

            //Set visibility of destructive energy lines
            energyLineVisible[0] = waterGemObtained && fireGemObtained ? true : false;
            energyLineVisible[1] = woodGemObtained && earthGemObtained ? true : false;
            energyLineVisible[2] = fireGemObtained && metalGemObtained ? true : false;
            energyLineVisible[3] = earthGemObtained && waterGemObtained ? true : false;
            energyLineVisible[4] = metalGemObtained && woodGemObtained ? true : false;

            //Set texture of gem button
            gemButtons[Element.Wood].Background = woodGemObtained ? TextureLibrary.Gems[Element.Wood] : TextureLibrary.GemsHidden[Element.Wood];
            gemButtons[Element.Fire].Background = fireGemObtained ? TextureLibrary.Gems[Element.Fire] : TextureLibrary.GemsHidden[Element.Fire];
            gemButtons[Element.Earth].Background = earthGemObtained ? TextureLibrary.Gems[Element.Earth] : TextureLibrary.GemsHidden[Element.Earth];
            gemButtons[Element.Metal].Background = metalGemObtained ? TextureLibrary.Gems[Element.Metal] : TextureLibrary.GemsHidden[Element.Metal];
            gemButtons[Element.Water].Background = waterGemObtained ? TextureLibrary.Gems[Element.Water] : TextureLibrary.GemsHidden[Element.Water];

            //Set activity of gem button
            gemButtons[Element.Wood].Active = woodGemObtained;
            gemButtons[Element.Fire].Active = fireGemObtained;
            gemButtons[Element.Earth].Active = earthGemObtained;
            gemButtons[Element.Metal].Active = metalGemObtained;
            gemButtons[Element.Water].Active = waterGemObtained;
        }

        public void Update(ref Screen screen, Mouse mouse, KeyboardState currentKeyboard, KeyboardState previousKeyboard, Random random, Running running, float elapsedSeconds, GraphicsDevice GraphicsDevice)
        {
            switch (stage)
            {
                case Stage.PickElement:
                    UpdatePickElement(currentKeyboard, previousKeyboard, mouse, ref screen);
                    UpdateEnergy();
                    break;

                case Stage.PickGem:
                    UpdatePickGem(currentKeyboard, previousKeyboard, mouse, random, running, GraphicsDevice);
                    UpdateEnergy();
                    break;

                case Stage.Versus:
                    UpdateVersus(ref screen, elapsedSeconds);
                    break;
            }
        }

        private void UpdateEnergy()
        {
            energyCircleRotation = (energyCircleRotation + 0.01f) % ((float)Math.PI * 2);

            for (int i = 0; i < energyLineSource.Length; i++)
                energyLineSource[i].X = (energyLineSource[i].X + 2) % (TextureLibrary.EnergyLine.Width - energyLineSource[i].Width);  
        }

        private void UpdatePickElement(KeyboardState currentKeyboard, KeyboardState previousKeyboard, Mouse mouse, ref Screen screen)
        {
            if (currentKeyboard.IsKeyUp(Keys.Escape) && previousKeyboard.IsKeyDown(Keys.Escape))
            {
                screen = Screen.Pregame;
                return;
            }

            Updatebuttons(mouse, ref elementToChannel);

            //Check if element selected
            foreach (KeyValuePair<Element, Button> button in gemButtons)
            {
                if (button.Value.IsReleased && button.Value.Active && elementToChannel != null)
                {
                    //Next stage
                    stage = Stage.PickGem;
                    SoundLibrary.Upgrade.Play();

                    //Make all gem buttons active
                    foreach (KeyValuePair<Element, Button> button2 in gemButtons)
                        button2.Value.Active = true;

                    break;
                }
            }
        }

        private void UpdatePickGem(KeyboardState currentKeyboard, KeyboardState previousKeyboard, Mouse mouse, Random random, Running running, GraphicsDevice GraphicsDevice)
        {
            if (currentKeyboard.IsKeyUp(Keys.Escape) && previousKeyboard.IsKeyDown(Keys.Escape))
            {
                stage = Stage.PickElement;
                gemToFind = null;
                return;
            }

            Updatebuttons(mouse, ref gemToFind);

            //Check if element selected
            foreach (KeyValuePair<Element, Button> button in gemButtons)
            {
                if (button.Value.IsReleased && gemToFind != null)
                {
                    //Next stage
                    stage = Stage.Versus;
                    SoundLibrary.Upgrade.Play();
                    helpButton.Active = false;

                    //Generate map on separate thread
                    mapGeneratorThread = new Thread( () => running.InitializeNewMap(GraphicsDevice, random, 15, button.Key, (Element)elementToChannel));
                    mapGeneratorThread.Start();
                    break;
                }
            }
        }

        private void Updatebuttons(Mouse mouse, ref Element? preview)
        {
            helpButton.Update(mouse);

            if (helpButton.IsReleased)
                drawHelpText = !drawHelpText;

            foreach (KeyValuePair<Element, Button> button in gemButtons)
                button.Value.Update(mouse);

            //Preview element
            foreach (KeyValuePair<Element, Button> button in gemButtons)
            {
                if (button.Value.IsHoveredOn && button.Value.Active)
                {
                    preview = button.Key;
                    break;
                }
            }
        }

        private void UpdateVersus(ref Screen screen, float elapsedSeconds)
        {
            versusScreenTimer += elapsedSeconds;
            if (versusScreenTimer >= 4 && !mapGeneratorThread.IsAlive)
            {
                versusScreenTimer = 0;
                screen = Screen.Running;
                Debug.WriteLine("Map generation thread is dead, and at least four seconds elapsed.");
            }
        }

        public void Draw(SpriteBatch spriteBatch, Rectangle window)
        {
            if (stage != Stage.Versus)
            {
                spriteBatch.Draw(TextureLibrary.BackgroundGray, window, Color.White);
                spriteBatch.Draw(energyCircle, energyCirclePosition, null, Color.White, 0, TextureLibrary.EnergyCircle.Bounds.Size.ToVector2() / 2, 1, SpriteEffects.None, 0);

                for (int i = 0; i < energyLineSource.Length; i++)
                    if (energyLineVisible[i])
                        spriteBatch.Draw(TextureLibrary.EnergyLine, energyCirclePosition + energyLinePosition[i], energyLineSource[i], Color.White, energyLineRotation[i], energyLineSource[i].Size.ToVector2() / 2, 1, SpriteEffects.FlipHorizontally, 0);

                helpButton.Draw(spriteBatch);

                foreach (KeyValuePair<Element, Button> button in gemButtons)
                    button.Value.Draw(spriteBatch);

                string select = stage == Stage.PickElement ? "SELECT AN ELEMENT TO CHANNEL" : "SELECT A GEM TO FIND";
                spriteBatch.DrawString(FontLibrary.Normal, select, new Vector2(window.Width / 2, 200), Color.White, 0, FontLibrary.Normal.MeasureString(select) / 2, 1, SpriteEffects.None, 0);

                if (elementToChannel != null)
                {
                    spriteBatch.Draw(TextureLibrary.Symbols[(Element)elementToChannel], new Vector2(window.Width * 0.2f, window.Height / 2), null, Color.White, 0, TextureLibrary.Symbols[(Element)elementToChannel].Bounds.Size.ToVector2() / 2, 0.4f, SpriteEffects.None, 0);
                    spriteBatch.DrawString(FontLibrary.Normal, elementToChannel.ToString().ToUpper(), new Vector2(window.Width * 0.2f, window.Height / 2 + 150), Color.White, 0, FontLibrary.Normal.MeasureString(elementToChannel.ToString().ToUpper()) / 2, 1, SpriteEffects.None, 0); ;
                }

                if (gemToFind != null)
                {
                    spriteBatch.Draw(TextureLibrary.Symbols[(Element)gemToFind], new Vector2(window.Width * 0.8f, window.Height / 2), null, Color.White, 0, TextureLibrary.Symbols[(Element)gemToFind].Bounds.Size.ToVector2() / 2, 0.4f, SpriteEffects.None, 0);
                    spriteBatch.DrawString(FontLibrary.Normal, gemToFind.ToString().ToUpper(), new Vector2(window.Width * 0.8f, window.Height / 2 + 150), Color.White, 0, FontLibrary.Normal.MeasureString(gemToFind.ToString().ToUpper()) / 2, 1, SpriteEffects.None, 0);

                    Vector2 position = new Vector2(window.Width / 2, window.Height - 200) - FontLibrary.Normal.MeasureString(elementToChannel.ToString().ToUpper() + " IS " + Effectiveness.GetString(elementToChannel, gemToFind).ToUpper() + " AGAINST " + gemToFind.ToString().ToUpper()) / 2;
                    Effectiveness.DrawFullString(spriteBatch, position, elementToChannel, gemToFind, 1);
                }

                if (drawHelpText)
                {
                    spriteBatch.Draw(TextureLibrary.WhitePixel, new Rectangle(0, 100, 1030, 730), Color.FromNonPremultiplied(30, 30, 30, 220));
                    spriteBatch.DrawString(FontLibrary.Normal, helpText, new Vector2(50, 150), Color.White, 0, Vector2.Zero, 0.8f, SpriteEffects.None, 0);
                }
            }

            else
            {
                spriteBatch.Draw(TextureLibrary.BackgroundSplit, window, Color.White);
                spriteBatch.Draw(TextureLibrary.Symbols[(Element)elementToChannel], new Vector2(window.Width * 0.3f, window.Height / 2), null, Color.White, 0, TextureLibrary.Symbols[(Element)elementToChannel].Bounds.Size.ToVector2() / 2, 0.6f, SpriteEffects.None, 0);
                spriteBatch.Draw(TextureLibrary.Symbols[(Element)gemToFind], new Vector2(window.Width * 0.7f, window.Height / 2), null, Color.White, 0, TextureLibrary.Symbols[(Element)gemToFind].Bounds.Size.ToVector2() / 2, 0.6f, SpriteEffects.None, 0);
            }
        }

        public void DrawCircleToTexture(SpriteBatch spriteBatch, GraphicsDevice GraphicsDevice)
        {
            GraphicsDevice.SetRenderTarget(energyCircle);
            GraphicsDevice.Clear(Color.Transparent);
            spriteBatch.Begin();

            //Draw rotated energy circle
            spriteBatch.Draw(TextureLibrary.EnergyCircle, energyCircle.Bounds.Size.ToVector2() / 2, null, Color.White, energyCircleRotation, energyCircle.Bounds.Size.ToVector2() / 2, 1, SpriteEffects.None, 0);

            //Cover with a black mask
            if (!woodGemObtained)
                spriteBatch.Draw(TextureLibrary.EnergyMask, energyCircle.Bounds.Size.ToVector2() / 2, null, Color.White, 0, TextureLibrary.EnergyMask.Bounds.Size.ToVector2() / 2, 1, SpriteEffects.None, 0);

            if (!fireGemObtained)
                spriteBatch.Draw(TextureLibrary.EnergyMask, energyCircle.Bounds.Size.ToVector2() / 2, null, Color.White, (float)Math.PI * 2 / 5, TextureLibrary.EnergyMask.Bounds.Size.ToVector2() / 2, 1, SpriteEffects.None, 0);

            if (!earthGemObtained)
                spriteBatch.Draw(TextureLibrary.EnergyMask, energyCircle.Bounds.Size.ToVector2() / 2, null, Color.White, (float)Math.PI * 2 / 5 * 2, TextureLibrary.EnergyMask.Bounds.Size.ToVector2() / 2, 1, SpriteEffects.None, 0);

            if (!metalGemObtained)
                spriteBatch.Draw(TextureLibrary.EnergyMask, energyCircle.Bounds.Size.ToVector2() / 2, null, Color.White, (float)Math.PI * 2 / 5 * 3, TextureLibrary.EnergyMask.Bounds.Size.ToVector2() / 2, 1, SpriteEffects.None, 0);

            if (!waterGemObtained)
                spriteBatch.Draw(TextureLibrary.EnergyMask, energyCircle.Bounds.Size.ToVector2() / 2, null, Color.White, (float)Math.PI * 2 / 5 * 4, TextureLibrary.EnergyMask.Bounds.Size.ToVector2() / 2, 1, SpriteEffects.None, 0);
            
            spriteBatch.End();
            
            //Replace black with transparent
            Color[] color = new Color[energyCircle.Width * energyCircle.Height];
            energyCircle.GetData(color);
            
            for (int i = 0; i < color.Length; i++)
                if (color[i] == Color.Black)
                    color[i] = Color.Transparent;
            
            energyCircle.SetData(color);
        }
    }
}

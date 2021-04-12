using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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

        private Texture2D elementToChannel;
        private Texture2D gemToFind;

        private Rectangle[] energyLineSource = new Rectangle[5];
        private Vector2[] energyLinePosition = new Vector2[5];
        private float[] energyLineRotation = new float[5];
        private bool[] energyLineVisible = new bool[5];

        bool woodGemObtained;
        bool fireGemObtained;
        bool earthGemObtained;
        bool metalGemObtained;
        bool waterGemObtained;

        private Dictionary<string, Button> gemButton = new Dictionary<string, Button>();
        private Task generateMap;
        private float versusScreenTimer;

        private enum Stage { PickElement, PickGem, Versus }
        private Stage stage;

        public NewGame(Rectangle window, GraphicsDevice GraphicsDevice)
        {
            stage = Stage.PickElement;
            energyCirclePosition = new Vector2(window.Width / 2, window.Height / 2);
            energyCircle = new RenderTarget2D(GraphicsDevice, TextureLibrary.EnergyCircle.Width, TextureLibrary.EnergyCircle.Height);

            for (int i = 0; i < energyLineSource.Length; i++)
            {
                energyLineSource[i].X = i * 300;
                energyLineSource[i].Width = 395;
                energyLineSource[i].Height = TextureLibrary.EnergyLine.Height;
                energyLineRotation[i] = (float)Math.PI * 2 / 5 * i;
                energyLinePosition[i] = Rotate.PointAroundZero(new Vector2(0, -62), energyLineRotation[i]);
            }

            gemButton.Add("Wood", new Button(
                (energyCirclePosition + energyLinePosition[0] * 3.5f).ToPoint(),
                new Point(150, 150),
                "", null,
                TextureLibrary.GemWood, null,
                ColorLibrary.SolidWhiteButtonBackgroundColor,
                null
                ));

            gemButton.Add("Fire", new Button(
                (energyCirclePosition + energyLinePosition[1] * 3.5f).ToPoint(),
                new Point(150, 150),
                "", null,
                TextureLibrary.GemFire, null,
                ColorLibrary.SolidWhiteButtonBackgroundColor,
                null
                ));

            gemButton.Add("Earth", new Button(
                (energyCirclePosition + energyLinePosition[2] * 3.5f).ToPoint(),
                new Point(150, 150),
                "", null,
                TextureLibrary.GemEarth, null,
                ColorLibrary.SolidWhiteButtonBackgroundColor,
                null
                ));

            gemButton.Add("Metal", new Button(
                (energyCirclePosition + energyLinePosition[3] * 3.5f).ToPoint(),
                new Point(150, 150),
                "", null,
                TextureLibrary.GemMetal, null,
                ColorLibrary.SolidWhiteButtonBackgroundColor,
                null
                ));

            gemButton.Add("Water", new Button(
                (energyCirclePosition + energyLinePosition[4] * 3.5f).ToPoint(),
                new Point(150, 150),
                "", null,
                TextureLibrary.GemWater, null,
                ColorLibrary.SolidWhiteButtonBackgroundColor,
                null
                ));
        }

        public void Refresh()
        {
            elementToChannel = null;
            gemToFind = null;
            stage = Stage.PickElement;

            //Gems obtained in this save file
            woodGemObtained = true;
            fireGemObtained = true;
            earthGemObtained = true;
            metalGemObtained = false;
            waterGemObtained = true;

            //Set visibility of destructive energy lines
            energyLineVisible[0] = waterGemObtained && fireGemObtained ? true : false;
            energyLineVisible[1] = woodGemObtained && earthGemObtained ? true : false;
            energyLineVisible[2] = fireGemObtained && metalGemObtained ? true : false;
            energyLineVisible[3] = earthGemObtained && waterGemObtained ? true : false;
            energyLineVisible[4] = metalGemObtained && woodGemObtained ? true : false;

            //Set texture of gem button
            gemButton["Wood"].Background = woodGemObtained ? TextureLibrary.GemWood : TextureLibrary.GemWoodHidden;
            gemButton["Fire"].Background = fireGemObtained ? TextureLibrary.GemFire : TextureLibrary.GemFireHidden;
            gemButton["Earth"].Background = earthGemObtained ? TextureLibrary.GemEarth : TextureLibrary.GemEarthHidden;
            gemButton["Metal"].Background = metalGemObtained ? TextureLibrary.GemMetal : TextureLibrary.GemMetalHidden;
            gemButton["Water"].Background = waterGemObtained ? TextureLibrary.GemWater : TextureLibrary.GemWaterHidden;

            //Set activity of gem button
            gemButton["Wood"].Active = woodGemObtained;
            gemButton["Fire"].Active = fireGemObtained;
            gemButton["Earth"].Active = earthGemObtained;
            gemButton["Metal"].Active = metalGemObtained;
            gemButton["Water"].Active = waterGemObtained;
        }

        public void Update(ref Screen screen, Mouse mouse, KeyboardState currentKeyboard, KeyboardState previousKeyboard, Random random, Running running, GameTime gameTime)
        {
            switch (stage)
            {
                case Stage.PickElement:
                    UpdatePickElement(currentKeyboard, previousKeyboard, mouse, ref screen);
                    UpdateEnergy();
                    break;

                case Stage.PickGem:
                    UpdatePickGem(currentKeyboard, previousKeyboard, mouse, random, running);
                    UpdateEnergy();
                    break;

                case Stage.Versus:
                    UpdateVersus(ref screen, gameTime);
                    break;
            }

        }

        private void UpdateEnergy()
        {
            energyCircleRotation += 0.01f;
            energyCircleRotation %= (float)Math.PI * 2;

            for (int i = 0; i < energyLineSource.Length; i++)
            {
                energyLineSource[i].X += 2;
                energyLineSource[i].X %= TextureLibrary.EnergyLine.Width - energyLineSource[i].Width;
            }
        }

        private void UpdatePickElement(KeyboardState currentKeyboard, KeyboardState previousKeyboard, Mouse mouse, ref Screen screen)
        {
            if (currentKeyboard.IsKeyUp(Keys.Escape) && previousKeyboard.IsKeyDown(Keys.Escape))
                screen = Screen.Pregame;

            foreach (KeyValuePair<string, Button> item in gemButton)
                item.Value.Update(mouse);

            //Preview element
            if (gemButton["Wood"].IsHoveredOn)
                elementToChannel = TextureLibrary.SymbolWood;

            else if (gemButton["Fire"].IsHoveredOn)
                elementToChannel = TextureLibrary.SymbolFire;

            else if (gemButton["Earth"].IsHoveredOn)
                elementToChannel = TextureLibrary.SymbolEarth;

            else if (gemButton["Metal"].IsHoveredOn)
                elementToChannel = TextureLibrary.SymbolMetal;

            else if (gemButton["Water"].IsHoveredOn)
                elementToChannel = TextureLibrary.SymbolWater;

            //Element selected
            foreach (KeyValuePair<string, Button> item in gemButton)
            {
                if (item.Value.IsReleased && elementToChannel != null)
                {
                    stage = Stage.PickGem;

                    foreach (KeyValuePair<string, Button> item2 in gemButton)
                        item2.Value.Active = true;

                    break;
                }
            }
        }

        private void UpdatePickGem(KeyboardState currentKeyboard, KeyboardState previousKeyboard, Mouse mouse, Random random, Running running)
        {
            if (currentKeyboard.IsKeyUp(Keys.Escape) && previousKeyboard.IsKeyDown(Keys.Escape))
            {
                stage = Stage.PickElement;
                gemToFind = null;
            }
                
            foreach (KeyValuePair<string, Button> item in gemButton)
                item.Value.Update(mouse);

            //Preview element
            if (gemButton["Wood"].IsHoveredOn)
                gemToFind = TextureLibrary.SymbolWood;

            else if (gemButton["Fire"].IsHoveredOn)
                gemToFind = TextureLibrary.SymbolFire;

            else if (gemButton["Earth"].IsHoveredOn)
                gemToFind = TextureLibrary.SymbolEarth;

            else if (gemButton["Metal"].IsHoveredOn)
                gemToFind = TextureLibrary.SymbolMetal;

            else if (gemButton["Water"].IsHoveredOn)
                gemToFind = TextureLibrary.SymbolWater;

            //Element selected
            foreach (KeyValuePair<string, Button> item in gemButton)
            {
                if (item.Value.IsReleased && gemToFind != null)
                {
                    stage = Stage.Versus;
                    generateMap = Task.Run(() => running.InitializeNewMap(random, 13, (Element)Enum.Parse(typeof(Element), item.Key)));
                    break;
                }
            }
        }

        private void UpdateVersus(ref Screen screen, GameTime gameTime)
        {
            versusScreenTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (versusScreenTimer >= 4 && generateMap.IsCompleted)
            {
                versusScreenTimer = 0;
                screen = Screen.Running;
                Debug.WriteLine("Task " + generateMap.Status + ", and at least four seconds elapsed.");
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

                foreach (KeyValuePair<string, Button> item in gemButton)
                    item.Value.Draw(spriteBatch);

                string select = stage == Stage.PickElement ? "SELECT AN ELEMENT TO CHANNEL" : "SELECT A GEM TO FIND";
                spriteBatch.DrawString(FontLibrary.Normal, select, new Vector2(window.Width / 2, 200), Color.White, 0, FontLibrary.Normal.MeasureString(select) / 2, 1, SpriteEffects.None, 0);

                if (elementToChannel != null)
                {
                    spriteBatch.Draw(elementToChannel, new Vector2(window.Width * 0.2f, window.Height / 2), null, Color.White, 0, elementToChannel.Bounds.Size.ToVector2() / 2, 0.4f, SpriteEffects.None, 0);
                    string element = elementToChannel.Name.Replace("Elements\\Symbol ", "").ToUpper();
                    spriteBatch.DrawString(FontLibrary.Normal, element, new Vector2(window.Width * 0.2f, window.Height / 2 + 150), Color.White, 0, FontLibrary.Normal.MeasureString(element) / 2, 1, SpriteEffects.None, 0);
                }

                if (gemToFind != null)
                {
                    spriteBatch.Draw(gemToFind, new Vector2(window.Width * 0.8f, window.Height / 2), null, Color.White, 0, gemToFind.Bounds.Size.ToVector2() / 2, 0.4f, SpriteEffects.None, 0);
                    string element = gemToFind.Name.Replace("Elements\\Symbol ", "").ToUpper();
                    spriteBatch.DrawString(FontLibrary.Normal, element, new Vector2(window.Width * 0.8f, window.Height / 2 + 150), Color.White, 0, FontLibrary.Normal.MeasureString(element) / 2, 1, SpriteEffects.None, 0);
                }
            }

            else
            {
                spriteBatch.Draw(TextureLibrary.BackgroundSplit, window, Color.White);
                spriteBatch.Draw(elementToChannel, new Vector2(window.Width * 0.3f, window.Height / 2), null, Color.White, 0, elementToChannel.Bounds.Size.ToVector2() / 2, 0.6f, SpriteEffects.None, 0);
                spriteBatch.Draw(gemToFind, new Vector2(window.Width * 0.7f, window.Height / 2), null, Color.White, 0, gemToFind.Bounds.Size.ToVector2() / 2, 0.6f, SpriteEffects.None, 0);
            }
        }

        public void DrawCircleToTexture(SpriteBatch spriteBatch, GraphicsDevice GraphicsDevice)
        {
            GraphicsDevice.SetRenderTarget(energyCircle);
            GraphicsDevice.Clear(Color.Transparent);
            spriteBatch.Begin();

            //Draw a fresh energy circle
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
                if (color[i].R < 200)
                    color[i] = Color.Transparent;

            energyCircle.SetData(color);
        }
    }
}

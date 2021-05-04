using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Wu_Xing
{
    class Conveyor : Tile
    {
        private float speed; //Tiles per second
        private float exactYValue;
        private bool powered;
        private static List<Character> charactersToBeIgnored = new List<Character>();

        public Conveyor(Vector2 position, Element? element, Random random, float direction, float speed) : base(position, element, random)
        {
            //GameObject
            texture = TextureLibrary.Conveyor;
            hitbox.Size = new Point(100);
            MoveTo(position);
            rotation = (float)Math.PI / 2 * direction;

            //Conveyor
            this.speed = speed;
            powered = true;
        }

        public override void Update(float elapsedSeconds, List<GameObject> gameObjects, Adam adam, KeyboardState currentKeyboard, MapManager mapManager, Random random)
        {
            if (powered)
            {
                Animate(elapsedSeconds);
                PushCharacters(elapsedSeconds, gameObjects, adam);
            }

            base.Update(elapsedSeconds, gameObjects, adam, currentKeyboard, mapManager, random);
        }

        private void PushCharacters(float elapsedSeconds, List<GameObject> gameObjects, Adam adam)
        {
            foreach (GameObject character in gameObjects)
                if (character is Character && hitbox.Contains(character.Position))
                    PushCharacter((Character)character);

            if (hitbox.Contains(adam.Position))
                PushCharacter(adam);

            void PushCharacter(Character character)
            {
                if (charactersToBeIgnored.Contains(character))
                {
                    charactersToBeIgnored.Remove(character);
                }

                else
                {
                    character.MoveTo(character.Position + speed * 100 * elapsedSeconds * Rotate.PointAroundZero(new Vector2(0, -1), rotation));

                    //If this character was moved out of this conveyor's hitbox
                    //into another conveyor's hitbox, which has not yet been updated this frame,
                    //make that other conveyor ignore this character
                    if (!hitbox.Contains(character.Position))
                    {
                        foreach (GameObject otherConveyor in gameObjects)
                        {
                            if (otherConveyor is Conveyor && otherConveyor != this && (otherConveyor.Position.X > position.X || otherConveyor.Position.Y > position.Y) && otherConveyor.Hitbox.Contains(character.Position))
                            {
                                charactersToBeIgnored.Add(character);
                                break;
                            }
                        }
                    }
                }
            }
        }

        private void Animate(float elapsedSeconds)
        {
            animationTimer += elapsedSeconds;
            if (animationTimer >= 1 / 60)
            {
                animationTimer -= 1 / 60;
                exactYValue += source.Width / 60f * speed;

                if (exactYValue >= texture.Height - source.Height)
                    exactYValue -= texture.Height - source.Height;

                source.Y = (int)exactYValue;
            }
        }
    }
}

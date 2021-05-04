﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Wu_Xing
{
    class Spikes : Tile
    {
        private enum State { Primed, Activated, Recharging }
        private State state;

        private float interval;

        public Spikes(Vector2 position, Element? element, Random random, float interval) : base(position, element, random)
        {
            //GameObject
            texture = TextureLibrary.Spikes;
            hitbox.Size = new Point(50);
            MoveTo(position);

            //Spikes
            this.interval = interval;
        }

        public override void Update(float elapsedSeconds, List<GameObject> gameObjects, Adam adam, KeyboardState currentKeyboard, MapManager mapManager, Random random)
        {
            if (interval != 0)
                IntervalSequence(elapsedSeconds, adam, gameObjects);

            else
                PlayerActivatedSequence(elapsedSeconds, adam, gameObjects);

            base.Update(elapsedSeconds, gameObjects, adam, currentKeyboard, mapManager, random);
        }

        private void IntervalSequence(float elapsedSeconds, Adam adam, List<GameObject> gameObjects)
        {
            if (state == State.Activated)
            {
                DamageCharacters(adam, gameObjects);

                animationTimer += elapsedSeconds;
                if (animationTimer >= interval)
                {
                    animationTimer -= interval;
                    state = State.Recharging;
                    source.X = 0;
                }
            }

            else // (state == State.Recharging)
            {
                animationTimer += elapsedSeconds;
                if (animationTimer >= interval)
                {
                    animationTimer -= interval;
                    state = State.Activated;
                    source.X = 100;
                }
            }
        }

        private void PlayerActivatedSequence(float elapsedSeconds, Adam adam, List<GameObject> gameObjects)
        {
            if (state == State.Primed)
            {
                if (DamageCharacters(adam, gameObjects))
                {
                    state = State.Activated;
                    source.X = 100;
                }
            }

            else if (state == State.Activated)
            {
                animationTimer += elapsedSeconds;
                if (animationTimer >= 2)
                {
                    animationTimer = 0;
                    state = State.Recharging;
                    source.X = 0;
                }
            }

            else // (state == State.Recharging)
            {
                animationTimer += elapsedSeconds;
                if (animationTimer >= 2)
                {
                    animationTimer = 0;
                    state = State.Primed;
                }
            }
        }

        private bool DamageCharacters(Adam adam, List<GameObject> gameObjects)
        {
            bool damageDealt = false;

            foreach (GameObject gameObject in gameObjects)
            {
                if (gameObject is Character && hitbox.Intersects(gameObject.Hitbox))
                {
                    ((Character)gameObject).TakeDamage(1);
                    damageDealt = true;
                }
            }

            if (hitbox.Intersects(adam.Hitbox))
            {
                adam.TakeDamage(1);
                damageDealt = true;
            }

            return damageDealt;
        }

    }
}

using System;
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
            hitbox = new Hitbox(Hitbox.HitboxType.Flat, false, position, new Point(50));

            //Spikes
            this.interval = interval;
        }

        public override void Update(float elapsedSeconds, List<GameObject> gameObjects, Adam adam, KeyboardState currentKeyboard, MapManager mapManager, Random random)
        {
            if (interval != 0)
                IntervalSequence(elapsedSeconds, adam, gameObjects, random);

            else
                PlayerActivatedSequence(elapsedSeconds, adam, gameObjects, random);

            base.Update(elapsedSeconds, gameObjects, adam, currentKeyboard, mapManager, random);
        }

        private void IntervalSequence(float elapsedSeconds, Adam adam, List<GameObject> gameObjects, Random random)
        {
            if (state == State.Activated)
            {
                DamageCharacters(adam, gameObjects, random);

                animationTimer += elapsedSeconds;
                if (animationTimer >= interval)
                {
                    animationTimer -= interval;
                    state = State.Recharging;
                    source.X = 0;
                }
            }

            else
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

        private void PlayerActivatedSequence(float elapsedSeconds, Adam adam, List<GameObject> gameObjects, Random random)
        {
            if (state == State.Primed)
            {
                if (DamageCharacters(adam, gameObjects, random))
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

            else
            {
                animationTimer += elapsedSeconds;
                if (animationTimer >= 2)
                {
                    animationTimer = 0;
                    state = State.Primed;
                }
            }
        }

        private bool DamageCharacters(Adam adam, List<GameObject> gameObjects, Random random)
        {
            bool damageDealt = false;

            foreach (GameObject gameObject in gameObjects)
            {
                if (gameObject is Character && hitbox.Intersects(gameObject.Hitbox))
                {
                    ((Character)gameObject).TakeDamage(1, random);
                    damageDealt = true;
                }
            }

            if (hitbox.Intersects(adam.Hitbox))
            {
                adam.TakeDamage(1, random);
                damageDealt = true;
            }

            return damageDealt;
        }

    }
}

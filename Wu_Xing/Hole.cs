using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Wu_Xing
{
    class Hole : Tile
    {
        public Hole(Vector2 position, Element? element, Random random) : base(position, element, random)
        {
            //GameObject
            texture = TextureLibrary.Hole;
            hitbox = new Hitbox(Hitbox.HitboxType.Flat, true, position, new Point(100));
        }

        public void UpdateAppearence(List<GameObject> gameObjects, Random random)
        {
            Vector2[] relativePositions = new Vector2[8];
            relativePositions[0] = new Vector2(-100, -100);
            relativePositions[1] = new Vector2(100, -100);
            relativePositions[2] = new Vector2(100, 100);
            relativePositions[3] = new Vector2(-100, 100);

            relativePositions[4] = new Vector2(0, -100);
            relativePositions[5] = new Vector2(100, 0);
            relativePositions[6] = new Vector2(0, 100);
            relativePositions[7] = new Vector2(-100, 0);

            bool[] diagonalHoles = new bool[4];
            bool[] cardinalHoles = new bool[4];

            foreach (GameObject gameObject in gameObjects)
            {
                if (gameObject is Hole && gameObject.Element == element)
                {
                    for (int i = 0; i < relativePositions.Length; i++)
                    {
                        if (gameObject.Position == position + relativePositions[i])
                        {
                            if (i < 4)
                                diagonalHoles[i] = true;

                            else
                                cardinalHoles[i - 4] = true;

                            break;
                        }
                    }       
                }
            }

            int diagonalConnections = 0;
            foreach (bool diagonalHole in diagonalHoles)
                if (diagonalHole)
                    diagonalConnections += 1;

            int cardinalConnections = 0;
            foreach (bool cardinalhole in cardinalHoles)
                if (cardinalhole)
                    cardinalConnections += 1;

            //1,1
            if (cardinalConnections == 0)
            {
                source.Location = new Point(100, 100);
                RandomRotation(random, 4);
            }

            //0,1
            else if (cardinalConnections == 1)
            {
                source.Location = new Point(0, 100);
                for (int i = 0; i < cardinalHoles.Length; i++)
                    if (cardinalHoles[i])
                        rotation = (float)Math.PI / 2 * i;
            }

            else if (cardinalConnections == 2)
            {
                //2,0
                if ((cardinalHoles[0] && cardinalHoles[2]) || (cardinalHoles[1] && cardinalHoles[3]))
                {
                    source.Location = new Point(200, 0);
                    rotation = cardinalHoles[0] ? 0 : (float)Math.PI / 2;
                }

                else
                {
                    int i = 0;
                    for (; i < cardinalHoles.Length; i++)
                    {
                        if (cardinalHoles[i] && cardinalHoles[(i + 1) % 4])
                        {
                            rotation = (float)Math.PI / 2 * i;
                            break;
                        }
                    }
                        
                    //2,2
                    if (!diagonalHoles[(i + 1) % 4])
                        source.Location = new Point(200, 200);

                    //1,0
                    else
                        source.Location = new Point(100, 0);
                }
            }

            else if (cardinalConnections == 3)
            {
                int i = 0;
                for (; i < cardinalHoles.Length; i++)
                {
                    if (!cardinalHoles[i])
                    {
                        rotation = (float)Math.PI / 2 * i;
                        break;
                    }
                }
                
                //1,3
                if (!diagonalHoles[(i + 2) % 4] && !diagonalHoles[(i + 3) % 4])
                    source.Location = new Point(100, 300);

                //0,2
                else if (!diagonalHoles[(i + 2) % 4])
                    source.Location = new Point(0, 200);

                //1,2
                else if (!diagonalHoles[(i + 3) % 4])
                    source.Location = new Point(100, 200);

                //0,0
                else // (diagonalHoles[(i + 2) % 4] && diagonalHoles[(i + 3) % 4])
                    source.Location = new Point(0, 0);
            }

            else
            {
                //2,3
                if (diagonalConnections == 0)
                {
                    source.Location = new Point(200, 300);
                }

                //3,3
                else if (diagonalConnections == 1)
                {
                    source.Location = new Point(300, 300);
                    for (int i = 0; i < diagonalHoles.Length; i++)
                        if (diagonalHoles[i])
                            rotation = (float)Math.PI / 2 * i;
                }

                //3,0
                else if (diagonalConnections == 3)
                {
                    source.Location = new Point(300, 0);
                    for (int i = 0; i < diagonalHoles.Length; i++)
                        if (!diagonalHoles[i])
                            rotation = (float)Math.PI / 2 * i;
                }

                //2,1
                else if (diagonalConnections == 4)
                {
                    source.Location = new Point(200, 100);
                }

                //3,2
                else if ((diagonalHoles[0] && diagonalHoles[2]) || (diagonalHoles[1] && diagonalHoles[3]))
                {
                    source.Location = new Point(300, 200);
                    rotation = diagonalHoles[0] ? 0 : (float)Math.PI / 2;
                }

                //3,1
                else // (two diagonalHoles next to each other)
                {
                    source.Location = new Point(300, 100);
                    for (int i = 0; i < diagonalHoles.Length; i++)
                        if (diagonalHoles[i] && diagonalHoles[(i + 1) % 4])
                            rotation = (float)Math.PI / 2 * i;
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 roomPosition, bool drawHitbox)
        {
            base.Draw(spriteBatch, roomPosition, drawHitbox);

            if (element != null)
                spriteBatch.Draw(TextureLibrary.WaterHole, roomPosition + position, source, color, rotation, origin, 1, SpriteEffects.None, layerDepth + 0.001f);
        }
    }
}

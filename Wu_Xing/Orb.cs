using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Wu_Xing
{
    class Orb : Enemy
    {
        public Orb(Vector2 position, Element element, Random random) : base(position, element, random)
        {
            //GameObject
            texture = TextureLibrary.Orb;
            color = ColorLibrary.Element[element];
            source.Size = new Point(100, 130);
            hitbox.Size = new Point(60);
            origin = source.Size.ToVector2() / 2 + new Vector2(0, 15);
            animationFPS = 60;
            MoveTo(position);
            RandomSourceLocation(random);

            //Character
            health = maxHealth = 30;
            speed = 1.1f;
            shadowSize = 80;
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 roomPosition, bool drawHitbox)
        {
            base.Draw(spriteBatch, roomPosition, drawHitbox);
            spriteBatch.Draw(TextureLibrary.ElementalEyes, roomPosition + position + new Vector2(0, 35), null, Color.White, 0, origin, 1, SpriteEffects.None, layerDepth + 0.001f);
        }
    }
}

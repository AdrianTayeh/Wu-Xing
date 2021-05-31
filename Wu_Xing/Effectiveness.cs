using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Wu_Xing
{
    static class Effectiveness
    {
        private static Dictionary<Element?, int> indexes = new Dictionary<Element?, int>();

        public static void Load()
        {
            Array elements = Enum.GetValues(typeof(Element));

            for (int i = 0; i < elements.Length; i++)
                indexes.Add((Element)elements.GetValue(i), i);
        }

        public static float GetMultiplier(Element? dealingElement, Element? receivingElement)
        {
            //Neutral if dealing and receiving element is equal,
            //or if one of the elements is neutral
            if (dealingElement == receivingElement || dealingElement == null || receivingElement == null)
                return 1;

            //Highly effective if receiving element is +2, relative to the element of the projectile
            if (indexes[receivingElement] == (indexes[dealingElement] + 2) % indexes.Count)
                return 1.2f;

            //Slighly effective if receiving element is +4, relative to the element of the projectile
            if (indexes[receivingElement] == (indexes[dealingElement] + 4) % indexes.Count)
                return 1.1f;

            //Slightly ineffective if receiving element is +3, relative to the element of the projectile
            if (indexes[receivingElement] == (indexes[dealingElement] + 3) % indexes.Count)
                return 0.9f;

            //Highly ineffective if receiving element is +1, relative to the element of the projectile
            // (indexes[receivingElement] == (indexes[dealingElement] + 1) % indexes.Count)
                return 0.8f;
        }

        public static string GetString(Element? dealingElement, Element? receivingElement)
        {
            float damageMultiplier = GetMultiplier(dealingElement, receivingElement);

            if (damageMultiplier == 1.2f)
                return "Highly effective";

            if (damageMultiplier == 1.1f)
                return "Slightly effective";

            if (damageMultiplier == 1f)
                return "Neutral";

            if (damageMultiplier == 0.9f)
                return "Slightly ineffective";

            // (damageMultiplier == 0.8)
                return "Highly ineffective";
        }

        public static Color GetColor(Element? dealingElement, Element? receivingElement)
        {
            float damageMultiplier = GetMultiplier(dealingElement, receivingElement);

            if (damageMultiplier == 1.2f)
                return Color.Lime;

            if (damageMultiplier == 1.1f)
                return Color.GreenYellow;

            if (damageMultiplier == 1f)
                return Color.Yellow;

            if (damageMultiplier == 0.9f)
                return Color.Orange;

            // (damageMultiplier == 0.8)
            return Color.Red;
        }

        public static void DrawFullString(SpriteBatch spriteBatch, Vector2 position, Element? dealingElement, Element? receivingElement, float opacity)
        {
            string elementIs = dealingElement.ToString().ToUpper() + " IS ";
            string effectiveness = GetString(dealingElement, receivingElement).ToUpper();
            string againstElement = " AGAINST " + receivingElement.ToString().ToUpper();

            spriteBatch.DrawString(FontLibrary.Normal, elementIs, position, ColorLibrary.Opacity(Color.White, opacity));
            spriteBatch.DrawString(FontLibrary.Normal, effectiveness, position + new Vector2(FontLibrary.Normal.MeasureString(elementIs).X, 0), ColorLibrary.Opacity(GetColor(dealingElement, receivingElement), opacity));
            spriteBatch.DrawString(FontLibrary.Normal, againstElement, position + new Vector2(FontLibrary.Normal.MeasureString(elementIs + effectiveness).X, 0), ColorLibrary.Opacity(Color.White, opacity));
        }
    }
}

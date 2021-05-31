using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Wu_Xing
{
    static class ColorLibrary
    {
        public static Color ButtonNormal { get; private set; }
        public static Color ButtonHover { get; private set; }
        public static Color ButtonHeld { get; private set; }

        public static Dictionary<Button.State, Color> WhiteButtonBackgroundColor { get; private set; }
        public static Dictionary<Button.State, Color> LockedWhiteButtonBackgroundColor { get; private set; }
        public static Dictionary<Button.State, Color> WhiteButtonLabelColor { get; private set; }
        public static Dictionary<Button.State, Color> BlackButtonLabelColor { get; private set; }
        public static Dictionary<Button.State, Color> GreenButtonBackgroundColor { get; private set; }
        public static Dictionary<Button.State, Color> RedButtonBackgroundColor { get; private set; }
        public static Dictionary<Button.State, Color> SolidWhiteButtonBackgroundColor { get; private set; }

        public static Dictionary<Element, Color> Element { get; private set; }
        public static Dictionary<Element, Color> Room { get; private set; }

        public static void Load()
        {
            ButtonNormal = Color.White;
            ButtonHover = Color.FromNonPremultiplied(175, 200, 255, 255);
            ButtonHeld = Color.FromNonPremultiplied(130, 160, 220, 255);

            WhiteButtonBackgroundColor = new Dictionary<Button.State, Color> {
                { Button.State.None, ButtonNormal },
                { Button.State.Hover, ButtonHover },
                { Button.State.Held, ButtonHeld } };

            LockedWhiteButtonBackgroundColor = new Dictionary<Button.State, Color> {
                { Button.State.None, Color.FromNonPremultiplied(255, 255, 255, 100) },
                { Button.State.Hover, Color.FromNonPremultiplied(255, 255, 255, 100) },
                { Button.State.Held, Color.FromNonPremultiplied(255, 255, 255, 100) } };

            WhiteButtonLabelColor = new Dictionary<Button.State, Color> {
                { Button.State.None, Color.Black },
                { Button.State.Hover, Color.Black },
                { Button.State.Held, Color.Black } };

            BlackButtonLabelColor = new Dictionary<Button.State, Color> {
                { Button.State.None, Color.White },
                { Button.State.Hover, Color.White },
                { Button.State.Held, Color.White } };

            GreenButtonBackgroundColor = new Dictionary<Button.State, Color> {
                { Button.State.None, Color.Lime },
                { Button.State.Hover, Color.FromNonPremultiplied(0, 200, 0, 255) },
                { Button.State.Held, Color.FromNonPremultiplied(0, 170, 0, 255) } };

            RedButtonBackgroundColor = new Dictionary<Button.State, Color> {
                { Button.State.None, Color.Red },
                { Button.State.Hover, Color.FromNonPremultiplied(220, 0, 0, 255) },
                { Button.State.Held, Color.FromNonPremultiplied(190, 0, 0, 255) } };

            SolidWhiteButtonBackgroundColor = new Dictionary<Button.State, Color> {
                { Button.State.None, Color.White },
                { Button.State.Hover, Color.White },
                { Button.State.Held, Color.White } };

            Element = new Dictionary<Element, Color> {
                { Wu_Xing.Element.Earth, Color.Yellow },
                { Wu_Xing.Element.Fire, Color.Red },
                { Wu_Xing.Element.Metal, Color.White },
                { Wu_Xing.Element.Water, Color.FromNonPremultiplied(0, 200, 255, 255) },
                { Wu_Xing.Element.Wood, Color.Lime } };

            Room = new Dictionary<Element, Color> {
                { Wu_Xing.Element.Earth, Color.FromNonPremultiplied(60, 60, 10, 255) },
                { Wu_Xing.Element.Fire, Color.FromNonPremultiplied(60, 10, 10, 255) },
                { Wu_Xing.Element.Metal, Color.FromNonPremultiplied(50, 50, 50, 255) },
                { Wu_Xing.Element.Water, Color.FromNonPremultiplied(10, 47, 60, 255) },
                { Wu_Xing.Element.Wood, Color.FromNonPremultiplied(10, 60, 10, 255) } };
        }

        public static Color Opacity(Color color, float opacity)
        {
            return Color.FromNonPremultiplied(color.R, color.G, color.B, (int)(color.A * opacity));
        }

    }
}

﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Wu_Xing
{
    static class ColorLibrary
    {
        public static Color WoodGreen { get; private set; }
        public static Color FireRed { get; private set; }
        public static Color EarthYellow { get; private set; }
        public static Color MetalWhite { get; private set; }
        public static Color WaterBlue { get; private set; }

        public static Color ButtonNormal { get; private set; }
        public static Color ButtonHover { get; private set; }
        public static Color ButtonHeld { get; private set; }

        public static Dictionary<Button.State, Color> WhiteButtonBackgroundColor { get; private set; }
        public static Dictionary<Button.State, Color> WhiteButtonLabelColor { get; private set; }
        public static Dictionary<Button.State, Color> GreenButtonBackgroundColor { get; private set; }
        public static Dictionary<Button.State, Color> RedButtonBackgroundColor { get; private set; }

        public static void Load()
        {
            WoodGreen = Color.Lime;
            FireRed = Color.Red;
            EarthYellow = Color.Yellow;
            MetalWhite = Color.White;
            WaterBlue = Color.FromNonPremultiplied(0, 200, 255, 255);

            ButtonNormal = Color.White;
            ButtonHover = Color.FromNonPremultiplied(175, 200, 255, 255);
            ButtonHeld = Color.FromNonPremultiplied(130, 160, 220, 255);

            WhiteButtonBackgroundColor = new Dictionary<Button.State, Color> {
                { Button.State.None, ButtonNormal },
                { Button.State.Hover, ButtonHover },
                { Button.State.Held, ButtonHeld } };

            WhiteButtonLabelColor = new Dictionary<Button.State, Color> {
                { Button.State.None, Color.Black },
                { Button.State.Hover, Color.Black },
                { Button.State.Held, Color.Black } };

            GreenButtonBackgroundColor = new Dictionary<Button.State, Color> {
                { Button.State.None, Color.Lime },
                { Button.State.Hover, Color.FromNonPremultiplied(0, 200, 0, 255) },
                { Button.State.Held, Color.FromNonPremultiplied(0, 170, 0, 255) },
                { Button.State.Released, Color.Lime } };

            RedButtonBackgroundColor = new Dictionary<Button.State, Color> {
                { Button.State.None, Color.Red },
                { Button.State.Hover, Color.FromNonPremultiplied(220, 0, 0, 255) },
                { Button.State.Held, Color.FromNonPremultiplied(190, 0, 0, 255) },
                { Button.State.Released, Color.Red } };
        }
    }
}
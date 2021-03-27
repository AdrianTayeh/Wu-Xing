using System;
using Microsoft.Xna.Framework;

namespace iOSgame
{
    static class Rotate
    {
        public static Vector2 PointAroundZero(Vector2 point, float degrees)
        {
            float radians = degrees * MathF.PI / 180;

            return new Vector2(
                MathF.Cos(radians) * point.X - MathF.Sin(radians) * point.Y,
                MathF.Sin(radians) * point.X + MathF.Cos(radians) * point.Y);
        }

        public static Vector2 PointAroundCenter(Vector2 point, Vector2 center, float degrees)
        {
            float radians = degrees * MathF.PI / 180;

            return new Vector2(
                MathF.Cos(radians) * (point.X - center.X) - MathF.Sin(radians) * (point.Y - center.Y) + center.X,
                MathF.Sin(radians) * (point.X - center.X) + MathF.Cos(radians) * (point.Y - center.Y) + center.Y);
        }
    }
}

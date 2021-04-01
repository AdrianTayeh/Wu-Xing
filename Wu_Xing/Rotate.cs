using System;
using Microsoft.Xna.Framework;

namespace Wu_Xing
{
    static class Rotate
    {
        public static Vector2 PointAroundZero(Vector2 point, float radians)
        {
            return new Vector2(
                (float)Math.Cos(radians) * point.X - (float)Math.Sin(radians) * point.Y,
                (float)Math.Sin(radians) * point.X + (float)Math.Cos(radians) * point.Y);
        }

        public static Vector2 PointAroundCenter(Vector2 point, Vector2 center, float radians)
        {
            return new Vector2(
                (float)Math.Cos(radians) * (point.X - center.X) - (float)Math.Sin(radians) * (point.Y - center.Y) + center.X,
                (float)Math.Sin(radians) * (point.X - center.X) + (float)Math.Cos(radians) * (point.Y - center.Y) + center.Y);
        }
    }
}

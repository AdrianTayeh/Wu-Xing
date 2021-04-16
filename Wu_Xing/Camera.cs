using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Wu_Xing
{
    class Camera
    {
        private Matrix matrix;
        private Viewport viewport;
        private Rectangle bounds;

        public Camera(Rectangle window)
        {
            viewport = new Viewport(window);
            bounds.X = window.Width / 2 - 20;
            bounds.Y = window.Height / 2;
        }

        public Matrix Matrix { get { return matrix; } }

        public void UpdateFocus(Vector2 focusPoint, Point roomSize)
        {
            bounds.Width = (roomSize.X - 1) * 1500;
            bounds.Height = (roomSize.Y - 1) * 700;

            if (focusPoint.X < bounds.Left)
                focusPoint.X = bounds.Left;

            else if (focusPoint.X > bounds.Right)
                focusPoint.X = bounds.Right;

            if (focusPoint.Y < bounds.Top)
                focusPoint.Y = bounds.Top;

            else if (focusPoint.Y > bounds.Bottom)
                focusPoint.Y = bounds.Bottom;

            matrix = Matrix.CreateTranslation(-focusPoint.X + viewport.Width / 2, -focusPoint.Y + viewport.Height / 2, 0);
        }

    }
}

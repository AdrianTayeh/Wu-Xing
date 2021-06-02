using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Level_Editor
{
    class Camera
    {
        private Matrix matrix;
        private Viewport viewport;
        private Rectangle bounds;
        private Vector2 position;
        public Camera(Rectangle window)
        {
            viewport = new Viewport(window);
            bounds.X = window.Width / 2 - 20;
            bounds.Y = window.Height / 2;
        }

        public Matrix Matrix { get { return matrix; } }

        public void UpdateFocus(Vector2 position)
        {
            this.position = position;           

            matrix = Matrix.CreateTranslation(-position.X + viewport.Width / 2, -position.Y + viewport.Height / 2, 0);
        }

        public Vector2 MousePositionInWorld(Mouse mouse)
        {
            return Vector2.Transform(new Vector2(mouse.Position.X, mouse.Position.Y), Matrix.Invert(Matrix));
        }

    }
}

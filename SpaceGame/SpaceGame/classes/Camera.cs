using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace SpaceGame
{
    class Camera
    {
        public Matrix transform;
        Viewport view;
        Vector2 center;

        //zoom
        float zoom = 0.9f;
        static float zoomStatic = 0.9f;

        //rotation
        float rotation = 0f;

        Vector2 origin;

        public Camera(Viewport initViewPort)
        {
            view = initViewPort;

            origin = new Vector2(initViewPort.Width / 2.0f, initViewPort.Height / 2.0f);
        }

        public void Update(Rectangle playerRectangle)
        {
            //center = new Vector2(playerRectangle.X + (playerRectangle.Width / 2) - origin.X, playerRectangle.Y + (playerRectangle.Height / 2) - origin.Y);

            center = new Vector2(playerRectangle.X + origin.X, playerRectangle.Y + origin.Y);

            transform = Matrix.CreateScale(new Vector3(1, 1, 0)) * Matrix.CreateTranslation(new Vector3(-center.X, -center.Y, 0));

            /*
            transform = Matrix.CreateTranslation(new Vector3(-center, 0.0f)) *
                        Matrix.CreateTranslation(new Vector3(-origin, 0.0f)) *
                        Matrix.CreateRotationZ(rotation) *
                        Matrix.CreateScale(zoom, zoom, 0) *
                        Matrix.CreateTranslation(new Vector3(origin, 0.0f));
            */
        }
    }
}

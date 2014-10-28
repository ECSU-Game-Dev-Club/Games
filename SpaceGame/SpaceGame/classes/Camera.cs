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
        float zoom = 1f;
        static float zoomStatic = 1f;

        //rotation
        float rotation = 0f;

        Vector2 origin;

        public Camera(Viewport initViewPort)
        {
            view = initViewPort;

            origin = new Vector2(initViewPort.Width / 2.0f, initViewPort.Height / 2.0f);
        }

        public void zoomCamera(int initZoom)
        {
            zoom = initZoom;
        }

        public void defaultCameraZoom()
        {
            zoom = zoomStatic;
        }

        public void Update(Vector2 playerVector)
        {
            center = new Vector2(playerVector.X - origin.X, playerVector.Y - origin.Y);

            transform = Matrix.CreateTranslation(new Vector3(-center, 0.0f)) *
                        Matrix.CreateTranslation(new Vector3(-origin, 0.0f)) *
                        Matrix.CreateRotationZ(rotation) *
                        Matrix.CreateScale(zoom, zoom, 0) *
                        Matrix.CreateTranslation(new Vector3(origin, 0.0f));
        }
    }
}

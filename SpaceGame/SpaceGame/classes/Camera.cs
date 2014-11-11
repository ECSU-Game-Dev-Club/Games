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
        Vector2 cameraOriginalCenter;

        //zoom
        public static float zoom = 1f;
        const float ZOOMCONST = 1f;

        //rotation
        float rotation = 0f;

        public static Vector2 cameraOrigin;
        public static Vector2 cameraCenter;

        public Camera(Viewport initViewPort)
        {
            view = initViewPort;

            cameraOriginalCenter = new Vector2(initViewPort.Width / 2.0f, initViewPort.Height / 2.0f);
        }

        public void zoomCamera(int initZoom)
        {
            zoom = initZoom;
        }

        public void zoomIncriment() //Zoom in
        {
            if (zoom < 1.0f)
            {
                zoom += 0.01f;
            }
        }

        public void zoomDecriment() //Zoom out
        {
            if (zoom > 0.5f)
            {
                zoom -= 0.01f;
            }
        }

        public void defaultCameraZoom()
        {
            zoom = ZOOMCONST;
        }

        public void Update(Vector2 playerVector)
        {
            cameraOrigin = new Vector2(playerVector.X - cameraOriginalCenter.X, playerVector.Y - cameraOriginalCenter.Y);
            cameraCenter = new Vector2(cameraOrigin.X + (view.Width / 2), cameraOrigin.Y + (view.Height / 2));

            transform = Matrix.CreateTranslation(new Vector3(-cameraOrigin, 0.0f)) *
                        Matrix.CreateTranslation(new Vector3(-cameraOriginalCenter, 0.0f)) *
                        Matrix.CreateRotationZ(rotation) *
                        Matrix.CreateScale(zoom, zoom, 0) *
                        Matrix.CreateTranslation(new Vector3(cameraOriginalCenter, 0.0f));
        }

        public Vector2 getCameraOrigin()
        {
            return cameraOrigin;
        }
    }
}

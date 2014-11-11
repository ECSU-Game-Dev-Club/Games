using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace SpaceGame
{
    class Background
    {
        //Setting up a variable for the players screen size
        Rectangle screenSizeRectangle;

        //Create a new content manager to load content used just by this level.
        ContentManager content;

        //grid layout image
        Texture2D starTexture;
        Rectangle starRectangleBackground;
        const int STARWIDTHANDHEIGHTBACKGROUND = 4;
        const float STARBACKGROUNDVECTORCONST = 0.2f;

        Rectangle starRectangleForeground;
        const int STARWIDTHANDHEIGHTFOREGROUND = 9;

        Random random;

        bool buildStarsBool = true;

        const int SCREENMULTIPLIER = 2;

        //stores the location of the grid layout
        const int MAXBACKGROUNDSTARS = 120;
        const int MAXFOREGROUNDSTARS = 60;
        Vector2[] starArrayForeground = new Vector2[MAXFOREGROUNDSTARS];
        Vector2[] starArrayBackground = new Vector2[MAXBACKGROUNDSTARS];

        public Background(IServiceProvider serviceProvider, Rectangle initScreenSizeRectangle)
        {
            content = new ContentManager(serviceProvider, "Content");

            random = new Random();

            screenSizeRectangle = initScreenSizeRectangle;

            starRectangleBackground = new Rectangle(0, 0, STARWIDTHANDHEIGHTBACKGROUND, STARWIDTHANDHEIGHTBACKGROUND);
            starRectangleForeground = new Rectangle(0, 0, STARWIDTHANDHEIGHTFOREGROUND, STARWIDTHANDHEIGHTFOREGROUND);

            this.LoadContent();
        }

        public void LoadContent()
        {
            starTexture = content.Load<Texture2D>("whiteTexture");
        }

        public void buildStars()
        {
            //BACKGROUND
            for (int i = 0; i < MAXBACKGROUNDSTARS; i++)
            {
                starArrayBackground[i].X = random.Next((int)Camera.cameraOrigin.X - screenSizeRectangle.Width * SCREENMULTIPLIER, (int)((screenSizeRectangle.Width * SCREENMULTIPLIER) + Camera.cameraOrigin.X) + 1);
                starArrayBackground[i].Y = random.Next((int)Camera.cameraOrigin.Y - screenSizeRectangle.Height * SCREENMULTIPLIER, (int)((screenSizeRectangle.Height * SCREENMULTIPLIER) + Camera.cameraOrigin.Y) + 1);
            }

            //FOREGROUND
            for (int i = 0; i < MAXFOREGROUNDSTARS; i++)
            {
                starArrayForeground[i].X = random.Next((int)Camera.cameraOrigin.X - screenSizeRectangle.Width * SCREENMULTIPLIER, (int)((screenSizeRectangle.Width * SCREENMULTIPLIER) + Camera.cameraOrigin.X) + 1);
                starArrayForeground[i].Y = random.Next((int)Camera.cameraOrigin.Y - screenSizeRectangle.Height * SCREENMULTIPLIER, (int)((screenSizeRectangle.Height * SCREENMULTIPLIER) + Camera.cameraOrigin.Y) + 1);
            }
        }

        public void Update()
        {
            if (buildStarsBool)
            {
                this.buildStars();
                buildStarsBool = false;
            }

            //BACKGROUND
            for (int i = 0; i < MAXBACKGROUNDSTARS; i++)
            {
                //X (Greater than)
                if (starArrayBackground[i].X > (screenSizeRectangle.Width * SCREENMULTIPLIER) + Camera.cameraOrigin.X)
                {
                    starArrayBackground[i].X = Camera.cameraOrigin.X - (screenSizeRectangle.Width);
                }
                //Y (Greater than)
                if (starArrayBackground[i].Y > (screenSizeRectangle.Height * SCREENMULTIPLIER) + Camera.cameraOrigin.Y)
                {
                    starArrayBackground[i].Y = Camera.cameraOrigin.Y - (screenSizeRectangle.Width);
                }
                //X (Less than)
                if (starArrayBackground[i].X < Camera.cameraOrigin.X - (screenSizeRectangle.Width * SCREENMULTIPLIER))
                {
                    starArrayBackground[i].X = Camera.cameraOrigin.X + (screenSizeRectangle.Width * SCREENMULTIPLIER);
                }
                //Y (Less than)
                if (starArrayBackground[i].Y < Camera.cameraOrigin.Y - (screenSizeRectangle.Height * SCREENMULTIPLIER))
                {
                    starArrayBackground[i].Y = Camera.cameraOrigin.Y + (screenSizeRectangle.Height * SCREENMULTIPLIER);
                }
            }

            //FOREGROUND
            for (int i = 0; i < MAXFOREGROUNDSTARS; i++)
            {
                //X (Greater than)
                if (starArrayForeground[i].X > (screenSizeRectangle.Width * SCREENMULTIPLIER) + Camera.cameraOrigin.X)
                {
                    starArrayForeground[i].X = Camera.cameraOrigin.X - (screenSizeRectangle.Width);
                }
                //Y (Greater than)
                if (starArrayForeground[i].Y > (screenSizeRectangle.Height * SCREENMULTIPLIER) + Camera.cameraOrigin.Y)
                {
                    starArrayForeground[i].Y = Camera.cameraOrigin.Y - (screenSizeRectangle.Width);
                }
                //X (Less than)
                if (starArrayForeground[i].X < Camera.cameraOrigin.X - (screenSizeRectangle.Width * SCREENMULTIPLIER))
                {
                    starArrayForeground[i].X = Camera.cameraOrigin.X + (screenSizeRectangle.Width * SCREENMULTIPLIER);
                }
                //Y (Less than)
                if (starArrayForeground[i].Y < Camera.cameraOrigin.Y - (screenSizeRectangle.Height * SCREENMULTIPLIER))
                {
                    starArrayForeground[i].Y = Camera.cameraOrigin.Y + (screenSizeRectangle.Height * SCREENMULTIPLIER);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //BACKGROUND
            for (int i = 0; i < MAXBACKGROUNDSTARS; i++)
            {
                spriteBatch.Draw(starTexture, starArrayBackground[i], starRectangleBackground, Color.White);
            }

            //FOREGROUND
            for (int i = 0; i < MAXFOREGROUNDSTARS; i++)
            {
                spriteBatch.Draw(starTexture, starArrayForeground[i], starRectangleForeground, Color.White);
            }
        }
    }
}

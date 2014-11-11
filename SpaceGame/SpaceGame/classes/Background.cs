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

        //Star Texture - WhiteTexture
        Texture2D starTexture;
        //The size of the star rectangle in the background
        Rectangle starRectangleBackground;
        const int STAR_WIDTH_AND_HEIGHT_BACKGROUND = 2;
        //The size of the star rectangle in the foreground
        Rectangle starRectangleForeground;
        const int STAR_WIDTH_AND_HEIGHT_FOREGROUND = 9;

        //Random class to get random variables
        Random random;

        //Buffer for background stars
        const int STAR_BUFFER_ZONE = 10;

        //On first update method, build stars
        bool buildStarsBool = true;

        const int SCREEN_MULTIPLIER = 2;

        const int BACKGROUND_SPEED_PARALAX = 20;

        const int MAX_BACKGROUND_STARS = 300;
        const int MAX_FOREGROUND_STARS = 60;
        //1-D array of Vector2s for foreground
        Vector2[] starArrayForeground = new Vector2[MAX_FOREGROUND_STARS];
        //1-D array of Vector2s for background
        Vector2[] starArrayBackground = new Vector2[MAX_BACKGROUND_STARS];

        public Background(IServiceProvider serviceProvider, Rectangle initScreenSizeRectangle)
        {
            content = new ContentManager(serviceProvider, "Content");

            random = new Random();

            screenSizeRectangle = initScreenSizeRectangle;

            starRectangleBackground = new Rectangle(0, 0, STAR_WIDTH_AND_HEIGHT_BACKGROUND, STAR_WIDTH_AND_HEIGHT_BACKGROUND);
            starRectangleForeground = new Rectangle(0, 0, STAR_WIDTH_AND_HEIGHT_FOREGROUND, STAR_WIDTH_AND_HEIGHT_FOREGROUND);

            this.LoadContent();
        }

        public void LoadContent()
        {
            starTexture = content.Load<Texture2D>("whiteTexture");
        }

        public void buildStars()
        {
            /* UNCOMMENT IF BACKGROUND ISNT STATIC
            //BACKGROUND
            for (int i = 0; i < MAXBACKGROUNDSTARS; i++)
            {
                starArrayBackground[i].X = random.Next((int)Camera.cameraOrigin.X - screenSizeRectangle.Width * SCREENMULTIPLIER, (int)((screenSizeRectangle.Width * SCREENMULTIPLIER) + Camera.cameraOrigin.X) + 1);
                starArrayBackground[i].Y = random.Next((int)Camera.cameraOrigin.Y - screenSizeRectangle.Height * SCREENMULTIPLIER, (int)((screenSizeRectangle.Height * SCREENMULTIPLIER) + Camera.cameraOrigin.Y) + 1);
            }
            */

            //BACKGROUND
            for (int i = 0; i < MAX_BACKGROUND_STARS; i++)
            {
                starArrayBackground[i].X = random.Next(-STAR_BUFFER_ZONE, (screenSizeRectangle.Width + STAR_BUFFER_ZONE) + 1);
                starArrayBackground[i].Y = random.Next(-STAR_BUFFER_ZONE, (screenSizeRectangle.Height + STAR_BUFFER_ZONE) + 1);
            }

            //FOREGROUND
            for (int i = 0; i < MAX_FOREGROUND_STARS; i++)
            {
                starArrayForeground[i].X = random.Next((int)Camera.cameraOrigin.X - screenSizeRectangle.Width * SCREEN_MULTIPLIER, (int)((screenSizeRectangle.Width * SCREEN_MULTIPLIER) + Camera.cameraOrigin.X) + 1);
                starArrayForeground[i].Y = random.Next((int)Camera.cameraOrigin.Y - screenSizeRectangle.Height * SCREEN_MULTIPLIER, (int)((screenSizeRectangle.Height * SCREEN_MULTIPLIER) + Camera.cameraOrigin.Y) + 1);
            }
        }

        public void Update(Vector2 playerLocationDeltaVector)
        {
            if (buildStarsBool)
            {
                this.buildStars();
                buildStarsBool = false;
            }
            //BACKGROUND
            for (int i = 0; i < MAX_BACKGROUND_STARS; i++)
            {
                starArrayBackground[i].X -= playerLocationDeltaVector.X / BACKGROUND_SPEED_PARALAX;
                starArrayBackground[i].Y -= playerLocationDeltaVector.Y / BACKGROUND_SPEED_PARALAX;

                //X (Greater than)
                if (starArrayBackground[i].X > (screenSizeRectangle.Width + STAR_BUFFER_ZONE))
                {
                    starArrayBackground[i].X = -STAR_BUFFER_ZONE;
                }
                //X (Less than)
                if (starArrayBackground[i].X < -STAR_BUFFER_ZONE)
                {
                    starArrayBackground[i].X = screenSizeRectangle.Width + STAR_BUFFER_ZONE;
                }
                //Y (Greater than)
                if (starArrayBackground[i].Y > (screenSizeRectangle.Height + STAR_BUFFER_ZONE))
                {
                    starArrayBackground[i].Y = -STAR_BUFFER_ZONE;
                }
                //Y (Less than)
                if (starArrayBackground[i].Y < -STAR_BUFFER_ZONE)
                {
                    starArrayBackground[i].Y = screenSizeRectangle.Height + STAR_BUFFER_ZONE;
                }
            }

            //FOREGROUND
            for (int i = 0; i < MAX_FOREGROUND_STARS; i++)
            {
                //X (Greater than)
                if (starArrayForeground[i].X > (screenSizeRectangle.Width * SCREEN_MULTIPLIER) + Camera.cameraOrigin.X)
                {
                    starArrayForeground[i].X = Camera.cameraOrigin.X - (screenSizeRectangle.Width);
                }
                //Y (Greater than)
                if (starArrayForeground[i].Y > (screenSizeRectangle.Height * SCREEN_MULTIPLIER) + Camera.cameraOrigin.Y)
                {
                    starArrayForeground[i].Y = Camera.cameraOrigin.Y - (screenSizeRectangle.Width);
                }
                //X (Less than)
                if (starArrayForeground[i].X < Camera.cameraOrigin.X - (screenSizeRectangle.Width * SCREEN_MULTIPLIER))
                {
                    starArrayForeground[i].X = Camera.cameraOrigin.X + (screenSizeRectangle.Width * SCREEN_MULTIPLIER);
                }
                //Y (Less than)
                if (starArrayForeground[i].Y < Camera.cameraOrigin.Y - (screenSizeRectangle.Height * SCREEN_MULTIPLIER))
                {
                    starArrayForeground[i].Y = Camera.cameraOrigin.Y + (screenSizeRectangle.Height * SCREEN_MULTIPLIER);
                }
            }
        }

        public void DrawBackgroundStars(SpriteBatch spriteBatch)
        {
            //BACKGROUND
            for (int i = 0; i < MAX_BACKGROUND_STARS; i++)
            {
                spriteBatch.Draw(starTexture, starArrayBackground[i], starRectangleBackground, Color.White);
            }
        }

        public void DrawForegroundStars(SpriteBatch spriteBatch)
        {
            //FOREGROUND
            for (int i = 0; i < MAX_FOREGROUND_STARS; i++)
            {
                spriteBatch.Draw(starTexture, starArrayForeground[i], starRectangleForeground, Color.White);
            }
        }
    }
}

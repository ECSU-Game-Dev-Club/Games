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
    class Bullet_prototype : Bullet
    {
        const int BULLET_VELOCITY_INCREMENT = 13;
        float bulletVelocityXIncrement;
        float bulletVelocityYIncrement;

        Vector2 bulletLocation;

        Vector2 bulletVelocity;

        Rectangle bulletHitBox;
        const int BULLET_HITBOX_WIDTH = 3;//3
        const int BULLET_HITBOX_HEIGHT = 16;//16

        Rectangle bulletRectangle;
        const int WIDTH = 3;//3
        const int HEIGHT = 16;//16

        const int BULLET_LIFETIME = 3;//in seconds
        int gameTimeStamp;

        bool playerBullet;

        bool deleteMeBool = false;

        float rotation;

        Vector2 bulletOrigin;

        public Bullet_prototype(float spawnX, float spawnY, Vector2 startingVelocity, float init_Rotation, bool init_PlayerBullet, GameTime gameTime, Vector2 gamePad_Thumbstick_Right)
        {
            gameTimeStamp = gameTime.TotalGameTime.Seconds;

            playerBullet = init_PlayerBullet;

            bulletVelocity = startingVelocity;

            rotation = init_Rotation;

            if (Math.Abs(gamePad_Thumbstick_Right.X) + Math.Abs(gamePad_Thumbstick_Right.Y) > 0.1)
            {
                bulletVelocityXIncrement = gamePad_Thumbstick_Right.X * BULLET_VELOCITY_INCREMENT;
                bulletVelocityYIncrement = (gamePad_Thumbstick_Right.Y * BULLET_VELOCITY_INCREMENT) * -1;
            }
            else
            {
                bulletVelocityXIncrement = 0;
                bulletVelocityYIncrement = (1 * BULLET_VELOCITY_INCREMENT) * -1;
            }

            bulletRectangle = new Rectangle(0, 0, WIDTH, HEIGHT);

            bulletHitBox = new Rectangle((int)bulletLocation.X, (int)bulletLocation.Y, BULLET_HITBOX_WIDTH, BULLET_HITBOX_HEIGHT);

            bulletOrigin = new Vector2(bulletRectangle.Width / 2, bulletRectangle.Height / 2);

            bulletLocation.X = spawnX;
            bulletLocation.Y = spawnY;
        }

        public override void update(GameTime gameTime)
        {
            bulletLocation.X += bulletVelocity.X + bulletVelocityXIncrement;
            bulletLocation.Y += bulletVelocity.Y + bulletVelocityYIncrement;

            bulletHitBox = new Rectangle((int)bulletLocation.X, (int)bulletLocation.Y, BULLET_HITBOX_WIDTH, BULLET_HITBOX_HEIGHT);

            if (Math.Abs(gameTime.TotalGameTime.Seconds - gameTimeStamp) > BULLET_LIFETIME)
            {
                deleteMeBool = true;
            }
        }

        public override bool deleteMe()
        {
            return deleteMeBool;
        }

        public override float getRotation()
        {
            return rotation;
        }

        public override Vector2 getLocationVector()
        {
            return bulletLocation;
        }

        public override Rectangle getRectangle()
        {
            return bulletRectangle;
        }

        public override Rectangle getHitBox()
        {
            return bulletHitBox;
        }

        public override Vector2 getBulletOrigin()
        {
            return bulletOrigin;
        }
    }
}

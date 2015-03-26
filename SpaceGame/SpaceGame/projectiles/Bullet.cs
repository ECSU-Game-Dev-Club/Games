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
    class Bullet : Projectile
    {
        const int BULLET_VELOCITY_INCREMENT = 13;

        float bulletVelocityXIncrement;
        float bulletVelocityYIncrement;

        const int BULLET_LIFETIME = 3;//in seconds

        int gameTimeStamp;

        Random rand;

        int bulletSpread = 48;//DEFAULT: 48

        public Bullet(float spawnX, float spawnY, Vector2 startingVelocity, 
            float init_Rotation, bool init_PlayerBullet, GameTime gameTime)
        {
            drawBox_Width = 3;
            drawBox_Height = 16;
            hitBox_Width = 3;
            hitBox_Height = 16;

            rand = new Random();

            //Current projectile(1=gatling 2=missile 3=??? 4=???)
            currentProjectile = 1;
            damage = 1;

            gameTimeStamp = gameTime.TotalGameTime.Seconds;

            playerBullet = init_PlayerBullet;

            deleteMeBool = false;

            velocity = startingVelocity;

            rotation = init_Rotation;

            bulletVelocityXIncrement = (float)(Math.Cos((rotation + (0.5 * Math.PI) - ((rand.NextDouble() * Math.PI / bulletSpread) * (float)rand.Next(-1, 2)))) * BULLET_VELOCITY_INCREMENT);
            bulletVelocityYIncrement = (float)(Math.Sin((rotation + (0.5 * Math.PI) - ((rand.NextDouble() * Math.PI / bulletSpread) * (float)rand.Next(-1, 2)))) * BULLET_VELOCITY_INCREMENT);

            //Drawbox DOES NOT need to move, thus X=0 Y=0
            drawBox = new Rectangle(0, 0, drawBox_Width, drawBox_Height);

            hitBox = new Rectangle((int)position.X, (int)position.Y, hitBox_Width, hitBox_Height);

            origin = new Vector2(drawBox.Width / 2, drawBox.Height / 2);

            position.X = spawnX;
            position.Y = spawnY;
        }

        public override void update(GameTime gameTime)
        {
            position.X += velocity.X + bulletVelocityXIncrement;
            position.Y += velocity.Y + bulletVelocityYIncrement;

            hitBox = new Rectangle((int)position.X, (int)position.Y, hitBox_Width, hitBox_Height);

            if (Math.Abs(gameTime.TotalGameTime.Seconds - gameTimeStamp) > BULLET_LIFETIME)
            {
                deleteMeBool = true;
            }
        }

        
    }
}

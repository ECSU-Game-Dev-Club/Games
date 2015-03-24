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
    class Projectile
    {
        public Vector2 position;
        public Vector2 velocity;
        public float acceleration;

        public bool deleteMeBool;

        public int drawBox_Width;
        public int drawBox_Height;
        public int hitBox_Width;
        public int hitBox_Height;

        public float rotation;//Rotation of projectile

        public Rectangle drawBox;//An area that the object is drawn in

        public Rectangle hitBox;

        public Vector2 origin;//Middle of projectile

        public bool playerBullet;

        public int currentProjectile;

        public int damage;

        public Projectile()
        {

        }

        public virtual void update(GameTime gameTime)
        {
            position += velocity;
        }

        public int getDamageOfProjectile()
        {
            return damage;
        }

        public int getCurrentProjectile()
        {
            return currentProjectile;
        }

        public float getRotation()
        {
            return rotation;
        }

        public bool deleteMe()
        {
            return deleteMeBool;
        }

        public Vector2 getLocationVector()
        {
            return position;
        }

        public Vector2 getBulletOrigin()
        {
            return origin;
        }

        public Rectangle getHitBox()
        {
            return hitBox;
        }

        public Rectangle getDrawBox()
        {
            return drawBox;
        }
    }
}

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
    class Missile : Projectile
    {
        const float MAXTURNSPEED = 0.1f;
        const float MAXVELOCITY = 20;

        float rotation;

        Vector2 enemyPosition;

        bool deleteMeBool;

        public Missile(Vector2 enemyPos, Vector2 origPos, float origRotation)
        {
            position = origPos;

            enemyPosition = enemyPos;

            deleteMeBool = false;
        }

        public void updateMissiles()
        {
            rotation = Helper.TurnToFace_Radians(position, enemyPosition, rotation, MAXTURNSPEED);

            Vector2 heading = new Vector2((float)Math.Cos(rotation), (float)Math.Sin(rotation));
            position += heading * MAXVELOCITY;
        }

        public bool deleteMe()
        {
            return deleteMeBool;
        }
    }
}

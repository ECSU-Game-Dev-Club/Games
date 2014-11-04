using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SpaceGame
{
    class Gravity
    {
        //gravitational constant is the strength of gravity in the game
        const float GRAVITATIONALCONSTANT = 0.1f;
        double mass1;
        Vector2 gLocationVector;
        double gLocationX;
        double gLocationY;

        double distance;
        double degree;
        //actualAcceleration is the acceleration as a 1D value
        //gVectorAcceleration turns the 1D acceleration into a 2D vector for use in our game
        double gActualAcceleration;
        public Vector2 gVectorAcceleration;

        //X and Y is the location for the gravity well
        public Gravity(double x, double y, double initMass)
        {
            mass1 = initMass;

            gLocationX = x;
            gLocationY = y;

            gLocationVector = new Vector2((float)x, (float)y);
        }

        public double getGravityLocationX()
        {
            return gLocationX;
        }

        public double getGravityLocationY()
        {
            return gLocationY;
        }

        public Vector2 getGravityLocationVector()
        {
            return gLocationVector;
        }

        //calculates gravitational pull of xy1 on xy2
        public Vector2 calcGVectorAcceleration(double x2, double y2, double mass2)
        {
            distance = Math.Sqrt(Math.Pow(gLocationX - x2, 2) + Math.Pow(gLocationY - y2, 2));
            degree = (Math.Atan((gLocationY - y2) / (gLocationX - x2))) * 180 / Math.PI;
            gActualAcceleration = -1 * (GRAVITATIONALCONSTANT * mass1 * mass2) / Math.Pow(distance, 2);

            if (gLocationX - x2 >= 0)
            {
                gVectorAcceleration.X = (float)(-1 * (gActualAcceleration * Math.Cos(degree * Math.PI / 180)));
            }
            else
            {
                gVectorAcceleration.X = (float)(gActualAcceleration * Math.Cos(degree * Math.PI / 180));
            }

            if (gLocationX - x2 >= 0)
            {
                gVectorAcceleration.Y = (float)(-1 * (gActualAcceleration * Math.Sin(degree * Math.PI / 180)));
            }
            else
            {
                gVectorAcceleration.Y = (float)(gActualAcceleration * Math.Sin(degree * Math.PI / 180));
            }


            //Console.Write("{0, 10}", gVectorAcceleration.X + "       ");
            //Console.Write("{0, 10}", gVectorAcceleration.Y);
            //Console.WriteLine(" ");

            return gVectorAcceleration;
        }
    }
}

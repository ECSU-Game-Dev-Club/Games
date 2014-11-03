using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SpaceGame
{
    class Gravity
    {
        double pi = 3.14159265;
        double distance;
        double degree;
        //gravitational constant is the strength of gravity in the game
        double gravitationalConstant;
        //actualAcceleration is the acceleration as a 1D value
        //gVectorAcceleration turns the 1D acceleration into a 2D vector for use in our game
        double gActualAcceleration;
        public Vector2 gVectorAcceleration;
        public Vector2 gPlayerVectorAcceleration;  //for player class
        double[,] gWellLocationMass = new double[10, 3]; //0 = x, 1 = y, 2 = mass;

        public Gravity(float initGravitationalConstant)
        {
            gravitationalConstant = initGravitationalConstant;
        }

        public Vector2 calcGPlayerVectorAcceleration(double x2, double y2, double mass2)
        {
            Vector2 temp;
            temp.X = 0;
            temp.Y = 0;
            for (int k = 0; k < gWellLocationMass.GetUpperBound(0); k++)
            {
                temp += calcGVectorAcceleration(gWellLocationMass[k, 0], gWellLocationMass[k, 1], x2, y2, gWellLocationMass[k, 2], mass2);
            }
            gPlayerVectorAcceleration = temp;
            return gPlayerVectorAcceleration;
        }
        
        //calculates gravitational pull of xy1 on xy2
        public Vector2 calcGVectorAcceleration(double x1, double y1, double x2, double y2, double mass1, double mass2)
        {
            distance = Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));
            degree = (Math.Atan((y1 - y2) / (x1 - x2))) * 180 / pi;
            gActualAcceleration = -1 * (gravitationalConstant * mass1 * mass2) / Math.Pow(distance, 2);
            if (x1 - x2 >= 0)
                gVectorAcceleration.X = (float)(-1 * (gActualAcceleration * Math.Cos(degree * pi / 180)));
            else
                gVectorAcceleration.X = (float)(gActualAcceleration * Math.Cos(degree * pi / 180));

            if (x1 - x2 >= 0)
                gVectorAcceleration.Y = (float)(-1 * (gActualAcceleration * Math.Sin(degree * pi / 180)));
            else
                gVectorAcceleration.Y = (float)(gActualAcceleration * Math.Sin(degree * pi / 180));


            Console.Write("{0, 10}", gVectorAcceleration.X + "       ");
            Console.Write("{0, 10}", gVectorAcceleration.Y);
            Console.WriteLine(" ");

            return gVectorAcceleration;
        }
    }
}

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
        //vectorAcceleration turns the 1D acceleration into a 2D vector for use in our game
        double actualAcceleration;  
        public Vector2 vectorAcceleration; 
       
       public Gravity(float initGravitationalConstant)
        {
            gravitationalConstant = initGravitationalConstant;
        } 
        //calculates gravitational pull of xy1 on xy2
        public Vector2 calcVectorAcceleration(float x1,float y1, float x2, float y2, float mass1, float mass2)
        {
            distance = Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));
            degree = Math.Tan((x1 - x2) / (y1 - y2) * pi / 180);
            actualAcceleration = (gravitationalConstant * mass1 * mass2) / Math.Pow(distance, 2);
            vectorAcceleration.X = (float) (actualAcceleration * Math.Cos(degree * pi / 180));
            vectorAcceleration.Y = (float) (actualAcceleration * Math.Sin(degree * pi / 180));
            return vectorAcceleration;
        }
    }
}

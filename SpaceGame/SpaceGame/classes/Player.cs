using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SpaceGame
{
    class Player
    {
        Gravity gravity = new Gravity(.1f);
        Rectangle playerRectangle;
        int width = 10;
        int height = 10;
        public double playerMass;

        //Players location
        public Vector2 playerLocation;

        //Players velocity and acceleration for calculating speed
        Vector2 playerVelocity;
        Vector2 playerAcceleration;
        Vector2 playerGVectorAcceleration;

        //thrust is the triggers on the gamepad
        Vector2 playerThrust;
        static float playerThrustScale = .1f;
        
        //Maximum player speed
        static float playerMax = 10; //NOT CORRECT   

        //Constructor for player, starts everything
        public Player(float x, float y, double mass)
        {
            playerLocation.X = x;
            playerLocation.Y = y;
            playerMass = mass;

            playerThrust = new Vector2(0, 0);

            playerAcceleration = new Vector2(0, 0);

            playerRectangle = new Rectangle((int)playerLocation.X, (int)playerLocation.Y, width, height);
        }

        public void update(GameTime gameTime)
        {
            calcAcceleration(gameTime);

            playerLocation.X += playerVelocity.X;
            playerLocation.Y += playerVelocity.Y;

            playerRectangle = new Rectangle((int)playerLocation.X, (int)playerLocation.Y, width, height);
        }

        public void setThrust(Vector2 initThrust)
        {
            playerThrust = initThrust;
        }

        public Rectangle getPlayerRectangle()
        {
            return playerRectangle;
        }

        public Vector2 getPlayerVelocityVector()
        {
            return playerVelocity;
        }

        public Vector2 getPlayerAccelerationVector()
        {
            return playerAcceleration;
        }
 
        public Vector2 getPlayerLocation()
        {
            return playerLocation;
        }

        void setPlayerGVectorAcceleration(Vector2 gVectorAcceleration)
        {
           playerGVectorAcceleration = gVectorAcceleration;
        }

        private void calcAcceleration(GameTime gameTime)
        {
            // (300,300) is a temporary value for gravity well location, 10000f is a temporary value for gravity well mass (planet mass)
            setPlayerGVectorAcceleration(gravity.calcGVectorAcceleration(300, 300, playerLocation.X, playerLocation.Y, 30000f, playerMass));
            playerAcceleration = (playerThrust * playerThrustScale) /*add gravity effect here*/ + playerGVectorAcceleration;
            /* temporary test for 2 gravity wells (SUPER COOOOL)
            setPlayerGVectorAcceleration(gravity.calcGVectorAcceleration(700, 700, playerLocation.X, playerLocation.Y, 30000f, playerMass));
            playerAcceleration += playerGVectorAcceleration;
            */ 
            playerVelocity += playerAcceleration;

            playerVelocity.X = MathHelper.Clamp(playerVelocity.X, (-1) * playerMax, playerMax);
            playerVelocity.Y = MathHelper.Clamp(playerVelocity.Y, (-1) * playerMax, playerMax);
        }
    }
}

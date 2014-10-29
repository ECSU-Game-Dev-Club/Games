﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SpaceGame
{
    class Player
    {
        Gravity gravity;
        Rectangle playerRectangle;
        int width = 10;
        int height = 10;
        double playerMass;

        //Players location
        Vector2 playerLocation;

        //Players velocity and acceleration for calculating speed
        Vector2 playerVelocity;
        Vector2 playerAcceleration;
        Vector2 playerVectorAcceleration;

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

        public void setPlayerVectorAcceleration(Vector2 vectorAcceleration)
        {
           playerVectorAcceleration = vectorAcceleration;
        }

        private void calcAcceleration(GameTime gameTime)
        {
            playerAcceleration = (playerThrust * playerThrustScale) /*add gravity effect here*/ + playerVectorAcceleration;
            playerVelocity += playerAcceleration;

            playerVelocity.X = MathHelper.Clamp(playerVelocity.X, (-1) * playerMax, playerMax);
            playerVelocity.Y = MathHelper.Clamp(playerVelocity.Y, (-1) * playerMax, playerMax);
        }
    }
}

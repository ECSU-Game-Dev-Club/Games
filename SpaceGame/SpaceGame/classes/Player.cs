﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SpaceGame
{
    class Player
    {
        Rectangle playerRectangle;
        int width = 10;
        int height = 10;

        //Players location
        Vector2 playerLocation;

        //Players velocity and acceleration for calculating speed
        Vector2 playerVelocity;
        Vector2 playerAcceleration;

        //thrust is the triggers on the gamepad
        Vector2 playerThrust;

        //Constructor for player, starts everything
        public Player(float x, float y)
        {
            playerLocation.X = x;
            playerLocation.Y = y;

            playerThrust = new Vector2(0, 0);

            playerVelocity = playerLocation;
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

        private void calcAcceleration(GameTime gameTime)
        {
            playerAcceleration.X = playerThrust.X; //add gravity effect here
            playerAcceleration.Y = playerThrust.Y; //add gravity effect here

            playerVelocity.X = playerAcceleration.X + playerVelocity.X;
            playerVelocity.Y = playerAcceleration.Y + playerVelocity.Y;
        }

    }
}

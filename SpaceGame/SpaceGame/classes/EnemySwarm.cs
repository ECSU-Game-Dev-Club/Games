﻿using System;
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
    class EnemySwarm
    {
        Rectangle enemyRectangle;
        int width = 20;
        int height = 20;

        Random random;

        //Create a new content manager to load content used just by this level.
        ContentManager content;

        //Player origin(or middle of player)
        Vector2 enemyOrigin;

        //Target Stuff
        const int TARGET_RADIUS = 400;
        const int TARGET_ATTACH_RADIUS = 20;

        double[] playersDistances = new double[4];

        int targetIndex;
        double targetDistance;
        Vector2 targetVector;
        Vector2 secondaryTargetVector;

        bool targetAquired = false;
        bool targetAttached = false;

        //Players texture
        Texture2D enemyTexture;

        //Players location
        Vector2 enemyLocation;

        //Players velocity and acceleration for calculating speed
        Vector2 enemyVelocity;
        Vector2 enemyAcceleration;
        //Player Rotation redians
        double enemyRotation;
        Vector2 temp;

        //Enemy mass
        const double ENEMY_MASS = 1;

        //thrust is the location of thumbstick left
        Vector2 enemyThrust;

        const float ENEMY_THRUST_SCALE = .1f;

        //Player Boost Velocity
        const float ENEMY_BOOST_VELOCITY = 5;

        //DEVELOPER FUN COMMANDS
        const bool ENEMIES_MERCILESS = false;

        //Constructor for player, starts/initializes everything
        public EnemySwarm(float x, float y, IServiceProvider serviceProvider)
        {
            content = new ContentManager(serviceProvider, "Content");

            random = new Random();

            temp = new Vector2();

            enemyLocation.X = x;
            enemyLocation.Y = y;

            enemyThrust = new Vector2(0, 0);

            enemyAcceleration = new Vector2(0, 0);

            enemyRectangle = new Rectangle(0, 0, width, height);

            enemyOrigin.X = width / 2;
            enemyOrigin.Y = height / 2;

            enemyRotation = 0;

            targetVector = new Vector2(0, 0);
            secondaryTargetVector = new Vector2(0, 0);

            this.LoadContent();
        }

        private void LoadContent()
        {
            enemyTexture = content.Load<Texture2D>("Enemy/Swarmling");
        }

        //Updates the player every frame
        public void update(List<Gravity> gravityList, Player[] players)
        {
            //Console.WriteLine("Player Velocity: " + enemyVelocity);
            Console.WriteLine("Enemy Location: " + enemyLocation);
            //Console.WriteLine("Player Acceleration: " + enemyAcceleration);

            #region"Get closest player"
            for (int i = 0; i < players.Count(); i++)
            {
                if (players[i].isPlayerReady())//Is the player playing.
                {
                    //Get distance between player and me
                    playersDistances[i] = Math.Sqrt(Math.Pow(players[i].getPlayerLocation().X - enemyLocation.X, 2) + Math.Pow(players[i].getPlayerLocation().Y - enemyLocation.Y, 2));

                    //Is the player within my vision
                    if (playersDistances[i] <= TARGET_RADIUS)
                    {
                        //If no target
                        if (!targetAquired)
                        {
                            //Target found
                            targetAquired = true;

                            //Keep track of what player I am targeting
                            targetIndex = i;

                            //Keep track of distance between the target and me
                            targetDistance = playersDistances[i];
                        }
                    }
                }
            }

            if (!ENEMIES_MERCILESS)
            {
                if (targetDistance > TARGET_RADIUS)
                {
                    targetAquired = false;
                }
            }
            #endregion

            #region"Attach to player if near"
            //If player is within radius -X-
            if (playersDistances[targetIndex] < TARGET_ATTACH_RADIUS)
            {
                targetAttached = true;
                enemyThrust.X = 0;
                enemyThrust.Y = 0;
                enemyVelocity.X = 0;
                enemyVelocity.Y = 0;
                enemyAcceleration.X = 0;
                enemyAcceleration.X = 0;
            }

            if (targetAttached)
            {
                enemyLocation.X = random.Next((int)players[targetIndex].getPlayerLocation().X - (players[targetIndex].getPlayerRectangle().Width / 2), (int)players[targetIndex].getPlayerLocation().X + (players[targetIndex].getPlayerRectangle().Width / 2)) - (width / 2);
                enemyLocation.Y = random.Next((int)players[targetIndex].getPlayerLocation().Y - (players[targetIndex].getPlayerRectangle().Width / 2), (int)players[targetIndex].getPlayerLocation().Y + (players[targetIndex].getPlayerRectangle().Width / 2)) - (height / 2);

            }
            #endregion

            #region"Move toward player"
            //If target acquired update the target vector with the targets position
            if (targetAquired)
            {
                targetVector.X = players[targetIndex].getPlayerLocation().X - (players[targetIndex].getPlayerRectangle().Width / 2);
                targetVector.Y = players[targetIndex].getPlayerLocation().Y - (players[targetIndex].getPlayerRectangle().Height / 2);

                Vector2 targetDistanceFromMe = new Vector2(players[targetIndex].getPlayerLocation().X - enemyLocation.X, players[targetIndex].getPlayerLocation().Y - enemyLocation.Y);

                //Move toward target
                if (targetVector.X < enemyLocation.X)
                {
                    enemyThrust.X = -1;
                }
                if (targetVector.X > enemyLocation.X)
                {
                    enemyThrust.X = 1;
                }
                if (targetVector.Y < enemyLocation.Y)
                {
                    enemyThrust.Y = -1;
                }
                if (targetVector.Y > enemyLocation.Y)
                {
                    enemyThrust.Y = 1;
                }
            }
            else
            {
                targetVector.X = 0;
                targetVector.Y = 0;
            }
            #endregion

            if (players[targetIndex].getPlayerBoost() && targetAttached)
            {
                targetAquired = false;
            }

            if (!targetAttached)
            {
                calcAcceleration(gravityList);
            }

            //Updates enemy location based on velocity
            enemyLocation += enemyVelocity;
            enemyRotation = (double)Math.Atan2((double)enemyVelocity.Y , (double)enemyVelocity.X) + (Math.PI / 2);
        }

        /// <summary>
        /// Which direction to thrust in
        /// </summary>
        /// <param name="initThrust">Provides a Vector2 X(0-1) and Y(0-1). This vector comes from LEFT THUMBSTICK</param>
        public void setThrust(Vector2 initThrust)
        {
            enemyThrust = initThrust;

            enemyThrust.Y = enemyThrust.Y * -1;
            enemyThrust.X = enemyThrust.X * -1;
        }

        public Vector2 getEnemyLocationVector()
        {
            return enemyLocation;
        }

        /// <summary>
        /// Calculate Acceleration
        /// and calculate predictive path
        /// </summary>
        /// <param name="gravityList">Provides a list of all gravity wells near you.</param>
        private void calcAcceleration(List<Gravity> gravityList)
        {
            for (int i = 0; i < gravityList.Count(); i++)
            {
                temp += gravityList[i].calcGVectorAcceleration(enemyLocation.X, enemyLocation.Y, ENEMY_MASS);
            }

            enemyAcceleration = (enemyThrust * ENEMY_THRUST_SCALE) /*add gravity effect here*/ + temp;

            temp = new Vector2();

            enemyVelocity += enemyAcceleration;
        }

        /// <summary>
        /// Draws the enemy location
        /// </summary>
        /// <param name="spriteBatch">Provides the SpriteBatch to allow drawing.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(enemyTexture, enemyLocation, enemyRectangle, Color.LightBlue, (float)enemyRotation, enemyOrigin, 1.0f, SpriteEffects.FlipHorizontally, 0);
        }
    }
}

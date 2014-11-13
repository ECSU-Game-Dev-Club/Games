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
    class Enemy_prototype
    {
        Rectangle enemyRectangle;
        Rectangle enemyPredictedRectangle;
        int width = 20;
        int height = 20;

        //Create a new content manager to load content used just by this level.
        ContentManager content;

        //Player origin(or middle of player)
        Vector2 enemyOrigin;

        //Target Stuff
        int targetIndex;
        Vector2 targetVector;
        Vector2 secondaryTargetVector;
        bool targetAquired;

        //Players texture
        Texture2D enemyTexture;
        Texture2D enemyPredictionTexture;

        //Players location
        Vector2 enemyLocation;

        //Players velocity and acceleration for calculating speed
        Vector2 enemyVelocity;
        Vector2 enemyAcceleration;
        //Player Rotation redians
        float enemyRotation;

        //Enemy mass
        const double ENEMY_MASS = 1;

        //Enemy 
        const int TARGET_RADIUS = 400;

        //predictive
        const int MAX_PREDICTED_FRAMES = 50;
        Vector2[] enemyPredictedLocation = new Vector2[MAX_PREDICTED_FRAMES];
        Vector2[] enemyPredictedVelocity = new Vector2[MAX_PREDICTED_FRAMES];
        Vector2[] enemyPredictedAcceleration = new Vector2[MAX_PREDICTED_FRAMES];

        //thrust is the location of thumbstick left
        Vector2 enemyThrust;

        const float ENEMY_THRUST_SCALE = .1f;

        //Player Boost Velocity
        const float ENEMY_BOOST_VELOCITY = 5;

        //DEVELOPER FUN COMMANDS
        const bool ENEMIES_MERCILESS = true;

        //Constructor for player, starts/initializes everything
        public Enemy_prototype(float x, float y, IServiceProvider serviceProvider)
        {
            content = new ContentManager(serviceProvider, "Content");

            enemyLocation.X = x;
            enemyLocation.Y = y;

            enemyThrust = new Vector2(0, 0);

            enemyAcceleration = new Vector2(0, 0);

            enemyRectangle = new Rectangle(0, 0, width, height);
            enemyPredictedRectangle = new Rectangle(0, 0, 30, 30);

            enemyOrigin.X = width / 2;
            enemyOrigin.Y = height / 2;

            enemyRotation = 0;

            targetVector = new Vector2(0, 0);
            secondaryTargetVector = new Vector2(0, 0);

            targetAquired = false;

            this.LoadContent();
        }

        private void LoadContent()
        {
            enemyTexture = content.Load<Texture2D>("enemy/Swarmling");

            enemyPredictionTexture = content.Load<Texture2D>("whiteTexture");
        }

        //Updates the player every frame
        public void update(List<Gravity> gravityList, Player[] players)
        {
            //Console.WriteLine("Player Velocity: " + enemyVelocity);
            Console.WriteLine("Enemy Location: " + enemyLocation);
            //Console.WriteLine("Player Acceleration: " + enemyAcceleration);

            #region"Get closest player"
            //If target has not been aquired
            if (!targetAquired)
            {
                //For all players
                for (int i = 0; i < players.Count() - 1; i++)
                {
                    if (players[i].isPlayerReady())
                    {
                        //If player is within radius -X-
                        if (players[i].getPlayerLocation().X >= enemyLocation.X - TARGET_RADIUS && players[i].getPlayerLocation().X <= enemyLocation.X + TARGET_RADIUS)
                        {
                            //If player is within radius -Y-
                            if (players[i].getPlayerLocation().Y >= enemyLocation.Y - TARGET_RADIUS && players[i].getPlayerLocation().Y <= enemyLocation.Y + TARGET_RADIUS)
                            {
                                //player is within radius, player is now secondary target
                                secondaryTargetVector = players[i].getPlayerLocation();

                                //This is now the target 
                                targetIndex = i;

                                targetAquired = true;

                                //Check if any players are closer
                                //If player is closer to enemy than target -X-
                                if ((players[i].getPlayerLocation().X > secondaryTargetVector.X && players[i].getPlayerLocation().X < enemyLocation.X) ||
                                    (players[i].getPlayerLocation().X <= secondaryTargetVector.X && players[i].getPlayerLocation().X > enemyLocation.X))
                                {
                                    //If player is closer to enemy than target -Y-
                                    if ((players[i].getPlayerLocation().Y > secondaryTargetVector.Y && players[i].getPlayerLocation().Y < enemyLocation.Y) ||
                                        (players[i].getPlayerLocation().Y <= secondaryTargetVector.Y && players[i].getPlayerLocation().Y > enemyLocation.Y))
                                    {
                                        //This is now the target
                                        targetIndex = i;

                                        targetAquired = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (!ENEMIES_MERCILESS)
            {
                if ((players[targetIndex].getPlayerLocation().X < enemyLocation.X - TARGET_RADIUS || //X
                    players[targetIndex].getPlayerLocation().X > enemyLocation.X + TARGET_RADIUS) || //X

                    (players[targetIndex].getPlayerLocation().Y < enemyLocation.Y - TARGET_RADIUS || //Y
                    players[targetIndex].getPlayerLocation().Y > enemyLocation.Y + TARGET_RADIUS))   //Y
                {
                    targetAquired = false;
                }
            }
            #endregion

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

            calcAcceleration(gravityList);

            //Updates enemy location based on velocity
            enemyLocation += enemyVelocity;
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

        /// <summary>
        /// Calculate Acceleration
        /// and calculate predictive path
        /// </summary>
        /// <param name="gravityList">Provides a list of all gravity wells near you.</param>
        private void calcAcceleration(List<Gravity> gravityList)
        {
            Vector2 temp = new Vector2();

            for (int i = 0; i < gravityList.Count(); i++)
            {
                temp += gravityList[i].calcGVectorAcceleration(enemyLocation.X, enemyLocation.Y, ENEMY_MASS);
            }

            enemyAcceleration = (enemyThrust * ENEMY_THRUST_SCALE) /*add gravity effect here*/ + temp;

            temp = new Vector2();

            enemyVelocity += enemyAcceleration;

            //playerVelocity.X = MathHelper.Clamp(playerVelocity.X, (-1) * PLAYERMAX, PLAYERMAX);
            //playerVelocity.Y = MathHelper.Clamp(playerVelocity.Y, (-1) * PLAYERMAX, PLAYERMAX);

            //predictive path
            enemyPredictedLocation[0] = enemyLocation;
            enemyPredictedVelocity[0] = enemyVelocity;
            enemyPredictedAcceleration[0] = enemyAcceleration;

            for (int k = 1; k < MAX_PREDICTED_FRAMES; k++)
            {
                Vector2 pTemp = new Vector2();
                //playerPredictedVelocity[k].X = MathHelper.Clamp(playerPredictedVelocity[k].X, (-1) * PLAYERMAX, PLAYERMAX);
                //playerPredictedVelocity[k].Y = MathHelper.Clamp(playerPredictedVelocity[k].Y, (-1) * PLAYERMAX, PLAYERMAX);
                enemyPredictedLocation[k] = enemyPredictedLocation[k - 1] + enemyPredictedVelocity[k];
                for (int i = 0; i < gravityList.Count(); i++)
                {
                    pTemp += gravityList[i].calcGVectorAcceleration(enemyPredictedLocation[k].X, enemyPredictedLocation[k].Y, ENEMY_MASS);
                }

                enemyPredictedAcceleration[k] = pTemp;
                enemyPredictedVelocity[k] = enemyPredictedVelocity[k - 1] + enemyPredictedAcceleration[k];

                pTemp = new Vector2();
            }
        }

        /// <summary>
        /// Draws the enemy location
        /// </summary>
        /// <param name="spriteBatch">Provides the SpriteBatch to allow drawing.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            //Draws a box around target
            if (targetAquired)
            {
                spriteBatch.Draw(enemyPredictionTexture, targetVector, enemyPredictedRectangle, Color.Red * 0.5f);
            }

            spriteBatch.Draw(enemyTexture, enemyLocation, enemyRectangle, Color.White);
        }
    }
}

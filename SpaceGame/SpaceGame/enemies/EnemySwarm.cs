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
    class EnemySwarm
    {
        Rectangle enemyRectangle;
        int WIDTH = 20;
        int HEIGHT = 20;

        Rectangle enemyHitBox;
        const int ENEMY_HITBOX_WIDTH = 20;
        const int ENEMY_HITBOX_HEIGHT = 20;

        const int ENEMY_MAX_HEALTH = 3;
        int enemyHealth;

        Random random;

        //Is enemy idle?
        bool enemyIdle = true;

        //Create a new content manager to load content used just by this level.
        ContentManager content;

        //Player origin(or middle of player)
        Vector2 enemyOrigin;

        //Target Stuff
        const int TARGET_RADIUS = 4000;
        const int TARGET_ATTACH_RADIUS = 50;

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

            enemyHealth = ENEMY_MAX_HEALTH;

            enemyLocation.X = x;
            enemyLocation.Y = y;

            enemyThrust = new Vector2(0, 0);

            enemyAcceleration = new Vector2(0, 0);

            enemyHitBox = new Rectangle((int)enemyLocation.X, (int)enemyLocation.Y, ENEMY_HITBOX_WIDTH, ENEMY_HITBOX_HEIGHT);

            enemyRectangle = new Rectangle(0, 0, WIDTH, HEIGHT);

            enemyOrigin.X = WIDTH / 2;
            enemyOrigin.Y = HEIGHT / 2;

            enemyRotation = 0;

            targetVector = new Vector2(0, 0);
            secondaryTargetVector = new Vector2(0, 0);

            this.LoadContent();
        }

        private void LoadContent()
        {
            enemyTexture = content.Load<Texture2D>("Enemy/Swarmling");
        }
int count = 0;
        //Updates the player every frame
        public void update(List<Gravity> gravityList, Player[] players)
        {
            
            if (enemyIdle)
            {
                for (int i = 0; i < players.Count(); i++)
                {
                    if (players[i].isPlayerReady())//Is the player playing.
                    {
                        //Get distance between player and me
                        calculateDistancesFromPlayers(players, i);
                    }
                }
            }
            else
            {

                #region"Get closest player"
                for (int i = 0; i < players.Count(); i++)
                {
                    if (players[i].isPlayerReady())//Is the player playing.
                    {
                        //Get distance between player and me
                        calculateDistancesFromPlayers(players, i);

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

                    //Update the targets distance
                    targetDistance = playersDistances[targetIndex];
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
                    count++;
                }

                if (targetAttached && count == 10)
                {
                    enemyLocation.X = random.Next((int)players[targetIndex].getPlayerLocation().X - (players[targetIndex].getPlayerRectangle().Width / 2), (int)players[targetIndex].getPlayerLocation().X + (players[targetIndex].getPlayerRectangle().Width / 2)) - (WIDTH / 2);
                    enemyLocation.Y = random.Next((int)players[targetIndex].getPlayerLocation().Y - (players[targetIndex].getPlayerRectangle().Width / 2), (int)players[targetIndex].getPlayerLocation().Y + (players[targetIndex].getPlayerRectangle().Width / 2)) - (HEIGHT / 2);
                   
                    count = 0;
                }
 targetAttached = false;
                #endregion

                #region"Move toward player"
                //If target acquired update the target vector with the targets position
                if (targetAquired)
                {
                    targetVector.X = players[targetIndex].getPlayerLocation().X - (players[targetIndex].getPlayerRectangle().Width / 2);
                    targetVector.Y = players[targetIndex].getPlayerLocation().Y - (players[targetIndex].getPlayerRectangle().Height / 2);

                    //Move toward target
                    if (targetVector.X < enemyLocation.X)
                    {
                        enemyThrust.X = -1;
                        if (enemyAcceleration.X > -4)
                        {
                            enemyThrust.X = -3;
                        }
                    }
                    if (targetVector.X > enemyLocation.X)
                    {
                        enemyThrust.X = 1;
                        if (enemyAcceleration.X < 4)
                        {
                            enemyThrust.X = 3;
                        }
                    }
                    if (targetVector.Y < enemyLocation.Y)
                    {
                        enemyThrust.Y = -1;
                        if (enemyAcceleration.Y < -4)
                        {
                            enemyThrust.Y = -2;
                        }
                    }
                    if (targetVector.Y > enemyLocation.Y)
                    {
                        enemyThrust.Y = 1;
                        if (enemyAcceleration.Y > 4)
                        {
                            enemyThrust.Y = -2;
                        }
                    }
                }
                else
                {
                    //targetVector.X = 0;
                    //targetVector.Y = 0;
                    if (enemyAcceleration.X > 0)
                        enemyThrust.X = -1;
                    if (enemyAcceleration.X < 0)
                        enemyThrust.X = 1;
                    if (enemyAcceleration.Y > 0)
                        enemyThrust.Y = -1;
                    if (enemyAcceleration.Y < 0)
                        enemyThrust.Y = 1;
                }
                #endregion

                if (players[targetIndex].getPlayerBoost() && targetAttached)
                {
                    targetAquired = false;
                    targetAttached = false;

                    enemyVelocity.X = -1 * players[targetIndex].getPlayerVelocityVector().X;

                    enemyVelocity.Y = -1 * players[targetIndex].getPlayerVelocityVector().Y;
                }

                if (!targetAttached)
                {
                    calcAcceleration(gravityList);
                }

                //Updates enemy location based on velocity
                enemyLocation += enemyVelocity;
                if(targetAttached)
                    enemyRotation = (double)Math.Atan2((double)enemyLocation.Y - targetVector.X, (double)enemyLocation.X - targetVector.Y) /*+ (Math.PI / 2)*/;
                else
                    enemyRotation = (double)Math.Atan2((double)enemyVelocity.Y, (double)enemyVelocity.X) + (Math.PI / 2);

                enemyHitBox = new Rectangle((int)enemyLocation.X, (int)enemyLocation.Y, ENEMY_HITBOX_WIDTH, ENEMY_HITBOX_HEIGHT);
            }
        }

        public void calculateDistancesFromPlayers(Player[] players, int i)
        {
            playersDistances[i] = Math.Sqrt(Math.Pow(players[i].getPlayerLocation().X - enemyLocation.X, 2) + Math.Pow(players[i].getPlayerLocation().Y - enemyLocation.Y, 2));
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

        public void hurtEnemy(int damage)
        {
            enemyHealth -= damage;
        }

        public bool getTargetAquired()
        {
            return targetAquired;
        }

        public int getEnemyHealth()
        {
            return enemyHealth;
        }

        public Vector2 getEnemyLocationVector()
        {
            return enemyLocation;
        }

        public bool getAttached()
        {
            return targetAttached;
        }

        public double getDistanceToPlayer(int init_playerIndex)
        {
            return playersDistances[init_playerIndex];
        }

        public Rectangle getEnemyRectangle()
        {
            return enemyRectangle;
        }

        public Rectangle getEnemyHitBox()
        {
            return enemyHitBox;
        }

        public bool getIdle()
        {
            return enemyIdle;
        }

        public void setIdle(bool idle)
        {
            enemyIdle = idle;
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
            spriteBatch.Draw(enemyTexture, enemyLocation, null, Color.LightBlue, (float)enemyRotation, enemyOrigin, 1.0f, SpriteEffects.FlipHorizontally, 0);
        }
    }
}

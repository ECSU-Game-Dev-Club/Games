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
    class EnemySwarm_re
    {
        //Create a new content manager to load content used just by this level.
        ContentManager content;

        //Random Variable
        Random random;

        //Enumerators for different enemy states
        enum EnemySwarmAIState
        {
            //Enemy found a target and is chasing
            chasing,

            //Enemy is out of players range and is no longer updating
            notUpdating,

            //Enemy is within players range, but has no target
            idle
        }

        //Default to idle(can change within this frame)
        EnemySwarmAIState enemyState = EnemySwarmAIState.idle;

        //Enemy Damage
        double damage = 0.5;

        //Boolean hurt
        bool enemyHurt = false;

        //Enemy texture
        Texture2D enemyTexture;

        #region"Enemy Variables (Speed, Location, and Shit)"
        //Enemy location
        Vector2 enemyLocation;

        const float ENEMY_MAX_TURNSPEED = 0.03f;
        Vector2 ENEMY_MAX_SPEED = new Vector2(40);

        //Enemy mass
        const double ENEMY_MASS = 1;

        const float ENEMY_THRUST_SCALE = 0.8f;

        //Player Boost Velocity
        const float ENEMY_BOOST_VELOCITY = 5;

        //Does the enemy continue to attack the target even when the enemy is out of range
        const bool ENEMIES_MERCILESS = true;

        // X is min ----- Y is Max 45deg
        Vector2 ENEMY_MAX_CONE = new Vector2((float)-(Math.PI / 10), (float)Math.PI / 10);

        //Enemy velocity and acceleration for calculating speed
        float enemySpeed;
        Vector2 enemyVelocity;
        Vector2 enemyAcceleration;
        //Enemy Rotation redians
        float enemyRotation;

        //thrust is the location of thumbstick left
        Vector2 enemyThrust;

        //Will the enemy delete itself if the player is too far away
        const bool DELETE_ME_IF_FARAWAY = true;

        //If true delete this enemy
        bool deleteMeBool = false;
        #endregion

        #region"Rectangle Width and Height"
        Rectangle enemyRectangle;
        int WIDTH = 20;
        int HEIGHT = 20;
        #endregion

        #region"Hitbox Variables"
        Rectangle enemyHitBox;
        const int ENEMY_HITBOX_WIDTH = 20;
        const int ENEMY_HITBOX_HEIGHT = 20;
        #endregion

        #region"Health Variables"
        const int ENEMY_MAX_HEALTH = 1;
        int enemyHealth;
        #endregion

        //Enemy origin(or middle of enemy)
        Vector2 enemyOrigin;

        #region"Stop Updating Radius / Vision Radius / Attach Radius Delete Radius"
        //How far the enemy can see the target
        const int TARGET_RADIUS = 4000;

        //How far the target needs to be until the enemy can attach
        const int TARGET_ATTACH_RADIUS = 50;

        //How far the player can be from the enemy to stop updating
        const int STOPUPDATING_RADIUS = 8000;

        //How far the player can be from the enemy to delete the enemy
        const int DELETE_RADIUS = 10000;
        #endregion

        //Array of player distances from this enemy
        double[] playersDistances = new double[4];

        #region"Target Variables"
        //The index in the player array of the target
        //This is the index the enemy is targeting
        int targetIndex;

        //The distance of the target from the enemy
        double targetDistance;

        //The location vector of the target
        Vector2 targetVector;

        //Boolean if the enemy is attached to a player
        bool targetAttached = false;

        //Counts frames for attaching/animation
        int frameCount = 0;
        #endregion

        //Gravity IDK what this does.
        Vector2 temp;

        //Constructor for player, starts/initializes everything
        public EnemySwarm_re(float x, float y, IServiceProvider serviceProvider)
        {
            content = new ContentManager(serviceProvider, "Content");

            random = new Random();

            temp = new Vector2();

            enemyHealth = ENEMY_MAX_HEALTH;

            enemyLocation.X = x;
            enemyLocation.Y = y;

            enemyThrust = new Vector2(0, 0);

            enemyAcceleration = new Vector2(0, 0);

            enemySpeed = 0;

            enemyHitBox = new Rectangle((int)enemyLocation.X, (int)enemyLocation.Y, ENEMY_HITBOX_WIDTH, ENEMY_HITBOX_HEIGHT);

            enemyRectangle = new Rectangle(0, 0, WIDTH, HEIGHT);

            enemyOrigin.X = WIDTH / 2;
            enemyOrigin.Y = HEIGHT / 2;

            enemyRotation = 0;

            targetVector = new Vector2(0, 0);

            this.LoadContent();
        }

        private void LoadContent()
        {
            enemyTexture = content.Load<Texture2D>("Enemy/Swarmling");
        }

        //Updates the player every frame
        public void update(List<Gravity> gravityList, Player[] players)
        {
            enemyHurt = false;

            #region"Not Updating"
            if (enemyState == EnemySwarmAIState.notUpdating)
            {
                //Calculate the distance from player 1
                calculateDistancesFromPlayers(players, 0); //0 = player1

                //If he is within range(the display adapter)
                if (playersDistances[0] < GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width)
                {
                    enemyState = EnemySwarmAIState.idle;
                }

                //If 'DELETE_ME_IF_FARAWAY' is true and the player is close, DELETE ME
                if(DELETE_ME_IF_FARAWAY && playersDistances[0] >= DELETE_RADIUS)
                {
                    deleteMeBool = true;
                }
            }
            #endregion
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
                            //If not chasing
                            if (enemyState == EnemySwarmAIState.idle)
                            {
                                //Target found, chasing
                                enemyState = EnemySwarmAIState.chasing;

                                //Keep track of what player I am targeting
                                targetIndex = i;

                                //Keep track of distance between the target and me
                                targetDistance = playersDistances[i];
                            }
                            //If the enemy is currently chasing something
                            if(enemyState == EnemySwarmAIState.chasing)
                            {
                                //Check to see if i has a shorter distance
                                if(targetDistance > playersDistances[i])
                                {
                                    //Keep track of what player I am targeting
                                    targetIndex = i;

                                    //Keep track of distance between the target and me
                                    targetDistance = playersDistances[i];
                                }
                            }
                        }
                    }

                    //Update the targets distance
                    targetDistance = playersDistances[targetIndex];
                }

                //Keeps targets vector
                targetVector = players[targetIndex].getPlayerLocation();

                if (!ENEMIES_MERCILESS)
                {
                    if (targetDistance > TARGET_RADIUS)
                    {
                        enemyState = EnemySwarmAIState.idle;
                    }
                }

                if(playersDistances[0] > STOPUPDATING_RADIUS) //Distance from player1
                {
                    enemyState = EnemySwarmAIState.notUpdating;
                }
                #endregion

                #region"Check to see if I should not update next frame"

                if (playersDistances[0] > GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width * 2)
                {
                    enemyState = EnemySwarmAIState.notUpdating;
                }

                #endregion

                #region"Attach to target if near"

                //If player is within radius -X-
                if (playersDistances[targetIndex] < TARGET_ATTACH_RADIUS)
                {
                    

                    targetAttached = true;
                    enemyThrust.X = 0;
                    enemyThrust.Y = 0;
                    //enemyVelocity.X = 0;
                    //enemyVelocity.Y = 0;
                    enemyAcceleration.X = 0;
                    enemyAcceleration.X = 0;
                    enemySpeed = 0;
                    frameCount++;
                }

                if (targetAttached && frameCount == 10)
                {
                    players[targetIndex].hurtPlayer(damage);

                    enemyLocation.X = random.Next((int)players[targetIndex].getPlayerLocation().X - (players[targetIndex].getPlayerRectangle().Width / 2), (int)players[targetIndex].getPlayerLocation().X + (players[targetIndex].getPlayerRectangle().Width / 2)) - (WIDTH / 2);
                    enemyLocation.Y = random.Next((int)players[targetIndex].getPlayerLocation().Y - (players[targetIndex].getPlayerRectangle().Width / 2), (int)players[targetIndex].getPlayerLocation().Y + (players[targetIndex].getPlayerRectangle().Width / 2)) - (HEIGHT / 2);

                    frameCount = 0;
                }
                targetAttached = false;
                #endregion

                #region"Move toward target"
                //If target acquired update the target vector with the targets position
                if (enemyState == EnemySwarmAIState.chasing)
                {
                    
                    enemyRotation = Helper.TurnToFace_Radians(enemyLocation, targetVector + players[targetIndex].getPlayerVelocityVector(), enemyRotation, ENEMY_MAX_TURNSPEED);
                    enemyThrust = Helper.TurnToFace_Vector2(enemyLocation, targetVector + players[targetIndex].getPlayerVelocityVector(), enemyRotation, ENEMY_MAX_TURNSPEED);

                    float angularDifference = Helper.getAngularDifferenceBetweenObjects(enemyLocation, targetVector + players[targetIndex].getPlayerVelocityVector(), enemyRotation);

                    if(!(angularDifference > ENEMY_MAX_CONE.X && angularDifference < ENEMY_MAX_CONE.Y))
                    {
                        enemyVelocity = enemyVelocity * 0.75f;
                    }
                }
                else
                {
                    //Remain stationary(unless gravity)
                    enemyThrust.X = 0;
                    enemyThrust.Y = 0;
                }
                #endregion


                if (!targetAttached)
                {
                    calcAcceleration(gravityList);
                }

                //Clamp velocity right before we add it to location.
                enemyVelocity = Vector2.Clamp(enemyVelocity, -ENEMY_MAX_SPEED, ENEMY_MAX_SPEED);

                //Add Velocity to Location
                enemyLocation += enemyVelocity;

                //Updates hitbox to the enemies location
                enemyHitBox.X = (int)enemyLocation.X;
                enemyHitBox.Y = (int)enemyLocation.Y;
            }
        }

        public void calculateDistancesFromPlayers(Player[] players, int i)
        {
            playersDistances[i] = Vector2.Distance(players[i].getPlayerLocation(), enemyLocation);
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
            enemyHurt = true;

            enemyHealth -= damage;
        }


        public bool deleteMe()
        {
            return deleteMeBool;
        }

        public bool getTargetAquired()
        {
            if(enemyState == EnemySwarmAIState.chasing)
            {
                return true;
            }
            else 
            {
                return false;
            }
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
            if (enemyState == EnemySwarmAIState.idle)
                return true;
            else
                return false;
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
            spriteBatch.Draw(enemyTexture, enemyLocation, null, Color.White, enemyRotation + (float)(Math.PI / 2), enemyOrigin, 1.0f, SpriteEffects.None, 0);
        }
    }
}

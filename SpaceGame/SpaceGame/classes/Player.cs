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
    class Player
    {
        //Preferences
        bool staticTurrent = true;

        Rectangle playerRectangle;
        Rectangle playerPredictedRectangle;
        int width = 60;
        int height = 60;

        //what player you are (0 = player one)
        int playerIndex;

        //Create a new content manager to load content used just by this level.
        ContentManager content;

        //Player origin(or middle of player texture)
        Vector2 playerOrigin;

        //Players texture
        Texture2D playerTexture;
        Texture2D playerPredictionTexture;

        Color playerColor;

        //Player Did Not Press Start(Not Player1)
        bool playerReady = false;

        //Players location
        Vector2 playerLocation;

        #region"Weapons"
        //Current weapon(1=gatling 2=missile 3=??? 4=???)
        int currentWeapon = 1;

        #region"Gatling"
        GatlingWeapon gatlingWeapon;
        Texture2D playerGatlingTurretTexture;
        #endregion

        #region"Missile"
        const int MAX_LASER_DISTANCE = 600;

        Rectangle playerTargetingLaserRectangle;

        MissileWeapon missileWeapon;

        Texture2D playerMissileTurretTexture;
        #endregion

        #endregion

        #region"Movement Variables"

        //Players velocity and acceleration for calculating speed
        Vector2 playerVelocity;
        Vector2 playerAcceleration;

        //Max Velocity
        Vector2 PLAYER_MAX_VELOCITY = new Vector2(40);

        //Player Rotation redians
        float playerRotation;
        float playerRotationDifference;
        float playerDesiredRotation;
        const float MAX_PLAYER_ROT_SPEED = 0.1f;

        float playerAimRotation;
        float previousPlayerAimRotation;

        //thrust is the triggers on the gamepad
        float playerThrust;
        const float PLAYER_THRUST_SCALE = .1f;
        bool playerMoving = false;

        //The maximum velocity in which direction assist will assist
        const int PLAYER_DIRECTIONAL_ASSIT = 3; //Default 3

        //Player Boost Velocity
        const float PLAYER_BOOST_VELOCITY = 5;
        bool playerBoosted = false;

        //Player mass
        const double PLAYER_MASS = 1;
        #endregion

        #region"Prediction/Previous Variables"

        //Toggle prediction drawing/updating
        bool calcLocationPrediction = false;

        //Prediction
        Vector2 pTemp = new Vector2();
        const int MAX_PREDICTED_FRAMES = 1000;
        Vector2[] playerPredictedLocation = new Vector2[MAX_PREDICTED_FRAMES];
        Vector2[] playerPredictedVelocity = new Vector2[MAX_PREDICTED_FRAMES];
        Vector2[] playerPredictedAcceleration = new Vector2[MAX_PREDICTED_FRAMES];

        //Previous
        //Players vector2 location delta
        Vector2 playerLocationDelta;

        const int MAX_PREVIOUS_FRAMES = 500;
        int currentFrame = 0;
        Vector2[] playerPreviousLocation = new Vector2[MAX_PREVIOUS_FRAMES];
        #endregion

        #region"Animation Objects/Times"

        #region"Thrust/Moving"
        //Texture
        Texture2D animation_ThrustTexture;

        //View for the texture
        Rectangle animation_ThrustRectangle;
        int animation_thrust_WidthHeight = 60;
        int animation_thrust_frame = 0;

        //Time to add to current gameTime
        int animation_thrust_frameTime = 0;
        //startup frame time
        int animation_thrust_startupTime = 20;//Milliseconds
        //loop fram time
        int animation_thrust_loopTime = 80;//Milliseconds

        //thrust start
        bool animation_thrust_starting = true;
        #endregion


        #endregion

        //Constructor for player, starts/initializes everything, sets spawn location
        public Player(float x, float y, IServiceProvider serviceProvider, int init_playerIndex)
        {
            playerIndex = init_playerIndex; //0 = first player, 1 = seconds player, etc...

            content = new ContentManager(serviceProvider, "Content");

            playerTargetingLaserRectangle = new Rectangle((int)playerLocation.X, (int)playerLocation.Y, MAX_LASER_DISTANCE, 10);

            playerLocation.X = x;
            playerLocation.Y = y;

            #region"Set Player Colors"
            if (playerIndex == 0)
            {
                playerColor = Color.White;
            }
            else if (playerIndex == 1)
            {
                playerColor = Color.Yellow;
            }
            else if (playerIndex == 2)
            {
                playerColor = Color.Fuchsia;
            }
            else if (playerIndex == 3)
            {
                playerColor = Color.DeepSkyBlue;
            }
            #endregion

            playerThrust = 0;

            playerAcceleration = new Vector2(0, 0);

            playerRectangle = new Rectangle(0, 0, width, height);
            playerPredictedRectangle = new Rectangle(0, 0, 2, 2);

            playerOrigin.X = width / 2;
            playerOrigin.Y = height / 2;

            playerRotation = 0;
            playerDesiredRotation = 0;
            playerAimRotation = 0;

            //Weapons:
            gatlingWeapon = new GatlingWeapon();
            missileWeapon = new MissileWeapon();

            //Animation Objects:
            animation_ThrustRectangle = new Rectangle(0, 0, animation_thrust_WidthHeight, animation_thrust_WidthHeight);

            this.LoadContent();
        }

        /// <summary>
        /// Load all content via
        /// content pipeline
        /// </summary>
        private void LoadContent()
        {
            if (playerIndex == 0)
            {
                playerTexture = content.Load<Texture2D>("player/player_ship");
            }
            else
            {
                playerTexture = content.Load<Texture2D>("player/player_ship_mk2");
            }

            playerGatlingTurretTexture = content.Load<Texture2D>("weapons/gatling_turret");

            playerMissileTurretTexture = content.Load<Texture2D>("weapons/missile_turret");

            playerPredictionTexture = content.Load<Texture2D>("whiteTexture");

            animation_ThrustTexture = content.Load<Texture2D>("player/anims/player_ship_thrust");
        }

        /// <summary>
        /// Player1 update method.
        /// </summary>
        /// <param name="gamepad">Provides gamepad state.</param>
        /// <param name="gamepad_OLDSTATE">Provides previous frame gamepad state.</param>
        /// <param name="keyboard">Provides keyboard state.</param>
        /// <param name="keyboard_OLDSTATE">Provides previous frame keyboard state.</param>
        /// <param name="gravityList">Provides the total number of gravity wells near you.</param>
        public void update(GamePadState gamepad, GamePadState gamepad_OLDSTATE, KeyboardState keyboard, KeyboardState keyboard_OLDSTATE, List<Gravity> gravityList, GameTime gameTime)
        {
            currentFrame++;

            //Player is playing
            playerReady = true;
            //Player didnt boost
            playerBoosted = false;
            //Player isnt moving.
            playerMoving = false;

            //Figure out if user wants player to move(movement logic)
            playerControls(gamepad, gamepad_OLDSTATE, keyboard, keyboard_OLDSTATE, gameTime);

            if (gamepad.ThumbSticks.Left.X != 0 && gamepad.ThumbSticks.Left.Y != 0)
            {
                //calculates player desired rotation based on gamepad.
                playerDesiredRotation = (float)Math.Atan2(gamepad.ThumbSticks.Left.X, gamepad.ThumbSticks.Left.Y);
                playerRotationDifference = Helper.WrapAngle(playerDesiredRotation - playerRotation);
                playerRotationDifference = MathHelper.Clamp(playerRotationDifference, -MAX_PLAYER_ROT_SPEED, MAX_PLAYER_ROT_SPEED);
                playerRotation = Helper.WrapAngle(playerRotationDifference + playerRotation);
            }

            //Clamp players velocity before adding to location
            playerVelocity = Vector2.Clamp(playerVelocity , -(PLAYER_MAX_VELOCITY), PLAYER_MAX_VELOCITY);

            //Calculate acceleration
            calcAcceleration(gravityList);

            calculatePreviousLocation();

            //Animation:
            animation_thrust(gameTime);

            //Updates player location based on velocity
            playerLocation += playerVelocity; //ALWAYS ON BOTTOM

            if (currentFrame == MAX_PREVIOUS_FRAMES)
            {
                currentFrame = 0;
            }
        }
        #region"update overload method for not player1"
        /// <summary>
        /// Overload of update method for players that
        /// are not player1
        /// </summary>
        /// <param name="gamepad">Provides gamepad state.(Buttons</param>
        /// <param name="gamepad_OLDSTATE">Provides previous frame gamepad state.(Buttons</param>
        /// <param name="gravityList">Provides the total number of gravity wells near you.</param>
        public void update(GamePadState gamepad, GamePadState gamepad_OLDSTATE, List<Gravity> gravityList, GameTime gameTime)
        {
            currentFrame++;

            //Player is playing
            playerReady = true;
            //Player didnt boost
            playerBoosted = false;
            //Player isnt moving.
            playerMoving = false;

            //Figure out if user wants player to move(movement logic)
            playerControls(gamepad, gamepad_OLDSTATE, gameTime);

            if (gamepad.ThumbSticks.Left.X != 0 && gamepad.ThumbSticks.Left.Y != 0)
            {
                //calculates player desired rotation based on gamepad.
                playerDesiredRotation = (float)Math.Atan2(gamepad.ThumbSticks.Left.X, gamepad.ThumbSticks.Left.Y);
                playerRotationDifference = Helper.WrapAngle(playerDesiredRotation - playerRotation);
                playerRotationDifference = MathHelper.Clamp(playerRotationDifference, -MAX_PLAYER_ROT_SPEED, MAX_PLAYER_ROT_SPEED);
                playerRotation = Helper.WrapAngle(playerRotationDifference + playerRotation);
            }

            //Clamp players velocity before adding to location
            playerVelocity = Vector2.Clamp(playerVelocity, -(PLAYER_MAX_VELOCITY), PLAYER_MAX_VELOCITY);

            //Calculate acceleration
            calcAcceleration(gravityList);

            calculatePreviousLocation();

            //Updates player location based on velocity
            playerLocation += playerVelocity;

            if (currentFrame == MAX_PREVIOUS_FRAMES)
            {
                currentFrame = 0;
            }
        }
#endregion
        #region"Dynamic Spawn"
        /// <summary>
        /// Overload of update method for players that
        /// are not player1
        /// </summary>
        /// <param name="dynamicSpawn">Provides player.</param>
        public void updateDynamicSpawn(Player player1)
        {
            playerPreviousLocation = new Vector2[MAX_PREVIOUS_FRAMES];

            playerLocation = player1.getPlayerLocation();

            playerVelocity.X = player1.getPlayerVelocityVector().X;
            playerVelocity.Y = player1.getPlayerVelocityVector().Y;
            playerAcceleration.X = player1.getPlayerAccelerationVector().X;
            playerAcceleration.Y = player1.getPlayerAccelerationVector().Y;
        }
        #endregion

        /// <summary>
        /// Controls the players movement
        /// </summary>
        /// <param name="gamepad">Provides gamepad state.</param>
        /// <param name="gamepad_OLDSTATE">Provides previous frame gamepad state.</param>
        /// /// <param name="keyboard">Provides keyboard state.</param>
        /// /// <param name="keyboard_OLDSTATE">Provides previous frame keyboard state.</param>
        private void playerControls(GamePadState gamePad, GamePadState gamePad_OLDSTATE, KeyboardState keyboard, KeyboardState keyboard_OLDSTATE, GameTime gameTime)
        {
            #region"GamePad Movement Logic"

            //If the left thumbstick is pressed in
            //Toggle Prediction
            if (gamePad.Buttons.LeftStick != ButtonState.Pressed && gamePad_OLDSTATE.Buttons.LeftStick == ButtonState.Pressed)
            {
                calcLocationPrediction = !calcLocationPrediction;
            }

            //If the user moves the left thumbstick(in any direction)
            //Rotate Player
            if (gamePad.Triggers.Left >= 0.1)
            {
                //Pass the left trigger to thrust
                setThrust(gamePad.Triggers.Left);
                //GamePad.SetVibration(PlayerIndex.One, Math.Sqrt(Math.Pow(gamePad.ThumbSticks.Left.X,2) + Math.Pow(gamePad.ThumbSticks.Left.Y,2)), 0);
            }
            else
            {
                
                GamePad.SetVibration(PlayerIndex.One, 0, 0);
            }

            #region"Boost"
            //left shoulder or spacebar was clicked(pressed, then released)
            //Boost player
            if ((gamePad.Buttons.LeftShoulder != ButtonState.Pressed && gamePad_OLDSTATE.Buttons.LeftShoulder == ButtonState.Pressed))
            {
                boostDirection(gamePad.ThumbSticks.Left.X, gamePad.ThumbSticks.Left.Y);
            }
            #endregion

            #region"Turret Rotation"
            if (Math.Abs(gamePad.ThumbSticks.Right.X) + Math.Abs(gamePad.ThumbSticks.Right.Y) > 0.1)
            {
                playerAimRotation = (float)Math.Atan2(gamePad.ThumbSticks.Right.X * (-1), gamePad.ThumbSticks.Right.Y * (-1));
            }
            else
            {
                
                if (staticTurrent)
                {
                    playerAimRotation = playerRotation + (float)Math.PI;
                    previousPlayerAimRotation = playerAimRotation;
                }
                else
                {
                    //Make the turret have its previous angle relative to the ship
                }
            }
            #endregion

            #region"D-Pad"
            //Gatling
            if (gamePad.DPad.Up != ButtonState.Pressed && gamePad_OLDSTATE.DPad.Up == ButtonState.Pressed)
            {
                currentWeapon = 1;
            }
            //???
            if (gamePad.DPad.Left != ButtonState.Pressed && gamePad_OLDSTATE.DPad.Left == ButtonState.Pressed)
            {
                currentWeapon = 2;
            }
            //???
            if (gamePad.DPad.Down != ButtonState.Pressed && gamePad_OLDSTATE.DPad.Down == ButtonState.Pressed)
            {
                currentWeapon = 3;
            }
            //???
            if (gamePad.DPad.Right != ButtonState.Pressed && gamePad_OLDSTATE.DPad.Right == ButtonState.Pressed)
            {
                currentWeapon = 4;
            }
            #endregion

            #region"Weapons"
            if (gamePad.Triggers.Right > 0.2)
            {
                //GATLING
                if (currentWeapon == 1)
                {
                    gatlingWeapon.shoot(gameTime, this, gamePad);
                }
                if (currentWeapon == 2)
                {
                    //???
                }
                if (currentWeapon == 3)
                {
                    //???
                }
                if (currentWeapon == 4)
                {
                    //???
                }
            }
            else
            {
                gatlingWeapon.setShooting(false);
            }
            #endregion

            #endregion
        }
        #region"playerControls Overload"
        /// <summary>
        /// Controls the players movement
        /// </summary>
        /// <param name="gamepad">Provides gamepad state.</param>
        /// <param name="gamepad_OLDSTATE">Provides previous frame gamepad state.</param>
        /// /// <param name="keyboard">Provides keyboard state.</param>
        /// /// <param name="keyboard_OLDSTATE">Provides previous frame keyboard state.</param>
        private void playerControls(GamePadState gamePad, GamePadState gamePad_OLDSTATE, GameTime gameTime)
        {
            //If the left thumbstick is pressed in or Q on keyboard
            //Toggle Prediction
            if ((gamePad.Buttons.LeftStick != ButtonState.Pressed && gamePad_OLDSTATE.Buttons.LeftStick == ButtonState.Pressed))
            {
                calcLocationPrediction = !calcLocationPrediction;
            }

            //If the user moves the left thumbstick(in any direction)
            //Rotate Player
            if (gamePad.Triggers.Left >= 0.1)
            {
                //Pass the left trigger to thrust
                setThrust(gamePad.Triggers.Left);
                //GamePad.SetVibration(PlayerIndex.One, Math.Sqrt(Math.Pow(gamePad.ThumbSticks.Left.X,2) + Math.Pow(gamePad.ThumbSticks.Left.Y,2)), 0);
            }
            else
            {
            }

            #region"Boost"
            //left shoulder or spacebar was clicked(pressed, then released)
            if ((gamePad.Buttons.LeftShoulder != ButtonState.Pressed && gamePad_OLDSTATE.Buttons.LeftShoulder == ButtonState.Pressed))
            {
                boostDirection(gamePad.ThumbSticks.Left.X, gamePad.ThumbSticks.Left.Y);
            }
            #endregion

            #region"Turret Rotation"
            //Set rotation of turret
            if (Math.Abs(gamePad.ThumbSticks.Right.X) + Math.Abs(gamePad.ThumbSticks.Right.Y) > 0.1)
            {
                playerAimRotation = (float)Math.Atan2(gamePad.ThumbSticks.Right.X * (-1), gamePad.ThumbSticks.Right.Y * (-1));
            }
            else
            {

                if (staticTurrent)
                {
                    playerAimRotation = playerRotation + (float)Math.PI;
                    previousPlayerAimRotation = playerAimRotation;
                }
                else
                {
                    //Make the turret have its previous angle relative to the ship
                }
            }
            #endregion

            #region"D-Pad"
            //Gatling
            if (gamePad.DPad.Up != ButtonState.Pressed && gamePad_OLDSTATE.DPad.Up == ButtonState.Pressed)
            {
                currentWeapon = 1;
            }
            //???
            if (gamePad.DPad.Left != ButtonState.Pressed && gamePad_OLDSTATE.DPad.Left == ButtonState.Pressed)
            {
                currentWeapon = 2;
            }
            //???
            if (gamePad.DPad.Down != ButtonState.Pressed && gamePad_OLDSTATE.DPad.Down == ButtonState.Pressed)
            {
                currentWeapon = 3;
            }
            //???
            if (gamePad.DPad.Right != ButtonState.Pressed && gamePad_OLDSTATE.DPad.Right == ButtonState.Pressed)
            {
                currentWeapon = 4;
            }
            #endregion

            #region"Weapons"
            if (gamePad.Triggers.Right > 0.2)
            {
                //GATTLING
                if (currentWeapon == 1)
                {
                    gatlingWeapon.shoot(gameTime, this, gamePad);
                }
                if (currentWeapon == 2)
                {
                    //???
                }
                if (currentWeapon == 3)
                {
                    //???
                }
                if (currentWeapon == 4)
                {
                    //???
                }
            }
            else
            {
                gatlingWeapon.setShooting(false);
            }
            #endregion
        }
        #endregion


        /// <summary>
        /// Which direction to thrust in
        /// </summary>
        /// <param name="initThrust">Provides a Vector2 X(0-1) and Y(0-1). This vector comes from LEFT THUMBSTICK</param>
        public void setThrust(float initThrust)
        {
            playerMoving = true;
            playerThrust = initThrust;
        }

        public void animation_thrust(GameTime gameTime)
        {
            if (playerMoving)
            {
                //Startup Flame
                if(animation_thrust_starting)
                {
                    if(animation_thrust_frame <= 8)
                    {
                        if(gameTime.TotalGameTime.TotalMilliseconds >= animation_thrust_frameTime)
                        {
                            animation_thrust_frameTime = (int)gameTime.TotalGameTime.TotalMilliseconds + animation_thrust_startupTime;
                            animation_thrust_frame++;
                        }
                    }
                    else
                    {
                        animation_thrust_starting = false;
                    }
                }
                //Loop
                else
                {
                    if (gameTime.TotalGameTime.TotalMilliseconds >= animation_thrust_frameTime)
                    {
                        animation_thrust_frameTime = (int)gameTime.TotalGameTime.TotalMilliseconds + animation_thrust_loopTime;

                        if (animation_thrust_frame >= 20)
                        {
                            animation_thrust_frame = 9;
                        }
                        else
                        {
                            animation_thrust_frame++;
                        }
                    }
                }
            }
            else
            {
                animation_thrust_frame = 0;
                animation_thrust_starting = true;
            }

            //Sets the frame to the correct view location
            animation_ThrustRectangle = new Rectangle(
                (animation_thrust_frame % 12) * animation_thrust_WidthHeight, //X 
                (animation_thrust_frame / 12) * animation_thrust_WidthHeight, //Y
                animation_thrust_WidthHeight,                                 //Width
                animation_thrust_WidthHeight);                                //Height
        }

        /// <summary>
        /// Thrust in a direction
        ///  X(0-1) Y(0-1)
        /// </summary>
        /// <param name="X">Provides a float X(0-1)</param>
        /// <param name="Y">Provides a float Y(0-1)</param>
        private void boostDirection(float x, float y)
        {
            playerBoosted = true;

            playerVelocity.X += PLAYER_BOOST_VELOCITY * x;
            playerVelocity.Y += PLAYER_BOOST_VELOCITY * (-1 * y);
        }

        private void calculatePreviousLocation()
        {
            playerPreviousLocation[currentFrame - 1] = playerLocation;

            if (currentFrame != 1)
            {
                playerLocationDelta.X = playerPreviousLocation[currentFrame - 1].X - playerPreviousLocation[currentFrame - 2].X;
                playerLocationDelta.Y = playerPreviousLocation[currentFrame - 1].Y - playerPreviousLocation[currentFrame - 2].Y;
            }
        }

        private void calculatePrediction(List<Gravity> gravityList)
        {
           
            //predictive path
            playerPredictedLocation[0] = playerLocation;
            playerPredictedVelocity[0] = playerVelocity;
            playerPredictedAcceleration[0] = playerAcceleration;

            if (calcLocationPrediction)
            {
                for (int k = 1; k < MAX_PREDICTED_FRAMES; k++)
                {
                    playerPredictedLocation[k] = playerPredictedLocation[k - 1] + playerPredictedVelocity[k];
                    for (int i = 0; i < gravityList.Count(); i++)
                    {
                        pTemp += gravityList[i].calcGVectorAcceleration(playerPredictedLocation[k].X, playerPredictedLocation[k].Y, PLAYER_MASS);
                    }

                    playerPredictedAcceleration[k] = pTemp;
                    playerPredictedVelocity[k] = playerPredictedVelocity[k - 1] + playerPredictedAcceleration[k];

                    pTemp = new Vector2();
                }
            }
        }

        public bool isPlayerShooting()
        {
            if(currentWeapon == 1)
            {
                return gatlingWeapon.getShooting();
            }
            /*
            else if (currentWeapon == 2)
            {
                //return missileWeapon.getShooting();
            }
            else if (currentWeapon == 3)
            {
                //return ???.getShooting();
            }
            else if (currentWeapon == 4)
            {
                //return ???.getShooting();
            }
            */
            else
            {
                return false;
            }
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
                temp += gravityList[i].calcGVectorAcceleration(playerLocation.X, playerLocation.Y, PLAYER_MASS);
            }

            playerAcceleration.X = ((playerThrust * PLAYER_THRUST_SCALE) * (float)Math.Sin(playerRotation)) + temp.X;
            playerAcceleration.Y = -1 * ((playerThrust * PLAYER_THRUST_SCALE) * (float)Math.Cos(playerRotation)) + temp.Y;

            #region"Directional Assistance"
            //VELX NEG, assist to go right(going left)
            if (playerVelocity.X < -PLAYER_DIRECTIONAL_ASSIT && (playerRotation > 0 && playerRotation <= Math.PI))
            {
                playerAcceleration.X = (playerThrust * (PLAYER_THRUST_SCALE * (Math.Abs(playerVelocity.X) / 2)) * (float)Math.Sin(playerRotation)) + temp.X;
            }
            //VELX POS, assist to go left(going right)
            if (playerVelocity.X > PLAYER_DIRECTIONAL_ASSIT && (playerRotation < 0 && playerRotation >= -Math.PI))
            {
                playerAcceleration.X = (playerThrust * (PLAYER_THRUST_SCALE * (Math.Abs(playerVelocity.X) / 2)) * (float)Math.Sin(playerRotation)) + temp.X;
            }

            //VELY POS, assist to go up(going down)
            if (playerVelocity.Y > PLAYER_DIRECTIONAL_ASSIT && (Math.Abs(playerRotation) >= 0 && Math.Abs(playerRotation) <= Math.PI / 2))
            {
                playerAcceleration.Y = (playerThrust * (-1 * PLAYER_THRUST_SCALE * (Math.Abs(playerVelocity.Y) / 2)) * (float)Math.Cos(playerRotation)) + temp.Y;
            }
            //VELY NEG, assist to go down(going up)
            if (playerVelocity.Y < -PLAYER_DIRECTIONAL_ASSIT && (Math.Abs(playerRotation) >= Math.PI / 2 && Math.Abs(playerRotation) <= Math.PI))
            {
                playerAcceleration.Y = (playerThrust * (-1 * PLAYER_THRUST_SCALE * (Math.Abs(playerVelocity.Y) / 2)) * (float)Math.Cos(playerRotation)) + temp.Y;
            }
            #endregion

            temp = new Vector2();

            playerVelocity += playerAcceleration;

            this.calculatePrediction(gravityList);

        }

        public void setIsPlayerReady(bool initPlayerReady)
        {
            playerReady = initPlayerReady;
        }

        public bool isPlayerReady()
        {
            return playerReady;
        }

        public GatlingWeapon getGatlingWeapon()
        {
            return gatlingWeapon;
        }

        public float getPlayerThrust()
        {
            return playerThrust;
        }

        public int getCurrentWeapon()
        {
            return currentWeapon;
        }

        public float getAimRotation()
        {
            return playerAimRotation;
        }

        public float getRotation()
        {
            return playerRotation;
        }

        public bool getPlayerBoost()
        {
            return playerBoosted;
        }

        public Rectangle getPlayerRectangle()
        {
            return playerRectangle;
        }
        public Rectangle getPlayerPredictedRectangle()
        {
            return playerPredictedRectangle;
        }

        public Vector2 getPlayerVelocityVector()
        {
            return playerVelocity;
        }

        public Vector2 getPlayerLocationDelta()
        {
            return playerLocationDelta;
        }

        public Vector2 getPlayerAccelerationVector()
        {
            return playerAcceleration;
        }

        public Vector2 getPlayerLocation()
        {
            return playerLocation;
        }
        //predictive
        public Vector2 getPlayerPredictedLocation(int k)
        {
            return playerPredictedLocation[k];
        }

        /// <summary>
        /// Draws the player location
        /// and draws player predicted locations
        /// </summary>
        /// <param name="spriteBatch">Provides the SpriteBatch to allow drawing.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < MAX_PREVIOUS_FRAMES; i++)
            {
                spriteBatch.Draw(playerPredictionTexture, playerPreviousLocation[i], playerPredictedRectangle, Color.Purple * 1f);
            }

            if (calcLocationPrediction)
            {
                for (int i = 0; i < MAX_PREDICTED_FRAMES; i++)
                {
                    spriteBatch.Draw(playerPredictionTexture, playerPredictedLocation[i], playerPredictedRectangle, Color.Green * 1f);
                }
            }

            //Thrust Animation
            if (playerMoving)
            {
                spriteBatch.Draw(animation_ThrustTexture, playerLocation, animation_ThrustRectangle, playerColor, playerRotation, playerOrigin, 1.0f, SpriteEffects.None, 0);
            }

            spriteBatch.Draw(playerTexture, playerLocation, null, playerColor, playerRotation, playerOrigin, 1.0f, SpriteEffects.None, 0);

            if (currentWeapon == 1) //Gatling Cannon
            {
                spriteBatch.Draw(playerGatlingTurretTexture, playerLocation, null, playerColor, playerAimRotation, playerOrigin, 1.0f, SpriteEffects.None, 0);
            }
            if (currentWeapon == 2) //Missile
            {
                spriteBatch.Draw(playerMissileTurretTexture, playerLocation, null, playerColor, playerAimRotation + (float)Math.PI, playerOrigin, 1.0f, SpriteEffects.None, 0);
            }
            if (currentWeapon == 3) //???
            {
            }
            if (currentWeapon == 4) //???
            {
            }

            //THIS IS THE THRUST LAYER
            //spriteBatch.Draw(playerThrustTexture, playerLocation, null, Color.White * (Math.Abs(playerThrust.X) + Math.Abs(playerThrust.Y)), playerRotation, playerOrigin, 1.0f, SpriteEffects.None, 0);
        }
    }
}

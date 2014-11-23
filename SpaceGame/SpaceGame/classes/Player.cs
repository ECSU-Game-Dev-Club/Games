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
    class Player
    {
        Rectangle playerRectangle;
        Rectangle playerPredictedRectangle;
        int width = 30;
        int height = 30;

        //what player you are (0 = player one)
        int playerIndex;

        //Create a new content manager to load content used just by this level.
        ContentManager content;

        //Player origin(or middle of player texture)
        Vector2 playerOrigin;

        //Players texture
        Texture2D playerTexture;
        Texture2D playerThrustTexture;
        Texture2D playerPredictionTexture;
        Color playerColor;

        //Inverted Y or X?
        bool invertedY = true;
        bool invertedX = false;

        //Player Did Not Press Start(Not Player1)
        bool playerReady = false;

        //Keyboard vector(used to help smooth movement on keyboard)
        Vector2 keyboardVector;

        //Players location
        Vector2 playerLocation;

        //Players vector2 location delta
        Vector2 playerLocationDelta;

        #region"Weapons"
        //Is player shooting
        bool playerShooting = false;

        //GameTime Stamp
        int gameTimeStampMillisecond;
        int gameTimeStampSecond;
        const int GATTLING_FREQUENCY = 75;//in milliseconds

        //Current weapon(1=gattling 2=??? 3=??? 4=???)
        int currentWeapon = 1;
        #endregion

        #region"Movement Variables"

        //Players velocity and acceleration for calculating speed
        Vector2 playerVelocity;
        Vector2 playerAcceleration;
        //Player Rotation redians
        float playerRotation;
        float playerAimRotation;

        //thrust is the triggers on the gamepad
        Vector2 playerThrust;
        const float PLAYER_THRUST_SCALE = .1f;

        //Player Boost Velocity
        const float PLAYER_BOOST_VELOCITY = 5;
        bool playerBoosted = false;

        //Player mass
        const double PLAYER_MASS = 1;
        #endregion

        #region"Prediction/Previous Variables"

        //Prediction
        const int MAX_PREDICTED_FRAMES = 1000;
        Vector2[] playerPredictedLocation = new Vector2[MAX_PREDICTED_FRAMES];
        Vector2[] playerPredictedVelocity = new Vector2[MAX_PREDICTED_FRAMES];
        Vector2[] playerPredictedAcceleration = new Vector2[MAX_PREDICTED_FRAMES];

        //Previous
        const int MAX_PREVIOUS_FRAMES = 500;
        int currentFrame = 0;
        Vector2[] playerPreviousLocation = new Vector2[MAX_PREVIOUS_FRAMES];
        #endregion

        //Constructor for player, starts/initializes everything, sets spawn location
        public Player(float x, float y, IServiceProvider serviceProvider, int init_playerIndex)
        {
            playerIndex = init_playerIndex;

            content = new ContentManager(serviceProvider, "Content");

            playerLocation.X = x;
            playerLocation.Y = y;

            gameTimeStampMillisecond = 0;
            gameTimeStampSecond = 0;

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
                playerColor = Color.LightBlue;
            }
            else if (playerIndex == 3)
            {
                playerColor = Color.LightGreen;
            }
            #endregion

            keyboardVector = new Vector2(0, 0);

            playerThrust = new Vector2(0, 0);

            playerAcceleration = new Vector2(0, 0);

            playerRectangle = new Rectangle(0, 0, width, height);
            playerPredictedRectangle = new Rectangle(0, 0, 2, 2);

            playerOrigin.X = width / 2;
            playerOrigin.Y = height / 2;

            playerRotation = 0;
            playerAimRotation = 0;

            this.LoadContent();
        }

        /// <summary>
        /// Load all content via
        /// content pipeline
        /// </summary>
        private void LoadContent()
        {
            playerTexture = content.Load<Texture2D>("Player/game_ship");

            playerThrustTexture = content.Load<Texture2D>("Player/game_ship_thrust");

            playerPredictionTexture = content.Load<Texture2D>("whiteTexture");
        }

        /// <summary>
        /// Overload of update method for players that
        /// are not player1
        /// </summary>
        /// <param name="gamepad">Provides gamepad state.</param>
        /// <param name="gamepad_OLDSTATE">Provides previous frame gamepad state.</param>
        /// /// <param name="keyboard">Provides keyboard state.</param>
        /// /// <param name="keyboard_OLDSTATE">Provides previous frame keyboard state.</param>
        /// <param name="gravityList">Provides the total number of gravity wells near you.</param>
        public void update(GamePadState gamepad, GamePadState gamepad_OLDSTATE, KeyboardState keyboard, KeyboardState keyboard_OLDSTATE, List<Gravity> gravityList, GameTime gameTime)
        {
            currentFrame++;

            //Player is playing
            playerReady = true;
            //Player didnt boost
            playerBoosted = false;

            //Figure out if user wants player to move(movement logic)
            playerControls(gamepad, gamepad_OLDSTATE, keyboard, keyboard_OLDSTATE, gameTime);

            //Calculate acceleration
            calcAcceleration(gravityList);

            //Updates player location based on velocity
            playerLocation += playerVelocity;

            //Console.WriteLine("Player Velocity: " + playerVelocity);
            Console.WriteLine("Player Location: " + playerLocation);
            //Console.WriteLine("Player Acceleration: " + playerAcceleration);

            calculatePreviousLocation();

            if (currentFrame == MAX_PREVIOUS_FRAMES)
            {
                currentFrame = 0;
            }
        }
        #region"update overload method for not player1 - Also DynamicSpawn method"
        /// <summary>
        /// Overload of update method for players that
        /// are not player1
        /// </summary>
        /// <param name="gamepad">Provides gamepad state.(Buttons</param>
        /// <param name="gamepad_OLDSTATE">Provides previous frame gamepad state.(Buttons</param>
        /// <param name="gravityList">Provides the total number of gravity wells near you.</param>
        public void update(GamePadState gamepad, GamePadState gamepad_OLDSTATE, List<Gravity> gravityList, GameTime gameTime)
        {
            //Player is playing
            playerReady = true;
            //Player didnt boost
            playerBoosted = false;

            //Calculate acceleration
            calcAcceleration(gravityList);

            //Figure out if user wants player to move(movement logic)
            playerControls(gamepad, gamepad_OLDSTATE, gameTime);

            //Updates player location based on velocity
            playerLocation += playerVelocity;

            //Updates player rectangle
            playerRectangle = new Rectangle(0, 0, width, height);
        }
        /// <summary>
        /// Overload of update method for players that
        /// are not player1
        /// </summary>
        /// <param name="dynamicSpawn">Provides player.</param>
        public void updateDynamicSpawn(Player player1)
        {
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
            //If the user moves the left thumbstick(in any direction)
            if ((gamePad.ThumbSticks.Left.X <= 0.2) || (gamePad.ThumbSticks.Left.X >= -0.2) || (gamePad.ThumbSticks.Left.Y >= 0.2) || (gamePad.ThumbSticks.Left.Y <= 0.2))
            {
                //Pass the X and Y value to the thrust method in the Player class
                setThrust(gamePad.ThumbSticks.Left);
                //GamePad.SetVibration(PlayerIndex.One, Math.Sqrt(Math.Pow(gamePad.ThumbSticks.Left.X,2) + Math.Pow(gamePad.ThumbSticks.Left.Y,2)), 0);
            }
            else
            {
                GamePad.SetVibration(PlayerIndex.One, 0, 0);
            }

            //left shoulder or spacebar was clicked(pressed, then released)
            if ((gamePad.Buttons.LeftShoulder != ButtonState.Pressed && gamePad_OLDSTATE.Buttons.LeftShoulder == ButtonState.Pressed))
            {
                boostDirection(gamePad.ThumbSticks.Left.X, gamePad.ThumbSticks.Left.Y);
            }

            playerAimRotation = (float)Math.Atan2(gamePad.ThumbSticks.Right.X * (-1), gamePad.ThumbSticks.Right.Y * (-1));

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
            if (gamePad.Triggers.Left > 0.2 || gamePad.Triggers.Right > 0.2)
            {
                //GATTLING
                if (currentWeapon == 1)
                {
                    //If gameTimeStampMillisecond and gameTimeStampSecond are less than actual gameTime (FREQUENCY in which you can shoot the gattling)
                    if (gameTime.TotalGameTime.Milliseconds >= gameTimeStampMillisecond && gameTime.TotalGameTime.Seconds >= gameTimeStampSecond)
                    {
                        playerShooting = true;

                        //Since milliseconds reverts to 0 after 1000, modulate it
                        gameTimeStampMillisecond = (gameTime.TotalGameTime.Milliseconds + GATTLING_FREQUENCY) % 1000;

                        //Update seconds
                        gameTimeStampSecond = gameTime.TotalGameTime.Seconds;

                        //Yeah...
                        if (gameTime.TotalGameTime.Milliseconds >= gameTimeStampMillisecond)
                        {
                            gameTimeStampSecond = gameTime.TotalGameTime.Seconds + 1;
                        }
                    }
                    else
                    {
                        playerShooting = false;

                        //gameTime.TotalGameTime.Seconds reverts back to 0 after 60 seconds has past.
                        //So make gameTimeStampSecond relative
                        if (Math.Abs(gameTimeStampSecond - gameTime.TotalGameTime.Seconds) > 2)
                        {
                            gameTimeStampSecond = gameTime.TotalGameTime.Seconds;
                        }

                        //For some reason bugs happen and sometimes gameTimeStampSecond is less then actual time.
                        //So we catch it here
                        if (gameTimeStampSecond < gameTime.TotalGameTime.Seconds)
                        {
                            gameTimeStampSecond = gameTime.TotalGameTime.Seconds;
                        }
                        //For some reason bugs happen and gameTimeStampMillisecond somehow has too much time added to it.
                        //So we catch it here
                        if (gameTimeStampMillisecond > (gameTime.TotalGameTime.Milliseconds + GATTLING_FREQUENCY) % 1000)
                        {
                            gameTimeStampMillisecond = (gameTime.TotalGameTime.Milliseconds + GATTLING_FREQUENCY) % 1000;
                        }
                    }
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
                playerShooting = false;
            }
            #endregion

            #endregion

            #region"Keyboard Movement Logic"
            //If keyboard key 'Up' is pressed
            if (keyboard.IsKeyDown(Keys.Up))
            {
                //The user is pressing UP so the Y value is +1
                keyboardVector.Y = 1;

                //Passing the keyboards vector to player1.
                setThrust(keyboardVector);
            }
            //If keyboard key 'Down' is pressed
            else if (keyboard.IsKeyDown(Keys.Down))
            {
                //The user is pressing DOWN so the Y value is -1
                keyboardVector.Y = -1;

                //Passing the keyboards vector to player1.
                setThrust(keyboardVector);

            }
            //Up for Down not pressed
            else
            {
                keyboardVector.Y = 0;
            }
            //If keyboard key 'Left' is pressed
            if (keyboard.IsKeyDown(Keys.Left))
            {
                //The user is pressing LEFT so the X value is -1
                keyboardVector.X = -1;

                //Passing the keyboards vector to player1.
                setThrust(keyboardVector);
            }
            //If keyboard key 'Right' is pressed
            else if (keyboard.IsKeyDown(Keys.Right))
            {
                //The user is pressing RIGHT so the X value is +1
                keyboardVector.X = 1;

                //Passing the keyboards vector to player1.
                setThrust(keyboardVector);
            }
            //Left or Right not pressed
            else
            {
                keyboardVector.X = 0;
            }

            //Spacebar
            if ((keyboard.IsKeyUp(Keys.Space) && keyboard_OLDSTATE.IsKeyDown(Keys.Space)))
            {
                boostDirection(keyboardVector.X, keyboardVector.Y);
            }
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
            //If the user moves the left thumbstick(in any direction)
            if ((gamePad.ThumbSticks.Left.X <= 0.2) || (gamePad.ThumbSticks.Left.X >= -0.2) || (gamePad.ThumbSticks.Left.Y >= 0.2) || (gamePad.ThumbSticks.Left.Y <= 0.2))
            {
                //Pass the X and Y value to the thrust method in the Player class
                setThrust(gamePad.ThumbSticks.Left);
            }

            //left shoulder or spacebar was clicked(pressed, then released)
            if ((gamePad.Buttons.LeftShoulder != ButtonState.Pressed && gamePad_OLDSTATE.Buttons.LeftShoulder == ButtonState.Pressed))
            {
                boostDirection(gamePad.ThumbSticks.Left.X, gamePad.ThumbSticks.Left.Y);
            }

            playerAimRotation = (float)Math.Atan2(gamePad.ThumbSticks.Right.X, (-1) * gamePad.ThumbSticks.Right.Y);

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
            if (gamePad.Triggers.Left > 0 || gamePad.Triggers.Right > 0)
            {
                //GATTLING
                if (currentWeapon == 1)
                {
                    //If gameTimeStampMillisecond and gameTimeStampSecond are less than actual gameTime (FREQUENCY in which you can shoot the gattling)
                    if (gameTime.TotalGameTime.Milliseconds >= gameTimeStampMillisecond && gameTime.TotalGameTime.Seconds >= gameTimeStampSecond)
                    {
                        playerShooting = true;

                        //Since milliseconds reverts to 0 after 1000, modulate it
                        gameTimeStampMillisecond = (gameTime.TotalGameTime.Milliseconds + GATTLING_FREQUENCY) % 1000;

                        //Update seconds
                        gameTimeStampSecond = gameTime.TotalGameTime.Seconds;

                        //Yeah...
                        if (gameTime.TotalGameTime.Milliseconds >= gameTimeStampMillisecond)
                        {
                            gameTimeStampSecond = gameTime.TotalGameTime.Seconds + 1;
                        }
                    }
                    else
                    {
                        playerShooting = false;
                        //gameTime.TotalGameTime.Seconds reverts back to 0 after 60 seconds has past.
                        //So make gameTimeStampSecond relative
                        if (Math.Abs(gameTimeStampSecond - gameTime.TotalGameTime.Seconds) > 2)
                        {
                            gameTimeStampSecond = gameTime.TotalGameTime.Seconds;
                        }

                        //For some reason bugs happen and sometimes gameTimeStampSecond is less then actual time.
                        //So we catch it here
                        if (gameTimeStampSecond < gameTime.TotalGameTime.Seconds)
                        {
                            gameTimeStampSecond = gameTime.TotalGameTime.Seconds;
                        }
                        //For some reason bugs happen and gameTimeStampMillisecond somehow has too much time added to it.
                        //So we catch it here
                        if (gameTimeStampMillisecond > (gameTime.TotalGameTime.Milliseconds + GATTLING_FREQUENCY) % 1000)
                        {
                            gameTimeStampMillisecond = (gameTime.TotalGameTime.Milliseconds + GATTLING_FREQUENCY) % 1000;
                        }
                    }
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
            #endregion
        }
        #endregion


        /// <summary>
        /// Which direction to thrust in
        /// </summary>
        /// <param name="initThrust">Provides a Vector2 X(0-1) and Y(0-1). This vector comes from LEFT THUMBSTICK</param>
        public void setThrust(Vector2 initThrust)
        {
            playerThrust = initThrust;

            if (invertedY)
            {
                playerThrust.Y = playerThrust.Y * -1;
            }
            if (invertedX)
            {
                playerThrust.X = playerThrust.X * -1;
            }

            if (playerThrust.X != 0 || playerThrust.Y != 0)
            {
                playerRotation = (float)Math.Atan2(playerThrust.X, (-1) * playerThrust.Y);
            }
        }

        public void setIsPlayerReady(bool initPlayerReady)
        {
            playerReady = initPlayerReady;
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

            for (int k = 1; k < MAX_PREDICTED_FRAMES; k++)
            {
                Vector2 pTemp = new Vector2();
                //playerPredictedVelocity[k].X = MathHelper.Clamp(playerPredictedVelocity[k].X, (-1) * PLAYERMAX, PLAYERMAX);
                //playerPredictedVelocity[k].Y = MathHelper.Clamp(playerPredictedVelocity[k].Y, (-1) * PLAYERMAX, PLAYERMAX);
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

        public bool isPlayerShooting()
        {
            return playerShooting;
        }

        public bool isPlayerReady()
        {
            return playerReady;
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
                //Console.WriteLine("There are " + gravityList.Count() + "gravity wells. Gravity Vector: " + gravityList[i].calcGVectorAcceleration(playerLocation.X, playerLocation.Y, PLAYER_MASS) + "\tGravity Location: " + gravityList[i].getGravityLocationX() + " " + gravityList[i].getGravityLocationY());
            }

            playerAcceleration = (playerThrust * PLAYER_THRUST_SCALE) /*add gravity effect here*/ + temp;

            temp = new Vector2();

            playerVelocity += playerAcceleration;

            //playerVelocity.X = MathHelper.Clamp(playerVelocity.X, (-1) * PLAYERMAX, PLAYERMAX);
            //playerVelocity.Y = MathHelper.Clamp(playerVelocity.Y, (-1) * PLAYERMAX, PLAYERMAX);

            this.calculatePrediction(gravityList);

        }

        /// <summary>
        /// Draws the player location
        /// and draws player predicted locations
        /// </summary>
        /// <param name="spriteBatch">Provides the SpriteBatch to allow drawing.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            //UNCOMMENT FOR PREVIOUS PLAYER LOCATIONS(trails)
            for (int i = 0; i < MAX_PREVIOUS_FRAMES; i++)
            {
                spriteBatch.Draw(playerPredictionTexture, playerPreviousLocation[i], playerPredictedRectangle, Color.Purple * 1f);
            }


            for (int i = 0; i < MAX_PREDICTED_FRAMES; i++)
            {
                spriteBatch.Draw(playerPredictionTexture, playerPredictedLocation[i], playerPredictedRectangle, Color.Green * 1f);
            }

            spriteBatch.Draw(playerTexture, playerLocation, playerRectangle, playerColor, playerRotation, playerOrigin, 1.0f, SpriteEffects.None, 0);

            spriteBatch.Draw(playerThrustTexture, playerLocation, playerRectangle, Color.White * (Math.Abs(playerThrust.X) + Math.Abs(playerThrust.Y)), playerRotation, playerOrigin, 1.0f, SpriteEffects.None, 0);
        }
    }
}

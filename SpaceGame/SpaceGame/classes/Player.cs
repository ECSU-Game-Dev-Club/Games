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

        //Create a new content manager to load content used just by this level.
        ContentManager content;

        //Player origin(or middle of player)
        Vector2 playerOrigin;

        //Players texture
        Texture2D playerTexture;
        Texture2D playerThrustTexture;
        Texture2D playerPredictionTexture;
        Color playerColor;

        //Inverted Y or X?
        bool invertedY = true;
        bool invertedX = false;

        //Keyboard vector(used to help smooth movement on keyboard)
        Vector2 keyboardVector;

        //Players location
        Vector2 playerLocation;

        //Players velocity and acceleration for calculating speed
        Vector2 playerVelocity;
        Vector2 playerAcceleration;
        //Player Rotation redians
        float playerRotation;

        //predictive 
        Vector2[] playerPredictedLocation = new Vector2[2400];
        Vector2[] playerPredictedVelocity = new Vector2[2400];
        Vector2[] playerPredictedAcceleration = new Vector2[2400];

        //Player mass
        const double PLAYERMASS = 1;

        //thrust is the triggers on the gamepad
        Vector2 playerThrust;
        const float PLAYERTHRUSTSCALE = .1f;

        //Player Boost Velocity
        const float PLAYERBOOSTVELOCITY = 5;

        //Maximum player speed
        const float PLAYERMAX = 10; //NOT CORRECT   

        //Constructor for player, starts/initializes everything
        public Player(float x, float y, IServiceProvider serviceProvider)
        {
            content = new ContentManager(serviceProvider, "Content");

            playerLocation.X = x;
            playerLocation.Y = y;

            playerColor = Color.White;

            keyboardVector = new Vector2(0, 0);

            playerThrust = new Vector2(0, 0);

            playerAcceleration = new Vector2(0, 0);

            playerRectangle = new Rectangle(0, 0, width, height);
            playerPredictedRectangle = new Rectangle(0, 0, 2, 2);

            playerOrigin.X = width / 2;
            playerOrigin.Y = height / 2;

            playerRotation = 0;

            this.LoadContent();
        }

        private void LoadContent()
        {
            playerTexture = content.Load<Texture2D>("player/game_ship");

            playerThrustTexture = content.Load<Texture2D>("player/game_ship_thrust");

            playerPredictionTexture = content.Load<Texture2D>("whiteTexture");
        }

        //Updates the player every frame
        public void update(GamePadState gamepad, GamePadState gamepad_OLDSTATE, KeyboardState keyboard, KeyboardState keyboard_OLDSTATE, List<Gravity> gravityList)
        {
            //Calculate acceleration
            calcAcceleration(gravityList);

            //Figure out if user wants player to move(movement logic)
            playerControls(gamepad, gamepad_OLDSTATE, keyboard, keyboard_OLDSTATE);

            //Updates player location based on velocity
            playerLocation += playerVelocity;

            //Updates player rectangle
            playerRectangle = new Rectangle(0, 0, width, height);

            Console.WriteLine("Player Velocity: " + playerVelocity);
        }

        //Does player movement logic here
        private void playerControls(GamePadState gamePad, GamePadState gamePad_OLDSTATE, KeyboardState keyboard, KeyboardState keyboard_OLDSTATE)
        {
            #region"GamePad Movement Logic"
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
            //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
            //If keyboard key 'Down' is pressed
            else if (keyboard.IsKeyDown(Keys.Down))
            {
                //The user is pressing DOWN so the Y value is -1
                keyboardVector.Y = -1;

                //Passing the keyboards vector to player1.
                setThrust(keyboardVector);

            }
            //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
            //If keyboard key 'Left' is pressed
            else if (keyboard.IsKeyDown(Keys.Left))
            {
                //The user is pressing LEFT so the X value is -1
                keyboardVector.X = -1;

                //Passing the keyboards vector to player1.
                setThrust(keyboardVector);
            }
            //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
            //If keyboard key 'Right' is pressed
            else if (keyboard.IsKeyDown(Keys.Right))
            {
                //The user is pressing RIGHT so the X value is +1
                keyboardVector.X = 1;

                //Passing the keyboards vector to player1.
                setThrust(keyboardVector);
            }
            //If no arrow keys are pressed
            else
            {
                keyboardVector.X = 0;
                keyboardVector.Y = 0;
            }

            //Spacebar
            if ((keyboard.IsKeyUp(Keys.Space) && keyboard_OLDSTATE.IsKeyDown(Keys.Space)))
            {
                boostDirection(keyboardVector.X, keyboardVector.Y);
            }
            #endregion
        }

        //Which direction to thrust
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

        //Boost in a direction
        private void boostDirection(float x, float y)
        {
            playerVelocity.X += PLAYERBOOSTVELOCITY * x;
            playerVelocity.Y += PLAYERBOOSTVELOCITY * (-1 * y);
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

        private void calcAcceleration(List<Gravity> gravityList)
        {
            Vector2 temp = new Vector2();

            for (int i = 0; i < gravityList.Count(); i++)
            {
                temp += gravityList[i].calcGVectorAcceleration(playerLocation.X, playerLocation.Y, PLAYERMASS);
                Console.WriteLine("There are " + gravityList.Count() + "gravity wells. Gravity Vector: " + gravityList[i].calcGVectorAcceleration(playerLocation.X, playerLocation.Y, PLAYERMASS) + "\tGravity Location: " + gravityList[i].getGravityLocationX() + " " + gravityList[i].getGravityLocationY());
            }

            playerAcceleration = (playerThrust * PLAYERTHRUSTSCALE) /*add gravity effect here*/ + temp;

            temp = new Vector2();

            playerVelocity += playerAcceleration;

            //playerVelocity.X = MathHelper.Clamp(playerVelocity.X, (-1) * PLAYERMAX, PLAYERMAX);
            //playerVelocity.Y = MathHelper.Clamp(playerVelocity.Y, (-1) * PLAYERMAX, PLAYERMAX);

            //predictive path
            playerPredictedLocation[0] = playerLocation;
            playerPredictedVelocity[0] = playerVelocity;
            playerPredictedAcceleration[0] = playerAcceleration;

            for (int k = 1; k < 1000; k++)
            {
                Vector2 pTemp = new Vector2();
                //playerPredictedVelocity[k].X = MathHelper.Clamp(playerPredictedVelocity[k].X, (-1) * PLAYERMAX, PLAYERMAX);
                //playerPredictedVelocity[k].Y = MathHelper.Clamp(playerPredictedVelocity[k].Y, (-1) * PLAYERMAX, PLAYERMAX);
                playerPredictedLocation[k] = playerPredictedLocation[k - 1] + playerPredictedVelocity[k];
                for (int i = 0; i < gravityList.Count(); i++)
                {
                    pTemp += gravityList[i].calcGVectorAcceleration(playerPredictedLocation[k].X, playerPredictedLocation[k].Y, PLAYERMASS);
                }

                playerPredictedAcceleration[k] = pTemp;
                playerPredictedVelocity[k] = playerPredictedVelocity[k - 1] + playerPredictedAcceleration[k];

                pTemp = new Vector2();


            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < 240; i++)
            {
                spriteBatch.Draw(playerPredictionTexture, playerPredictedLocation[i], playerPredictedRectangle, Color.Green * 1f);
            }

            spriteBatch.Draw(playerTexture, playerLocation, playerRectangle, playerColor, playerRotation, playerOrigin, 1.0f, SpriteEffects.None, 0);

            spriteBatch.Draw(playerThrustTexture, playerLocation, playerRectangle, Color.White * (Math.Abs(playerThrust.X) + Math.Abs(playerThrust.Y)), playerRotation, playerOrigin, 1.0f, SpriteEffects.None, 0);
        }
    }
}

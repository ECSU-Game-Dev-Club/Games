using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
namespace SpaceGame
{
    class Player
    {
        Rectangle playerRectangle;
        int width = 10;
        int height = 10;

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

        //Player mass
        static double playerMass = 1;

        //thrust is the triggers on the gamepad
        Vector2 playerThrust;
        static float playerThrustScale = .1f;

        //Player Boost Velocity
        static float playerBoostVelocity = 5;

        //Maximum player speed
        static float playerMax = 10; //NOT CORRECT   

        //Constructor for player, starts/initializes everything
        public Player(float x, float y)
        {
            playerLocation.X = x;
            playerLocation.Y = y;

            keyboardVector = new Vector2(0, 0);

            playerThrust = new Vector2(0, 0);

            playerAcceleration = new Vector2(0, 0);

            playerRectangle = new Rectangle((int)playerLocation.X, (int)playerLocation.Y, width, height);
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
            playerRectangle = new Rectangle((int)playerLocation.X, (int)playerLocation.Y, width, height);
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
        }

        //Boost in a direction
        private void boostDirection(float x, float y)
        {
            playerVelocity.X += playerBoostVelocity * x;
            playerVelocity.Y += playerBoostVelocity * (-1 * y);
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

        private void calcAcceleration(List<Gravity> gravityList)
        {
            /* TEMP GRAVITY
            // (300,300) is a temporary value for gravity well location, 10000f is a temporary value for gravity well mass (planet mass)
            // for loop here for multiple gravity wells
            //calcGPlayerVectortAcceleration(300, 300, 10000);
            */
            for (int i = 1; i < gravityList.Count(); i++)
            {
                Console.WriteLine("i: " + i + "\tgravity list count: " + gravityList.Count());
                playerAcceleration = (playerThrust * playerThrustScale) /*add gravity effect here*/ + gravityList[i].calcGVectorAcceleration(playerLocation.X, playerLocation.Y, playerMass);
            }
            
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

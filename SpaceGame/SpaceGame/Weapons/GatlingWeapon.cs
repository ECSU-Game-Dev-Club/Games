using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SpaceGame
{
    class GatlingWeapon
    {
        bool playerShooting;

        //GameTime Stamp
        int gameTimeStampMillisecond;
        int gameTimeStampSecond;
        const int GATLING_FREQUENCY = 75; // In Milliseconds

        public GatlingWeapon()
        {
            playerShooting = false;

            gameTimeStampMillisecond = 0;
            gameTimeStampSecond = 0;
        }

        public void shoot(GameTime gameTime, Player player, GamePadState gamePad)
        {
            //If gameTimeStampMillisecond and gameTimeStampSecond are less than actual gameTime (FREQUENCY in which you can shoot the gattling)
            if (gameTime.TotalGameTime.Milliseconds >= gameTimeStampMillisecond && gameTime.TotalGameTime.Seconds >= gameTimeStampSecond)
            {
                playerShooting = true;

                //Since milliseconds reverts to 0 after 1000, modulate it
                gameTimeStampMillisecond = (gameTime.TotalGameTime.Milliseconds + GATLING_FREQUENCY) % 1000;

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
                if (gameTimeStampMillisecond > (gameTime.TotalGameTime.Milliseconds + GATLING_FREQUENCY) % 1000)
                {
                    gameTimeStampMillisecond = (gameTime.TotalGameTime.Milliseconds + GATLING_FREQUENCY) % 1000;
                }
            }
        }

        public bool getShooting()
        {
            return playerShooting;
        }

        public void setShooting(bool shooting)
        {
            playerShooting = shooting;
        }
        
    }
}

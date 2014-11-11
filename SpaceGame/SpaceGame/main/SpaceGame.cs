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
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class SpaceGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //FPS Variables
        int totalFrames = 0;
        float elapsedTime = 0.0f;
        int fps = 0;

        //Player classes with player variables, draw method, and mobility calculations
        Player player1;
        Player player2;
        Player player3;
        Player player4;

        //Array of player locations
        Player[] playerArray = new Player[4];

        //Background class for drawing/building the background
        Background background;

        #region"gamepads for all players:
        //Player1's gamepad
        GamePadState gamePad1;
        GamePadState gamePad1_OLDSTATE;

        //Player2's gamepad
        GamePadState gamePad2;
        GamePadState gamePad2_OLDSTATE;
        bool isPlayerTwoPlaying = false;

        //Player3's gamepad
        GamePadState gamePad3;
        GamePadState gamePad3_OLDSTATE;
        bool isPlayerThreePlaying = false;

        //Player4's gamepad
        GamePadState gamePad4;
        GamePadState gamePad4_OLDSTATE;
        bool isPlayerFourPlaying = false;
        #endregion

        //KEYBOAD
        KeyboardState keyboard;
        KeyboardState keyboard_OLDSTATE;

        //Mouse input
        MouseState mouse;
        MouseState mouse_OLDSTATE;

        //Setting up a rectangle for the users screen size
        Rectangle screenSize;
        //Is the game in fullscreen mode
        bool isFullScreen = false;

        //Sets up camera class
        Camera camera;

        //GRAVITY STUFF - DELELTE ME WHEN LEVELS COME IN
        List<Gravity> gravityList = new List<Gravity>();              //DELETE ME WHEN LEVELS COME IN
        List<Rectangle> gravityRectangleList = new List<Rectangle>(); //DELETE ME WHEN LEVELS COME IN
        Texture2D gravityTexture;                                     //DELETE ME WHEN LEVELS COME IN
        float gravityWellRotation;                                    //DELETE ME WHEN LEVELS COME IN

        //DELETE ME WHEN ENEMIES ARE FINISHED
        Enemy_prototype enemy1;

        #region"Developer Stuff"
        Rectangle cameraRectangle;
        #endregion

        public SpaceGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            //Sets the buffer(res) to the hardware res/ and gives it to a rectangle
            //X
            graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            screenSize.Width = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            //Y
            graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            screenSize.Height = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            //Is the game multisampling
            graphics.PreferMultiSampling = false;

            //Is it in fullscreen mode
            graphics.IsFullScreen = isFullScreen;

            IsMouseVisible = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            //Initializes the player class for player1 with the spawn at 
            player1 = new Player(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Width / 2, Services);
            player2 = new Player(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Width / 2, Services);
            player3 = new Player(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Width / 2, Services);
            player4 = new Player(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Width / 2, Services);

            //Initializing Camera
            camera = new Camera(GraphicsDevice.Viewport);

            //Initializing Mouse
            mouse = Mouse.GetState();

            //Initializing background(builds stars)
            background = new Background(Services, screenSize);

            enemy1 = new Enemy_prototype(1600, 1600, Services);

            //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
            // TODO: Add your initialization logic here
            //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            gravityTexture = Content.Load<Texture2D>("gWell");

            //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
            // TODO: use this.Content to load your game content here
            //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
            // TODO: Unload any non ContentManager content here
            //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            #region"FPS COUNTER"
            //Calculates FPS
            elapsedTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            //1 Second has passed
            if (elapsedTime >= 1000.0f)
            {
                fps = totalFrames;
                totalFrames = 0;
                elapsedTime = 0;
            }

            //Writes FPS to console
            Console.Clear();
            Console.WriteLine(fps);
            #endregion

            #region"Gets all input states"
            //Gets both gamepad and keyboard states(what buttons are pressed)
            gamePad1 = GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular);
            gamePad2 = GamePad.GetState(PlayerIndex.Two, GamePadDeadZone.Circular);
            gamePad3 = GamePad.GetState(PlayerIndex.Three, GamePadDeadZone.Circular);
            gamePad4 = GamePad.GetState(PlayerIndex.Four, GamePadDeadZone.Circular);
            keyboard = Keyboard.GetState();
            mouse = Mouse.GetState();
            #endregion

            #region"Checks if a player wants to play(Press Start)"
            //Player2
            if (gamePad2.IsButtonUp(Buttons.Start) && gamePad2.IsButtonDown(Buttons.Start))
            {
                isPlayerTwoPlaying = !isPlayerTwoPlaying;
            }
            //Player3
            if (gamePad3.IsButtonUp(Buttons.Start) && gamePad3.IsButtonDown(Buttons.Start))
            {
                isPlayerThreePlaying = !isPlayerThreePlaying;
            }
            //Player4
            if (gamePad4.IsButtonUp(Buttons.Start) && gamePad4.IsButtonDown(Buttons.Start))
            {
                isPlayerFourPlaying = !isPlayerFourPlaying;
            }
            #endregion

            // Allows the game to exit, by pressing back on gamepad OR escape on keyboard
            if (gamePad1.Buttons.Back == ButtonState.Pressed || keyboard.IsKeyDown(Keys.Escape))
            {
                this.Exit();
            }

            //toggle Fullscreen
            if ((keyboard.IsKeyUp(Keys.F11) && keyboard_OLDSTATE.IsKeyDown(Keys.F11)))
            {
                graphics.ToggleFullScreen();
            }

            //Gravity wells with a click of mouse1
            #region"DELETE ME WHEN YOU MAKE LEVELS"
            gravityWellRotation -= 0.1f;
            if (mouse.LeftButton != ButtonState.Pressed && mouse_OLDSTATE.LeftButton == ButtonState.Pressed)
            {
                gravityList.Add(new Gravity(mouse.X + camera.getCameraOrigin().X, mouse.Y + camera.getCameraOrigin().Y, 25000));
                gravityRectangleList.Add(new Rectangle(0, 0, 50, 50));
            }
            #endregion

            #region"Updates all players"

            //Updates the player1 class and passes all inputs
            player1.update(gamePad1, gamePad1_OLDSTATE, keyboard, keyboard_OLDSTATE, gravityList);

            //If player 2 is playing
            if (isPlayerTwoPlaying)
            {
                //Updates the player2 class and passes all inputs
                player2.update(gamePad2, gamePad2_OLDSTATE, gravityList);
            }
            else
            {
                player2.updateDynamicSpawn(player1.getPlayerLocation());
            }

            //If player 3 is playing
            if (isPlayerThreePlaying)
            {
                //Updates the player3 class and passes all inputs
                player3.update(gamePad3, gamePad3_OLDSTATE, gravityList);
            }
            else
            {
                player3.updateDynamicSpawn(player1.getPlayerLocation());
            }

            //If player 4 is playing
            if (isPlayerFourPlaying)
            {
                //Updates the player4 class and passes all inputs
                player4.update(gamePad4, gamePad4_OLDSTATE, gravityList);
            }
            else
            {
                player4.updateDynamicSpawn(player1.getPlayerLocation());
            }

            playerArray[0] = player1;
            playerArray[1] = player2;
            playerArray[2] = player3;
            playerArray[3] = player4;
            #endregion

            //Updates camera by passing the players rectangle and gametime
            camera.Update(player1.getPlayerLocation());

            //Updates background(Stars snap with camera)
            background.Update(player1.getPlayerLocationDelta());

            #region"Dev Stuff"
            cameraRectangle = new Rectangle((int)Camera.cameraCenter.X, (int)Camera.cameraCenter.Y, 20, 20);

            enemy1.update(gravityList, playerArray);

            if (keyboard.IsKeyDown(Keys.OemOpenBrackets))
            {
                camera.zoomIncriment();
            }
            if (keyboard.IsKeyDown(Keys.OemCloseBrackets))
            {
                camera.zoomDecriment();
            }
            #endregion

            //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
            // TODO: Add your update logic here
            //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

            #region"Gets all old inputs - ALWAYS AT BOTTOM"
            //PREVIOUS FRAME GAMEPAD AND KEYBOARD STATES - Always put at bottom
            gamePad1_OLDSTATE = gamePad1;
            gamePad2_OLDSTATE = gamePad2;
            gamePad3_OLDSTATE = gamePad3;
            gamePad4_OLDSTATE = gamePad4;

            keyboard_OLDSTATE = keyboard;
            mouse_OLDSTATE = mouse;
            #endregion

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            //Counts Frames for FPS
            totalFrames++;

            GraphicsDevice.Clear(Color.Black);

            #region"Static drawing"
            //STATIC DRAW
            spriteBatch.Begin();

            //Draw background stars as static
            background.DrawBackgroundStars(spriteBatch);

            spriteBatch.End();
            #endregion

            //Begins the sprite batch so we can draw things on the screen(USING CAMERA)
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, camera.transform);

            background.DrawForegroundStars(spriteBatch);

            #region"Draws gravity wells - DELETE ME WHEN LEVELS COME IN"
            for (int i = 0; i < gravityList.Count(); i++)
            {
                spriteBatch.Draw(gravityTexture, gravityList[i].getGravityLocationVector(), gravityRectangleList[i], Color.White, gravityWellRotation, new Vector2(25, 25), 1.0f, SpriteEffects.None, 0);
            }
            #endregion

            enemy1.Draw(spriteBatch);

            #region"Draw all players"
            //Draw everything in player1 class
            player1.Draw(spriteBatch);

            //Draw everything in player2 class
            if (isPlayerTwoPlaying)
            {
                player2.Draw(spriteBatch);
            }

            //Draw everything in player3 class
            if (isPlayerThreePlaying)
            {
                player3.Draw(spriteBatch);
            }

            //Draw everything in player4 class
            if (isPlayerFourPlaying)
            {
                player4.Draw(spriteBatch);
            }
            #endregion

            //spriteBatch.Draw(playerTexture, cameraRectangle, Color.White); //DEV            

            //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
            // DRAW EVERYTHING IN HERE!!!!!!!!!
            //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}

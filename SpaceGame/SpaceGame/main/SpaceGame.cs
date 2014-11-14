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

        //Player3's gamepad
        GamePadState gamePad3;
        GamePadState gamePad3_OLDSTATE;

        //Player4's gamepad
        GamePadState gamePad4;
        GamePadState gamePad4_OLDSTATE;
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

        #region"Developer Stuff"
        List<Enemy_prototype> enemyList = new List<Enemy_prototype>();
        List<EnemySwarmAttach> enemySwarmAttachList = new List<EnemySwarmAttach>();

        //GRAVITY STUFF - DELELTE ME WHEN LEVELS COME IN
        List<Gravity> gravityList = new List<Gravity>();              //DELETE ME WHEN LEVELS COME IN
        List<Rectangle> gravityRectangleList = new List<Rectangle>(); //DELETE ME WHEN LEVELS COME IN
        Texture2D gravityTexture;                                     //DELETE ME WHEN LEVELS COME IN
        float gravityWellRotation;

        int[] listLocations = new int[20];

        Rectangle cameraRectangle;

        SpriteFont font;

        public static bool devMode = false;
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

            #region"DEV CONTENT"

            font = Content.Load<SpriteFont>("Tools/spriteFont");

            #endregion

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
            //Player1 - will spawn a player2
            if (gamePad1.IsButtonUp(Buttons.Start) && gamePad1_OLDSTATE.IsButtonDown(Buttons.Start))
            {
                player2.setIsPlayerReady(!player2.isPlayerReady());
            }
            //Player2
            if (gamePad2.IsButtonUp(Buttons.Start) && gamePad2_OLDSTATE.IsButtonDown(Buttons.Start))
            {
                player2.setIsPlayerReady(!player2.isPlayerReady());
            }
            //Player3
            if (gamePad3.IsButtonUp(Buttons.Start) && gamePad3_OLDSTATE.IsButtonDown(Buttons.Start))
            {
                player3.setIsPlayerReady(!player3.isPlayerReady());
            }
            //Player4
            if (gamePad4.IsButtonUp(Buttons.Start) && gamePad4_OLDSTATE.IsButtonDown(Buttons.Start))
            {
                player4.setIsPlayerReady(!player4.isPlayerReady());
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
            if (player2.isPlayerReady())
            {
                //Updates the player2 class and passes all inputs
                player2.update(gamePad1, gamePad1_OLDSTATE, keyboard, keyboard_OLDSTATE, gravityList);
            }
            else
            {
                player2.updateDynamicSpawn(player1.getPlayerLocation());
            }

            //If player 3 is playing
            if (player3.isPlayerReady())
            {
                //Updates the player3 class and passes all inputs
                player3.update(gamePad1, gamePad1_OLDSTATE, gravityList);
            }
            else
            {
                player3.updateDynamicSpawn(player1.getPlayerLocation());
            }

            //If player 4 is playing
            if (player4.isPlayerReady())
            {
                //Updates the player4 class and passes all inputs
                player4.update(gamePad1, gamePad1_OLDSTATE, gravityList);
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
            //DEVMODE
            if (keyboard.IsKeyUp(Keys.OemTilde) && keyboard_OLDSTATE.IsKeyDown(Keys.OemTilde))
            {
                devMode = !devMode;
            }

            if (mouse.RightButton != ButtonState.Pressed && mouse_OLDSTATE.RightButton == ButtonState.Pressed)
            {
                //enemyList.Add(new Enemy_prototype(mouse.X + camera.getCameraOrigin().X, mouse.Y + camera.getCameraOrigin().Y, Services));
                enemySwarmAttachList.Add(new EnemySwarmAttach(mouse.X + camera.getCameraOrigin().X, mouse.Y + camera.getCameraOrigin().Y, Services));
            }

            cameraRectangle = new Rectangle((int)Camera.cameraCenter.X, (int)Camera.cameraCenter.Y, 20, 20);

            for (int i = 0; i < enemyList.Count; i++)
            {
                enemyList[i].update(gravityList, playerArray);
            }

            for (int i = 0; i < enemySwarmAttachList.Count; i++)
            {
                enemySwarmAttachList[i].update(gravityList, playerArray);
            }

            if (devMode)
            {
                //ZOOM
                if (keyboard.IsKeyDown(Keys.OemOpenBrackets))
                {
                    camera.zoomIncriment();
                }
                if (keyboard.IsKeyDown(Keys.OemCloseBrackets))
                {
                    camera.zoomDecriment();
                }
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

            //DRAW ENEMIES
            for (int i = 0; i < enemyList.Count; i++)
            {
                enemyList[i].Draw(spriteBatch);
            }
            for (int i = 0; i < enemySwarmAttachList.Count; i++)
            {
                enemySwarmAttachList[i].Draw(spriteBatch);
            }

            #region"Draw all players"
            //Draw everything in player1 class
            player1.Draw(spriteBatch);

            //Draw everything in player2 class
            if (player2.isPlayerReady())
            {
                player2.Draw(spriteBatch);
            }

            //Draw everything in player3 class
            if (player3.isPlayerReady())
            {
                player3.Draw(spriteBatch);
            }

            //Draw everything in player4 class
            if (player4.isPlayerReady())
            {
                player4.Draw(spriteBatch);
            }
            #endregion

            //spriteBatch.Draw(playerTexture, cameraRectangle, Color.White); //DEV            

            //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
            // DRAW EVERYTHING IN HERE!!!!!!!!!
            //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

            #region"DEV DRAWING - CAMERA"
            if (devMode)
            {
                //GRAVITY
                for (int i = 0; i < gravityList.Count; i++)
                {
                    spriteBatch.DrawString(font, "#" + i, new Vector2(gravityList[i].getGravityLocationVector().X - 40, gravityList[i].getGravityLocationVector().Y - 40), Color.Yellow);
                }
                //ENEMIES - PROTOTYPE
                for (int i = 0; i < enemyList.Count; i++)
                {
                    spriteBatch.DrawString(font, "#" + i, new Vector2(enemyList[i].getEnemyLocationVector().X - 20, enemyList[i].getEnemyLocationVector().Y - 20), Color.Yellow);
                }
                //ENEMIES - ATTACH
                for (int i = 0; i < enemySwarmAttachList.Count; i++)
                {
                    spriteBatch.DrawString(font, "#" + i, new Vector2(enemySwarmAttachList[i].getEnemyLocationVector().X - 20, enemySwarmAttachList[i].getEnemyLocationVector().Y - 20), Color.Yellow);
                }
                //PLAYERS
                if (player2.isPlayerReady() || player3.isPlayerReady() || player4.isPlayerReady())
                {
                    for (int i = 0; i < enemyList.Count; i++)
                    {
                        spriteBatch.DrawString(font, "#" + i, new Vector2(enemyList[i].getEnemyLocationVector().X - 20, enemyList[i].getEnemyLocationVector().Y - 20), Color.Blue);
                    }
                }
            }
            #endregion

            spriteBatch.End();

            #region"DEV DRAWING - STATIC"
            if (devMode)
            {
                spriteBatch.Begin();

                for (int i = 0; i < listLocations.Length; i++)
                {
                    if (i == 0)
                    {
                        spriteBatch.DrawString(font, "Player Velocity: {X:" + player1.getPlayerVelocityVector().X + "} {Y: " + player1.getPlayerVelocityVector().Y + "}", new Vector2(0, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - (20 * (i + 1))), Color.Yellow);
                    }
                    if (i == 1)
                    {
                        spriteBatch.DrawString(font, "Player Acceleration: {X:" + player1.getPlayerAccelerationVector().X + "} {Y: " + player1.getPlayerAccelerationVector().Y + "}", new Vector2(0, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - (20 * (i + 1))), Color.Yellow);
                    }
                    if (i == 2)
                    {
                        spriteBatch.DrawString(font, "Player Location: {X:" + player1.getPlayerLocation().X + "} {Y: " + player1.getPlayerLocation().Y + "}", new Vector2(0, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - (20 * (i + 1))), Color.Yellow);
                    }
                }


                spriteBatch.End();
            }
            #endregion

            base.Draw(gameTime);
        }
    }
}

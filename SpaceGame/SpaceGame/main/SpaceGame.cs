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

        //GATTLING
        List<Projectile> worldBullets = new List<Projectile>();
        Texture2D gattlingBulletTexture;

        //1x1 white texture
        Texture2D whiteTexture;

        //Maximum distance between all targetting enemies
        double maxEnemyDistanceFromPlayer = 0;
        //atleast one enemy is targetting the player
        bool playerTargeted = false;
        //The index of the furthest enemy
        int farthestEnemyIndex = 0;

        #region"Developer Stuff"
        List<Enemy_prototype> enemyList = new List<Enemy_prototype>();
        List<EnemySwarm> enemySwarmAttachList = new List<EnemySwarm>();
        List<EnemySwarm_re> enemySwarmAttachTESTList = new List<EnemySwarm_re>();

        //GRAVITY STUFF - DELELTE ME WHEN LEVELS COME IN
        List<Gravity> gravityList = new List<Gravity>();              //DELETE ME WHEN LEVELS COME IN
        List<Rectangle> gravityRectangleList = new List<Rectangle>(); //DELETE ME WHEN LEVELS COME IN
        Texture2D gravityTexture;                                     //DELETE ME WHEN LEVELS COME IN
        const float GRAVITY_WELL_ROTATION = -0.1f;
        float gravityRotation = 0;

        const int MAX_CONSOLE = 20;
        int[] listLocations = new int[MAX_CONSOLE];

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
            player1 = new Player(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Width / 2, Services, 0);
            player2 = new Player(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Width / 2, Services, 1);
            player3 = new Player(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Width / 2, Services, 2);
            player4 = new Player(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Width / 2, Services, 3);

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

            whiteTexture = Content.Load<Texture2D>("whiteTexture");

            gattlingBulletTexture = Content.Load<Texture2D>("projectile_textures/gatling_projectile");

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
                this.Dispose();
                this.Exit();
            }

            //toggle Fullscreen
            if ((keyboard.IsKeyUp(Keys.F11) && keyboard_OLDSTATE.IsKeyDown(Keys.F11)))
            {
                graphics.ToggleFullScreen();
            }

            //Update All Players
            updatePlayers(gameTime);

            #region"Updates bullets and checks collisions"
            //Updates all bullets
            for (int i = 0; i < worldBullets.Count; i++)
            {
                //Update Bullets
                worldBullets[i].update(gameTime);

                if(worldBullets[i].deleteMe())
                {
                    worldBullets.RemoveAt(i);
                }

                //Save processing power check ALL enemies
                for (int e = 0; e < (enemyList.Count + enemySwarmAttachList.Count + enemySwarmAttachTESTList.Count); e++)
                {
                    //Check enemyList
                    if (e < enemyList.Count)
                    {
                        //If not idle
                        if (!enemyList[e].getIdle())
                        {

                            if (i < worldBullets.Count)
                            {
                                //If hit
                                if (worldBullets[i].getHitBox().Intersects(enemyList[e].getEnemyHitBox()))
                                {
                                    enemyList[e].hurtEnemy(worldBullets[i].getDamageOfProjectile());

                                    //Check health, if below 0 remove enemy
                                    if (enemyList[e].getEnemyHealth() <= 0)
                                    {
                                        enemyList.RemoveAt(e);
                                    }
                                    worldBullets.RemoveAt(i);
                                }
                            }
                        }
                    }
                    //Check enemySwarm
                    if (e < enemySwarmAttachList.Count)
                    {
                        //If not idle
                        if (!enemySwarmAttachList[e].getIdle())
                        {
                            if (i < worldBullets.Count)
                            {
                                //If hit
                                if (worldBullets[i].getHitBox().Intersects(enemySwarmAttachList[e].getEnemyHitBox()) && !enemySwarmAttachList[e].getAttached())
                                {
                                    enemySwarmAttachList[e].hurtEnemy(worldBullets[i].getDamageOfProjectile());

                                    if (enemySwarmAttachList[e].getEnemyHealth() <= 0)
                                    {
                                        enemySwarmAttachList.RemoveAt(e);
                                    }

                                    worldBullets.RemoveAt(i);
                                }
                            }
                        }
                    }

                    //Check enemySwarmTEST
                    if (e < enemySwarmAttachTESTList.Count)
                    {
                        //If not idle
                        if (!enemySwarmAttachTESTList[e].getIdle())
                        {
                            if (i < worldBullets.Count)
                            {
                                //If hit
                                if (worldBullets[i].getHitBox().Intersects(enemySwarmAttachTESTList[e].getEnemyHitBox()) && !enemySwarmAttachTESTList[e].getAttached())
                                {
                                    enemySwarmAttachTESTList[e].hurtEnemy(worldBullets[i].getDamageOfProjectile());

                                    if (enemySwarmAttachTESTList[e].getEnemyHealth() <= 0)
                                    {
                                        enemySwarmAttachTESTList.RemoveAt(e);
                                    }

                                    worldBullets.RemoveAt(i);
                                }
                            }
                        }
                    }
                }
            }
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

            //For showing the middle of the camera (dev helper)
            cameraRectangle = new Rectangle((int)Camera.cameraCenter.X, (int)Camera.cameraCenter.Y, 20, 20);

            #region"Enemy Prototype"
            //Updates enemy prototypes
            for (int i = 0; i < enemyList.Count; i++)
            {
                //IF ENEMY IS CLOSE ENEOUGH - IDLE = FALSE
                if (enemyList[i].getDistanceToPlayer(0) < GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width)
                {
                    enemyList[i].setIdle(false);
                }
                //IF NOT - IDLE = TRUE
                else
                {
                    enemyList[i].setIdle(true);
                }

                enemyList[i].update(gravityList, playerArray);
            }
            #endregion

            #region"Enemy Swarm"
            //Updates enemy swarms
            for (int i = 0; i < enemySwarmAttachList.Count; i++)
            {
                //IF ENEMY IS CLOSE ENEOUGH -> IDLE = FALSE
                if (enemySwarmAttachList[i].getDistanceToPlayer(0) < GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width)
                {
                    enemySwarmAttachList[i].setIdle(false);
                }
                //IF NOT -> IDLE = TRUE
                else
                {
                    enemySwarmAttachList[i].setIdle(true);
                }

                enemySwarmAttachList[i].update(gravityList, playerArray);
            }
            #endregion

            #region"Enemy Swarm TEST"
            //Updates enemy swarms
            for (int i = 0; i < enemySwarmAttachTESTList.Count; i++)
            {
                enemySwarmAttachTESTList[i].update(gravityList, playerArray);
            }
            #endregion


            //DEVMODE
            if (devMode)
            {
                gravityRotation += GRAVITY_WELL_ROTATION;

                //ZOOM
                if (keyboard.IsKeyDown(Keys.OemOpenBrackets))
                {
                    camera.zoomIncriment();
                }
                if (keyboard.IsKeyDown(Keys.OemCloseBrackets))
                {
                    camera.zoomDecriment();
                }

                //Right Mouse Button
                if (mouse.RightButton != ButtonState.Pressed && mouse_OLDSTATE.RightButton == ButtonState.Pressed)
                {
                    //enemyList.Add(new Enemy_prototype(mouse.X + camera.getCameraOrigin().X, mouse.Y + camera.getCameraOrigin().Y, Services));
                    //enemySwarmAttachList.Add(new EnemySwarm(mouse.X + camera.getCameraOrigin().X, mouse.Y + camera.getCameraOrigin().Y, Services));
                    enemySwarmAttachTESTList.Add(new EnemySwarm_re(mouse.X + camera.getCameraOrigin().X, mouse.Y + camera.getCameraOrigin().Y, Services));
                }
                //Left Mouse Button
                if (mouse.LeftButton != ButtonState.Pressed && mouse_OLDSTATE.LeftButton == ButtonState.Pressed)
                {
                    gravityList.Add(new Gravity(mouse.X + camera.getCameraOrigin().X, mouse.Y + camera.getCameraOrigin().Y, 720000));
                    gravityRectangleList.Add(new Rectangle(0, 0, 50, 50));
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

        public void updatePlayers(GameTime gameTime)
        {
            #region"Player 1"
            //Shooting
            if (player1.isPlayerShooting())
            {
                if (player1.getCurrentWeapon() == 1) // GATLING
                {
                    worldBullets.Add(new Bullet(player1.getPlayerLocation().X,
                        player1.getPlayerLocation().Y, player1.getPlayerVelocityVector(),
                        player1.getAimRotation(), true, gameTime));
                }
                if (player1.getCurrentWeapon() == 2) // MISSILE
                {
                    //Add missiles here, get targets from player
                }
                if (player1.getCurrentWeapon() == 3) // ???
                {

                }
                if (player1.getCurrentWeapon() == 4) // ???
                {

                }
            }

            //Updates the player1 class and passes all inputs
            player1.update(gamePad1, gamePad1_OLDSTATE, keyboard, keyboard_OLDSTATE, gravityList, gameTime);
            #endregion

            #region"Player 2"
            if (player2.isPlayerReady())
            {
                //Shooting
                if (player2.isPlayerShooting())
                {
                    if (player2.getCurrentWeapon() == 1) // GATLING
                    {
                        worldBullets.Add(new Bullet(player2.getPlayerLocation().X,
                            player2.getPlayerLocation().Y, player2.getPlayerVelocityVector(),
                            player2.getAimRotation(), true, gameTime));
                    }
                    if (player2.getCurrentWeapon() == 2) // MISSILE
                    {
                        //Add missiles here, get targets from player
                    }
                    if (player2.getCurrentWeapon() == 3) // ???
                    {

                    }
                    if (player2.getCurrentWeapon() == 4) // ???
                    {

                    }
                }

                //Updates the player2 class and passes all inputs
                player2.update(gamePad2, gamePad2_OLDSTATE, gravityList, gameTime);
            }
            else
            {
                player2.updateDynamicSpawn(player1);
            }
            #endregion

            #region"Player 3"
            if (player3.isPlayerReady())
            {
                //Shooting
                if (player3.isPlayerShooting())
                {
                    if (player3.getCurrentWeapon() == 1) // GATLING
                    {
                        worldBullets.Add(new Bullet(player3.getPlayerLocation().X,
                            player3.getPlayerLocation().Y, player3.getPlayerVelocityVector(),
                            player3.getAimRotation(), true, gameTime));
                    }
                    if (player3.getCurrentWeapon() == 2) // MISSILE
                    {
                        //Add missiles here, get targets from player
                    }
                    if (player3.getCurrentWeapon() == 3) // ???
                    {

                    }
                    if (player3.getCurrentWeapon() == 4) // ???
                    {

                    }
                }

                //Updates the player3 class and passes all inputs
                player3.update(gamePad3, gamePad3_OLDSTATE, gravityList, gameTime);
            }
            else
            {
                player3.updateDynamicSpawn(player1);
            }
            #endregion

            #region"Player 4"
            if (player4.isPlayerReady())
            {
                //Shooting
                if (player4.isPlayerShooting())
                {
                    if (player4.getCurrentWeapon() == 1) // GATLING
                    {
                        worldBullets.Add(new Bullet(player4.getPlayerLocation().X,
                            player4.getPlayerLocation().Y, player4.getPlayerVelocityVector(),
                            player4.getAimRotation(), true, gameTime));
                    }
                    if (player4.getCurrentWeapon() == 2) // MISSILE
                    {
                        //Add missiles here, get targets from player
                    }
                    if (player4.getCurrentWeapon() == 3) // ???
                    {

                    }
                    if (player4.getCurrentWeapon() == 4) // ???
                    {

                    }
                }

                //Updates the player4 class and passes all inputs
                player4.update(gamePad4, gamePad4_OLDSTATE, gravityList, gameTime);
            }
            else
            {
                player4.updateDynamicSpawn(player1);
            }
            #endregion

            playerArray[0] = player1;
            playerArray[1] = player2;
            playerArray[2] = player3;
            playerArray[3] = player4;
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

            //Draw background and middle ground stars as static
            background.DrawBackground_AND_MiddleGound(spriteBatch);

            spriteBatch.End();
            #endregion

            //Begins the sprite batch so we can draw things on the screen(USING CAMERA)
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, camera.transform);

            background.DrawForegroundStars(spriteBatch);

            #region"Draws gravity wells - DELETE ME WHEN LEVELS COME IN"
            for (int i = 0; i < gravityList.Count(); i++)
            {
                spriteBatch.Draw(gravityTexture, gravityList[i].getGravityLocationVector(), gravityRectangleList[i], Color.White, gravityRotation, new Vector2(25, 25), 1.0f, SpriteEffects.None, 0);
            }
            #endregion

            //DRAW ENEMIES
            for (int i = 0; i < enemyList.Count; i++)//Enemy Prototype
            {
                enemyList[i].Draw(spriteBatch);
            }
            for (int i = 0; i < enemySwarmAttachList.Count; i++)//Enemy Swarm
            {
                enemySwarmAttachList[i].Draw(spriteBatch);
            }
            for (int i = 0; i < enemySwarmAttachTESTList.Count; i++)//Enemy Swarm TEST
            {
                enemySwarmAttachTESTList[i].Draw(spriteBatch);
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

            //Draws bullets
            for (int i = 0; i < worldBullets.Count; i++)
            {
                if (worldBullets[i].getCurrentProjectile() == 1)//Gatling
                {
                    spriteBatch.Draw(gattlingBulletTexture, worldBullets[i].getLocationVector(), worldBullets[i].getDrawBox(), Color.Yellow, worldBullets[i].getRotation(), worldBullets[i].getBulletOrigin(), 2.0f, SpriteEffects.None, 0);
                }
            }

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
                spriteBatch.DrawString(font, "#1", new Vector2(playerArray[0].getPlayerLocation().X - 30, playerArray[0].getPlayerLocation().Y - 30), Color.PaleGreen);

                if (player2.isPlayerReady())
                {
                    spriteBatch.DrawString(font, "#2", new Vector2(playerArray[1].getPlayerLocation().X - 30, playerArray[1].getPlayerLocation().Y - 30), Color.PaleGreen);
                }
                if (player3.isPlayerReady())
                {
                    spriteBatch.DrawString(font, "#3", new Vector2(playerArray[2].getPlayerLocation().X - 30, playerArray[2].getPlayerLocation().Y - 30), Color.PaleGreen);
                }
                if (player4.isPlayerReady())
                {
                    spriteBatch.DrawString(font, "#4", new Vector2(playerArray[3].getPlayerLocation().X - 30, playerArray[3].getPlayerLocation().Y - 30), Color.PaleGreen);
                }
            }
            #endregion

            spriteBatch.End();

            #region"DEV DRAWING - STATIC - CONSOLE"
            if (devMode)
            {
                spriteBatch.Begin();

                //Console draw
                for (int i = 0; i < MAX_CONSOLE; i++)
                {
                    if (i == 0)
                    {
                        spriteBatch.DrawString(font, "Left Thumbstick: " + gamePad1.ThumbSticks.Left + " Right Thumbstick: " + gamePad1.ThumbSticks.Right, new Vector2(0, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - (20 * (i + 1))), Color.Green);
                    }
                    if (i == 1)
                    {
                        spriteBatch.DrawString(font, "Player Velocity: {X:" + player1.getPlayerVelocityVector().X + "} {Y: " + player1.getPlayerVelocityVector().Y + "}", new Vector2(0, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - (20 * (i + 1))), Color.Yellow);
                    }
                    if (i == 2)
                    {
                        spriteBatch.DrawString(font, "Player Acceleration: {X:" + player1.getPlayerAccelerationVector().X + "} {Y: " + player1.getPlayerAccelerationVector().Y + "}", new Vector2(0, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - (20 * (i + 1))), Color.Yellow);
                    }
                    if (i == 3)
                    {
                        spriteBatch.DrawString(font, "Player Location: {X:" + player1.getPlayerLocation().X + "} {Y: " + player1.getPlayerLocation().Y + "}", new Vector2(0, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - (20 * (i + 1))), Color.Yellow);
                    }
                    if (i == 4)
                    {
                        spriteBatch.DrawString(font, "FPS: " + fps, new Vector2(0, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - (20 * (i + 1))), Color.Turquoise);
                    }
                    if (i == 5)
                    {
                        spriteBatch.DrawString(font, "Total # of enemies: " + (enemySwarmAttachList.Count + enemyList.Count + enemySwarmAttachTESTList.Count), new Vector2(0, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - (20 * (i + 1))), Color.Red);
                    }
                    if (i == 6)
                    {
                        spriteBatch.DrawString(font, "Total # of bullets: " + worldBullets.Count, new Vector2(0, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - (20 * (i + 1))), Color.Red);
                    }
                    if (i == 7)
                    {
                        spriteBatch.DrawString(font, "Farthest enemy from player: " + maxEnemyDistanceFromPlayer, new Vector2(0, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - (20 * (i + 1))), Color.Red);
                    }
                    if(i == 8)
                    {
                        spriteBatch.DrawString(font, "Player 1 Rotation: " + player1.getRotation(), new Vector2(0, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - (20 * (i + 1))), Color.PapayaWhip);
                    }

                }

                spriteBatch.End();
            }
            #endregion

            base.Draw(gameTime);
        }
    }
}

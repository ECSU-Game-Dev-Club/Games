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

        //(Public AND Static so we can see it in other classes (EX: camera))
        //Player class with player variables and calculations
        Player player1;

        //Player1's gamepad
        GamePadState gamePad1;
        GamePadState gamePad1_OLDSTATE;

        //Simple 1x1 png texture to fill the drawn square
        Texture2D playerTexture;

        //KEYBOAD
        KeyboardState keyboard;
        KeyboardState keyboard_OLDSTATE;

        //Mouse input
        MouseState mouse;
        MouseState mouse_OLDSTATE;

        //Setting up a rectangle for the users screen size
        Rectangle ScreenSize;
        //Is the game in fullscreen mode
        bool isFullScreen = false;

        //Sets up camera class
        Camera camera;

        //GRAVITY STUFF - DELELTE ME WHEN LEVELS COME IN
        List<Gravity> gravityList = new List<Gravity>();              //DELETE ME WHEN LEVELS COME IN
        List<Rectangle> gravityRectangleList = new List<Rectangle>(); //DELETE ME WHEN LEVELS COME IN
        Texture2D gravityTexture;                                     //DELETE ME WHEN LEVELS COME IN
        float gravityWellRotation;                                    //DELETE ME WHEN LEVELS COME IN

        public SpaceGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            //Sets the buffer(res) to the hardware res/ and gives it to a rectangle
            //X
            graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            ScreenSize.Width = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            //Y
            graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            ScreenSize.Height = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
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
            player1 = new Player(500, 500, Services);

            //Initializing Camera
            camera = new Camera(GraphicsDevice.Viewport);

            //Initializing Mouse
            mouse = Mouse.GetState();

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

            //Sets the players texture to the white 1x1 png in Content
            playerTexture = Content.Load<Texture2D>("whiteTexture");

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

            //Gets both gamepad and keyboard states(what buttons are pressed)
            gamePad1 = GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular);
            keyboard = Keyboard.GetState();
            mouse = Mouse.GetState();

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
                gravityList.Add(new Gravity(mouse.X + camera.getCameraCenter().X, mouse.Y + camera.getCameraCenter().Y, 25000));
                gravityRectangleList.Add(new Rectangle(0, 0, 50, 50));
            }
            #endregion

            //Updates the player class and passes all inputs
            player1.update(gamePad1, gamePad1_OLDSTATE, keyboard, keyboard_OLDSTATE, gravityList);

            //Updates camera by passing the players rectangle and gametime
            camera.Update(player1.getPlayerLocation());

            //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
            // TODO: Add your update logic here
            //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

            //PREVIOUS FRAME GAMEPAD AND KEYBOARD STATES - Always put at bottom
            gamePad1_OLDSTATE = gamePad1;
            keyboard_OLDSTATE = keyboard;
            mouse_OLDSTATE = mouse;

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

            //Begins the sprite batch so we can draw things on the screen(USING CAMERA)
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, camera.transform);

            //Begins the sprite batch so we can draw things on the screen(DEFAULT)
            //###################
            //UNCOMMENT THIS OUT TO DRAW NORMALLY WITH THE CAMERA NOT FOLLOWING THE PLAYER
            //spriteBatch.Begin();
            //###################

            //Draws gravity wells
            #region"DELETE ME WHEN LEVELS COME IN"
            for (int i = 0; i < gravityList.Count(); i++)
            {
                spriteBatch.Draw(gravityTexture, gravityList[i].getGravityLocationVector(), gravityRectangleList[i], Color.White, gravityWellRotation, new Vector2(25, 25), 1.0f, SpriteEffects.None, 0);
            }
            #endregion

            //Draw everything in player class
            player1.Draw(spriteBatch);

            //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
            // DRAW EVERYTHING IN HERE!!!!!!!!!
            //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}

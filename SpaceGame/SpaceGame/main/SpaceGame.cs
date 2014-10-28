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

        //(Public AND Static so we can see it in other classes (EX: camera))
        //Player class with player variables and calculations
        Player player1;

        //Player1's gamepad
        GamePadState gamePad1;

        //Simple 1x1 png texture to fill the drawn square
        Texture2D playerTexture;

        //KEYBOAD
        KeyboardState keyboard;

        //Setting up a rectangle for the users screen size
        Rectangle ScreenSize;
        //Is the game in fullscreen mode
        bool isFullScreen = false;

        //Sets up camera class
        Camera camera;

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
            player1 = new Player(500, 500);

            //Initializing Camera
            camera = new Camera(GraphicsDevice.Viewport);

            //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
            // TODO: Add your initialization logic here
            //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

            //Teset delete me 

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
            //Gets both gamepad and keyboard states(what buttons are pressed)
            gamePad1 = GamePad.GetState(PlayerIndex.One);
            keyboard = Keyboard.GetState();

            // Allows the game to exit, by pressing back on gamepad OR escape on keyboard
            if (gamePad1.Buttons.Back == ButtonState.Pressed || keyboard.IsKeyDown(Keys.Escape))
            {
                this.Exit();
            }

            //If the user moves the left thumbstick(in any direction)
            if((gamePad1.ThumbSticks.Left.X != 0) || (gamePad1.ThumbSticks.Left.Y != 0))
            {
                //Pass the X and Y value to the thrust method in the Player class
                player1.setThrust(new Vector2(gamePad1.ThumbSticks.Left.X, gamePad1.ThumbSticks.Left.Y));
            }

            //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
            //When using keyboards 1 is too fast so I did .5
            float keyboardMaxValue = 0.5f;

            //If keyboard key 'Up' is pressed
            if(keyboard.IsKeyDown(Keys.Up))
            {
                //The user is pressing UP so the Y value is -keyboardMaxValue
                player1.setThrust(new Vector2(0, (-1) * keyboardMaxValue));
            }
            //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
            //If keyboard key 'Down' is pressed
            if (keyboard.IsKeyDown(Keys.Down))
            {
                //The user is pressing DOWN so the Y value is +keyboardMaxValue
                player1.setThrust(new Vector2(0, keyboardMaxValue));
            }
            //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
            //If keyboard key 'Left' is pressed
            if (keyboard.IsKeyDown(Keys.Left))
            {
                //The user is pressing LEFT so the X value is -keyboardMaxValue
                player1.setThrust(new Vector2((-1) * keyboardMaxValue, 0));
            }
            //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
            //If keyboard key 'Right' is pressed
            if (keyboard.IsKeyDown(Keys.Right))
            {
                //The user is pressing RIGHT so the X value is +keyboardMaxValue
                player1.setThrust(new Vector2(keyboardMaxValue, 0));
            }

            //Updates the player class
            player1.update(gameTime);

            //Updates camera by passing the players rectangle and gametime
            camera.Update(player1.getPlayerRectangle());
            
            //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
            // TODO: Add your update logic here
            //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.PapayaWhip);

            //Begins the sprite batch so we can draw things on the screen
            //spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, camera.transform);

            //Begins the sprite batch so we can draw things on the screen
            spriteBatch.Begin();

            //Drawing the player here, (the texture of the player, the vector of the player, the rectangle of the player, the color is black (0.0f - 1.0f for transparency)
            spriteBatch.Draw(playerTexture, player1.getPlayerVelocityVector(), player1.getPlayerRectangle(), Color.Black * 1f);

            //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
            // DRAW EVERYTHING IN HERE!!!!!!!!!
            //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}

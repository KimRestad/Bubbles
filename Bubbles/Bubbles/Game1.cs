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


namespace Bubbles
{
    /// <summary>
    /// The possible states for the game.
    /// </summary>
    public enum GameState
    {
        Start,
        InGame,
        Highscore,
        End
    }

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        // Graphics variables.
        private GraphicsDeviceManager mGraphics;
        private SpriteBatch mSpriteBatch;
        private Color mBGColour;

        // Gamestate variables.
        private GameState mCurrentState;
        private StartScreen mStartScreen;
        private GameScreen mGameScreen;
        private HighscoreScreen mHSScreen;
        private EndScreen mEndScreen;
        
        // State variables.
        private KeyboardState mPrevKS;

        public Game1()
        {
            mGraphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            mBGColour = new Color(20, 140, 120);

            // Minimum: Width ~1229; Height ~691
            mGraphics.PreferredBackBufferWidth = 1280;
            mGraphics.PreferredBackBufferHeight = 768;
            mGraphics.ApplyChanges();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // Initialise the core and button classes.
            Core.Initialize(this);
            Button.Initialize();

            // Create the different screens and initialise the game state.
            mStartScreen = new StartScreen();
            mGameScreen = new GameScreen();
            mHSScreen = new HighscoreScreen();
            mEndScreen = new EndScreen();

            mCurrentState = GameState.Start;

            IsMouseVisible = true;
            
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            mSpriteBatch = new SpriteBatch(GraphicsDevice);
            
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // If the game is not the active application do not update (but keep drawing)
            if (!IsActive)
            {
                // If the state is in game, pause the game.
                if(mCurrentState == GameState.InGame)
                    mGameScreen.Pause();
                
                return;
            }

            // Global keys, Ctrl + Enter toggles full screen
            KeyboardState currKS = Keyboard.GetState();

            if ((currKS.IsKeyDown(Keys.LeftControl) || currKS.IsKeyDown(Keys.RightControl)) &&
                currKS.IsKeyDown(Keys.Enter) && mPrevKS.IsKeyUp(Keys.Enter))
                mGraphics.ToggleFullScreen();
            
            mPrevKS = currKS;

            // If the game is not paused, update the correct screen based on the game state.
            
            switch (mCurrentState)
            {
                case GameState.Start:
                    mStartScreen.Update();
                    break;
                case GameState.InGame:
                    mGameScreen.Update(gameTime);
                    break;
                case GameState.Highscore:
                    mHSScreen.Update();
                    break;
                case GameState.End:
                    mEndScreen.Update();
                    break;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(mBGColour);

            mSpriteBatch.Begin();
            // Based on the game state, draw the correct screen.
            switch (mCurrentState)
            {
                case GameState.Start:
                    mStartScreen.Draw(mSpriteBatch);
                    break;
                case GameState.InGame:
                    mGameScreen.Draw(mSpriteBatch);
                    break;
                case GameState.Highscore:
                    mHSScreen.Draw(mSpriteBatch);
                    break;
                case GameState.End:
                    mEndScreen.Draw(mSpriteBatch);
                    break;
            }

            mSpriteBatch.End();

            base.Draw(gameTime);
        }

        public bool Fullscreen
        {
            get { return mGraphics.IsFullScreen; }
            set
            {
                if (mGraphics.IsFullScreen != value)
                    mGraphics.ToggleFullScreen();
            }
        }

        public GameWindow GameWindow
        {
            get { return Window; }
        }

        public StartScreen StartScreen
        {
            get { return mStartScreen; }
        }

        public GameScreen GameScreen
        {
            get { return mGameScreen; }
        }

        public HighscoreScreen HighscoreScreen
        {
            get { return mHSScreen; }
        }

        public EndScreen EndScreen
        {
            get { return mEndScreen; }
        }

        public Vector2 UserResolution
        {
            get
            {
                return new Vector2(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width,
                                   GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height);
            }
        }

        public GameState GameState
        {
            get { return mCurrentState; }
            set { mCurrentState = value; }
        }
    }
}

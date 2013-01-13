using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace Bubbles
{
    /// <summary>
    /// The possible difficulties.
    /// </summary>
    public enum Difficulty
    {
        Easy, Normal, Hard
    }

    /// <summary>
    /// The possible levels.
    /// </summary>
    public enum Level
    {
        Deca, Hecto, Kilo, Mega, Giga, Tera, All
    }

    public class GameScreen
    {
        // Game information.
        private Texture2D mPanelBG;
        private Texture2D mChalkBoard;
        private Texture2D mProgressBar;
        private SpriteFont mChalkFont;
        private Rectangle mChalkBoardPosition;
        private Rectangle mProgressPosition;
        private Vector2 mInfoPosition;
        private Vector2 mScorePosition;
        private string mChalkText;
        private Button mBtnMenu;

        // Gameplay objects.
        private Board mBoard;
        private Aim mAim;
        private bool mSingleLevelGame;
        private Level mCurrentLevel;
        private Difficulty mCurrentDiff;
        
        // State variables.
        private KeyboardState mPrevKeyboard;
        private NextLevelScreen mNextLvlScreen;
        private PauseScreen mPauseScreen;
        private bool mIsPaused;
        private Rectangle mBounds;

        /// <summary>
        /// Create the game screen and its contents.
        /// </summary>
        public GameScreen()
        {
            // Load textures and fonts.
            mPanelBG = Core.Content.Load<Texture2D>(@"Textures\bricks");
            mChalkBoard = Core.Content.Load<Texture2D>(@"Textures\chalkBoard");
            mProgressBar = Core.Content.Load<Texture2D>(@"Textures\wallTop");
            mChalkFont = Core.Content.Load<SpriteFont>(@"Fonts\chalk");

            // Set up state variables.
            mBounds = new Rectangle(0, 0, Core.ClientBounds.Width - 400, Core.ClientBounds.Height);
            mNextLvlScreen = new NextLevelScreen();
            mPauseScreen = new PauseScreen();
            UnPause();

            // Set up panel (chalk board, progress bar and button).
            int padding = 30;
            int boardwidth = (Core.ClientBounds.Width - (mBounds.X + mBounds.Width)) - padding * 2;
            int boardheight = (int)(mChalkBoard.Height / ((float)mChalkBoard.Width / boardwidth));
            int textHeight = (int)mChalkFont.MeasureString("Score").Y;
            mChalkBoardPosition = new Rectangle(mBounds.X + mBounds.Width + padding, 100, boardwidth, boardheight);
            mProgressPosition = new Rectangle(mChalkBoardPosition.X, 500, 200, 50);
            mInfoPosition = new Vector2(mChalkBoardPosition.X + padding, mChalkBoardPosition.Y + padding);
            mScorePosition = new Vector2(mChalkBoardPosition.X + padding, 
                                         mChalkBoardPosition.Y + mChalkBoardPosition.Height - padding - textHeight);

            mBtnMenu = new Button(BtnMenuClick, new Rectangle(mChalkBoardPosition.X, 660, mChalkBoardPosition.Width, 64), "Return to menu");
        }

        /// <summary>
        /// Initialise the game. Start a new game with the specified difficulty, level and start score.
        /// </summary>
        /// <param name="levelDifficulty">The difficulty to play.</param>
        /// <param name="levelToPlay">The level to play.</param>
        /// <param name="startScore">Optional. Starting score.</param>
        public void StartGame(Difficulty levelDifficulty, Level levelToPlay, int startScore = 0)
        {
            // Save information.
            LevelDetails levelSettings;
            mCurrentDiff = levelDifficulty;

            if (levelToPlay == Level.All)
            {
                mCurrentLevel = Level.Deca;
                mSingleLevelGame = false;
            }
            else
            {
                mCurrentLevel = levelToPlay;
                if(startScore == 0)
                    mSingleLevelGame = true;
            }

            switch (mCurrentDiff)
            {
                case Difficulty.Easy:
                    levelSettings = LevelInformation.Easy(mCurrentLevel);
                    break;
                case Difficulty.Normal:
                    levelSettings = LevelInformation.Normal(mCurrentLevel);
                    break;
                case Difficulty.Hard:
                    levelSettings = LevelInformation.Hard(mCurrentLevel);
                    break;
                default:
                    return;
            }

            // Set information text.
            mChalkText = "Difficulty: " + mCurrentDiff.ToString() + "\nLevel: " + mCurrentLevel.ToString();

            // Initialise game objects.
            Ball.Initialise(levelSettings.NumColours, levelSettings.BallSize);
            mBoard = new Board(mBounds, startScore);

            for (int i = 0; i < levelSettings.NumRowsStart; ++i)
            {
                mBoard.AddRowTop();
            }

            mAim = new Aim(ref mBoard);

            Core.IsMouseVisible = false;
        }

        /// <summary>
        /// Update the game screen.
        /// </summary>
        /// <param name="gameTime">Elapsed time.</param>
        public void Update(GameTime gameTime)
        {
            // If the next level screen is showing, only update that.
            if (mNextLvlScreen.Visible)
            {
                mNextLvlScreen.Update();
                return;
            }

            HandleInput();
            mAim.Update(gameTime, ref mBoard);

            // If there are no balls left on the board, the level is finished.
            if (mBoard.BallsLeft <= 0)
            {
                if (mSingleLevelGame)       // If it was a single level game, go to end screen.
                    Core.EndGame(mBoard.Score, true);
                else                        // Else, go to next level unless on the last level.
                {
                    mCurrentLevel++;
                    if (mCurrentLevel == Level.All)
                        Core.EndGame(mBoard.Score, true);
                    else
                    {
                        mNextLvlScreen.Show(mCurrentLevel);
                        StartGame(mCurrentDiff, mCurrentLevel, mBoard.Score);
                        Core.IsMouseVisible = true;
                    }
                }
            }

            // If the game is over, end the game-
            if (mBoard.HasLost)
                Core.EndGame(mBoard.Score, false);

            // Update board, button and progressbar width.
            mBoard.Update(gameTime);
            mBtnMenu.Update();

            mProgressPosition.Width = (int)(mChalkBoardPosition.Width * mBoard.AddRowTime);
        }

        /// <summary>
        /// Draw the screen and its contents.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to use when drawing the screen.</param>
        public void Draw(SpriteBatch spritebatch)
        {
            // Draw the panel.
            spritebatch.Draw(mPanelBG, new Vector2(mBounds.X + mBounds.Width, 0), Color.White);
            spritebatch.Draw(mChalkBoard, mChalkBoardPosition, Color.White);
            spritebatch.Draw(mProgressBar, new Rectangle(mProgressPosition.X, mProgressPosition.Y, mChalkBoardPosition.Width, mProgressPosition.Height),
                             Color.Black * 0.7f);
            spritebatch.Draw(mProgressBar, mProgressPosition, Color.White);

            // Draw the board and aim.
            mBoard.Draw(spritebatch);
            mAim.Draw(spritebatch);

            // Draw game information.
            string scoreText = mChalkText;
            spritebatch.DrawString(mChalkFont, scoreText, mInfoPosition, Color.LightBlue, 0.0f, Vector2.Zero, 0.8f, SpriteEffects.None, 0.0f);
            spritebatch.DrawString(mChalkFont, "Score: " + mBoard.Score, mScorePosition, Color.White);

            // Draw button and pause screen or next level screen.
            mBtnMenu.Draw(spritebatch);

            if (mIsPaused)
                mPauseScreen.Draw(spritebatch);
            else
                mNextLvlScreen.Draw(spritebatch);
        }

        /// <summary>
        /// Handle mouse and keyboard input.
        /// </summary>
        private void HandleInput()
        {
            KeyboardState currKeyboard = Keyboard.GetState();

            MouseState ms = Mouse.GetState();
            Rectangle mouseRect = new Rectangle(ms.X, ms.Y, 1, 1);
            
            // If the mouse is hovering the board, it is invisible, if not it is visible.
            if (mBounds.Intersects(mouseRect))
                Core.IsMouseVisible = false;
            else
                Core.IsMouseVisible = true;

            // If unpause game when right button is pressed.
            if (ms.RightButton == ButtonState.Pressed)
                mIsPaused = false;

            // If space is pressed - move the mouse pointer to the middle of the chalk board to make it easier to find.
            if (currKeyboard.IsKeyDown(Keys.Space) && mPrevKeyboard.IsKeyUp(Keys.Space))
                Mouse.SetPosition(mChalkBoardPosition.X + (int)(mChalkBoardPosition.Width * 0.5f),
                                  mChalkBoardPosition.Y + (int)(mChalkBoardPosition.Height * 0.5f));
            // If the S or F8 key is pressed, toggle sound.
            else if ((currKeyboard.IsKeyDown(Keys.S) && mPrevKeyboard.IsKeyUp(Keys.S)) ||
                (currKeyboard.IsKeyDown(Keys.F8) && mPrevKeyboard.IsKeyUp(Keys.F8)))
                mBoard.ToggleSound();
            // If the F1 key is pressed, toggle whether to show helper shapes.
            else if (currKeyboard.IsKeyDown(Keys.F1) && mPrevKeyboard.IsKeyUp(Keys.F1))
                Core.ShowShapes = !Core.ShowShapes;
            // If the F2 key is pressed, toggle whether to show the helper aim.
            else if (currKeyboard.IsKeyDown(Keys.F2) && mPrevKeyboard.IsKeyUp(Keys.F2))
                Core.ShowLongAim = !Core.ShowLongAim;
            // If the P key is pressed, pause the game.
            else if (currKeyboard.IsKeyDown(Keys.P) && mPrevKeyboard.IsKeyUp(Keys.P))
                Pause();

            mPrevKeyboard = currKeyboard;
        }

        /// <summary>
        /// Pause the game (only if it is in the game).
        /// </summary>
        public void Pause()
        {
            mIsPaused = true;
            mPauseScreen.Visible = true;
        }

        /// <summary>
        /// Unpause the game.
        /// </summary>
        private void UnPause()
        {
            mIsPaused = false;
            mPauseScreen.Visible = false;
        }

        /// <summary>
        /// Menu button click event. Return to the menu.
        /// </summary>
        private void BtnMenuClick()
        {
            Core.ReturnToMenu();
        }

        /// <summary>
        /// Read only. Returns the currently chosen difficulty.
        /// </summary>
        public Difficulty Difficulty
        {
            get { return mCurrentDiff; }
        }

        /// <summary>
        /// Read only. Returns the currently chosen level.
        /// </summary>
        public Level Level
        {
            get { return mCurrentLevel; }
        }
    }
}

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
    public enum Difficulty
    {
        Easy, Normal, Hard
    }

    public enum Level
    {
        Deca, Hecto, Kilo, Mega, Giga, Tera, All
    }

    public class GameScreen
    {
        // Game information
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

        // Gameplay objects
        private Board mBoard;
        private Aim mAim;
        private bool mSingleLevelGame;
        private Level mCurrentLevel;
        private Difficulty mCurrentDiff;
        
        // State variables
        private KeyboardState mPrevKeyboard;
        private NextLevelScreen mNextLvlScreen;
        private Rectangle mBounds;

        public GameScreen()
        {
            // Load textures and fonts
            mPanelBG = Core.Content.Load<Texture2D>(@"Textures\bricks");
            mChalkBoard = Core.Content.Load<Texture2D>(@"Textures\chalkBoard");
            mProgressBar = Core.Content.Load<Texture2D>(@"Textures\wallTop");
            mChalkFont = Core.Content.Load<SpriteFont>(@"Fonts\chalk");

            // Set up state variables
            mBounds = new Rectangle(0, 0, Core.ClientBounds.Width - 400, Core.ClientBounds.Height);
            mNextLvlScreen = new NextLevelScreen();

            // Set up panel.
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

        public void StartGame(Difficulty levelDifficulty, Level levelToPlay, int startScore = 0)
        {
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

            mChalkText = "Difficulty: " + mCurrentDiff.ToString() + "\nLevel: " + mCurrentLevel.ToString();

            Ball.Initialize(levelSettings.NumColours, levelSettings.BallSize);
            mBoard = new Board(mBounds, startScore);

            for (int i = 0; i < levelSettings.NumRowsStart; ++i)
            {
                mBoard.AddRowTop();
            }

            mAim = new Aim(ref mBoard);

            Core.IsMouseVisible = false;
        }

        public void Update(GameTime gameTime)
        {
            if (mNextLvlScreen.Visible)
            {
                mNextLvlScreen.Update();
                return;
            }

            HandleInput();
            mAim.Update(gameTime, ref mBoard);

            // If there are no balls left on the board, the level is finished
            if (mBoard.BallsLeft <= 0)
            {
                if (mSingleLevelGame)       // If it was a single level game, go to end screen
                    Core.EndGame(mBoard.Score, true);
                else                        // Else, go to next level unless on the last level
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

            if (mBoard.HasLost)
                Core.EndGame(mBoard.Score, false);

            mBoard.Update();
            mBtnMenu.Update();

            mProgressPosition.Width = (int)(mChalkBoardPosition.Width * mBoard.AddRowTime);
        }

        public void Draw(SpriteBatch spritebatch)
        {
            spritebatch.Draw(mPanelBG, new Vector2(mBounds.X + mBounds.Width, 0), Color.White);
            spritebatch.Draw(mChalkBoard, mChalkBoardPosition, Color.White);
            spritebatch.Draw(mProgressBar, new Rectangle(mProgressPosition.X, mProgressPosition.Y, mChalkBoardPosition.Width, mProgressPosition.Height),
                             Color.Black * 0.7f);
            spritebatch.Draw(mProgressBar, mProgressPosition, Color.White);
            mBoard.Draw(spritebatch);
            mAim.Draw(spritebatch);

            string scoreText = mChalkText;
            spritebatch.DrawString(mChalkFont, scoreText, mInfoPosition, Color.LightBlue, 0.0f, Vector2.Zero, 0.8f, SpriteEffects.None, 0.0f);
            spritebatch.DrawString(mChalkFont, "Score: " + mBoard.Score, mScorePosition, Color.White);

            mBtnMenu.Draw(spritebatch);

            mNextLvlScreen.Draw(spritebatch);
        }

        private void HandleInput()
        {
            KeyboardState currKeyboard = Keyboard.GetState();

            MouseState ms = Mouse.GetState();
            Rectangle mouseRect = new Rectangle(ms.X, ms.Y, 1, 1);
            
            if (mBounds.Intersects(mouseRect))
                Core.IsMouseVisible = false;
            else
                Core.IsMouseVisible = true;

            if (currKeyboard.IsKeyDown(Keys.Space) && mPrevKeyboard.IsKeyUp(Keys.Space))
                Mouse.SetPosition(mChalkBoardPosition.X + (int)(mChalkBoardPosition.Width * 0.5f),
                                  mChalkBoardPosition.Y + (int)(mChalkBoardPosition.Height * 0.5f));
            else if ((currKeyboard.IsKeyDown(Keys.S) && mPrevKeyboard.IsKeyUp(Keys.S)) ||
                (currKeyboard.IsKeyDown(Keys.F8) && mPrevKeyboard.IsKeyUp(Keys.F8)))
                mBoard.ToggleSound();
            else if (currKeyboard.IsKeyDown(Keys.F1) && mPrevKeyboard.IsKeyUp(Keys.F1))
                Ball.ShowShapes = !Ball.ShowShapes;

            mPrevKeyboard = currKeyboard;
        }

        private void BtnMenuClick()
        {
            Core.ReturnToMenu();
        }

        public Difficulty Difficulty
        {
            get { return mCurrentDiff; }
        }

        public Level Level
        {
            get { return mCurrentLevel; }
        }
    }
}

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
        // Draw
        private Texture2D mBackground;
        private Rectangle mBGPosition;
        private Rectangle mBounds;

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

        
        private KeyboardState mPrevKeyboard;
        
        // DEBUG
        private string mDebugString = "";
        private SpriteFont mDebugFont;

        // Constants
        private const int C_TOP_OFFSET = 0;

        public GameScreen()
        {
            mBackground = Core.Content.Load<Texture2D>(@"Textures\background2");
            mBGPosition = new Rectangle(0, 0, Core.ClientBounds.Width, Core.ClientBounds.Height);
            mBounds = new Rectangle(0, C_TOP_OFFSET, Core.ClientBounds.Width - 400, Core.ClientBounds.Height - C_TOP_OFFSET);
            //mBounds = new Rectangle(0, C_TOP_OFFSET, 767, 512);

            mPanelBG = Core.Content.Load<Texture2D>(@"Textures\bricks");
            mChalkBoard = Core.Content.Load<Texture2D>(@"Textures\chalkBoard");
            mProgressBar = Core.Content.Load<Texture2D>(@"Textures\wallTop");
            mChalkFont = Core.Content.Load<SpriteFont>(@"Fonts\chalk");

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

            // DEBUG
            mDebugFont = Core.Content.Load<SpriteFont>(@"Fonts\default");
            mDebugString = "Keys: 'F2' starts a new game, 'A' adds a new row of balls and 'ESC' quits. AddRowTime: ";
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
            HandleInput();

            mProgressPosition.Width = (int)(mChalkBoardPosition.Width * mBoard.AddRowTime);

            #region DebugCode
            //if (currKeyboard.IsKeyDown(Keys.F1) && mPrevKeyboard.IsKeyUp(Keys.F1))
            //{
            //    List<Point> neighbours = mBoard.GetNeighbours(new Point(0, 0));

            //    foreach (Point pos in neighbours)
            //    {
            //        mBoard.mBalls[pos.Y].Row[pos.X].mColour = BallColour.Red;
            //    }
            //}

            //if (currKeyboard.IsKeyDown(Keys.D0) && mPrevKeyboard.IsKeyUp(Keys.D0))
            //    HandleDigitPress(0);
            //if (currKeyboard.IsKeyDown(Keys.D1) && mPrevKeyboard.IsKeyUp(Keys.D1))
            //    HandleDigitPress(1);
            //if (currKeyboard.IsKeyDown(Keys.D2) && mPrevKeyboard.IsKeyUp(Keys.D2))
            //    HandleDigitPress(2);
            //if (currKeyboard.IsKeyDown(Keys.D3) && mPrevKeyboard.IsKeyUp(Keys.D3))
            //    HandleDigitPress(3);
            //if (currKeyboard.IsKeyDown(Keys.D4) && mPrevKeyboard.IsKeyUp(Keys.D4))
            //    HandleDigitPress(4);
            //if (currKeyboard.IsKeyDown(Keys.D5) && mPrevKeyboard.IsKeyUp(Keys.D5))
            //    HandleDigitPress(5);
            //if (currKeyboard.IsKeyDown(Keys.D6) && mPrevKeyboard.IsKeyUp(Keys.D6))
            //    HandleDigitPress(6);
            //if (currKeyboard.IsKeyDown(Keys.D7) && mPrevKeyboard.IsKeyUp(Keys.D7))
            //    HandleDigitPress(7);
            //if (currKeyboard.IsKeyDown(Keys.D8) && mPrevKeyboard.IsKeyUp(Keys.D8))
            //    HandleDigitPress(8);
            //if (currKeyboard.IsKeyDown(Keys.D9) && mPrevKeyboard.IsKeyUp(Keys.D9))
            //    HandleDigitPress(9);
            #endregion DebugCode
            
            mAim.Update(gameTime, ref mBoard);

            // If there are no balls left on the board, the level is finished
            if (mBoard.BallsLeft <= 0)
            {
                if (mSingleLevelGame)       // If it was a single level game, go to end screen
                    Core.EndGame(mBoard.Score, true);
                else                        // Else, go to next level unless on the last level
                {
                    mCurrentLevel++;
                    if(mCurrentLevel == Level.All)
                        Core.EndGame(mBoard.Score, true);
                    else
                        StartGame(mCurrentDiff, mCurrentLevel, mBoard.Score);
                }
            }

            if (mBoard.HasLost)
                Core.EndGame(mBoard.Score, false);

            mBtnMenu.Update();
        }

        // DEBUG
        //private void HandleDigitPress(int digit)
        //{
        //    Point currPoint = new Point(digit, 2);

        //    if (mBoard.HasBall(currPoint))
        //    {
        //        List<Point> danglingBalls = new List<Point>();

        //        if (mBoard.GetConnectorList(currPoint, ref danglingBalls))
        //            mDebugString = "True";
        //        else
        //            mDebugString = "False";

        //        foreach (Point cell in danglingBalls)
        //        {
        //            //if(cell != currPoint)
        //            //    mBoard.mBalls[cell.Y].Row[cell.X].Colour = BallColour.Yellow;
        //            mBoard.mBalls[cell.Y].Row[cell.X] = null;
        //        }

        //        //Ball currBall = mBoard.mBalls[currPoint.Y].Row[currPoint.X];

        //        //if (currBall.Colour == BallColour.Red)
        //        //    currBall.Colour = BallColour.Blue;
        //        //else
        //        //    currBall.Colour = BallColour.Red;
        //    }
        //}

        public void Draw(SpriteBatch spritebatch)
        {
            //spritebatch.Draw(mBackground, mBounds, Color.White);
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
        }

        private void HandleInput()
        {
            KeyboardState currKeyboard = Keyboard.GetState();

            MouseState ms = Mouse.GetState();
            Rectangle mouseRect = new Rectangle(ms.X, ms.Y, 1, 1);
            
            if (mBoard.InnerBounds.Intersects(mouseRect)) // TODO: Include board walls
                Core.IsMouseVisible = false;
            else
                Core.IsMouseVisible = true;

            // DEBUG
            if (currKeyboard.IsKeyDown(Keys.A) && mPrevKeyboard.IsKeyUp(Keys.A))
                mBoard.AddRowTop();

            if (currKeyboard.IsKeyDown(Keys.Space) && mPrevKeyboard.IsKeyUp(Keys.Space))
                Mouse.SetPosition(mChalkBoardPosition.X + (int)(mChalkBoardPosition.Width * 0.5f),
                                  mChalkBoardPosition.Y + (int)(mChalkBoardPosition.Height * 0.5f));

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

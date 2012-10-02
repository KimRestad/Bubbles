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
    public class GameScreen
    {
        private Texture2D mBackground;
        private Rectangle mBGPosition;
        private Rectangle mBounds;

        private Texture2D mPanelBG;
        private Texture2D mChalkBoard;
        private Texture2D mProgressBar;
        private SpriteFont mChalkFont;
        private Vector2 mScorePosition;
        private Rectangle mProgressPosition;

        private Board mBoard;
        private Aim mAim;

        // DEBUG
        private KeyboardState mPrevKeyboard;
        private string mDebugString = "";
        private SpriteFont mDebugFont;

        // Constants
        private const int C_TOP_OFFSET = 0;

        public GameScreen()
        {
            mBackground = Core.Content.Load<Texture2D>(@"Textures\background2");
            mBGPosition = new Rectangle(0, 0, Core.ClientBounds.Width, Core.ClientBounds.Height);
            mBounds = new Rectangle(0, C_TOP_OFFSET, Core.ClientBounds.Width - 500, Core.ClientBounds.Height - C_TOP_OFFSET);
            //mBounds = new Rectangle(0, C_TOP_OFFSET, 767, 512);

            mPanelBG = Core.Content.Load<Texture2D>(@"Textures\bricks");
            mChalkBoard = Core.Content.Load<Texture2D>(@"Textures\chalkBoard");
            mProgressBar = Core.Content.Load<Texture2D>(@"Textures\wallTop");
            mChalkFont = Core.Content.Load<SpriteFont>(@"Fonts\chalk");
            mScorePosition = new Vector2(mBounds.X + mBounds.Width + 90, 150);
            mProgressPosition = new Rectangle(mBounds.X + mBounds.Width + 30, 500, mChalkBoard.Width, 50);

            // DEBUG
            mDebugFont = Core.Content.Load<SpriteFont>(@"Fonts\default");
            mDebugString = "Keys: 'F2' starts a new game, 'A' adds a new row of balls and 'ESC' quits. AddRowTime: ";
        }

        public void StartGame()
        {
            Ball.Initialize(4);
            mBoard = new Board(mBounds);
            mBoard.AddRowTop();
            //mAim = new Aim(mBounds, mBoard.InnerBounds.X - mBounds.X);
            mAim = new Aim(ref mBoard);
            
        }

        public void Update(GameTime gameTime)
        {
            KeyboardState currKeyboard = Keyboard.GetState();

            if (currKeyboard.IsKeyDown(Keys.A) && mPrevKeyboard.IsKeyUp(Keys.A))
                mBoard.AddRowTop();

            if (currKeyboard.IsKeyDown(Keys.F2) && mPrevKeyboard.IsKeyUp(Keys.F2))
            {
                StartGame();
            }

            mPrevKeyboard = currKeyboard;

            mProgressPosition.Width = (int)(mChalkBoard.Width * mBoard.AddRowTime);

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

            if (mBoard.BallsLeft <= 0)
                Core.EndGame();
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
            spritebatch.Draw(mBackground, mBounds, Color.White);
            spritebatch.Draw(mPanelBG, new Vector2(mBounds.X + mBounds.Width, 0), Color.White);
            spritebatch.Draw(mChalkBoard, new Vector2(mBounds.X + mBounds.Width + 30, 100), Color.White);
            spritebatch.Draw(mProgressBar, mProgressPosition, Color.White);
            mBoard.DrawAll(spritebatch);
            mAim.Draw(spritebatch);

            string scoreText = "Score: " + mBoard.Score + "\n\n\nHigh Score: 3675";

            spritebatch.DrawString(mChalkFont, scoreText, mScorePosition, Color.White);

            // DEBUG
            spritebatch.DrawString(mDebugFont, mDebugString + mBoard.AddRowTime, Vector2.Zero, Color.White);

            for (int i = 0; i < (int)BallColour.Count; i++)
            {
                spritebatch.DrawString(mDebugFont, ((BallColour)i).ToString() + ": " + mBoard.mColoursInPlay[i], new Vector2(mScorePosition.X, 600 + 20 * i), Color.Red);
            }

            spritebatch.DrawString(mDebugFont, "Total: " + mBoard.BallsLeft, new Vector2(mScorePosition.X, 600 + 20 * (int)BallColour.Count), Color.Red);
        }
    }
}

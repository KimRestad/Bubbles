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

        private Board mBoard;
        private Aim mAim;

        // DEBUG
        private KeyboardState mPrevKeyboard;
        private string mDebugString = "";
        private SpriteFont mDebugFont;

        // Constants
        private const int C_TOP_OFFSET = 24;

        public GameScreen()
        {
            mBackground = Core.Content.Load<Texture2D>(@"Textures\background2");
            mBGPosition = new Rectangle(0, 0, Core.ClientBounds.Width, Core.ClientBounds.Height);
            mBounds = new Rectangle(0, C_TOP_OFFSET, Core.ClientBounds.Width, Core.ClientBounds.Height - C_TOP_OFFSET);

            // DEBUG
            mDebugFont = Core.Content.Load<SpriteFont>(@"Fonts\default");
        }

        public void StartGame()
        {
            Ball.Initialize(9, mBounds);
            mAim = new Aim(mBounds);

            mBoard = new Board(mBounds);
        }

        public void Update(GameTime gameTime)
        {
            KeyboardState currKeyboard = Keyboard.GetState();

            if (currKeyboard.IsKeyDown(Keys.A) && mPrevKeyboard.IsKeyUp(Keys.A))
                mBoard.AddRowTop();

            //if (currKeyboard.IsKeyDown(Keys.F1) && mPrevKeyboard.IsKeyUp(Keys.F1))
            //{
            //    List<Point> neighbours = mBoard.GetNeighbours(new Point(0, 0));

            //    foreach (Point pos in neighbours)
            //    {
            //        mBoard.mBalls[pos.Y].Row[pos.X].mColour = BallColour.Red;
            //    }
            //}

            if (currKeyboard.IsKeyDown(Keys.F2) && mPrevKeyboard.IsKeyUp(Keys.F2))
            {
                StartGame();
            }

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

            mPrevKeyboard = currKeyboard;
            
            mAim.Update(gameTime, mBoard);
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
            mBoard.DrawAll(spritebatch);
            mAim.Draw(spritebatch);

            spritebatch.DrawString(mDebugFont, "F2 starts a new game, A adds a new row of balls.", Vector2.Zero, Color.White);
        }
    }
}

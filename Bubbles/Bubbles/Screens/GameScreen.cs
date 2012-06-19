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
        private Rectangle mBoard;

        private BallTable mBalls;
        private Aim mAim;

        // DEBUG
        private KeyboardState mPrevKeyboard;

        public GameScreen()
        {
            mBackground = Core.Content.Load<Texture2D>(@"Textures\background");
            mBGPosition = new Rectangle(0, 0, Core.ClientBounds.Width, Core.ClientBounds.Height);
            mBoard = new Rectangle(0, 0, Core.ClientBounds.Width - 32, Core.ClientBounds.Height);
        }

        public void StartGame()
        {
            Ball.InitializeTextures(9, mBoard);
            mAim = new Aim(mBoard);

            int row1 = (int)Math.Floor(mBoard.Width / Ball.Size.X);
            int row2 = (int)Math.Floor((mBoard.Width - Ball.Size.X * 0.5) / Ball.Size.X);
            
            // Calculate the offset based on which row is the longest, the length of that row and board width.
            // Subtract row length from board width and divide remainder by 2 to get a centered offset.
            float xOffset = mBoard.Width;
            xOffset -= (row1 > row2 ? row1 : row2 + 0.5f) * Ball.Size.X;
            xOffset *= 0.5f;

            mBalls = new BallTable(row1, row2, new Vector2(xOffset, 0));
        }

        private void AddRowOfBalls()
        {
            List<Ball> ballList = new List<Ball>();

            for (int i = 0; i < mBalls.RowSizeTop; ++i)
            {
                ballList.Add(new Ball((BallColour)Core.RandomGen.Next(Ball.NumColours), Vector2.Zero));
            }

            ballList[6] = null;
            mBalls.AddRowTop(ballList);
        }

        public void Update(GameTime gameTime)
        {
            KeyboardState currKeyboard = Keyboard.GetState();

            if (currKeyboard.IsKeyDown(Keys.A) && mPrevKeyboard.IsKeyUp(Keys.A))
                AddRowOfBalls();

            mPrevKeyboard = currKeyboard;
            
            mAim.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(mBackground, mBoard, Color.Red);
            mAim.Draw(spriteBatch);
            mBalls.DrawAll(spriteBatch);
        }
    }
}

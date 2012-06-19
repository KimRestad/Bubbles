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

        private Board mBalls;
        private Aim mAim;

        // DEBUG
        private KeyboardState mPrevKeyboard;

        public GameScreen()
        {
            mBackground = Core.Content.Load<Texture2D>(@"Textures\background");
            mBGPosition = new Rectangle(0, 0, Core.ClientBounds.Width, Core.ClientBounds.Height);
            mBoard = new Rectangle(0, 0, Core.ClientBounds.Width, Core.ClientBounds.Height);
        }

        public void StartGame()
        {
            Ball.InitializeTextures(9);
            mAim = new Aim(mBoard);

            mBalls = new Board(mBoard);
            Ball.InitializeBoard(mBalls, mBoard);
        }

        private void AddRowOfBalls()
        {
            List<Ball> ballList = new List<Ball>();

            for (int i = 0; i < mBalls.RowSizeTop; ++i)
            {
                ballList.Add(new Ball((BallColour)Core.RandomGen.Next(Ball.NumColours), Vector2.Zero));
            }

            ballList[5] = null;
            ballList[6] = null;
            ballList[7] = null;

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
            spriteBatch.Draw(mBackground, mBoard, Color.PapayaWhip);
            mBalls.DrawAll(spriteBatch);
            mAim.Draw(spriteBatch);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Bubbles
{
    class Aim
    {
        private const float C_MIN_DIR_Y = 12.0f;

        private Texture2D mTexture;
        private Vector2 mPosition;
        private Vector2 mOrigin;            // This point, relative to the position, is moved to be drawn at position
        private float mRotation;

        private Ball mShot;
        private Ball mNextShot;
        private Vector2 mNextShotPos;
        private List<Ball> mShotBalls;

        private MouseState mMousePrev;

        public Aim(Rectangle board, int wallThickness)
        {
            mTexture = Core.Content.Load<Texture2D>(@"Textures\aim");
            mPosition = new Vector2(board.Left + board.Width * 0.5f, board.Top + board.Height - Ball.Size.X * 0.5f);
            mOrigin = new Vector2(0, mTexture.Height * 0.5f);
            mRotation = 0.0f;

            mNextShotPos = new Vector2(board.Left + board.Width - Ball.Size.X * 0.5f - wallThickness, mPosition.Y);
            mShot = CreateRandomBall(mPosition);
            mNextShot = CreateRandomBall(mNextShotPos);
            mShotBalls = new List<Ball>();

            mMousePrev = Mouse.GetState();
        }

        public void Update(GameTime gameTime, ref Board board)
        {
            MouseState mouseCurr = Mouse.GetState();

            Vector2 mousePos = new Vector2(mouseCurr.X, mouseCurr.Y);
            Vector2 direction = mousePos - mPosition;

            if (direction.Y > -C_MIN_DIR_Y)
                direction.Y = -C_MIN_DIR_Y;

            mRotation = (float)Math.Atan2((double)direction.Y, (double)direction.X);

            if (mouseCurr.LeftButton == ButtonState.Pressed && mMousePrev.LeftButton == ButtonState.Released)
            {
                mShot.Shoot(direction);

                mShotBalls.Add(mShot);
                mShot = mNextShot;
                mShot.Position = mPosition;
                mNextShot = CreateRandomBall(mNextShotPos);
            }

            mMousePrev = mouseCurr;

            mShot.Update(ref board);
            mNextShot.Update(ref board);

            for (int i = mShotBalls.Count - 1; i >= 0; --i)
            {
                if (mShotBalls[i].State == BallState.Still)
                    mShotBalls.RemoveAt(i);
                else
                    mShotBalls[i].Update(ref board);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(mTexture, mPosition, null, Color.White, mRotation, mOrigin, 1.0f, SpriteEffects.None, 0.0f);
            mShot.Draw(spriteBatch);
            mNextShot.Draw(spriteBatch);

            foreach (Ball ball in mShotBalls)
            {
                ball.Draw(spriteBatch);
            }
        }

        private Ball CreateRandomBall(Vector2 position)
        {
            return new Ball((BallColour)Core.RandomGen.Next(Ball.NumColours), position);
        }
    }
}

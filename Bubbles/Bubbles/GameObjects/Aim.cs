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

        // Graphics variables.
        private Texture2D mTexture;
        private Vector2 mPosition;
        private Vector2 mOrigin;            // This point in the source texture, is drawn at position
        private float mRotation;

        // Shots variables.
        private Ball mLoaded;
        private Ball mNextLoaded;
        private Vector2 mNextShotPos;
        private List<Ball> mShots;

        // State variables.
        private MouseState mMousePrev;

        /// <summary>
        /// Creates and initalises the aim. The aim is placed in the middle of the board, with a shot at the base of
        /// the aim and the following shot by the right wall of the board.
        /// </summary>
        /// <param name="board">The board the aim is created for.</param>
        public Aim(ref Board board)
        {
            // Load texture.
            mTexture = Core.Content.Load<Texture2D>(@"Textures\aim");

            // Sets the graphics variables.
            Rectangle boardBounds = board.InnerBounds;
            mPosition = new Vector2(boardBounds.Left + boardBounds.Width * 0.5f, 
                                    boardBounds.Top + boardBounds.Height - Ball.Size.X * 0.5f);
            mOrigin = new Vector2(0, mTexture.Height * 0.5f);
            mRotation = 0.0f;

            // Sets the shot variables.
            mNextShotPos = new Vector2(boardBounds.Left + boardBounds.Width - Ball.Size.X * 0.5f, mPosition.Y);
            mLoaded = CreateRandomBall(mPosition, ref board);
            mNextLoaded = CreateRandomBall(mNextShotPos, ref board);
            mShots = new List<Ball>();
        }

        /// <summary>
        /// Updates the aim to point to mouse pointer, shoot if left mouse button is clicked, reload if needed,
        /// and update its shots.
        /// </summary>
        /// <param name="gameTime">Elapsed game time.</param>
        /// <param name="board">The board the aim belongs to.</param>
        public void Update(GameTime gameTime, ref Board board)
        {
            // Get the mouse state and calculate the direction as the vector between the aim and mouse position.
            MouseState mouseCurr = Mouse.GetState();

            Vector2 mousePos = new Vector2(mouseCurr.X, mouseCurr.Y);
            Vector2 direction = mousePos - mPosition;

            // Clamp the direction y to be at least little larger than the aim's y position.
            if (direction.Y > -C_MIN_DIR_Y)
                direction.Y = -C_MIN_DIR_Y;

            // Calculate the rotation from the direction vector.
            mRotation = (float)Math.Atan2((double)direction.Y, (double)direction.X);

            // If the left mouse button was just pressed, shoot the ball load, next shot and load new next shot.
            if (mouseCurr.LeftButton == ButtonState.Pressed && mMousePrev.LeftButton == ButtonState.Released)
            {
                mLoaded.Shoot(direction);

                mShots.Add(mLoaded);
                mLoaded = mNextLoaded;
                mLoaded.Position = mPosition;
                mNextLoaded = CreateRandomBall(mNextShotPos, ref board);
            }

            mMousePrev = mouseCurr;

            // Update loaded balls.
            mLoaded.Update(board, gameTime);
            mNextLoaded.Update(board, gameTime);

            // Update shots and if they are stuck to the board (no longer in "Shot" state) - remove them from 
            // shot list.
            for (int i = mShots.Count - 1; i >= 0; --i)
            {
                if (mShots[i].State != BallState.Shot)
                    mShots.RemoveAt(i);
                else
                    mShots[i].Update(board, gameTime);
            }
        }

        /// <summary>
        /// Draws the aim, the current loaded ball, the next ball and all shot balls.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch used when drawing the ball.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw aim and its longer "shadow" (if it is specified).
            if (Core.ShowLongAim)
                spriteBatch.Draw(mTexture, mPosition, null, Color.White * 0.3f, mRotation, mOrigin,
                                 new Vector2(2.0f, Ball.Scale), SpriteEffects.None, 0.0f);

            spriteBatch.Draw(mTexture, mPosition, null, Color.White, mRotation, mOrigin,
                             new Vector2(1.0f, Ball.Scale * 1.2f), SpriteEffects.None, 0.0f);
            
            // Draw loaded and shot balls.
            mLoaded.Draw(spriteBatch);
            mNextLoaded.Draw(spriteBatch);

            foreach (Ball ball in mShots)
                ball.Draw(spriteBatch);
        }

        /// <summary>
        /// Create a new ball with a colour that is still in play.
        /// </summary>
        /// <param name="position">The position to place the ball at.</param>
        /// <param name="board">The board to use when finding valid colours.</param>
        /// <returns></returns>
        private Ball CreateRandomBall(Vector2 position, ref Board board)
        {
            return new Ball(board.GenerateColourInPlay(), position);
        }
    }
}

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

        public GameScreen()
        {
            mBackground = Core.Content.Load<Texture2D>(@"Textures\background");
            mBGPosition = new Rectangle(0, 0, Core.ClientBounds.Width, Core.ClientBounds.Height);
            mBounds = new Rectangle(0, 0, Core.ClientBounds.Width, Core.ClientBounds.Height);
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

            mPrevKeyboard = currKeyboard;
            
            mAim.Update(gameTime, mBoard);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(mBackground, mBounds, Color.PapayaWhip);
            mBoard.DrawAll(spriteBatch);
            mAim.Draw(spriteBatch);
        }
    }
}

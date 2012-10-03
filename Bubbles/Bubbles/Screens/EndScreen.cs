using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bubbles
{
    public class EndScreen
    {
        // High Score Board variables
        private Texture2D mHSBoard;
        private Texture2D mHSSign;
        private SpriteFont mHSTextFont;
        private Rectangle mHSBoardPosition;
        private Rectangle mHSSignPosition;
        private Vector2 mHSPosition;

        //
        private SpriteFont mTextFont;
        private Vector2 mTextPosition;
        private Color mTextColour;

        private int mScore;
        private bool mGameWasWon;

        public EndScreen()
        {
            mHSBoard = Core.Content.Load<Texture2D>(@"Textures/chalkBoard");
            //mHSSign = Core.Content.Load<Texture2D>(@"Textures/chalkBoard");
            mHSTextFont = Core.Content.Load<SpriteFont>(@"Fonts/chalk");
            float scale = 1.5f;
            mHSBoardPosition = new Rectangle((int)(Core.ClientBounds.Width - mHSBoard.Width * scale) - 20,
                100, (int)(mHSBoard.Width * scale), (int)(mHSBoard.Height * scale));

            //mHSSignPosition = new Rectangle(mHSBoardPosition.X + (int)((mHSBoardPosition.Width - mHSSign.Width) * 0.5),
            //    mHSBoardPosition.Y - mHSSign.Height, mHSSign.Width, mHSSign.Height); 

            Vector2 textOffset = new Vector2(40, 30);
            mHSPosition = new Vector2(mHSBoardPosition.X + textOffset.X, mHSBoardPosition.Y + textOffset.Y);

            // DEBUG - change the font
            mTextFont = Core.Content.Load<SpriteFont>(@"Fonts/default");
            mTextPosition = new Vector2(50, 200);
            mTextColour = Color.Turquoise;
        }

        public void SetInfo(int score, bool gameWasWon)
        {
            mScore = score;
            mGameWasWon = gameWasWon;
        }

        public void Update()
        {
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(mHSBoard, mHSBoardPosition, Color.White);

            float scale = 0.8f;
            int textheight = (int)(mHSTextFont.MeasureString("Highscore").Y * scale);
            for (int i = 0; i < 10; i++)
            {
                spriteBatch.DrawString(mHSTextFont, "HighScore", mHSPosition + new Vector2(0, i * textheight),
                    Color.White,0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
            }

            spriteBatch.DrawString(mTextFont, "Your score: " + mScore, mTextPosition, mTextColour);
        }
    }
}

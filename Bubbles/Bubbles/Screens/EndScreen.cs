using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bubbles
{
    public class EndScreen
    {
        private struct DrawText
        {
            public string Text;
            public Vector2 DrawPos;

            public DrawText(string text, Vector2 position)
            {
                Text = text;
                DrawPos = position;
            }
        }

        // Highscore board and sign
        private Texture2D mHSSign;
        private Texture2D mHSBoard;
        private SpriteFont mSignFont;
        private Rectangle mSignPos;
        private Rectangle mBoardPos;
        private Vector2 mBoardNamePos;
        private Vector2 mBoardScorePos;
        private DrawText mSignText;
        private HighscoreList mHighscores;

        // Score information
        private DrawText mWinningText;
        private DrawText mScorePresText;
        private DrawText mScoreText;
        private SpriteFont mTextFont;
        private Color mWinningColour;
        private Color mTextColour;
        private bool mGameWasWon;
        private float mScorePresScale = 0.8f;

        // Highscore information
        private Difficulty mDifficulty;
        private Level mLevel;

        // Other
        List<Button> mButtons;
        Texture2D mBackground;

        public EndScreen()
        {
            // Load chalk board textures and fonts.
            mHSBoard = Core.Content.Load<Texture2D>(@"Textures\chalkBoard");
            mHSSign = Core.Content.Load<Texture2D>(@"Textures\sign");
            mSignFont = Core.Content.Load<SpriteFont>(@"Fonts\button");
            mBackground = Core.Content.Load<Texture2D>(@"Textures\bricks");

            // Calculate chalk board, sign and text positions.
            float scale = 1.5f;
            mBoardPos = new Rectangle((int)(Core.ClientBounds.Width - mHSBoard.Width * scale) - 32,
                128, (int)(mHSBoard.Width * scale), (int)(mHSBoard.Height * scale));

            int height = 64;
            int padding = 16;
            mSignPos = new Rectangle(mBoardPos.X, mBoardPos.Y - height - padding,
                                            mBoardPos.Width, height);
            Vector2 textOffset = new Vector2(40, 30);
            mBoardNamePos = new Vector2(mBoardPos.X + textOffset.X, mBoardPos.Y + textOffset.Y);
            mBoardScorePos = new Vector2(mBoardPos.X +  mBoardPos.Width - textOffset.X, mBoardNamePos.Y);

            // Set the score information text variables.
            mTextFont = Core.Content.Load<SpriteFont>(@"Fonts/endText");
            mScorePresText.Text = "Final score:";
            mScorePresText.DrawPos = new Vector2((mBoardPos.X - 
                                                 mTextFont.MeasureString(mScorePresText.Text).X * 
                                                 mScorePresScale) * 0.5f, 220);
            mTextColour = Color.Black;

            // Calculate positions and create the buttons.
            int btnWidth = 384;
            int btnHeight = 64;
            int btnX = (int)((mBoardPos.X - btnWidth) * 0.5f);
            int btnBottomY = mBoardPos.Y + mBoardPos.Height - btnHeight;
            int btnPadding = btnHeight + 32;

            mButtons = new List<Button>();
            mButtons.Add(new Button(BtnMenuClick, new Rectangle(btnX, btnBottomY - btnPadding * 2, btnWidth, btnHeight),
                                    "Return to Menu"));
            mButtons.Add(new Button(BtnHighscoreClick, new Rectangle(btnX, btnBottomY - btnPadding, btnWidth, btnHeight), 
                                    "All Highscores"));
            mButtons.Add(new Button(BtnExitClick, new Rectangle(btnX, btnBottomY, btnWidth, btnHeight), "Exit Game"));
        }

        public void SetInfo(Difficulty difficulty, Level level, int score, bool gameWasWon)
        {
            Core.IsMouseVisible = true;

            mGameWasWon = gameWasWon;
            mDifficulty = difficulty;
            mLevel = level;

            // Set up winning presentation text.
            if (mGameWasWon)
            {
                mWinningColour = Color.Gold;
                mWinningText.Text = "YOU WON!!!";
            }
            else
            {
                mWinningColour = Color.Red;
                mWinningText.Text = "YOU LOST";
            }

            Vector2 textSize = mTextFont.MeasureString(mWinningText.Text);
            mWinningText.DrawPos = new Vector2((mBoardPos.X - textSize.X) * 0.5f,
                                               mScorePresText.DrawPos.Y - 32 - textSize.Y);

            // Set up score presentation text.
            mScoreText.Text = score.ToString();
            textSize = mTextFont.MeasureString(mScoreText.Text) * mScorePresScale;
            mScoreText.DrawPos = new Vector2((mBoardPos.X - textSize.X) * 0.5f, 
                                             mScorePresText.DrawPos.Y + textSize.Y - 12);
            
            // Set up sign text.
            mSignText.Text = mDifficulty.ToString() + ": " + mLevel.ToString();
            textSize = mSignFont.MeasureString(mSignText.Text);
            mSignText.DrawPos = new Vector2(mSignPos.X + (mSignPos.Width - textSize.X) * 0.5f,
                                       mSignPos.Y + (mSignPos.Height - textSize.Y) * 0.5f);

            mHighscores = new HighscoreList(mDifficulty, mLevel, gameWasWon ? score : -1);
            mHighscores.SaveToFile();
        }

        public void Update()
        {
            foreach (Button btn in mButtons)
                btn.Update();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int x = 0; x < Core.ClientBounds.Width; x += mBackground.Width)
            {
                spriteBatch.Draw(mBackground, new Vector2(x, 0), Color.White);
            }

            // Draw chalk board and sign.
            spriteBatch.Draw(mHSSign, mSignPos, Color.White);
            spriteBatch.Draw(mHSBoard, mBoardPos, Color.White);
            spriteBatch.DrawString(mSignFont, mSignText.Text, mSignText.DrawPos + new Vector2(1, 1), Color.White * 0.5f);
            spriteBatch.DrawString(mSignFont, mSignText.Text, mSignText.DrawPos, Color.Black * 0.85f);

            // Draw the high scores on the chalk board.
            mHighscores.Draw(spriteBatch, 0.8f, mBoardNamePos, mBoardScorePos);

            // Draw score text.
            spriteBatch.DrawString(mTextFont, mWinningText.Text, mWinningText.DrawPos + new Vector2(1, 1),
                                   Color.Black);
            spriteBatch.DrawString(mTextFont, mWinningText.Text, mWinningText.DrawPos, mWinningColour);
            spriteBatch.DrawString(mTextFont, mScorePresText.Text, mScorePresText.DrawPos + new Vector2(1, 1),
                                   Color.White, 0.0f, Vector2.Zero, mScorePresScale, SpriteEffects.None, 0.0f);
            spriteBatch.DrawString(mTextFont, mScorePresText.Text, mScorePresText.DrawPos, mTextColour,
                                   0.0f, Vector2.Zero, mScorePresScale, SpriteEffects.None, 0.0f);
            spriteBatch.DrawString(mTextFont, mScoreText.Text, mScoreText.DrawPos + new Vector2(1, 1),
                                   Color.White, 0.0f, Vector2.Zero, mScorePresScale, SpriteEffects.None, 0.0f);
            spriteBatch.DrawString(mTextFont, mScoreText.Text, mScoreText.DrawPos, mTextColour,
                                   0.0f, Vector2.Zero, mScorePresScale, SpriteEffects.None, 0.0f);

            // Draw the buttons.
            foreach (Button btn in mButtons)
                btn.Draw(spriteBatch);
        }

        private void BtnMenuClick()
        {
            Core.ReturnToMenu();
        }

        private void BtnHighscoreClick()
        {
            Core.ShowHighscore();
        }

        private void BtnExitClick()
        {
            Core.Exit();
        }
    }
}

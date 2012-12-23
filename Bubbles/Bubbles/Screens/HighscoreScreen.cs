using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Bubbles
{
    public class HighscoreScreen
    {
        private Texture2D mTexBoard;
        private Texture2D mTexSign;
        private SpriteFont mFontChalk;
        private SpriteFont mFontSign;

        private Rectangle mFocusBoardPosition;
        private bool mFocusVisible;
        private Level mFocusLvl;

        private Rectangle[] mBoardPositions;
        private int mSignYOffset;
        private int mSignHeight;
        private Vector2 mSignCenterOffset;
        private Vector2 mBoardCenterOffset;

        private List<Button> mButtons;
        private HighscoreList[,] mHighscores;
        private Difficulty mCurrDiff;
        private MouseState mPrevMS;

        public HighscoreScreen()
        {
            // Load textures and fonts.
            mTexBoard = Core.Content.Load<Texture2D>(@"Textures\chalkBoard");
            mTexSign = Core.Content.Load<Texture2D>(@"Textures\sign");
            mFontChalk = Core.Content.Load<SpriteFont>(@"Fonts\chalk");
            mFontSign = Core.Content.Load<SpriteFont>(@"Fonts\button");

            // Save positions for board, relative sign and relative text.
            int signPad = 8;
            mSignHeight = 40;
            mSignYOffset = -(mSignHeight + signPad);

            int height = (int)(Core.ClientBounds.Height * 0.7f);
            int width = (int)(((float)height / mTexBoard.Height) * mTexBoard.Width);
            mFocusBoardPosition = new Rectangle((int)((Core.ClientBounds.Width - width) * 0.5f),
                                                (int)((Core.ClientBounds.Height - height) * 0.5f),
                                                width, height);
           
            mBoardPositions = new Rectangle[7];
            Point padding = new Point(64, 80);
            width = (int)((Core.ClientBounds.Width - padding.X * 5) * 0.25f);
            height = (int)(((float)width / mTexBoard.Width) * mTexBoard.Height);
            int xPos = padding.X;
            int yPos = 128 - mSignYOffset;

            for (int lvl = 0; lvl < mBoardPositions.Length; ++lvl)
            {
                mBoardPositions[lvl] = new Rectangle(xPos, yPos, width, height);

                if (lvl == 3)
                {
                    xPos = padding.X + (int)((width + padding.X) * 0.5f);
                    yPos += height + padding.Y;
                }
                else
                    xPos += padding.X + width;

            }
            mSignCenterOffset = new Vector2(width * 0.5f, mSignHeight * 0.5f);
            mBoardCenterOffset = new Vector2(width * 0.5f, height * 0.5f);

            // Create buttons.
            mButtons = new List<Button>();
            int btnWidth = width;
            int btnHeight = 64;
            int btnPadding = 64;
            int btnX = (int)((Core.ClientBounds.Width - btnWidth) * 0.5f);
            int btnY = 16;

            mButtons.Add(new Button(BtnEasyClick, new Rectangle(btnX - (btnWidth + btnPadding),
                                                                btnY, btnWidth, btnHeight), "Easy"));
            mButtons.Add(new Button(BtnNormalClick, new Rectangle(btnX, btnY, 
                                                                btnWidth, btnHeight), "Normal"));
            mButtons.Add(new Button(BtnHardClick, new Rectangle(btnX + (btnWidth + btnPadding), btnY,
                                                                btnWidth, btnHeight), "Hard"));

            btnWidth = 384;
            btnX = (int)((Core.ClientBounds.Width - btnWidth) * 0.5f);
            mButtons.Add(new Button(BtnMenuReturnClick, new Rectangle(btnX,
                                                        Core.ClientBounds.Height - btnHeight - btnY,
                                                        btnWidth, btnHeight),
                                    "Return to menu"));
            CreateHighscoreLists();
            BtnEasyClick();
        }

        public void Update(GameTime gametime)
        {
            MouseState currMS = Mouse.GetState();

            if(currMS.LeftButton == ButtonState.Pressed && mPrevMS.LeftButton == ButtonState.Released)
            {
                if (mFocusVisible)
                    mFocusVisible = false;
                else
                {
                    Rectangle mouseRect = new Rectangle(currMS.X, currMS.Y, 1, 1);
                    for (int i = 0; i < mBoardPositions.Length; i++)
                    {
                        if (mBoardPositions[i].Intersects(mouseRect))
                        {
                            mFocusVisible = true;
                            mFocusLvl = (Level)i;
                            break;
                        }
                    }
                }
            }

            mPrevMS = currMS;

            foreach (Button btn in mButtons)
                btn.Update();
        }

        public void Draw(SpriteBatch spritebatch)
        {
            if (mFocusVisible)
            {
                Vector2 nameOffset = new Vector2(mFocusBoardPosition.X + 40, mFocusBoardPosition.Y + 30);
                Vector2 scoreOffset = new Vector2(mFocusBoardPosition.X + mFocusBoardPosition.Width - 40,
                                                  nameOffset.Y);

                spritebatch.Draw(mTexBoard, mFocusBoardPosition, Color.White);
                mHighscores[(int)mCurrDiff, (int)mFocusLvl].Draw(spritebatch, 0.8f, nameOffset, scoreOffset);
            }
            else
            {
                int i = 0;
                foreach (Rectangle pos in mBoardPositions)
                {
                    // Draw board with highest score.
                    string text = mHighscores[(int)mCurrDiff, i].GetFirstScore();
                    Vector2 textSize = mFontChalk.MeasureString(text);
                    Vector2 textPos = new Vector2(pos.X + mBoardCenterOffset.X - textSize.X * 0.5f,
                                                  pos.Y + mBoardCenterOffset.Y - textSize.Y);

                    spritebatch.Draw(mTexBoard, pos, Color.White);
                    spritebatch.DrawString(mFontChalk, text, textPos, Color.White);

                    // Draw sign and sign text.
                    Rectangle signPos = new Rectangle(pos.X, pos.Y + mSignYOffset,
                                                      pos.Width, mSignHeight);
                    text = ((Level)i).ToString();
                    textSize = mFontSign.MeasureString(text) * 0.5f;
                    textPos = new Vector2(signPos.X + mSignCenterOffset.X - textSize.X,
                                                  signPos.Y + mSignCenterOffset.Y - textSize.Y);

                    spritebatch.Draw(mTexSign, signPos, Color.White);
                    spritebatch.DrawString(mFontSign, text, textPos + new Vector2(1, 1), Color.White * 0.5f);
                    spritebatch.DrawString(mFontSign, text, textPos, Color.Black * 0.85f);

                    ++i;
                }
            }

            foreach (Button btn in mButtons)
                btn.Draw(spritebatch);
        }

        public void CreateHighscoreLists()
        {
            mHighscores = new HighscoreList[3, 7];

            for (int diff = 0; diff < mHighscores.GetLength(0); ++diff)
            {
                for (int lvl = 0; lvl < mHighscores.GetLength(1); ++lvl)
                {
                    mHighscores[diff, lvl] = new HighscoreList((Difficulty)diff, (Level)lvl);
                }
            }
        }

        private void BtnEasyClick()
        {
            mCurrDiff = Difficulty.Easy;

            mButtons[0].Marked = true;
            mButtons[1].Marked = false;
            mButtons[2].Marked = false;
        }

        private void BtnNormalClick()
        {
            mCurrDiff = Difficulty.Normal;

            mButtons[1].Marked = true;
            mButtons[0].Marked = false;
            mButtons[2].Marked = false;
        }

        private void BtnHardClick()
        {
            mCurrDiff = Difficulty.Hard;

            mButtons[2].Marked = true;
            mButtons[0].Marked = false;
            mButtons[1].Marked = false;
        }

        private void BtnMenuReturnClick()
        {
            Core.ReturnToMenu();
        }
    }
}

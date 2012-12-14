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
        private struct HighscorePair
        {
            public string Name;
            public string Score;

            public HighscorePair(string name, string score)
            {
                Name = name;
                Score = score;
            }
        }

        // Highscore board and sign
        private Texture2D mHSSign;
        private Texture2D mHSBoard;
        private SpriteFont mHSTextFont;
        private Rectangle mSignPos;
        private Rectangle mBoardPos;
        private Vector2 mSignTextPos;
        private Vector2 mBoardNamePos;
        private Vector2 mBoardScorePos;
        private string mSignText;
        private List<HighscorePair> mHighscores;
        private int mHighlightIndex;

        // Score information
        private string mScoreText;
        private SpriteFont mTextFont;
        private Color mTextColour;
        private Vector2 mTextPos;
        private Vector2 mScoreTextPos;
        private string mScore;
        private bool mGameWasWon;

        // Highscore information
        private Difficulty mDifficulty;
        private Level mLevel;

        // Buttons
        List<Button> mButtons;

        public EndScreen()
        {
            // Load chalk board textures and fonts.
            mHSBoard = Core.Content.Load<Texture2D>(@"Textures/chalkBoard");
            mHSSign = Core.Content.Load<Texture2D>(@"Textures/sign");
            mHSTextFont = Core.Content.Load<SpriteFont>(@"Fonts/chalk");

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
            mScoreText = "Your final score:\n";
            mTextFont = Core.Content.Load<SpriteFont>(@"Fonts/button");
            mTextPos = new Vector2((mBoardPos.X - mTextFont.MeasureString(mScoreText).X) * 0.5f, 192);
            mTextColour = Color.Turquoise;

            // Calculate positions and create the buttons.
            int btnWidth = 384;
            int btnHeight = 64;
            int btnX = (int)((mBoardPos.X - btnWidth) * 0.5f);
            int btnBottomY = mBoardPos.Y + mBoardPos.Height - btnHeight;
            int btnPadding = btnHeight + 32;

            mButtons = new List<Button>();
            mButtons.Add(new Button(BtnMenuClick, new Rectangle(btnX, btnBottomY - btnPadding * 2, btnWidth, btnHeight), "Return to Menu"));
            mButtons.Add(new Button(BtnMenuClick, new Rectangle(btnX, btnBottomY - btnPadding, btnWidth, btnHeight), "All Highscores"));
            mButtons.Add(new Button(BtnExitClick, new Rectangle(btnX, btnBottomY, btnWidth, btnHeight), "Exit Game"));

            mButtons[1].Enabled = false;
        }

        public void SetInfo(Difficulty difficulty, Level level, int score, bool gameWasWon)
        {
            mScore = score.ToString();
            mGameWasWon = gameWasWon;
            mDifficulty = difficulty;
            mLevel = level;

            Vector2 textSize = mTextFont.MeasureString(mScore);
            mScoreTextPos = new Vector2((mBoardPos.X - textSize.X) * 0.5f, mTextPos.Y + textSize.Y);
            mSignText = mDifficulty.ToString() + ": " + mLevel.ToString();
            textSize = mTextFont.MeasureString(mSignText);
            mSignTextPos = new Vector2(mSignPos.X + (mSignPos.Width - textSize.X) * 0.5f,
                                       mSignPos.Y + (mSignPos.Height - textSize.Y) * 0.5f);

            LoadHighscore(score);
            SaveHighscore();
        }

        public void Update()
        {
            foreach (Button btn in mButtons)
                btn.Update();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw chalk board and sign.
            spriteBatch.Draw(mHSSign, mSignPos, Color.White);
            spriteBatch.Draw(mHSBoard, mBoardPos, Color.White);
            spriteBatch.DrawString(mTextFont, mSignText, mSignTextPos + new Vector2(1, 1), Color.White * 0.5f);
            spriteBatch.DrawString(mTextFont, mSignText, mSignTextPos, Color.Black * 0.85f);

            // Draw the high scores on the chalk board.
            float scale = 0.8f;
            int textheight = (int)(mHSTextFont.MeasureString("Highscore").Y * scale);
            for (int i = 0; i < mHighscores.Count; i++)
            {
                Color textColour = Color.White;
                if(i == mHighlightIndex)
                    textColour = Color.Yellow;

                spriteBatch.DrawString(mHSTextFont, mHighscores[i].Name, mBoardNamePos + new Vector2(0, i * textheight),
                                       textColour, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
                spriteBatch.DrawString(mHSTextFont, mHighscores[i].Score, 
                                       mBoardScorePos + new Vector2(-mHSTextFont.MeasureString(mHighscores[i].Score).X, i * textheight),
                                       textColour, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
            }

            // Draw score text.
            spriteBatch.DrawString(mTextFont, mScoreText, mTextPos, mTextColour);
            spriteBatch.DrawString(mTextFont, mScore, mScoreTextPos, mTextColour);

            // Draw the buttons.
            foreach (Button btn in mButtons)
                btn.Draw(spriteBatch);
        }

        private void LoadHighscore(int newScore)
        {
            mHighscores = new List<HighscorePair>();
            string filename = GetFilename();
            string name = DateTime.Today.ToShortDateString();
            mHighlightIndex = -1;

            if (!File.Exists(filename))
            {
                mHighscores.Add(new HighscorePair(name, newScore.ToString()));
                mHighlightIndex = 0;
                return;
            }

            FileStream file = new FileStream(filename, FileMode.Open);
            StreamReader reader = new StreamReader(file);

            string line = reader.ReadLine();

            while (line != null && mHighscores.Count < 9)
            {
                string[] split = line.Split('|');

                if (newScore > int.Parse(split[1]))
                {
                    mHighscores.Add(new HighscorePair(name, newScore.ToString()));

                    if (mHighscores.Count == 9)
                        break;
                }

                mHighscores.Add(new HighscorePair(split[0], split[1]));
                line = reader.ReadLine();
            }

            reader.Close();
            file.Close();
        }

        private void SaveHighscore()
        {
            string filename = GetFilename();

            Directory.CreateDirectory(Path.GetDirectoryName(filename));
            FileStream file = new FileStream(filename, FileMode.Create);
            StreamWriter writer = new StreamWriter(file);

            foreach (HighscorePair pair in mHighscores)
            {
                writer.WriteLine(pair.Name + "|" + pair.Score);
            }

            writer.Close();
            file.Close();
        }

        private string GetFilename()
        {
            string filename = "Content/Data/";

            switch (mDifficulty)
            {
                case Difficulty.Easy:
                    filename += "easy";
                    break;
                case Difficulty.Normal:
                    filename += "normal";
                    break;
                case Difficulty.Hard:
                    filename += "hard";
                    break;
            }

            switch (mLevel)
            {
                case Level.Deca:
                    filename += "Deca.txt";
                    break;
                case Level.Hecto:
                    filename += "Hecto.txt";
                    break;
                case Level.Kilo:
                    filename += "Kilo.txt";
                    break;
                case Level.Mega:
                    filename += "Mega.txt";
                    break;
                case Level.Giga:
                    filename += "Giga.txt";
                    break;
                case Level.Tera:
                    filename += "Tera.txt";
                    break;
                case Level.All:
                    filename += "All.txt";
                    break;
            }

            return filename;
        }

        private void BtnExitClick()
        {
            Core.Exit();
        }

        private void BtnMenuClick()
        {
            Core.ReturnToMenu();
        }
    }
}

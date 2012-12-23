using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bubbles
{
    class HighscoreList
    {
        private struct Entry
        {
            public string Name;
            public string Score;

            public Entry(string name, string score)
            {
                Name = name;
                Score = score;
            }
        }

        private List<Entry> mHighscores;
        private Difficulty mDifficulty;
        private Level mLevel;

        private SpriteFont mFont;

        private int mHighlightIndex = -1;

        // Constants
        public const int C_MAX_LIST_COUNT = 9;

        public HighscoreList(Difficulty difficulty, Level level, int scoreToAdd = -1)
        {
            mDifficulty = difficulty;
            mLevel = level;

            mFont = Core.Content.Load<SpriteFont>(@"Fonts\chalk");

            mHighscores = new List<Entry>();
            if(scoreToAdd < 0)
                Load();
            else
                LoadAndAdd(scoreToAdd);
            
        }

        public void Draw(SpriteBatch spriteBatch, float scale, Vector2 nameStartOffset, Vector2 scoreEndOffset)
        {
            int textheight = (int)(mFont.MeasureString("Highscore").Y * scale);
            for (int i = 0; i < mHighscores.Count; i++)
            {
                Color textColour = Color.White;
                if (i == mHighlightIndex)
                    textColour = Color.Yellow;

                spriteBatch.DrawString(mFont, mHighscores[i].Name, nameStartOffset + new Vector2(0, i * textheight),
                                       textColour, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
                spriteBatch.DrawString(mFont, mHighscores[i].Score,
                                       scoreEndOffset + new Vector2(-mFont.MeasureString(mHighscores[i].Score).X, i * textheight),
                                       textColour, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
            }
        }

        public void SaveToFile()
        {
            string filename = GetFilename();

            Directory.CreateDirectory(Path.GetDirectoryName(filename));
            FileStream file = new FileStream(filename, FileMode.Create);
            StreamWriter writer = new StreamWriter(file);

            foreach (HighscoreList.Entry pair in mHighscores)
            {
                writer.WriteLine(pair.Name + "|" + pair.Score);
            }

            writer.Close();
            file.Close();
        }

        public string GetFirstScore()
        {
            if (mHighscores.Count == 0)
                return "No score";
            else
                return mHighscores[0].Score;
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

        private void Load()
        {
            mHighscores = new List<HighscoreList.Entry>();
            string filename = GetFilename();

            if (!File.Exists(filename))
                return;

            FileStream file = new FileStream(filename, FileMode.Open);
            StreamReader reader = new StreamReader(file);

            string line = reader.ReadLine();

            while (line != null && mHighscores.Count < C_MAX_LIST_COUNT)
            {
                string[] split = line.Split('|');

                mHighscores.Add(new HighscoreList.Entry(split[0], split[1]));
                line = reader.ReadLine();
            }

            reader.Close();
            file.Close();
        }

        private void LoadAndAdd(int newScore)
        {
            mHighscores = new List<Entry>();
            string filename = GetFilename();
            string name = DateTime.Today.ToString("yyyy-MM-dd");
            mHighlightIndex = -1;

            if (!File.Exists(filename))
            {
                mHighscores.Add(new HighscoreList.Entry(name, newScore.ToString()));
                mHighlightIndex = 0;
                return;
            }

            FileStream file = new FileStream(filename, FileMode.Open);
            StreamReader reader = new StreamReader(file);

            string line = reader.ReadLine();

            while (line != null && mHighscores.Count < C_MAX_LIST_COUNT)
            {
                string[] split = line.Split('|');

                if (newScore > int.Parse(split[1]) && mHighlightIndex < 0)
                {
                    mHighscores.Add(new HighscoreList.Entry(name, newScore.ToString()));
                    mHighlightIndex = mHighscores.Count - 1;

                    if (mHighscores.Count == C_MAX_LIST_COUNT)
                        break;
                }

                mHighscores.Add(new HighscoreList.Entry(split[0], split[1]));
                line = reader.ReadLine();
            }

            reader.Close();
            file.Close();

            // If the new high score is not on the list and the list is not full, add it anyway.
            if (mHighlightIndex == -1 && mHighscores.Count < C_MAX_LIST_COUNT)
            {
                mHighscores.Add(new HighscoreList.Entry(name, newScore.ToString()));
                mHighlightIndex = mHighscores.Count - 1;
            }
        }
    }
}

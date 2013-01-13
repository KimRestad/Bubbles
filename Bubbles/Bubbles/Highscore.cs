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
        /// <summary>
        /// An entry in the highscore list.
        /// </summary>
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

        // Constants
        public const int C_MAX_LIST_COUNT = 9;

        // Highscore information.
        private List<Entry> mHighscores;
        private Difficulty mDifficulty;
        private Level mLevel;

        // Other variables.
        private SpriteFont mFont;
        private int mHighlightIndex = -1;

        public HighscoreList(Difficulty difficulty, Level level, int scoreToAdd = -1)
        {
            // Save the highscore difficulty and level.
            mDifficulty = difficulty;
            mLevel = level;

            // Load the font and create the highscore list.
            mFont = Core.Content.Load<SpriteFont>(@"Fonts\chalk");

            mHighscores = new List<Entry>();
            if(scoreToAdd < 0)
                Load();
            else
                LoadAndAdd(scoreToAdd);
            
        }

        /// <summary>
        /// Draw the highscore list
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to use when drawing the list.</param>
        /// <param name="scale">The text scale.</param>
        /// <param name="nameStartOffset">Where to start drawing the name part.</param>
        /// <param name="scoreEndOffset">Where to end drawing the score part.</param>
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

        /// <summary>
        /// Save the highscore to a file.
        /// </summary>
        public void SaveToFile()
        {
            // Get the filename. If the path does not exist, create it. Create the file and a reader.
            string filename = GetFilename();

            Directory.CreateDirectory(Path.GetDirectoryName(filename));
            FileStream file = new FileStream(filename, FileMode.Create);
            StreamWriter writer = new StreamWriter(file);

            // Write the highscore entries to the file, then close the writer and the file.
            foreach (HighscoreList.Entry pair in mHighscores)
                writer.WriteLine(pair.Name + "|" + pair.Score);

            writer.Close();
            file.Close();
        }

        /// <summary>
        /// Get the highest score on the list.
        /// </summary>
        /// <returns>A string with the highest score on the list or "No Score" if there are none.</returns>
        public string GetFirstScore()
        {
            if (mHighscores.Count == 0)
                return "No score";
            else
                return mHighscores[0].Score;
        }

        /// <summary>
        /// Generate the correct filename based on the difficulty and level.
        /// </summary>
        /// <returns>The filename to save to or load from.</returns>
        private string GetFilename()
        {
            // Save base folder.
            string filename = "Content/Data/";

            // Add difficulty.
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

            // Add level and file ending.
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

        /// <summary>
        /// Load the highscore list from file based on the difficulty and level.
        /// </summary>
        private void Load()
        {
            // Create a new, empty list and get file name. If file does not exist, return, leaving the list empty.
            mHighscores = new List<HighscoreList.Entry>();
            string filename = GetFilename();

            if (!File.Exists(filename))
                return;

            // If the file exists, create the file and reader.
            FileStream file = new FileStream(filename, FileMode.Open);
            StreamReader reader = new StreamReader(file);

            // For as long as there are lines or until the maximum amount of lines is reached, read the next line.
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

        /// <summary>
        /// Load highscore while adding the new score if it qualifies.
        /// </summary>
        /// <param name="newScore">The aspiring high score.</param>
        private void LoadAndAdd(int newScore)
        {
            // Create a new, empty list and get filename. Save the name as the current date and reset highlight index.
            mHighscores = new List<Entry>();
            string filename = GetFilename();
            string name = DateTime.Today.ToString("yyyy-MM-dd");
            mHighlightIndex = -1;

            // If file does not exist, return, leaving the list empty, else continue by creating file and reader.
            if (!File.Exists(filename))
            {
                mHighscores.Add(new HighscoreList.Entry(name, newScore.ToString()));
                mHighlightIndex = 0;
                return;
            }

            FileStream file = new FileStream(filename, FileMode.Open);
            StreamReader reader = new StreamReader(file);

            // For as long as there are lines or until the maximum amount of lines is reached, read the next line.
            string line = reader.ReadLine();

            while (line != null && mHighscores.Count < C_MAX_LIST_COUNT)
            {
                string[] split = line.Split('|');

                // if the new score qualifies, add it to list and update highlight index.
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Bubbles
{
    abstract class Core
    {
        private static Game1 sGame;
        private static Random sRandom;

        #region Methods

        /// <summary>
        /// Initialize the core. Must be done before it can be used.
        /// </summary>
        /// <param name="game">The game to initialize the core for</param>
        public static void Initialize(Game1 game)
        {
            sGame = game;
            sRandom = new Random();

            // Load preferences.
            string filename = "Content/Data/preferences";

            if (!File.Exists(filename))
            {
                // If the file does not exist, load defaults.
                Fullscreen = false;
                PlaySounds = true;
                ShowShapes = false;
                ShowLongAim = false;
            }
            else
            {
                // If the file exists, create the file and reader.
                FileStream file = new FileStream(filename, FileMode.Open);
                StreamReader reader = new StreamReader(file);

                string[] setting = reader.ReadLine().Split(':');
                Fullscreen = setting[1].ToLower() == "true" ? true : false;

                setting = reader.ReadLine().Split(':');
                PlaySounds = setting[1].ToLower() == "true" ? true : false;

                setting = reader.ReadLine().Split(':');
                ShowShapes = setting[1].ToLower() == "true" ? true : false;

                setting = reader.ReadLine().Split(':');
                ShowLongAim = setting[1].ToLower() == "true" ? true : false;

                reader.Close();
                file.Close();
            }
        }

        /// <summary>
        /// Exit the game.
        /// </summary>
        public static void Exit()
        {
            // Save preferences.
            string filename = "Content/Data/preferences";

            // If the path does not exist, create it. Create the file and a reader.
            Directory.CreateDirectory(Path.GetDirectoryName(filename));
            FileStream file = new FileStream(filename, FileMode.Create);
            StreamWriter writer = new StreamWriter(file);

            // Write the preferences to the file, then close the writer and the file.
            writer.WriteLine("Fullscreen:" + Fullscreen);
            writer.WriteLine("Sounds:" + PlaySounds);
            writer.WriteLine("Shapes:" + ShowShapes);
            writer.WriteLine("HelpAim:" + ShowLongAim);

            writer.Close();
            file.Close();
            
            // End game.
            sGame.Exit();
        }

        /// <summary>
        /// Start the game.
        /// </summary>
        public static void StartGame(Difficulty levelDifficulty, Level levelToPlay)
        {
            sGame.GameState = GameState.InGame;
            sGame.GameScreen.StartGame(levelDifficulty, levelToPlay);
        }

        /// <summary>
        /// Show the highscore screen.
        /// </summary>
        public static void ShowHighscore()
        {
            sGame.GameState = GameState.Highscore;
            sGame.HighscoreScreen.CreateHighscoreLists();
        }

        /// <summary>
        /// Game is over. Pass on score and whether the game was won
        /// </summary>
        /// <param name="score">The user's score</param>
        /// <param name="won">True if the user won the game, else false</param>
        public static void EndGame(int score, bool won)
        {
            sGame.GameState = GameState.End;
            sGame.EndScreen.SetInfo(sGame.GameScreen.Difficulty, sGame.GameScreen.Level, score, won);
        }

        /// <summary>
        /// Return to main menu.
        /// </summary>
        public static void ReturnToMenu()
        {
            sGame.GameState = GameState.Start;
        }

        #endregion Methods

        #region Properties

        /// <summary>
        /// Read only. Random generator.
        /// </summary>
        public static Random RandomGen
        {
            get { return sRandom; }
        }

        /// <summary>
        /// Read only. Get the size of the window.
        /// </summary>
        public static Rectangle ClientBounds
        {
            get { return sGame.Window.ClientBounds; }
        }

        /// <summary>
        /// Read only. Get the content manager.
        /// </summary>
        public static ContentManager Content
        {
            get { return sGame.Content; }
        }

        /// <summary>
        /// Get the user's current screen resolution.
        /// </summary>
        public static Vector2 UserScreenResolution
        {
            get { return sGame.UserResolution; }
        }

        /// <summary>
        /// Get or set whether the mouse is visible.
        /// </summary>
        public static bool IsMouseVisible
        {
            get { return sGame.IsMouseVisible; }
            set { sGame.IsMouseVisible = value; }
        }

        /// <summary>
        /// Get or set whether the game is in fullscreen.
        /// </summary>
        public static bool Fullscreen
        {
            get { return sGame.Fullscreen; }
            set { sGame.Fullscreen = value; }
        }

        public static bool PlaySounds { get; set; }

        public static bool ShowShapes { get; set; }

        public static bool ShowLongAim { get; set; }

        #endregion Properties
    }
}

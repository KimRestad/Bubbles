using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        }

        /// <summary>
        /// Exit the game.
        /// </summary>
        public static void Exit()
        {
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
        /// Game is over. Pass on score and whether the game was won
        /// </summary>
        /// <param name="score">The user's score</param>
        /// <param name="won">True if the user won the game, else false</param>
        public static void EndGame(int score, bool won)
        {
            sGame.GameState = GameState.End;
            sGame.EndScreen.SetInfo(sGame.GameScreen.Difficulty, sGame.GameScreen.Level, score, won);
        }

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

        public static bool IsMouseVisible
        {
            get { return sGame.IsMouseVisible; }
            set { sGame.IsMouseVisible = value; }
        }

        #endregion Properties
    }
}

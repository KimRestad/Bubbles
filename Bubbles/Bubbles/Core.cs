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

        public static void Initialize(Game1 game)
        {
            sGame = game;
            sRandom = new Random();
        }

        public static void Exit()
        {
            sGame.Exit();
        }

        public static void StartGame()
        {
            sGame.GameState = GameState.InGame;
            sGame.GameScreen.StartGame();
        }

        #endregion Methods

        #region Properties

        public static Random RandomGen
        {
            get { return sRandom; }
        }

        public static Rectangle ClientBounds
        {
            get { return sGame.Window.ClientBounds; }
        }

        public static ContentManager Content
        {
            get { return sGame.Content; }
        }

        public static Vector2 UserScreenResolution
        {
            get { return sGame.UserResolution; }
        }

        //public static GameState Gamestate
        //{
        //    get { return sGame.GameState; }
        //    set { sGame.GameState = value; }
        //}

        #endregion Properties
    }
}

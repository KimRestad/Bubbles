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

        #region Methods

        public static void Initialize(Game1 game)
        {
            sGame = game;
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

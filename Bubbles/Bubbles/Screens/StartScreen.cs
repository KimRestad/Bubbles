using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Bubbles
{
    public class StartScreen
    {
        // Graphics variables.
        private SpriteFont mDebugFont;
        private Texture2D mBackground;
        private Rectangle mBGPosition;

        // Overlay screens.
        private ChoiceScreen mGameChoices;
        private CreditsScreen mCredits;
        private OptionScreen mOptions;

        // Other variables.
        private List<Button> mButtons;
        private KeyboardState mPrevKeyboard;
        private int mButtonIndex;

        /// <summary>
        /// Creates the start screen and its contents.
        /// </summary>
        public StartScreen()
        {
            // Create the screens.
            mGameChoices = new ChoiceScreen();
            mCredits = new CreditsScreen();
            mOptions = new OptionScreen();

            // Load fonts and background.
            mDebugFont = Core.Content.Load<SpriteFont>(@"Fonts\default");
            mBackground = Core.Content.Load<Texture2D>(@"Textures\background2big");

            // Calculate the background position.
            if (Core.ClientBounds.Width > mBackground.Width && Core.ClientBounds.Height > mBackground.Height)
            {
                mBGPosition = new Rectangle((int)((Core.ClientBounds.Width - mBackground.Width) * 0.5f),
                                            (int)((Core.ClientBounds.Height - mBackground.Height) * 0.5f),
                                            mBackground.Width, mBackground.Height);
            }
            else
            {
                // TODO: calculate correct ratio for smaller screens
                mBGPosition = new Rectangle(0, 0, Core.ClientBounds.Width, Core.ClientBounds.Height);
            }

            // Create the buttons.
            int btnWidth = 320;
            int btnHalfWidth = (int)(btnWidth * 0.5f);
            int btnWidthS = 224;
            int btnHeight = 64;
            int btnStartY = 356;
            int btnStrideY = btnHeight + 16;
            int btnPaddingX = 8;

            int x = (int)(Core.ClientBounds.Width * 0.5f);

            mButtons = new List<Button>();
            mButtons.Add(new Button(BtnPlayClicked,
                         new Rectangle(x - btnHalfWidth, btnStartY, btnWidth, btnHeight), "New Game"));
            mButtons.Add(new Button(BtnHSClicked,
                         new Rectangle(x - btnHalfWidth, btnStartY + btnStrideY, btnWidth, btnHeight),
                         "Highscore"));
            mButtons.Add(new Button(BtnOptionsClicked,
                         new Rectangle(x - btnWidthS - btnPaddingX, btnStartY + btnStrideY * 2, btnWidthS, btnHeight), 
                         "Options"));
            mButtons.Add(new Button(BtnCreditsClicked,
                         new Rectangle(x + btnPaddingX, btnStartY + btnStrideY * 2, btnWidthS, btnHeight),
                         "Credits"));
            mButtons.Add(new Button(BtnExitClicked,
                         new Rectangle(x - btnHalfWidth, btnStartY + btnStrideY * 3, btnWidth, btnHeight), "Exit"));

            mButtonIndex = mButtons.Count - 1;
        }

        /// <summary>
        /// Update the screen and contents.
        /// </summary>
        public void Update()
        {
            // Only update buttons if no overlay screens are shown.
            if (!mGameChoices.Visible && !mCredits.Visible && !mOptions.Visible)
            {
                foreach (Button btn in mButtons)
                    btn.Update();
                
                HandleInput();
            }

            // Update the screens.
            mGameChoices.Update();
            mCredits.Update();
            mOptions.Update();
        }

        /// <summary>
        /// Draw the screen and contents.
        /// </summary>
        /// <param name="spritebatch">The sprite batch to use when drawing the screen.</param>
        public void Draw(SpriteBatch spritebatch)
        {
            // Only draw buttons if no overlay screens are shown.
            if (!mGameChoices.Visible && !mCredits.Visible && !mOptions.Visible)
            {
                spritebatch.Draw(mBackground, mBGPosition, Color.White);
                foreach (Button btn in mButtons)
                    btn.Draw(spritebatch);
            }

            // Draw the screens.
            mGameChoices.Draw(spritebatch);
            mCredits.Draw(spritebatch);
            mOptions.Draw(spritebatch);
        }

        /// <summary>
        /// Handle input from the keyboard.
        /// </summary>
        private void HandleInput()
        {
            KeyboardState currKeyboard = Keyboard.GetState();

            // If down key has just been pressed, increment the button index, keeping it within bounds, and move mouse.
            if (currKeyboard.IsKeyDown(Keys.Down) && mPrevKeyboard.IsKeyUp(Keys.Down))
            {
                mButtonIndex = (mButtonIndex + 1) % mButtons.Count;
                Vector2 newMousePos = mButtons[mButtonIndex].Center;
                Mouse.SetPosition((int)newMousePos.X, (int)newMousePos.Y);
            }
            // If down key has just been pressed, decrement the button index, keeping it within bounds, and move mouse.
            else if (currKeyboard.IsKeyDown(Keys.Up) && mPrevKeyboard.IsKeyUp(Keys.Up))
            {
                if (mButtonIndex <= 0)
                    mButtonIndex = mButtons.Count;
                mButtonIndex--;
                Vector2 newMousePos = mButtons[mButtonIndex].Center;
                Mouse.SetPosition((int)newMousePos.X, (int)newMousePos.Y);
            }
            // If enter key has just been pressed and the chosen button is hovered, click it.
            else if (currKeyboard.IsKeyDown(Keys.Enter) && mPrevKeyboard.IsKeyUp(Keys.Enter))
            {
                if (mButtons[mButtonIndex].Hovered)
                    mButtons[mButtonIndex].Click();
            }

            mPrevKeyboard = currKeyboard;
        }

        /// <summary>
        /// Play button click event. Show game setup window.
        /// </summary>
        private void BtnPlayClicked()
        {
            mGameChoices.Visible = true;
        }

        /// <summary>
        /// Highscore button click event. Switch to highscore screen.
        /// </summary>
        private void BtnHSClicked()
        {
            Core.ShowHighscore();
        }

        /// <summary>
        /// Options button click event. Show option screen.
        /// </summary>
        private void BtnOptionsClicked()
        {
            mOptions.Show();
        }

        /// <summary>
        /// Credits button click event. Show credits screen.
        /// </summary>
        private void BtnCreditsClicked()
        {
            mCredits.Visible = true;
        }

        /// <summary>
        /// Exit button click event. Exit game.
        /// </summary>
        private void BtnExitClicked()
        {
            Core.Exit();
        }
    }
}

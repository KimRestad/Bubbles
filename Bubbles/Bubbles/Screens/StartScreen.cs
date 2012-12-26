﻿using System;
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
        // Graphics
        private SpriteFont mDebugFont;
        private Texture2D mBackground;
        private Rectangle mBGPosition;

        // Overlay screens
        private ChoiceScreen mGameChoices;
        private CreditsScreen mCredits;

        private List<Button> mButtons;
        private KeyboardState mPrevKeyboard;
        private int mButtonIndex;

        public StartScreen()
        {
            mGameChoices = new ChoiceScreen();
            mCredits = new CreditsScreen();

            mDebugFont = Core.Content.Load<SpriteFont>(@"Fonts\default");
            mBackground = Core.Content.Load<Texture2D>(@"Textures\background2big");
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

            int buttonWidth = 256;
            int buttonHeight = 64;
            int buttonStartY = 356;
            int buttonStrideY = buttonHeight + 16;

            int x = (int)((Core.ClientBounds.Width - buttonWidth) * 0.5);

            mButtons = new List<Button>();
            mButtons.Add(new Button(BtnPlayClicked, new Rectangle(x, buttonStartY, buttonWidth, buttonHeight), "Play"));
            mButtons.Add(new Button(BtnHSClicked, new Rectangle(x, buttonStartY + buttonStrideY, buttonWidth, buttonHeight), "Highscore"));
            mButtons.Add(new Button(BtnCreditsClicked, new Rectangle(x, buttonStartY + buttonStrideY*2, buttonWidth, buttonHeight), "Credits"));
            mButtons.Add(new Button(BtnExitClicked, new Rectangle(x, buttonStartY + buttonStrideY * 3, buttonWidth, buttonHeight), "Exit"));

            mButtonIndex = mButtons.Count - 1;
        }

        public void Update()
        {
            // Only update buttons if the choices window is not showing
            if (!mGameChoices.Visible && !mCredits.Visible)
            {
                foreach (Button btn in mButtons)
                    btn.Update();
                
                HandleInput();
            }

            mGameChoices.Update();
            mCredits.Update();
        }

        public void Draw(SpriteBatch spritebatch)
        {
            // Only draw buttons if the choices window is not showing
            if (!mGameChoices.Visible && !mCredits.Visible)
            {
                spritebatch.Draw(mBackground, mBGPosition, Color.White);
                foreach (Button btn in mButtons)
                    btn.Draw(spritebatch);
            }

            mGameChoices.Draw(spritebatch);
            mCredits.Draw(spritebatch);
        }

        private void HandleInput()
        {
            KeyboardState currKeyboard = Keyboard.GetState();

            if (currKeyboard.IsKeyDown(Keys.Down) && mPrevKeyboard.IsKeyUp(Keys.Down))
            {
                mButtonIndex = (mButtonIndex + 1) % mButtons.Count;
                Vector2 newMousePos = mButtons[mButtonIndex].Center;
                Mouse.SetPosition((int)newMousePos.X, (int)newMousePos.Y);
            }
            else if (currKeyboard.IsKeyDown(Keys.Up) && mPrevKeyboard.IsKeyUp(Keys.Up))
            {
                if (mButtonIndex <= 0)
                    mButtonIndex = mButtons.Count;
                mButtonIndex--;
                Vector2 newMousePos = mButtons[mButtonIndex].Center;
                Mouse.SetPosition((int)newMousePos.X, (int)newMousePos.Y);
            }
            else if (currKeyboard.IsKeyDown(Keys.Enter) && mPrevKeyboard.IsKeyUp(Keys.Enter))
            {
                if (mButtons[mButtonIndex].Hovered)
                    mButtons[mButtonIndex].Click();
            }

            mPrevKeyboard = currKeyboard;
        }

        private void BtnPlayClicked()
        {
            mGameChoices.Visible = true;
        }

        private void BtnHSClicked()
        {
            Core.ShowHighscore();
        }

        private void BtnCreditsClicked()
        {
            mCredits.Visible = true;
        }

        private void BtnExitClicked()
        {
            Core.Exit();
        }
    }
}

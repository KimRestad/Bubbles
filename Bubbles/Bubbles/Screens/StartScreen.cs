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
        // Graphics
        private SpriteFont mDebugFont;
        private Texture2D mBackground;
        private Rectangle mBGPosition;

        // Overlay screens
        private ChoiceScreen mGameChoices;
        private CreditsScreen mCredits;

        private List<Button> mButtons;

        private MouseState mPrevMouse;

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
            mButtons.Add(new Button(BtnCreditsClicked, new Rectangle(x, buttonStartY + buttonStrideY, buttonWidth, buttonHeight), "Highscore"));
            mButtons.Add(new Button(BtnCreditsClicked, new Rectangle(x, buttonStartY + buttonStrideY*2, buttonWidth, buttonHeight), "Credits"));
            mButtons.Add(new Button(BtnExitClicked, new Rectangle(x, buttonStartY + buttonStrideY * 3, buttonWidth, buttonHeight), "Exit"));

            mButtons[1].Enabled = false;
        }

        public void Update()
        {
            MouseState currMouse = Mouse.GetState();

            // Only update buttons if the choices window is not showing
            if (!mGameChoices.Visible && !mCredits.Visible)
                foreach (Button btn in mButtons)
                    btn.Update();

            mPrevMouse = currMouse;

            mGameChoices.Update();
            mCredits.Update();
        }

        public void Draw(SpriteBatch spritebatch)
        {
            //spritebatch.Draw(mBackground, mBGPosition, Color.White);

            // Only draw buttons if the choices window is not showing
            if (!mGameChoices.Visible && !mCredits.Visible)
            {
                // DEBUG
                spritebatch.Draw(mBackground, mBGPosition, Color.White);
                foreach (Button btn in mButtons)
                    btn.Draw(spritebatch);
            }

            mGameChoices.Draw(spritebatch);
            mCredits.Draw(spritebatch);
        }

        private void BtnPlayClicked()
        {
            mGameChoices.Visible = true;
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

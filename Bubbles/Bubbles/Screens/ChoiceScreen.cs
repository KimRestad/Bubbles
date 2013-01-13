using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Bubbles
{
    class ChoiceScreen : OverlayScreen
    {
        // Constants
        private const string C_FILENAME = @"Content\Data\gameSetup";

        // Text variables.
        private Vector2 mDiffTextPos;
        private Vector2 mLvlTextPos;
        private string mDiffText;
        private string mLvlText;
        private SpriteFont mFont;

        // Button variables.
        private List<Button> mButtons;
        private int mButtonIndex;           // Used when using arrow keys to choose button.

        // Game choice variables.
        private Difficulty mChosenDiff;
        private Level mChosenLvl;
        
        // State variables.
        private KeyboardState mPrevKeyboard;
        
        /// <summary>
        /// Create a choice screen.
        /// </summary>
        public ChoiceScreen() 
            : base(new Point(896, 640), new Rectangle(0, 0, Core.ClientBounds.Width, Core.ClientBounds.Height), Color.Black, 0.75f)
        {
            // Initialise information text and font.
            mDiffText = "CHOOSE A DIFFICULTY";
            mLvlText = "CHOOSE ONE OR ALL LEVELS TO PLAY";
            mFont = Core.Content.Load<SpriteFont>("Fonts/credits");

            // Create the buttons.
            Point size = new Point(256, 64);
            Point offset = new Point((int)(mPosition.X + (mPosition.Width - size.X) * 0.5f), (int)mPosition.Y + 96);
            Point padding = new Point(size.X + 16, size.Y + 16);

            mDiffTextPos = new Vector2(mPosition.X + (mPosition.Width - mFont.MeasureString(mDiffText).X) * 0.5f, offset.Y - 64);
            mButtons = new List<Button>();
            mButtons.Add(new Button(BtnEasyClick, new Rectangle(offset.X - padding.X, offset.Y, size.X, size.Y), "Easy"));
            mButtons.Add(new Button(BtnNormalClick, new Rectangle(offset.X, offset.Y, size.X, size.Y), "Normal"));
            mButtons.Add(new Button(BtnHardClick, new Rectangle(offset.X + padding.X, offset.Y, size.X, size.Y), "Hard"));

            offset.Y += padding.Y + 96;
            mLvlTextPos = new Vector2(mPosition.X + (mPosition.Width - mFont.MeasureString(mLvlText).X) * 0.5f, offset.Y - 64);

            mButtons.Add(new Button(BtnDecaClick, new Rectangle(offset.X - padding.X, offset.Y, size.X, size.Y), "Deca"));
            mButtons.Add(new Button(BtnHectoClick, new Rectangle(offset.X, offset.Y, size.X, 64), "Hekto"));
            mButtons.Add(new Button(BtnKiloClick, new Rectangle(offset.X + padding.X, offset.Y, size.X, size.Y), "Kilo"));

            mButtons.Add(new Button(BtnMegaClick, new Rectangle(offset.X - padding.X, offset.Y + padding.Y, size.X, size.Y), "Mega"));
            mButtons.Add(new Button(BtnGigaClick, new Rectangle(offset.X, offset.Y + padding.Y, size.X, size.Y), "Giga"));
            mButtons.Add(new Button(BtnTeraClick, new Rectangle(offset.X + padding.X, offset.Y + padding.Y, size.X, size.Y), "Tera"));

            mButtons.Add(new Button(BtnAllClick, new Rectangle(offset.X, offset.Y + padding.Y * 2, size.X, size.Y), "All"));

            offset.Y += padding.Y * 3 + 32;
            mButtons.Add(new Button(BtnCloseClick, new Rectangle(offset.X - padding.X, offset.Y, size.X, size.Y), "Close"));
            mButtons.Add(new Button(BtnPlayClick, new Rectangle(offset.X + padding.X, offset.Y, size.X, size.Y), "Start Game"));

            // Load preference and initialise button index variable.
            LoadLastChoice();
            mButtonIndex = mButtons.Count - 1;
        }

        /// <summary>
        /// Update the screen if it is visible.
        /// </summary>
        public override void Update()
        {
            if (!Visible)
                return;

            base.Update();

            foreach (Button btn in mButtons)
                btn.Update();

            HandleInput();
        }

        /// <summary>
        /// Draw the screen and its contents if the screen is visible.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to use when drawing the screen.</param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!Visible)
                return;

            base.Draw(spriteBatch);
            
            // Draw buttons.
            foreach (Button btn in mButtons)
                btn.Draw(spriteBatch);

            // Draw information text.
            spriteBatch.DrawString(mFont, mDiffText, mDiffTextPos, Color.Yellow);
            spriteBatch.DrawString(mFont, mLvlText, mLvlTextPos, Color.Yellow);
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
        /// Load the game setup preference (what was chosen last time).
        /// </summary>
        private void LoadLastChoice()
        {
            // If the file does not exist, choose difficulty Easy and level Deca.
            if(!File.Exists(C_FILENAME))
            {
                mButtons[0].Marked = true;
                mButtons[3].Marked = true;
                return;
            }

            FileStream file = new FileStream(C_FILENAME, FileMode.Open);
            StreamReader reader = new StreamReader(file);

            // Read difficulty and choose it.
            string line = reader.ReadLine();
            if (line != null && line.Contains("Difficulty:"))
            {
                line = line.Substring(11);
                if (line == "Easy")
                    BtnEasyClick();
                else if (line == "Normal")
                    BtnNormalClick();
                else if (line == "Hard")
                    BtnHardClick();
            }

            // Read level and choose it.
            line = reader.ReadLine();
            if (line != null && line.Contains("Level:"))
            {
                line = line.Substring(6);
                if (line == "Deca")
                    BtnDecaClick();
                else if (line == "Hecto")
                    BtnHectoClick();
                else if (line == "Kilo")
                    BtnKiloClick();
                else if (line == "Mega")
                    BtnMegaClick();
                else if (line == "Giga")
                    BtnGigaClick();
                else if (line == "Tera")
                    BtnTeraClick();
                else if (line == "All")
                    BtnAllClick();
            }

            reader.Close();
            file.Close();
        }

        /// <summary>
        /// Save the chosen setup until next time.
        /// </summary>
        private void SaveChoice()
        {
            // If the directory does not exist, create it, then create file and writer.
            Directory.CreateDirectory(Path.GetDirectoryName(C_FILENAME));
            FileStream file = new FileStream(C_FILENAME, FileMode.Create);
            StreamWriter writer = new StreamWriter(file);

            // Write chosen difficulty and level to file, then close writer and file.
            writer.WriteLine("Difficulty:" + mChosenDiff.ToString());
            writer.WriteLine("Level:" + mChosenLvl.ToString());

            writer.Close();
            file.Close();
        }

        /// <summary>
        /// Easy button click event. Mark this button, unmarking previously marked difficulty button.
        /// </summary>
        private void BtnEasyClick()
        {
            mChosenDiff = Difficulty.Easy;
            mButtons[0].Marked = true;
            mButtons[1].Marked = false;
            mButtons[2].Marked = false;
        }

        /// <summary>
        /// Normal button click event. Mark this button, unmarking previously marked difficulty button.
        /// </summary>
        private void BtnNormalClick()
        {
            mChosenDiff = Difficulty.Normal;
            mButtons[1].Marked = true;
            mButtons[0].Marked = false;
            mButtons[2].Marked = false;
        }
        
        /// <summary>
        /// Hard button click event. Mark this button, unmarking previously marked difficulty button.
        /// </summary>
        private void BtnHardClick()
        {
            mChosenDiff = Difficulty.Hard;
            mButtons[2].Marked = true;
            mButtons[0].Marked = false;
            mButtons[1].Marked = false;
        }

        /// <summary>
        /// Unmark all level buttons.
        /// </summary>
        private void BtnUnmarkLevel()
        {
            for (int i = 3; i < mButtons.Count - 2; ++i)
            {
                mButtons[i].Marked = false;
            }
        }

        /// <summary>
        /// Deca button click event. Mark this button, unmarking previously marked level button.
        /// </summary>
        private void BtnDecaClick()
        {
            BtnUnmarkLevel();
            mButtons[3].Marked = true;
            mChosenLvl = Level.Deca;
        }

        /// <summary>
        /// Hecto button click event. Mark this button, unmarking previously marked level button.
        /// </summary>
        private void BtnHectoClick()
        {
            BtnUnmarkLevel();
            mButtons[4].Marked = true;
            mChosenLvl = Level.Hecto;
        }

        /// <summary>
        /// Kilo button click event. Mark this button, unmarking previously marked level button.
        /// </summary>
        private void BtnKiloClick()
        {
            BtnUnmarkLevel();
            mButtons[5].Marked = true;
            mChosenLvl = Level.Kilo;
        }

        /// <summary>
        /// Mega button click event. Mark this button, unmarking previously marked level button.
        /// </summary>
        private void BtnMegaClick()
        {
            BtnUnmarkLevel();
            mButtons[6].Marked = true;
            mChosenLvl = Level.Mega;
        }

        /// <summary>
        /// Giga button click event. Mark this button, unmarking previously marked level button.
        /// </summary>
        private void BtnGigaClick()
        {
            BtnUnmarkLevel();
            mButtons[7].Marked = true;
            mChosenLvl = Level.Giga;
        }

        /// <summary>
        /// Tera button click event. Mark this button, unmarking previously marked level button.
        /// </summary>
        private void BtnTeraClick()
        {
            BtnUnmarkLevel();
            mButtons[8].Marked = true;
            mChosenLvl = Level.Tera;
        }

        /// <summary>
        /// All button click event. Mark this button, unmarking previously marked level button.
        /// </summary>
        private void BtnAllClick()
        {
            BtnUnmarkLevel();
            mButtons[9].Marked = true;
            mChosenLvl = Level.All;
        }

        /// <summary>
        /// Play button click event. Save choice and start game.
        /// </summary>
        private void BtnPlayClick()
        {
            SaveChoice();
            Visible = false;
            Core.StartGame(mChosenDiff, mChosenLvl);
        }

        /// <summary>
        /// Close button click event. Hide this window.
        /// </summary>
        private void BtnCloseClick()
        {
            Visible = false;
        }
    }
}

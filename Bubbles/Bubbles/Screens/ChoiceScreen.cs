using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bubbles
{
    class ChoiceScreen : OverlayScreen
    {
        // Text variables.
        private Vector2 mDiffTextPos;
        private Vector2 mLvlTextPos;
        private string mDiffText;
        private string mLvlText;
        private SpriteFont mFont;

        private List<Button> mButtons;
        private Difficulty mChosenDiff;
        private Level mChosenLvl;

        private string mFilename = @"Content\Data\GamePreference";

        public ChoiceScreen() 
            : base(new Point(896, 640), new Rectangle(0, 0, Core.ClientBounds.Width, Core.ClientBounds.Height), Color.Black, 0.75f)
        {
            mDiffText = "CHOOSE A DIFFICULTY";
            mLvlText = "CHOOSE ONE OR ALL LEVELS TO PLAY";
            mFont = Core.Content.Load<SpriteFont>("Fonts/credits");

            // Create the buttons
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
            mButtons.Add(new Button(BtnHektoClick, new Rectangle(offset.X, offset.Y, size.X, 64), "Hekto"));
            mButtons.Add(new Button(BtnKiloClick, new Rectangle(offset.X + padding.X, offset.Y, size.X, size.Y), "Kilo"));

            mButtons.Add(new Button(BtnMegaClick, new Rectangle(offset.X - padding.X, offset.Y + padding.Y, size.X, size.Y), "Mega"));
            mButtons.Add(new Button(BtnGigaClick, new Rectangle(offset.X, offset.Y + padding.Y, size.X, size.Y), "Giga"));
            mButtons.Add(new Button(BtnTeraClick, new Rectangle(offset.X + padding.X, offset.Y + padding.Y, size.X, size.Y), "Tera"));

            mButtons.Add(new Button(BtnAllClick, new Rectangle(offset.X, offset.Y + padding.Y * 2, size.X, size.Y), "All"));

            offset.Y += padding.Y * 3 + 32;
            mButtons.Add(new Button(BtnExitClick, new Rectangle(offset.X - padding.X, offset.Y, size.X, size.Y), "Close"));
            mButtons.Add(new Button(BtnPlayClick, new Rectangle(offset.X + padding.X, offset.Y, size.X, size.Y), "Start Game"));

            LoadLastChoice();
        }

        public override void Update()
        {
            if (!Visible)
                return;

            base.Update();

            foreach (Button btn in mButtons)
                btn.Update();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!Visible)
                return;

            base.Draw(spriteBatch);
            
            foreach (Button btn in mButtons)
                btn.Draw(spriteBatch);

            spriteBatch.DrawString(mFont, mDiffText, mDiffTextPos, Color.Yellow);
            spriteBatch.DrawString(mFont, mLvlText, mLvlTextPos, Color.Yellow);
        }

        private void LoadLastChoice()
        {
            if(!File.Exists(mFilename))
            {
                mButtons[0].Marked = true;
                mButtons[3].Marked = true;
                return;
            }

            FileStream file = new FileStream(mFilename, FileMode.Open);
            StreamReader reader = new StreamReader(file);

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
            line = reader.ReadLine();
            if (line != null && line.Contains("Level:"))
            {
                line = line.Substring(6);
                if (line == "Deca")
                    BtnDecaClick();
                else if (line == "Hecto")
                    BtnHektoClick();
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

        private void SaveChoice()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(mFilename));
            FileStream file = new FileStream(mFilename, FileMode.Create);
            StreamWriter writer = new StreamWriter(file);

            writer.WriteLine("Difficulty:" + mChosenDiff.ToString());
            writer.WriteLine("Level:" + mChosenLvl.ToString());

            writer.Close();
            file.Close();
        }

        private void BtnEasyClick()
        {
            mChosenDiff = Difficulty.Easy;
            mButtons[0].Marked = true;
            mButtons[1].Marked = false;
            mButtons[2].Marked = false;
        }

        private void BtnNormalClick()
        {
            mChosenDiff = Difficulty.Normal;
            mButtons[1].Marked = true;
            mButtons[0].Marked = false;
            mButtons[2].Marked = false;
        }

        private void BtnHardClick()
        {
            mChosenDiff = Difficulty.Hard;
            mButtons[2].Marked = true;
            mButtons[0].Marked = false;
            mButtons[1].Marked = false;
        }

        private void BtnUnmarkLevel()
        {
            for (int i = 3; i < mButtons.Count - 2; ++i)
            {
                mButtons[i].Marked = false;
            }
        }

        private void BtnDecaClick()
        {
            BtnUnmarkLevel();
            mButtons[3].Marked = true;
            mChosenLvl = Level.Deca;
        }

        private void BtnHektoClick()
        {
            BtnUnmarkLevel();
            mButtons[4].Marked = true;
            mChosenLvl = Level.Hecto;
        }

        private void BtnKiloClick()
        {
            BtnUnmarkLevel();
            mButtons[5].Marked = true;
            mChosenLvl = Level.Kilo;
        }

        private void BtnMegaClick()
        {
            BtnUnmarkLevel();
            mButtons[6].Marked = true;
            mChosenLvl = Level.Mega;
        }

        private void BtnGigaClick()
        {
            BtnUnmarkLevel();
            mButtons[7].Marked = true;
            mChosenLvl = Level.Giga;
        }

        private void BtnTeraClick()
        {
            BtnUnmarkLevel();
            mButtons[8].Marked = true;
            mChosenLvl = Level.Tera;
        }

        private void BtnAllClick()
        {
            BtnUnmarkLevel();
            mButtons[9].Marked = true;
            mChosenLvl = Level.All;
        }

        private void BtnPlayClick()
        {
            SaveChoice();
            Visible = false;
            Core.StartGame(mChosenDiff, mChosenLvl);
        }

        private void BtnExitClick()
        {
            Visible = false;
        }
    }
}

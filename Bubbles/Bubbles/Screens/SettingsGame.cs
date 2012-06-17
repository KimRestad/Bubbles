using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Microsoft.Xna.Framework;

namespace Bubbles
{
    public partial class SettingsGame : Form
    {
        private const int C_DIFF_EASY = 2;
        private const int C_DIFF_MEDIUM = 1;
        private const int C_DIFF_HARD = 0;

        private SettingObject mSettings;

        public SettingsGame(SettingObject settingsObject)
        {
            InitializeComponent();
            mSettings = settingsObject;
        }

        private void SettingsWindow_Load(object sender, EventArgs e)
        {
            trkDifficulty.Value = C_DIFF_EASY;
            optRowMove.Checked = true;
        }

        private void trkDifficulty_Scroll(object sender, EventArgs e)
        {
            switch (trkDifficulty.Value)
            {
                case C_DIFF_EASY:
                    SetNumberColours(4);
                    SetRowAdded(SettingObject.RowAdded.Add);
                    break;
                case C_DIFF_MEDIUM:
                    SetNumberColours(6);
                    SetRowAdded(SettingObject.RowAdded.Add);
                    break;
                case C_DIFF_HARD:
                    SetNumberColours(9);
                    SetRowAdded(SettingObject.RowAdded.Move);
                    break;
            }
        }

        private void trkNoColours_Scroll(object sender, EventArgs e)
        {
            lblShowNoCols.Text = "" + trkNoColours.Value;
        }

        private void chkDiffCustom_CheckedChanged(object sender, EventArgs e)
        {
            // If custom is chosen, disable difficulty slider, else enable
            trkDifficulty.Enabled = !chkDiffCustom.Checked;
            lblDiffEasy.Enabled = !chkDiffCustom.Checked;
            lblDiffMedium.Enabled = !chkDiffCustom.Checked;
            lblDiffHard.Enabled = !chkDiffCustom.Checked;

            // If custom is chosen, enable row group and colour controls, else disable
            trkNoColours.Enabled = chkDiffCustom.Checked;
            lblNoDiffColours.Enabled = chkDiffCustom.Checked;
            lblShowNoCols.Enabled = chkDiffCustom.Checked;
            grpRowAdded.Enabled = chkDiffCustom.Checked;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (Owner != null)
                Owner.Hide();
            this.Hide();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveSettings();
            
            if (Owner != null)
                Owner.Hide();
            this.Hide();
        }

        private void SaveSettings()
        {
            // Save settings in setting object
            mSettings.NumberOfColours = trkNoColours.Value;
            if (optRowAdd.Checked)
                mSettings.WhenRowIsAdded = SettingObject.RowAdded.Add;
            else
                mSettings.WhenRowIsAdded = SettingObject.RowAdded.Move;

            // Create the file reader and temporary variable to hold read lines.
            string path = @"Content\settings";
            FileStream file = new FileStream(path, FileMode.Create);
            StreamWriter writer = new StreamWriter(file);

            // Write to file
            if (chkDiffCustom.Checked)
            {
                writer.WriteLine("Custom");
                if(optRowAdd.Checked)
                    writer.WriteLine("Add");
                else
                    writer.WriteLine("Move");
                writer.WriteLine(lblShowNoCols.Text);
            }
            else
            {
                switch (trkDifficulty.Value)
                {
                    case C_DIFF_EASY:
                        writer.WriteLine("Easy");
                        break;
                    case C_DIFF_MEDIUM:
                        writer.WriteLine("Medium");
                        break;
                    case C_DIFF_HARD:
                        writer.WriteLine("Hard");
                        break;
                }
            }

            // Close file.
            writer.Close();
            file.Close();
        }

        private void LoadSettings()
        {
            //// Create the file reader and temporary variable to hold read lines.
            //string path = @"Content\HighScore.txt";
            //FileStream file = new FileStream(path, FileMode.Open);
            //StreamReader reader = new StreamReader(file);
            //string highScoreLine;
            //string[] highScoreEntries;

            //// Get the next line from the highscore file.
            //highScoreLine = reader.ReadLine();

            //// If it is not null or empty, split it into name and score and add to the highscore list.
            //while (highScoreLine != null && highScoreLine != "")
            //{
            //    highScoreEntries = highScoreLine.Split(',');
            //    m_highScore.Add(new KeyValuePair<string, string>(highScoreEntries[0], highScoreEntries[1]));

            //    // Only read 10 highscores.
            //    if (m_highScore.Count >= 10)
            //        break;

            //    highScoreLine = reader.ReadLine();
            //}

            //// Close file.
            //reader.Close();
            //file.Close();
        }

        private void SetRowAdded(SettingObject.RowAdded rowAdded)
        {
            //mSettings.WhenRowIsAdded = rowAdded;

            switch (rowAdded)
            {
                case SettingObject.RowAdded.Add:
                    optRowAdd.Checked = true;
                    optRowMove.Checked = false;
                    break;
                case SettingObject.RowAdded.Move:
                    optRowAdd.Checked = false;
                    optRowMove.Checked = true;
                    break;
            }
        }

        private void SetNumberColours(int colours)
        {
            colours = (int)MathHelper.Clamp(colours, SettingObject.C_MIN_COLOURS, SettingObject.C_MAX_COLOURS);
            //mSettings.NumberOfColours = colours;
            lblShowNoCols.Text = "" + colours;
            trkNoColours.Value = colours;
        }
    }

    public class SettingObject
    {
        public enum RowAdded { Add, Move }
        public const int C_MAX_COLOURS = 9;
        public const int C_MIN_COLOURS = 4;

        private int mNumberColours;
        private RowAdded mRowAdded;

        public SettingObject()
        {
            mNumberColours = 4;
            mRowAdded = RowAdded.Add;
        }

        public SettingObject(int numberOfColours, RowAdded rowAdded)
        {
            mNumberColours = numberOfColours;
            mRowAdded = rowAdded;
        }

        public int NumberOfColours
        {
            get { return mNumberColours; }
            set { mNumberColours = (int)MathHelper.Clamp(value, C_MIN_COLOURS, C_MAX_COLOURS); }
        }

        public RowAdded WhenRowIsAdded
        {
            get { return mRowAdded; }
            set { mRowAdded = value; }
        }
    }
}

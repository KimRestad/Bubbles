using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Xna.Framework;

namespace Bubbles
{
    public partial class SettingsMain : Form
    {
        SettingsGame mGameSettings;
        SettingsGraphics mGraphicsSettings;

        public SettingsMain(SettingObject settingObject)
        {
            InitializeComponent();

            mGameSettings = new SettingsGame(settingObject);
            mGraphicsSettings = new SettingsGraphics();
        }

        private void btnGraphics_Click(object sender, EventArgs e)
        {
            mGraphicsSettings.Show(this);
        }

        private void btnGame_Click(object sender, EventArgs e)
        {
            mGameSettings.Show(this);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

    }
}

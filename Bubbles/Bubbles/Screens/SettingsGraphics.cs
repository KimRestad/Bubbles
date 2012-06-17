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
    public partial class SettingsGraphics : Form
    {
        public SettingsGraphics()
        {
            InitializeComponent();

            lblComputerRes.Text += Core.UserScreenResolution.X + "x" + Core.UserScreenResolution.Y;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (Owner != null)
                Owner.Hide();
            this.Hide();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (Owner != null)
                Owner.Hide();
            this.Hide();
        }
    }
}

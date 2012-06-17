namespace Bubbles
{
    partial class SettingsGame
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.grpDifficulty = new System.Windows.Forms.GroupBox();
            this.chkDiffCustom = new System.Windows.Forms.CheckBox();
            this.lblDiffHard = new System.Windows.Forms.Label();
            this.lblDiffMedium = new System.Windows.Forms.Label();
            this.lblDiffEasy = new System.Windows.Forms.Label();
            this.trkDifficulty = new System.Windows.Forms.TrackBar();
            this.grpRowAdded = new System.Windows.Forms.GroupBox();
            this.optRowAdd = new System.Windows.Forms.RadioButton();
            this.optRowMove = new System.Windows.Forms.RadioButton();
            this.lblNoDiffColours = new System.Windows.Forms.Label();
            this.trkNoColours = new System.Windows.Forms.TrackBar();
            this.lblShowNoCols = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.grpDifficulty.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trkDifficulty)).BeginInit();
            this.grpRowAdded.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trkNoColours)).BeginInit();
            this.SuspendLayout();
            // 
            // grpDifficulty
            // 
            this.grpDifficulty.Controls.Add(this.chkDiffCustom);
            this.grpDifficulty.Controls.Add(this.lblDiffHard);
            this.grpDifficulty.Controls.Add(this.lblDiffMedium);
            this.grpDifficulty.Controls.Add(this.lblDiffEasy);
            this.grpDifficulty.Controls.Add(this.trkDifficulty);
            this.grpDifficulty.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpDifficulty.Location = new System.Drawing.Point(12, 12);
            this.grpDifficulty.Name = "grpDifficulty";
            this.grpDifficulty.Size = new System.Drawing.Size(237, 239);
            this.grpDifficulty.TabIndex = 0;
            this.grpDifficulty.TabStop = false;
            this.grpDifficulty.Text = "Choose Difficulty";
            // 
            // chkDiffCustom
            // 
            this.chkDiffCustom.AutoSize = true;
            this.chkDiffCustom.Location = new System.Drawing.Point(16, 184);
            this.chkDiffCustom.Name = "chkDiffCustom";
            this.chkDiffCustom.Size = new System.Drawing.Size(193, 29);
            this.chkDiffCustom.TabIndex = 4;
            this.chkDiffCustom.Text = "Custom Difficulty";
            this.chkDiffCustom.UseVisualStyleBackColor = true;
            this.chkDiffCustom.CheckedChanged += new System.EventHandler(this.chkDiffCustom_CheckedChanged);
            // 
            // lblDiffHard
            // 
            this.lblDiffHard.AutoSize = true;
            this.lblDiffHard.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDiffHard.Location = new System.Drawing.Point(67, 138);
            this.lblDiffHard.Name = "lblDiffHard";
            this.lblDiffHard.Size = new System.Drawing.Size(58, 25);
            this.lblDiffHard.TabIndex = 3;
            this.lblDiffHard.Text = "Hard";
            // 
            // lblDiffMedium
            // 
            this.lblDiffMedium.AutoSize = true;
            this.lblDiffMedium.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDiffMedium.Location = new System.Drawing.Point(67, 87);
            this.lblDiffMedium.Name = "lblDiffMedium";
            this.lblDiffMedium.Size = new System.Drawing.Size(88, 25);
            this.lblDiffMedium.TabIndex = 2;
            this.lblDiffMedium.Text = "Medium";
            // 
            // lblDiffEasy
            // 
            this.lblDiffEasy.AutoSize = true;
            this.lblDiffEasy.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDiffEasy.Location = new System.Drawing.Point(67, 36);
            this.lblDiffEasy.Name = "lblDiffEasy";
            this.lblDiffEasy.Size = new System.Drawing.Size(60, 25);
            this.lblDiffEasy.TabIndex = 1;
            this.lblDiffEasy.Text = "Easy";
            // 
            // trkDifficulty
            // 
            this.trkDifficulty.LargeChange = 1;
            this.trkDifficulty.Location = new System.Drawing.Point(16, 36);
            this.trkDifficulty.Maximum = 2;
            this.trkDifficulty.Name = "trkDifficulty";
            this.trkDifficulty.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trkDifficulty.Size = new System.Drawing.Size(45, 127);
            this.trkDifficulty.TabIndex = 0;
            this.trkDifficulty.Scroll += new System.EventHandler(this.trkDifficulty_Scroll);
            // 
            // grpRowAdded
            // 
            this.grpRowAdded.Controls.Add(this.optRowAdd);
            this.grpRowAdded.Controls.Add(this.optRowMove);
            this.grpRowAdded.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Bold);
            this.grpRowAdded.Location = new System.Drawing.Point(270, 12);
            this.grpRowAdded.Name = "grpRowAdded";
            this.grpRowAdded.Size = new System.Drawing.Size(335, 138);
            this.grpRowAdded.TabIndex = 1;
            this.grpRowAdded.TabStop = false;
            this.grpRowAdded.Text = "When a new row is added:";
            // 
            // optRowAdd
            // 
            this.optRowAdd.AutoSize = true;
            this.optRowAdd.Location = new System.Drawing.Point(25, 36);
            this.optRowAdd.Name = "optRowAdd";
            this.optRowAdd.Size = new System.Drawing.Size(278, 29);
            this.optRowAdd.TabIndex = 1;
            this.optRowAdd.TabStop = true;
            this.optRowAdd.Text = "Add a new row of bubbles";
            this.optRowAdd.UseVisualStyleBackColor = true;
            // 
            // optRowMove
            // 
            this.optRowMove.AutoSize = true;
            this.optRowMove.Location = new System.Drawing.Point(25, 87);
            this.optRowMove.Name = "optRowMove";
            this.optRowMove.Size = new System.Drawing.Size(269, 29);
            this.optRowMove.TabIndex = 0;
            this.optRowMove.TabStop = true;
            this.optRowMove.Text = "Move row one step down";
            this.optRowMove.UseVisualStyleBackColor = true;
            // 
            // lblNoDiffColours
            // 
            this.lblNoDiffColours.AutoSize = true;
            this.lblNoDiffColours.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Bold);
            this.lblNoDiffColours.Location = new System.Drawing.Point(269, 168);
            this.lblNoDiffColours.Name = "lblNoDiffColours";
            this.lblNoDiffColours.Size = new System.Drawing.Size(278, 25);
            this.lblNoDiffColours.TabIndex = 2;
            this.lblNoDiffColours.Text = "Number of different colours:";
            // 
            // trkNoColours
            // 
            this.trkNoColours.LargeChange = 1;
            this.trkNoColours.Location = new System.Drawing.Point(270, 206);
            this.trkNoColours.Maximum = 9;
            this.trkNoColours.Minimum = 4;
            this.trkNoColours.Name = "trkNoColours";
            this.trkNoColours.Size = new System.Drawing.Size(335, 45);
            this.trkNoColours.TabIndex = 5;
            this.trkNoColours.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this.trkNoColours.Value = 4;
            this.trkNoColours.Scroll += new System.EventHandler(this.trkNoColours_Scroll);
            // 
            // lblShowNoCols
            // 
            this.lblShowNoCols.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Bold);
            this.lblShowNoCols.Location = new System.Drawing.Point(553, 168);
            this.lblShowNoCols.Name = "lblShowNoCols";
            this.lblShowNoCols.Size = new System.Drawing.Size(52, 25);
            this.lblShowNoCols.TabIndex = 7;
            this.lblShowNoCols.Text = "4";
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Bold);
            this.btnCancel.Location = new System.Drawing.Point(26, 291);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(223, 43);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Bold);
            this.btnSave.Location = new System.Drawing.Point(374, 291);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(223, 43);
            this.btnSave.TabIndex = 9;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // SettingsGame
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(622, 366);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.lblShowNoCols);
            this.Controls.Add(this.trkNoColours);
            this.Controls.Add(this.lblNoDiffColours);
            this.Controls.Add(this.grpRowAdded);
            this.Controls.Add(this.grpDifficulty);
            this.Name = "SettingsGame";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SettingsWindow";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.SettingsWindow_Load);
            this.grpDifficulty.ResumeLayout(false);
            this.grpDifficulty.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trkDifficulty)).EndInit();
            this.grpRowAdded.ResumeLayout(false);
            this.grpRowAdded.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trkNoColours)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox grpDifficulty;
        private System.Windows.Forms.CheckBox chkDiffCustom;
        private System.Windows.Forms.Label lblDiffHard;
        private System.Windows.Forms.Label lblDiffMedium;
        private System.Windows.Forms.Label lblDiffEasy;
        private System.Windows.Forms.TrackBar trkDifficulty;
        private System.Windows.Forms.GroupBox grpRowAdded;
        private System.Windows.Forms.RadioButton optRowAdd;
        private System.Windows.Forms.RadioButton optRowMove;
        private System.Windows.Forms.Label lblNoDiffColours;
        private System.Windows.Forms.TrackBar trkNoColours;
        private System.Windows.Forms.Label lblShowNoCols;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSave;
    }
}
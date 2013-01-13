using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bubbles
{
    class OptionScreen : OverlayScreen
    {
        private class TextLine
        {
            private SpriteFont mFont;

            public string Text { get; private set; }
            public Color Colour { get; private set; }
            public Vector2 Position { get; set; }
            public float Scale { get; private set; }
            public bool LeftAlign { get; set; }

            public TextLine(string textLine, Color lineColour, Vector2 position, bool leftAlign = true, 
                            float scale = 1.0f)
            {
                mFont = Core.Content.Load<SpriteFont>(@"Fonts\button");

                Text = textLine;
                Colour = lineColour;
                LeftAlign = leftAlign;
                Scale = scale;

                if (LeftAlign)
                    Position = position;
                else
                {
                    float textWidth = mFont.MeasureString(Text).X * Scale;
                    Position = new Vector2(position.X - textWidth, position.Y);
                }
            }

            public void Draw(SpriteBatch spritebatch)
            {
                spritebatch.DrawString(mFont, Text, Position + new Vector2(2, 2), Color.Black * 0.6f, 0.0f,
                                       Vector2.Zero, Scale, SpriteEffects.None, 0.0f);
                spritebatch.DrawString(mFont, Text, Position, Colour, 0.0f, Vector2.Zero, Scale, SpriteEffects.None,
                                       0.0f);
            }
        }

        // Frame variables.
        private Texture2D mFrameHor;
        private Texture2D mFrameVer;
        private Rectangle mFrameHorPos1;
        private Rectangle mFrameHorPos2;
        private Rectangle mFrameVerPos1;
        private Rectangle mFrameVerPos2;

        // Checkboxes.
        private Checkbox mChkFullscreen;
        private Checkbox mChkShowShapes;
        private Checkbox mChkShowHelpAim;
        private Checkbox mChkPlaySounds;

        // Texts
        private List<TextLine> mShortcutKeys;
        private TextLine mSettingsTitle;
        private TextLine mShortcutTitle;
        private TextLine mInfoTitle;
        private TextLine mInfoText1;
        private TextLine mInfoText2;

        private List<Button> mButtons;

        /// <summary>
        /// Create the credits screen, an overlay screen, and its contents.
        /// </summary>
        public OptionScreen()
            : base(new Point(800, 600), new Rectangle(0, 0, Core.ClientBounds.Width, Core.ClientBounds.Height), Color.White, 0.9f)
        {
            // Load textures.
            mFrameHor = Core.Content.Load<Texture2D>("Textures/wallTop");
            mFrameVer = Core.Content.Load<Texture2D>("Textures/wallSide");

            // Calculate frame positions.
            int width = 52;
            mFrameHorPos1 = new Rectangle(mPosition.X, mPosition.Y, mPosition.Width, width);
            mFrameHorPos2 = new Rectangle(mPosition.X, mPosition.Y + mPosition.Height - width, mPosition.Width, width);
            mFrameVerPos1 = new Rectangle(mPosition.X, mPosition.Y + width, width, mPosition.Height - width * 2);
            mFrameVerPos2 = new Rectangle(mPosition.X + mPosition.Width - width, mPosition.Y + width, width, 
                                          mPosition.Height - width * 2);

            // Create checkboxes.
            int chkX = mPosition.X + 96;
            int chkStartY = mPosition.Y + 128;
            mChkFullscreen = new Checkbox(new Vector2(chkX, chkStartY), "Show in fullscreen");
            
            int chkStrideY = mChkFullscreen.Size.Y + 16;
            mChkPlaySounds = new Checkbox(new Vector2(chkX, chkStartY + chkStrideY), "Play sounds");
            mChkShowShapes = new Checkbox(new Vector2(chkX, chkStartY + chkStrideY * 2), "Show shapes on balls");
            mChkShowHelpAim = new Checkbox(new Vector2(chkX, chkStartY + chkStrideY * 3), "Show helper aim");
            
            // Create shortcut keys text list.
            mShortcutKeys = new List<TextLine>();
            float textScale = 0.8f;
            int txtEndX = mPosition.X + mPosition.Width - 96;
            Color txtKeyCol = Color.Goldenrod;
            mShortcutKeys.Add(new TextLine("Ctrl + Enter", txtKeyCol, new Vector2(txtEndX, mChkFullscreen.TextPosition.Y),
                                           false, textScale));
            mShortcutKeys.Add(new TextLine("S or F8", txtKeyCol, new Vector2(txtEndX, mChkPlaySounds.TextPosition.Y),
                                           false, textScale));
            mShortcutKeys.Add(new TextLine("F1", txtKeyCol, new Vector2(txtEndX, mChkShowShapes.TextPosition.Y), false,
                                           textScale));
            mShortcutKeys.Add(new TextLine("F2", txtKeyCol, new Vector2(txtEndX, mChkShowHelpAim.TextPosition.Y), false,
                                           textScale));

            // Create other texts.
            Color titleColour = new Color(142, 217, 184, 255);
            Color txtCol = Color.Ivory;
            int infoStartY = chkStartY + chkStrideY * 4 + 16;
            mSettingsTitle = new TextLine("Settings", titleColour, new Vector2(chkX, chkStartY - 64));
            mShortcutTitle = new TextLine("Shortcut", titleColour, new Vector2(txtEndX, chkStartY - 64), false);
            mInfoTitle = new TextLine("In-game key commands", titleColour,
                                      new Vector2(chkX, infoStartY));
            mInfoText1 = new TextLine("To pause the game, press the 'P' key.\n",
                                      txtCol, new Vector2(chkX, infoStartY + chkStrideY), true, textScale);
            mInfoText2 = new TextLine("To move the mouse cursor to the middle\nof the score board, " +
                                      "press the 'Space' key.",
                                      txtCol, new Vector2(chkX, infoStartY + chkStrideY * 2 - 8), true, textScale);
            mShortcutKeys.Add(new TextLine("P", txtKeyCol, new Vector2(chkX + 452, infoStartY + chkStrideY),
                                           true, textScale));
            mShortcutKeys.Add(new TextLine("\nSpace", txtKeyCol, new Vector2(chkX + 448, 
                                                                                 infoStartY + chkStrideY * 2 - 8),
                                           true, textScale));

            // Add buttons.
            mButtons = new List<Button>();

            int centerX = (int)(mPosition.X + mPosition.Width * 0.5f);
            int btnWidth = 336;
            int btnHeight = (int)(width * 0.95f);
            int btnHalfPadding = 16;
            int btnY = mPosition.Y + mPosition.Height - btnHeight;
            mButtons.Add(new Button(BtnCloseClick,
                                    new Rectangle(centerX - btnWidth - btnHalfPadding, btnY, btnWidth, btnHeight),
                                    "Cancel"));
            mButtons.Add(new Button(BtnSaveClick,
                                    new Rectangle(centerX + btnHalfPadding, btnY, btnWidth, btnHeight),
                                    "Save & Return"));
        }

        /// <summary>
        /// Update the screen if it is visible, along with its contents.
        /// </summary>
        public override void Update()
        {
            if (!Visible)
                return;

            base.Update();

            mChkFullscreen.Update();
            mChkPlaySounds.Update();
            mChkShowShapes.Update();
            mChkShowHelpAim.Update();

            foreach(Button btn in mButtons)
                btn.Update();
        }

        /// <summary>
        /// Draw the screen and contents if the screen is visible.
        /// </summary>
        /// <param name="spritebatch">The sprite batch to use when drawing the screen.</param>
        public override void Draw(SpriteBatch spritebatch)
        {
            if (!Visible)
                return;

            base.Draw(spritebatch);

            // Draw the frames.
            spritebatch.Draw(mFrameHor, mFrameHorPos1, null, Color.White, 0.0f, Vector2.Zero, 
                             SpriteEffects.FlipVertically, 0.0f);
            spritebatch.Draw(mFrameHor, mFrameHorPos2, null, Color.White, 0.0f, Vector2.Zero, 
                             SpriteEffects.FlipVertically, 0.0f);
            spritebatch.Draw(mFrameVer, mFrameVerPos1, null, Color.White, 0.0f, Vector2.Zero, 
                             SpriteEffects.FlipVertically, 0.0f);
            spritebatch.Draw(mFrameVer, mFrameVerPos2, null, Color.White, 0.0f, Vector2.Zero, 
                             SpriteEffects.FlipVertically | SpriteEffects.FlipHorizontally, 0.0f);

            // Draw checkboxes.
            mChkFullscreen.Draw(spritebatch);
            mChkPlaySounds.Draw(spritebatch);
            mChkShowShapes.Draw(spritebatch);
            mChkShowHelpAim.Draw(spritebatch);

            // Draw texts.
            mSettingsTitle.Draw(spritebatch);
            mShortcutTitle.Draw(spritebatch);
            mInfoTitle.Draw(spritebatch);
            mInfoText1.Draw(spritebatch);
            mInfoText2.Draw(spritebatch);
            foreach (TextLine txt in mShortcutKeys)
                txt.Draw(spritebatch);

            // Draw buttons.
            foreach (Button btn in mButtons)
                btn.Draw(spritebatch);
        }

        public void Show()
        {
            LoadPreferences();
            Visible = true;
        }

        private void LoadPreferences()
        {
            mChkFullscreen.Checked = Core.Fullscreen;
            mChkPlaySounds.Checked = Core.PlaySounds;
            mChkShowShapes.Checked = Core.ShowShapes;
            mChkShowHelpAim.Checked = Core.ShowLongAim;
        }

        /// <summary>
        /// Close button click event. Return to menu.
        /// </summary>
        private void BtnCloseClick()
        {
            Visible = false;
        }

        /// <summary>
        /// Save & Return button click event. Apply and save preferences then return to menu.
        /// </summary>
        private void BtnSaveClick()
        {
            Core.Fullscreen = mChkFullscreen.Checked;
            Core.PlaySounds = mChkPlaySounds.Checked;
            Core.ShowShapes = mChkShowShapes.Checked;
            Core.ShowLongAim = mChkShowHelpAim.Checked;

            Visible = false;
        }
    }
}

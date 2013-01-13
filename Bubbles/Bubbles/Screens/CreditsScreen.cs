using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bubbles
{
    class CreditsScreen : OverlayScreen
    {
        private class TextLine
        {
            public string Text { get; private set; }
            public Color Colour { get; private set; }
            public Vector2 Position { get; set; }
            public float Scale { get; private set; }

            public TextLine(string textLine, Color lineColour, Vector2 position, float scale = 1.0f)
            {
                Text = textLine;
                Colour = lineColour;
                Position = position;
                Scale = scale;
            }
        }

        // Text variables.
        private SpriteFont mFont;
        private List<TextLine> mLines;
        private Color mTextColour = Color.Yellow;
        private Vector2 mOffset;
        private float mMaxYOffset;

        // Frame variables.
        private Texture2D mFrameHor;
        private Texture2D mFrameVer;
        private Rectangle mFrameHorPos1;
        private Rectangle mFrameHorPos2;
        private Rectangle mFrameVerPos1;
        private Rectangle mFrameVerPos2;

        private Button mBtnClose;

        /// <summary>
        /// Create the credits screen, an overlay screen, and its contents.
        /// </summary>
        public CreditsScreen()
            : base(new Point(600, 700), new Rectangle(0, 0, Core.ClientBounds.Width, Core.ClientBounds.Height), 
                   Color.White, 0.9f)
        {
            // Load fonts and textures.
            mFont = Core.Content.Load<SpriteFont>("Fonts/credits");
            mFrameHor = Core.Content.Load<Texture2D>("Textures/wallTop");
            mFrameVer = Core.Content.Load<Texture2D>("Textures/wallSide");

            // Calculate frame positions.
            int width = 48;
            mFrameHorPos1 = new Rectangle(mPosition.X, mPosition.Y, mPosition.Width, width);
            mFrameHorPos2 = new Rectangle(mPosition.X, mPosition.Y + mPosition.Height - width, mPosition.Width, width);
            mFrameVerPos1 = new Rectangle(mPosition.X, mPosition.Y + width, width, mPosition.Height - width * 2);
            mFrameVerPos2 = new Rectangle(mPosition.X + mPosition.Width - width, mPosition.Y + width, width, 
                                          mPosition.Height - width * 2);

            // Calculate text positions.
            float textScale = 0.75f;
            int midScreenX = (int)(mPosition.X + mPosition.Width * 0.5f);
            int strideY = (int)(mFont.MeasureString("Kim").Y * textScale);
            int textX = mPosition.X + width + 16;
            int textY = mPosition.Y + mPosition.Height - width + strideY * 3;
            
            // Add text lines.
            mLines = new List<TextLine>();
            mLines.Add(new TextLine("Credits", Color.Yellow, new Vector2(textX, 
                                                                         mPosition.Y + mPosition.Height - width - 16), 
                                                                         1.5f));
            mLines.Add(new TextLine("- PROGRAMMING -", mTextColour, new Vector2(textX, textY), textScale));
            mLines.Add(new TextLine("Kim Restad", mTextColour, new Vector2(textX, textY + strideY), textScale));
            mLines.Add(new TextLine("- GRAPHICS -", mTextColour, new Vector2(textX, textY + strideY * 3), textScale));
            mLines.Add(new TextLine("Kim Restad", mTextColour, new Vector2(textX, textY + strideY * 4), textScale));
            mLines.Add(new TextLine("- TESTER -", mTextColour, new Vector2(textX, textY + strideY * 6), textScale));
            mLines.Add(new TextLine("Odd Restad", mTextColour, new Vector2(textX, textY + strideY * 7), textScale));
            mLines.Add(new TextLine("Daniel Bengtsson", mTextColour, new Vector2(textX, textY + strideY * 8),
                                    textScale));
            mLines.Add(new TextLine("- SPECIAL THANKS TO -", mTextColour, new Vector2(textX, textY + strideY * 10), 
                                    textScale));
            mLines.Add(new TextLine("My mother Susanne Restad for", mTextColour, new Vector2(textX, 
                                                                                             textY + strideY * 11), 
                                    textScale));
            mLines.Add(new TextLine("whom this game was created.", mTextColour, new Vector2(textX, 
                                                                                            textY + strideY * 12),
                                    textScale));

            // Add button and calculate end point for text scrolling.
            int height = (int)(width * 0.95f);
            mBtnClose = new Button(BtnCloseClick, new Rectangle(midScreenX - 128, 
                                                                mPosition.Y + mPosition.Height - height, 256, height), 
                                   "Close");

            mMaxYOffset = mPosition.Y + mFrameHorPos1.Height - mLines[0].Position.Y;
        }

        /// <summary>
        /// Update the screen if it is visible, along with its contents.
        /// </summary>
        public override void Update()
        {
            if (!Visible)
                return;

            base.Update();

            if (mOffset.Y > mMaxYOffset)
                mOffset.Y -= 0.9f;

            mBtnClose.Update();
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

            // Draw the text lines.
            foreach (TextLine tl in mLines)
            {
                Vector2 drawPos = tl.Position + mOffset;
                if (drawPos.Y > mPosition.Y && drawPos.Y < mPosition.Y + mPosition.Height - mFrameHorPos2.Height)
                    spritebatch.DrawString(mFont, tl.Text, drawPos, tl.Colour, 0.0f, Vector2.Zero, tl.Scale, 
                                           SpriteEffects.None, 0.0f);
            }

            // Draw the frames.
            spritebatch.Draw(mFrameHor, mFrameHorPos1, null, Color.White, 0.0f, Vector2.Zero, 
                             SpriteEffects.FlipVertically, 0.0f);
            spritebatch.Draw(mFrameHor, mFrameHorPos2, null, Color.White, 0.0f, Vector2.Zero, 
                             SpriteEffects.FlipVertically, 0.0f);
            spritebatch.Draw(mFrameVer, mFrameVerPos1, null, Color.White, 0.0f, Vector2.Zero, 
                             SpriteEffects.FlipVertically, 0.0f);
            spritebatch.Draw(mFrameVer, mFrameVerPos2, null, Color.White, 0.0f, Vector2.Zero, 
                             SpriteEffects.FlipVertically | SpriteEffects.FlipHorizontally, 0.0f);

            mBtnClose.Draw(spritebatch);
        }

        /// <summary>
        /// Close button click event. Reset offset and set visible to false.
        /// </summary>
        private void BtnCloseClick()
        {
            mOffset = Vector2.Zero;
            Visible = false;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Bubbles
{
    public delegate void OnClick();

    class Button
    {
        // Text variables.
        private string mCaption;
        private SpriteFont mFont;
        private Vector2 mTextPos;
        private Vector2 mTextShadowPos;
        private Color mTextColour;

        // Button variables.
        private Texture2D mBackground;
        private Rectangle mPosition;
        private Color mTint;
        private Color mHighlight;

        // Functionality variables.
        private OnClick mOnClickMethod;
        private MouseState mPrevMouse;
        private bool mHovered;
        private bool mVisible;
        private bool mEnabled;
        private bool mMarked;

        // Static variables.
        private static SoundEffect sSound;

        public Button(OnClick onClickMethod, Rectangle position, string caption)
        {
            // Save click method and initialise state variables.
            mOnClickMethod = onClickMethod;
            mHovered = false;
            mVisible = true;
            mEnabled = true;
            mMarked = false;

            // Load texture and save information about its appearance.
            mBackground = Core.Content.Load<Texture2D>("Textures/button");
            mPosition = position;
            mTint = Color.White;
            mHighlight = new Color(2, 84, 85, 255);

            // Save caption, its colour, position and load the font.
            mCaption = caption;
            mTextColour = Color.Ivory;
            mFont = Core.Content.Load<SpriteFont>("Fonts/button");

            Vector2 textSize = mFont.MeasureString(mCaption);
            mTextPos = new Vector2(mPosition.X + (mPosition.Width - textSize.X) * 0.5f,
                                   mPosition.Y + (mPosition.Height - textSize.Y) * 0.5f);
            mTextShadowPos = mTextPos + new Vector2(2, 2);
        }

        /// <summary>
        /// Updates the button only if it is visible. If it is clicked, the OnClick method provided
        /// with the constructor is called.
        /// </summary>
        public void Update()
        {
            if (!mVisible || !mEnabled)
                return;

            MouseState currMouse = Mouse.GetState();
            Rectangle mouseRect = new Rectangle((int)currMouse.X, (int)currMouse.Y, 1, 1);

            mHovered = mouseRect.Intersects(mPosition);

            if(!mMarked)
                if (mHovered)
                    mTint = mHighlight;
                else
                    mTint = Color.White;

            if (currMouse.LeftButton == ButtonState.Released && mPrevMouse.LeftButton == ButtonState.Pressed)
            {
                if (mHovered)
                    Click();
            }

            mPrevMouse = currMouse;
        }

        /// <summary>
        /// Draws the button only if it is visible.
        /// </summary>
        /// <param name="spriteBatch">The spritebatch to use when drawing the button.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            if (!mVisible)
                return;

            if (mEnabled)
            {
                spriteBatch.Draw(mBackground, mPosition, mTint);
                spriteBatch.DrawString(mFont, mCaption, mTextShadowPos, Color.Black);
                spriteBatch.DrawString(mFont, mCaption, mTextPos, mTextColour);
            }
            else
            {
                spriteBatch.Draw(mBackground, mPosition, mTint);
                spriteBatch.DrawString(mFont, mCaption, mTextShadowPos, Color.Black * 0.6f);
                spriteBatch.DrawString(mFont, mCaption, mTextPos, mTextColour * 0.6f);
            }

        }

        public void Click()
        {
            OnClick();
            mOnClickMethod();
        }

        private void OnClick()
        {
            if (sSound != null)
                sSound.Play();
        }

        /// <summary>
        /// Read only. Returns whether the mouse is currently hovering the button.
        /// </summary>
        public bool Hovered
        {
            get { return mHovered; }
        }

        /// <summary>
        /// Gets or sets whether the button is visible or not.
        /// </summary>
        public bool Visible
        {
            get { return mVisible; }
            set { mVisible = value; }
        }

        /// <summary>
        /// Gets or sets whether the button is clickable or not.
        /// </summary>
        public bool Enabled
        {
            get { return mEnabled; }
            set
            {
                mEnabled = value;
                if (mEnabled)
                {
                    mTint = Color.White;
                    mTextColour = Color.White;
                }
                else
                {
                    mTint = Color.LightGray;
                    mTextColour = Color.Gray;
                }
            }
        }

        /// <summary>
        /// Gets or sets whether the button is marked or not.
        /// </summary>
        public bool Marked
        {
            get { return mMarked; }
            set 
            { 
                mMarked = value;
                if (mMarked)
                    mTint = mHighlight;
            }
        }

        /// <summary>
        /// Read only. Returns the center position of the button.
        /// </summary>
        public Vector2 Center
        {
            get
            {
                return new Vector2(mPosition.X + mPosition.Width * 0.5f,
                                   mPosition.Y + mPosition.Height * 0.5f);
            }
        }

        /// <summary>
        /// Initialise the button class. Loads the button click sound.
        /// </summary>
        public static void Initialize()
        {
            sSound = Core.Content.Load<SoundEffect>(@"Sounds\button");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Bubbles
{
    class Checkbox
    {
        private Texture2D mTexNormal;
        private Texture2D mTexChecked;
        private Texture2D mTexBG;

        private Vector2 mTextPos;
        private Rectangle mBoxPos;
        private Rectangle mBoundingBox;

        private SpriteFont mFont;
        private string mCaption;
        private float mTextScale;

        private Color mBGColour;
        private Color mDrawColour;
        private Color mDisabledColour;
        private Color mNormalColour;
        private Color mHighlightColour;

        private bool mShowBG;
        private bool mChecked;
        private bool mEnabled;
        private bool mVisible;

        private MouseState mPrevMS;

        public Checkbox(Vector2 position, string caption, Color? bgColour = null)
        {
            mTexNormal = Core.Content.Load<Texture2D>(@"Textures\chkNormal");
            mTexChecked = Core.Content.Load<Texture2D>(@"Textures\chkChecked");
            mTexBG = Core.Content.Load<Texture2D>(@"Textures\chkBG");
            mFont = Core.Content.Load<SpriteFont>(@"Fonts\button");

            mCaption = caption;
            mTextScale = 0.8f;
            Point size = new Point(40, 40);
            mBoxPos = new Rectangle((int)(position.X), (int)(position.Y), size.X, size.Y);

            Vector2 textSize = mFont.MeasureString(mCaption) * mTextScale;
            int xPadding = (int)(mBoxPos.Width * 0.125f);
            int yOffset = (int)((mBoxPos.Height - textSize.Y) * 0.5f);
            mTextPos = new Vector2(mBoxPos.X + mBoxPos.Width + xPadding, mBoxPos.Y + yOffset);

            mBoundingBox = new Rectangle((int)mTextPos.X - xPadding, (int)mTextPos.Y,
                                         (int)textSize.X + xPadding, (int)textSize.Y);

            if (bgColour == null)
            {
                mShowBG = false;
                mBGColour = Color.Transparent;
            }
            else
            {
                mShowBG = true;
                mBGColour = (Color)bgColour;
            }

            mDisabledColour = Color.DarkGray;
            mNormalColour = Color.Ivory;
            mHighlightColour = Color.Goldenrod;
            mDrawColour = mNormalColour;

            mChecked = true;
            mEnabled = true;
            mVisible = true;
        }

        public void Update()
        {
            if (!mVisible || !mEnabled)
                return;

            MouseState currMS = Mouse.GetState();
            Rectangle mouseRect = new Rectangle(currMS.X, currMS.Y, 1, 1);

            if (mBoundingBox.Intersects(mouseRect) || mBoxPos.Intersects(mouseRect))
            {
                mDrawColour = mHighlightColour;

                if (currMS.LeftButton == ButtonState.Pressed && mPrevMS.LeftButton == ButtonState.Released)
                    mChecked = !mChecked;
            }
            else
                mDrawColour = mNormalColour;

            mPrevMS = currMS;
        }

        public void Draw(SpriteBatch sb)
        {
            if (!mVisible)
                return;

            if (mShowBG)
                sb.Draw(mTexBG, mBoxPos, mBGColour);

            if (mChecked)
                sb.Draw(mTexChecked, mBoxPos, mDrawColour);
            else
                sb.Draw(mTexNormal, mBoxPos, mDrawColour);

            sb.DrawString(mFont, mCaption, mTextPos + new Vector2(2, 2), Color.Black * 0.6f, 0.0f, Vector2.Zero,
                          mTextScale, SpriteEffects.None, 0.0f);
            sb.DrawString(mFont, mCaption, mTextPos, mDrawColour, 0.0f, Vector2.Zero, mTextScale, SpriteEffects.None,
                          0.0f);
        }

        public bool Enabled
        {
            get { return mEnabled; }
            set
            {
                mEnabled = value;
                if (mEnabled)
                    mDrawColour = mNormalColour;
                else
                    mDrawColour = mDisabledColour;
            }
        }

        public bool Visible
        {
            get { return mVisible; }
            set { mVisible = value; }
        }

        public bool Checked
        {
            get { return mChecked; }
            set { mChecked = value; }
        }

        public Point Size
        {
            get 
            {
                Vector2 textSize = mFont.MeasureString(mCaption) * mTextScale;
                return new Point((int)(mTextPos.X + textSize.X) - mBoxPos.X, mBoxPos.Height); 
            }
        }


        public Vector2 TextPosition
        {
            get { return mTextPos; }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace Bubbles
{
    class Button
    {
        private Rectangle mPosition;
        private bool mHovered;

        private string mCaption;
        private SpriteFont mFont;
        private Vector2 mTextPosition;
        private Vector2 mShadowPosition;
        private Color mTextColour = Color.Ivory;

        private Texture2D mBackground;
        private Color mTint;
        private Color mHighlight = new Color(2, 84, 85, 30); //new Color(0, 255, 0, 60);

        public Button(Rectangle position, string caption)
        {
            mPosition = position;
            mHovered = false;

            mCaption = caption;
            mFont = Core.Content.Load<SpriteFont>(@"Fonts\button");

            Vector2 textSize = mFont.MeasureString(mCaption);
            mTextPosition = new Vector2(mPosition.X + (mPosition.Width - textSize.X) * 0.5f, 
                                        mPosition.Y + (mPosition.Height - textSize.Y) * 0.5f);
            mShadowPosition = mTextPosition + new Vector2(3, 3);

            mBackground = Core.Content.Load<Texture2D>(@"Textures\button");  
        }

        public void Update()
        {
            MouseState mouse = Mouse.GetState();
            Rectangle mouseRect = new Rectangle(mouse.X, mouse.Y, 1, 1);

            mHovered = mouseRect.Intersects(mPosition);

            if (mHovered)
                mTint = mHighlight;
            else
                mTint = Color.White;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(mBackground, mPosition, mTint);
            spriteBatch.DrawString(mFont, mCaption, mShadowPosition, Color.Black);
            spriteBatch.DrawString(mFont, mCaption, mTextPosition, mTextColour);
        }

        public bool IsHovered()
        {
            return mHovered;
        }
    }
}

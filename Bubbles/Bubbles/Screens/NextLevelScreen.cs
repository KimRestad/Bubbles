using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Bubbles
{
    class NextLevelScreen : OverlayScreen
    {
        private SpriteFont mFont;
        private Vector2 mTextPosition;
        private string mText;

        public NextLevelScreen()
            : base(new Rectangle(0, 100, Core.ClientBounds.Width, 300), Color.Black, 0.75f)
        {
            mFont = Core.Content.Load<SpriteFont>("Fonts/credits");
        }

        public override void Update()
        {
            if (!Visible)
                return;

            if (Mouse.GetState().RightButton == ButtonState.Pressed)
                Visible = false;
        }

        public void Draw(SpriteBatch spritebatch)
        {
            if (!Visible)
                return;

            base.Draw(spritebatch);

            spritebatch.DrawString(mFont, mText, mTextPosition, Color.Yellow);
        }

        public void Show(Level nextLevel)
        {
            Visible = true;

            mText = "LEVEL WON\nNext level: " + nextLevel.ToString() + ".\n(Right click to continue)";
            Vector2 textSize = mFont.MeasureString(mText);
            mTextPosition = new Vector2(mPosition.X + (mPosition.Width - textSize.X) * 0.5f,
                                        mPosition.Y + (mPosition.Height - textSize.Y) * 0.5f);
        }
    }
}

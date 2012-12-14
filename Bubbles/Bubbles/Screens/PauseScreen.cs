using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bubbles
{
    class PauseScreen : OverlayScreen
    {
        private SpriteFont mFont;
        private Vector2 mTextPosition;
        private string mText;

        public PauseScreen()
            : base(new Rectangle(0, 100, Core.ClientBounds.Width, 300), Color.Black, 0.75f)
        {
            mFont = Core.Content.Load<SpriteFont>("Fonts/credits");
            mText = "GAME PAUSED!\nRight click to unpause.";
            Vector2 textSize = mFont.MeasureString(mText);
            mTextPosition = new Vector2(mPosition.X + (mPosition.Width - textSize.X) * 0.5f,
                                        mPosition.Y + (mPosition.Height - textSize.Y) * 0.5f);
        }

        public override void Draw(SpriteBatch spritebatch)
        {
            if (!Visible)
                return;

            base.Draw(spritebatch);

            spritebatch.DrawString(mFont, mText, mTextPosition, Color.Yellow);
        }
    }
}

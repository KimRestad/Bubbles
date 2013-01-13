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
        // Text variables.
        private SpriteFont mFont;
        private Vector2 mTextPosition;
        private string mText;

        /// <summary>
        /// Create and initialise the pause screen.
        /// </summary>
        public PauseScreen()
            : base(new Rectangle(0, 100, Core.ClientBounds.Width, 300), Color.Black, 0.75f)
        {
            // Load font and save text and its position.
            mFont = Core.Content.Load<SpriteFont>("Fonts/credits");
            mText = "GAME PAUSED!\nRight click to unpause.";
            Vector2 textSize = mFont.MeasureString(mText);
            mTextPosition = new Vector2(mPosition.X + (mPosition.Width - textSize.X) * 0.5f,
                                        mPosition.Y + (mPosition.Height - textSize.Y) * 0.5f);
        }

        /// <summary>
        /// Draw the screen if it is visible.
        /// </summary>
        /// <param name="spritebatch">The sprite batch to use when drawing the screen.</param>
        public override void Draw(SpriteBatch spritebatch)
        {
            if (!Visible)
                return;

            base.Draw(spritebatch);

            spritebatch.DrawString(mFont, mText, mTextPosition, Color.Yellow);
        }
    }
}

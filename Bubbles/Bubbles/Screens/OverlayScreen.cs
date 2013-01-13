using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bubbles
{
    class OverlayScreen
    {
        // Screen draw variables.
        private Texture2D mBGTexture;
        private Color mBGColour;

        protected Rectangle mPosition;

        /// <summary>
        /// Create an overlay screen of the specified size and colour, with the selected transparency level and
        /// centered in the specified rectangle.
        /// </summary>
        /// <param name="size">The size of the screen.</param>
        /// <param name="centerIn">The bounds where the screen is to be centered.</param>
        /// <param name="colour">The background colour of the screen.</param>
        /// <param name="transparency">The transparency factor of the screen.</param>
        public OverlayScreen(Point size, Rectangle centerIn, Color colour, float transparency)
            : this(new Rectangle(centerIn.X + (int)((centerIn.Width - size.X) * 0.5f), 
                                 centerIn.Y + (int)((centerIn.Height - size.Y) * 0.5f), size.X, size.Y)
            , colour
            , transparency)
        {
        }

        /// <summary>
        /// Create a screen by specifying the position, colour and transparency of the screen.
        /// </summary>
        /// <param name="position">The position and size of the screen.</param>
        /// <param name="colour">The background colour of the screen.</param>
        /// <param name="transparency">The transparency factor of the screen.</param>
        public OverlayScreen(Rectangle position, Color colour, float transparency)
        {
            mBGTexture = Core.Content.Load<Texture2D>("Textures/bricks");
            mBGColour = colour * transparency;

            mPosition = position;

            Visible = false;
        }

        public virtual void Update()
        {
        }

        /// <summary>
        /// Draw the screen if it is visible.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to use when drawing the screen.</param>
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (!Visible)
                return;

            spriteBatch.Draw(mBGTexture, mPosition, mBGColour);
        }

        /// <summary>
        /// Get or set whether the screen is visible or not.
        /// </summary>
        public bool Visible { get; set; }
    }
}

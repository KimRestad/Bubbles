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

        public OverlayScreen(Point size, Rectangle centerIn, Color colour, float transparency)
            : this(new Rectangle(centerIn.X + (int)((centerIn.Width - size.X) * 0.5f), 
                                 centerIn.Y + (int)((centerIn.Height - size.Y) * 0.5f), size.X, size.Y)
            , colour
            , transparency)
        {
        }

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

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (!Visible)
                return;

            spriteBatch.Draw(mBGTexture, mPosition, mBGColour);
        }

        public bool Visible { get; set; }
    }
}

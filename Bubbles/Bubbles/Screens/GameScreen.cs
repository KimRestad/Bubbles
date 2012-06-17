using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Bubbles
{
    public class GameScreen
    {
        private Texture2D mBackground;
        private Rectangle mBGPosition;

        private Aim mAim;

        public GameScreen()
        {
            mBackground = Core.Content.Load<Texture2D>(@"Textures\background");
            mBGPosition = new Rectangle(0, 0, Core.ClientBounds.Width, Core.ClientBounds.Height);    
        }

        public void StartGame()
        {
            Ball.InitializeTextures(4);
            mAim = new Aim();
        }

        public void Update(GameTime gameTime)
        {
            mAim.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(mBackground, mBGPosition, Color.Red);
            mAim.Draw(spriteBatch);
        }
    }
}

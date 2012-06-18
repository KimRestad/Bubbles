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
        private Rectangle mBoard;

        private Aim mAim;

        public GameScreen()
        {
            mBackground = Core.Content.Load<Texture2D>(@"Textures\background");
            mBGPosition = new Rectangle(0, 0, Core.ClientBounds.Width, Core.ClientBounds.Height);
            mBoard = new Rectangle(0, 0, 900, 600);
        }

        public void StartGame()
        {
            Ball.InitializeTextures(9, mBoard);
            mAim = new Aim(mBoard);
        }

        public void Update(GameTime gameTime)
        {
            mAim.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(mBackground, mBoard, Color.Red);
            mAim.Draw(spriteBatch);
        }
    }
}

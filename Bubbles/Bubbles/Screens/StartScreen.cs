using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Bubbles
{
    public class StartScreen
    {
        // Settings objects
        private SettingsMain mSettingsWindow;
        private SettingObject mSettings;
        
        // Graphics
        private SpriteFont mDefault;
        private Texture2D mBackground;
        private Rectangle mBGPosition;

        Button mBtnPlay;
        Button mBtnSettings;
        Button mBtnExit;

        MouseState mPrevMouse;

        public StartScreen()
        {
            // Create settings objects
            mSettings = new SettingObject();
            mSettingsWindow = new SettingsMain(mSettings);

            mDefault = Core.Content.Load<SpriteFont>(@"Fonts\default");
            mBackground = Core.Content.Load<Texture2D>(@"Textures\background");
            mBGPosition = new Rectangle(0, 0, Core.ClientBounds.Width, Core.ClientBounds.Height);

            int buttonWidth = 300;
            int buttonHeight = 100;
            int buttonStartY = (int)((Core.ClientBounds.Height + buttonHeight) * 0.5);

            int x = (int)((Core.ClientBounds.Width - buttonWidth) * 0.5);

            mBtnPlay = new Button(new Rectangle(x, buttonStartY, buttonWidth, buttonHeight), "Play");
            mBtnSettings = new Button(new Rectangle(x, buttonStartY + buttonHeight + 25, buttonWidth, buttonHeight),
                                  "Settings");
            mBtnExit = new Button(new Rectangle(x, buttonStartY + (buttonHeight + 25) * 2, buttonWidth, buttonHeight),
                                  "Exit");
        }

        public void Update()
        {
            MouseState currMouse = Mouse.GetState();

            mBtnPlay.Update();
            mBtnSettings.Update();
            mBtnExit.Update();

            // If the mousebutton just has been pressed, and the settings window is not showing, then find out if
            // a button was clicked and act accordingly.
            if (currMouse.LeftButton == ButtonState.Pressed && mPrevMouse.LeftButton == ButtonState.Released
                && !mSettingsWindow.Visible)
            {
                if (mBtnPlay.IsHovered())
                    Core.StartGame();
                    //Core.Gamestate = GameState.InGame;
                else if (mBtnSettings.IsHovered())
                    mSettingsWindow.Show();
                else if (mBtnExit.IsHovered())
                    Core.Exit();
            }

            mPrevMouse = currMouse;
        }

        public void Draw(SpriteBatch spritebatch)
        {
            spritebatch.Draw(mBackground, mBGPosition, Color.White);
            spritebatch.DrawString(mDefault, "Settings is visible: "  + mSettingsWindow.Visible, Vector2.Zero, Color.White);

            mBtnPlay.Draw(spritebatch);
            mBtnSettings.Draw(spritebatch);
            mBtnExit.Draw(spritebatch);
        }
    }
}

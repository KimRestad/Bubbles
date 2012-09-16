﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Bubbles
{
    enum BallColour
    {
        Red, Blue, Green, Yellow, Purple, Turquoise, Orange, Pink, Black, Count
    }

    enum BallState
    {
        Still, Shot, Falling
    }

    class Ball
    {
        #region Variables

        // 
        private BallColour mColour;
        private Vector2 mOrigin;

        private BallState mState;
        private Vector2 mDirection;
        private Vector2 mPosition;

        #endregion Variables

        #region StaticVariables

        // Appearance variables
        private static Texture2D sTexture;
        private static List<Color> sColours;
        private static Vector2 sSize;
        private static float sScale = 1.0f;

        // Constants
        public const float C_SPEED = 10.0f;

        #endregion StaticVariables
        
        #region Methods

        public Ball(BallColour colour, Vector2 position)
        {
            if (sColours.Count <= 0)
                throw new Exception("Textures not initialized");

            mColour = (BallColour)MathHelper.Clamp((float)colour, 0, sColours.Count - 1);
            mOrigin = sSize * 0.5f;

            mState = BallState.Still;
            mDirection = Vector2.Zero;
            mPosition = position;
        }

        public void Update(ref Board board)
        {
            switch (mState)
            {
                case BallState.Still:
                    break;
                case BallState.Shot:
                    Move(ref board);
                    break;
                case BallState.Falling:
                    break;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(sTexture, mPosition, null, sColours[(int)mColour], 0.0f,
                             mOrigin, sScale, SpriteEffects.None, 0.0f);
        }

        public void Shoot(Vector2 direction)
        {
            mState = BallState.Shot;
            mDirection = direction;
            mDirection.Normalize();
        }

        private void Move(ref Board board)
        {
            mPosition += mDirection * C_SPEED;

            if (mPosition.X - Size.X * 0.5f < (board.InnerBounds.X) ||
                mPosition.X + Size.X * 0.5f > (board.InnerBounds.X + board.InnerBounds.Width))
                mDirection.X = -mDirection.X;

            if (board.Collision(this))
                mState = BallState.Still;
        }

        #endregion Methods

        #region Properties

        public Vector2 Position
        {
            get { return mPosition; }
            set { mPosition = value; }
        }

        public BallState State
        {
            get { return mState; }
        }

        public Vector2 Direction
        {
            get { return mDirection; }
        }

        public BallColour Colour
        {
            get { return mColour; }
            // DEBUG: public?
            set { mColour = value; }
        }

        #endregion Properties

        #region StaticMethods
        public static void Initialize(int numColours)
        {
            sColours = new List<Color>();
            numColours = (int)MathHelper.Clamp(numColours, 4, 9);

            switch (numColours)
            {
                case 4:
                    sColours.Insert(0, Color.Gold);             // Yellow
                    sColours.Insert(0, Color.LawnGreen);        // Green
                    sColours.Insert(0, Color.RoyalBlue);        // Blue
                    sColours.Insert(0, Color.Red);              // Red
                    break;
                case 5:
                    sColours.Insert(0, Color.Fuchsia);          // Purple
                    goto case 4;
                case 6: 
                    sColours.Insert(0, Color.Aqua);             // Turquoise
                    goto case 5;
                case 7:
                    sColours.Insert(0, Color.DarkOrange);       // Orange
                    goto case 6;
                case 8:
                    sColours.Insert(0, Color.LightPink);        // Pink
                    goto case 7;
                case 9:
                    sColours.Insert(0, Color.DimGray);          // Black
                    goto case 8;
            }

            sTexture = Core.Content.Load<Texture2D>(@"Textures\ballWhite");
            sSize = new Vector2(sTexture.Width, sTexture.Height);
        }
        #endregion StaticMethods

        #region StaticProperties

        public static float Scale
        {
            get { return sScale; }
            set { sScale = value; }
        }

        public static Vector2 Size
        {
            get { return new Vector2(sTexture.Width * sScale, sTexture.Height * sScale); }
        }

        public static int NumColours
        {
            get { return sColours.Count; }
        }

        #endregion StaticProperties
    }
}

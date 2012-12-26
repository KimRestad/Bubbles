using System;
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
        Red, Blue, Green, Yellow, Purple, Turquoise, Orange, Pink, Black, DarkGreen, Count
    }

    enum BallState
    {
        Still, Shot, Falling, Exploding, Dead
    }

    class Ball
    {
        #region Variables

        // Appearance variables.
        private BallColour mColour;
        private Vector2 mOrigin;
        
        // Behaviour variables.
        private BallState mState;
        private Vector2 mDirection;
        private Vector2 mPosition;
        private float mExplodeScale;

        #endregion Variables

        #region StaticVariables

        // Appearance variables
        private static Texture2D sTexBall;
        private static Texture2D[] sTexShapes;
        private static List<Color> sColours;
        private static Vector2 sSize;
        private static float sScale = 1.0f;
        private static bool sShowShapes;

        // Constants
        public const float C_SPEED = 12.0f;
        //public const BallColour C_NON_SHOOTABLE_COLOUR = BallColour.Red;

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

        public void Update(Board board)
        {
            switch (mState)
            {
                case BallState.Shot:
                    Move(ref board);
                    break;
                case BallState.Falling:
                    AnimateJump(ref board);
                    break;
                case BallState.Exploding:
                    AnimateExplosion();
                    break;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (mState == BallState.Exploding)
                spriteBatch.Draw(sTexBall, mPosition, null, sColours[(int)mColour], 0.0f,
                                 mOrigin, sScale * mExplodeScale, SpriteEffects.None, 0.0f);
            else
            {
                spriteBatch.Draw(sTexBall, mPosition, null, sColours[(int)mColour], 0.0f,
                                 mOrigin, sScale, SpriteEffects.None, 0.0f);
                if (sShowShapes && sTexShapes[(int)mColour] != null)
                {
                    spriteBatch.Draw(sTexShapes[(int)mColour], mPosition, null, Color.DarkGray,
                                     0.0f, mOrigin, sScale, SpriteEffects.None, 0.0f);
                }
            }
        }

        public void Shoot(Vector2 direction)
        {
            mState = BallState.Shot;
            mDirection = direction;
            mDirection.Normalize();
        }

        public void StartAnimation(bool goLeft)
        {
            mState = BallState.Falling;
            if (goLeft)
                mDirection = new Vector2(-1, -1);
            else
                mDirection = new Vector2(1, -1);

            mDirection.Normalize();
        }

        public void StartExploding()
        {
            mState = BallState.Exploding;
            mExplodeScale = 1.0f;
        }

        private void Move(ref Board board)
        {
            mPosition += mDirection * C_SPEED;

            if (mPosition.X - Size.X * 0.5f < (board.InnerBounds.X) ||
                mPosition.X + Size.X * 0.5f > (board.InnerBounds.X + board.InnerBounds.Width))
                mDirection.X = -mDirection.X;

            if (board.Collision(this) && mState == BallState.Shot)
                mState = BallState.Still;
        }

        private void AnimateJump(ref Board board)
        {
            mDirection += new Vector2(0, 0.05f);
            mPosition += mDirection * C_SPEED;

            // If the ball hits any of the walls, it is dead.
            if (mPosition.X - Size.X * 0.5f < (board.InnerBounds.X) ||
                mPosition.X + Size.X * 0.5f > (board.InnerBounds.X + board.InnerBounds.Width) ||
                mPosition.Y - Size.Y * 0.5f < board.InnerBounds.Y)
                mState = BallState.Dead;

            // If the ball hits the floor, it is dead.
            if (mPosition.Y > board.InnerBounds.Y + board.InnerBounds.Height)
                mState = BallState.Dead;
        }

        private void AnimateExplosion()
        {
            mExplodeScale -= 0.05f;
            if (mExplodeScale <= 0.0f)
                mState = BallState.Dead;
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
            private set { mColour = value; }
        }

        #endregion Properties

        #region StaticMethods
        public static void Initialize(int numColours, float ballSize)
        {
            sColours = new List<Color>();
            numColours = (int)MathHelper.Clamp(numColours, 4, 10);

            sTexShapes = new Texture2D[numColours];

            switch (numColours)
            {
                case 4:
                    sColours.Insert(0, new Color(255, 230, 0));     // Yellow
                    sColours.Insert(0, Color.LawnGreen);            // Green
                    sColours.Insert(0, Color.RoyalBlue);            // Blue
                    sColours.Insert(0, Color.Red);                  // Red
                    sTexShapes[0] = Core.Content.Load<Texture2D>(@"Textures\shLineHor");
                    sTexShapes[1] = Core.Content.Load<Texture2D>(@"Textures\shRomb");
                    sTexShapes[2] = Core.Content.Load<Texture2D>(@"Textures\shTriangleUp");
                    sTexShapes[3] = Core.Content.Load<Texture2D>(@"Textures\shSquare");
                    break;
                case 5:
                    sColours.Insert(0, new Color(230, 0, 230));     // Purple
                    sTexShapes[4] = Core.Content.Load<Texture2D>(@"Textures\shTriangleDown");
                    goto case 4;
                case 6: 
                    sColours.Insert(0, Color.Aqua);                 // Turquoise
                    sTexShapes[5] = Core.Content.Load<Texture2D>(@"Textures\shCircle");
                    goto case 5;
                case 7:
                    sColours.Insert(0, new Color(255, 90, 0));     // Orange
                    sTexShapes[6] = Core.Content.Load<Texture2D>(@"Textures\shStar");
                    goto case 6;
                case 8:
                    sColours.Insert(0, Color.LightPink);            // Pink
                    sTexShapes[7] = Core.Content.Load<Texture2D>(@"Textures\shLineVer");
                    goto case 7;
                case 9:
                    sColours.Insert(0, new Color(90, 90, 90));      // Black
                    sTexShapes[8] = null;
                    goto case 8;
                case 10:
                    sColours.Insert(0, Color.Green);                // Dark Green
                    sTexShapes[9] = Core.Content.Load<Texture2D>(@"Textures\shPoint");
                    goto case 9;
            }

            sTexBall = Core.Content.Load<Texture2D>(@"Textures\ballWhite");
            sSize = new Vector2(sTexBall.Width, sTexBall.Height);
            sScale = ballSize;
            sShowShapes = true;
        }
        #endregion StaticMethods

        #region StaticProperties

        public static float Scale
        {
            get { return sScale; }
            set { sScale = value; }
        }

        public static bool ShowShapes
        {
            get { return sShowShapes; }
            set { sShowShapes = value; }
        }

        public static Vector2 Size
        {
            get { return new Vector2(sTexBall.Width * sScale, sTexBall.Height * sScale); }
        }

        public static int ColourCount
        {
            get { return sColours.Count; }
        }

        #endregion StaticProperties
    }
}

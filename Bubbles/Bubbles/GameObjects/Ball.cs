using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Bubbles
{
    /// <summary>
    /// The possible colours for a ball. Count gives the total number of valid colours.
    /// </summary>
    enum BallColour
    {
        Red, Blue, Green, Yellow, Purple, Turquoise, Orange, Pink, Black, DarkGreen, Count
    }

    /// <summary>
    /// The possible states for a ball.
    /// </summary>
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
        
        // Constants.
        public const float C_SPEED = 0.8f;
        //public const BallColour C_NON_SHOOTABLE_COLOUR = BallColour.Red;

        // Appearance variables.
        private static Texture2D sTexBall;
        private static Texture2D[] sTexShapes;
        private static List<Color> sColours;
        private static Vector2 sSize;
        private static float sScale = 1.0f;

        #endregion StaticVariables
        
        #region Methods

        /// <summary>
        /// Creates a new ball. Ball class must be initialised by calling Ball.Initialise or an exception is thrown.
        /// </summary>
        /// <param name="colour">The colour of the ball. Is clamped to a valid colour.</param>
        /// <param name="position">The position of the ball.</param>
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

        /// <summary>
        /// Updates the ball based on the state.
        /// </summary>
        /// <param name="board">The board the ball is moving in.</param>
        /// <param name="gameTime">The game time.</param>
        public void Update(Board board, GameTime gameTime)
        {
            float dt = gameTime.ElapsedGameTime.Milliseconds;

            switch (mState)
            {
                case BallState.Shot:
                    Move(ref board, dt);
                    break;
                case BallState.Falling:
                    AnimateJump(ref board, dt);
                    break;
                case BallState.Exploding:
                    AnimateExplosion();
                    break;
            }
        }

        /// <summary>
        /// Draws the ball or the animation. If specified, the helper shape is drawn on the ball.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch used when drawing the ball.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            // If the ball is currently exploding, draw explosion animation
            if (mState == BallState.Exploding)
                spriteBatch.Draw(sTexBall, mPosition, null, sColours[(int)mColour], 0.0f,
                                 mOrigin, sScale * mExplodeScale, SpriteEffects.None, 0.0f);
            // Else draw the ball and, if specified, the helper shape.
            else
            {
                spriteBatch.Draw(sTexBall, mPosition, null, sColours[(int)mColour], 0.0f,
                                 mOrigin, sScale, SpriteEffects.None, 0.0f);
                if (Core.ShowShapes && sTexShapes[(int)mColour] != null)
                {
                    spriteBatch.Draw(sTexShapes[(int)mColour], mPosition, null, Color.DarkGray,
                                     0.0f, mOrigin, sScale, SpriteEffects.None, 0.0f);
                }
            }
        }

        /// <summary>
        /// Shoot the ball in the specified direction.
        /// </summary>
        /// <param name="direction">The direction to shoot the ball in.</param>
        public void Shoot(Vector2 direction)
        {
            mState = BallState.Shot;
            mDirection = direction;
            mDirection.Normalize();
        }

        /// <summary>
        /// Start the "jump"-animation to the left or right based on the goLeft parameter.
        /// </summary>
        /// <param name="goLeft">Specifies whether the ball should jump left or right.</param>
        public void StartJumpAnimation(bool goLeft)
        {
            mState = BallState.Falling;
            if (goLeft)
                mDirection = new Vector2(-1, -1);
            else
                mDirection = new Vector2(1, -1);

            mDirection.Normalize();
        }

        /// <summary>
        /// Start the explosion animation.
        /// </summary>
        public void StartExploding()
        {
            mState = BallState.Exploding;
            mExplodeScale = 1.0f;
        }

        /// <summary>
        /// Move the ball in its direction checking for collisions. If it hits a wall, the x-direction is reversed.
        /// If it hits the roof or a stationary ball, it is stopped.
        /// </summary>
        /// <param name="board">The board the ball is moving in.</param>
        /// <param name="dt">The elapsed milliseconds since last frame.</param>
        private void Move(ref Board board, float dt)
        {
            mPosition += mDirection * C_SPEED * dt;

            if (mPosition.X - Size.X * 0.5f < (board.InnerBounds.X) ||
                mPosition.X + Size.X * 0.5f > (board.InnerBounds.X + board.InnerBounds.Width))
                mDirection.X = -mDirection.X;

            if (board.Collision(this, dt) && mState == BallState.Shot)
                mState = BallState.Still;
        }

        /// <summary>
        /// Add gravity to the direction and update the position with direction. If a wall, the roof or the floor
        /// is hit, the ball dies.
        /// </summary>
        /// <param name="board">The board the ball is moving in.</param>
        /// <param name="dt">The elapsed milliseconds since last frame.</param>
        private void AnimateJump(ref Board board, float dt)
        {
            // Update direction with a gravity and update position with direction.
            mDirection += new Vector2(0, 0.05f);
            mPosition += mDirection * C_SPEED * dt;

            // If the ball hits any of the walls, it is dead.
            if (mPosition.X - Size.X * 0.5f < (board.InnerBounds.X) ||
                mPosition.X + Size.X * 0.5f > (board.InnerBounds.X + board.InnerBounds.Width) ||
                mPosition.Y - Size.Y * 0.5f < board.InnerBounds.Y)
                mState = BallState.Dead;

            // If the ball hits the floor, it is dead.
            if (mPosition.Y > board.InnerBounds.Y + board.InnerBounds.Height)
                mState = BallState.Dead;
        }

        /// <summary>
        /// Animate the explosion. If the animation is finished, the ball is dead.
        /// </summary>
        private void AnimateExplosion()
        {
            mExplodeScale -= 0.05f;
            if (mExplodeScale <= 0.0f)
                mState = BallState.Dead;
        }

        #endregion Methods

        #region Properties

        /// <summary>
        /// Get or set the ball's position.
        /// </summary>
        public Vector2 Position
        {
            get { return mPosition; }
            set { mPosition = value; }
        }

        /// <summary>
        /// Read only. Returns the ball's current state.
        /// </summary>
        public BallState State
        {
            get { return mState; }
        }

        /// <summary>
        /// Read only. Returns the ball's current direction.
        /// </summary>
        public Vector2 Direction
        {
            get { return mDirection; }
        }

        /// <summary>
        /// Read only. Returns the ball's colour.
        /// </summary>
        public BallColour Colour
        {
            get { return mColour; }
            private set { mColour = value; }
        }

        #endregion Properties

        #region StaticMethods

        /// <summary>
        /// Initialises the Ball class with the specifed number of colours and ball size.
        /// </summary>
        /// <param name="numColours">The number of colours the ball class should use.</param>
        /// <param name="ballSize">The scale of the balls, where 1.0 is 64x64 px.</param>
        public static void Initialise(int numColours, float ballSize)
        {
            sColours = new List<Color>();
            numColours = (int)MathHelper.Clamp(numColours, 4, 10);

            sTexShapes = new Texture2D[numColours];

            // Add the correct amount of colours to the colours list and shapes to the shape array.
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

            // Load the ball texture, set whether to show shapes and save ball size and scale.
            sTexBall = Core.Content.Load<Texture2D>(@"Textures\ballWhite");
            sSize = new Vector2(sTexBall.Width, sTexBall.Height);
            sScale = ballSize;
        }
        #endregion StaticMethods

        #region StaticProperties

        /// <summary>
        /// Gets or sets the scale of the balls.
        /// </summary>
        public static float Scale
        {
            get { return sScale; }
            set { sScale = value; }
        }

        /// <summary>
        /// Read only. Returns a vector representing the x and y size of the balls.
        /// </summary>
        public static Vector2 Size
        {
            get { return new Vector2(sTexBall.Width * sScale, sTexBall.Height * sScale); }
        }

        /// <summary>
        /// Read only. Returns the number of colours in play.
        /// </summary>
        public static int ColourCount
        {
            get { return sColours.Count; }
        }

        #endregion StaticProperties
    }
}

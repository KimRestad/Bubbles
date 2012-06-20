using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bubbles
{
    class Board
    {
        /// <summary>
        /// A row at the board. Consists of all the balls in that row and a type specifying whethet the row is
        /// at whole indexes or half (it is offset from the row above and under)
        /// </summary>
        private class BoardRow
        {
            public enum Type { Whole, Half }
            public Type RowType;
            public List<Ball> Row;

            public BoardRow(Type type, List<Ball> balls)
            {
                RowType = type;
                Row = balls;
            }
        }

        private List<BoardRow> mBalls;
        private int[] mColumns = new int[2];

        private Vector2 mOffset;


        #region Methods
        /// <summary>
        /// Create the board that contains all the balls and takes care of the collision checking
        /// </summary>
        /// <param name="bounds">The boundaries of the board</param>
        public Board(Rectangle bounds)
        {
            mColumns[0] = (int)Math.Floor(bounds.Width / Ball.Size.X);
            mColumns[1] = (int)Math.Floor((bounds.Width / Ball.Size.X) - 0.5f);

            // Calculate the offset based on which row is the longest, the length of that row and board width.
            // Subtract row length from board width and divide remainder by 2 to get a centered offset.
            float xOffset = bounds.Width;
            xOffset -= (mColumns[0] > mColumns[1] ? mColumns[0] : mColumns[1] + 0.5f) * Ball.Size.X;
            xOffset *= 0.5f;

            mOffset = new Vector2(bounds.X + xOffset, bounds.Y);
            mBalls = new List<BoardRow>();
        }

        /// <summary>
        /// Draw all the balls on the board
        /// </summary>
        /// <param name="spriteBatch">The sprite batch with which to draw the balls</param>
        public void DrawAll(SpriteBatch spriteBatch)
        {
            foreach (BoardRow row in mBalls)
                foreach (Ball ball in row.Row)
                    if (ball != null)
                        ball.Draw(spriteBatch);
        }

        /// <summary>
        /// Check for a collision between the moving ball and all the balls on the board
        /// </summary>
        /// <param name="movingBall">The moving ball to check for a collision against</param>
        /// <returns>True if the movingBall collided with a ball on the board, else false</returns>
        public bool Collision(Ball movingBall)
        {
            // If the ball hits the roof, there is a collision. Handle it and quit method returning true
            if (movingBall.Position.Y - Ball.Size.Y * 0.5 < 0)
            {
                HandleCollision(movingBall);
                return true;
            }

            // Go through all the rows and all the positions in each row to check for a collision
            for (int row = mBalls.Count - 1; row >= 0; --row)
            {
                for(int col = 0; col < mBalls[row].Row.Count; ++col)
                {
                    // If the position is empty, continue the loop
                    if (mBalls[row].Row[col] == null)
                        continue;

                    // Calculate the distance between the ball at the position and the moving ball
                    Vector2 distance = mBalls[row].Row[col].Position - movingBall.Position;

                    // If the distance is shorter than the ball diameter multiplied by 0.7f
                    // (so that the moving balls can pass sligthly through the static ones)
                    // Handle collision and quit method
                    if (distance.Length() < Ball.Size.X * 0.7f)
                    {
                        HandleCollision(movingBall);

                        return true;
                    }
                }
            }

            // If all the board's balls have been checked without breaking, there is no collision - return false
            return false;
        }

        /// <summary>
        /// Add a row of random balls to the top of the board
        /// </summary>
        public void AddRowTop()
        {
            AddRow(GenerateRandomRow(mColumns[(int)NextTypeTop]), true);
        }

        /// <summary>
        /// Add a new row to the bottom of the board.The row will be scaled 
        /// (ie the end is cut off or it is padded with null) to fit the row where it is inserted.
        /// </summary>
        /// <param name="balls">The new list of balls to add</param>
        private void AddRowBottom(List<Ball> balls)
        {
            AddRow(balls, false);
        }

        /// <summary>
        /// Add a row to the board, at the top or at the bottom as specified by atTop
        /// </summary>
        /// <param name="balls">The new list of balls to add<</param>
        /// <param name="atTop">If true, the new row will be added at the top, if false it
        /// will be added at the bottom</param>
        private void AddRow(List<Ball> balls, bool atTop)
        {
            // The max number of balls is set to the maximum number for the top or bottom row based on atTop
            int rowMax = atTop ? mColumns[(int)NextTypeTop] : mColumns[(int)NextTypeBottom];

            // If the number of balls in the list is not equal to the allowed number, fix it
            if (balls.Count != rowMax)
            {
                for (int i = balls.Count; i < rowMax; ++i)
                {
                    balls.Add(null);
                }

                if (balls.Count > rowMax)
                    balls.RemoveRange(rowMax, balls.Count - rowMax);
            }

            // Insert row at the top or bottom based on atTop, if at top - update all the positions
            if (atTop)
            {
                mBalls.Insert(0, new BoardRow(NextTypeTop, balls));
                UpdatePositions();
            }
            else
                mBalls.Insert(mBalls.Count, new BoardRow(NextTypeBottom, balls));
        }

        /// <summary>
        /// Update the positions for all the balls on the board to correspond to their place on the board
        /// </summary>
        private void UpdatePositions()
        {
            for (int row = 0; row < mBalls.Count; ++row)
                for (int col = 0; col < mBalls[row].Row.Count; ++col)
                    if (mBalls[row].Row[col] != null)
                        mBalls[row].Row[col].Position = CalculatePosition(new Point(col, row));

        }

        /// <summary>
        /// Handle collision: find out which cell the moving ball ends up in and add it to that cell
        /// </summary>
        /// <param name="movingBall">The moving ball that collides with the board</param>
        private void HandleCollision(Ball movingBall)
        {
            // Get the cell closest to the current position
            Point cell = CalculateCell(movingBall.Position);
            
            // If the requested cell is in an existing row, make sure the cell is empty, else
            // adjust position backwards along the direction it came in and request new cell
            if(cell.Y < mBalls.Count)
                while(mBalls[cell.Y].Row[cell.X] != null)
                {
                    movingBall.Position += Ball.C_SPEED * -movingBall.Direction;
                    cell = CalculateCell(movingBall.Position);
                    
                    if (cell.Y == mBalls.Count)
                        break;
                }

            // Calculate the ball's new position based on the cell it is in
            movingBall.Position = CalculatePosition(cell);

            // If the ball is in a new row, create the new row
            if (cell.Y == mBalls.Count)
                AddRowBottom(GenerateNullRow(mColumns[(int)NextTypeBottom]));

            // Add ball to the requested cell
            mBalls[cell.Y].Row[cell.X] = movingBall;
        }

        /// <summary>
        /// Generate a new empty list of the requested size filled with null
        /// </summary>
        /// <param name="size">The size of the list</param>
        /// <returns>An empty list of the requested size filled with null</returns>
        private List<Ball> GenerateNullRow(int size)
        {
            List<Ball> returnList = new List<Ball>(size);

            for (int i = 0; i < size; ++i)
                returnList.Add(null);

            return returnList;
        }

        /// <summary>
        /// Generate a new list of the requested size filled with random balls
        /// </summary>
        /// <param name="size">The size of the list</param>
        /// <returns>A list of the requested size filled with random balls</returns>
        private List<Ball> GenerateRandomRow(int size)
        {
            List<Ball> returnList = new List<Ball>();

            for (int i = 0; i < size; ++i)
            {
                returnList.Add(new Ball((BallColour)Core.RandomGen.Next(Ball.NumColours), Vector2.Zero));
            }

            return returnList;
        }

        // DEBUG
        public List<Point> GetNeighbours(Point cell)
        {
            List<Point> returnList = new List<Point>();

            // If the cell x or y indices are invalid (out of bounds) return the empty list
            if (cell.Y >= mBalls.Count || cell.Y < 0 || cell.X < 0 || cell.X >= mColumns[(int)mBalls[cell.Y].RowType])
                return returnList;

            // Save the lower, upper and max index of the rows above and below the current (based on row type)
            int otherRowsLower = (int)mBalls[cell.Y].RowType - 1;
            int otherRowsUpper = (int)mBalls[cell.Y].RowType;
            int maxIndexOthers = mColumns[(int)CalculateRowType(cell.Y + 1)];

            // Only add the indices from the row above if the current row is not the top row
            if (cell.Y > 0)
            {
                // Only add indices if they are within the bounds
                if (cell.X + otherRowsLower > 0)
                    returnList.Add(new Point(cell.X + otherRowsLower, cell.Y - 1));
                if (cell.X + otherRowsUpper < maxIndexOthers)
                    returnList.Add(new Point(cell.X + otherRowsUpper, cell.Y - 1));
            }

            // Only add indices if they are within the bounds
            if (cell.X > 0)
                returnList.Add(new Point(cell.X - 1, cell.Y));
            if (cell.X + 1 < mColumns[(int)mBalls[cell.Y].RowType])
                returnList.Add(new Point(cell.X + 1, cell.Y));

            // Only add the indices from the row below if the current row is not the bottom row
            if (cell.Y + 1 < mBalls.Count)
            {
                // Only add indices if they are within the bounds
                if (cell.X + otherRowsLower > 0)
                    returnList.Add(new Point(cell.X + otherRowsLower, cell.Y + 1));
                if (cell.X + otherRowsUpper < maxIndexOthers)
                    returnList.Add(new Point(cell.X + otherRowsUpper, cell.Y + 1));
            }

            return returnList;
        }

        /// <summary>
        /// Calculate the position on the screen that is in the middle of the cell
        /// </summary>
        /// <param name="cell">The cell to get the center position for</param>
        /// <returns>The center position in cell</returns>
        private Vector2 CalculatePosition(Point cell)
        {
            // Calculate the xPos from what column we're at and then add 0.5 or 1 (based on the row type)
            // ball widths to offset every other row. Add the x-offset
            float xPos = (cell.X * Ball.Size.X) +
                         (Ball.Size.X * 0.5f * (1 + (int)CalculateRowType(cell.Y) % 2)) + mOffset.X;

            // Calculate the yPox from what row we're at times 85% of the ball height add a half height 
            // to account for the origin being in the middle of the ball. Add the y-offset
            float yPos = (cell.Y * 0.85f * Ball.Size.Y) + (Ball.Size.Y * 0.5f) + mOffset.Y;

            return new Vector2(xPos, yPos);
        }

        /// <summary>
        /// Calculate what cell a position belongs to
        /// </summary>
        /// <param name="position">The position to find the cell for</param>
        /// <returns>The cell the position belongs to</returns>
        private Point CalculateCell(Vector2 position)
        {
            // First subtract the offset from the position, then divide this 0-based position by ball height.
            float row = position.Y - mOffset.Y;
            row /= Ball.Size.Y;

            // Divide by 0.85 (the rows overlap slightly) and use integer part
            row = (float)Math.Floor(row / 0.85f);

            // Make sure the row is between 0 and the last row + 1 (a new row may be added)
            row = MathHelper.Clamp(row, 0, mBalls.Count);

            // Find out what type this row is (for x-offset)
            BoardRow.Type rowType = CalculateRowType((int)row);

            // Subtract the board offset from position (want to translate the position to be based on (0, 0)
            float column = position.X - mOffset.X;

            // If the row is further offset (RowType.Half = 1), subtract this offset as well
            column -= ((int)rowType) * Ball.Size.X * 0.5f;

            // Divide by width, then use only integer part
            column /= Ball.Size.X;
            column = (float)Math.Floor(column);

            // Make sure the column is a valid one
            column = MathHelper.Clamp(column, 0, mColumns[(int)rowType]);

            return new Point((int)column, (int)row);
        }

        /// <summary>
        /// Get the row type for an arbitrary row from index 0 to the number of rows plus one (a new row)
        /// </summary>
        /// <param name="row">The row for which the type is requested</param>
        /// <returns>The typ for the requested row</returns>
        private BoardRow.Type CalculateRowType(int row)
        {
            // Initialize the type to Whole (to use if there are no rows)
            BoardRow.Type rowType = BoardRow.Type.Whole;

            // If there are rows and row is an existing index, get the rowtype, else get next bottom type
            if (mBalls.Count > 0 && row < mBalls.Count)
                rowType = mBalls[row].RowType;
            else if (row == mBalls.Count)
                rowType = NextTypeBottom;

            return rowType;
        }

        #endregion Methods

        #region Properties

        /// <summary>
        /// Get the number of rows on the board
        /// </summary>
        private int RowCount
        {
            get { return mBalls.Count; }
        }

        /// <summary>
        /// Get the type of the row below the current last row
        /// </summary>
        private BoardRow.Type NextTypeBottom
        {
            get
            {
                BoardRow.Type rowType = BoardRow.Type.Whole;

                if (mBalls.Count > 0)
                    rowType = (BoardRow.Type)(((int)mBalls[mBalls.Count - 1].RowType + 1) % 2);

                return rowType;
            }
        }

        /// <summary>
        /// Get the type of the row above the current top row
        /// </summary>
        private BoardRow.Type NextTypeTop
        {
            get
            {
                BoardRow.Type rowType = BoardRow.Type.Whole;

                if (mBalls.Count > 0)
                    rowType = (BoardRow.Type)(((int)mBalls[0].RowType + 1) % 2);

                return rowType;
            }
        }

        #endregion Properties
    }
}

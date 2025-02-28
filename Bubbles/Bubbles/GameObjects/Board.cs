﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace Bubbles
{
    class Board
    {
        /// <summary>
        /// A row on the board. Consists of all the balls in that row and a type specifying whether the row is
        /// at whole indexes or half (i.e. it is offset from the row above and under).
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

        #region Variables

        // Constants
        private const int C_WALL_THICKNESS = 40;
        private const int C_BALL_POINTS = 10;
        private const int C_DANGLING_POINTS = 15;
        private const float C_TIME_BONUS_PC = 1.25f; // Time bonus % of time modifier gotten from dangling balls

        // Function variables.
        private List<BoardRow> mBalls;
        private List<Ball> mAnimatingBalls;
        private int[] mColumns = new int[2];
        private Rectangle mInnerBounds;
        private int[] mColoursInPlay = new int[(int)BallColour.Count];
        private int mNumberOfBalls;
        private int mMaxRows;

        // Score variables.
        private int mScore;
        private float mAddRowTime;
        private float mAddRowModifier;

        // Graphics variables.
        private Vector2 mOffset;
        private Texture2D mWallTopTex;
        private Texture2D mWallSideTex;
        private Rectangle mWallLeftRect;
        private Rectangle mWallRightRect;
        private Rectangle mWallTopRect;

        // Sound variables.
        private SoundEffect mSoundPop;
        private SoundEffect mSoundDangling;

        #endregion Variables

        #region Methods

        /// <summary>
        /// Create the board that contains all the rows of balls. 
        /// </summary>
        /// <param name="bounds">The boundaries of the board.</param>
        /// <param name="startScore">Optional. The starting score of the board.</param>
        public Board(Rectangle bounds, int startScore = 0)
        {
            // Load wall textures
            mWallSideTex = Core.Content.Load<Texture2D>(@"Textures\wallSide");
            mWallTopTex = Core.Content.Load<Texture2D>(@"Textures\wallTop");

            // Calculate wall positions and the inner bounds rectangle (excluding walls)
            mWallLeftRect = new Rectangle(bounds.X, bounds.Y + C_WALL_THICKNESS, C_WALL_THICKNESS, bounds.Height - C_WALL_THICKNESS);
            mWallRightRect = new Rectangle(bounds.X + bounds.Width - C_WALL_THICKNESS, bounds.Y + C_WALL_THICKNESS, 
                                           C_WALL_THICKNESS, bounds.Height - C_WALL_THICKNESS);
            mWallTopRect = new Rectangle(bounds.X, bounds.Y, bounds.Width, C_WALL_THICKNESS);

            mInnerBounds = new Rectangle(bounds.X + C_WALL_THICKNESS, bounds.Y + C_WALL_THICKNESS,
                                         bounds.Width - (C_WALL_THICKNESS * 2), bounds.Height - C_WALL_THICKNESS);

            // Calculate max number of rows
            mMaxRows = (int)(mInnerBounds.Height / Ball.Size.Y);

            // Reset score, time and time modifier
            mScore = startScore;
            mAddRowTime = 1.0f;
            mAddRowModifier = 0.05f;

            // Reset ball and colour counters
            for (int i = 0; i < mColoursInPlay.Length; i++)
            {
                mColoursInPlay[i] = 0;
            }
            mNumberOfBalls = 0;

            // Calculate the number of balls in whole and half rows respectively
            mColumns[0] = (int)Math.Floor(mInnerBounds.Width / Ball.Size.X);
            mColumns[1] = (int)Math.Floor((mInnerBounds.Width / Ball.Size.X) - 0.5f);

            // Calculate the offset based on which row is the longest, the length of that row and board width.
            // Subtract row length from board width and divide remainder by 2 to get a centered offset.
            float xOffset = mInnerBounds.Width;
            xOffset -= ((mColumns[0] > mColumns[1]) ? mColumns[0] : (mColumns[1] + 0.5f)) * Ball.Size.X;
            xOffset *= 0.5f;

            mOffset = new Vector2(bounds.X + xOffset + C_WALL_THICKNESS, bounds.Y + C_WALL_THICKNESS);
            mBalls = new List<BoardRow>();
            mAnimatingBalls = new List<Ball>();

            mSoundPop = Core.Content.Load<SoundEffect>(@"Sounds\bubblePop");
            mSoundDangling = Core.Content.Load<SoundEffect>(@"Sounds\dangling");
        }

        /// <summary>
        /// Update the board and remove dead balls.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        public void Update(GameTime gameTime)
        {
            for (int i = 0; i < mAnimatingBalls.Count; ++i)
            {
                mAnimatingBalls[i].Update(this, gameTime);
                if (mAnimatingBalls[i].State == BallState.Dead)
                {
                    mAnimatingBalls.RemoveAt(i);
                    --i;
                }
            }
        }

        /// <summary>
        /// Draw all the balls on the board, the animating balls and the walls.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch used when drawing the ball.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw walls.
            spriteBatch.Draw(mWallTopTex, mWallTopRect, Color.White);
            spriteBatch.Draw(mWallSideTex, mWallLeftRect, Color.White);
            spriteBatch.Draw(mWallSideTex, mWallRightRect, Color.White);

            // Draw balls in rows and animating balls.
            foreach (BoardRow row in mBalls)
                foreach (Ball ball in row.Row)
                    if (ball != null)
                        ball.Draw(spriteBatch);

            foreach (Ball b in mAnimatingBalls)
                b.Draw(spriteBatch);
        }

        /// <summary>
        /// Check for a collision between the moving ball and all the balls on the board.
        /// </summary>
        /// <param name="movingBall">The moving ball to check for a collision against.</param>
        /// <param name="dt">The elapsed milliseconds since last frame.</param>
        /// <returns>True if the movingBall collided with a ball on the board, else false.</returns>
        public bool Collision(Ball movingBall, float dt)
        {
            // If the ball hits the roof, there is a collision. Handle it and quit method returning true.
            if (movingBall.Position.Y - Ball.Size.Y * 0.5 < (mOffset.Y))
            {
                HandleCollision(movingBall, dt);
                return true;
            }

            // Go through all the rows and all the positions in each row to check for a collision.
            for (int row = mBalls.Count - 1; row >= 0; --row)
            {
                // If the ball's top is below the y position of this row, there can be no collision on
                // this or any rows above it.
                if (movingBall.Position.Y - Ball.Size.Y > CalculateYPosition(row))
                    break;

                for(int col = 0; col < mBalls[row].Row.Count; ++col)
                {
                    // If the position is empty, continue the loop.
                    if (mBalls[row].Row[col] == null)
                        continue;

                    // Calculate the distance between the ball at the position and the moving ball.
                    Vector2 distance = mBalls[row].Row[col].Position - movingBall.Position;

                    // If the distance is shorter than the ball diameter multiplied by 0.7f (so that the moving 
                    // balls can pass sligthly through the static ones) - handle collision and quit method.
                    if (distance.Length() < Ball.Size.X * 0.7f)
                    {
                        HandleCollision(movingBall, dt);

                        return true;
                    }
                }
            }

            // If all the board's balls have been checked without breaking, there is no collision - return false
            return false;
        }

        /// <summary>
        /// Generates a random colour from the ones on the board. If there are no balls on the board, a random
        /// color is chosen from the ones allowed on the board (set at initialization of the balls).
        /// </summary>
        /// <returns>A random, valid ball colour</returns>
        public BallColour GenerateColourInPlay()
        {
            BallColour randomCol;

            // If there are balls on the board, make sure the colour generated is in play, and that it is not of 
            // the non shootable colour else generate random colour from the ones set for the game.
            if (mNumberOfBalls > 0)
            {
                do
                {
                    randomCol = (BallColour)Core.RandomGen.Next(Ball.ColourCount);
                } while (mColoursInPlay[(int)randomCol] == 0); // || 
                        //randomCol == Ball.C_NON_SHOOTABLE_COLOUR); 
                        // Infinite loop when only non shootable colour left
            }
            else
                randomCol = (BallColour)Core.RandomGen.Next(Ball.ColourCount);

            return randomCol;
        }

        /// <summary>
        /// Toggle whether to play sounds.
        /// </summary>
        public void ToggleSound()
        {
            Core.PlaySounds = !Core.PlaySounds;
        }

        /// <summary>
        /// Add a row of random balls to the top of the board. The row will be scaled (i.e. the end is cut off or it 
        /// is padded with null) to fit the row size where it is to be inserted.
        /// </summary>
        public void AddRowTop()
        {
            AddRow(GenerateRandomRow(mColumns[(int)NextTypeTop]), true);
        }

        /// <summary>
        /// Add a new row to the bottom of the board. The row will be scaled (i.e. the end is cut off or it is 
        /// padded with null) to fit the row size where it is to be inserted.
        /// </summary>
        /// <param name="balls">The new list of balls to add</param>
        private void AddRowBottom(List<Ball> balls)
        {
            AddRow(balls, false);
        }

        /// <summary>
        /// Add a row to the board, either cutting off the end or padding it with null so that the length corresponds
        /// to the length of the row where it is to be insertetd. The row is inserted at the top or at the bottom 
        /// as specified by atTop.
        /// </summary>
        /// <param name="balls">The new list of balls to add.</param>
        /// <param name="atTop">If true, the new row will be added at the top, if false it
        /// will be added at the bottom.</param>
        private void AddRow(List<Ball> balls, bool atTop)
        {
            // The max number of balls is set to the maximum number for the top or bottom row based on atTop
            int rowMax = atTop ? mColumns[(int)NextTypeTop] : mColumns[(int)NextTypeBottom];

            // If the number of balls in the list is not equal to the row length where it is to be inserted - fix it.
            if (balls.Count != rowMax)
            {
                for (int i = balls.Count; i < rowMax; ++i)
                {
                    balls.Add(null);
                }

                if (balls.Count > rowMax)
                    balls.RemoveRange(rowMax, balls.Count - rowMax);
            }

            // Insert row at the top or bottom based on atTop, if at top - update all the positions.
            BoardRow newRow;
            if (atTop)
            {
                newRow = new BoardRow(NextTypeTop, balls);
                mBalls.Insert(0, newRow);
                UpdatePositions();
            }
            else
            {
                newRow = new BoardRow(NextTypeBottom, balls);
                mBalls.Insert(mBalls.Count, newRow);
            }

            // Update the number of balls array with the newly added balls.
            foreach (Ball ball in newRow.Row)
            {
                if(ball != null)
                    ChangeNumberOfBalls(ball.Colour, true);
            }
        }

        /// <summary>
        /// Update the positions for all the balls on the board to correspond to the cell they are in.
        /// </summary>
        private void UpdatePositions()
        {
            for (int row = 0; row < mBalls.Count; ++row)
                for (int col = 0; col < mBalls[row].Row.Count; ++col)
                    if (mBalls[row].Row[col] != null)
                        mBalls[row].Row[col].Position = CalculatePosition(new Point(col, row));

        }

        /// <summary>
        /// Handle collision: find out which cell the moving ball ends up in and add it to that cell.
        /// </summary>
        /// <param name="movingBall">The moving ball that collides with the board.</param>
        /// <param name="dt">The elapsed milliseconds since last frame.</param>
        private void HandleCollision(Ball movingBall, float dt)
        {
            // Get the cell closest to the current position.
            Point cell = CalculateCell(movingBall.Position);
            
            // If the requested cell is in an existing row, make sure the cell is empty, else adjust position 
            // backwards along its direction and request new cell.
            if(cell.Y < mBalls.Count)
                while(HasBall(cell))
                {
                    movingBall.Position += -movingBall.Direction * Ball.C_SPEED * dt;
                    cell = CalculateCell(movingBall.Position);
                    
                    if (cell.Y == mBalls.Count)
                        break;
                }

            // Calculate the ball's new position based on the cell it is in.
            movingBall.Position = CalculatePosition(cell);

            // If the ball is in a new row, create the new row.
            if (cell.Y == mBalls.Count)
                AddRowBottom(GenerateNullRow(mColumns[(int)NextTypeBottom]));

            // Add ball to the requested cell and update the number of balls.
            mBalls[cell.Y].Row[cell.X] = movingBall;
            ChangeNumberOfBalls(movingBall.Colour, true);

            // Get a sequence of all the balls connected to the shot ball, that have the same colour as the shot ball.
            List<Point> sequence = GetSequence(cell);

            // If sequence is of at last 3 balls, remove all balls in the sequence.
            if (sequence.Count > 2)
            {
                RemoveBalls(ref sequence);
                ClearDanglingBalls(ref sequence);
                ClearEmptyRows();                
            }

            if (mAddRowTime <= 0)
            {
                AddRowTop();
                mAddRowTime = 1.0f;
            }

            mAddRowTime -= mAddRowModifier;
        }

        /// <summary>
        /// Generate a new empty list of the requested size filled with null.
        /// </summary>
        /// <param name="size">The size of the list.</param>
        /// <returns>An empty list of the requested size filled with null.</returns>
        private List<Ball> GenerateNullRow(int size)
        {
            List<Ball> returnList = new List<Ball>(size);

            for (int i = 0; i < size; ++i)
                returnList.Add(null);

            return returnList;
        }

        /// <summary>
        /// Generate a new list of the requested size filled with random balls.
        /// </summary>
        /// <param name="size">The size of the list.</param>
        /// <returns>A list of the requested size filled with random balls.</returns>
        private List<Ball> GenerateRandomRow(int size)
        {
            List<Ball> returnList = new List<Ball>();

            // Generate a new row. The colours are randomized from the ones allowed (set at initialization of the 
            // balls).
            for (int i = 0; i < size; ++i)
            {
                returnList.Add(new Ball((BallColour)Core.RandomGen.Next(Ball.ColourCount), Vector2.Zero));
            }

            return returnList;
        }

        /// <summary>
        /// Get a list of all the cells that are neighbours to the chosen cell (maximum 6 cells) regardless of whether
        /// there is something in the cell or not.
        /// </summary>
        /// <param name="cell">The cell to get the neighbours of.</param>
        /// <returns>A list of points indicating which cells are neighbours to the chosen cell (maximum size is 6).
        /// </returns>
        private List<Point> GetNeighbours(Point cell)
        {
            List<Point> returnList = new List<Point>();

            // If the cell x or y indices are invalid (out of bounds) return the empty list.
            if (cell.Y >= mBalls.Count || cell.Y < 0 || cell.X < 0 || cell.X >= mColumns[(int)mBalls[cell.Y].RowType])
                return returnList;

            // Save the lower, upper and max index of the rows above and below the current (based on row type).
            int otherRowsLower = (int)mBalls[cell.Y].RowType - 1;
            int otherRowsUpper = (int)mBalls[cell.Y].RowType;
            int maxIndexOthers = mColumns[(int)CalculateRowType(cell.Y + 1)];

            // Only add the indices from the row above if the current row is not the top row.
            if (cell.Y > 0)
            {
                // Only add indices if they are within the bounds.
                if (cell.X + otherRowsLower >= 0)
                    returnList.Add(new Point(cell.X + otherRowsLower, cell.Y - 1));
                if (cell.X + otherRowsUpper < maxIndexOthers)
                    returnList.Add(new Point(cell.X + otherRowsUpper, cell.Y - 1));
            }

            // Only add indices on same row if they are within the bounds.
            if (cell.X > 0)
                returnList.Add(new Point(cell.X - 1, cell.Y));
            if (cell.X + 1 < mColumns[(int)mBalls[cell.Y].RowType])
                returnList.Add(new Point(cell.X + 1, cell.Y));

            // Only add the indices from the row below if the current row is not the bottom row.
            if (cell.Y + 1 < mBalls.Count)
            {
                // Only add indices if they are within the bounds.
                if (cell.X + otherRowsLower >= 0)
                    returnList.Add(new Point(cell.X + otherRowsLower, cell.Y + 1));
                if (cell.X + otherRowsUpper < maxIndexOthers)
                    returnList.Add(new Point(cell.X + otherRowsUpper, cell.Y + 1));
            }

            return returnList;
        }

        /// <summary>
        /// Get a sequence of cells that are connected to and cointains balls of the same colour as the start cell.
        /// </summary>
        /// <param name="startCell">The cell where the sequence starts.</param>
        /// <returns>A list of cells connected to and containing balls with the same colour as the start cell.
        /// </returns>
        private List<Point> GetSequence(Point startCell)
        {
            // If there is no ball in the start cell, exit function.
            if (!HasBall(startCell))
                return null;

            // Create a new list and add the start cell to it.
            List<Point> returnList = new List<Point>();
            returnList.Add(startCell);

            // Create lists to contain the neighbours at the current "radius" from the start cell.
            List<Point> neighbours = GetNeighbours(startCell);
            List<Point> nextNeighbours = new List<Point>();

            // Continue the search as long as there are unchecked neighbours.
            while (neighbours.Count > 0)
            {
                // Check each neighbour.
                foreach (Point neighbourPoint in neighbours)
                {
                    // If the neighbouring cell contains a ball and is not already on the returnlist, check if it is 
                    // of the same colour as the ball in the start cell.
                    if (HasBall(neighbourPoint) && !returnList.Contains(neighbourPoint))
                        if (mBalls[neighbourPoint.Y].Row[neighbourPoint.X].Colour == 
                            mBalls[startCell.Y].Row[startCell.X].Colour)
                        {
                            // If the correct colour, add all its neighbours to the list of neighbours to check and 
                            // add cell to the return list.
                            nextNeighbours.AddRange(GetNeighbours(neighbourPoint));
                            returnList.Add(neighbourPoint);
                        }
                }

                // Move all neighbours to be checked to the neighbour list and clear the temporary list.
                neighbours.Clear();
                neighbours.AddRange(nextNeighbours);
                nextNeighbours.Clear();
            }

            return returnList;
        }

        /// <summary>
        /// Returns whether the specified cell contains a ball or not.
        /// </summary>
        /// <param name="cell">The cell to search.</param>
        /// <returns>True if there is a ball in the cell, else false.</returns>
        private bool HasBall(Point cell)
        {
            int maxIndex = mColumns[(int)CalculateRowType(cell.Y)];

            if (cell.X < 0 || cell.X >= maxIndex || cell.Y < 0 || cell.Y >= mBalls.Count)
                return false;

            return mBalls[cell.Y].Row[cell.X] != null;
        }

        /// <summary>
        /// Calculate the position on the screen that is in the middle of the cell.
        /// </summary>
        /// <param name="cell">The cell to get the center position for.</param>
        /// <returns>The center position in cell.</returns>
        private Vector2 CalculatePosition(Point cell)
        {
            // Calculate the xPos from what column we're at and then add 0.5 or 1 (based on the row type)
            // ball widths to offset every other row. Add the x-offset
            float xPos = (cell.X * Ball.Size.X) +
                         (Ball.Size.X * 0.5f * (1 + (int)CalculateRowType(cell.Y) % 2)) + mOffset.X;

            return new Vector2(xPos, CalculateYPosition(cell.Y));
        }

        /// <summary>
        /// Calculate the y position based on the row index.
        /// </summary>
        /// <param name="rowIndex">The index of the row to calculate the y position for.</param>
        /// <returns>The y position of the row with the specified index.</returns>
        private float CalculateYPosition(int rowIndex)
        {
            // Calculate the yPox from what row we're at times 85% of the ball height add a half height 
            // to account for the origin being in the middle of the ball. Add the y-offset
            return (rowIndex * 0.85f * Ball.Size.Y) + (Ball.Size.Y * 0.5f) + mOffset.Y;
        }

        /// <summary>
        /// Calculate what cell a position belongs to.
        /// </summary>
        /// <param name="position">The position to find the cell for.</param>
        /// <returns>The cell the position belongs to.</returns>
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
            column = MathHelper.Clamp(column, 0, mColumns[(int)rowType] - 1);

            return new Point((int)column, (int)row);
        }

        /// <summary>
        /// Get the row type for an arbitrary row from index 0 to the number of rows plus one (a new row).
        /// </summary>
        /// <param name="row">The row for which the type is requested.</param>
        /// <returns>The typ for the requested row.</returns>
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

        /// <summary>
        /// Start at the bottom and scan for balls, if there is a ball, this row and all above contain at least one 
        /// ball, so return. If the whole row is empty, delete it.
        /// </summary>
        private void ClearEmptyRows()
        {
            for (int i = mBalls.Count - 1; i >= 0; --i)
            {
                foreach (Ball b in mBalls[i].Row)
                    if (b != null)
                        return;

                mBalls.RemoveAt(i);
            }
        }

        /// <summary>
        /// Checks the cleared cells for neighbours, then checks all neighbour to see if there is a sequence not
        /// connected to the roof. If there is, those balls are removed.
        /// </summary>
        /// <param name="clearedCells">The cells that were just cleared, that might have caused dangling balls.</param>
        private void ClearDanglingBalls(ref List<Point> clearedCells)
        {
            List<Point> allNeighbours = new List<Point>();

            // Get all the neighbours for all the cells in the cleared cells list.
            foreach (Point cell in clearedCells)
            {
                allNeighbours.AddRange(GetNeighbours(cell));
            }

            // Get the set of neighbours (meaning no duplicates).
            IEnumerable<Point> neighbours = allNeighbours.Distinct<Point>();
            List<Point> checkedCells = new List<Point>();

            // Go through all neighbours to check if they are connecgted to the roof.
            foreach (Point cell in neighbours)
            {
                if (!checkedCells.Contains(cell))
                {
                    List<Point> newCheckedCells = new List<Point>();
                    bool isConnected = IsConnectedToRoof(cell, ref newCheckedCells);

                    // If the ball is not connected add its cell to the checked, unconnected cells list.
                    if(!isConnected)
                        checkedCells.AddRange(newCheckedCells);
                }
            }

            // Remove the balls that are in the checked, unconnected cells list.
            RemoveDanglingBalls(ref checkedCells);
        }

        /// <summary>
        /// Starts at the specified start cell and tries to reach the roof. Returns whether the roof was reached
        /// or not. The checkedCells list contains all the searched cells, i.e. the return value applies to all
        /// cells in this list.
        /// </summary>
        /// <param name="startCell">The cell to start the search from.</param>
        /// <param name="checkedCells">A list containing all already searched cells.</param>
        /// <returns>True if the roof was reached, else false.</returns>
        private bool IsConnectedToRoof(Point startCell, ref List<Point> checkedCells)
        {
            checkedCells.Add(startCell);

            // Escape condition: if on the top row, it is connected to roof.
            if (startCell.Y == 0)
                return true;

            // Save the index of the upper left and upper right balls.
            int leftDiffIndex = (int)mBalls[startCell.Y].RowType - 1;
            int rightDiffIndex = (int)mBalls[startCell.Y].RowType;
            int upDownMaxIndex = mColumns[(int)CalculateRowType(startCell.Y + 1)];

            // Escape condition: if on the second row and either of the cells above contain balls, it is connected to
            // roof.
            if (startCell.Y == 1)
                if (HasBall(new Point(startCell.X + leftDiffIndex, startCell.Y - 1)) ||
                    HasBall(new Point(startCell.X + rightDiffIndex, startCell.Y - 1)))
                    return true;

            // Else check all surrounding balls, top (left, right), (left, right), down (left, right).
            // Check top left.
            Point currPoint = new Point(startCell.X + leftDiffIndex, startCell.Y - 1);

            if (currPoint.X >= 0) // Make sure index is valid.
                if (HasBall(currPoint) && !checkedCells.Contains(currPoint))
                {
                    if(IsConnectedToRoof(currPoint, ref checkedCells))
                        return true;
                }

            // Check top right.
            currPoint = new Point(startCell.X + rightDiffIndex, startCell.Y - 1);

            if (currPoint.X < upDownMaxIndex) // Make sure index is valid.
                if (HasBall(currPoint) && !checkedCells.Contains(currPoint))
                {
                    if (IsConnectedToRoof(currPoint, ref checkedCells))
                        return true;
                }

            // Check left.
            currPoint = new Point(startCell.X - 1, startCell.Y);

            if (currPoint.X >= 0) // Make sure index is valid.
                if (HasBall(currPoint) && !checkedCells.Contains(currPoint))
                {
                    if (IsConnectedToRoof(currPoint, ref checkedCells))
                        return true;
                }

            // Check right.
            currPoint = new Point(startCell.X + 1, startCell.Y);

            if (currPoint.X < mColumns[(int)mBalls[currPoint.Y].RowType])
                if (HasBall(currPoint) && !checkedCells.Contains(currPoint))
                {
                    if (IsConnectedToRoof(currPoint, ref checkedCells))
                        return true;
                }

            // Only check below row if this one is not the bottom row.
            if (startCell.Y + 1 < mBalls.Count)
            {
                // Check bottom left.
                currPoint = new Point(startCell.X + leftDiffIndex, startCell.Y + 1);

                if (currPoint.X >= 0) // Make sure index is valid.
                    if (HasBall(currPoint) && !checkedCells.Contains(currPoint))
                    {
                        if (IsConnectedToRoof(currPoint, ref checkedCells))
                            return true;
                    }

                // Check bottom right.
                currPoint = new Point(startCell.X + rightDiffIndex, startCell.Y + 1);

                if (currPoint.X < upDownMaxIndex) // Make sure index is valid.
                    if (HasBall(currPoint) && !checkedCells.Contains(currPoint))
                    {
                        if (IsConnectedToRoof(currPoint, ref checkedCells))
                            return true;
                    }
            }

            return false;
        }

        /// <summary>
        /// Remove all the balls in the cell list by setting their cells to null.
        /// </summary>
        /// <param name="cells">The list of cells to clear.</param>
        private void RemoveBalls(ref List<Point> cells)
        {
            foreach (Point ballCell in cells)
            {
                // Only if the cell contains a ball.
                if (HasBall(ballCell))
                {
                    // Add points and start animation.
                    Ball currBall = mBalls[ballCell.Y].Row[ballCell.X];

                    mScore += C_BALL_POINTS;
                    ChangeNumberOfBalls(currBall.Colour, false);
                    currBall.StartExploding();
                    mAnimatingBalls.Add(currBall);

                    // If sounds are on, play the sound for dangling balls.
                    if (Core.PlaySounds)
                        mSoundPop.Play();
                }

                // Clear cell.
                mBalls[ballCell.Y].Row[ballCell.X] = null;
            }
        }

        /// <summary>
        /// Remove all the balls in the cell list by setting their cells to null.
        /// </summary>
        /// <param name="cells">The list of "dangling ball" cells to clear.</param>
        private void RemoveDanglingBalls(ref List<Point> cells)
        {
            foreach (Point cell in cells)
            {
                // Only if the cell contains a ball to be removed.
                if (HasBall(cell))
                {
                    // Add score and time.
                    mScore += C_DANGLING_POINTS;
                    mAddRowTime += mAddRowModifier * C_TIME_BONUS_PC;

                    // Make sure time never exceeds 1 + modifier.
                    if (mAddRowTime > 1.0f + mAddRowModifier)
                        mAddRowTime = 1.0f + mAddRowModifier;

                    // Start ball animation, add it to animating balls list and update number of balls.
                    Ball currBall = mBalls[cell.Y].Row[cell.X];
                    int rowTotal = mColumns[(int)mBalls[cell.Y].RowType];                  

                    currBall.StartJumpAnimation((bool)(((float)cell.X / rowTotal) < 0.5f));
                    mAnimatingBalls.Add(currBall);
                    ChangeNumberOfBalls(currBall.Colour, false);

                    // If sounds are on, play the sound for dangling balls.
                    if (Core.PlaySounds)
                        mSoundDangling.Play();
                }

                // Clear cell
                mBalls[cell.Y].Row[cell.X] = null;
            }
        }

        /// <summary>
        /// Increase or decrease, as indicated by the increase parameter, the number of balls of a certain
        /// colour, specified by the parameter colour, that is still in play.
        /// </summary>
        /// <param name="colour">The colour of the added or removed ball.</param>
        /// <param name="wasAdded">Whether a ball of the specified was added or removed. True if it was 
        /// added, else false.</param>
        private void ChangeNumberOfBalls(BallColour colour, bool wasAdded)
        {
            // Based on whether the ball was added or removed, update the colours in play and the total number of 
            // balls in the game variables.
            int modifier = 0;

            if (wasAdded)
                modifier = 1;
            else
                modifier = -1;

            mColoursInPlay[(int)colour] += modifier;
            mNumberOfBalls += modifier;
        }

        #endregion Methods

        #region Properties

        /// <summary>
        /// Read only. Get the inner bounds rectangle (excluding the walls).
        /// </summary>
        public Rectangle InnerBounds
        {
            get { return mInnerBounds; }
        }

        /// <summary>
        /// Read only. The current score at the board.
        /// </summary>
        public int Score
        {
            get { return mScore; }
        }

        /// <summary>
        /// Read only. The time (in percent) until a new row is added.
        /// </summary>
        public float AddRowTime
        {
            get { return mAddRowTime; }
        }

        /// <summary>
        /// Read only. The number of balls left on the board.
        /// </summary>
        public int BallsLeft
        {
            get { return mNumberOfBalls; }
        }

        /// <summary>
        /// Read only. If there are more rows than allowed, true is returned, else false.
        /// </summary>
        public bool HasLost
        {
            get { return mBalls.Count > mMaxRows; }
        }

        /// <summary>
        /// Read only. Get the number of rows on the board.
        /// </summary>
        private int RowCount
        {
            get { return mBalls.Count; }
        }

        /// <summary>
        /// Read only. Get the type of the row below the current last row.
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
        /// Read only. Get the type of the row above the current top row.
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

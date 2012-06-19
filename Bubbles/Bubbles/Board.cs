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
        private class TableRow
        {
            public enum Type { Whole, Half }
            public Type RowType;
            public List<Ball> Row;

            public TableRow(Type type, List<Ball> balls)
            {
                RowType = type;
                Row = balls;
            }
        }

        private List<TableRow> mBalls;
        private int[] mColumns = new int[2];

        private Vector2 mOffset;


        #region Methods
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
            mBalls = new List<TableRow>();
        }

        public void DrawAll(SpriteBatch spriteBatch)
        {
            foreach (TableRow row in mBalls)
                foreach (Ball ball in row.Row)
                    if (ball != null)
                        ball.Draw(spriteBatch);
        }

        public bool Collision(Ball movingBall)
        {
            bool collision = false;

            if (mBalls.Count <= 0 && movingBall.Position.Y - Ball.Size.Y * 0.5 < 0)
            {
                HandleCollision(movingBall);
            }

            for (int row = mBalls.Count - 1; row >= 0; --row)
            {
                for(int col = 0; col < mBalls[row].Row.Count; ++col)
                {
                    Ball currBall = mBalls[row].Row[col];
                    if (currBall == null)
                        continue;

                    Vector2 distance = currBall.Position - movingBall.Position;
                    if (distance.Length() < Ball.Size.X * 0.7f || movingBall.Position.Y - Ball.Size.Y * 0.5f < 0)
                    {
                        HandleCollision(movingBall);

                        collision = true;
                    }
                }
            }


            return collision;
        }

        public void AddRowTop(List<Ball> balls)
        {
            AddRow(balls, true);
        }

        public void AddRowBottom(List<Ball> balls)
        {
            AddRow(balls, false);
        }

        private void AddRow(List<Ball> balls, bool atTop)
        {
            int rowMax = atTop ? mColumns[(int)NextTypeTop] : mColumns[(int)NextTypeBottom];

            if (balls.Count != rowMax)
            {
                for (int i = balls.Count; i < rowMax; ++i)
                {
                    balls.Add(null);
                }

                if (balls.Count > rowMax)
                    balls.RemoveRange(rowMax, balls.Count - rowMax);
            }

            if(atTop)
                mBalls.Insert(0, new TableRow(NextTypeTop, balls));
            else
                mBalls.Insert(mBalls.Count, new TableRow(NextTypeBottom, balls));

            UpdatePositions();
        }

        private void UpdatePositions()
        {
            for (int row = 0; row < mBalls.Count; ++row)
                for (int col = 0; col < mBalls[row].Row.Count; ++col)
                    if (mBalls[row].Row[col] != null)
                        mBalls[row].Row[col].Position = CalculatePosition(new Point(col, row));

        }

        private void HandleCollision(Ball movingBall)
        {
            Point cell = CalculateCell(movingBall.Position);
            movingBall.Position = CalculatePosition(cell);

            if (cell.Y == mBalls.Count)
            {
                List<Ball> newRow = GenerateNullList(mColumns[(int)NextTypeBottom]);
                newRow[cell.X] = movingBall;

                AddRowBottom(newRow);
            }
            else if (cell.Y < mBalls.Count)
                mBalls[cell.Y].Row[cell.X] = movingBall;
        }

        private List<Ball> GenerateNullList(int size)
        {
            List<Ball> returnList = new List<Ball>(size);

            for (int i = 0; i < size; ++i)
                returnList.Add(null);

            return returnList;
        }

        private Vector2 CalculatePosition(Point cell)
        {
            TableRow.Type rowType = TableRow.Type.Whole;
            if (mBalls.Count > 0 && cell.Y < mBalls.Count)
                rowType = mBalls[cell.Y].RowType;
            else if (cell.Y == mBalls.Count)
                rowType = NextTypeBottom;

            // Calculate the xPos from what column we're at and then add 0.5 or 1 (based on the row type)
            // ball widths to offset every other row. Add the x-offset
            float xPos = (cell.X * Ball.Size.X) + (Ball.Size.X * 0.5f * (1 + (int)rowType % 2))
                         + mOffset.X;

            // Calculate the yPox from what row we're at times 85% of the ball height add a half height 
            // to account for the origin being in the middle of the ball. Add the y-offset
            float yPos = (cell.Y * 0.85f * Ball.Size.Y) + (Ball.Size.Y * 0.5f) + mOffset.Y;

            return new Vector2(xPos, yPos);
        }

        public Point CalculateCell(Vector2 position)
        {
            float row = position.Y - mOffset.Y;
            row /= Ball.Size.Y;
            row = (float)Math.Floor((row /*+ 0.5f*/) / 0.85f);

            TableRow.Type rowType = TableRow.Type.Whole;
            if (mBalls.Count > 0 && row < mBalls.Count)
                rowType = mBalls[(int)row].RowType;
            else if (row == mBalls.Count)
                rowType = NextTypeBottom;

            // Subtract the board offset from position (want to translate the position to be based on (0, 0)
            float column = position.X - mOffset.X;

            // If the row is further offset (RowType.Half = 1), subtract this offset as well
            column -= ((int)rowType) * Ball.Size.X * 0.5f;

            // Divide by width, then use only integer part
            column /= Ball.Size.X;
            column = (float)Math.Floor(column);

            return new Point((int)column, (int)row);
        }

        #endregion Methods

        #region Properties
      
        public int RowSizeTop
        {
            get { return mColumns[(int)NextTypeTop]; }
        }

        public int RowCount
        {
            get { return mBalls.Count; }
        }

        private TableRow.Type NextTypeBottom
        {
            get
            {
                TableRow.Type rowType = TableRow.Type.Whole;

                if (mBalls.Count > 0)
                    rowType = (TableRow.Type)(((int)mBalls[mBalls.Count - 1].RowType + 1) % 2);

                return rowType;
            }
        }

        private TableRow.Type NextTypeTop
        {
            get
            {
                TableRow.Type rowType = TableRow.Type.Whole;

                if (mBalls.Count > 0)
                    rowType = (TableRow.Type)(((int)mBalls[0].RowType + 1) % 2);

                return rowType;
            }
        }

        #endregion Properties
    }
}

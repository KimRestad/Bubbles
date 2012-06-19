using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bubbles
{
    class BallTable
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
        private int mColumnsAt;

        private Vector2 mBoardOffset;

        public BallTable(int numColumns1, int numColumns2, Vector2 offset)
        {
            mColumns[0] = numColumns1;
            mColumns[1] = numColumns2;
            mColumnsAt = 0;

            mBoardOffset = offset;
            mBalls = new List<TableRow>();
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
            for (int i = balls.Count; i < mColumns[mColumnsAt]; ++i)
            {
                balls.Add(null);
            }

            if (balls.Count > mColumns[mColumnsAt])
                balls.RemoveRange(mColumns[mColumnsAt], balls.Count - mColumns[mColumnsAt]);

            if(atTop)
            {
                TableRow.Type rowType = TableRow.Type.Whole;
                
                if(mBalls.Count > 0)
                    rowType = (TableRow.Type)(((int)mBalls[0].RowType + 1) % 2);

                mBalls.Insert(0, new TableRow(rowType, balls));
            }
            else
            {
                TableRow.Type rowType = TableRow.Type.Whole;
                
                if(mBalls.Count > 0)
                    rowType = (TableRow.Type)(((int)mBalls[mBalls.Count - 1].RowType + 1) % 2);

                mBalls.Insert(mBalls.Count, new TableRow(rowType, balls));
            }

            mColumnsAt = (mColumnsAt + 1) % 2;

            UpdatePositions();
        }

        private void UpdatePositions()
        {
            for (int row = 0; row < mBalls.Count; ++row)
                for (int col = 0; col < mBalls[row].Row.Count; ++col)
                    if (mBalls[row].Row[col] != null)
                    {
                        // Calculate the xPos from what column we're at and then add 0.5 or 1 (based on the row type)
                        // ball widths to offset every other row. Add the x-offset
                        float xPos = (col * Ball.Size.X) + (Ball.Size.X * 0.5f * (1 + (int)mBalls[row].RowType % 2))
                                     + mBoardOffset.X;
                        
                        // Calculate the yPox from what row we're at times 85% of the ball height add a half height 
                        // to account for the origin being in the middle of the ball. Add the y-offset
                        float yPos = (row * 0.85f * Ball.Size.Y) + (Ball.Size.Y * 0.5f) + mBoardOffset.Y;
                        mBalls[row].Row[col].Position = new Vector2(xPos, yPos);
                    }

        }

        public void DrawAll(SpriteBatch spriteBatch)
        {
            foreach (TableRow row in mBalls)
                foreach (Ball ball in row.Row)
                    if(ball != null)
                        ball.Draw(spriteBatch);
        }

        public int RowSizeTop
        {
            get { return mColumns[mColumnsAt]; }
        }

        public int RowCount
        {
            get { return mBalls.Count; }
        }
    }
}

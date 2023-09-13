using System.Collections.Generic;
using System.Drawing;

namespace MagicLineLib
{
    public class CellBall:BaseNotify
    {

        public int GCost { get; set; }
        public int HCost { get; set; }
        public int FCost => GCost + HCost;


        private int rowNumber;

        public int RowNumber
        {
            get { return rowNumber; }
            set { SetProperty(ref rowNumber , value); }
        }

        private int columnNumber;

        public int ColumnNumber
        {
            get { return columnNumber; }
            set { SetProperty(ref columnNumber , value); }
        }

        public Position Position { get; private set; }

        private CellSize size;

        public CellSize Size
        {
            get { return size; }
            set { SetProperty(ref size , value); }
        }


        private string color;

        public string Color
        {
            get { return color; }
            set {SetProperty(ref color , value); }
        }

        public CellBall(int row, int column, double width)
        {
            RowNumber = row;
            ColumnNumber = column;
            Position = new Position(row*width, column*width);
        }
        public bool UnWalkable => Size == CellSize.Big ? true : false;

        public CellBall Parent { get; set; }

        public List<CellBall> GetNeighbors(CellBall[,] grid, int gridSize)
        {
            List<CellBall> neighbors = new List<CellBall>();
            try
            {
                if (ColumnNumber + 1 < gridSize)
                {
                    var cell = grid[RowNumber, ColumnNumber + 1];
                    if (!cell.UnWalkable)
                    {
                        neighbors.Add(cell);
                    }
                }

                if (ColumnNumber - 1 >= 0)
                {
                    var cell = grid[RowNumber, ColumnNumber - 1];
                    if (!cell.UnWalkable)
                    {
                        neighbors.Add(cell);
                    }
                }

                if (RowNumber - 1 >= 0)
                {
                    var cell = grid[RowNumber - 1, ColumnNumber];
                    if (!cell.UnWalkable)
                    {
                        neighbors.Add(cell);
                    }
                }

                if (RowNumber + 1 < gridSize)
                {
                    var cell = grid[RowNumber + 1, ColumnNumber];
                    if (!cell.UnWalkable)
                    {
                        neighbors.Add(cell);
                    }
                }
                return neighbors;
            }
            catch (System.Exception)
            {

                throw;
            }
        }
    }

}


public enum CellSize
{
    Empty, Small, Big
}
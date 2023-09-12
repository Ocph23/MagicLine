using System.Collections.Generic;
using System.Drawing;

namespace MagicLineLib
{
    public class Cell    :BaseNotify
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


        private CellSize size;

        public CellSize Size
        {
            get { return size; }
            set { SetProperty(ref size , value); }
        }


        private Color color;

        public Color Color
        {
            get { return color; }
            set {SetProperty(ref color , value); }
        }

        public Cell(int row, int column)
        {
            RowNumber = row;
            ColumnNumber = column;
        }
        public bool UnWalkable => Size == CellSize.Big ? true : false;

        public Cell Parent { get; set; }

        public List<Cell> GetNeighbors(Cell[,] grid, int gridSize)
        {
            List<Cell> neighbors = new List<Cell>();
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
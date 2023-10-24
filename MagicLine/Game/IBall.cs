using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicLine.Game
{
    public interface IBall
    {
        public int GCost { get; set; }
        public int HCost { get; set; }
        public int FCost => GCost + HCost;
        public int RowNumber { get; set; }
        public int ColumnNumber { get; set; }
        public double ColumnWidth { get; }
        public Position Position { get; set; }
        public BallSize Size { get; set; }
        public string Color { get; set; }
        public bool IsSelected { get; set; }
        Guid BallId { get; set; }
        public bool UnWalkable { get; }
        public IBall Parent { get; set; }
        List<IBall> GetNeighbors(IBall[,] grid, int gridSize);
        List<(IBall ball, BallDirection direction)> GetNeighborsSameColor(IBall[,] grid, int gridSize);
    }
}

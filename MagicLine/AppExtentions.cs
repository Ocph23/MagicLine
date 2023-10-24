using MagicLine.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicLine
{
    public static class AppExtentions
    {
        public static string ToHexString(this System.Drawing.Color c) => $"#{c.A:X2}{c.R:X2}{c.G:X2}{c.B:X2}";


        public static IBall[,] GetGrid(this Ball[,] grid, int gridSize)
        {
            IBall[,] newGrid = new IBall[gridSize, gridSize];
            var gridClone = grid.Clone() as Ball[,];
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    newGrid[i,j] = gridClone[i, j];
                }
            }
            return newGrid;
        }

    }
}

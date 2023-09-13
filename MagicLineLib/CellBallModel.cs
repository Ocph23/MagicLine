using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace MagicLineLib
{
    public class CellBallModel
    {

        public int R { get; set; }
        public int C { get; set; }

        public CellSize S { get; set; }

        public string Clr { get; set; }

        public CellBallModel(int row, int column, CellSize size, string hexColor)
        {
            R = row;
            C = column;
            Clr = hexColor;
            S = size;
        }
        public CellBallModel() { }
    }
}

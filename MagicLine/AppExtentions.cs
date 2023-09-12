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
    }
}

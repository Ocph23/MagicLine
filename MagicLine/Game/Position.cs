namespace MagicLine.Game
{
    public class Position
    {
        public Position(double row, double col)
        {
            this.Row = row;
            this.Column= col;
        }

        public double Row { get; private set; }
        public double Column { get; private set; }
    }
}
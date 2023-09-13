//using MagicLineLib;

//namespace MagicLine
//{
//    public class GridCell :Border
//    {
//        public double X { get; set; }
//        public double Y { get; set; }
//        public double W { get; set; }
//        public CellSize BallSize { get; set; }
//        public BallView Ball { get; set; }

//        public GridCell(double x, double y, double widthColumn)
//        {
//            X = x;
//            Y = y;
//            W = widthColumn;

//        }

//        public void Show(AbsoluteLayout canvas)
//        {
//            this.Stroke= Colors.Red;
//            canvas.Children.Add(this);
//            this.BackgroundColor= Colors.White;
//            var rect = new Rect(X,Y,W,W);
//            canvas.SetLayoutBounds(this, rect);
//        }

//    }
//}

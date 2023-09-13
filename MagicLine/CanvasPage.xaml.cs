//using MagicLineLib;

//namespace MagicLine;

//public partial class CanvasPage : ContentPage
//{
//    int canvasWidth = 500;
//    int rows = 9;
//    int cols = 9;
//    int widthColumn = 0;

//    Random random = new Random();

//    public GridCell[,] CanvasGrid { get; set; }
//    public CanvasPage()
//    {
//        InitializeComponent();
//        var screen = DeviceDisplay.Current.MainDisplayInfo;
//        main.WidthRequest = screen.Width / screen.Density;
//        main.HeightRequest = screen.Width / screen.Density;

//        #if WINDOWS
//             main.WidthRequest=canvasWidth;
//             main.HeightRequest=canvasWidth;
//        #endif


//        widthColumn = canvasWidth / cols;
//        CanvasGrid = new GridCell[rows, cols];
//        for (int i = 0; i < rows; i++)
//        {
//            for (int j = 0; j < rows; j++)
//            {
//                var cell = new GridCell(i * widthColumn, j * widthColumn, widthColumn);
//                if (random.Next(3) > 1)
//                    cell.BallSize = CellSize.Big;
//                CanvasGrid[i, j] = cell;

//            }
//        }

//        for (int i = 0; i < rows; i++)
//        {
//            for (int j = 0; j < rows; j++)
//            {
//                CanvasGrid[i, j].Show(main);
//                var tap = new TapGestureRecognizer();
//                tap.Tapped += Tap_Tapped; ;
//                CanvasGrid[i, j].GestureRecognizers.Add(tap);
//            }
//        }

//        CreateNewBall();

//    }

//    private void CreateNewBall()
//    {
//        var random = new Random();
//        var i = random.Next(cols);
//        var j = random.Next(cols);

//        var cell = CanvasGrid[i, j];
//        if (cell.Ball == null)
//        {
//            var ball = new BallView() { Color = "BlueAqua", Size = CellSize.Big };
//            var gesture = new TapGestureRecognizer();
//            gesture.Tapped += onClickBall;
//            ball.GestureRecognizers.Add(gesture);
//            cell.Content = ball;
//            var rect = new Rect(X, Y, widthColumn, widthColumn);
//            main.SetLayoutBounds(ball, rect);
//        }
//    }

  
//    private void Button_Clicked(object sender, EventArgs e)
//    {

//    }

//    private void Tap_Tapped(object sender, TappedEventArgs e)
//    {
//        if (SelectedBall != null)
//        {
//            var cellofBall = main.Children.Cast<GridCell>().Where(x => x.Content == SelectedBall).FirstOrDefault();
//            cellofBall.Content = null;
//            main.Children.Add(SelectedBall);

//            var gCell = (GridCell)sender;
//            AbsoluteLayout.SetLayoutBounds(SelectedBall, new Rect(gCell.X, gCell.Y, gCell.Width, gCell.Height));
//            gCell.Content = SelectedBall;
//        }

//    }



//    private void Button_Clicked_1(object sender, EventArgs e)
//    {
        
//    }

//    BallView SelectedBall = null;

//    private void onClickBall(object sender, TappedEventArgs e)
//    {
//        SelectedBall = (BallView)sender;
//    }
//}
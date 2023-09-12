
using MagicLineLib;

namespace MagicLine
{
    public partial class MainPage : ContentPage
    {
        private int score;

        public int Score
        {
            get { return score; }
            set { score = value; OnPropertyChanged(nameof(Score)); }
        }


        Board gameBoard;
        int count = 0;

        public MagicLineLib.Cell StartMove { get; private set; }
        public MagicLineLib.Cell DestinationMove { get; private set; }


        public MainPage()
        {
            InitializeComponent();
            gameBoard = new Board(9,countOfCOlor:3);
            gameBoard.OnChangeScore += (object obj, int score) =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    Score = score;
                });
            };
            gameBoard.NextGeneration();
            _ = DrawBoard(gameBoard);

            BindingContext = this;

        }

        private async Task DrawBoard(Board gameBoard)
        {
            var screen = DeviceDisplay.Current.MainDisplayInfo;
            mainboard.WidthRequest = screen.Width / screen.Density;
            mainboard.HeightRequest = screen.Width / screen.Density;
            await Task.Delay(100);

#if WINDOWS
           mainboard.WidthRequest=450;
           mainboard.HeightRequest=450;
#endif

            for (int i = 0; i < gameBoard.GridSize; i++)
            {
                mainboard.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Star });
                mainboard.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Star });
                mainboard.Margin = new Thickness(1);
            }

            mainboard.Children.Clear();
            for (int i = 0; i < gameBoard.GridSize; i++)
            {
                for (int j = 0; j < gameBoard.GridSize; j++)
                {
                    CellView cell = new CellView(gameBoard.Grid[i, j]) { Margin = new Thickness(1) };
                    Grid.SetRow(cell, i);
                    Grid.SetColumn(cell, j);
                    cell.OnClickCell += Cell_OnClickCell;
                    mainboard.Children.Add(cell);
                }
            }

        }

        private async Task Cell_OnClickCell(MagicLineLib.Cell cell)
        {
            if (cell.Size == CellSize.Big)
            {
                if (StartMove != null)
                {
                    var cellViewx = mainboard.Children.Cast<CellView>().Where(x => x.Cell == StartMove).FirstOrDefault();
                    cellViewx.Selected();
                    if (StartMove == cell)
                    {
                        return;
                    }
                }

                StartMove = cell;
                DestinationMove = null;
                var cellView = mainboard.Children.Cast<CellView>().Where(x => x.Cell == StartMove).FirstOrDefault();
                cellView.Selected();
                return;
            }

            if (StartMove != null)
            {
                DestinationMove = cell;
                var path = this.gameBoard.FindPath(StartMove, DestinationMove);
                if (path != null && path.Count > 0)
                {
                    // await this.MoveBall(path);
                    var cellView = mainboard.Children.Cast<CellView>().Where(x => x.Cell == StartMove).FirstOrDefault();
                    cellView.Selected();
                    this.gameBoard.Move(StartMove, path);
                    StartMove = null;
                    DestinationMove = null;

                }
            }
        }

        private async Task MoveBall(List<MagicLineLib.Cell> path)
        {
            StartMove.Size = CellSize.Empty;

            foreach (var item in path)
            {
                if (item == StartMove)
                {
                    item.Size = CellSize.Empty;
                }

                if (item == DestinationMove)
                {
                    item.Color = StartMove.Color;
                    item.Size = CellSize.Big;
                }


                if (item != StartMove && item != DestinationMove)
                {
                    var color = item.Color;
                    var size = item.Size;
                    item.Color = StartMove.Color;
                    item.Size = CellSize.Big;
                    await Task.Delay(100);
                    item.Color = color;
                    item.Size = size;
                }


            }

        }
    }
}
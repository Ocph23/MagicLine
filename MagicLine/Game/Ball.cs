using Microsoft.Maui.Controls.Shapes;

    public delegate Task AsyncEventHandler(object sender, EventArgs e);
namespace MagicLine.Game
{
    public class Ball : ContentView , IBall
    {
        public event AsyncEventHandler EventClickCell;

        public int GCost { get; set; }
        public int HCost { get; set; }
        public int FCost => GCost + HCost;

        private int rowNumber;

        public int RowNumber
        {
            get { return rowNumber; }
            set { rowNumber = value; OnPropertyChanged(nameof(RowNumber)); }
        }

        private int columnNumber;

        public int ColumnNumber
        {
            get { return columnNumber; }
            set { columnNumber = value; OnPropertyChanged(nameof(ColumnNumber)); }
        }

        public double ColumnWidth { get; }
        public Position Position { get; set; }

        private BallSize size;

        public BallSize Size
        {
            get { return size; }
            set
            {
                size = value;
                OnPropertyChanged(nameof(Size));
            }
        }

        private string color;

        public string Color
        {
            get { return color; }
            set { color = value; OnPropertyChanged(nameof(Color)); }
        }

        private bool isSelected;

        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                isSelected = value;
                OnPropertyChanged(nameof(IsSelected));
                if (value)
                {
                    StartAnimation();
                }
                else
                    StopAnimation();
            }
        }
        public Guid BallId { get; set; } = Guid.NewGuid();

        public Ball(int row, int column, double width, Position position)
        {
            this.RowNumber = row;
            this.ColumnNumber = column;
            this.ColumnWidth = width;
            this.Position = position;
            this.Background = new LinearGradientBrush()
            {
                GradientStops = new GradientStopCollection {
                  new GradientStop{ Color=Microsoft.Maui.Graphics.Color.FromHex("#EFFCFF"), Offset=0.1f },
                  new GradientStop{ Color=Microsoft.Maui.Graphics.Color.FromHex("#C2C7D5"), Offset=1.0f },
                  }
            };


            var bindingColor = new Binding(nameof(Color))
            {
                Converter = new ConventerBallColor(),
            };


            var bindingSize = new Binding(nameof(Size))
            {
                Converter = new ConventerBallSize(),
                ConverterParameter = ColumnWidth
            };

            var bindingSize1 = new Binding(nameof(Size))
            {
                Converter = new ConventerBallSize(),
                ConverterParameter = ColumnWidth
            };

            var ballVisibility = new Binding(nameof(Size))
            {
                Converter = new ConventerBallVisibility()
            };

            this.WidthRequest = ColumnWidth;
            this.HeightRequest = ColumnWidth;

            var ellipse = new Ellipse();
            ellipse.SetBinding(Ellipse.FillProperty, bindingColor);
            ellipse.SetBinding(WidthRequestProperty, bindingSize);
            ellipse.SetBinding(HeightRequestProperty, bindingSize1);
            ellipse.SetBinding(IsVisibleProperty, ballVisibility);
            var cellTap = new TapGestureRecognizer();
            cellTap.Tapped += OnCellClick;
            this.GestureRecognizers.Add(cellTap);
            this.Content = ellipse;
            this.BindingContext = this;

        }

        private void OnCellClick(object sender, TappedEventArgs e)
        {
            EventClickCell?.Invoke(this, EventArgs.Empty);
        }

        private Task StopAnimation()
        {
            this.AbortAnimation(BallId.ToString());
            Microsoft.Maui.Controls.ViewExtensions.CancelAnimations(this);
            return Task.CompletedTask;
        }

        private Task StartAnimation()
        {
            var ball = this.Content as Ellipse;
            new Animation {  { 0, 0.2, new Animation (v => ball.Scale = v, 1, 0.9) },
                { 0.2, 0.4, new Animation (v => ball.Scale = v, 0.9, 1) }}
            .Commit(this, BallId.ToString(), 16, 400, null, null, () => true);
            return Task.CompletedTask;
        }


        public bool UnWalkable => Size == BallSize.Big ? true : false;

        public IBall Parent { get; set; }

        public List<IBall> GetNeighbors(IBall[,] grid, int gridSize)
        {
            List<IBall> neighbors = new List<IBall>();
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

        public List<(IBall ball,BallDirection direction)> GetNeighborsSameColor(IBall[,] grid, int gridSize)
        {
            var minComplete = 5;
            List<(IBall ball, BallDirection direction)> neighbors = new List<(IBall ball, BallDirection type)>();
            try
            {
                if (ColumnNumber + 1 < gridSize && ColumnNumber+minComplete<=gridSize)
                {
                    var cell = grid[RowNumber, ColumnNumber + 1];
                    if (cell.Color== this.color)
                    {
                        neighbors.Add((cell, BallDirection.Right));
                    }
                }


                //if (ColumnNumber - 1 >=0)
                //{
                //    var cell = grid[RowNumber, ColumnNumber - 1];
                //    if (cell.Color == this.color)
                //    {
                //        neighbors.Add((cell, BallDirection.Left));
                //    }
                //}


                //if (RowNumber - 1 >= 0)
                //{
                //    var cell = grid[RowNumber - 1, ColumnNumber];
                //    if (cell.Color == this.color)
                //    {
                //        neighbors.Add((cell, BallDirection.Up));
                //    }
                //}

                if (RowNumber + 1 < gridSize && RowNumber+minComplete <=gridSize)
                {
                    var cell = grid[RowNumber + 1, ColumnNumber];
                    if (cell.Color == this.color)
                    {
                        neighbors.Add((cell,BallDirection.Down));
                    }
                }


                if (ColumnNumber + 1 < gridSize && RowNumber+1 < gridSize
                     && ColumnNumber  + minComplete <=gridSize
                        && RowNumber  + minComplete <=gridSize)
                {
                    var cell = grid[RowNumber + 1, ColumnNumber+1];
                    if (cell.Color == this.color)
                    {
                        neighbors.Add((cell, BallDirection.DownRight));
                    }
                }


                if (ColumnNumber - 1 >=0 && RowNumber + 1 < gridSize
                    && ColumnNumber+1 - minComplete >= 0 
                        && RowNumber + minComplete <= gridSize)
                {
                    var cell = grid[RowNumber + 1, ColumnNumber-1];
                    if (cell.Color == this.color 
                        && ColumnNumber+1-minComplete>=0
                        && RowNumber + minComplete <=gridSize)
                    {
                        neighbors.Add((cell, BallDirection.DownLeft));
                    }
                }



                //if (ColumnNumber + 1 < gridSize && RowNumber - 1 >=0)
                //{
                //    var cell = grid[RowNumber - 1, ColumnNumber + 1];
                //    if (cell.Color == this.color)
                //    {
                //        neighbors.Add((cell, BallDirection.UpRight));
                //    }
                //}


                //if (ColumnNumber - 1 >= 0 && RowNumber - 1 >=0)
                //{
                //    var cell = grid[RowNumber - 1, ColumnNumber - 1];
                //    if (cell.Color == this.color)
                //    {
                //        neighbors.Add((cell, BallDirection.UpLeft));
                //    }
                //}






                return neighbors;
            }
            catch (System.Exception)
            {

                throw;
            }
        }
    }
}

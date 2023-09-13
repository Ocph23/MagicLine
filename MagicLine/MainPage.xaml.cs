
using MagicLineLib;
using Microsoft.Maui.Controls.Shapes;
using Plugin.Maui.Audio;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Text.Json;

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


        private int bestScore;

        public int BestScore
        {
            get { return bestScore; }
            set { bestScore = value; OnPropertyChanged(nameof(BestScore)); }
        }


        public Board GameBoard { get; set; }
        int count = 0;

        public CellBall StartMove { get; private set; }
        public CellBall DestinationMove { get; private set; }

        public ObservableCollection<string> NextColors { get; set; } = new ObservableCollection<string>();

        IAudioPlayer audioManager;
        private bool giveused;

        [Obsolete]
        public MainPage()
        {
            InitializeComponent();

            // Preferences.Set("lastBoard", null);
            var data = Preferences.Get("lastBoard", null);
            BestScore = Preferences.Get("bestScore", 0);
            var screen = DeviceDisplay.Current.MainDisplayInfo;
            mainboard.WidthRequest = screen.Width / screen.Density;
            mainboard.HeightRequest = screen.Width / screen.Density;
#if WINDOWS
                                   mainboard.WidthRequest=450;
                                   mainboard.HeightRequest=450;
#endif





            if (string.IsNullOrEmpty(data))
            {
                GameBoard = new Board(9, mainboard.WidthRequest);
            }
            else
            {
                var dataModel = JsonSerializer.Deserialize<List<string>>(data, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                GameBoard = new Board(9, mainboard.WidthRequest, dataModel);
            }

            GameBoard.Hammer= Preferences.Get("hammer", 0);

            for (int i = 0; i < GameBoard.GridSize; i++)
            {
                mainboard.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Star });
                mainboard.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Star });
                mainboard.Margin = new Thickness(1);
            }
            GameBoard.OnBallStep += OnBallStep;

            GameBoard.OnGameOver += (object obj, bool gameOver) =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    Preferences.Set("hammer", GameBoard.Hammer);

                    if (BestScore < GameBoard.Score)
                    {
                        BestScore = GameBoard.Score;
                        Preferences.Set("bestScore", BestScore);
                    }
                    Shell.Current.DisplayAlert("Game Over", "Aplikasi Akan Diulang lagi !", "OK");
                    GameBoard = new Board(9, mainboard.WidthRequest);
                    GameBoard.NextGeneration();
                    _ = DrawBoard(GameBoard);
                });
            };

            GameBoard.OnGenerateNewColors += (object obj, string[] colors) =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    NextColors.Clear();
                    nextcolors.Children.Clear();
                    foreach (var item in colors)
                    {
                        nextcolors.Children.Add(new Ellipse()
                        {
                            WidthRequest = 15,
                            HeightRequest = 15,
                            Fill = Helper.ColorToGradientBall(item)
                        }); ;
                        NextColors.Add(item);
                    }
                });
            };

            GameBoard.NextGeneration();
            _ = DrawBoard(GameBoard);

            BindingContext = this;

        }

        private async Task DrawBoard(Board gameBoard)
        {

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


        private void OnBallStep(object obj, bool hasComplete)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                if (!hasComplete)
                {
                    var audioPlayer = AudioManager.Current.CreatePlayer(await FileSystem.OpenAppPackageFileAsync("stepf.wav"));
                    audioPlayer.Play();
                }
                else
                {
                    var audioPlayer = AudioManager.Current.CreatePlayer(await FileSystem.OpenAppPackageFileAsync("bomb.wav"));
                    audioPlayer.Play();
                }
            });
        }

        private async Task Cell_OnClickCell(CellBall cell)
        {
            if (giveused)
            {
                var oldData = AbsoluteLayout.GetLayoutBounds(hammer);
                AbsoluteLayout.SetLayoutBounds(hammer, new Rect(cell.Position.Column, cell.Position.Row, GameBoard.WidthColumn, GameBoard.WidthColumn));
                await Task.Delay(200);
                await hammer.RotateTo(20, 250);
                GameBoard.Grid[cell.RowNumber, cell.ColumnNumber].Size = CellSize.Empty;
                var audioPlayer = AudioManager.Current.CreatePlayer(await FileSystem.OpenAppPackageFileAsync("bomb.wav"));
                audioPlayer.Play();
                GameBoard.Hammer--;
                await hammer.RotateTo(0, 250);
                this.AbortAnimation($"Hammer");
                await Task.Delay(200);
                AbsoluteLayout.SetLayoutBounds(hammer, oldData);
                giveused = false;
                return;
            }

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
                var path = this.GameBoard.FindPath(StartMove, DestinationMove);
                if (path != null && path.Count > 0)
                {
                    // await this.MoveBall(path);
                    var cellView = mainboard.Children.Cast<CellView>().Where(x => x.Cell == StartMove).FirstOrDefault();
                    cellView.Selected();
                    this.GameBoard.Move(StartMove, path);
                    StartMove = null;
                    DestinationMove = null;

                }
            }
        }


        private async Task MoveBall(List<CellBall> path)
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

        private void Button_Clicked(object sender, EventArgs e)
        {
            var data = JsonSerializer.Serialize(this.GameBoard.Grid.Cast<CellBall>().Where(x => x.Size != CellSize.Empty)
                .Select(x => $"{x.RowNumber};{x.ColumnNumber};{x.Size};{x.Color}"),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Preferences.Set("lastBoard", data);
            Preferences.Set("lastScore", GameBoard.Score);
            Preferences.Set("hammer", GameBoard.Hammer);

        }
        Animation parentAnimation = new Animation();



        private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
        {
            if (GameBoard.Hammer < 1)
                return;

            giveused = !giveused;
            if (this.giveused)
            {
                new Animation {{ 0, 0.2, new Animation (v => hammer.Scale = v, 1, 0.8) },
                { 0.2, 0.4, new Animation (v => hammer.Scale = v, 0.8, 1) }}
                .Commit(this, "Hammer", 16, 500, null, null, () => true);
            }
            else
            {
                this.AbortAnimation("Hammer");
            }
        }
    }
}
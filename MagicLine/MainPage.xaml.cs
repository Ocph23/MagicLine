
using MagicLine.Game;
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

        public IBall StartMove { get; set; }
        public IBall DestinationMove { get; set; }

        public ObservableCollection<string> NextColors { get; set; } = new ObservableCollection<string>();

        private bool giveused;

        [Obsolete]
        public MainPage()
        {
            InitializeComponent();

            Preferences.Set("lastBoard", null);
            var data = Preferences.Get("lastBoard", null);
            BestScore = Preferences.Get("bestScore", 0);
            var screen = DeviceDisplay.Current.MainDisplayInfo;
            var width = (screen.Width / screen.Density) - 10;
            mainboard.WidthRequest = width;
            mainboard.HeightRequest = width;
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
                var dataModel = JsonSerializer.Deserialize<List<string>>(data, new JsonSerializerOptions
                { PropertyNameCaseInsensitive = true });
                GameBoard = new Board(9, mainboard.WidthRequest, dataModel);
            }

            GameBoard.Hammer = Preferences.Get("hammer", 0);

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


            _ = DrawBoard(GameBoard);

            BindingContext = this;

        }

        private async Task DrawBoard(Board gameBoard)
        {
            audioPlayer = AudioManager.Current.CreatePlayer(await FileSystem.OpenAppPackageFileAsync("stepf.wav"));
            audioBoom = AudioManager.Current.CreatePlayer(await FileSystem.OpenAppPackageFileAsync("bomb.wav"));
            // audioPlayer.Play();
            await GameBoard.NextGeneration();
            await GameBoard.NextGeneration();
            mainboard.Children.Clear();
            for (int i = 0; i < gameBoard.GridSize; i++)
            {
                for (int j = 0; j < gameBoard.GridSize; j++)
                {
                    Ball cell = GameBoard.GridBoard[i, j];
                    Grid.SetRow(cell, i);
                    Grid.SetColumn(cell, j);
                    cell.EventClickCell += Cell_OnSelectedBall;
                    mainboard.Children.Add(cell);
                }
            }

        }

        IAudioPlayer audioPlayer;
        IAudioPlayer audioBoom;

        //audioPlayer.Play();
        [Obsolete]
        private void OnBallStep(object obj, bool hasComplete)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                if (!hasComplete)
                {
                    if(audioPlayer.IsPlaying)
                    {
                        audioPlayer.Stop();
                    }
                    audioPlayer.Play();
                }
                else
                {
                    if(audioBoom.IsPlaying)
                    {
                        audioBoom.Stop();
                    }
                    audioBoom.Play();
                }
            });
        }

        private async Task Cell_OnSelectedBall(object sender, EventArgs e)
        {
            try
            {
                if (IsBusy) return;
                var cell = sender as Ball;
                if (giveused)
                {
                    var oldData = AbsoluteLayout.GetLayoutBounds(hammer);
                    AbsoluteLayout.SetLayoutBounds(hammer, new Rect(cell.Position.Column, cell.Position.Row, GameBoard.WidthColumn, GameBoard.WidthColumn));
                    await Task.Delay(200);
                    await hammer.RotateTo(20, 250);
                    GameBoard.GridBoard[cell.RowNumber, cell.ColumnNumber].Size = BallSize.Empty;
                    audioBoom.Play();
                    GameBoard.Hammer--;
                    await hammer.RotateTo(0, 250);
                    this.AbortAnimation($"Hammer");
                    await Task.Delay(200);
                    AbsoluteLayout.SetLayoutBounds(hammer, oldData);
                    giveused = false;
                    return;
                }

                if (cell.Size == BallSize.Big)
                {
                    if (StartMove == null)
                    {
                        StartMove = cell;
                        StartMove.IsSelected = true;
                    }
                    else
                    {
                        if (StartMove == cell)
                        {
                            StartMove.IsSelected = false;
                            StartMove = null;
                            return;
                        }
                        else
                        {
                            StartMove.IsSelected = false;
                            await Task.Delay(100);
                            StartMove = cell;
                            StartMove.IsSelected = true;
                            DestinationMove = null;
                            return;
                        }
                    }
                }

                if (StartMove != null && cell.Size != BallSize.Big)
                {
                    DestinationMove = cell;
                    var path = this.GameBoard.FindPath(StartMove, DestinationMove);
                    if (path != null && path.Count > 0)
                    {
                        // await this.MoveBall(path);
                        var cellView = mainboard.Children.Cast<Ball>().Where(x => x == StartMove).FirstOrDefault();
                        await this.GameBoard.Move(StartMove, path);
                        StartMove = null;
                    }
                    DestinationMove = null;
                }
            }
            catch (Exception ex)
            {
                IsBusy = false;
                await Shell.Current.DisplayAlert("Error", ex.Message, "Ok");
            }
            finally
            {
                IsBusy = false;
            }
        }



        private async Task MoveBall(List<Ball> path)
        {
            StartMove.Size = BallSize.Empty;

            foreach (var item in path)
            {
                if (item == StartMove)
                {
                    item.Size = BallSize.Empty;
                }

                if (item == DestinationMove)
                {
                    item.Color = StartMove.Color;
                    item.Size = BallSize.Big;
                }


                if (item != StartMove && item != DestinationMove)
                {
                    var color = item.Color;
                    var size = item.Size;
                    item.Color = StartMove.Color;
                    item.Size = BallSize.Big;
                    await Task.Delay(100);
                    item.Color = color;
                    item.Size = size;
                }


            }

        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            var data = JsonSerializer.Serialize(GameBoard.GridBoard.GetGrid(GameBoard.GridSize).Cast<Ball>().Where(x => x.Size != BallSize.Empty)
                .Select(x => $"{x.RowNumber};{x.ColumnNumber};{x.Size};{x.Color}"),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Preferences.Set("lastBoard", data);
            Preferences.Set("lastScore", GameBoard.Score);
            Preferences.Set("hammer", GameBoard.Hammer);
            Shell.Current.DisplayAlert("Berhasil", "Data Tersimpan !", "Ok");
        }

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

        private void MyBall_OnSelectedBall(object sender, EventArgs e)
        {

        }
    }
}


using System.Drawing;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;


namespace MagicLine.Game
{
    public class Board : BaseNotify
    {
        //Color[] colors = new Color[] { Color.Red, Color.Blue, Color.Yellow, Color.Pink, Color.White, Color.Green };
        string[] colors = ColorHelper.GetColorNames();

        //Color[] colors;
        Random random = new Random();

        int CountOfNewGeneration = 3;

        public double WidthColumn { get; private set; }

        int CountOfComplete = 5;

        public int GridSize { get; set; }

        public Ball[,] GridBoard { get; set; }

        private int score;

        public int Score
        {
            get { return score; }
            set { SetProperty(ref score, value); }
        }


        private int hammer;

        public int Hammer
        {
            get { return hammer; }
            set { SetProperty(ref hammer, value); }
        }


        public EventHandler<bool> OnGameOver;
        public EventHandler<string[]> OnGenerateNewColors;
        public EventHandler<bool> OnBallStep;

        public Board(int gridSize, double widthScreen, int countOfCOlor = 9, int countOfComplete = 5, int countOfNewGeneration = 3)
        {
            this.WidthColumn = widthScreen / gridSize;
            this.CountOfComplete = countOfComplete;
            this.CountOfNewGeneration = countOfNewGeneration;
            GridSize = gridSize;
            GridBoard = new Ball[GridSize, GridSize];
            CreateNextColor();
            InstantNewBall();
        }





        public Board(int gridSize, double widthScreen, List<string> source, int countOfCOlor = 9, int countOfComplete = 5, int countOfNewGeneration = 3)
        {
            this.WidthColumn = widthScreen / gridSize;
            this.CountOfComplete = countOfComplete;
            this.CountOfNewGeneration = countOfNewGeneration;
            // colors = colorHex.Select(x => ColorTranslator.FromHtml(x)).Take(countOfCOlor).ToArray();
            GridSize = gridSize;
            GridBoard = new Ball[GridSize, GridSize];
            InstantNewBall().Wait();
            foreach (var item in source)
            {
                var datas = item.Split(";");
                var row = Convert.ToInt32(datas[0]);
                var col = Convert.ToInt32(datas[1]);
                BallSize size = (BallSize)Enum.Parse(typeof(BallSize), datas[2].ToString());
                GridBoard[row, col].Color = datas[3].ToString();
                GridBoard[row, col].Size = size;
            }
        }

        public Task InstantNewBall()
        {
            for (int i = 0; i < GridSize; i++)
            {
                for (int j = 0; j < GridSize; j++)
                {
                    GridBoard[i, j] = new Ball(i, j, WidthColumn, new Position(i * WidthColumn, j * WidthColumn));
                }
            }
            return Task.CompletedTask;
        }

        public Task NextGeneration()
        {

            for (int i = 0; i < GridSize; i++)
            {
                for (int j = 0; j < GridSize; j++)
                {
                    var cell = GridBoard[i, j];
                    if (cell.Size == BallSize.Small)
                    {
                        cell.Size = BallSize.Big;
                    }
                }
            }
            CreateNewBall();
            OnBallsComplete();
            return Task.CompletedTask;
        }

        public Task CreateNewBall()
        {
            foreach (var color in NextColors)
            {
                var emptys = GridBoard.GetGrid(GridSize).Cast<Ball>().Where(x => x.Size == BallSize.Empty);
                if (emptys.Any())
                {
                    var c = random.Next(emptys.Count());
                    var cell = emptys.ToArray()[c];
                    cell.Size = BallSize.Small;
                    cell.Color = color;
                }

            }
            CreateNextColor();
            return Task.CompletedTask;
        }

        public List<string> NextColors { get; set; } = new List<string>();

        private Task CreateNextColor()
        {
            NextColors.Clear();
            for (int i = 0; i < CountOfNewGeneration; i++)
            {
                var r = random.Next(colors.Length);
                NextColors.Add(colors[r]);
            }

            OnGenerateNewColors?.Invoke(this, NextColors.ToArray());
            return Task.CompletedTask;
        }



        public bool OnBallsComplete()
        {
            try
            {
                List<IBall> allList = new List<IBall>();
                var newGrid = GridBoard.GetGrid(GridSize);
                bool hasCompleteBall = false;
                //new
                var hasball = newGrid.Cast<Ball>().Where(x => x.Size == BallSize.Big);
                List<IBall> periksa = new List<IBall>();
                foreach (var item in hasball)
                {
                    if (!periksa.Contains(item))
                    {
                        periksa.Add(item);
                        var neigboards = item.GetNeighborsSameColor(newGrid, GridSize);
                        foreach (var neighboar in neigboards)
                        {

                            var bals = new List<IBall>();
                            //Right
                            if (neighboar.direction == BallDirection.Right )
                            {
                                bals.Add(item);
                                bals.Add(neighboar.ball);
                                if (!periksa.Contains(neighboar.ball) && neigboards.Count==1 )
                                {
                                    periksa.Add(neighboar.ball);
                                }
                                for (int col = item.ColumnNumber + 2; col < GridSize; col++)
                                {
                                    var newItem = newGrid[item.RowNumber, col];
                                    if (newItem.Size == BallSize.Big && newItem.Color == item.Color)
                                    {
                                        bals.Add(newItem);
                                        if (!periksa.Contains(newItem))
                                        {
                                            periksa.Add(newItem);
                                        }
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }

                                if (bals.Count >= CountOfComplete)
                                {
                                    allList.AddRange(bals);
                                }

                                bals.Clear();
                            }



                            //Bottom

                            if (neighboar.direction == BallDirection.Down )
                            {
                                bals.Add(item);
                                bals.Add(neighboar.ball);
                                if (!periksa.Contains(neighboar.ball))
                                {
                                    periksa.Add(neighboar.ball);
                                }
                                for (int row = item.RowNumber + 2; row < GridSize; row++)
                                {
                                    var newItem = newGrid[row, item.ColumnNumber];
                                    if (newItem.Size == BallSize.Big && newItem.Color == item.Color)
                                    {
                                        bals.Add(newItem);
                                        if (!periksa.Contains(newItem))
                                        {
                                            periksa.Add(newItem);
                                        }
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }

                                if (bals.Count >= CountOfComplete)
                                {
                                    allList.AddRange(bals);
                                }

                                bals.Clear();
                            }

                            //horizontal to right

                            if (neighboar.direction == BallDirection.DownRight )
                            {
                                bals.Add(item);
                                bals.Add(neighboar.ball);
                                if (!periksa.Contains(neighboar.ball))
                                {
                                    periksa.Add(neighboar.ball);
                                }
                                int cc = item.ColumnNumber + 2;
                                for (int row = item.RowNumber + 2; row < GridSize; row++)
                                {
                                    if (cc >= GridSize) break;
                                    var newItem = newGrid[row, cc];
                                    if (newItem.Size == BallSize.Big && newItem.Color == item.Color)
                                    {
                                        bals.Add(newItem);
                                        if (!periksa.Contains(newItem))
                                        {
                                            periksa.Add(newItem);
                                        }
                                    }
                                    else
                                    {
                                        break;
                                    }
                                    cc++;
                                }

                                if (bals.Count >= CountOfComplete)
                                {
                                    allList.AddRange(bals);
                                }

                                bals.Clear();
                            }


                            //horizontal to left

                            if (neighboar.direction == BallDirection.DownLeft)
                            {
                                bals.Add(item);
                                bals.Add(neighboar.ball);
                                if (!periksa.Contains(neighboar.ball))
                                {
                                    periksa.Add(neighboar.ball);
                                }
                                int cc = item.ColumnNumber - 2;
                                for (int row = item.RowNumber + 2; row < GridSize; row++)
                                {
                                    if(cc < 0) break;

                                    var newItem = newGrid[row, cc];
                                    if (newItem.Size == BallSize.Big && newItem.Color == item.Color)
                                    {
                                        bals.Add(newItem);
                                        if (!periksa.Contains(newItem))
                                        {
                                            periksa.Add(newItem);
                                        }
                                    }
                                    else
                                    {
                                        break;
                                    }
                                    cc--;
                                }

                                if (bals.Count >= CountOfComplete)
                                {
                                    allList.AddRange(bals);
                                }

                                bals.Clear();
                            }
                        }
                    }
                }


                var group = allList.GroupBy(x => x.Color);
                foreach (var item in group)
                {
                    var score = 0;
                    if (item.Count() <= 5)
                        score = 5;
                    else
                    {
                        var x = item.Count() - 5;
                        score = (x + 1) * item.Count();
                    }

                    SetScore(score);
                }

                foreach (var item in allList)
                {
                    hasCompleteBall = true;
                    var oldBald = GridBoard[item.RowNumber, item.ColumnNumber];
                    oldBald.Color = string.Empty;
                    oldBald.Size = BallSize.Empty;
                    oldBald.IsSelected = false;
                }
                allList.Clear();

                return hasCompleteBall;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private async Task SetScore(int score)
        {
            if (score >= 45)
                Hammer++;
            for (int i = 0; i < score; i++)
            {
                Score++;
                await Task.Delay(100);
            }


        }

        //private List<Ball> DiagonalToleft(int i, int j)
        //{
        //    List<Ball> list = new List<Ball>();

        //    var newGrid = Grid.Clone() as Ball[,];
        //    for (int col = j; col >= 0; col--)
        //    {
        //        var row = i;
        //        var first = newGrid[row, col];
        //        if (first.Size == BallSize.Big && row + CountOfComplete <= GridSize && col - CountOfComplete >= 0)
        //        {
        //            list.Add(first);
        //            for (int z = row + 1; z < GridSize; z++)
        //            {
        //                col--;
        //                var last = newGrid[z, col];
        //                if (last.Size == BallSize.Big && last.Color == first.Color)
        //                {
        //                    list.Add(last);
        //                }
        //                else
        //                    break;
        //            }


        //            if (list.Count >= CountOfComplete)
        //            {
        //                break;
        //            }
        //            else
        //            {
        //                row = list.Last().RowNumber;
        //                col = list.Last().ColumnNumber;
        //                list.Clear();
        //            }

        //        }
        //    }
        //    return list;
        //}

        public async Task Move(IBall start, List<IBall> path)
        {

            var s = GridBoard[start.RowNumber, start.ColumnNumber];
            s.Size = BallSize.Empty;
          

            var destination = path.Last();
            (string color, BallSize size) last = (destination.Color, destination.Size);


            OnBallStep?.Invoke(this, false);

            foreach (var item in path.Select((value, index) => new { value, index }))
            {
                var color = item.value.Color;
                var size = item.value.Size;
                item.value.Color = start.Color;
                item.value.Size = BallSize.Big;
                await Task.Delay(100);

                if (item.index < path.Count - 1)
                {
                    OnBallStep?.Invoke(this, false);
                    item.value.Color = color;
                    item.value.Size = size;
                }

            }

            if (last.size == BallSize.Small)
            {
                var emptys = GridBoard.GetGrid(GridSize).Cast<Ball>().Where(x => x.Size == BallSize.Empty);
                if (emptys.Any())
                {
                    var c = random.Next(emptys.Count());
                    var cell = emptys.ToArray()[c];
                    cell.Size = BallSize.Big;
                    cell.Color = last.color;
                }
            }


            var hasComleteBall = OnBallsComplete();
            if (!hasComleteBall)
            {
                var emptys = GridBoard.GetGrid(GridSize).Cast<Ball>().Where(x => x.Size == BallSize.Empty);
                if (!emptys.Any())
                {
                    OnGameOver?.Invoke(this, true);
                }
                else
                {
                    await  NextGeneration();
                }
            }
            else
            {
                OnBallStep?.Invoke(this, true);
            }
            s.Color = string.Empty;
        }

        public List<IBall> FindPath(IBall start, IBall dest)
        {
            var grid = GridBoard.GetGrid(GridSize);
            List<IBall> Open = new List<IBall>();
            List<IBall> Closed = new List<IBall>();

            // foreach (var item in grid.Cast<Ball>().ToArray())
            // {
            //     item.SetNeighbors(grid, GridSize);
            // }
            Open.Add(start);
            while (Open.Count > 0)
            {
                var currentBall = Open[0];
                for (int i = 1; i < Open.Count; i++)
                {
                    if (Open[i].FCost < currentBall.FCost || Open[i].FCost == currentBall.FCost && Open[i].HCost < currentBall.HCost)
                    {
                        currentBall = Open[i];
                    }
                }

                Open.Remove(currentBall);
                Closed.Add(currentBall);

                if (currentBall == dest)
                {
                    var path = RetracePath(start, dest);
                    return path;
                }


                foreach (var neighbor in currentBall.GetNeighbors(grid, GridSize))
                {
                    if (neighbor.UnWalkable || Closed.Contains(neighbor))
                    {
                        continue;
                    }

                    int newMovementCostToNeighbor = currentBall.GCost + GetDistance(currentBall, neighbor);
                    if (newMovementCostToNeighbor < neighbor.GCost || !Open.Contains(neighbor))
                    {
                        neighbor.GCost = newMovementCostToNeighbor;
                        neighbor.HCost = GetDistance(neighbor, dest);
                        neighbor.Parent = currentBall;
                        if (!Open.Contains(neighbor))
                            Open.Add(neighbor);
                    }
                }
            }
            return null;
        }

        List<IBall> RetracePath(IBall start, IBall dest)
        {
            List<IBall> pathx = new List<IBall>();
            IBall currentNode = dest;
            while (currentNode != start)
            {
                pathx.Add(currentNode);
                currentNode = currentNode.Parent;
            }
            pathx.Reverse();
            return pathx;
        }

        int GetDistance(IBall a, IBall b)
        {
            int distx = Math.Abs(a.RowNumber - b.RowNumber);
            int disty = Math.Abs(a.ColumnNumber - b.ColumnNumber);
            if (distx > disty)
                return 14 * distx + 10 * (distx - disty);
            return 14 * distx + 10 * (disty - distx);
        }


    }
}
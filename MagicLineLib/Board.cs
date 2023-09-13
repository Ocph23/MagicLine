

using System.Drawing;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;


namespace MagicLineLib
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

        public CellBall[,] Grid { get; set; }

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
            // colors = colorHex.Select(x => ColorTranslator.FromHtml(x)).Take(countOfCOlor).ToArray();
            GridSize = gridSize;
            Grid = new CellBall[GridSize, GridSize];

            for (int i = 0; i < GridSize; i++)
            {
                for (int j = 0; j < GridSize; j++)
                {
                    Grid[i, j] = new CellBall(i, j, WidthColumn);
                }
            }
            NextGeneration();
            NextGeneration();
        }


        public Board(int gridSize, double widthScreen, List<string> source, int countOfCOlor = 9, int countOfComplete = 5, int countOfNewGeneration = 3)
        {
            this.WidthColumn = widthScreen / gridSize;
            this.CountOfComplete = countOfComplete;
            this.CountOfNewGeneration = countOfNewGeneration;
            // colors = colorHex.Select(x => ColorTranslator.FromHtml(x)).Take(countOfCOlor).ToArray();
            GridSize = gridSize;
            Grid = new CellBall[GridSize, GridSize];

            for (int i = 0; i < GridSize; i++)
            {
                for (int j = 0; j < GridSize; j++)
                {
                    Grid[i, j] = new CellBall(i, j, WidthColumn);
                }
            }


            foreach (var item in source)
            {
                var datas = item.Split(";");
                var row = Convert.ToInt32(datas[0]);
                var col = Convert.ToInt32(datas[1]);
                CellSize size = (CellSize)Enum.Parse(typeof(CellSize), datas[2].ToString());
                Grid[row, col].Color = datas[3].ToString();
                Grid[row, col].Size = size;
            }
        }

        public void CreateNewCellBall()
        {

            foreach (var color in NextColors)
            {
                var emptys = Grid.Cast<CellBall>().Where(x => x.Size == CellSize.Empty);
                if (emptys.Any())
                {
                    var c = random.Next(emptys.Count());
                    var cell = emptys.ToArray()[c];
                    cell.Size = CellSize.Small;
                    cell.Color = color;
                }

            }
            CreateNextColor();
        }

        public List<string> NextColors { get; set; } = new List<string>();

        private void CreateNextColor()
        {
            NextColors.Clear();
            for (int i = 0; i < CountOfNewGeneration; i++)
            {
                var r = random.Next(colors.Length);
                NextColors.Add(colors[r]);
            }

            OnGenerateNewColors?.Invoke(this, NextColors.ToArray());
        }

        public void NextGeneration()
        {

            for (int i = 0; i < GridSize; i++)
            {
                for (int j = 0; j < GridSize; j++)
                {
                    var cell = Grid[i, j];
                    if (cell.Size == CellSize.Small)
                    {
                        cell.Size = CellSize.Big;
                    }
                }
            }
            CreateNewCellBall();
            OnCellBallsComplete();
        }

        public bool OnCellBallsComplete()
        {
            try
            {
                List<CellBall> allList = new List<CellBall>();
                var newGrid = Grid.Clone() as CellBall[,];
                bool hasCompleteBall = false;

                //horizontal
                for (int i = 0; i < GridSize; i++)
                {
                    for (int j = 0; j < GridSize; j++)
                    {
                        if (j + CountOfComplete <= GridSize)
                        {
                            var first = newGrid[i, j];
                            if (first.Size == CellSize.Big)
                            {
                                List<CellBall> list = new List<CellBall>();
                                list.Add(first);
                                for (int z = j + 1; z < GridSize; z++)
                                {
                                    var last = newGrid[i, z];
                                    if (last.Size == CellSize.Big && last.Color == first.Color)
                                    {
                                        list.Add(last);
                                    }
                                    else
                                        break;
                                }

                                if (list.Count() > 1)
                                {
                                    j = list.Last().ColumnNumber;
                                }

                                if (list.Count() >= CountOfComplete)
                                {
                                    if (allList.Contains(list[0]) && allList.Contains(list[1]))
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        allList.AddRange(list);
                                    }
                                }
                                list.Clear();

                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }



                //vertical
                for (int j = 0; j < GridSize; j++)
                {
                    for (int i = 0; i < GridSize; i++)
                    {
                        //vertical
                        if (i + CountOfComplete <= GridSize)
                        {
                            var first = newGrid[i, j];
                            if (first.Size == CellSize.Big)
                            {
                                List<CellBall> list = new List<CellBall>();
                                list.Add(first);
                                for (int z = i + 1; z < GridSize; z++)
                                {
                                    var last = newGrid[z, j];
                                    if (last.Size == CellSize.Big && last.Color == first.Color)
                                    {
                                        list.Add(last);
                                    }
                                    else
                                        break;
                                }

                                if (list.Count() > 1)
                                {
                                    i = list.Last().RowNumber;
                                }

                                if (list.Count() >= CountOfComplete)
                                {
                                    if (allList.Contains(list[0]) && allList.Contains(list[1]))
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        allList.AddRange(list);
                                    }
                                }
                                list.Clear();
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }


                //horizontal to right
                for (int i = 0; i < GridSize; i++)
                {
                    for (int j = 0; j < GridSize; j++)
                    {
                        var row = i;
                        var col = j;
                        for (int y = row; y < GridSize; y++)
                        {

                            if (col < GridSize && y + CountOfComplete <= GridSize && col + CountOfComplete <= GridSize)
                            {
                                var first = newGrid[y, col];
                                if (first.Size == CellSize.Big)
                                {
                                    List<CellBall> list = new List<CellBall>();
                                    list.Add(first);
                                    for (int z = y + 1; z < GridSize; z++)
                                    {
                                        col++;
                                        if (col >= GridSize)
                                            break;
                                        var last = newGrid[z, col];
                                        if (last.Size == CellSize.Big && last.Color == first.Color)
                                        {
                                            list.Add(last);
                                        }
                                        else
                                            break;
                                    }

                                    if (list.Count() > 1)
                                    {
                                        y = list.Last().RowNumber + 1;
                                        col = list.Last().ColumnNumber + 1;
                                    }

                                    if (list.Count() >= CountOfComplete)
                                    {
                                        if (allList.Contains(list[0]) && allList.Contains(list[1]))
                                        {
                                            continue;
                                        }
                                        else
                                        {
                                            allList.AddRange(list);
                                        }
                                    }
                                    list.Clear();
                                }
                                else
                                    break;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }



                }

                //horizontal to left
                for (int i = 0; i < GridSize; i++)
                {
                    for (int j = 0; j < GridSize; j++)
                    {
                        var row = i;
                        var col = j;
                        for (int y = row; y < GridSize; y++)
                        {
                            if (col > 0 && y + CountOfComplete <= GridSize && col - (CountOfComplete - 1) >= 0)
                            {
                                var first = newGrid[y, col];
                                if (first.Size == CellSize.Big)
                                {
                                    List<CellBall> list = new List<CellBall>();
                                    list.Add(first);
                                    for (int z = y + 1; z < GridSize; z++)
                                    {
                                        col--;
                                        if (col < 0)
                                            break;
                                        var last = newGrid[z, col];
                                        if (last.Size == CellSize.Big && last.Color == first.Color)
                                        {
                                            list.Add(last);
                                        }
                                        else
                                            break;
                                    }

                                    if (list.Count() > 1)
                                    {
                                        y = list.Last().RowNumber + 1;
                                        col = list.Last().ColumnNumber - 1;
                                    }

                                    if (list.Count() >= CountOfComplete)
                                    {
                                        if (allList.Contains(list[0]) && allList.Contains(list[1]))
                                        {
                                            continue;
                                        }
                                        else
                                        {
                                            allList.AddRange(list);
                                        }
                                    }
                                    list.Clear();
                                }
                                else
                                    break;
                            }
                            else
                                break;
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
                    Grid[item.RowNumber, item.ColumnNumber].Size = CellSize.Empty;
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

        private List<CellBall> DiagonalToleft(int i, int j)
        {
            List<CellBall> list = new List<CellBall>();

            var newGrid = Grid.Clone() as CellBall[,];
            for (int col = j; col >= 0; col--)
            {
                var row = i;
                var first = newGrid[row, col];
                if (first.Size == CellSize.Big && row + CountOfComplete <= GridSize && col - CountOfComplete >= 0)
                {
                    list.Add(first);
                    for (int z = row + 1; z < GridSize; z++)
                    {
                        col--;
                        var last = newGrid[z, col];
                        if (last.Size == CellSize.Big && last.Color == first.Color)
                        {
                            list.Add(last);
                        }
                        else
                            break;
                    }


                    if (list.Count >= CountOfComplete)
                    {
                        break;
                    }
                    else
                    {
                        row = list.Last().RowNumber;
                        col = list.Last().ColumnNumber;
                        list.Clear();
                    }

                }
            }
            return list;
        }

        public async Task Move(CellBall start, List<CellBall> path)
        {

            var datastart = Grid.Cast<CellBall>().Where(x => x == start).FirstOrDefault();
            datastart.Size = CellSize.Empty;


            var destination = path.Last();
            (string color, CellSize size) last = (destination.Color, destination.Size);


            OnBallStep?.Invoke(this, false);

            foreach (var item in path.Select((value, index) => new { value, index }))
            {
                var color = item.value.Color;
                var size = item.value.Size;
                item.value.Color = datastart.Color;
                item.value.Size = CellSize.Big;
                await Task.Delay(100);

                if (item.index < path.Count - 1)
                {
                    OnBallStep?.Invoke(this, false);
                    item.value.Color = color;
                    item.value.Size = size;
                }

            }

            if (last.size == CellSize.Small)
            {
                var emptys = Grid.Cast<CellBall>().Where(x => x.Size == CellSize.Empty);
                if (emptys.Any())
                {
                    var c = random.Next(emptys.Count());
                    var cell = emptys.ToArray()[c];
                    cell.Size = CellSize.Big;
                    cell.Color = last.color;
                }
            }


            var hasComleteBall = OnCellBallsComplete();
            if (!hasComleteBall)
            {
                var emptys = Grid.Cast<CellBall>().Where(x => x.Size == CellSize.Empty);
                if (!emptys.Any())
                {
                    OnGameOver?.Invoke(this, true);
                }
                else
                {
                    NextGeneration();
                }
            }
            else
            {
                OnBallStep?.Invoke(this, true);
            }
        }

        public List<CellBall> FindPath(CellBall start, CellBall dest)
        {
            var grid = Grid.Clone() as CellBall[,];
            List<CellBall> Open = new List<CellBall>();
            List<CellBall> Closed = new List<CellBall>();

            // foreach (var item in grid.Cast<CellBall>().ToArray())
            // {
            //     item.SetNeighbors(grid, GridSize);
            // }
            Open.Add(start);
            while (Open.Count > 0)
            {
                var currentCellBall = Open[0];
                for (int i = 1; i < Open.Count; i++)
                {
                    if (Open[i].FCost < currentCellBall.FCost || Open[i].FCost == currentCellBall.FCost && Open[i].HCost < currentCellBall.HCost)
                    {
                        currentCellBall = Open[i];
                    }
                }

                Open.Remove(currentCellBall);
                Closed.Add(currentCellBall);

                if (currentCellBall == dest)
                {
                    var path = RetracePath(start, dest);
                    return path;
                }


                foreach (var neighbor in currentCellBall.GetNeighbors(grid, GridSize))
                {
                    if (neighbor.UnWalkable || Closed.Contains(neighbor))
                    {
                        continue;
                    }

                    int newMovementCostToNeighbor = currentCellBall.GCost + GetDistance(currentCellBall, neighbor);
                    if (newMovementCostToNeighbor < neighbor.GCost || !Open.Contains(neighbor))
                    {
                        neighbor.GCost = newMovementCostToNeighbor;
                        neighbor.HCost = GetDistance(neighbor, dest);
                        neighbor.Parent = currentCellBall;
                        if (!Open.Contains(neighbor))
                            Open.Add(neighbor);
                    }
                }
            }
            return null;
        }

        List<CellBall> RetracePath(CellBall start, CellBall dest)
        {
            List<CellBall> pathx = new List<CellBall>();
            CellBall currentNode = dest;
            while (currentNode != start)
            {
                pathx.Add(currentNode);
                currentNode = currentNode.Parent;
            }
            pathx.Reverse();
            return pathx;
        }

        int GetDistance(CellBall a, CellBall b)
        {
            int distx = Math.Abs(a.RowNumber - b.RowNumber);
            int disty = Math.Abs(a.ColumnNumber - b.ColumnNumber);
            if (distx > disty)
                return 14 * distx + 10 * (distx - disty);
            return 14 * distx + 10 * (disty - distx);
        }


    }
}
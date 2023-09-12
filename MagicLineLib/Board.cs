

using System.Drawing;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MagicLineLib
{
    public class Board
    {
        //Color[] colors = new Color[] { Color.Red, Color.Blue, Color.Yellow, Color.Pink, Color.White, Color.Green };
        string[] colorHex = new string[] { "#FFFFFF", "#235789", "#05B2DC", "#5B7553", "#FDE74C", "#E84855", "#9C0D38", "#FA9F42", "#B735FD", };
        Color[] colors;
        Random random = new Random();
        public int GridSize { get; set; }
        public Cell[,] Grid { get; set; }

        public int Score { get; set; }

        int CountOfNewGeneration = 3;
        int CountOfComplete = 5;

        public EventHandler<int> OnChangeScore;


        public Board(int gridSize, int countOfCOlor = 9, int countOfComplete = 5, int countOfNewGeneration = 3)
        {
            this.CountOfComplete = countOfComplete;
            this.CountOfNewGeneration = countOfNewGeneration;
            colors = colorHex.Select(x => ColorTranslator.FromHtml(x)).Take(countOfCOlor).ToArray();
            GridSize = gridSize;
            Grid = new Cell[GridSize, GridSize];

            for (int i = 0; i < GridSize; i++)
            {
                for (int j = 0; j < GridSize; j++)
                {
                    Grid[i, j] = new Cell(i, j);
                }
            }
            NextGeneration();
        }

        public void CreateNewCell()
        {

            var emptys = Grid.Cast<Cell>().Where(x => x.Size == CellSize.Empty);
            var c = random.Next(emptys.Count());
            var r = random.Next(colors.Length);
            Color color = colors[r];
            var cell = emptys.ToArray()[c];
            cell.Size = CellSize.Small;
            cell.Color = color;
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

            for (int i = 0; i < CountOfNewGeneration; i++)
            {
                CreateNewCell();
            }
            OnCellsComplete();

        }

        public bool OnCellsComplete()
        {
            try
            {
                List<Cell> allList = new List<Cell>();
                var newGrid = Grid.Clone() as Cell[,];
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
                                List<Cell> list = new List<Cell>();
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
                                List<Cell> list = new List<Cell>();
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
                                    List<Cell> list = new List<Cell>();
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
                                    List<Cell> list = new List<Cell>();
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

                    Score += score;
                    OnChangeScore?.Invoke(this, Score);
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


        private List<Cell> DiagonalToleft(int i, int j)
        {
            List<Cell> list = new List<Cell>();

            var newGrid = Grid.Clone() as Cell[,];
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

        public async Task Move(Cell start, List<Cell> path)
        {

            var datastart = Grid.Cast<Cell>().Where(x => x == start).FirstOrDefault();

            datastart.Size = CellSize.Empty;
            await Task.Delay(100);

            foreach (var item in path.Select((value, index) => new { value, index }))
            {
                var color = item.value.Color;
                var size = item.value.Size;
                item.value.Color = datastart.Color;
                item.value.Size = CellSize.Big;
                await Task.Delay(100);
                if (item.index < path.Count - 1)
                {
                    item.value.Color = color;
                    item.value.Size = size;
                }

            }
            var hasComleteBall = OnCellsComplete();
            if (!hasComleteBall)
                NextGeneration();
        }

        public List<Cell> FindPath(Cell start, Cell dest)
        {
            var grid = Grid.Clone() as Cell[,];
            List<Cell> Open = new List<Cell>();
            List<Cell> Closed = new List<Cell>();

            // foreach (var item in grid.Cast<Cell>().ToArray())
            // {
            //     item.SetNeighbors(grid, GridSize);
            // }
            Open.Add(start);
            while (Open.Count > 0)
            {
                var currentCell = Open[0];
                for (int i = 1; i < Open.Count; i++)
                {
                    if (Open[i].FCost < currentCell.FCost || Open[i].FCost == currentCell.FCost && Open[i].HCost < currentCell.HCost)
                    {
                        currentCell = Open[i];
                    }
                }

                Open.Remove(currentCell);
                Closed.Add(currentCell);

                if (currentCell == dest)
                {
                    var path = RetracePath(start, dest);
                    return path;
                }


                foreach (var neighbor in currentCell.GetNeighbors(grid, GridSize))
                {
                    if (neighbor.UnWalkable || Closed.Contains(neighbor))
                    {
                        continue;
                    }

                    int newMovementCostToNeighbor = currentCell.GCost + GetDistance(currentCell, neighbor);
                    if (newMovementCostToNeighbor < neighbor.GCost || !Open.Contains(neighbor))
                    {
                        neighbor.GCost = newMovementCostToNeighbor;
                        neighbor.HCost = GetDistance(neighbor, dest);
                        neighbor.Parent = currentCell;
                        if (!Open.Contains(neighbor))
                            Open.Add(neighbor);
                    }
                }
            }
            return null;
        }

        List<Cell> RetracePath(Cell start, Cell dest)
        {
            List<Cell> pathx = new List<Cell>();
            Cell currentNode = dest;
            while (currentNode != start)
            {
                pathx.Add(currentNode);
                currentNode = currentNode.Parent;
            }
            pathx.Reverse();
            return pathx;
        }

        int GetDistance(Cell a, Cell b)
        {
            int distx = Math.Abs(a.RowNumber - b.RowNumber);
            int disty = Math.Abs(a.ColumnNumber - b.ColumnNumber);
            if (distx > disty)
                return 14 * distx + 10 * (distx - disty);
            return 14 * distx + 10 * (disty - distx);
        }


    }
}
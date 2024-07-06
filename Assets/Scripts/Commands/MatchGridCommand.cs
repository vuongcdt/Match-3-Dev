using System.Collections.Generic;
using Events;
using QFramework;
using Queries;

namespace Commands
{
    public class MatchGridCommand : AbstractCommand
    {
        private Cell[,] _grid;

        protected override void OnExecute()
        {
            _grid = this.SendQuery(new GetGridQuery());
            MatchGrid();
        }

        private void MatchGrid()
        {
            List<List<Cell>> cellsList = new();
            for (int x = 0; x < _grid.GetLength(0); x++)
            {
                int index = 0;
                for (int y = 0; y < _grid.GetLength(1) - 2; y++)
                {
                    if (index >= y)
                    {
                        continue;
                    }

                    var currentCell = _grid[x, y];
                    index = MatchCellX(x, y, currentCell, cellsList);
                }
            }

            for (int y = 0; y < _grid.GetLength(1); y++)
            {
                int index = 0;
                for (int x = 0; x < _grid.GetLength(0) - 2; x++)
                {
                    if (index >= x)
                    {
                        continue;
                    }

                    var currentCell = _grid[x, y];
                    index = MatchCellY(x, y, currentCell, cellsList);
                }
            }

            MergeCells(cellsList);
        }

        private void MergeCells(List<List<Cell>> cellsList)
        {
            cellsList.Sort((listA, listB) => listB.Count - listA.Count);
            foreach (var cells in cellsList)
            {
                var isActiveAll = IsActiveAll(cells);
                if (!isActiveAll)
                {
                    continue;
                }

                foreach (var cellMerge in cells)
                {
                    cellMerge.gameObject.SetActive(false);
                    cellMerge.Type = CONSTANTS.CellType.None;
                }
            }

            if (cellsList.Count > 0)
            {
                this.SendEvent<ProcessingEvent>();
            }
        }

        private bool IsActiveAll(List<Cell> cells)
        {
            foreach (var cellMerge in cells)
            {
                if (!cellMerge.gameObject.activeSelf)
                {
                    return false;
                }
            }

            return true;
        }

        private int MatchCellX(int x, int y, Cell currentCell, List<List<Cell>> cellsList)
        {
            List<Cell> cells = new();
            for (int newY = y + 1; newY < _grid.GetLength(1); newY++)
            {
                var upCell = _grid[x, newY];
                if (upCell.Type == currentCell.Type)
                {
                    cells.Add(upCell);
                }
                else
                {
                    break;
                }
            }

            if (cells.Count >= 2)
            {
                cells.Add(currentCell);
                cellsList.Add(cells);
            }

            return cells.Count + y;
        }

        private int MatchCellY(int x, int y, Cell currentCell, List<List<Cell>> cellsList)
        {
            List<Cell> cells = new();
            for (int newX = x + 1; newX < _grid.GetLength(0); newX++)
            {
                var upCell = _grid[newX, y];
                if (upCell.Type == currentCell.Type)
                {
                    cells.Add(upCell);
                }
                else
                {
                    break;
                }
            }

            if (cells.Count >= 2)
            {
                cells.Add(currentCell);
                cellsList.Add(cells);
            }

            return cells.Count + x;
        }
    }
}
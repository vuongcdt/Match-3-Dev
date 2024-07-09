using System.Collections.Generic;
using Events;
using GameControllers;
using QFramework;
using Queries;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Commands
{
    public class MatchGridCommand : AbstractCommand<bool>
    {
        private Cell[,] _grid;
        private static readonly int RowAnimator = Animator.StringToHash("Row");
        private static readonly int ColumnAnimator = Animator.StringToHash("Column");

        protected override bool OnExecute()
        {
            _grid = this.SendQuery(new GetGridQuery());
            if (MatchGrid())
            {
                this.SendEvent<ProcessingGridEvent>();
                return true;
            }

            return false;
        }

        private bool MatchGrid()
        {
            List<Utils.MatchCell> cellsList = new();
            for (int x = 0; x < _grid.GetLength(0); x++)
            {
                int index = -1;
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
                int index = -1;
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

            return MergeCells(cellsList);
        }

        private bool MergeCells(List<Utils.MatchCell> cellsList)
        {
            cellsList.Sort((listA, listB) => listB.CellList.Count - listA.CellList.Count);
            foreach (var cells in cellsList)
            {
                var cellList = cells.CellList;

                var random = Random.Range(0, cellList.Count);
                RemoveObstacle(cellList);

                for (var index = 0; index < cellList.Count; index++)
                {
                    if (index == random && cellList.Count == 4)
                    {
                        SetTriggerAndSpecialType(cells, index);
                        continue;
                    }

                    if (index == random && cellList.Count > 5)
                    {
                        cellList[index].SpecialType = CONSTANTS.CellSpecialType.Color;
                        cellList[index].Type = CONSTANTS.CellType.Rainbow;
                        continue;
                    }

                    var cellMerge = cellList[index];
                    cellMerge.DeActive();
                }
            }

            return cellsList.Count > 0;
        }

        private void RemoveObstacle(List<Cell> cellList)
        {
            var configGame = ConfigGame.Instance;

            foreach (var currentCell in cellList)
            {
                int[] checkArr = { 1, -1 };
                foreach (var index in checkArr)
                {
                    Cell cellObstacle = null;
                    if (currentCell.GridPosition.x + index >= 0 &&
                        currentCell.GridPosition.x + index < configGame.Width)
                    {
                        cellObstacle = _grid[currentCell.GridPosition.x + index, currentCell.GridPosition.y];
                    }

                    if (currentCell.GridPosition.y + index >= 0 &&
                        currentCell.GridPosition.y + index < configGame.Height)
                    {
                        cellObstacle = _grid[currentCell.GridPosition.x, currentCell.GridPosition.y + index];
                    }

                    if (cellObstacle != null && cellObstacle.Type == CONSTANTS.CellType.Obstacle)
                    {
                        cellObstacle.Type = CONSTANTS.CellType.None;
                    }
                }
            }
        }

        private int MatchCellX(int x, int y, Cell currentCell, List<Utils.MatchCell> cellsList)
        {
            List<Cell> cells = new();
            for (int newY = y + 1; newY < _grid.GetLength(1); newY++)
            {
                var upCell = _grid[x, newY];
                if (IsCanMerge(currentCell, upCell, cells)) break;
            }

            if (cells.Count >= 2)
            {
                cells.Add(currentCell);
                cellsList.Add(new Utils.MatchCell(cells, CONSTANTS.GridType.Row));
            }

            return cells.Count + y;
        }

        private int MatchCellY(int x, int y, Cell currentCell, List<Utils.MatchCell> cellsList)
        {
            List<Cell> cells = new();
            for (int newX = x + 1; newX < _grid.GetLength(0); newX++)
            {
                var upCell = _grid[newX, y];
                if (IsCanMerge(currentCell, upCell, cells)) break;
            }

            if (cells.Count >= 2)
            {
                cells.Add(currentCell);
                cellsList.Add(new Utils.MatchCell(cells, CONSTANTS.GridType.Column));
            }

            return cells.Count + x;
        }

        private static bool IsCanMerge(Cell currentCell, Cell upCell, List<Cell> cells)
        {
            var isNotObstacle = currentCell.Type != CONSTANTS.CellType.Obstacle;
            var isNotNone = currentCell.Type != CONSTANTS.CellType.None;

            if (upCell.Type == currentCell.Type && isNotObstacle && isNotNone)
            {
                cells.Add(upCell);
            }
            else
            {
                return true;
            }

            return false;
        }

        private static void SetTriggerAndSpecialType(Utils.MatchCell matchCell, int index)
        {
            var cells = matchCell.CellList;
            var isTriggerRow = matchCell.Type == CONSTANTS.GridType.Row;

            cells[index].GetComponentInChildren<Animator>().SetTrigger(isTriggerRow ? RowAnimator : ColumnAnimator);
            cells[index].SpecialType = isTriggerRow ? CONSTANTS.CellSpecialType.Row : CONSTANTS.CellSpecialType.Column;
        }
    }
}
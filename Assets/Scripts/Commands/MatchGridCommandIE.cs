using System.Collections.Generic;
using Events;
using QFramework;
using Queries;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Commands
{
    public class MatchGridCommandIE : AbstractCommand
    {
        private Cell[,] _grid;
        private ConfigGame _configGame;
        private static readonly int RowAnimator = Animator.StringToHash("Row");
        private static readonly int ColumnAnimator = Animator.StringToHash("Column");

        protected override void OnExecute()
        {
            _grid = this.SendQuery(new GetGridQuery());
            _configGame = ConfigGame.Instance;

            // yield return new WaitForSeconds(_configGame.FillTime * 2);
            MatchGrid();
        }

        private void MatchGrid()
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

            MergeCells(cellsList);
            this.SendEvent<ProcessingGridEvent>();
        }

        private int MatchCellX(int x, int y, Cell currentCell, List<Utils.MatchCell> cellsList)
        {
            List<Cell> cells = new();
            for (int newY = y + 1; newY < _grid.GetLength(1); newY++)
            {
                var upCell = _grid[x, newY];
                if (GetValue(currentCell, upCell, cells)) break;
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
                if (GetValue(currentCell, upCell, cells)) break;
            }

            if (cells.Count >= 2)
            {
                cells.Add(currentCell);
                cellsList.Add(new Utils.MatchCell(cells, CONSTANTS.GridType.Column));
            }

            return cells.Count + x;
        }

        private static bool GetValue(Cell currentCell, Cell upCell, List<Cell> cells)
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

        private void MergeCells(List<Utils.MatchCell> cellsList)
        {
            cellsList.Sort((listA, listB) => listB.CellList.Count - listA.CellList.Count);
            foreach (var cells in cellsList)
            {
                var cellList = cells.CellList;
                var isActiveAll = IsActiveAll(cellList);

                if (!isActiveAll)
                {
                    continue;
                }

                var random = Random.Range(0, cellList.Count);
                for (var index = 0; index < cellList.Count; index++)
                {
                    if (index == random && cellList.Count == 4)
                    {
                        // SetTriggerAndSpecialType(cells, index);
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

        private static void SetTriggerAndSpecialType(Utils.MatchCell matchCell, int index)
        {
            var cells = matchCell.CellList;
            var isTriggerRow = matchCell.Type == CONSTANTS.GridType.Row;

            cells[index].GetComponentInChildren<Animator>().SetTrigger(isTriggerRow ? RowAnimator : ColumnAnimator);
            cells[index].SpecialType = isTriggerRow ? CONSTANTS.CellSpecialType.Row : CONSTANTS.CellSpecialType.Column;
        }
    }
}
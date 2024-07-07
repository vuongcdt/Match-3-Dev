using System;
using System.Collections.Generic;
using Events;
using QFramework;
using Queries;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Commands
{
    public class MatchGridCommand : AbstractCommand
    {
        private Cell[,] _grid;
        private static readonly int Row = Animator.StringToHash("Row");
        private static readonly int Column = Animator.StringToHash("Column");

        protected override void OnExecute()
        {
            _grid = this.SendQuery(new GetGridQuery());
            MatchGrid();
        }

        private void MatchGrid()
        {
            List<Utils.MatchCell> cellsList = new();
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

        private void MergeCells(List<Utils.MatchCell> cellsList)
        {
            cellsList.Sort((listA, listB) => listB.CellList.Count - listA.CellList.Count);
            foreach (var cells in cellsList)
            {
                var isActiveAll = IsActiveAll(cells.CellList);
                if (!isActiveAll)
                {
                    continue;
                }

                var random = Random.Range(0, cells.CellList.Count);
                for (var index = 0; index < cells.CellList.Count; index++)
                {
                    if (index == random && cells.CellList.Count > 3)
                    {
                        SetTrigger(cells, index);
                        continue;
                    }

                    var cellMerge = cells.CellList[index];
                    cellMerge.gameObject.SetActive(false);
                    cellMerge.Type = CONSTANTS.CellType.None;
                }
            }

            if (cellsList.Count > 0)
            {
                this.SendEvent<ProcessingEvent>();
            }
        }

        private static void SetTrigger(Utils.MatchCell matchCell, int index)
        {
            var cells = matchCell.CellList;
            var isTriggerRow = matchCell.Type == CONSTANTS.GridType.Row;

            cells[index].GetComponentInChildren<Animator>().SetTrigger(isTriggerRow ? Row : Column);
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

        private int MatchCellX(int x, int y, Cell currentCell, List<Utils.MatchCell> cellsList)
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
                cellsList.Add(new Utils.MatchCell(cells, CONSTANTS.GridType.Column));
            }

            return cells.Count + x;
        }
    }
}
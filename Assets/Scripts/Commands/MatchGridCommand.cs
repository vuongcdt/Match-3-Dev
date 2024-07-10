﻿using System.Collections.Generic;
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
        private ConfigGame _configGame;
        private static readonly int RowAnimator = Animator.StringToHash("Row");
        private static readonly int ColumnAnimator = Animator.StringToHash("Column");


        protected override bool OnExecute()
        {
            _grid = this.SendQuery(new GetGridQuery());
            _configGame = ConfigGame.Instance;
            if (MatchGrid())
            {
                this.SendEvent<ProcessingGridEvent>();
                return true;
            }

            return false;
        }

        private bool MatchGrid()
        {
            List<Utils.MatchCell> matchCellList = new();
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
                    index = MatchCellX(x, y, currentCell, matchCellList);
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
                    index = MatchCellY(x, y, currentCell, matchCellList);
                }
            }

            return MergeCells(matchCellList);
        }

        private bool MergeCells(List<Utils.MatchCell> matchCellList)
        {
            matchCellList.Sort((listA, listB) => listB.CellList.Count - listA.CellList.Count);
            foreach (var matchCell in matchCellList)
            {
                var cellList = matchCell.CellList;

                var random = Random.Range(0, cellList.Count);

                for (var index = 0; index < cellList.Count; index++)
                {
                    var cell = cellList[index];
                    this.SendCommand(new RemoveObstacleCommand(cell.GridPosition.x, cell.GridPosition.y));

                    MergeCellSpecial(cell);

                    if (MergeCellByCount(index, random, cellList, cell, matchCell)) continue;

                    cell.DeActive();
                }
            }

            return matchCellList.Count > 0;
        }

        private static bool MergeCellByCount(int index, int random, List<Cell> cellList, Cell cell,
            Utils.MatchCell matchCell)
        {
            if (index == random && cellList.Count >= 5)
            {
                cell.SpecialType = CONSTANTS.CellSpecialType.Rainbow;
                cell.Type = CONSTANTS.CellType.Rainbow;
                return true;
            }

            if (index == random && cellList.Count == 4)
            {
                SetTriggerAndSpecialType(matchCell, index);
                return true;
            }

            return false;
        }

        private void MergeCellSpecial(Cell cell)
        {
            if (cell.SpecialType == CONSTANTS.CellSpecialType.Column)
            {
                for (int y = 0; y < _configGame.Height; y++)
                {
                    _grid[cell.GridPosition.x, y].Type = CONSTANTS.CellType.None;
                }
            }

            if (cell.SpecialType == CONSTANTS.CellSpecialType.Row)
            {
                for (int x = 0; x < _configGame.Width; x++)
                {
                    _grid[x, cell.GridPosition.y].Type = CONSTANTS.CellType.None;
                }
            }
        }

        private int MatchCellX(int x, int y, Cell currentCell, List<Utils.MatchCell> matchCellList)
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
                matchCellList.Add(new Utils.MatchCell(cells, CONSTANTS.GridType.Column));
            }

            return cells.Count + y;
        }

        private int MatchCellY(int x, int y, Cell currentCell, List<Utils.MatchCell> matchCellList)
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
                matchCellList.Add(new Utils.MatchCell(cells, CONSTANTS.GridType.Row));
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
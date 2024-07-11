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
        private static readonly int ClearAnimator = Animator.StringToHash("Clear");


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

                MergeCellRowColumn(cellList);

                for (var index = 0; index < cellList.Count; index++)
                {
                    var cell = cellList[index];
                    this.SendCommand(new RemoveObstacleCommand(cell.GridPosition.x, cell.GridPosition.y));

                    if (MergeCellByCount(index, random, cellList, cell, matchCell)) continue;

                    // cell.ClearAvatar();
                    cell.DeActive();
                }
            }

            return matchCellList.Count > 0;
        }

        private void MergeCellRowColumn(List<Cell> cellList)
        {
            foreach (var cell in cellList)
            {
                if (cell.SpecialType == CONSTANTS.CellSpecialType.Column)
                {
                    Debug.Log("Column");
                    for (int newY = 0; newY < _configGame.Height; newY++)
                    {
                        var gridPos = cell.GridPosition;
                        SetObstacleAndFish(gridPos.x, newY);
                    }
                }

                if (cell.SpecialType == CONSTANTS.CellSpecialType.Row)
                {
                    Debug.Log("Row");
                    for (int newX = 0; newX < _configGame.Width; newX++)
                    {
                        var gridPos = cell.GridPosition;
                        SetObstacleAndFish(newX, gridPos.y);
                    }
                }
            }
        }

        private void SetObstacleAndFish(int x, int y)
        {
            this.SendCommand(new RemoveObstacleCommand(x, y));
            _grid[x, y].Type = CONSTANTS.CellType.None;
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
            // cells[index].GetComponentInChildren<Animator>().SetTrigger(ClearAnimator);
            cells[index].SpecialType = isTriggerRow ? CONSTANTS.CellSpecialType.Row : CONSTANTS.CellSpecialType.Column;
        }
    }
}
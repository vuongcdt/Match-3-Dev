using System.Collections.Generic;
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
        private bool _isInverted;

        public MatchGridCommand()
        {
        }

        public MatchGridCommand(bool isInverted)
        {
            _isInverted = isInverted;
        }

        protected override bool OnExecute()
        {
            _grid = this.SendQuery(new GetGridQuery());
            _configGame = ConfigGame.Instance;

            return MatchGrid();
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

            Cell cellCanRainbow = null;
            if (matchCellList.Count == 2 && _isInverted)
            {
                foreach (var cellOut in matchCellList[0].CellList)
                {
                    foreach (var cellIn in matchCellList[1].CellList)
                    {
                        if (cellIn.GridPosition.x == cellOut.GridPosition.x &&
                            cellIn.GridPosition.y == cellOut.GridPosition.y)
                        {
                            cellCanRainbow = cellIn;
                        }
                    }
                }
            }

            if (_isInverted)
            {
                return MergeCells(matchCellList, cellCanRainbow);
            }

            return MergeCells(matchCellList);
        }

        private bool MergeCells(List<Utils.MatchCell> matchCellList, Cell cellCanRainbow = null)
        {
            matchCellList.Sort((listA, listB) => listB.CellList.Count - listA.CellList.Count);

            foreach (var matchCell in matchCellList)
            {
                var cellList = matchCell.CellList;
                var random = Random.Range(0, cellList.Count);
                foreach (var cell in cellList)
                {
                    MergeCellRowColumn(cell);
                }

                for (var index = 0; index < cellList.Count; index++)
                {
                    var cell = cellList[index];
                    this.SendCommand(new ClearObstacleAroundCommand(cell.GridPosition.x, cell.GridPosition.y));

                    if (cellCanRainbow != null && _isInverted)
                    {
                        var isSameGrid = cellCanRainbow.GridPosition.x == cell.GridPosition.x &&
                                         cellCanRainbow.GridPosition.y == cell.GridPosition.y;
                        if (isSameGrid)
                        {
                            cell.SetTypeRainbow();
                        }
                        else
                        {
                            cell.ClearCell();
                        }

                        continue;
                    }

                    if (MergeCellByCount(index, random, cellList, cell, matchCell)) continue;

                    cell.ClearCell();
                }
            }

            return matchCellList.Count > 0;
        }

        private void MergeCellRowColumn(Cell cell)
        {
            var gridPos = cell.GridPosition;
            if (cell.SpecialType == CONSTANTS.CellSpecialType.Column)
            {
                cell.SpecialType = CONSTANTS.CellSpecialType.Normal;
                for (int newY = 0; newY < _configGame.Height; newY++)
                {
                    var newCell = _grid[gridPos.x, newY];
                    CheckRainbow(newCell);
                    MergeCellRowColumn(newCell);
                    this.SendCommand(new ClearObstacleAroundCommand(gridPos.x, newY));

                    ClearFish(gridPos.x, newY);
                }
            }

            if (cell.SpecialType == CONSTANTS.CellSpecialType.Row)
            {
                cell.SpecialType = CONSTANTS.CellSpecialType.Normal;
                for (int newX = 0; newX < _configGame.Width; newX++)
                {
                    var newCell = _grid[newX, gridPos.y];
                    CheckRainbow(newCell);
                    MergeCellRowColumn(newCell);
                    this.SendCommand(new ClearObstacleAroundCommand(newX, gridPos.y));

                    ClearFish(newX, gridPos.y);
                }
            }
        }

        private void CheckRainbow(Cell newCell)
        {
            if (newCell.Type == CONSTANTS.CellType.Rainbow)
            {
                var randomType = Random.Range(3, _configGame.MaxListImage);
                foreach (var cell in _grid)
                {
                    if (cell.Type == (CONSTANTS.CellType)randomType)
                    {
                        cell.ClearCell();
                        this.SendCommand(new ClearObstacleAroundCommand(cell.GridPosition.x, cell.GridPosition.y));
                    }
                }
            }
        }

        private void ClearFish(int x, int y)
        {
            _grid[x, y].ClearCell();
        }

        private static bool MergeCellByCount(int index, int random, List<Cell> cellList, Cell cell,
            Utils.MatchCell matchCell)
        {
            if (index == random && cellList.Count >= 5)
            {
                cell.SetTypeRainbow();
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
            var isRow = matchCell.Type == CONSTANTS.GridType.Row;

            cells[index].SpecialType = isRow ? CONSTANTS.CellSpecialType.Column : CONSTANTS.CellSpecialType.Row;
        }
    }
}
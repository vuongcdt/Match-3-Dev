﻿using GameControllers;
using QFramework;
using Queries;
using UnityEngine;

namespace Commands
{
    public class AddCellToGridCommand : AbstractCommand
    {
        private Cell[,] _grid;
        private ConfigGame _configGame;

        protected override void OnExecute()
        {
            _grid = this.SendQuery(new GetGridQuery());
            _configGame = ConfigGame.Instance;
            AddCellToGrid();
        }

        private void AddCellToGrid()
        {
            for (int x = 0; x < _configGame.Width; x++)
            {
                var cellBelow = _grid[x, _configGame.Height - 1];
                if (cellBelow.Type == CONSTANTS.CellType.None)
                {
                    var random = Random.Range(3, _configGame.MaxListImage);
                    _configGame.IsProcessing = true;
                    var newCell = _configGame.Cell.Create(
                        Utils.GetPositionCell(x, _configGame.Height, _configGame.Width, _configGame.Height,
                            _configGame.CellSize),
                        _configGame.GridBlock,
                        _configGame.AvatarSize,
                        (CONSTANTS.CellType)random);

                    newCell.Move(
                        Utils.GetPositionCell(x, _configGame.Height - 1, _configGame.Width, _configGame.Height,
                            _configGame.CellSize), _configGame.FillTime);
                    _grid[x, _configGame.Height - 1] = newCell;
                }
            }
        }
    }
}
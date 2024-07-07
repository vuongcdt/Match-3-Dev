using System.Collections;
using QFramework;
using Queries;
using UnityEngine;

namespace Commands
{
    public class AddCellToGridCommand : AbstractCommand
    {
        private Cell[,] _grid;
        private int _height;
        private int _width;
        private float _cellSize;
        private float _fillTime;
        
        private Cell _cell;
        private float _avatarSize;
        private Transform _gridBlock;

        private Utils.SettingsGrid _settingsGrid;

        public AddCellToGridCommand(Cell cell, float avatarSize, Transform gridBlock)
        {
            _cell = cell;
            _avatarSize = avatarSize;
            _gridBlock = gridBlock;
        }

        protected override void OnExecute()
        {
            _grid = this.SendQuery(new GetGridQuery());
            _width = _grid.GetLength(0);
            _height = _grid.GetLength(1);

            _settingsGrid = this.SendQuery(new GetSettingsGridQuery());
            _cellSize = _settingsGrid.CellSize;
            _fillTime = _settingsGrid.FillTime;
            
            AddCellToGrid();
        }

        private void AddCellToGrid()
        {
            for (int x = 0; x < _width; x++)
            {
                var cellBelow = _grid[x, _height - 1];
                if (cellBelow.Type == CONSTANTS.CellType.None)
                {
                    var random = Random.Range(3, 9);
                    this.SendQuery(new SetIsProcessingQuery(true));
                    var newCell = _cell.Create(
                        Utils.GetPositionCell(x, _height, _width, _height, _cellSize),
                        _gridBlock,
                        _avatarSize,
                        (CONSTANTS.CellType)random);
                    
                    newCell.Move(Utils.GetPositionCell(x, _height - 1, _width, _height, _cellSize), _fillTime);
                    _grid[x, _height - 1] = newCell;
                }
            }
        }
    }
}
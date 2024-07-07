using QFramework;
using Queries;
using UnityEngine;

namespace Commands
{
    public class RenderCellGridCommand : AbstractCommand
    {
        private Cell[,] _grid;
        private int _width;
        private int _height;

        private Cell _cell;
        private float _cellSize;
        private Transform _gridBlock;
        private float _avatarSize;

        public RenderCellGridCommand(Cell cell, float cellSize, Transform gridBlock, float avatarSize)
        {
            _cell = cell;
            _cellSize = cellSize;
            _gridBlock = gridBlock;
            _avatarSize = avatarSize;
        }

        protected override void OnExecute()
        {
            _grid = this.SendQuery(new GetGridQuery());
            _width = _grid.GetLength(0);
            _height = _grid.GetLength(1);
            RenderCellGrid();
        }

        private void RenderCellGrid()
        {
            for (int x = 0; x < _grid.GetLength(0); x++)
            {
                for (int y = 0; y < _grid.GetLength(1); y++)
                {
                    _cell.Type = CONSTANTS.CellType.None;
                    var newCell = _cell.Create(Utils.GetPositionCell(x, y, _width, _height, _cellSize),
                        _gridBlock,
                        _avatarSize,
                        CONSTANTS.CellType.None);
                    _grid[x, y] = newCell;
                }
            }
        }
    }
}
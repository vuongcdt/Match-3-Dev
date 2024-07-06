using QFramework;
using Queries;
using UnityEngine;

namespace Commands
{
    public class RenderBackgroundGridCommand : AbstractCommand
    {
        private Cell[,] _grid;
        private int _width;
        private int _height;
        
        private Cell _cell;
        private float _cellSize;
        private Transform _backgroundBlock;
        private float _backgroundSize;

        public RenderBackgroundGridCommand(Cell cell, float cellSize, Transform backgroundBlock, float backgroundSize)
        {
            _cell = cell;
            _cellSize = cellSize;
            _backgroundBlock = backgroundBlock;
            _backgroundSize = backgroundSize;
        }

        protected override void OnExecute()
        {
            _grid = this.SendQuery(new GetGridQuery());
            _width = _grid.GetLength(0);
            _height = _grid.GetLength(1);
            RenderBackgroundGrid();
        }

        private void RenderBackgroundGrid()
        {
            for (int x = 0; x < _grid.GetLength(0); x++)
            {
                for (int y = 0; y < _grid.GetLength(1); y++)
                {
                    var background = _cell.Create(
                        Utils.GetPositionCell(x, y, _width, _height, _cellSize),
                        _backgroundBlock,
                        _backgroundSize,
                        CONSTANTS.CellType.Background);
                    // background.name = nameof(CONSTANTS.CellType.Background);
                }
            }
        }
    }
}
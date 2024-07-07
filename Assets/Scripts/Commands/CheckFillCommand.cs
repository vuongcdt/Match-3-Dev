using QFramework;
using Queries;
using UnityEngine;

namespace Commands
{
    public class CheckFillCommand : AbstractCommand
    {
        private int _x;
        private int _y;
        private float _cellSize;
        private float _fillTime;

        private Cell[,] _grid;
        private int _width;
        private int _height;
        private bool _isRevertFill;
        private Utils.SettingsGrid _settingsGrid;

        public CheckFillCommand(int x, int y)
        {
            _x = x;
            _y = y;
        }

        protected override void OnExecute()
        {
            _grid = this.SendQuery(new GetGridQuery());
            _width = _grid.GetLength(0);
            _height = _grid.GetLength(1);

            _settingsGrid = this.SendQuery(new GetSettingsGridQuery());
            _cellSize = _settingsGrid.CellSize;
            _fillTime = _settingsGrid.FillTime;

            _isRevertFill = this.SendQuery(new GetIsRevertFillQuery());
            CheckFill(_x, _y);
        }

        private void CheckFill(int x, int y)
        {
            int[] checkArr = _isRevertFill
                ? new[] { 0, 1, -1 }
                : new[] { 0, -1, 1 };

            foreach (var index in checkArr)
            {
                if (x + index < 0 || x + index >= _width)
                {
                    continue;
                }

                var source = _grid[x, y];
                var target = _grid[x + index, y - 1];

                var isSourceFish = source.Type != CONSTANTS.CellType.None &&
                                   source.Type != CONSTANTS.CellType.Obstacle;
                var isTargetEmpty = target.Type == CONSTANTS.CellType.None;
                var isNextToObstacle = _grid[x + index, y].Type == CONSTANTS.CellType.Obstacle;

                var isSpecial = y + 1 < _height && _grid[x + index, y + 1].Type == CONSTANTS.CellType.Obstacle &&
                                _grid[x, y + 1].Type == CONSTANTS.CellType.Obstacle;

                if (index != 0 && !isNextToObstacle && !isSpecial)
                {
                    continue;
                }

                if (isSourceFish && isTargetEmpty )
                {
                    MoveToBelow(source, target, x, y, index);
                    break;
                }

                if (index != 0 && isSourceFish && isSpecial && _grid[x + index, y].Type == CONSTANTS.CellType.None)
                {
                    target = _grid[x + index, y];
                    MoveToNextTo(source, target, x, y, index);
                    break;
                }
            }
        }

        private void MoveToNextTo(Cell cellSource, Cell cellTarget, int x, int y, int index)
        {
            this.SendQuery(new SetIsProcessingQuery(true));

            cellSource.Move(Utils.GetPositionCell(x + index, y, _width, _height, _cellSize), _fillTime);

            _grid[x + index, y] = cellSource;
            _grid[x, y] = cellTarget;

            cellTarget.gameObject.SetActive(false);
        }

        private void MoveToBelow(Cell cellSource, Cell cellTarget, int x, int y, int index = 0)
        {
            this.SendQuery(new SetIsProcessingQuery(true));

            cellSource.Move(Utils.GetPositionCell(x + index, y - 1, _width, _height, _cellSize), _fillTime);

            _grid[x + index, y - 1] = cellSource;
            _grid[x, y] = cellTarget;

            cellTarget.gameObject.SetActive(false);
        }
    }
}
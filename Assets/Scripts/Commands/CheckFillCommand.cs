using QFramework;
using UnityEngine;

namespace Commands
{
    public class CheckFillCommand : AbstractCommand
    {
        private int x;
        private int y;
        private float cellSize;
        private float fillTime;
        private bool isProcessing;
        private Cell[,] _grid;

        private int width;
        private int height;
        private bool _isRevertFill;

        public CheckFillCommand(int x, int y, float cellSize, float fillTime, bool isProcessing, Cell[,] grid, bool isRevertFill)
        {
            this.x = x;
            this.y = y;
            this.cellSize = cellSize;
            this.fillTime = fillTime;
            this.isProcessing = isProcessing;
            _grid = grid;
            _isRevertFill = isRevertFill;
        }

        public CheckFillCommand(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        protected override void OnExecute()
        {
            CheckFill(x, y);
        }

        private void CheckFill(int x, int y)
        {
            int[] checkArr = _isRevertFill ? new[] { 0, 1, -1 } : new[] { 0, -1, 1 };

            foreach (var index in checkArr)
            {
                if (x + index < 0 || x + index >= width)
                {
                    continue;
                }

                var source = _grid[x, y];
                var target = _grid[x + index, y - 1];
                var isSourceFish = source.Type != CONSTANTS.CellType.None &&
                                   source.Type != CONSTANTS.CellType.Obstacle;
                var isTargetEmpty = target.Type == CONSTANTS.CellType.None;
                var isNextToObstacle = _grid[x + index, y].Type == CONSTANTS.CellType.Obstacle;
                if (index != 0 && !isNextToObstacle)
                {
                    continue;
                }

                if (isSourceFish && isTargetEmpty)
                {
                    MoveToBelow(source, target, x, y, index);
                    break;
                }
            }
        }

        private void MoveToBelow(Cell cellSource, Cell cellTarget, int x, int y, int index = 0)
        {
            isProcessing = true;
            cellSource.Move(GetPositionCell(x + index, y - 1), fillTime);
            _grid[x + index, y - 1] = cellSource;
            _grid[x, y] = cellTarget;
            cellTarget.gameObject.SetActive(false);
        }

        private Vector2 GetPositionCell(int x, int y)
        {
            return new Vector2(x - (width - 1) * 0.5f, y - (height - 1) * 0.5f) * cellSize;
        }
    }
}
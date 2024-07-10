using GameControllers;
using QFramework;
using Queries;

namespace Commands
{
    public class FillCommand : AbstractCommand
    {
        private Cell[,] _grid;

        protected override void OnExecute()
        {
            _grid = this.SendQuery(new GetGridQuery());
            Fill();
        }

        private void Fill()
        {
            var configGame = ConfigGame.Instance;
            for (int y = 1; y < configGame.Height; y++)
            {
                for (int x = 0; x < configGame.Width; x++)
                {
                    CheckFill(x, y);
                }

                configGame.IsRevertFill = !configGame.IsRevertFill;
            }
        }

        private void CheckFill(int x, int y)
        {
            var configGame = ConfigGame.Instance;
            int[] checkArr = configGame.IsRevertFill
                ? new[] { 0, 1, -1 }
                : new[] { 0, -1, 1 };

            foreach (var index in checkArr)
            {
                if (x + index < 0 || x + index >= configGame.Width)
                {
                    continue;
                }

                var source = _grid[x, y];
                var target = _grid[x + index, y - 1];

                var isSourceFish = source.Type != CONSTANTS.CellType.None &&
                                   source.Type != CONSTANTS.CellType.Obstacle;
                var isTargetEmpty = target.Type == CONSTANTS.CellType.None;
                var isNextToObstacle = _grid[x + index, y].Type == CONSTANTS.CellType.Obstacle;

                var isUpObstacle = IsUpObstacle(x, y);
                // var isSpecial = y + 1 < configGame.Height &&
                //                 _grid[x + index, y + 1].Type == CONSTANTS.CellType.Obstacle &&
                //                 _grid[x, y + 1].Type == CONSTANTS.CellType.Obstacle;     
                // if (index != 0 && !isNextToObstacle && !isSpecial)
                // {
                //     continue;
                // }
                if (index != 0 && !isNextToObstacle && !isUpObstacle)
                {
                    continue;
                }

                if (isSourceFish && isTargetEmpty)
                {
                    MoveToBelow(source, target, x, y, index);
                    break;
                }

                // if (index != 0 && isSourceFish && isSpecial && _grid[x + index, y].Type == CONSTANTS.CellType.None)
                // {
                //     target = _grid[x + index, y];
                //     MoveToNextTo(source, target, x, y, index);
                //     break;
                // }
            }
        }

        private bool IsUpObstacle(int x, int y)
        {
            if (y + 1 > ConfigGame.Instance.Height - 1)
            {
                return false;
            }

            return _grid[x, y + 1].Type == CONSTANTS.CellType.Obstacle;
        }

        private void MoveToNextTo(Cell cellSource, Cell cellTarget, int x, int y, int index)
        {
            cellSource.GridPosition = new Utils.GridPos(x + index, y);
            _grid[x + index, y] = cellSource;
            _grid[x, y] = cellTarget;

            cellTarget.DeActive();
        }

        private void MoveToBelow(Cell cellSource, Cell cellTarget, int x, int y, int index = 0)
        {
            cellSource.GridPosition = new Utils.GridPos(x + index, y - 1);
            _grid[x + index, y - 1] = cellSource;
            _grid[x, y] = cellTarget;

            cellTarget.DeActive();
        }
    }
}
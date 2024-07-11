using Events;
using GameControllers;
using QFramework;
using Queries;

namespace Commands
{
    public class InvertedCellCommand : AbstractCommand<bool>
    {
        private Utils.GridPos _sourcePos;
        private Utils.GridPos _targetPos;
        private Cell[,] _grid;

        public InvertedCellCommand(Utils.GridPos sourcePos, Utils.GridPos targetPos)
        {
            _sourcePos = sourcePos;
            _targetPos = targetPos;
        }

        protected override bool OnExecute()
        {
            _grid = this.SendQuery(new GetGridQuery());
            return InvertedCell(_sourcePos, _targetPos);
        }

        private bool InvertedCell(Utils.GridPos sourceGridPos, Utils.GridPos targetGridPos)
        {
            var sourceCell = _grid[sourceGridPos.x, sourceGridPos.y];
            var targetCell = _grid[targetGridPos.x, targetGridPos.y];

            if (!IsInverted(sourceCell, targetCell))
            {
                return false;
            }

            if (InvertedRainbow(targetGridPos, targetCell, sourceCell)) return true;

            sourceCell.GridPosition = targetGridPos;
            targetCell.GridPosition = sourceGridPos;

            _grid[sourceGridPos.x, sourceGridPos.y] = targetCell;
            _grid[targetGridPos.x, targetGridPos.y] = sourceCell;
            return false;
        }

        private bool InvertedRainbow(Utils.GridPos targetGridPos, Cell targetCell, Cell sourceCell)
        {
            var isTargetRainbow = targetCell.SpecialType == CONSTANTS.CellSpecialType.Rainbow;
            var isSourceRanbow = sourceCell.SpecialType == CONSTANTS.CellSpecialType.Rainbow;
            if (!(isTargetRainbow || isSourceRanbow))
            {
                return false;
            }

            sourceCell.GridPosition = targetGridPos;
            var typeFish = isTargetRainbow
                ? sourceCell.Type
                : targetCell.Type;

            var rainbowCell = isTargetRainbow ? targetCell : sourceCell;
            rainbowCell.Type = CONSTANTS.CellType.None;
            rainbowCell.SpecialType = CONSTANTS.CellSpecialType.Normal;

            foreach (var cell in _grid)
            {
                if (cell.Type == typeFish)
                {
                    cell.Type = CONSTANTS.CellType.None;
                    this.SendCommand(new ClearObstacleCommand(cell.GridPosition.x, cell.GridPosition.y));
                }
            }

            this.SendEvent<ProcessingGridEvent>();

            return true;
        }

        private bool IsInverted(Cell sourceCell, Cell targetCell)
        {
            var isObstacle = sourceCell.Type == CONSTANTS.CellType.Obstacle ||
                             targetCell.Type == CONSTANTS.CellType.Obstacle;
            var isEmpty = sourceCell.Type == CONSTANTS.CellType.None || targetCell.Type == CONSTANTS.CellType.None;

            return !isObstacle && !isEmpty;
        }
    }
}
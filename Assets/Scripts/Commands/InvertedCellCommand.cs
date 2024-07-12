using GameControllers;
using QFramework;
using Queries;

namespace Commands
{
    public class InvertedCellCommand : AbstractCommand<bool>
    {
        private Cell _sourceCell;
        private Cell _targetCell;
        private Cell[,] _grid;
        private bool _isInverted;

        public InvertedCellCommand(Cell sourceCell, Cell targetCell)
        {
            _sourceCell = sourceCell;
            _targetCell = targetCell;
        }

        protected override bool OnExecute()
        {
            _grid = this.SendQuery(new GetGridQuery());
            return InvertedCell(_sourceCell, _targetCell);
        }

        private bool InvertedCell(Cell sourceCell, Cell targetCell)
        {
            if (!IsInverted(sourceCell, targetCell))
            {
                return false;
            }

            var sourceGridPos = sourceCell.GridPosition;
            var targetGridPos = targetCell.GridPosition;

            _grid[sourceGridPos.x, sourceGridPos.y] = targetCell;
            _grid[targetGridPos.x, targetGridPos.y] = sourceCell;

            var tempPos = new Utils.GridPos(sourceGridPos.x, sourceGridPos.y);
            sourceCell.GridPosition = targetGridPos;
            targetCell.GridPosition = tempPos;
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
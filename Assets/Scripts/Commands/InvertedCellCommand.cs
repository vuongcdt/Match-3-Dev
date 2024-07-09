using QFramework;
using Queries;

namespace Commands
{
    public class InvertedCellCommand : AbstractCommand
    {
        private Utils.GridPos _sourcePos;
        private Utils.GridPos _targetPos;

        public InvertedCellCommand(Utils.GridPos sourcePos, Utils.GridPos targetPos)
        {
            _sourcePos = sourcePos;
            _targetPos = targetPos;
        }

        protected override void OnExecute()
        {
            InvertedCell(_sourcePos, _targetPos);
        }

        private void InvertedCell(Utils.GridPos sourceGridPos, Utils.GridPos targetGridPos)
        {
            var grid = this.SendQuery(new GetGridQuery());

            var sourceCell = grid[sourceGridPos.x, sourceGridPos.y];
            var targetCell = grid[targetGridPos.x, targetGridPos.y];

            if (targetCell is { Type : CONSTANTS.CellType.Obstacle } or { Type: CONSTANTS.CellType.None })
            {
                return;
            }

            sourceCell.GridPosition = targetGridPos;
            targetCell.GridPosition = sourceGridPos;

            grid[sourceGridPos.x, sourceGridPos.y] = targetCell;
            grid[targetGridPos.x, targetGridPos.y] = sourceCell;
        }
    }
}
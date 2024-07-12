using System.Collections;
using GameControllers;
using QFramework;
using Queries;
using UnityEngine;

namespace Commands
{
    public class InvertedCellAndMatchCommand : AbstractCommand<IEnumerator>
    {
        private Cell _sourceCell;
        private Cell _targetCell;
        private Cell[,] _grid;
        private ConfigGame _configGame;

        public InvertedCellAndMatchCommand(Cell sourceCell, Cell targetCell)
        {
            _sourceCell = sourceCell;
            _targetCell = targetCell;
        }

        protected override IEnumerator OnExecute()
        {
            _configGame = ConfigGame.Instance;
            _grid = this.SendQuery(new GetGridQuery());
            return InvertedAndMatch(_sourceCell, _targetCell);
        }

        private IEnumerator InvertedAndMatch(Cell sourceCell, Cell targetCell)
        {
            if (IsNotInverted(sourceCell, targetCell))
            {
                _configGame.IsDragged = false;
                _configGame.IsProcessing = false;
                yield break;
            }

            var isInverted = InvertedCell(sourceCell, targetCell);

            yield return new WaitForSeconds(_configGame.FillTime);

            var isMatch = this.SendCommand(new MatchGridCommand());

            if (!isMatch || !isInverted)
            {
                InvertedCell(targetCell, sourceCell);
            }

            yield return new WaitForSeconds(_configGame.MatchTime);

            this.SendCommand<ProcessingGridEventCommand>();

            _configGame.IsDragged = false;
        }

        private bool InvertedCell(Cell sourceCell, Cell targetCell)
        {
            var sourceGridPos = sourceCell.GridPosition;
            var targetGridPos = targetCell.GridPosition;

            _grid[sourceGridPos.x, sourceGridPos.y] = targetCell;
            _grid[targetGridPos.x, targetGridPos.y] = sourceCell;

            var tempPos = new Utils.GridPos(sourceGridPos.x, sourceGridPos.y);
            sourceCell.GridPosition = targetGridPos;
            targetCell.GridPosition = tempPos;
            return true;
        }

        private bool IsNotInverted(Cell sourceCell, Cell targetCell)
        {
            var isObstacle = sourceCell.Type == CONSTANTS.CellType.Obstacle ||
                             targetCell.Type == CONSTANTS.CellType.Obstacle;
            var isEmpty = sourceCell.Type == CONSTANTS.CellType.None || targetCell.Type == CONSTANTS.CellType.None;

            return isObstacle || isEmpty;
        }
    }
}
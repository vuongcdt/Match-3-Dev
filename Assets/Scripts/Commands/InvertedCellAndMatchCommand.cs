using System.Collections;
using Events;
using GameControllers;
using QFramework;
using Queries;
using UnityEngine;

namespace Commands
{
    public class InvertedCellAndMatchCommand : AbstractCommand<IEnumerator>,ICanSendEvent
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
            if (Utils.IsNotInverted(sourceCell, targetCell))
            {
                _configGame.IsDragged = false;
                _configGame.IsProcessing = false;
                yield break;
            }

            var isInverted = InvertedCell(sourceCell, targetCell);

            yield return new WaitForSeconds(_configGame.FillTime);

            var isMatch = this.SendCommand(new MatchGridCommand(true));

            if (!isMatch || !isInverted)
            {
                InvertedCell(targetCell, sourceCell);
            }

            if (isMatch)
            {
                this.SendCommand<SetStepsTotalCommand>();
                yield return new WaitForSeconds(_configGame.MatchTime);
                this.SendEvent<ProcessingGridEvent>();
            }
            
            _configGame.IsProcessing = false;
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
    }
}
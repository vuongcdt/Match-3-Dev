using System.Collections;
using GameControllers;
using QFramework;
using Queries;
using UnityEngine;

namespace Commands
{
    public class InvertedRainbowCommand : AbstractCommand<IEnumerator>
    {
        private Cell[,] _grid;
        private Cell _sourceCell, _targetCell;
        private Utils.GridPos _targetGridPos;
        private bool _isTargetRainbow;
        private ConfigGame _configGame;

        public InvertedRainbowCommand(Cell sourceCell, Cell targetCell, Utils.GridPos targetGridPos,
            bool isTargetRainbow)
        {
            _sourceCell = sourceCell;
            _targetCell = targetCell;
            _targetGridPos = targetGridPos;
            _isTargetRainbow = isTargetRainbow;
        }

        protected override IEnumerator OnExecute()
        {
            _configGame = ConfigGame.Instance;
            _grid = this.SendQuery(new GetGridQuery());
            return InvertedRainbow(_sourceCell, _targetCell, _targetGridPos, _isTargetRainbow);
        }

        private IEnumerator InvertedRainbow(Cell sourceCell, Cell targetCell,
            Utils.GridPos targetGridPos, bool isTargetRainbow)
        {
            sourceCell.GridPosition = targetGridPos;

            var typeFish = isTargetRainbow
                ? sourceCell.Type
                : targetCell.Type;

            var rainbowCell = isTargetRainbow ? targetCell : sourceCell;

            rainbowCell.Type = CONSTANTS.CellType.None;

            foreach (var cell in _grid)
            {
                if (cell.Type == typeFish)
                {
                    cell.ClearCell();
                    this.SendCommand(new ClearObstacleAroundCommand(cell.GridPosition.x, cell.GridPosition.y));
                }
            }

            yield return new WaitForSeconds(ConfigGame.Instance.MatchTime);

            _configGame.IsDragged = false;

            this.SendCommand<ProcessingGridEventCommand>();
        }
    }
}
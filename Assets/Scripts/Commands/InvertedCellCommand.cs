using Interfaces;
using QFramework;
using Queries;
using UnityEngine;

namespace Commands
{
    public class InvertedCellCommand : AbstractCommand, IGameCommand
    {
        private Vector2 _sourcePos;
        private Vector2 _targetPos;
 
        private ConfigGame _configGame;

        public InvertedCellCommand(Vector2 sourcePos, Vector2 targetPos)
        {
            _sourcePos = sourcePos;
            _targetPos = targetPos;
        }

        protected override void OnExecute()
        {
            _configGame = ConfigGame.Instance;
            InvertedCell(_sourcePos, _targetPos);
        }

        private void InvertedCell(Vector2 sourcePos, Vector2 targetPos)
        {
            var grid = this.SendQuery(new GetGridQuery());
            var sourceGridPos = GetGridPos(sourcePos);
            var targetGridPos = GetGridPos(targetPos);

            var sourceCell = grid[sourceGridPos.x, sourceGridPos.y];
            var targetCell = grid[targetGridPos.x, targetGridPos.y];

            if (targetCell is { Type : CONSTANTS.CellType.Obstacle } or { Type: CONSTANTS.CellType.None })
            {
                return;
            }

            sourceCell.Move(targetPos, 0.1f);
            targetCell.Move(sourcePos, 0.1f);

            grid[sourceGridPos.x, sourceGridPos.y] = targetCell;
            grid[targetGridPos.x, targetGridPos.y] = sourceCell;
        }

        private Utils.GridPos GetGridPos(Vector2 pos)
        {
            var width = (-pos.x / _configGame.CellSize + (_configGame.Width - 1) * 0.5f);
            var height = (-pos.y / _configGame.CellSize + (_configGame.Height - 1) * 0.5f);

            var gridWidth = _configGame.Width - 1 - (int)width;
            var gridHeight = _configGame.Height - 1 - (int)height;

            return new Utils.GridPos(gridWidth, gridHeight);
        }
    }
}
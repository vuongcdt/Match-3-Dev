using QFramework;
using Queries;
using UnityEngine;

namespace Commands
{
    public class InvertedCellCommand : AbstractCommand
    {
        private Vector2 _sourcePos;
        private Vector2 _targetPos;
        private Utils.SettingsGrid _settingsGrid;
        private Cell _currentCell;

        public InvertedCellCommand(Vector2 sourcePos, Vector2 targetPos)
        {
            this._sourcePos = sourcePos;
            this._targetPos = targetPos;
        }
        protected override void OnExecute()
        {
            _settingsGrid = this.SendQuery(new GetSettingsGridQuery());

            InvertedCell(_sourcePos, _targetPos);
        }
        
        private void InvertedCell(Vector2 sourcePos, Vector2 targetPos)
        {
            var grid = this.SendQuery(new GetGridQuery());
            var sourceGridPos = GetGridPos(sourcePos);
            var targetGridPos = GetGridPos(targetPos);

            var sourceCell = grid[sourceGridPos.x, sourceGridPos.y];
            var targetCell = grid[targetGridPos.x, targetGridPos.y];

            sourceCell.Move(targetPos, 0.1f);
            targetCell.Move(sourcePos, 0.1f);

            grid[sourceGridPos.x, sourceGridPos.y] = targetCell;
            grid[targetGridPos.x, targetGridPos.y] = sourceCell;
        }

        private Utils.GridPos GetGridPos(Vector2 pos)
        {
            var width = (-pos.x / _settingsGrid.CellSize + (_settingsGrid.Width - 1) * 0.5f);
            var height = (-pos.y / _settingsGrid.CellSize + (_settingsGrid.Height - 1) * 0.5f);

            var gridWidth = _settingsGrid.Width - 1 - (int)width;
            var gridHeight = _settingsGrid.Height - 1 - (int)height;

            return new Utils.GridPos(gridWidth, gridHeight);
        }
    }
}
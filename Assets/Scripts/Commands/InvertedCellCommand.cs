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
        private Utils.SettingsGrid _settingsGrid;
        private Cell _sourceCell;
        private Cell _targetCell;
        private Cell[,] _grid;
        private Utils.GridPos _sourceGridPos;
        private Utils.GridPos _targetGridPos;

        public InvertedCellCommand(Vector2 sourcePos, Vector2 targetPos)
        {
            this._sourcePos = sourcePos;
            this._targetPos = targetPos;
        }

        public void Undo()
        {
            _sourceCell.Move(_sourcePos, 0.1f);
            _targetCell.Move(_targetPos, 0.1f);

            // _grid[_sourceGridPos.x, _sourceGridPos.y] = _sourceCell;
            // _grid[_targetGridPos.x, _targetGridPos.y] = _targetCell;
        }

        protected override void OnExecute()
        {
            _settingsGrid = this.SendQuery(new GetSettingsGridQuery());

            InvertedCell(_sourcePos, _targetPos);
        }

        private void InvertedCell(Vector2 sourcePos, Vector2 targetPos)
        {
            var grid = this.SendQuery(new GetGridQuery());
            _sourceGridPos = GetGridPos(sourcePos);
            _targetGridPos = GetGridPos(targetPos);

            _sourceCell = grid[_sourceGridPos.x, _sourceGridPos.y];
            _targetCell = grid[_targetGridPos.x, _targetGridPos.y];

            if (_targetCell is { Type : CONSTANTS.CellType.Obstacle } or { Type: CONSTANTS.CellType.None })
            {
                return;
            }

            _sourceCell.Move(targetPos, 0.1f);
            _targetCell.Move(sourcePos, 0.1f);

            grid[_sourceGridPos.x, _sourceGridPos.y] = _targetCell;
            grid[_targetGridPos.x, _targetGridPos.y] = _sourceCell;
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
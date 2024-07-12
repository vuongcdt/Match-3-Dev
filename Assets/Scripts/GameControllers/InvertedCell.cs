using System.Collections;
using Commands;
using QFramework;
using Queries;
using UnityEngine;

namespace GameControllers
{
    public class InvertedCell : MonoBehaviour, IController
    {
        private Cell _positionCell;
        private IEnumerator _moveIE;
        private Vector3 _clampMagnitude;
        private bool _isDragged;
        private ConfigGame _configGame;
        private Cell[,] _grid;

        private void Awake()
        {
            _positionCell = GetComponent<Cell>();
            _configGame = ConfigGame.Instance;
        }

        private void OnMouseDown()
        {
            if (_configGame.IsDragged || _configGame.IsProcessing)
            {
                return;
            }

            _configGame.IsDragged = true;
        }

        private void OnMouseDrag()
        {
            if (!_configGame.IsDragged || _configGame.IsProcessing)
            {
                return;
            }

            var offset = GetOffset();

            _clampMagnitude = this.SendCommand(new GetClampMagnitudeVectorCommand(offset));
            var newPoint = _clampMagnitude * _configGame.CellSize * 0.9f + _positionCell.WorldPosition;

            this.transform.position = newPoint + Vector3.back;
        }

        private Vector3 GetOffset()
        {
            var input = GetWorldPoint();
            return input - _positionCell.WorldPosition;
        }

        private void OnMouseUp()
        {
            if (!_configGame.IsDragged || _configGame.IsProcessing)
            {
                return;
            }

            _configGame.IsProcessing = true;
            this.transform.position = _positionCell.WorldPosition;

            var targetGridPos = GetTargetGridPos();
            if (!IsPositionInGrid(targetGridPos))
            {
                _configGame.IsProcessing = false;
                return;
            }

            var sourceGridPos = _positionCell.GridPosition;
            _grid = this.SendQuery(new GetGridQuery());

            var targetCell = _grid[sourceGridPos.x, sourceGridPos.y];
            var sourceCell = _grid[targetGridPos.x, targetGridPos.y];

            var isTargetRainbow = targetCell.Type == CONSTANTS.CellType.Rainbow;
            var isSourceRainbow = sourceCell.Type == CONSTANTS.CellType.Rainbow;

            if (isTargetRainbow || isSourceRainbow)
            {
                StartCoroutine(this.SendCommand(
                    new InvertedRainbowCommand(sourceCell, targetCell, targetGridPos, isTargetRainbow)));
            }
            else
            {
                StartCoroutine(this.SendCommand(new InvertedCellAndMatchCommand(sourceCell, targetCell)));
            }
        }

        private Utils.GridPos GetTargetGridPos()
        {
            var directionAxis = _clampMagnitude * _configGame.Sensitivity;

            directionAxis.Normalize();
            var targetGridPos =
                new Utils.GridPos((int)(_positionCell.GridPosition.x + directionAxis.x),
                    (int)(_positionCell.GridPosition.y + directionAxis.y));
            return targetGridPos;
        }

        private bool IsPositionInGrid(Utils.GridPos targetGridPos)
        {
            return targetGridPos.x >= 0 && targetGridPos.x < _configGame.Width &&
                   targetGridPos.y >= 0 && targetGridPos.y < _configGame.Height;
        }

        private Vector3 GetWorldPoint()
        {
            return Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        public IArchitecture GetArchitecture()
        {
            return GameApp.Interface;
        }
    }
}
using System.Collections;
using Commands;
using QFramework;
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

        private void Awake()
        {
            _positionCell = GetComponent<Cell>();
            _configGame = ConfigGame.Instance;
        }

        public void StopMoveIE()
        {
            if (_moveIE != null)
            {
                StopCoroutine(_moveIE);
            }
        }

        private void OnMouseDown()
        {
            if (_configGame.IsDragged)
            {
                return;
            }

            _configGame.IsDragged = true;
        }

        private void OnMouseDrag()
        {
            if (!_configGame.IsDragged)
            {
                return;
            }

            var offset = GetOffset();

            _clampMagnitude = this.SendCommand(new GetClampMagnitudeVectorCommand(offset));
            var newPoint = _clampMagnitude * _configGame.CellSize * 0.9f + _positionCell.WorldPosition;

            this.transform.position = newPoint;
        }

        private Vector3 GetOffset()
        {
            var input = GetWorldPoint();
            return input - _positionCell.WorldPosition;
        }

        private void OnMouseUp()
        {
            this.transform.position = _positionCell.WorldPosition;

            if (!_configGame.IsDragged)
            {
                return;
            }

            var targetGridPos = GetTargetGridPos();

            if (IsPositionInGrid(targetGridPos))
            {
                StartCoroutine(
                    this.SendCommand(new InvertedAndMatchCommandIE(_positionCell.GridPosition, targetGridPos)));
            }

            _configGame.IsDragged = false;
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
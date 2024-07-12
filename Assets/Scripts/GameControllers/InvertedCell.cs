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
                StartCoroutine(InvertedAndMatch(_positionCell.GridPosition, targetGridPos));
            }

            _configGame.IsDragged = false;
        }
        
        private IEnumerator InvertedAndMatch(Utils.GridPos sourceGridPos, Utils.GridPos targetGridPos)
        {
            var isSpecial = this.SendCommand(new InvertedCellCommand(sourceGridPos, targetGridPos));

            yield return new WaitForSeconds(ConfigGame.Instance.FillTime);

            var isMatch = this.SendCommand(new MatchGridCommand());

            if (!isMatch && !isSpecial)
            {
                this.SendCommand(new InvertedCellCommand(targetGridPos, sourceGridPos));
            }

            yield return new WaitForSeconds(ConfigGame.Instance.MatchTime);
            this.SendCommand<ProcessingGridEventCommand>();

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
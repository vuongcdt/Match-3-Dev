using System.Collections;
using Commands;
using Events;
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
            if (!IsPositionInGrid(targetGridPos))
            {
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
                StartCoroutine(InvertedRainbow(sourceCell, targetCell, targetGridPos, isTargetRainbow));
            }
            else
            {
                StartCoroutine(InvertedAndMatch(sourceCell, targetCell));
            }

            _configGame.IsDragged = false;
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
                    this.SendCommand(new ClearObstacleCommand(cell.GridPosition.x, cell.GridPosition.y));
                }
            }

            yield return new WaitForSeconds(_configGame.MatchTime);

            this.SendCommand<ProcessingGridEventCommand>();
        }

        private IEnumerator InvertedAndMatch(Cell sourceCell, Cell targetCell)
        {
            var isInverted = this.SendCommand(new InvertedCellCommand(sourceCell, targetCell));

            yield return new WaitForSeconds(ConfigGame.Instance.FillTime);

            var isMatch = this.SendCommand(new MatchGridCommand());

            if (!isMatch || !isInverted)
            {
                this.SendCommand(new InvertedCellCommand(targetCell, sourceCell));
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
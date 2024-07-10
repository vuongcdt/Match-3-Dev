using System.Collections;
using Commands;
using QFramework;
using Queries;
using UnityEngine;

namespace GameControllers
{
    public class Cell : MonoBehaviour, IController
    {
        [SerializeField] private BoxCollider2D box2D;

        private CONSTANTS.CellType _type;
        private CONSTANTS.CellSpecialType _specialType;
        private IEnumerator _moveIE;
        [SerializeField] private Utils.GridPos _gridPos;
        [SerializeField]private Vector3 _worldPos;

        private Vector3 _clampMagnitude;
        private bool _isDragged;

        public Utils.GridPos GridPosition
        {
            get => _gridPos;
            set
            {
                _gridPos = value;
                _worldPos = GetWorldPos(value);
                Move(value, ConfigGame.Instance.FillTime);
            }
        }

        public Vector3 WorldPosition
        {
            get => _worldPos;
            set
            {
                SetWorldPosition(value);
                _worldPos = value;
                Move(value, ConfigGame.Instance.FillTime);
            }
        }


        public CONSTANTS.CellSpecialType SpecialType
        {
            get => _specialType;
            set => _specialType = value;
        }

        public CONSTANTS.CellType Type
        {
            get => _type;
            set
            {
                _type = value;
                SetAvatar(value);
            }
        }

        private void SetWorldPosition(Vector3 value)
        {
            var gridPos = GetGridPos(value);

            if (IsPositionInGrid(gridPos))
            {
                _gridPos = gridPos;
            }
        }

        public Cell Create(Utils.GridPos pos, Transform transformParent, float cellSize, CONSTANTS.CellType cellType)
        {
            var worldPos = GetWorldPos(pos);
            return Create(worldPos, transformParent, cellSize, cellType);
        }

        private Cell Create(Vector2 pos, Transform transformParent, float cellSize, CONSTANTS.CellType cellType)
        {
            var configGame = ConfigGame.Instance;
            var isCellNormal =
                cellType is not (CONSTANTS.CellType.Background or CONSTANTS.CellType.Obstacle
                    or CONSTANTS.CellType.None);

            this.transform.localScale = Vector2.one * cellSize;

            Cell cell = null;
            if (configGame.Pool.Count > 0)
            {
                cell = configGame.Pool.Pop();
                cell.GetComponent<BoxCollider2D>().enabled = true;
            }
            else
            {
                box2D.enabled = isCellNormal;
                cell = Instantiate(this, pos, Quaternion.identity, transformParent);
            }

            cell._worldPos = pos;
            cell._gridPos = GetGridPos(pos);
            cell.transform.position = pos;
            cell.Type = cellType;

            return cell;
        }

        private void SetAvatar(CONSTANTS.CellType type)
        {
            var avatar = this.GetComponentInChildren<SpriteRenderer>();
            var image = ConfigGame.Instance.Sprites[(int)type];
            if (image == null)
            {
                avatar.sprite = null;
                return;
            }

            avatar.sprite = image;
        }

        public void DeActive()
        {
            this.Type = CONSTANTS.CellType.None;
        }

        private void Move(Utils.GridPos pos, float time)
        {
            var worldPos = GetWorldPos(pos);
            Move(worldPos, time);
        }

        private void Move(Vector2 pos, float time)
        {
            if (_moveIE != null)
            {
                StopCoroutine(_moveIE);
            }

            _moveIE = MoveIE(pos, time);
            if (this.gameObject.activeSelf)
            {
                this.gameObject.name = $"{this._type.ToString()} {this._gridPos.x}_{this._gridPos.y}";
                StartCoroutine(_moveIE ?? MoveIE(pos, time));
            }
        }

        private IEnumerator MoveIE(Vector2 pos, float time)
        {
            time *= 20;
            for (float t = 0; t <= time; t += Time.deltaTime)
            {
                this.transform.position = Vector3.Lerp(this.transform.position, pos, t / time);
                yield return null;
            }

            _worldPos = pos;
            _gridPos = GetGridPos(pos);
            this.transform.position = pos;
        }

        private void OnMouseDown()
        {
            var configGame = ConfigGame.Instance;
            if (configGame.IsDragged)
            {
                return;
            }

            configGame.IsDragged = true;
        }

        private Vector3 GetWorldPoint()
        {
            return Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        private void OnMouseDrag()
        {
            var configGame = ConfigGame.Instance;
            if (!configGame.IsDragged)
            {
                return;
            }

            var offset = GetOffset();

            _clampMagnitude = this.SendCommand(new GetClampMagnitudeVectorCommand(offset));
            var newPoint = _clampMagnitude * configGame.CellSize * 0.9f + _worldPos;

            this.transform.position = newPoint;
        }

        private Vector3 GetOffset()
        {
            var input = GetWorldPoint();
            return input - _worldPos;
        }

        private void OnMouseUp()
        {
            var configGame = ConfigGame.Instance;
            if (!configGame.IsDragged)
            {
                return;
            }

            this.transform.position = _worldPos;
            var directionAxis = _clampMagnitude * configGame.Sensitivity;

            directionAxis.Normalize();
            var targetGridPos =
                new Utils.GridPos((int)(_gridPos.x + directionAxis.x), (int)(_gridPos.y + directionAxis.y));

            if (IsInverted(targetGridPos))
            {
                StartCoroutine(InvertedAndMatch(targetGridPos));
            }

            configGame.IsDragged = false;
        }

        private bool IsInverted(Utils.GridPos targetGridPos)
        {
            if (!IsPositionInGrid(targetGridPos))
            {
                return false;
            }

            var grid = this.SendQuery(new GetGridQuery());

            var currentCell = grid[_gridPos.x, _gridPos.y];
            var targetCell = grid[targetGridPos.x, targetGridPos.y];

            var isObstacle = currentCell.Type == CONSTANTS.CellType.Obstacle ||
                             targetCell.Type == CONSTANTS.CellType.Obstacle;
            var isEmpty = currentCell.Type == CONSTANTS.CellType.None || targetCell.Type == CONSTANTS.CellType.None;

            return !isObstacle && !isEmpty;
        }

        private IEnumerator InvertedAndMatch(Utils.GridPos targetGridPos)
        {
            var sourceGridPos = new Utils.GridPos(_gridPos.x, _gridPos.y);
            this.SendCommand(new InvertedCellCommand(sourceGridPos, targetGridPos));

            yield return new WaitForSeconds(ConfigGame.Instance.FillTime * 2);

            var isMatch = this.SendCommand(new MatchGridCommand());
            if (!isMatch)
            {
                this.SendCommand(new InvertedCellCommand(targetGridPos, sourceGridPos));
                yield return new WaitForSeconds(ConfigGame.Instance.FillTime * 2);
            }
        }

        private bool IsPositionInGrid(Utils.GridPos targetGridPos)
        {
            var configGame = ConfigGame.Instance;

            return targetGridPos.x >= 0 && targetGridPos.x < configGame.Width &&
                   targetGridPos.y >= 0 && targetGridPos.y < configGame.Height;
        }

        public IArchitecture GetArchitecture()
        {
            return GameApp.Interface;
        }

        private Utils.GridPos GetGridPos(Vector3 pos)
        {
            var configGame = ConfigGame.Instance;
            return Utils.GetGridPos(pos.x, pos.y, configGame.Width, configGame.Height, configGame.CellSize);
        }

        private Vector3 GetWorldPos(Utils.GridPos pos)
        {
            var configGame = ConfigGame.Instance;
            return Utils.GetWorldPosition(pos.x, pos.y, configGame.Width, configGame.Height, configGame.CellSize);
        }
    }
}